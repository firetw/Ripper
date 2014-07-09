﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using WebLoginer.Tools;

namespace Ripper.View.Model
{
    [Serializable]
    public class Entity : BindableObject
    {
        private int _leDou;
        private string _status;
        public Dispatcher Dispatcher { get; set; }
        public int Seq { get; set; }
        public string Tel { get; set; }
        public string Pwd { get; set; }
        public int LeDou
        {
            get { return _leDou; }
            set
            {
                _leDou = value;
                OnPropertyChanged("LeDou");
            }
        }

        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged("Status");
            }
        }


        public Entity() { }

        public Entity(string tel, string pwd)
        {
            Tel = tel;
            Pwd = pwd;
        }

        public bool Success { get; set; }

        public Entity(string line)
        {
            string[] paras = Utils.Split(line);
            if (paras == null || paras.Length < 2) return;



            Tel = paras[0];
            Pwd = paras[0];
        }


        public CookieContainer CookieContainer { get; set; }
        public CookieCollection Cookies { get; set; }
    }

    public class BindableObject : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;


        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                System.Threading.Interlocked.Exchange<string>(ref mChangedPropertyName, propertyName);
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        private string mChangedPropertyName = string.Empty;
        /// <summary>
        /// 当前是那个属性值改变
        /// </summary>
        public string ChangedPropertyName
        {

            set { mChangedPropertyName = value; }
            get { return mChangedPropertyName; }
        }
    }
}
