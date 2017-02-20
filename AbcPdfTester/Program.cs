using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AbcPdfTester
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Doc theDoc = new Doc();
            theDoc.AddImageUrl("http://www.google.com/");
            theDoc.Save(Server.MapPath("htmlimport.pdf"));
            theDoc.Clear();
        }
    }
}
