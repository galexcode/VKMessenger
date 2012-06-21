using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IMV
{
    public partial class Status : Form
    {
        uint ID = 0; // Айди пользователя, которому отправляем статус. Если 0, то себе
        delegate string SetStatus(string text);
        delegate string SetWallStatus(string text, string name, uint id);
        delegate string UploadFile(uint id, string path);
        delegate void NotifyEvent(string name, string text, uint id);

        public Status()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            vk start = new vk();
            if (checkBox1.Checked) // если флажон для статуса
            {
                SetStatus setStatus = new SetStatus(start.setStatus);
                IAsyncResult res1 = setStatus.BeginInvoke(richTextBox1.Text, null, null);
                res1.AsyncWaitHandle.WaitOne(2000);
                if (setStatus.EndInvoke(res1).IndexOf("error") != -1)
                {
                    NotifyEvent ShowNotify = new NotifyEvent(ShowNotifyWindow);
                    this.Invoke(ShowNotify, "Ошибка!", "При публикации\nвозникли проблемы!", (uint)0);
                }
                //start.setStatus(richTextBox1.Text);
            }
            else // если для записи на стене
            {
                string name = "";

                if (checkBox2.Checked) // если нужно изображение, загружаем
                {
                    OpenFileDialog open = new OpenFileDialog();
                    open.FileName = "";
                    open.Filter = "Изображения|*.jpg;*.gif;*.png";

                    if (open.ShowDialog() == DialogResult.OK)
                    {
                        UploadFile upload = new UploadFile(start.UploadFile);
                        IAsyncResult res11 = upload.BeginInvoke(ID, open.FileName, null, null);

                        int iter = 0;

                        while (!res11.IsCompleted)
                        {
                            iter++;
                            if (iter == 500)
                            {
                                iter = 0;
                                Application.DoEvents();
                            }
                        }

                        res11.AsyncWaitHandle.WaitOne();

                        name = upload.EndInvoke(res11);
                    }

                    if (name == "")
                    {
                        NotifyEvent ShowNotify = new NotifyEvent(ShowNotifyWindow);
                        this.Invoke(ShowNotify, "Ошибка!", "При публикации\nвозникли проблемы!", (uint)0);
                    }
                }

                if (ID != 0) // не себе
                {
                    if (name != "")
                    {
                        SetWallStatus setWallStatus = new SetWallStatus(start.SetWallStatus);
                        IAsyncResult res1 = setWallStatus.BeginInvoke(richTextBox1.Text, name, ID, null, null);
                        res1.AsyncWaitHandle.WaitOne(2000);
                        if (setWallStatus.EndInvoke(res1).IndexOf("error") != -1)
                        {
                            NotifyEvent ShowNotify = new NotifyEvent(ShowNotifyWindow);
                            this.Invoke(ShowNotify, "Ошибка!", "При публикации\nвозникли проблемы!", (uint)0);
                        }
                        //start.SetWallStatus(richTextBox1.Text, name, ID);
                    }
                    else
                    {
                        SetWallStatus setWallStatus = new SetWallStatus(start.SetWallStatus);
                        IAsyncResult res1 = setWallStatus.BeginInvoke(richTextBox1.Text, "", ID, null, null);
                        res1.AsyncWaitHandle.WaitOne(2000);
                        if (setWallStatus.EndInvoke(res1).IndexOf("error") != -1)
                        {
                            NotifyEvent ShowNotify = new NotifyEvent(ShowNotifyWindow);
                            this.Invoke(ShowNotify, "Ошибка!", "При публикации\nвозникли проблемы!", (uint)0);
                        }
                        //start.SetWallStatus(richTextBox1.Text, "", ID);
                    }
                }
                else // себе
                {
                    if (name != "")
                    {
                        SetWallStatus setWallStatus = new SetWallStatus(start.SetWallStatus);
                        IAsyncResult res1 = setWallStatus.BeginInvoke(richTextBox1.Text, name, ID, null, null);
                        res1.AsyncWaitHandle.WaitOne(2000);
                        if (setWallStatus.EndInvoke(res1).IndexOf("error") != -1)
                        {
                            NotifyEvent ShowNotify = new NotifyEvent(ShowNotifyWindow);
                            this.Invoke(ShowNotify, "Ошибка!", "При публикации\nвозникли проблемы!", (uint)0);
                        }
                        //start.SetWallStatus(richTextBox1.Text, name, ID);
                    }
                    else
                    {
                        SetStatus setWallStatus = new SetStatus(start.SetWallStatus);
                        IAsyncResult res1 = setWallStatus.BeginInvoke(richTextBox1.Text, null, null);
                        res1.AsyncWaitHandle.WaitOne(2000);
                        if (setWallStatus.EndInvoke(res1).IndexOf("error") != -1)
                        {
                            NotifyEvent ShowNotify = new NotifyEvent(ShowNotifyWindow);
                            this.Invoke(ShowNotify, "Ошибка!", "При публикации\nвозникли проблемы!", (uint)0);
                        }
                        //start.SetWallStatus(richTextBox1.Text);
                    }
                }
            }
            this.Close();
        }

        void ShowNotifyWindow(string name, string text, uint ID)
        {
            Notify noti = new Notify(name, text, ID);
            noti.ShowWindow();
            noti.ShowTime();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Wait_Paint(object sender, PaintEventArgs e)
        {// Рисуем рамочку вокруг textBox'a
            e.Graphics.DrawRectangle(Pens.Black, 4, 4, 392, 64);
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {// открываем окно поиска пользователя
                FindUser find = new FindUser();
                find.ShowDialog();
                ID = find.UserID;
                find.Close();
                find.Dispose();
                if (ID == 0)
                    checkBox3.Checked = false;
                else
                    checkBox1.Enabled = false;
            }
            else
                checkBox1.Enabled = true;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                checkBox3.Enabled = false;
            else
                checkBox3.Enabled = true;

        }
    }
}
