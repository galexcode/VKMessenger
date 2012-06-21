using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IMV
{
    sealed public class myRichTextBox : RichTextBox
    {
        public myRichTextBox()
        {
			this.Enabled = true;
            this.ReadOnly = true;
            this.BackColor = Color.White;
            this.MouseEnter += delegate(object sender, EventArgs e)
            {
                this.Cursor = Cursors.Default;
            };
        }
    }
}