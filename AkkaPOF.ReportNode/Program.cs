using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using AkkaPOF.Actors;

namespace AkkaPOF.ReportNode
{
    class Program
    {
        public static ActorSystem System;

        static void Main(string[] args)
        {
            System = ActorSystem.Create("cb5");
            System.WhenTerminated.Wait();
        }
    }
}
