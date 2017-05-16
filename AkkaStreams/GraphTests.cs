using System;
using System.Linq;
using System.Threading.Tasks;
using Akka;
using Akka.IO;
using Akka.Streams;
using Akka.Streams.Dsl;
using Akka.TestKit.NUnit3;
using NUnit.Framework;
using static AkkaStreams.Helper;
using Tcp = Akka.Streams.Dsl.Tcp;

namespace AkkaStreams
{
    [TestFixture]
    public class GraphTests : TestKit
    {
        [Test]
        public void ShouldCreateSimpleClosedGraph()
        {
            var sink = Sink.Aggregate<int, int>(0, (acc, i) => acc + i);

            var g = RunnableGraph.FromGraph(GraphDsl.Create(sink, (b, s) =>
            {
                var source = Source.From(Enumerable.Repeat(1, 100)).MapMaterializedValue<Task<int>>(_ => null);

                var broadcast = b.Add(new Broadcast<int>(2));
                var merge = b.Add(new Merge<int>(2));

                var f1 = Flow.Create<int>().Select(x => x + 1);
                var f2 = Flow.Create<int>().Select(x => x + 1);
                var f3 = Flow.Create<int>().Select(x => x + 1);
                var f4 = Flow.Create<int>().Select(x => x + 1);

                b.From(source).Via(f1).Via(broadcast).Via(f2).Via(merge).Via(f3).To(s);
                b.From(broadcast.Out(1)).Via(f4).To(merge);

                return ClosedShape.Instance;
            }));

            var res = WithMaterializer(g.Run).Result;
            Assert.That(res, Is.EqualTo(800));
        }

        [Test]
        public void TheSameFlowIsMaterializedAsTwoConnections()
        {
            var topHeadSink = Sink.Last<int>();
            var bottomHeadSink = Sink.First<int>();
            var sharedDoubler = Flow.Create<int>().Select(x => x * 2);

            var g = RunnableGraph.FromGraph(GraphDsl.Create(topHeadSink, bottomHeadSink, Keep.Both,
                (builder, topHs, bottomHs) =>
                {
                    var broadcast = builder.Add(new Broadcast<int>(2));
                    var source = Source.From(Enumerable.Range(1, 10)).MapMaterializedValue<Tuple<Task<int>, Task<int>>>(_ => null);

                    builder.From(source).To(broadcast.In);

                    builder.From(broadcast.Out(0)).Via(sharedDoubler).To(topHs.Inlet);
                    builder.From(broadcast.Out(1)).Via(sharedDoubler).To(bottomHs.Inlet);

                    return ClosedShape.Instance;
                }));

            var (a, b) = WithMaterializer(g.Run);
            Assert.That(a.Result, Is.EqualTo(20));
            Assert.That(b.Result, Is.EqualTo(2));
        }

        [Test]
        public void PartialGraph()
        {
            var pickMaxOfThree = GraphDsl.Create(b =>
            {
                var zip1 = b.Add(ZipWith.Apply<int, int, int>(Math.Max));
                var zip2 = b.Add(ZipWith.Apply<int, int, int>(Math.Max));
                b.From(zip1.Out).To(zip2.In0);

                return new UniformFanInShape<int, int>(zip2.Out, zip1.In0, zip1.In1, zip2.In1);
            });

            var resultSink = Sink.First<int>();

            var g = RunnableGraph.FromGraph(GraphDsl.Create(resultSink, (b, sink) =>
            {
                var pm3 = b.Add(pickMaxOfThree);
                var s1 = Source.Single(1).MapMaterializedValue<Task<int>>(_ => null);
                var s2 = Source.Single(2).MapMaterializedValue<Task<int>>(_ => null);
                var s3 = Source.Single(3).MapMaterializedValue<Task<int>>(_ => null);

                b.From(s1).To(pm3.In(0));
                b.From(s2).To(pm3.In(1));
                b.From(s3).To(pm3.In(2));

                b.From(pm3.Out).To(sink);

                return ClosedShape.Instance;
            }));

            var max = WithMaterializer(g.Run);
            Assert.That(max.Result, Is.EqualTo(3));
        }

        [Test]
        public void CombiningSourcesWithSimplifiedApi()
        {
            var sourceOne = Source.Single(1);
            var sourceTwo = Source.Single(2);

            var merged = Source.Combine(sourceOne, sourceTwo, i => new Merge<int, int>(i));

            var mergedResult = WithMaterializer(m => merged.RunWith(Sink.Aggregate<int, int>(0, (agg, i) => agg + i), m));
            Assert.That(mergedResult.Result, Is.EqualTo(3));
        }

        [Test]
        public void CombiningSinksWithSimplifiedApi()
        {
            var actor = CreateTestProbe();

            var sendRemotely = Sink.ActorRef<int>(actor.Ref, "Done");
            var localProcessing = Sink.Aggregate<int, int>(0, (acc, i) => acc + i)
                .MapMaterializedValue(_ => NotUsed.Instance);

            var sink = Sink.Combine(i => new Broadcast<int>(i), sendRemotely, localProcessing);

            WithMaterializer(m =>
            {
                Source.From(new[] {0, 1, 2}).RunWith(sink, m);
                var received = actor.ReceiveN(3);
                Assert.That(received, Is.EquivalentTo(new[] { 0, 1, 2 }));
            });

        }

        [Test]
        public void MaterializedValues()
        {
            // Materializes to TaskCompletionSource<int>      (red)
            var source = Source.Maybe<int>();

            // Materializes to NotUsed                        (black)
            var flow = Flow.Create<int>().Take(100);

            // Materializes to TaskCompletionSource<int>      (red)
            var nestedSource = source.ViaMaterialized(flow, Keep.Left).Named("nestedSource");
            
            // Materializes to NotUsed                      (orange)  
            var flow1 = Flow.Create<int>().Select(x => ByteString.FromString(x.ToString()));

            // Materializes to Task<OutgoingConnection>     (yellow)  
            var flow2 = Sys.TcpStream().OutgoingConnection("localhost", 8080);

            // Materializes to Task<OutgoingConnection>     (yellow)  
            var nestedFlow = flow1.ViaMaterialized(flow2, Keep.Right).Named("nestedFlow");

            // Materializes to Task<String>                                 (green)
            var sink = Sink.Aggregate<ByteString, string>("", (agg, s) => agg + s.DecodeString());

            // Materializes to (Task<OutgoingConnection>, Task<String>)     (blue)
            var nestedSink = nestedFlow.ToMaterialized(sink, Keep.Both);

            // Materializes to Task<MyClass>        (purple)
            var runnableGraph = nestedSource.ToMaterialized(nestedSink, (completion, rest) =>
            {
                var connectionTask = rest.Item1;
                return connectionTask.ContinueWith(task => new MyClass(completion, task.Result));
            });
        }
        [Test]
        public void TestAsync()
        {
            Console.WriteLine("Start");
            WithMaterializer(materializer =>
            {
                Console.WriteLine(
                Source.From(Enumerable.Range(1, 100))
                    .Select(i =>
                    {
                        Console.WriteLine($"A: {i}");
                        return i;
                    })
                    .Async()
                    .Select(i =>
                    {
                        Console.WriteLine($"B: {i}");
                        return i;
                    })
                    .Async()
                    .Select(i =>
                    {
                        Console.WriteLine($"C: {i}");
                        return i;
                    })
                    .Async()
                    .RunWith(Sink.Aggregate<int, int>(0, (acc, i) => acc + i), materializer)
                    .Result);

                Task.Delay(100).Wait();
            });
        }

    }

    public sealed class MyClass
    {
        private readonly TaskCompletionSource<int> _completion;
        private readonly Tcp.OutgoingConnection _connection;

        public MyClass(TaskCompletionSource<int> completion, Tcp.OutgoingConnection connection)
        {
            _completion = completion;
            _connection = connection;
        }

        public void Close() => _completion.SetResult(1);
    }



}