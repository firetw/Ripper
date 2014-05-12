using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FetionLoginer
{
    public interface IVerify
    {
        string RecognizeByUrl(string url);

        string RecognizeByBytes(byte[] picContent);
    }
}
