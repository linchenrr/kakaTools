using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProtocol
{
    class Program
    {
        static TestProtocol tester;
        static void Main(string[] args)
        {
            tester = new TestProtocol();
            tester.start();
        }
    }
}
