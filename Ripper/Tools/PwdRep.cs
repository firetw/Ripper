using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools
{
    public class PwdRep : List<string>
    {
        private static readonly PwdRep _instance = new PwdRep();

        public static PwdRep Instance
        {
            get { return PwdRep._instance; }
        }


        int[] Nums = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        char[] LowChars = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
        char[] UpperChars = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
        //char[] SpecialChars = { '!', '@', '#', '%', '^' };
        char[] SpecialChars = { '!', '!', '_' };//, '@', '#', '%', '^' 

        Random _random = new Random();
        public PwdRep()
            : base()
        {
            Initilize();

            int numLen = Nums.Length;
            int lowLen = LowChars.Length;
            int upperLen = UpperChars.Length;
            int specLen = SpecialChars.Length;
            for (int i = 0; i < 100; i++)
            {

                string pwd = Nums[_random.Next(0, numLen)].ToString() + LowChars[_random.Next(0, lowLen)].ToString() +
                    UpperChars[_random.Next(0, upperLen)].ToString() + UpperChars[_random.Next(0, upperLen)].ToString() +
                     SpecialChars[_random.Next(0, specLen)] + LowChars[_random.Next(0, lowLen)].ToString() + LowChars[_random.Next(0, lowLen)].ToString() + UpperChars[_random.Next(0, upperLen)].ToString();
                Add(pwd);
            }
        }


        public string GetPwd()
        {
            return this[_random.Next(0, this.Count)];
        }

        public void Initilize()
        {
            string pwd = "1qaz!QAZ,1Hblsqt!,2wsx@WSX,6tfc^TFC,9ijn^IJN";
            string[] arrays = pwd.Split(',');

            foreach (var item in arrays)
            {
                Add(item);
            }

        }
    }
}
