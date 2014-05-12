using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Ripper.Model
{
    public class LoginResult
    {
        public CookieContainer CookieContainer { get; set; }

        /// <summary>
        /// 为0表示正常，其他值表示登陆失败
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 异常消息
        /// </summary>
        public string ErrorMsg { get; set; }


        public Exception Exception { get; set; }


        public string UserName { get; set; }
        public string Pwd { get; set; }



        public LoginResult(string userName, string pwd)
        {
            this.UserName = userName;
            this.Pwd = pwd;
        }
    }
}
