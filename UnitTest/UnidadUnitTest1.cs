using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Controller;

namespace UnitTest
{
    [TestClass]
    public class UnidadUnitTest1
    {
        [TestMethod]
        public void Unidad()
        {

            ReadFileStaadPRO std = new ReadFileStaadPRO();
            std.path = @"C:\test.txt";
            std.ReadStaadPro();
            Console.WriteLine("IS VALID " + std.error);
            Console.WriteLine("RAISE " + std.raise);
        }
    }
}
