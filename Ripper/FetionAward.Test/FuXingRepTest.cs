using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WeiBoGrab.Verify.SamSung;
using System.Text;

namespace FetionAward.Test
{
    [TestClass]
    public class FuXingRepTest
    {
        [TestMethod]
        public void TestLoad()
        {
            Assert.IsTrue(FuXingRep.Map.Contains("公户"));
        }

        [TestMethod]
        public void TestEncoding()
        {
            Encoding encoding = Encoding.GetEncoding("utf-8");
            Assert.IsNotNull(encoding);
            Assert.IsTrue(encoding == Encoding.UTF8);
        }

    }
}
