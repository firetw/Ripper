using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    class LinqTester
    {

        public static void Test()
        {
            List<String> charMap = new List<String>();
            for (int i = 65; i < 68; i++)
            {
                Console.WriteLine((char)i);
                charMap.Add(((char)i).ToString());
            }
            Random random = new Random();
            List<Item> pList = new List<Item>();
            //List<List<string>> list = new List<List<string>>();
            //for (int i = 0; i < 20; i++)
            //{
            //    List<string> rows = new List<string>();
            //    for (int j = 0; j < random.Next(30, 30); j++)
            //    {
            //        rows.Add(charMap[random.Next(1, 3)]);
            //    }
            //    list.Add(rows);
            //}
            for (int i = 0; i < 100; i++)
            {
                pList.Add(new Item
                {
                    ItemName = "I" + charMap[random.Next(1, 3)],
                    Material = "M" + charMap[random.Next(1, 3)],
                    ItemCode = "C" + charMap[random.Next(1, 3)],
                    Type = "T" + charMap[random.Next(1, 3)],
                    Amount = (random.NextDouble() * 100)
                });
            }
            var _list = from d in pList group d by new { d.ItemName, d.Material, d.ItemCode, d.Type } into g select new { g.Key, TotalAmount = g.Sum(p => p.Amount), g };
            Console.WriteLine(_list.Count());
            int itemCount = _list.Count();

            var groupResult = pList.GroupBy(d => new { d.ItemName, d.Material, d.ItemCode, d.Type });
            foreach (var item in groupResult)
            {

                int _count = item.Count();
                double total = item.Sum(i => i.Amount);

                Console.WriteLine(total + " count:" + _count);




            }





        }
    }
    //d.ItemName, d.Material, d.ItemCode, d.Type
    class Item
    {
        public string ItemName { get; set; }
        public string Material { get; set; }
        public string ItemCode { get; set; }

        public string Type { get; set; }


        public double Amount { get; set; }
    }
}
