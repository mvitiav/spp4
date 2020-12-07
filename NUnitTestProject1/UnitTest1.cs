using ConsoleApp1;
using NUnit.Framework;
using System.IO;

namespace NUnitTestProject1
{
    [TestFixture]
    public class Tests
    {
        ItestGen itg;
        [SetUp]
        public void Setup()
        {
            itg = new TestGen();
        }

        [Test]
        public void TestGenTestMethods()
        {
            string src = File.ReadAllText(@"G:\SPP\4\testSources\forUnitTest.txt");
            var temp = itg.generate(src);

            Assert.AreEqual(1, temp.Length);
            Assert.IsTrue(temp[0].test.Contains("public void method1Test()"));
            Assert.IsTrue(temp[0].test.Contains("public void method2Test()"));   
            Assert.IsTrue(temp[0].test.Contains("[Test]"));
        }

        [Test]
        public void TestGenTestClasses()
        {
            string src = File.ReadAllText(@"G:\SPP\4\testSources\forUnitTest2.txt");
            var temp = itg.generate(src);
            Assert.AreEqual(2, temp.Length);
        }

        [Test]
        public void TestGenTestNamespaces()
        {
            string src = File.ReadAllText(@"G:\SPP\4\testSources\forUnitTest3.txt");
            var temp = itg.generate(src);
            Assert.AreEqual(2, temp.Length);
            Assert.AreEqual("namespace1.myClass1", temp[0].fileName);
            Assert.AreEqual("namespace1.ns2.myClass2", temp[1].fileName);

        }

        [Test]
        public void TestGenTestMocks()
        {
            string src = File.ReadAllText(@"G:\SPP\4\testSources\forUnitTest4.txt");
            var temp = itg.generate(src);
            Assert.AreEqual(1, temp.Length);
            Assert.IsTrue(temp[0].test.Contains("public void Setup()"));
            Assert.IsTrue(temp[0].test.Contains("_myClass1Instance = new myClass1();"));
            Assert.IsTrue(temp[0].test.Contains("_method1Instance.method1(_iDependency.Object);"));

        }

        [Test]
        public void ReturnTest()
        {
            string src = File.ReadAllText(@"G:\SPP\4\testSources\forUnitTest5.txt");
            var temp = itg.generate(src);
            Assert.AreEqual(1, temp.Length);
            Assert.IsTrue(temp[0].test.Contains("public void Setup()"));
            Assert.IsTrue(temp[0].test.Contains("Assert.That(actual, Is.EqualTo(expected));"));
            Assert.IsTrue(temp[0].test.Contains("string expected = default;"));

        }


    }
}