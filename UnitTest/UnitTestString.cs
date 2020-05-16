using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    [TestClass]
    public class UnitTestString
    {
        [TestMethod]
        public void TestString()
        {
            // triple A
            // declare
            string cad = "$ LOG";
            // asignation
            int result = 0;
            // Assert
            Assert.AreEqual(result, cad.IndexOf("$"));
        }
    }
}
