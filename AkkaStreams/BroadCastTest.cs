using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Akka;
using Akka.IO;
using Akka.Streams;
using Akka.Streams.Dsl;
using Akka.Streams.IO;

namespace AkkaStreams
{
    public static class BroadCastTest
    {
        public static IRunnableGraph<NotUsed> BuildGraph()
        {

            Sink<User, NotUsed> writeAuthors = WriteAuthors2;
            Sink<HashTagEntity, NotUsed> writeHashTags = WriteHashTags2;

            Source<Tweet, NotUsed> tweetSource = TweetSource;

            return RunnableGraph.FromGraph(GraphDsl.Create(b =>
            {
                var broadcast = b.Add(new Broadcast<Tweet>(2));
                b.From(tweetSource).To(broadcast.In);

                b.From(broadcast.Out(0))
                    .Via(Flow.Create<Tweet>().Select(tweet => tweet.CreatedBy))
                    .To(writeAuthors);

                b.From(broadcast.Out(1))
                    .Via(Flow.Create<Tweet>().SelectMany(tweet => tweet.HashTags))
                    .To(writeHashTags);

                return ClosedShape.Instance;
            }));

        }

        private static Sink<User, NotUsed> WriteAuthors =>
            Flow.Create<User, NotUsed>()
                .Select(s => ByteString.FromString($"{s}\n"))
                .ToMaterialized(FileIO.ToFile(new FileInfo(@"c:\temp\authors.txt")), Keep.Left);

        private static Sink<T, Task> ConsoleSink<T>() where T: class =>
            Sink.ForEach<T>(Console.WriteLine);

        private static Sink<User, NotUsed> WriteAuthors2 =>
            Flow.Create<User, NotUsed>()
                .ToMaterialized(ConsoleSink<User>(), Keep.Left);

        private static Sink<HashTagEntity, NotUsed> WriteHashTags =>
            Flow.Create<HashTagEntity, NotUsed>()
                .Select(s => ByteString.FromString($"{s}\n"))
                .ToMaterialized(FileIO.ToFile(new FileInfo(@"c:\temp\hashtags.txt")), Keep.Left);

        private static Sink<HashTagEntity, NotUsed> WriteHashTags2 =>
            Flow.Create<HashTagEntity, NotUsed>()
                .ToMaterialized(ConsoleSink<HashTagEntity>(), Keep.Left);


        private static Source<Tweet, NotUsed> TweetSource =>
            Source.From(
                new[]
                {
                    new Tweet {CreatedBy = new User {Name = "Matej Hron"}, HashTags = new []
                    {
                        new HashTagEntity{Value = "#hashtag1"},
                        new HashTagEntity{Value = "#hashtag2"},
                        new HashTagEntity{Value = "#hashtag3"},
                    }},
                    new Tweet {CreatedBy = new User {Name = "Josef Czanko"}, HashTags = new []
                    {
                        new HashTagEntity{Value = "#hashtag4"},
                        new HashTagEntity{Value = "#hashtag5"},
                        new HashTagEntity{Value = "#hashtag6"},
                    }}
                }
            );

    }



    public class User
    {
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class HashTagEntity
    {
        public string Value { get; set; }

        public override string ToString()
        {
            return Value;
        }
    }

    public class Tweet {
        public User CreatedBy { get; set; }

        public IReadOnlyCollection<HashTagEntity> HashTags { get; set; }
    }
}

