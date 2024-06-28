using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ScrollableMessageBoxLib.Utils
{
    public static class HotKeyHelper
    {
        public static char GetHotKeyFromString(string value, char signature = '_')
        {
            char hotkey;

            if (!string.IsNullOrWhiteSpace(value))
            {
                if (value.Contains(signature))
                {
                    int detectedHotKeyPrefix = 0;
                    foreach (char item in value)
                    {
                        if (item == signature)
                        {
                            detectedHotKeyPrefix = value.IndexOf(item);
                            if (detectedHotKeyPrefix < value.Length - 1)
                            {
                                return value[detectedHotKeyPrefix + 1];
                                // return (Key)char.ToUpper(hotkey);
                            }
                        }
                    }
                }
            }
            return (char)0x00;
        }

        public static char ToChar(this Key key)
        {
            char c = '\0';
            if ((key >= Key.A) && (key <= Key.Z))
            {
                c = (char)((int)'a' + (int)(key - Key.A));
            }

            else if ((key >= Key.D0) && (key <= Key.D9))
            {
                c = (char)((int)'0' + (int)(key - Key.D0));
            }

            return c;
        }
    }
}
