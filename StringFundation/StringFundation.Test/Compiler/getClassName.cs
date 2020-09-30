using System.IO;
using NUnit.Framework;
using Unikon.serialize;

namespace Tests
{
    public class getClassName
    {
        [Test]
        public void Test1()
        {
            var path = Directory.GetCurrentDirectory();
          
            Assert.IsTrue("Unikon.serialize.SimpleCompiler"== SimpleCompiler.getFullClassName(path + "/../../../classnametest.txt"));  

        }
        
    }
}