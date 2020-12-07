using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
   public class TestInfo
    {
       public string fileName { get; }
       public string test { get; }

        public TestInfo(string fileName, string test)
        {
            this.fileName = fileName;
            this.test = test;
        }
    }
}
