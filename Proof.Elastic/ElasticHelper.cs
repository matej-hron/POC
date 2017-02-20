using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Elasticsearch.Net;
using Nest;

namespace Proof.Elastic
{
    public class ElasticHelper
    {
        public static ElasticClient GetClient()
        {
            var nodes = new[]
            {
                new Uri("http://localhost:9200"),
            };

            var pool = new StaticConnectionPool(nodes);

            var settings = new ConnectionSettings(pool).
                BasicAuthentication("elastic", "matess")
                .DisableDirectStreaming()
                .OnRequestCompleted(details =>
                {
                    Debug.WriteLine("### ES REQEUST ###");
                    if (details.RequestBodyInBytes != null) Debug.WriteLine(Encoding.UTF8.GetString(details.RequestBodyInBytes));
                    Debug.WriteLine("### ES RESPONSE ###");
                    if (details.ResponseBodyInBytes != null) Debug.WriteLine(Encoding.UTF8.GetString(details.ResponseBodyInBytes));
                })
                 .PrettyJson();

            return new ElasticClient(settings);
        }


        public static void TestIndex()
        {
            var individual = new MatchingRequestIndividual
            {
                FirstName = "Josef",
                LastName = "Czanko",
                Identifications = new List<Identification> {new Identification {IdentificationType = 1, Number = "XYZ-456"}, new Identification { IdentificationType = 2, Number = "9874654" } } ,
                
            };
            var client = GetClient();
            var response = client.Index(individual, i => i.Index("individualindex"));
        }

        public static void Pump(int maxCount = 0)
        {
            var transferedRecords = 0;

            var reader = new MatchingRequestRepository();
            var pages = reader.GetAll();
            var client = GetClient();

            foreach (var page in pages)
            {
                transferedRecords += WriteSubjects(page, client, 1, "matchingindex", "individual");
                transferedRecords += WriteSubjects(page, client, 2, "matchingindex", "company");

                if(maxCount > 0 && transferedRecords > maxCount)
                    continue;
            }

            Console.WriteLine("Pump finished");
        }

        

        public static void Map()
        {
            var client = GetClient();

            var res = client.CreateIndex("matchingindex", d => d.
                Mappings(ms => ms
                    .Map<MatchingRequest>(m => m.AutoMap())));


        }

        private static int WriteSubjects(List<MatchingRequest> page, ElasticClient client, int subjectType, string index, string type)
        {
            var records = page.Where(x => x.SubjectType == subjectType).ToList();

            if (records.Any())
            {
                var ri = client.IndexMany(records, index, type);
            }

            return records.Count;
        }

        public static ISearchResponse<MatchingRequest> Search()
        {
            var client = GetClient();

            var res = SearchResponse2(client);

            foreach (var hit in res.Hits.OrderByDescending(h => h.Score))
            {
                Console.WriteLine($"{hit.Source.NationalId} {hit.Source.FullName} {hit.Source.DateOfBirth} {hit.Source.Gender} {hit.Score}");
            }

            return res;
        }

        private static ISearchResponse<MatchingRequest> SearchResponse(ElasticClient client)
        {
            var res = client.Search<MatchingRequest>(s => s
                .From(0)
                .Size(10)
                .Query(q =>
                    q.Term(t => t.NationalId, "1992-02-12")
                    || q.Match(mq => mq.Field(f => f.FullName).Query("Aleksandrs Kinduris"))
                )
                );
            return res;
        }

        private static ISearchResponse<MatchingRequest> SearchResponse2(ElasticClient client)
        {
            var res = client.Search<MatchingRequest>(s => s
                .Query(q => q.Match(p => p.Field(f => f.FullName).Query("Ieva Paškeviča")) || q.Term(p => p.NationalId, "300774-13063"))
            );


            return res;
        }
    }
}