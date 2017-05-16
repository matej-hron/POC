using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using Akka.IO;
using Akka.Streams;
using Akka.Streams.Dsl;
using Akka.Streams.Implementation;
using Akka.Streams.IO;

namespace AkkaStreams
{
    class Program
    {
        static void Main(string[] args)
        {
            //Test1();
            //Test2();
            //Test3();
            //Test4();
            //TestGraph();
            //Test5();
            //TestDifferentWaysToCreateSource();
            //TestExplicitWiringSourceFlowAndSink();
            //TestFussion();
            //TestTick();
            TestAsync();

            Console.WriteLine("Finished");
            Console.ReadLine();
        }

        private static void TestAsync()
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

        private static void TestTick()
        {
            var source = Source.Tick(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), 1);
            WithMaterializer(m => source.RunForeach(Console.WriteLine, m).Wait());


        }

        private static void TestFussion()
        {
            var flow = Flow.Create<int>().Select(x => x * 2).Where(x => x > 500);
            var fused = Fusing.Aggressive(flow);
            var s = Source.From(Enumerable.Range(0, int.MaxValue)).Via(fused).Take(1000);
            WithMaterializer(m => s.RunForeach(x => Console.WriteLine(x), m)).Wait();
        }

        private static void TestExplicitWiringSourceFlowAndSink()
        {
            var pipe = Source.From(Enumerable.Range(1, 6))
                .Via(Flow.Create<int>().Select(x => x * 2))
                .ToMaterialized(Sink.ForEach<int>(Console.WriteLine), Keep.Right);

            WithMaterializer(m => pipe.Run(m)).Wait();
        }

        private static void TestDifferentWaysToCreateSource()
        {

            var s0 = Source.From(Enumerable.Range(1, 10));
            WithMaterializer(m => s0.RunForeach(Console.WriteLine, m).Wait());
            var s1 = Source.FromTask(Task.FromResult(1));
            WithMaterializer(m => s1.RunForeach(Console.WriteLine, m).Wait());
            var s2 = Source.Single(2);
            WithMaterializer(m => s2.RunForeach(Console.WriteLine, m).Wait());
            var s3 = Source.Empty<int>();
            WithMaterializer(m => s3.RunForeach(Console.WriteLine, m).Wait());

            WithMaterializer(m => Sink.First<int>().RunWith(s0, m));

            var sink = Sink.Aggregate<int, int>(0, (sum, i) => sum + i);
            var res = WithMaterializer(m => s0.ToMaterialized(sink, Keep.Right).Run(m)).Result;
            Console.WriteLine(res);

            var sink2 = Sink.First<int>();
            Console.WriteLine(WithMaterializer(m => s0.ToMaterialized(sink2, Keep.Right).Run(m)).Result);
        }

        private static void Test5()
        {
            var source = Source.From(Enumerable.Range(1, 10));
            var sink = Sink.Aggregate<int, int>(0, (agg, i) => agg + i);
            
            var runnable = source.ToMaterialized(sink, Keep.Right);

            var x = WithMaterializer(runnable.Run).Result;
            Console.WriteLine(x);
        }

        private static void TestGraph()
        {
            WithMaterializer(m =>
            {
                var graph = BroadCastTest.BuildGraph();
                graph.Run(m);
                Task.Delay(1000).Wait();
            });
        }

        private static void Test1()
        {
            WithMaterializer(m =>
            {
                var source = GetSource;
                source.RunForeach(Console.WriteLine, m);
                Task.Delay(500).Wait();
            });
        }

        private static void Test2()
        {
            WithMaterializer(materializer =>
            {
                var result =
                    Factorials
                        .Select(num => ByteString.FromString($"{num}\n"))
                        .RunWith(FileIO.ToFile(new FileInfo(@"c:\temp\factorials.txt")), materializer);

                Console.WriteLine(result.Result);
            });
        }

        private static void Test3()
        {
            WithMaterializer(m =>
            {
                var result = Factorials.Select(_ => _.ToString()).RunWith(LineSink(), m);
                Console.WriteLine(result.Result);
            });
        }

        private static async void Test4()
        {
            WithMaterializer(m =>
            {
                var x = Factorials
                    .ZipWith(GetSource, (num, idx) => $"{idx}! = {num}")
                    .Throttle(1, TimeSpan.FromSeconds(1), 1, ThrottleMode.Shaping)
                    .RunForeach(Console.WriteLine, m);

                x.Wait();
            });
        }

        private static Source<int, NotUsed> GetSource => Source.From(Enumerable.Range(1, 100));

        private static Source<BigInteger, NotUsed> Factorials => GetSource.Scan(new BigInteger(1), (acc, next) => acc * next);


        private static Sink<string, Task<IOResult>> LineSink()
        {
            return Flow.Create<string>()
                .Select(s => ByteString.FromString($"{s}\n"))
                .ToMaterialized(FileIO.ToFile(new FileInfo(@"c:\temp\factorials2.txt")), Keep.Right);
        }


        private static T WithMaterializer<T>(Func<ActorMaterializer, T> call)
        {
            using (var system = ActorSystem.Create("system"))
            using (var materializer = system.Materializer())
            {
                return call(materializer);
            }

        }

        private static void WithMaterializer(Action<ActorMaterializer> call)
        {
            WithMaterializer<object>(m =>
            {
                call(m);
                return null;
            });
        }
    }
}
