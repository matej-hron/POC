using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brutal.Dev.StrongNameSigner;

namespace StrongNamer
{
    class Program
    {
        static void Main(string[] args)
        {
            var newInfo = SigningHelper.SignAssembly(@"C:\MyAssembly.dll");
        }
    }
}
