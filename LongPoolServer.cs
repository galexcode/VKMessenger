using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;

namespace IMV
{
    public delegate void ChangeStatusEventHandler(ChangeStatusArgs info);
    public delegate void IncomingMessageEventHandler(IncomingMessageArgs info);

    public class ChangeStatusArgs : EventArgs
    {
        public readonly uint id;
        public readonly ushort flag;

        public ChangeStatusArgs(uint id, ushort flag)
        {
            this.id = id;
            this.flag = flag;
        }
    }

    public class IncomingMessageArgs : EventArgs
    {
        public readonly uint MessageID, flag, outID;
        public readonly string subj, text;
        public readonly double date;

        public IncomingMessageArgs(uint MessageID, uint flag, uint outID, double date, string subj, string text)
        {
            this.MessageID = MessageID;
            this.flag = flag;
            this.outID = outID;
            this.date = date;
            this.subj = subj;
            this.text = text;
        }
    }

    class RefreshContactList
    {
        const byte HEIGHT_ITEM = 30;
        delegate void refresh();

        public RefreshContactList(LongPoolServer server)
        {
            server.NewStatus += new ChangeStatusEventHandler(OnOffStatus);
        }

        #region Обновление списка контактов

        /// <summary>
        /// Обновление онлайн/оффлайн списка
        /// </summary>
        /// <param name="name">ID пользователя</param>
        /// <param name="stat">Статус пользователя (онлайн/оффлайн)</param>

        public void OnOffStatus(ChangeStatusArgs info)
        {
            uint name = info.id;
            ushort stat = info.flag;

            vk.profile temp;
            int last, now;
            if (stat == 1) // Из оффлайна в онлайн
            {
                try
                {
                    temp = vars.VARS.Contact[name];
                    temp.online = true;
                    vars.VARS.Contact.Remove(name);
                    vars.VARS.Contact.Add(name, temp);
                    last = myContactList.ContactList.Items.IndexOf(name);
                    myContactList.ContactList.Sort();
                    now = myContactList.ContactList.Items.IndexOf(name);
                    if ((now < (myContactList.ContactList.Offset / HEIGHT_ITEM + myContactList.ContactList.Height / HEIGHT_ITEM)) ||
                        ((last < (myContactList.ContactList.Offset / HEIGHT_ITEM + myContactList.ContactList.Height / HEIGHT_ITEM)))) // Проверяем видим ли элемент, если да, перерисовываем
                    {
                        refresh UpdateContact = new refresh(myContactList.ContactList.Refresh);
                        myContactList.ContactList.Invoke(UpdateContact);
                    }
                    if (vars.VARS.User_online_on && vars.VARS.Sound)
                        GeneralMethods.NotifySound("UserOnline");
                }
                catch (ArgumentNullException exe)
                {
                    GeneralMethods.WriteError(exe.Source, exe.Message, exe.TargetSite);
                }
            }
            else // Из онлайна в оффлайн
            {
                try
                {
                    temp = vars.VARS.Contact[name];
                    temp.online = false;
                    vars.VARS.Contact.Remove(name);
                    vars.VARS.Contact.Add(name, temp);
                    last = myContactList.ContactList.Items.IndexOf(name);
                    myContactList.ContactList.Sort();
                    now = myContactList.ContactList.Items.IndexOf(name);
                    if ((last > myContactList.ContactList.Offset / HEIGHT_ITEM) &
                        (last < (myContactList.ContactList.Offset / HEIGHT_ITEM + myContactList.ContactList.Height / HEIGHT_ITEM)) ||
                        (now > myContactList.ContactList.Offset / HEIGHT_ITEM) &
                        (now < (myContactList.ContactList.Offset / HEIGHT_ITEM + myContactList.ContactList.Height / HEIGHT_ITEM)))
                    {
                        refresh UpdateContact = new refresh(myContactList.ContactList.Refresh);
                        myContactList.ContactList.Invoke(UpdateContact);
                    }
                    if (vars.VARS.User_offline_on && vars.VARS.Sound)
                        GeneralMethods.NotifySound("UserOffline");
                }
                catch (ArgumentNullException exe)
                {
                    GeneralMethods.WriteError(exe.Source, exe.Message, exe.TargetSite);
                }
            }
        }

        #endregion
    }

    class IncomingMessage
    {
        delegate void AccessChat(uint msgId, uint flat, uint outId, double date, string subj, string text, vk start);

        public IncomingMessage(LongPoolServer server)
        {
            server.NewMessage += new IncomingMessageEventHandler(server_NewMessage);
        }

        #region Входящее сообщение

        /// <summary>
        /// Метод, обрабатывающий получение сообщения
        /// </summary>
        /// <param name="msgId">ID сообщения</param>
        /// <param name="flag">Флаг сообщения (суммирование определённых флажков)</param>
        /// <param name="outId">ID отправителя</param>
        /// <param name="date">Дата в UNIX формате</param>
        /// <param name="subj">Тема сообщения</param>
        /// <param name="text">Текст сообщения</param>


        void server_NewMessage(IncomingMessageArgs info)
        {
            uint msgId = info.MessageID;
            uint flag = info.flag;
            uint outId = info.outID;
            double date = info.date;
            string subj = info.subj;
            string text = info.text;
            vk start = new vk();

            AccessChat NewAccess = new AccessChat(Processing);
            vars.VARS.Chat.Invoke(NewAccess, msgId, flag, outId, date, subj, text, start);
        }

        void Processing(uint msgId, uint flag, uint outId, double date, string subj, string text, vk start)
        {
            if ((flag & 2) != 2) // Если не содержит флаг 2 (исходящее)
            {
                string name = null;

                if ((flag & 32) == 32) // Выбираем какое имя (Если 32, то друг, значит должен быть в списке)
                    name = vars.VARS.Contact[outId].UserName;
                else
                    name = outId.ToString();

                if (vars.VARS.Chat.richtbox.ContainsKey(outId)) // Если содержит, значит вкладка была открыта, значит печатаем
                {
                    GeneralMethods.printMsg(outId, text, name, date);
                    start.markAsReadMsg(msgId);
                }
                else
                {
                    bool flag1;
                    if (vars.VARS.Chat.WindowState == FormWindowState.Minimized)
                    {
                        flag1 = true;
                        vars.VARS.Chat.WindowState = FormWindowState.Normal;
                    }
                    else
                        flag1 = false;

                    vars.VARS.Chat.chatIn(outId, name, flag1); // Создаём новую вкладку

                    if (!vars.VARS.Chat.Visible) // Если невидимый чат - делаем видимым
                    {
                        vars.VARS.Chat.Show();
                        vars.VARS.Chat.Activate();
                        start.markAsReadMsg(msgId);
                    }
                    else // Или просто активируем
                        vars.VARS.Chat.Activate();

                    GeneralMethods.printMsg(outId, text, name, date); // Печатаем сообщение                    
                }

                if ((flag & 3) != 3) // Если не "исходящее непрочитанное"
                {
                    if (vars.VARS.NumbMass.ContainsKey(outId)) // Если содержит ключ, добавляем новый айди сообщения
                        vars.VARS.NumbMass[outId].Add(msgId);
                    else // Иначе новый ключ и добавляем айди
                    {
                        vars.VARS.NumbMass.Add(outId, new List<uint>());
                        vars.VARS.NumbMass[outId].Add(msgId);
                    }
                }

                if (vars.VARS.Incoming_message_on && vars.VARS.Sound) // Если разрешено играть музыку, играем в другом потоку
                {
                    GeneralMethods.NotifySound("IncomeMessage");
                }

                if (vars.VARS.Frequency)
                {
                    if (vars.VARS.FrequencyUse.ContainsKey(outId))
                    {
                        uint j = vars.VARS.FrequencyUse[outId];
                        j++;
                        vars.VARS.FrequencyUse.Remove(outId);
                        vars.VARS.FrequencyUse.Add(outId, j);
                    }
                }
            }
            else if ((flag & 16) != 16)
            {
                if (vars.VARS.Chat.richtbox.ContainsKey(outId))
                    GeneralMethods.printMsg(outId, text, vars.VARS.Mid.ToString(), date);
            }
        }

        #endregion
    }

    class LongPoolServer
    {
        Dictionary<string, string> par = new Dictionary<string, string>();

        vk start = new vk();
        bool flagConnect = true;

        public event ChangeStatusEventHandler NewStatus;
        public event IncomingMessageEventHandler NewMessage;


        #region Получение подключение к серверу сообщений

        /// <summary>
        /// Получение ключа, адреса и чего-то ещё для подключения
        /// </summary>

        public void getLongPollServer()
        {
            string s = "";
            string value = "";
            RefreshContactList upd = new RefreshContactList(this);
            IncomingMessage incm = new IncomingMessage(this);

            do
            {
                string uri = "messages.getLongPollServer?";

                s = start.vk_call(uri); // Отправляем запрос, получаем ответ

                par.Clear(); // Очищаем словарь

                if (s.IndexOf("error") == -1 && s != "") // Если нет ошибок, парсим
                {
                    s = s.Remove(0, 13);
                    s = s.Remove(s.Length - 2);
                    s = s.Replace("\"", "");
                    value = s.Substring(s.IndexOf(":") + 1, s.IndexOf(",") - s.IndexOf(":") - 1);
                    par.Add("key", value); // Добавляем ключ
                    s = s.Remove(0, s.IndexOf(",") + 1);
                    value = s.Substring(s.IndexOf(":") + 1, s.IndexOf(",") - s.IndexOf(":") - 1);
                    value = value.Replace("\\", "");
                    par.Add("server", value); // Добавляем адрес сервера
                    s = s.Remove(0, s.IndexOf(",") + 1);
                    value = s.Substring(s.IndexOf(":") + 1);
                    par.Add("ts", value); // Уник
                }
                else // Если ошибка вылетаем...в будущем дописать!
                {
                    break;
                }
            } while (LongPoll(par)); // Делаем запрос к пулу
        }

        /// <summary>
        /// Подключение к серверу с помощью полученнх значений ключа, имени сервера и ещё чего-то
        /// </summary>
        /// <param name="par">Словарь значений</param>
        /// <returns></returns>

        public bool LongPoll(Dictionary<string, string> par)
        {
            string s = "";
            string uri = "";
            string value = "";
            Hashtable jsonResp = new Hashtable();

            do
            {
                uri = "http://" + par["server"] + "?act=a_check&key=" + par["key"] + "&ts=" + par["ts"] + "&wait=25";
                try
                {
                    HttpWebRequest req = (HttpWebRequest)
                        WebRequest.Create(uri); // Создаём запрос

                    HttpWebResponse resp = (HttpWebResponse)
                        req.GetResponse(); // Запрашиваем ответ

                    Stream jstr = resp.GetResponseStream(); // Получаем ответный поток
                    TextReader reader = new StreamReader(jstr, true); // Читаем поток
                    s = reader.ReadToEnd(); // Переводим к строке

                    if (!flagConnect)
                    {
                        try
                        {
                            start.getOnline();
                            flagConnect = true;
                        }
                        catch
                        {
                            // Ошибка при обновлении контактов
                        }
                    }

                    if (!(s.IndexOf("failed") == -1)) // Если ошибка, то пробуем перезапросить ключ
                        return true;

                    if (s != "" && s.IndexOf("error") == -1)
                    {
                        jsonResp = (Hashtable)JSON.JsonDecode(s); // Парсим
                        value = jsonResp["ts"].ToString();
                        par.Remove("ts");
                        par.Add("ts", value); // Добавляем новый уник

                        ArrayList updates = (ArrayList)jsonResp["updates"];
                        if (updates.Count == 0) continue;
                        foreach (ArrayList item in updates) // Обработка сообщений
                        {
                            switch (Convert.ToUInt32(item[0]))
                            {
                                case 4:
                                    {
                                        IncomingMessageArgs info = new IncomingMessageArgs(
                                            Convert.ToUInt32(item[1]),
                                            Convert.ToUInt32(item[2]),
                                            Convert.ToUInt32(item[3]),
                                            Convert.ToUInt64(item[4]),
                                            item[5].ToString(),
                                            item[6].ToString()
                                        );
                                        NewMessage(info);
                                    } break; // Новое сообщение [4,message_id,flags,from_id,timestamp,subject,text]

                                case 8:
                                    {
                                        ChangeStatusArgs info = new ChangeStatusArgs((UInt32)(0 - Convert.ToInt32(item[1])), (ushort)1);
                                        NewStatus(info);
                                    } break; // Пользователь вышел в онлайн [8, -userid]

                                case 9:
                                    {
                                        ChangeStatusArgs info = new ChangeStatusArgs((UInt32)(0 - Convert.ToInt32(item[1])), (ushort)0);
                                        NewStatus(info);
                                    } break; // Статус пользователя изменился на оффлайн ( у второго параметра есть 2 флага! ДОРАБОТАТЬ! (2 * Convert.ToUInt16(item[2])) )
                            }
                        }
                    }
                }

                catch (WebException exe)
                {
                    GeneralMethods.WriteError(exe.Source, exe.Message, exe.TargetSite);

                    if (flagConnect)
                    {
                        if (MessageBox.Show("Сервер сообщений перестал отвечать!\nНажмите OK, если хотите, чтобы программа попыталась исправить ошибку\nНажмите Отмена, если хотите выйти из программы", "Ошибка", MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.OK)
                        {
                            flagConnect = false;
                            continue;
                        }
                        else
                            Application.Exit();
                    }
                    else
                        Application.Restart();
                }
            } while (s.IndexOf("error") == -1);

            return false;
        }
        #endregion
    }
}
