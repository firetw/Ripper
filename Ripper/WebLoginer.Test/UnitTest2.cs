using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebLoginer.Test
{
    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public void TestMethod1()
        {

            List<A> list = new List<A>();
            
        }
    }

    public class A
    {

        public string GroupName { get; set; }

        public A()
        {
            SubCollections = new BCollections();
        }

        public BCollections SubCollections { get; set; }

        public IEnumerable<B> SumSubCollections()
        {
            return SubCollections.SumByID();

        }
    }

    public class B
    {

        public string ID { get; set; }

        public double Money { get; set; }



    }

    public class BCollections : List<B>
    {


        public IEnumerable<B> SumByID()
        {

            //var query=from item in  
            var query = from item in this
                        group item by item.ID into g
                        select new B
                        {
                            ID = g.Key,
                            Money = g.Sum(item => item.Money)
                        };




            return query.ToList();

        }


    }
}
