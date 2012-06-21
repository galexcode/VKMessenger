using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IMV
{
    public partial class ChatForm : Form
    {
        delegate void getEvent(uint id, int offset);
        delegate void NotifyEvent(string name, string text, uint id);
        delegate bool SendMessage(string text, uint id);
        public Dictionary<uint, TextBox> textbox = new Dictionary<uint, TextBox>(); // Словарь текстбоксов для каждого пользователя свой
        public Dictionary<uint, myRichTextBox> richtbox = new Dictionary<uint, myRichTextBox>(); // Словарь ричтекстбоксов для каждого пользователя свой

        string text;

        protected ChatForm() 
        {
            InitializeComponent();
            //this.KeyPreview = true;
        }

        private sealed class ChatFormCreate
        {
            private static readonly ChatForm instance = new ChatForm();
            public static ChatForm Instance
            {
                get
                {
                    return instance;
                }
            }
        }

        public static ChatForm chat
        {
            get
            {
                return ChatFormCreate.Instance;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true; // Запрещаем закрываться, но...
            chat.Visible = false; // делаем форму невидимой
        }
        
        /// <summary>
        /// Конвертирование времени в обычный формат из UNIX
        /// </summary>
        /// <param name="timestamp">Число секунд</param>
        /// <returns>Дата</returns>

        public static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 4, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try 
            { 
                chat.Text = myTabControl1.SelectedTab.Text;
                vk start = new vk();
                start.markAsRead(Convert.ToUInt32(myTabControl1.SelectedTab.Tag));
                textbox[(uint)(myTabControl1.SelectedTab.Tag)].Select();
            } // При смене вкладки меняем текст заголовка окна
            catch { }
        }

        public void chatIn(uint name, string text, bool show) // Для каждой вкладки создаются отдельные поля ввода и вывода
        {
            chat.tabPage1 = new System.Windows.Forms.TabPage();
            chat.splitContainer1 = new System.Windows.Forms.SplitContainer();
            chat.myRichTextBox1 = new IMV.myRichTextBox();
            chat.textBox1 = new System.Windows.Forms.TextBox();
            chat.splitContainer1.Panel1.SuspendLayout();
            chat.splitContainer1.Panel2.SuspendLayout();
            chat.splitContainer1.SuspendLayout();
            chat.tabPage1.SuspendLayout();
            //chat.SuspendLayout();
            // 
            // myTabControl1
            //
            chat.myTabControl1.Controls.Add(chat.tabPage1);
            // 
            // tabPage1
            // 
            chat.tabPage1.Controls.Add(chat.splitContainer1);
            chat.tabPage1.Location = new System.Drawing.Point(4, 22);
            chat.tabPage1.Tag = name;
            chat.tabPage1.Name = name.ToString();
            chat.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            chat.tabPage1.TabIndex = 3;
            chat.tabPage1.Text = text;
            //chat.tabPage1.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            //
            chat.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            chat.splitContainer1.Location = new System.Drawing.Point(3, 3);
            chat.splitContainer1.Name = "splitContainer1";
            chat.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            chat.splitContainer1.SplitterDistance = 143;
            chat.splitContainer1.TabIndex = 2;
            // 
            // splitContainer1.Panel1
            // 
            chat.splitContainer1.Panel1.Controls.Add(chat.myRichTextBox1);
            // 
            // splitContainer1.Panel2
            // 
            chat.splitContainer1.Panel2.Controls.Add(chat.textBox1);
            // 
            // myRichTextBox1
            //
            chat.myRichTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            chat.myRichTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            chat.myRichTextBox1.Location = new System.Drawing.Point(0, 0);
            chat.myRichTextBox1.Tag = name;
            chat.myRichTextBox1.TabIndex = 1;
            chat.myRichTextBox1.Text = "";
            richtbox.Add(name, myRichTextBox1);
            // 
            // textBox1
            //
            chat.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            chat.textBox1.Location = new System.Drawing.Point(0, 0);
            chat.textBox1.Multiline = true;
            chat.textBox1.Tag = name;
            chat.textBox1.TabIndex = 0;
            chat.textBox1.KeyDown += new KeyEventHandler(textBox1_KeyDown);
            textbox.Add(name, textBox1);

            chat.Text = text;
            chat.ResumeLayout(false);
            chat.tabPage1.ResumeLayout(false);
            chat.splitContainer1.ResumeLayout(false);
            chat.splitContainer1.Panel1.ResumeLayout(false);
            chat.splitContainer1.Panel2.ResumeLayout(false);                        
            chat.Refresh();
            if (show) // Позволяет не прерывать ввод, если сообщение пришло, когда пользователь набирал текст
                chat.myTabControl1.SelectTab(name.ToString());
            chat.textbox[name].Select();
        }

        void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (((e.KeyCode == Keys.Enter) && !e.Control) && (toolStripButton3.Checked == false)) // Как отправлять: ENTER или CTRL+ENTER
            {
                e.SuppressKeyPress = true;
                
                toolStripButton2_Click(toolStripButton2, new EventArgs());
                             
                return;
            }

            if ((e.KeyCode == Keys.Enter && e.Control) && (toolStripButton3.Checked == true))
            {
                e.SuppressKeyPress = true;

                toolStripButton2_Click(toolStripButton2, new EventArgs());

                return;
            }

            if (e.KeyCode == Keys.W && e.Control) // Закрытие вкладки
            {
                toolStripButton1_Click((object)Keys.Enter, (EventArgs)e);
                e.SuppressKeyPress = true;
                return;
            }
            //throw new NotImplementedException();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            text = textbox[(uint)(myTabControl1.SelectedTab.Tag)].Text;
            textbox[(uint)(myTabControl1.SelectedTab.Tag)].Text = "";

            if (!string.IsNullOrEmpty(text.Trim())) // Если textBox не пустой, то печатаем сообщение в richTextBox, а затем отправляем
            {
                richtbox[(uint)(myTabControl1.SelectedTab.Tag)].SelectionStart = richtbox[Convert.ToUInt32(myTabControl1.SelectedTab.Tag)].Text.Length;
                richtbox[(uint)(myTabControl1.SelectedTab.Tag)].SelectionLength = 0;
                richtbox[(uint)(myTabControl1.SelectedTab.Tag)].SelectionFont = new Font(FontFamily.GenericSerif, 8, FontStyle.Bold);
                richtbox[(uint)(myTabControl1.SelectedTab.Tag)].SelectionColor = Color.FromArgb(0, 0, 255);

                richtbox[(uint)(myTabControl1.SelectedTab.Tag)].SelectedText = "Я (" + DateTime.Now.ToShortTimeString() + " " + DateTime.Now.ToShortDateString() + ")";
                richtbox[(uint)(myTabControl1.SelectedTab.Tag)].SelectionStart = richtbox[(uint)(myTabControl1.SelectedTab.Tag)].Text.Length;
                richtbox[(uint)(myTabControl1.SelectedTab.Tag)].SelectionLength = 0;
                richtbox[(uint)(myTabControl1.SelectedTab.Tag)].SelectionFont = new Font(FontFamily.GenericSerif, 9, FontStyle.Regular);
                richtbox[(uint)(myTabControl1.SelectedTab.Tag)].SelectionColor = Color.Black;
                richtbox[(uint)(myTabControl1.SelectedTab.Tag)].SelectedText = "\r\n" + text + "\r\n";
                richtbox[(uint)(myTabControl1.SelectedTab.Tag)].SelectionStart = richtbox[(uint)(myTabControl1.SelectedTab.Tag)].Text.Length;
                richtbox[(uint)(myTabControl1.SelectedTab.Tag)].SelectionLength = 0;
                richtbox[(uint)(myTabControl1.SelectedTab.Tag)].SelectionFont = new Font(FontFamily.GenericSerif, 4, FontStyle.Regular);
                richtbox[(uint)(myTabControl1.SelectedTab.Tag)].SelectedText = "\r\n";
                richtbox[(uint)(myTabControl1.SelectedTab.Tag)].ScrollToCaret();

                if (vars.VARS.Out_message_on && vars.VARS.Sound)
                    GeneralMethods.NotifySound("OutMessage");
            }

            vk start = new vk();

            SendMessage SendMsg = new SendMessage(start.sendMsg);
            IAsyncResult res1 = SendMsg.BeginInvoke(text, (uint)myTabControl1.SelectedTab.Tag, null, null);

            res1.AsyncWaitHandle.WaitOne();

            bool success = SendMsg.EndInvoke(res1);

            if (!success)
            {
                NotifyEvent ShowNotify = new NotifyEvent(ShowNotifyWindow);
                this.Invoke(ShowNotify, "Ошибка!", "Ошибка при отправке!\nПовторите ещё раз!", (uint)0);
            }
        }

        void ShowNotifyWindow(string name, string text, uint ID)
        {
            Notify noti = new Notify(name, text, ID);
            noti.ShowWindow();
            noti.ShowTime();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            try
            {
                richtbox.Remove((uint)myTabControl1.SelectedTab.Tag); // Удаление ричтекстбокса
                textbox.Remove((uint)myTabControl1.SelectedTab.Tag); // УДаление текстбокса
                myTabControl1.TabPages.RemoveAt(myTabControl1.SelectedIndex); // Удаление вкладки и..
                if (myTabControl1.TabPages.Count == 0) // если больше не осталось, скрываем окно
                {
                    chat.Hide();
                    chat.Text = "";
                }
                else
                    myTabControl1.SelectTab(myTabControl1.TabPages.Count - 1);
            }
            catch
            {
                // Возникает ошибка, которая крашит программу
            }
        }

        private void ChatForm_FormClosing(object sender, FormClosingEventArgs e)
        { // Чистим всё, если "закрываем"
            try
            {
                chat.Text = "";
                richtbox.Clear();
                textbox.Clear();
                myTabControl1.TabPages.Clear();
            }
            catch
            { }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            History hist = new History();
            getEvent newEvent = new getEvent(hist.getHistory);
            IAsyncResult res2 = newEvent.BeginInvoke(Convert.ToUInt32(myTabControl1.SelectedTab.Tag), 0, null, null); // Заправшиваем историю
            hist.Text = chat.Text;
            hist.Show(); // Открываем окно с историей
        }
    }
}