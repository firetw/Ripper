using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WeiBoGrab.Core;
using WeiBoGrab.Verify.SamSung;

namespace FetionAward.Test
{
    [TestClass]
    public class SanSungTest
    {
        [TestMethod]
        public void TestSplit()
        {
            //姓名,手机号,身份证号
            string line = "姓名,手机号,身份证号";
            string line1 = "姓名 , 手机号  ,  身份证号";
            string line2 = "姓名  手机号  ,  身份证号";

            string[] array = Utils.Split(line);
            string[] array1 = Utils.Split(line1);
            string[] array2 = Utils.Split(line2);

            Assert.IsTrue(array != null && array.Length == 3);
            Assert.IsTrue(array1 != null && array1.Length == 3);
            Assert.IsTrue(array2 != null && array2.Length == 3);



        }
  

        [TestMethod]
        public void CreateRegisterContext()
        {
            string line = "姓名,手机号,身份证号";
            string line1 = "王明 , 手机号  ,  身份证号";
            string line2 = "王 宁  手机号  ,  身份证号";
            string line3 = "欧阳名  手机号  ,  身份证号";

            RegisterContext context = new RegisterContext(line);

            RegisterContext context1 = new RegisterContext(line1);
            RegisterContext context2 = new RegisterContext(line2);
            RegisterContext context3 = new RegisterContext(line3);
        }

    }
}
