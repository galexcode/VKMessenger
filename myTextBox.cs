using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace IMV
{
    class myTextBox : Control
    {
        public myTextBox()
        {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }

        protected override Cursor DefaultCursor
        {
            get
            {
                return Cursors.IBeam;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            int x = this.ClientRectangle.X;
            int y = this.ClientRectangle.Y;

            int rectWidth = this.Width;
            int rectHeight = this.Height;

            int radius = 2;
            GraphicsPath GraphPath = new GraphicsPath();

            GraphPath.AddLine(x + radius, y, x + rectWidth - (radius * 2), y);
            GraphPath.AddArc(x + rectWidth - (radius * 2), y, radius * 2, radius * 2, 270, 90);
            GraphPath.AddLine(x + rectWidth, y + radius, x + rectWidth, y + rectHeight - (radius * 2));
            GraphPath.AddArc(x + rectWidth - (radius * 2), y + rectHeight - (radius * 2), radius * 2, radius * 2, 0, 90);
            GraphPath.AddLine(x + rectWidth - (radius * 2), y + rectHeight, x + radius, y + rectHeight);
            GraphPath.AddArc(x, y + rectHeight - (radius * 2), radius * 2, radius * 2, 90, 90);
            GraphPath.AddLine(x, y + rectHeight - (radius * 2), x, y + radius);
            GraphPath.AddArc(x, y, radius * 2, radius * 2, 180, 90);
            GraphPath.CloseFigure();

            this.Region = new Region(GraphPath);

            e.Graphics.FillPath(Brushes.White, GraphPath);

            e.Graphics.DrawString(this.Text, Font, Brushes.Black, x + 2, y + 2);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            this.Text += e.KeyChar;
            this.Invalidate();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back) // если бакспейс, то...
            {
                if (this.Text.Length > 0) // если естье ещё текс, удаляем 1 символ и перерисовываем
                {
                    this.Text = this.Text.Remove(this.Text.Length - 1);
                    this.Invalidate();
                    e.SuppressKeyPress = true;
                    return;
                }
                else
                    e.SuppressKeyPress = true;
            }
            if (e.KeyCode == Keys.Escape) // если эспейп, стираем текст, делаем невидимым
            {
                this.Text = "";
                this.Visible = false;
                myContactList.ContactList.Sort();
                myContactList.ContactList.Invalidate();
                e.SuppressKeyPress = true;
                return;
            }
            if (e.KeyCode == Keys.Enter) // игнорируем клавишу
            {
                e.SuppressKeyPress = true;
                return;
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            this.Focus();
            base.OnMouseClick(e);
        }
    }
}
