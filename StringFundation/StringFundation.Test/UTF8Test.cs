using System.IO;
using System.Text;
using NUnit.Framework;
using StringFundation;

namespace Tests
{
    public class UTF8Test
    {
        [SetUp]
        public void Setup()
        {
            
        }
        public static bool IsStringValidUTF8(ref string source)
        {
            return UTF8.IsStringValidUTF8(source);
        }
        [Test]
        public void Test1()
        {
            var path = Directory.GetCurrentDirectory();
            string s = File.ReadAllText(path + "/../../../utf8.txt");
            string s2 =  File.ReadAllText(path + "/../../../assis.txt");
            Assert.IsTrue(IsStringValidUTF8(ref s));
            Assert.IsFalse(IsStringValidUTF8(ref s2));
            Assert.Pass();
        }
    }
}