using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CBW.Core;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace CBW.Core.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void StringParserTest()
        {
            //StringParser parser = new StringParser();
            List<String> result = StringParser.Parse("This is a parser");
            int k = result.IndexOf("is");
            Assert.IsTrue(result.IndexOf("is") == 0);
            Assert.IsTrue(result.IndexOf("parser") == 1);
        }
    }
}
