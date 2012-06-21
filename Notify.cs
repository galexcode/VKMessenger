using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Runtime.InteropServices;


namespace IMV
{
    class Notify : Form
    {
        const int VISIBLE_TIME = 5000;

        const int SW_SHOWNOACTIVATE = 4; // Параметр, который делает форму неактивной
        const int HWND_TOPMOST = -1; // Создаём форму поверх всех остальных
        const uint SWP_NOACTIVATE = 0x0010; // Запрещаем окну брать на себя фокус
        const int CS_DROPSHADOW = 0x00020000; // Тень вокруг формы

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        static extern bool SetWindowPos(int hWnd, int hWndIA, int X, int Y, int CX, int CY, uint Flags);       

        uint id;
        string name;
        string status;
        int timervis = 0;
        bool flag = true;

        public Notify(string name, string status, uint id)
        {
            this.Opacity = 0;
            this.ShowInTaskbar = false;
            this.Width = 180;
            this.Height = 60;
            this.name = name;
            status = status.Replace("<br>", "\n");
            this.status = status;
            this.id = id;
            this.StartPosition = FormStartPosition.Manual;
            int width = Screen.PrimaryScreen.Bounds.Width;
            int height = Screen.PrimaryScreen.Bounds.Height;
            int panel = Screen.PrimaryScreen.Bounds.Height - Screen.PrimaryScreen.WorkingArea.Height;
            int openNotify = vars.VARS.OpenNotify * this.Height + (vars.VARS.OpenNotify >= 1 ? 10 * vars.VARS.OpenNotify : 0);
            this.Location = new System.Drawing.Point(width - this.Width - 10, height - this.Height - panel - 10 - openNotify);
            vars.VARS.OpenNotify += 1;
            this.FormBorderStyle = FormBorderStyle.None;
        }
      
        public void ShowWindow()
        {
            ShowWindow(this.Handle, SW_SHOWNOACTIVATE);
            SetWindowPos(this.Handle.ToInt32(), HWND_TOPMOST, Left, Top, Width, Height, SWP_NOACTIVATE);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                // для того, чтобы была тень
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                GraphicsPath GraphPath = new GraphicsPath();

                int x = e.ClipRectangle.X;
                int y = e.ClipRectangle.Y;

                int rectWidth = e.ClipRectangle.Width;
                int rectHeight = e.ClipRectangle.Height;

                int radius = 5;
                // ----- Рисуем в памяти фигуру окошка -----
                GraphPath.AddLine(x + radius, y, x + rectWidth - (radius * 2), y);
                GraphPath.AddArc(x + rectWidth - (radius * 2), y, radius * 2, radius * 2, 270, 90);
                GraphPath.AddLine(x + rectWidth, y + radius, x + rectWidth, y + rectHeight - (radius * 2));
                GraphPath.AddArc(x + rectWidth - (radius * 2), y + rectHeight - (radius * 2), radius * 2, radius * 2, 0, 90);
                GraphPath.AddLine(x + rectWidth - (radius * 2), y + rectHeight, x + radius, y + rectHeight);
                GraphPath.AddArc(x, y + rectHeight - (radius * 2), radius * 2, radius * 2, 90, 90);
                GraphPath.AddLine(x, y + rectHeight - (radius * 2), x, y + radius);
                GraphPath.AddArc(x, y, radius * 2, radius * 2, 180, 90);
                GraphPath.CloseFigure();
                // ----- Закончили рисовать -----

                this.Region = new Region(GraphPath);

                e.Graphics.DrawPath(new Pen(Brushes.Black, 1), GraphPath); // Вырисовываем на экран ранее нарисованное в памяти
                e.Graphics.FillPath(Brushes.White, GraphPath); // Заполняем внутреннюю часть фигуры
                GraphPath.Dispose();


                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Near;
                Rectangle rect = new Rectangle(e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle.Width, e.ClipRectangle.Y + 20);

                using (Brush br = new LinearGradientBrush(rect, Color.FromArgb(78, 121, 161), Color.FromArgb(22, 70, 115), LinearGradientMode.Vertical))
                    e.Graphics.FillRectangle(br, e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle.Width, e.ClipRectangle.Y + 20);
                e.Graphics.DrawString(name, new Font(this.Font.FontFamily, 12F, FontStyle.Bold, GraphicsUnit.Pixel), Brushes.White, e.ClipRectangle.X + 5, e.ClipRectangle.Y + 1, format);
                e.Graphics.DrawString(status, new Font(this.Font.FontFamily, 14F, FontStyle.Regular, GraphicsUnit.Pixel), Brushes.Black, e.ClipRectangle.X + 5, e.ClipRectangle.Y + 20, format);
            }
            catch (Exception exe)
            {
                GeneralMethods.WriteError(exe.Source, exe.Message, exe.TargetSite);
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                this.Close();

            if (e.Button == MouseButtons.Left)
            {
                if (this.id != 0)
                {
                    vk start = new vk();

                    if (!vars.VARS.Chat.richtbox.ContainsKey(this.id))
                    {
                        vars.VARS.Chat.WindowState = FormWindowState.Normal;
                        vars.VARS.Chat.chatIn(this.id, this.name, true);
                    }

                    start.getHistory(this.id, 5);

                    if (vars.VARS.Chat.Visible == false)
                    {
                        vars.VARS.Chat.Show();
                        vars.VARS.Chat.Text = this.name;
                    }
                }
                else
                    this.Close();
            }
            base.OnMouseClick(e);
        }

        public void ShowTime()
        {
            Timer time = new Timer();
            time.Interval = 10;
            time.Tick += new EventHandler(time_Tick);
            time.Start();
        }

        void time_Tick(object sender, EventArgs e)
        {
            try
            {
                if (flag)
                    this.Opacity += 0.01;

                if (this.Opacity == 1)
                {
                    flag = false;
                    timervis += 10;
                }

                if (timervis == VISIBLE_TIME)
                {
                    this.Close();
                    timervis = 0;
                    flag = true;
                    ((Timer)(sender)).Stop();
                    //this.Opacity -= 0.01;
                }

                //if (this.Opacity == 0)
                //{
                //    this.Close();

                //    timervis = 0;
                //    flag = true;
                //    ((Timer)(sender)).Stop();
                //}
            }
            catch (ObjectDisposedException exe)
            {
                GeneralMethods.WriteError(exe.Source, exe.Message, exe.TargetSite);
            }
            //throw new NotImplementedException();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            vars.VARS.OpenNotify -= 1;
            base.OnClosing(e);
        }
    }
}
