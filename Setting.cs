using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace IMV
{
    public partial class Setting : Form
    {
        public Setting()
        {// Устанавливаем флажки где надо
            InitializeComponent();
            Offline.Checked = vars.VARS.ShowOffline;
            OfflineMsg.Checked = vars.VARS.GetOfflineMsg;
            Frequency.Checked = vars.VARS.Frequency;
            SaveSettings.Checked = vars.VARS.SaveSettings;
            OtherFolder.Checked = vars.VARS.OtherFolder;
            textBox1.Text = vars.VARS.Directory;
            updateFriends.Checked = vars.VARS.UpdateFriends;
            exitOnCloser.Checked = vars.VARS.ExitOnCloser;
            exitVK.Checked = vars.VARS.ExitVK;
            incom_msg.Checked = vars.VARS.Incoming_message_on;
            out_msg.Checked = vars.VARS.Out_message_on;
            userOnline.Checked = vars.VARS.User_online_on;
            userOffline.Checked = vars.VARS.User_offline_on;
            visual_notify.Checked = vars.VARS.Visual_notify;
            if (vars.VARS.Visual_notify)
            {
                notify_online.Visible = true;
                notify_offline.Visible = true;
                notify_income.Visible = true;
                notify_online.Checked = vars.VARS.Notify_online;
                notify_offline.Checked = vars.VARS.Notify_offline;
                notify_income.Checked = vars.VARS.Notify_income;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Offline.Checked != vars.VARS.ShowOffline)
            {
                vars.VARS.ShowOffline = Offline.Checked;
                myContactList.ContactList.FinishUpdate();
            }

            vars.VARS.GetOfflineMsg = OfflineMsg.Checked;

            vars.VARS.Frequency = Frequency.Checked;

            if (Frequency.Checked)
            {
                try
                {
                    if (!File.Exists(vars.VARS.Directory + "frequency.dat"))
                        File.Create(vars.VARS.Directory + "frequency.dat"); // Создаём фалй с частотой
                    foreach (KeyValuePair<uint, vk.profile> item in vars.VARS.Contact) // заполняем словарь частоты
                        vars.VARS.FrequencyUse.Add(item.Key, 0);

                }
                catch
                { 
                }
            }

            if (vars.VARS.OtherFolder != OtherFolder.Checked)
            { // Меняем папку по умолчанию для сохранения всех настроек и файлов
                if (!OtherFolder.Checked)
                {
                    vars.VARS.Directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\IMV\\";
                    textBox1.Text = vars.VARS.Directory;
                }
                else
                {
                    vars.VARS.OtherFolder = OtherFolder.Checked;
                    vars.VARS.Directory = textBox1.Text + "\\IMV\\";
                }
            }
            vars.VARS.UpdateFriends = updateFriends.Checked;
            vars.VARS.ExitOnCloser = exitOnCloser.Checked;
            vars.VARS.ExitVK = exitVK.Checked;

            vars.VARS.Incoming_message_on = incom_msg.Checked;
            vars.VARS.Out_message_on = out_msg.Checked;
            vars.VARS.User_online_on = userOnline.Checked;
            vars.VARS.User_offline_on = userOffline.Checked;
            vars.VARS.Visual_notify = visual_notify.Checked;
            if (visual_notify.Checked)
            {
                vars.VARS.Notify_online = notify_online.Checked;
                vars.VARS.Notify_offline = notify_offline.Checked;
                vars.VARS.Notify_income = notify_income.Checked;
            }
            if (SaveSettings.Checked) // Если сохраняем настроки, то...
            {
                vars.VARS.SaveSettings = SaveSettings.Checked;
                string path = vars.VARS.Directory + "settings.cfg";
                try
                { // Пишем в файл
                    FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                    StreamWriter write = new StreamWriter(fs, Encoding.UTF8);
                    write.WriteLine("Sound|" + vars.VARS.Sound.ToString());
                    write.WriteLine("SaveSettings|" + vars.VARS.SaveSettings.ToString());
                    write.WriteLine("ShowOffline|" + vars.VARS.ShowOffline.ToString());
                    write.WriteLine("GetOfflineMsg|" + vars.VARS.GetOfflineMsg.ToString());
                    write.WriteLine("Frequency|" + vars.VARS.Frequency.ToString());
                    write.WriteLine("OtherFolder|" + vars.VARS.OtherFolder.ToString());
                    write.WriteLine("ExitOnCloser|" + vars.VARS.ExitOnCloser.ToString());
                    write.WriteLine("directory|" + vars.VARS.Directory);
                    write.WriteLine("updateFriends|" + vars.VARS.UpdateFriends);
                    write.WriteLine("ExitVK|" + vars.VARS.ExitVK);
                    write.WriteLine("incoming_message_on|" + vars.VARS.Incoming_message_on.ToString());
                    write.WriteLine("out_message_on|" + vars.VARS.Out_message_on.ToString());
                    write.WriteLine("user_online_on|" + vars.VARS.User_online_on.ToString());
                    write.WriteLine("user_offline_on|" + vars.VARS.User_offline_on.ToString());
                    write.WriteLine("incoming_message|" + vars.VARS.Incoming_message);
                    write.WriteLine("out_message|" + vars.VARS.Out_message);
                    write.WriteLine("user_online|" + vars.VARS.User_online);
                    write.WriteLine("user_offline|" + vars.VARS.User_offline);
                    write.WriteLine("visual_notify|" + vars.VARS.Visual_notify.ToString());
                    if (visual_notify.Checked)
                    {
                        write.WriteLine("notify_online|" + vars.VARS.Notify_online.ToString());
                        write.WriteLine("notify_offline|" + vars.VARS.Notify_offline.ToString());
                        write.WriteLine("notify_income|" + vars.VARS.Notify_income.ToString());
                    }
                    write.Close();
                    write.Dispose();
                    fs.Dispose();
                }
                catch (Exception exe)
                {

                    GeneralMethods.WriteError(exe.Source, exe.Message, exe.TargetSite);
                    MessageBox.Show("Возникла ошибка при попытке\nзаписи настроек в файл!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            this.Close();
        }

        private void OtherFolder_CheckedChanged(object sender, EventArgs e)
        {// Включает текстбокс или отключаем. Если нужно выбрать другую папку, то включаем
            if (OtherFolder.Checked)
            {
                textBox1.Enabled = true;
                button3.Visible = true;
            }
            else
            {
                textBox1.Text = vars.VARS.Directory;
                textBox1.Enabled = false;
                button3.Visible = false;
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog of = new FolderBrowserDialog();
            of.Description = "Выберите папку для сохранения"; // Надпись у диалогового окна
            of.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); // В какой папке открываемся

            if (of.ShowDialog() == DialogResult.OK)
                textBox1.Text = of.SelectedPath;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Выберите wav файл для входящего сообщения";
            op.FileName = "";
            op.Filter = "Звуковые файлы|*.wav";
            if (op.ShowDialog() == DialogResult.OK)
                vars.VARS.Incoming_message = op.FileName;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Выберите wav файл для исходящего сообщения";
            op.FileName = "";
            op.Filter = "Звуковые файлы|*.wav";
            if (op.ShowDialog() == DialogResult.OK)
                vars.VARS.Out_message = op.FileName;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Выберите wav файл для пользователя вышедшего в онлайн";
            op.FileName = "";
            op.Filter = "Звуковые файлы|*.wav";
            if (op.ShowDialog() == DialogResult.OK)
                vars.VARS.User_online = op.FileName;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Выберите wav файл для пользователя вышедшего в оффлайн";
            op.FileName = "";
            op.Filter = "Звуковые файлы|*.wav";
            if (op.ShowDialog() == DialogResult.OK)
                vars.VARS.User_offline = op.FileName;
        }

        private void visual_notify_CheckedChanged(object sender, EventArgs e)
        {
            if (visual_notify.Checked)
            {
                notify_online.Visible = true;
                notify_offline.Visible = true;
                notify_income.Visible = true;
            }
            else
            {
                notify_online.Visible = false;
                notify_offline.Visible = false;
                notify_income.Visible = false;
            }
        }
    }
}
