using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WeiBoGrab
{
    public delegate void OneObjectParaDelegate(object state);

    public delegate void ListViewItemAddStringDelegate(ListViewItem state, string subItemText);

    public delegate void InvokeDelegate();
}
