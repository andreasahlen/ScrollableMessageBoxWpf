using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ScrollableMessageBoxLib.Enums
{
    public enum MessageBoxResultEx
    {
        None = MessageBoxResult.None,
        OK = MessageBoxResult.OK,
        Cancel = MessageBoxResult.Cancel,
        Yes = MessageBoxResult.Yes,
        No = MessageBoxResult.No,
        Abort = 1024,
        Retry = 2048,
        Ignore = 4096
    }
}
