using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScrollableMessageBoxLib.Core
{
    public sealed class ScreenResolution
    {
        private Rectangle _Resolution;

        private int _Width = 0;

        private int _Height = 0;

        private int _BorderSize = 0;

        public int Width => this._Width;

        public int Height => this._Height;

        public int BorderSize => this._BorderSize;

        public ScreenResolution(int borderSize = 64)
        {
            this._BorderSize = borderSize;
            this.GetResolution(Screen.PrimaryScreen, borderSize);

        }

        private void GetResolution(Screen scr, int borderSize = 32)
        {
            this._Height = scr.Bounds.Height - this._BorderSize;
            this._Width = scr.Bounds.Width - this._BorderSize;
        }
    }
}
