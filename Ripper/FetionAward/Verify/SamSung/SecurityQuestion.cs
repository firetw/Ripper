using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;


namespace WeiBoGrab.Verify.SamSung
{
    public class SecurityQuestionManager
    {
        static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static readonly SecurityQuestionManager _instance = new SecurityQuestionManager();
        public static SecurityQuestionManager Instance
        {
            get { return _instance; }
        }

        List<string> _questions = new List<string>(20);

        private SecurityQuestionManager()
        {

        }

        IDictionary<string, string> _map = new Dictionary<string, string>();

        public void Load()
        {
            _map.Clear();
            string file = AppDomain.CurrentDomain.BaseDirectory + "/Config/Security.xml";
            XElement root = XElement.Load(file);
            DefaultAnswer = GetAttribute(root, "A");
            Pwd = GetAttribute(root, "Pwd");


            var elements = root.Elements("Item");

            foreach (var item in elements)
            {
                string q = GetAttribute(item, "Q");
                string a = GetAttribute(item, "A");

                if (!string.IsNullOrEmpty(q) && !string.IsNullOrEmpty(a))
                {
                    _map.Add(q, a);
                }
            }

            log.Info(string.Format("加载 [{0}]", file));
        }


        public IDictionary<string, string> Map { get { return _map; } }

        private string GetAttribute(XElement element, XName attName)
        {
            if (element == null) return null;
            if (attName == null) return null;

            XAttribute att = element.Attribute(attName);
            if (att == null) return null;
            return att.Value;
        }


        public string DefaultAnswer { get; private set; }


        internal void AddQuestion(string question)
        {
            _questions.Add(question);
        }

        public void FlushQuestion()
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "/Config/新增问题.txt";
                foreach (var item in _questions)
                {
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(path, true))
                    {
                        sw.WriteLine(item);
                    }
                }
            }
            catch (Exception ex)
            {

                log.Error(ex);
            }

            _questions.Clear();
        }

        public string Pwd { get; private set; }

    }
}
