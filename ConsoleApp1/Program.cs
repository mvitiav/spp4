using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ConsoleApp1
{
    class Program
    {

        static void Main(string[] args)
        {
            //ItestGen itg = new TestGen();

            //string src = File.ReadAllText(@"G:\SPP\4\testSources\forUnitTest5.txt");
            //var temp = itg.generate(src);
            //foreach (var testiinfo in temp)
            //{
            //    Console.WriteLine(testiinfo.test);
            //    testiinfo.test.Contains("public void method1Test()");
            //    testiinfo.test.Contains("public void method2Test()");
            //}

            var setup = new Setup(2, 3, 2, @"..\outputClaZZEZ\");
            setup.inputPath.Add(@"G:\SPP\4\testSources\TestFile1.cs");
            setup.inputPath.Add(@"G:\SPP\4\testSources\TestFile2.cs");
            setup.inputPath.Add(@"G:\SPP\5\ConsoleApp1\DependencyContainer.cs");
            setup.inputPath.Add(@"G:\SPP\5\ConsoleApp1\DependencyRecord.cs");
            setup.inputPath.Add(@"G:\SPP\5\ConsoleApp1\DependencyConfiguration.cs");


            new LibraryUser(setup).gen().Wait();
            Console.WriteLine("Generation completed");


        }




    }
}
