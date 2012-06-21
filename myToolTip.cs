using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace IMV
{
    class myToolTip : ToolTip
    {
        public myToolTip()
        {
            this.Popup += new PopupEventHandler(myToolTip_Popup);
        }

        void myToolTip_Popup(object sender, PopupEventArgs e)
        {
            e.ToolTipSize = mySize;
            //throw new NotImplementedException();
        } 
        
        Size mySize = new Size(100, 100);

        public Size Size
        {
            get
            {
                return mySize;
            }
            set
            {
                mySize = value;
            }
        }
    }
}
