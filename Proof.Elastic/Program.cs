using System;
using Nest;

namespace Proof.Elastic
{
    class Program
    {
        public static void Main()
        {
            //DataHelper.CopyData();
            Search();
            Console.WriteLine("Proof finished");
            Console.ReadLine();
        }

        private void MapAndPump()
        {
            ElasticHelper.Map();
            ElasticHelper.Pump(1000000);

        }

        private static void Search()
        {
            var res = ElasticHelper.Search();

          
        }
    }
}
