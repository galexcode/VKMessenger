using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace IMV
{
    public sealed class myContactList : Control
    {
        int time;
        uint uid;
        public List<uint> Items = new List<uint>();
        const byte HEIGHT_ITEM = 30; // Высота одного элемента
        const int TIME_TOOLTIP = 1000; // Время удержания курсора для вывода доп.информации, в миллисекундах
        vk.profile selectedItem = new vk.profile();
        int selectedIndex = -1;
        VScrollBar vscroll;
        delegate void GetHistory(uint id, int count);
        delegate Image GetPhoto(uint id, string size);
        delegate System.Collections.Hashtable GetInfo(uint id);

        Image photo;
        string birthday = "";
        string name = "";
        string phone = "";

        ContextMenuStrip context = new ContextMenuStrip();
        myToolTip tip = new myToolTip();
        Timer timer = new Timer();

        protected myContactList() 
        {
            SetStyle(ControlStyles.UserPaint, true); // Рисуем всё сами
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            this.Visible = true;
            vscroll = new VScrollBar();
            vscroll.Dock = DockStyle.Right;
            vscroll.Visible = false;
            this.Controls.Add(vscroll);

            vscroll.SmallChange = 1; // На какое количество контактов смещаться (стрелочка)
            vscroll.LargeChange = 60; // На какое кол-во контактов смещаться (поле)
            //vscroll.Maximum = Items.Count * HEIGHT_ITEM - this.Height + vscroll.LargeChange;
            vscroll.ValueChanged += new EventHandler(vscroll_ValueChanged);

            //ToolStripItem item = new ToolStripItem();
            context.ItemClicked += new ToolStripItemClickedEventHandler(context_ItemClicked);
            context.VisibleChanged += new EventHandler(context_VisibleChanged);

            tip.AutoPopDelay = 15000; // Сколько будет видна доп.информация
            tip.InitialDelay = 0; 
            tip.OwnerDraw = true; // Рисуем сами
            tip.Draw += new DrawToolTipEventHandler(tip_Draw);

            timer.Tick += new EventHandler(timer_Tick_About);
        }

        private sealed class myContactListCreate
        {
            private static readonly myContactList instance = new myContactList();
            public static myContactList Instance
            {
                get
                {
                    return instance;
                }
            }
        }

        public static myContactList ContactList
        {
            get
            {
                return myContactListCreate.Instance;
            }
        }

        void tip_Draw(object sender, DrawToolTipEventArgs e)
        {
            try
            {
                GraphicsPath GraphPath = new GraphicsPath();

                int x = e.Bounds.X;
                int y = e.Bounds.Y;

                int rectWidth = e.Bounds.Width;
                int rectHeight = e.Bounds.Height;

                int radius = 2;
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
                
                e.Graphics.DrawPath(new Pen(Brushes.White, 1), GraphPath); // Вырисовываем на экран ранее нарисованное в памяти
                e.Graphics.FillPath(Brushes.White, GraphPath); // Заполняем внутреннюю часть фигуры
                GraphPath.Dispose();


                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Near;

                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(218, 225, 232)), e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Y + 20);
                e.Graphics.DrawString(name, new Font(e.Font.FontFamily, 14F, FontStyle.Bold, GraphicsUnit.Pixel), Brushes.Black, e.Bounds.X + 5, e.Bounds.Y + 1, format);
                try
                {
                    e.Graphics.DrawImage(photo, e.Bounds.X + 5, e.Bounds.Y + 25);
                }
                catch (ArgumentNullException exe)
                {
                    GeneralMethods.WriteError(exe.Source, exe.Message, exe.TargetSite);
                }

                e.Graphics.DrawString("День рождения", e.Font, Brushes.Black, photo.Width + 5, e.Bounds.Y + 22, format);
                e.Graphics.DrawString(birthday, new Font(e.Font.FontFamily, 11F, FontStyle.Bold, GraphicsUnit.Pixel), Brushes.Black, photo.Width + 5, e.Bounds.Y + 37, format);
                e.Graphics.DrawString("Мобильный", e.Font, Brushes.Black, photo.Width + 5, e.Bounds.Y + 52, format);
                e.Graphics.DrawString(phone, new Font(e.Font.FontFamily, 11F, FontStyle.Bold, GraphicsUnit.Pixel), Brushes.Black, photo.Width + 5, e.Bounds.Y + 67, format);
            }
            catch
            {

            }
            //throw new NotImplementedException();
        }

        void context_VisibleChanged(object sender, EventArgs e)
        {
            if (context.Visible != true)
                context.Items.Clear();
            //throw new NotImplementedException();
        }

        void context_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(Convert.ToString(e.ClickedItem.Tag));
            //throw new NotImplementedException();
        }

        void vscroll_ValueChanged(object sender, EventArgs e)
        {
            this.Invalidate();
            //throw new NotImplementedException();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                //base.OnPaint(e);
                if (this.Height < Items.Count * HEIGHT_ITEM)
                    vscroll.Visible = true;
                else
                {
                    vscroll.Visible = false;
                    vscroll.Value = 0;
                }

                e.Graphics.FillRectangle(Brushes.White, e.ClipRectangle);

                int screenOffset = 0;
                for (int i = vscroll.Value; i < Items.Count; i++)
                {
                    OnDrawItem(e, screenOffset * HEIGHT_ITEM, i);
                    screenOffset += 1;
                    if (screenOffset * HEIGHT_ITEM > this.Height)
                        break;
                }
            }
            catch (ArgumentNullException exe)
            {
                GeneralMethods.WriteError(exe.Source, exe.Message, exe.TargetSite);
            }
        }
        
        private void OnDrawItem(PaintEventArgs e, int offset, int idx)
        {
            try
            {
                if ((vars.VARS.Contact[Items[idx]].online == true) && (vars.VARS.Contact[Items[idx]].uid != selectedItem.uid)) // Если контакт в онлайне и он не выбран
                {
                    //e.Graphics.DrawLine(Pens.Gray, 0, offset, this.Width, offset);
                    try
                    {
                        if (vars.VARS.SmallPhoto.Images.ContainsKey(vars.VARS.Contact[Items[idx]].uid.ToString()))
                            vars.VARS.SmallPhoto.Draw(e.Graphics, 2, offset + 2, 26, 26, vars.VARS.SmallPhoto.Images.IndexOfKey(vars.VARS.Contact[Items[idx]].uid.ToString())); // рисуем фотку

                    }
                    catch (ArgumentOutOfRangeException exe)
                    {
                        GeneralMethods.WriteError(exe.Source, exe.Message, exe.TargetSite);
                    }
                    e.Graphics.DrawString(vars.VARS.Contact[Items[idx]].UserName, Font, Brushes.Black, 35F, offset + 8); // имя
                    e.Graphics.DrawLine(Pens.Gray, 0, offset + HEIGHT_ITEM, this.Width, offset + HEIGHT_ITEM);
                }
                else if ((vars.VARS.Contact[Items[idx]].online != true) && (vars.VARS.Contact[Items[idx]].uid != selectedItem.uid) && vars.VARS.ShowOffline) // Если контакт в оффлайне и он не выбран
                {
                    //e.Graphics.DrawLine(Pens.Gray, 0, offset, this.Width, offset);
                    try
                    {
                        if (vars.VARS.SmallPhoto.Images.ContainsKey(vars.VARS.Contact[Items[idx]].uid.ToString()))
                            vars.VARS.SmallPhoto.Draw(e.Graphics, 2, offset + 2, 26, 26, vars.VARS.SmallPhoto.Images.IndexOfKey(vars.VARS.Contact[Items[idx]].uid.ToString())); // рисуем фотку
                    }
                    catch (ArgumentOutOfRangeException exe)
                    {
                        GeneralMethods.WriteError(exe.Source, exe.Message, exe.TargetSite);
                    }
                    e.Graphics.DrawString(vars.VARS.Contact[Items[idx]].UserName, Font, Brushes.Black, 35F, offset + 8); // имя
                    e.Graphics.DrawLine(Pens.Gray, 0, offset + HEIGHT_ITEM, this.Width, offset + HEIGHT_ITEM);
                    Brush br = new SolidBrush(Color.FromArgb(98, 170, 170, 170));
                    e.Graphics.FillRectangle(br, 0, offset, this.Width, HEIGHT_ITEM);
                    br.Dispose();
                }

                else if (vars.VARS.Contact[Items[idx]].uid == selectedItem.uid) // Если контакт выбран
                {
                    if (vars.VARS.ShowOffline || vars.VARS.Contact[Items[idx]].online)
                    {
                        Brush br = new SolidBrush(Color.FromArgb(255, 121, 187, 255));
                        e.Graphics.FillRectangle(br, 0, offset, this.Width, HEIGHT_ITEM);
                        br.Dispose();
                        try
                        {
                            if (vars.VARS.SmallPhoto.Images.ContainsKey(selectedItem.uid.ToString()))
                                vars.VARS.SmallPhoto.Draw(e.Graphics, 2, offset + 2, 26, 26, vars.VARS.SmallPhoto.Images.IndexOfKey(selectedItem.uid.ToString())); // рисуем фотку
                        }
                        catch (ArgumentOutOfRangeException exe)
                        {
                            GeneralMethods.WriteError(exe.Source, exe.Message, exe.TargetSite);
                        }
                        e.Graphics.DrawString(vars.VARS.Contact[Items[idx]].UserName, Font, Brushes.White, 35F, offset + 8); // имя
                    }
                }
            }
            catch (Exception exe)
            {
                GeneralMethods.WriteError(exe.Source, exe.Message, exe.TargetSite);
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                vk start = new vk();

                if (!vars.VARS.Chat.richtbox.ContainsKey(selectedItem.uid)) // если чат с пользователем ещё не открыт
                {
                    vars.VARS.Chat.WindowState = FormWindowState.Normal;
                    vars.VARS.Chat.chatIn(selectedItem.uid, selectedItem.UserName, true);
                }

                GetHistory newEvent = new GetHistory(start.getHistory);
                IAsyncResult res2 = newEvent.BeginInvoke(selectedItem.uid, 5, null, null); // в асинхронном потоке получаем историю сообщений, чтобы чат открывался сразу
                //start.getHistory(temp.uid, 5); // заправшиваем историю

                if (vars.VARS.Chat.Visible == false) // если невидим, делаем видимым
                {
                    vars.VARS.Chat.Show();
                    vars.VARS.Chat.Text = selectedItem.UserName;
                }

                //vars.VARS.Chat.Activate();
                if (vars.VARS.Frequency) // если настройка включена обновляем данные частоты
                {
                    if (vars.VARS.FrequencyUse.ContainsKey(selectedItem.uid))
                    {
                        uint j = vars.VARS.FrequencyUse[selectedItem.uid];
                        j++;
                        vars.VARS.FrequencyUse.Remove(selectedItem.uid);
                        vars.VARS.FrequencyUse.Add(selectedItem.uid, j);
                    }
                }
                e.SuppressKeyPress = true;
            }
            base.OnKeyUp(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                if (Items.Count >= ++selectedIndex)
                {
                    ++selectedIndex;
                    this.Update();
                }
            }

            if (e.KeyCode == Keys.Down)
            {
                if (--selectedIndex >= 0)
                {
                    --selectedIndex;
                    this.Update();
                }
            }
            base.OnKeyDown(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            this.Focus();
            try
            {
                selectedItem = vars.VARS.Contact[Items[(vscroll.Value) + (int)(e.Y / HEIGHT_ITEM)]];
                selectedIndex = (vscroll.Value) + (int)(e.Y / HEIGHT_ITEM);
                this.Refresh();
                if (e.Button == MouseButtons.Right)
                {
                    context.Items.Add("Перейти на страницу");
                    context.Items.Add("К фотографиям");
                    context.Items[0].Tag = "http://vkontakte.ru/id" + selectedItem.uid;
                    context.Items[1].Tag = "http://vkontakte.ru/tag" + selectedItem.uid;
                    context.Show(this, e.X, e.Y);
                }
            }
            catch (ArgumentOutOfRangeException exe)
            {
                //GeneralMethods.WriteError(exe.Source, exe.Message, exe.TargetSite);
            }
            base.OnMouseClick(e);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            this.Focus();
            vk start = new vk();
            vk.profile temp = new vk.profile();
            try
            {
                temp = vars.VARS.Contact[Items[(vscroll.Value) + (int)(e.Y / HEIGHT_ITEM)]]; // если контакт на этом месте существует, то его данные записываются в переменную
            }
            catch (ArgumentOutOfRangeException exe)
            {
                //GeneralMethods.WriteError(exe.Source, exe.Message, exe.TargetSite);
                return; 
            }

            if (!(temp.online || vars.VARS.ShowOffline)) // когда оффлайн скрыты предотвращаем случайные нажатия
                return;

            if (!vars.VARS.Chat.richtbox.ContainsKey(temp.uid)) // если чат с пользователем ещё не открыт
            {
                vars.VARS.Chat.WindowState = FormWindowState.Normal;
                vars.VARS.Chat.chatIn(temp.uid, temp.UserName, true);
            }

            GetHistory newEvent = new GetHistory(start.getHistory);
            IAsyncResult res2 = newEvent.BeginInvoke(temp.uid, 5, null, null); // в асинхронном потоке получаем историю сообщений, чтобы чат открывался сразу
                //start.getHistory(temp.uid, 5); // заправшиваем историю

            if (vars.VARS.Chat.Visible == false) // если невидим, делаем видимым
            {
                vars.VARS.Chat.Show();
                vars.VARS.Chat.Text = temp.UserName;
            }

            //vars.VARS.Chat.Activate();
            if (vars.VARS.Frequency) // если настройка включена обновляем данные частоты
            {
                if (vars.VARS.FrequencyUse.ContainsKey(temp.uid))
                {
                    uint j = vars.VARS.FrequencyUse[temp.uid];
                    j++;
                    vars.VARS.FrequencyUse.Remove(temp.uid);
                    vars.VARS.FrequencyUse.Add(temp.uid, j);
                }
            }

            base.OnMouseDoubleClick(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (vscroll.Visible)
            {
                if ((vscroll.Value - (e.Delta / 40)) > 0 && (vscroll.Value - (e.Delta / 40)) < vscroll.Maximum - vscroll.LargeChange) // прокрутка списка контактов
                    vscroll.Value += -(e.Delta / 40);
                else if (vscroll.Value - (e.Delta / 40) <= 0) // чтобы в минус не уходило значение скролда
                    vscroll.Value = 0;
                else if (vscroll.Value - (e.Delta / 40) >= vscroll.Maximum - vscroll.LargeChange) // чтобы не вылетало за значение максимум
                    vscroll.Value = vscroll.Maximum - vscroll.LargeChange + 1;
            }

            if (tip.Active)
                tip.Hide(this);
            timer.Stop(); // отключаем таймер всплывающей подсказки
            time = 0; // обнуляем все переменные
            uid = 0;
            photo = null;
            base.OnMouseWheel(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            try
            {
                if ((uid != vars.VARS.Contact[Items[(vscroll.Value) + (int)(e.Y / HEIGHT_ITEM)]].uid) && (vars.VARS.Contact[Items[(vscroll.Value) + (int)(e.Y / HEIGHT_ITEM)]].online || vars.VARS.ShowOffline)) // если мы не просматривали этот контакт до этого
                {
                    photo = null;
                    uid = vars.VARS.Contact[Items[(vscroll.Value) + (int)(e.Y / HEIGHT_ITEM)]].uid; // номер пользователя записываем
                    tip.Hide(this);
                    time = 0;
                    timer.Interval = 500;
                    timer.Start(); // включаем таймер
                }
            }
            catch (ArgumentOutOfRangeException exe)
            {
                //GeneralMethods.WriteError(exe.Source, exe.Message, exe.TargetSite);
            }
            base.OnMouseMove(e);
        }

        void timer_Tick_About(object sender, EventArgs e)
        {
            time += 500;
            if (time == TIME_TOOLTIP) // когда прошло нужное количество времени, запрашиваем данные о пользователе
            {
                tip.Hide(this);
                vk start = new vk();

                GetPhoto getPhoto = new GetPhoto(start.getPhoto);
                IAsyncResult res1 = getPhoto.BeginInvoke(uid, "medium", null, null);
                
                while (!res1.IsCompleted)
                    Application.DoEvents();

                res1.AsyncWaitHandle.WaitOne();
                photo = getPhoto.EndInvoke(res1);
//                photo = start.getPhoto(uid, "medium");

                if (photo == null)
                {
                    if (File.Exists(vars.VARS.Directory + "\\medium\\" + uid.ToString()))
                        try
                        {
                            photo = Image.FromFile(vars.VARS.Directory + "\\medium\\" + uid.ToString());
                        }
                        catch (OutOfMemoryException exe)
                        {
                            GeneralMethods.WriteError(exe.Source, exe.Message, exe.TargetSite);
                        }
                    else
                    {
                        timer.Stop();
                        time = 0;
                        return;
                    }
                }

                GetInfo getInfo = new GetInfo(start.getInfo);
                IAsyncResult res2 = getInfo.BeginInvoke(uid, null, null);

                while (!res2.IsCompleted)
                    Application.DoEvents();

                res1.AsyncWaitHandle.WaitOne();
                System.Collections.Hashtable Info = getInfo.EndInvoke(res2);

                //System.Collections.Hashtable Info = start.getInfo(uid);
                if (Info != null && !Info.ContainsKey("error"))
                {
                    System.Collections.ArrayList Data = (System.Collections.ArrayList)Info["response"];


                    name = Convert.ToString(((System.Collections.Hashtable)Data[0])["first_name"]) + " " + Convert.ToString(((System.Collections.Hashtable)Data[0])["last_name"]);
                    string bdate = Convert.ToString(((System.Collections.Hashtable)Data[0])["bdate"]);
                    phone = Convert.ToString(((System.Collections.Hashtable)Data[0])["mobile_phone"]);

                    string month = "";

                    if (bdate != "")
                    {
                        string[] bday = bdate.Split('.');
                        switch (bday[1]) // месяц текстом чтобы был
                        {
                            case "1": month = "января"; break;
                            case "2": month = "февраля"; break;
                            case "3": month = "марта"; break;
                            case "4": month = "апреля"; break;
                            case "5": month = "мая"; break;
                            case "6": month = "июня"; break;
                            case "7": month = "июля"; break;
                            case "8": month = "августа"; break;
                            case "9": month = "сентября"; break;
                            case "10": month = "октября"; break;
                            case "11": month = "ноября"; break;
                            case "12": month = "декабря"; break;
                        }
                        birthday = bday[0] + " " + month + " " + (bday.Length == 2 ? "" : bday[2]); // формируем строку дня рождения
                    }

                    tip.Size = new Size(220, (photo == null) ? 200 : (photo.Height + 40)); // задаём размер подсказки
                    tip.Show(uid.ToString(), this, this.ClientRectangle.X + this.Width, this.PointToClient(Control.MousePosition).Y); // показываем подсказку                   
                }
                timer.Stop();
                time = 0;
            }
            //throw new NotImplementedException();
        }

        protected override void OnMouseLeave(EventArgs e)
        {// сбрасываем все настройки
            if (tip.Active)
                tip.Hide(this);
            timer.Stop();
            time = 0;
            uid = 0;
            photo = null;
            base.OnMouseLeave(e);
        }

        public int Offset
        {
            get
            {
                return vscroll.Value;
            }
        }

        public void FinishUpdate()
        {
            vscroll.Minimum = 0;
            if (vars.VARS.ShowOffline)
                vscroll.Maximum = Math.Abs(Items.Count + vscroll.LargeChange - this.Height / HEIGHT_ITEM - 1); //Items.Count * HEIGHT_ITEM - this.Height + vscroll.LargeChange;// -Items.Count; Устанавливаем максимум прокрутки
            else // для режима, когда оффлайн пользователи не показываются
            {
                int j = 0;
                for (int i = 0; i < this.Items.Count; i++)
                    if (vars.VARS.Contact[this.Items[i]].online)
                        j++;
                vscroll.Maximum = Math.Abs(j + vscroll.LargeChange - this.Height / HEIGHT_ITEM - 1);
            }

            this.Refresh();
        }

        protected override void OnResize(EventArgs e)
        {
            FinishUpdate(); // обновляем настроки контакт листа
            base.OnResize(e);
        }

        public void Sort()
        {
            if (!vars.VARS.Frequency) // если по алфавиту
            {
                for (int i = 0; i < this.Items.Count; i++)
                {
                    for (int j = 0; j < this.Items.Count - 1; j++)
                    {
                        if ((vars.VARS.Contact[this.Items[j]].online == vars.VARS.Contact[this.Items[j + 1]].online &&
                            vars.VARS.Contact[this.Items[j]].UserName.CompareTo(vars.VARS.Contact[this.Items[j + 1]].UserName) > 0) ||
                            (!vars.VARS.Contact[this.Items[j]].online && vars.VARS.Contact[this.Items[j + 1]].online))
                        {
                            uint tmp = this.Items[j + 1];
                            this.Items[j + 1] = this.Items[j];
                            this.Items[j] = tmp;
                        }
                    }
                }
            }
            else // иначе по частоте
            {
                for (int i = 0; i < this.Items.Count; i++)
                {
                    for (int j = 0; j < this.Items.Count - 1; j++)
                    {
                        if ((vars.VARS.Contact[this.Items[j]].online == vars.VARS.Contact[this.Items[j + 1]].online) &&
                            (vars.VARS.FrequencyUse[this.Items[j]] < vars.VARS.FrequencyUse[this.Items[j + 1]]) ||
                            (!vars.VARS.Contact[this.Items[j]].online && vars.VARS.Contact[this.Items[j + 1]].online))
                        {
                            uint tmp = this.Items[j + 1];
                            this.Items[j + 1] = this.Items[j];
                            this.Items[j] = tmp;
                        }
                    }
                }
            }
        }
    }
}