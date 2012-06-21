using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Collections;
using System.IO;
using System.Net;

namespace IMV
{
    struct GeneralMethods
    {
        [DllImport("winmm.dll", EntryPoint = "sndPlaySound")]
        public static extern long PlaySound(string fileName, long flags); // Импортируем функцию воспроизведения звука

        #region Печать сообщения в чат

        /// <summary>
        /// Метод печатает сообщение в чат выбранного пользователя
        /// </summary>
        /// <param name="outId">Номер пользователя</param>
        /// <param name="text">Текст сообщения</param>
        /// <param name="name">Имя пользователя</param>
        /// <param name="date">Дата сообщения</param>

        public static void printMsg(uint outId, string text, string name, double date)
        {
            Color myColor;

            text = text.Replace("<br>", "\n"); // Заменяем html вырианты
            text = text.Replace("&quot;", "\"");

            if (name == vars.VARS.Mid.ToString())
            {
                name = "Я";
                myColor = Color.FromArgb(0, 0, 255);
            }
            else
                myColor = Color.FromArgb(250, 28, 33);


            vars.VARS.Chat.richtbox[outId].SelectionStart = vars.VARS.Chat.richtbox[outId].Text.Length;
            vars.VARS.Chat.richtbox[outId].SelectionLength = 0;
            vars.VARS.Chat.richtbox[outId].SelectionFont = new Font(FontFamily.GenericSerif, 8, FontStyle.Bold);
            vars.VARS.Chat.richtbox[outId].SelectionColor = myColor;

            vars.VARS.Chat.richtbox[outId].SelectedText = name + " (" + ChatForm.ConvertFromUnixTimestamp(date).ToShortTimeString() + " " + ChatForm.ConvertFromUnixTimestamp(date).ToShortDateString() + ")";
            vars.VARS.Chat.richtbox[outId].SelectionStart = vars.VARS.Chat.richtbox[outId].Text.Length;
            vars.VARS.Chat.richtbox[outId].SelectionLength = 0;
            vars.VARS.Chat.richtbox[outId].SelectionFont = new Font(FontFamily.GenericSerif, 9, FontStyle.Regular);
            vars.VARS.Chat.richtbox[outId].SelectionColor = Color.Black;
            vars.VARS.Chat.richtbox[outId].SelectedText = "\r\n" + text + "\r\n";
            vars.VARS.Chat.richtbox[outId].SelectionStart = vars.VARS.Chat.richtbox[outId].Text.Length;
            vars.VARS.Chat.richtbox[outId].SelectionLength = 0;
            vars.VARS.Chat.richtbox[outId].SelectionFont = new Font(FontFamily.GenericSerif, 4, FontStyle.Regular);
            vars.VARS.Chat.richtbox[outId].SelectedText = "\r\n";
            vars.VARS.Chat.richtbox[outId].ScrollToCaret();
        }

        /// <summary>
        /// Метод печатает сообщение в чат выбранного пользователя
        /// </summary>
        /// <param name="outId">Номер пользователя</param>
        /// <param name="text">Текст сообщения</param>
        /// <param name="name">Номер пользователя(для котогоро имя искать)</param>
        /// <param name="date">Дата сообщения</param>

        public static void printMsg(uint outId, string text, uint name, double date)
        {
            Color myColor;
            string chatName;

            text = text.Replace("<br>", "\n");
            text = text.Replace("&quot;", "\"");

            if (name == vars.VARS.Mid)
            {
                chatName = "Я";
                myColor = Color.FromArgb(0, 0, 255);
            }
            else
            {
                myColor = Color.FromArgb(250, 28, 33);
                if (vars.VARS.Contact.ContainsKey(name))
                    chatName = vars.VARS.Contact[name].UserName;
                else
                    chatName = name.ToString();
            }


            vars.VARS.Chat.richtbox[outId].SelectionStart = vars.VARS.Chat.richtbox[outId].Text.Length;
            vars.VARS.Chat.richtbox[outId].SelectionLength = 0;
            vars.VARS.Chat.richtbox[outId].SelectionFont = new Font(FontFamily.GenericSerif, 8, FontStyle.Bold);
            vars.VARS.Chat.richtbox[outId].SelectionColor = myColor;

            vars.VARS.Chat.richtbox[outId].SelectedText = chatName + " (" + ChatForm.ConvertFromUnixTimestamp(date).ToShortTimeString() + " " + ChatForm.ConvertFromUnixTimestamp(date).ToShortDateString() + ")";
            vars.VARS.Chat.richtbox[outId].SelectionStart = vars.VARS.Chat.richtbox[outId].Text.Length;
            vars.VARS.Chat.richtbox[outId].SelectionLength = 0;
            vars.VARS.Chat.richtbox[outId].SelectionFont = new Font(FontFamily.GenericSerif, 9, FontStyle.Regular);
            vars.VARS.Chat.richtbox[outId].SelectionColor = Color.Black;
            vars.VARS.Chat.richtbox[outId].SelectedText = "\r\n" + text + "\r\n";
            vars.VARS.Chat.richtbox[outId].SelectionStart = vars.VARS.Chat.richtbox[outId].Text.Length;
            vars.VARS.Chat.richtbox[outId].SelectionLength = 0;
            vars.VARS.Chat.richtbox[outId].SelectionFont = new Font(FontFamily.GenericSerif, 4, FontStyle.Regular);
            vars.VARS.Chat.richtbox[outId].SelectedText = "\r\n";
            vars.VARS.Chat.richtbox[outId].ScrollToCaret();
        }

        #endregion

        #region Добавление контактов в список
        
        /// <summary>
        /// Метод добавляет контакты в список
        /// </summary>

        public static void AddItem()
        {
            foreach (KeyValuePair<uint, vk.profile> item in vars.VARS.Contact) // Добавляем контакты в список контактов на главной форме
                myContactList.ContactList.Items.Add(item.Key);
            myContactList.ContactList.FinishUpdate(); // Обновляем настройки списка контактов
        }

        #endregion

        #region Проигрывание звука
        /// <summary>
        /// Проигрывание звука
        /// </summary>
        /// <param name="Event">Событие, к которому проиграть звук</param>
        public static void NotifySound(string Event)
        {
            switch (Event)
            {
                case "IncomeMessage": PlaySound(vars.VARS.Incoming_message, 1);
                    break;
                case "OutMessage": PlaySound(vars.VARS.Out_message, 1);
                    break;
                case "UserOnline": PlaySound(vars.VARS.User_online, 1);
                    break;
                case "UserOffline": PlaySound(vars.VARS.User_offline, 1);
                    break;
            }
        }
        #endregion

        #region Считывание частоты использования контактов

        /// <summary>
        /// Метод считывает из файла частоту использования контактов
        /// </summary>

        public static void tryGetFrequency()
        {
            try
            {
                System.IO.FileStream fs = new System.IO.FileStream(vars.VARS.Directory + "frequency.dat", System.IO.FileMode.Open, System.IO.FileAccess.Read);
                System.IO.StreamReader reader = new System.IO.StreamReader(fs, Encoding.UTF8);
                string line;
                string[] lines = new string[2];
                while (reader.Peek() != -1)
                {
                    line = reader.ReadLine();
                    lines = line.Split(':'); // Разделитель номера пользователя и частоты
                    vars.VARS.FrequencyUse.Remove(Convert.ToUInt32(lines[0]));
                    vars.VARS.FrequencyUse.Add(Convert.ToUInt32(lines[0]), Convert.ToUInt32(lines[1])); // Добавляем в словарь новую частоту
                }
            }
            catch (Exception exe)
            {
                GeneralMethods.WriteError(exe.Source, exe.Message, exe.TargetSite);
                MessageBox.Show("При считывании файла frequency.dat произошла ошибка!\nДанные ошибки были записаны в файл errors.txt", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Считывание настроек

        /// <summary>
        /// Метод считывает настройки из файла
        /// </summary>

        public static void tryGetSettings()
        {
            try
            {
                System.IO.FileStream fs = new System.IO.FileStream(vars.VARS.Directory + "settings.cfg", System.IO.FileMode.Open, System.IO.FileAccess.Read);
                System.IO.StreamReader reader = new System.IO.StreamReader(fs, Encoding.UTF8);
                string line;
                string[] lines = new string[2];
                while (reader.Peek() != -1)
                {
                    line = reader.ReadLine();
                    lines = line.Split('|');
                    switch (lines[0])
                    {
                        case "Sound": vars.VARS.Sound = Convert.ToBoolean(lines[1]);
                            break;
                        case "SaveSettings": vars.VARS.SaveSettings = Convert.ToBoolean(lines[1]);
                            break;
                        case "ShowOffline": vars.VARS.ShowOffline = Convert.ToBoolean(lines[1]);
                            break;
                        case "GetOfflineMsg": vars.VARS.GetOfflineMsg = Convert.ToBoolean(lines[1]);
                            break;
                        case "Frequency": vars.VARS.Frequency = Convert.ToBoolean(lines[1]);
                            break;
                        case "OtherFolder": vars.VARS.OtherFolder = Convert.ToBoolean(lines[1]);
                            break;
                        case "ExitOnCloser": vars.VARS.ExitOnCloser = Convert.ToBoolean(lines[1]);
                            break;
                        case "ExitVK": vars.VARS.ExitVK = Convert.ToBoolean(lines[1]);
                            break;
                        case "directory": vars.VARS.Directory = Convert.ToString(lines[1]);
                            break;
                        case "incoming_message_on": vars.VARS.Incoming_message_on = Convert.ToBoolean(lines[1]);
                            break;
                        case "out_message_on": vars.VARS.Out_message_on = Convert.ToBoolean(lines[1]);
                            break;
                        case "user_online_on": vars.VARS.User_online_on = Convert.ToBoolean(lines[1]);
                            break;
                        case "user_offline_on": vars.VARS.User_offline_on = Convert.ToBoolean(lines[1]);
                            break;
                        case "incoming_message": vars.VARS.Incoming_message = Convert.ToString(lines[1]);
                            break;
                        case "out_message": vars.VARS.Out_message = Convert.ToString(lines[1]);
                            break;
                        case "user_online": vars.VARS.User_online = Convert.ToString(lines[1]);
                            break;
                        case "user_offline": vars.VARS.User_offline = Convert.ToString(lines[1]);
                            break;
                        case "visual_notify": vars.VARS.Visual_notify = Convert.ToBoolean(lines[1]);
                            break;
                        case "notify_online": vars.VARS.Notify_online = Convert.ToBoolean(lines[1]);
                            break;
                        case "notify_offline": vars.VARS.Notify_offline = Convert.ToBoolean(lines[1]);
                            break;
                        case "notify_income": vars.VARS.Notify_income = Convert.ToBoolean(lines[1]);
                            break;
                        case "updateFriends": vars.VARS.UpdateFriends = Convert.ToBoolean(lines[1]);
                            break;
                    }
                }
            }
            catch (System.IO.FileNotFoundException exe)
            {
                // Если файл не найден все настройки просто по умолчанию
            }
            catch (Exception exe) // Иные ошибки записываем в файл и оповещаем
            {
                GeneralMethods.WriteError(exe.Source, exe.Message, exe.TargetSite);
                MessageBox.Show("При считывании файла settings.cfg произошла ошибка!\nДанные ошибки были записаны в файл errors.txt", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Поиск удалившихся друзей

        /// <summary>
        /// Метод ищет удалившихся из списка друзей с прошлого запуска
        /// </summary>

        public static void SearchFriends()
        {
            Dictionary<uint, string> delete = new Dictionary<uint, string>();
            string s;
            try
            {
                System.IO.StreamReader list = new System.IO.StreamReader(vars.VARS.Directory + "contact.last");
                s = list.ReadToEnd(); // Считываем прошлый список
                list.Close();
            }

            catch (System.IO.IOException exe)
            {
                GeneralMethods.WriteError(exe.Source, exe.Message, exe.TargetSite);
                MessageBox.Show("Не удалось прочитать файл!\nДанные ошибки записаны в errors.txt", "Ошибка чтения файла друзей!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Hashtable jsonResp = (Hashtable)JSON.JsonDecode(s);
            if (jsonResp != null)
            {
                ArrayList users_last = (ArrayList)jsonResp["response"];
                foreach (Hashtable item in users_last)
                {
                    if (vars.VARS.Contact.ContainsKey(Convert.ToUInt32(item["uid"]))) // Если нынешний список содержит просто идём дальше
                        continue;
                    delete.Add(Convert.ToUInt32(item["uid"]), Convert.ToString(item["first_name"]) + " " + Convert.ToString(item["last_name"])); // Добавляем в словарь удалившихся
                }

                if (delete.Count != 0) // Если количество удалённых друзей не 0, то открываем форму и показываем их
                {
                    DeleteFriends del = new DeleteFriends();
                    del.AddList(delete);
                    del.ShowDialog();
                }
            }
        }
        
        #endregion

        #region Запись ошибки в файл

        /// <summary>
        /// Метод записывает ошибки в файл
        /// </summary>
        /// <param name="Source">Источник</param>
        /// <param name="Message">Сообщение, которое описывает ошибку</param>
        /// <param name="TargetSite">Метод, в котором была ошибка</param>

        public static void WriteError(string Source, string Message, System.Reflection.MethodBase TargetSite)
        {
            FileStream stream = new FileStream(vars.VARS.Directory + "errors.txt", FileMode.Append, FileAccess.Write);
            StreamWriter writer = new StreamWriter(stream, Encoding.UTF8);
            writer.Write(DateTime.Now.ToLongTimeString() + " : ");
            writer.WriteLine(Source);
            writer.WriteLine(Message);
            writer.WriteLine(TargetSite);
            writer.Close();
            writer.Dispose();
            stream.Dispose();
        }

        /// <summary>
        /// Метод записывает ошибки в файл
        /// </summary>
        /// <param name="Source">Источник</param>
        /// <param name="Message">Сообщение, которое описывает ошибку</param>
        /// <param name="Info">Дополнительная информация</param>

        public static void WriteError(string Source, string Message, string Info)
        {
            FileStream stream = new FileStream(vars.VARS.Directory + "errors.txt", FileMode.Append, FileAccess.Write);
            StreamWriter writer = new StreamWriter(stream, Encoding.UTF8);
            writer.Write(DateTime.Now.ToLongTimeString() + " : ");
            writer.WriteLine(Source);
            writer.WriteLine(Message);
            writer.WriteLine(Info);
            writer.Close();
            writer.Dispose();
            stream.Dispose();
        }
        #endregion

        #region Обновление программы

        public static void CheckNewVersion()
        {
            WebClient web = new WebClient();
            bool flag = false;
            try
            {
                web = new WebClient();
                string verion = web.DownloadString("http://malstoun.ru/IMV");
                if (verion != vars.VARS.Version)
                    if (MessageBox.Show("Доступна новая версия программы!\nОбновить?", "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        flag = true;
            }
            catch (Exception exe)
            {
                GeneralMethods.WriteError(exe.Source, exe.Message, exe.TargetSite);
            }

            try
            {
                if (flag)
                {
                    web.DownloadFile("http://malstoun.ru/IMVSetup.exe", Environment.GetFolderPath(Environment.SpecialFolder.InternetCache) + "\\IMVSetup.exe");
                    System.Diagnostics.Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.InternetCache) + "\\IMVSetup.exe");
                    Application.Exit();
                }
            }
            catch (Exception exe)
            {
                GeneralMethods.WriteError(exe.Source, exe.Message, exe.TargetSite);
            }
        }


        #endregion
    }
}