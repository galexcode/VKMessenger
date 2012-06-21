using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Text;
using System.Runtime.InteropServices;

namespace IMV
{

    public class vk
    {
        const string url = "https://api.vkontakte.ru/method/";
        const int HEIGHT_ITEM = 30; // высота контакта в списке контактов

        delegate void contactJob();
        delegate void history(uint id, string text, uint outId, double date);
        delegate void history1(uint id, int offset);
        delegate void newTab(uint uid, string userName, bool flag);
        delegate void Notify(string name, string text, uint id);

        public struct profile
        {
            public uint uid; // номер пользователя
            public string f_name; // Имя
            public string l_name; // Фамилия
            public bool online; // Статус на сайте
            public string photo; // Адрес фотографии

            public string UserName
            {
                get
                {
                    return f_name + " " + l_name;
                }
            }
        }

        public enum InternetConnectionState : int // необходимое перечисления для InternetGetConnectedState
        {
            INTERNET_CONNECTION_MODEM = 0x1,
            INTERNET_CONNECTION_LAN = 0x2,
            INTERNET_CONNECTION_PROXY = 0x4,
            INTERNET_RAS_INSTALLED = 0x10,
            INTERNET_CONNECTION_OFFLINE = 0x20,
            INTERNET_CONNECTION_CONFIGURED = 0x40
        }

        [DllImport("WININET", CharSet = CharSet.Auto)]
        static extern bool InternetGetConnectedState(ref InternetConnectionState lpdwFlags, int dwReserved); // проверка интернет соединения

        #region Отправка запроса на сервер

        /// <summary>
        /// Медот, отсылающий запрос к api
        /// </summary>
        /// <param name="str">Строка с параметрами запроса</param>
        /// <returns>Ответ сервера</returns>

        protected internal string vk_call(string str)
        {
            string s = "";
            string uri = url + str + "&access_token=" + vars.VARS.Token;

            InternetConnectionState flags = 0;
            bool InternetConnect = InternetGetConnectedState(ref flags, 0);

            if (InternetConnect)
            {
                try
                {
                    HttpWebRequest req = (HttpWebRequest)
                        WebRequest.Create(uri); // создааём

                    HttpWebResponse resp = (HttpWebResponse)
                        req.GetResponse(); //запрашиваем ответ

                    Stream jstr = resp.GetResponseStream(); // получаем поток

                    StreamReader reader = new StreamReader(jstr);
                    s = reader.ReadToEnd(); // читаем поток
                }

                catch (ProtocolViolationException exe)
                {
                    GeneralMethods.WriteError(exe.Source, exe.Message, exe.TargetSite);
                    MessageBox.Show("Неудачное соединение с сервером!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (ObjectDisposedException exe)
                {
                    GeneralMethods.WriteError(exe.Source, exe.Message, exe.TargetSite);
                }
                catch (IOException exe)
                {
                    GeneralMethods.WriteError(exe.Source, exe.Message, exe.TargetSite);
                }
                catch (WebException exe)
                {
                    GeneralMethods.WriteError(exe.Source, exe.Message, exe.TargetSite);
                }
                if (s.IndexOf("\"error_code\":14") != -1)
                {
                    Hashtable hash1 = (Hashtable)JSON.JsonDecode(s);
                    Hashtable hash = (Hashtable)hash1["error"];
                    string captcha_sid = Convert.ToString(hash["captcha_sid"]);
                    string captcha_img = Convert.ToString(hash["captcha_img"]);
                    WebClient web = new WebClient();
                    web.DownloadFile(captcha_img, Environment.GetFolderPath(Environment.SpecialFolder.InternetCache) + "\\captch.img");
                    Image captcha = Image.FromFile(Environment.GetFolderPath(Environment.SpecialFolder.InternetCache) + "\\captch.img");
                    Captcha capt = new Captcha();
                    capt.pictureBox1.Image = captcha;
                    if (capt.ShowDialog() == DialogResult.OK)
                    {
                        str += "&captcha_sid=" + captcha_sid + "&captcha_key=" + capt.textBox1.Text;
                        s = vk_call(str);
                    }
                }
                return s;
            }
            else
            {
                GeneralMethods.WriteError("IMV", "Отсутствует соединения с интернет", "vk_call");
                return "error";
            }
        }

        #endregion

        #region Получение базовой информации о пользователях

        /// <summary>
        /// Метод, возвращающий массив пользовательского типа
        /// profile с полями uid, f_name, l_name, online.
        /// Создаёт в папке с программой файл списка контактов.
        /// </summary>

        public void getProfiles()
        {
            string s = "";
            StreamReader list;
            WebClient webClient = new WebClient();          
            Hashtable jsonResp = new Hashtable();
            profile userProf = new profile();
            vars.VARS.SmallPhoto.ColorDepth = ColorDepth.Depth32Bit;
            vars.VARS.SmallPhoto.ImageSize = new Size(26, 26);

            WebClient user = new WebClient();
            string uri = url + "friends.get?" + "fields=uid,first_name,last_name,online,photo" + "&access_token=" + vars.VARS.Token;

            try
            {
                if (!Directory.Exists(vars.VARS.Directory))
                {
                    Directory.CreateDirectory(vars.VARS.Directory);
                }
                if (vars.VARS.UpdateFriends)
                    if (File.Exists(vars.VARS.Directory + "сontact.pro"))
                    {
                        if (File.Exists(vars.VARS.Directory + "contact.last"))
                            File.Delete(vars.VARS.Directory + "contact.last");
                        File.Move(vars.VARS.Directory + "сontact.pro", vars.VARS.Directory + "contact.last");
                    }

                user.DownloadFile(uri, vars.VARS.Directory + "сontact.pro"); // загружаем файл со списком контактов
                list = new StreamReader(vars.VARS.Directory + "сontact.pro");
                s = list.ReadToEnd(); // читаем файл
                list.Close();
            }

            catch (WebException exe)
            {
                GeneralMethods.WriteError(exe.Source, exe.Message, exe.TargetSite);
                MessageBox.Show("Не удалось загрузить с сервера список контактов! Проверьте подключение!\nДанные ошибки записаны в errors.txt", "Ошибка сети!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (s != "" && s.IndexOf("error") == -1)
            {
                jsonResp = (Hashtable)JSON.JsonDecode(s);
                ArrayList users = (ArrayList)jsonResp["response"];
                foreach (Hashtable item in users)
                {
                    userProf.uid = Convert.ToUInt32(item["uid"]);
                    userProf.f_name = Convert.ToString(item["first_name"]);
                    userProf.l_name = Convert.ToString(item["last_name"]);
                    userProf.online = Convert.ToBoolean(item["online"]);
                    userProf.photo = Convert.ToString(item["photo"]);
                    vars.VARS.Contact.Add(userProf.uid, userProf);
                    if (vars.VARS.Frequency)
                        vars.VARS.FrequencyUse.Add(userProf.uid, 0);
                }

                if (!Directory.Exists(vars.VARS.Directory))
                    Directory.CreateDirectory(vars.VARS.Directory + "photo");
            }
        }

        /// <summary>
        /// Получаение статуса пользователя на сайте (онлайн, оффлайн)
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <returns>Статус (true-онлайн,false-оффлайн)</returns>

        public bool getStatus(uint uid)
        {
            bool status = false;
            string uri = "getProfiles?" + "fields=online&uids=" + uid;
            string s = vk_call(uri);
            if (s != "" && s.IndexOf("error") == -1)
            {
                Hashtable jsonResp = (Hashtable)JSON.JsonDecode(s);
                ArrayList data = (ArrayList)jsonResp["response"];
                status = true;
            }
            return status;
        }

        #endregion

        #region Загрузка онлайн пользователей

        /// <summary>
        /// Метод для получения онлайн пользователей
        /// </summary>

        public void getOnline()
        {
            string uri = "friends.getOnline?";

            string s = vk_call(uri);

            Hashtable resp = (Hashtable)JSON.JsonDecode(s);
            ArrayList data = (ArrayList)resp["response"];

            for (int i = 0; i < data.Count; i++)
            {
                vk.profile user;
                uint id = Convert.ToUInt32(data[i]);
                user = vars.VARS.Contact[id];
                user.online = true;
                vars.VARS.Contact.Remove(id);
                vars.VARS.Contact.Add(id, user);
            }
        }

        #endregion

        #region Загрузка маленьких фотографий для листа контактов

        /// <summary>
        /// Загрузка иконок
        /// </summary>

        public void getIcon()
        {
            if (!Directory.Exists(vars.VARS.Directory + "photo\\"))
                Directory.CreateDirectory(vars.VARS.Directory + "photo\\");
            WebClient webClient = new WebClient();
            Image newImage;
            contactJob Refresh = new contactJob(myContactList.ContactList.Refresh);
            SortedDictionary<uint, profile> temp = new SortedDictionary<uint,profile>(vars.VARS.Contact);
            foreach (KeyValuePair<uint, profile> item in temp)
            {
                try
                {
                    webClient.DownloadFile(item.Value.photo, vars.VARS.Directory + "photo\\" + item.Value.uid);
                    newImage = Image.FromFile(vars.VARS.Directory + "photo\\" + item.Value.uid, true);
                    vars.VARS.SmallPhoto.Images.Add(item.Key.ToString(), newImage);
                    if ((myContactList.ContactList.Items.IndexOf(item.Key) >= myContactList.ContactList.Offset / HEIGHT_ITEM) &
                        (myContactList.ContactList.Items.IndexOf(item.Key) <= (myContactList.ContactList.Offset / HEIGHT_ITEM + myContactList.ContactList.Height / HEIGHT_ITEM))) // если контакт видим, перерисовываем, если нет, то незачем обновлять лишний раз
                        myContactList.ContactList.Invoke(Refresh);
                }
                catch (WebException exe)
                {
                    GeneralMethods.WriteError(exe.Source, exe.Message, exe.TargetSite);
                    vars.VARS.SmallPhoto.Images.Add(item.Key.ToString(), Properties.Resources.redBall);
                }
            }
        }

        #endregion

        #region Получение подробной информации о пользователе

        /// <summary>
        /// Получение подробной информации о пользователе
        /// </summary>
        /// <param name="uid">ID пользователя</param>
        /// <param name="size">Размер фотографии, получаемой с главной страницы (small, medium, ~large)</param>
        /// <returns></returns>

        public Hashtable getInfo(uint uid)
        {
            string s = "";

            string uri = "getProfiles?fields=uid,first_name,last_name,bdate,city,country,contacts&format=JSON&uids=" + uid;

            s = vk_call(uri);

            Hashtable info = (Hashtable)JSON.JsonDecode(s);

            return info;
        }

        #endregion

        #region Получение фотографии пользователя

        /// <summary>
        /// Получение фотографии пользователя с его страницы
        /// </summary>
        /// <param name="uid">ID пользоателя</param>
        /// <param name="size">Размер фотографии (small, medium)</param>
        /// <returns></returns>

        public Image getPhoto(uint uid, string size)
        {
            string s = "";
            ArrayList arrResp;
            Hashtable info;

            WebClient user = new WebClient();
            string uri = "getProfiles?fields=photo_" + size + "&format=JSON&uids=" + uid;
            Image image = null;
            s = vk_call(uri);
            if (s != "" && s.IndexOf("error") == -1)
            {
                Hashtable resp = (Hashtable)JSON.JsonDecode(s);
                arrResp = (ArrayList)resp["response"];
                info = (Hashtable)arrResp[0];
                string foto = Convert.ToString(info["photo_" + size]);

                try
                {
                    if (!Directory.Exists(vars.VARS.Directory + size + "\\"))
                        Directory.CreateDirectory(vars.VARS.Directory + size + "\\");
                    user.DownloadFile(foto, vars.VARS.Directory + size + "\\" + uid);
                    image = Image.FromFile(vars.VARS.Directory + size + "\\" + uid);
                }
                catch (WebException exe)
                {
                    GeneralMethods.WriteError(exe.Source, exe.Message, exe.TargetSite);
                }
            }

            return image;
        }

        public void getPhoto(List<uint> uids, string size)
        {
            string s = "";
            string zap = "";
            string separ;
            ArrayList arrResp;
            contactJob Refresh = new contactJob(myContactList.ContactList.Refresh);

            if (uids.Count == 1)
                separ = "";
            else separ = ",";

            foreach (uint item in uids)
                zap = item.ToString() + separ + zap;

            WebClient user = new WebClient();
            string uri = "getProfiles?fields=" + size + "&format=JSON&uids=" + zap;

            s = vk_call(uri);

            if (!Directory.Exists(vars.VARS.Directory + size + "\\")) // если для такого размера ещё нет папки, создаём...
                Directory.CreateDirectory(vars.VARS.Directory + size + "\\");

            if (s != "" && s.IndexOf("error") == -1)
            {
                Hashtable resp = (Hashtable)JSON.JsonDecode(s);
                arrResp = (ArrayList)resp["response"];
                foreach (Hashtable item in arrResp)
                {
                    try
                    {
                        user.DownloadFile(Convert.ToString(item["photo"]), vars.VARS.Directory + size + "\\" + Convert.ToString(item["uid"])); // загружаем
                        if (vars.VARS.SmallPhoto.Images.ContainsKey(Convert.ToString(item["uid"])))
                        { // добавляем новое изображение к контакту
                            vars.VARS.SmallPhoto.Images.RemoveByKey(Convert.ToString(item["uid"]));
                            vars.VARS.SmallPhoto.Images.Add(Convert.ToString(item["uid"]), Image.FromFile(vars.VARS.Directory + size + "\\" + Convert.ToString(item["uid"])));
                        }
                        else
                            vars.VARS.SmallPhoto.Images.Add(Convert.ToString(item["uid"]), Image.FromFile(vars.VARS.Directory + size + "\\" + Convert.ToString(item["uid"])));
                    }
                    catch (WebException exe)
                    {
                        GeneralMethods.WriteError(exe.Source, exe.Message, exe.TargetSite);
                    }
                }
            }
            myContactList.ContactList.Invoke(Refresh); // обновляем контакт лист
        }

        #endregion

        #region Получение непрочитанных сообщений

        /// <summary>
        /// Получение непрочитанных сообщений
        /// </summary>

        public void messageGet()
        {
            string s;

            string uri = "messages.get?filters=1&format=JSON&preview_length=0&uids=" + vars.VARS.Mid;

            s = vk_call(uri);

            if (!(s.Length <= 15) && s.IndexOf("error") == -1)
            {
                Hashtable jsonResp = (Hashtable)JSON.JsonDecode(s);
                ArrayList arrResp = (ArrayList)jsonResp["response"];
                Hashtable msg = new Hashtable();
                uint uid, mid;
                contactJob show = new contactJob(vars.VARS.Chat.Show);

                for (int i = 0; i < Convert.ToInt32(arrResp[0]); i++)
                {
                    msg = (Hashtable)arrResp[i + 1];
                    uid = Convert.ToUInt32(msg["uid"]);
                    mid = Convert.ToUInt32(msg["mid"]);

                    if (vars.VARS.Chat.richtbox.ContainsKey(uid) == false)
                    {
                        newTab tab = new newTab(ChatForm.chat.chatIn);
                        vars.VARS.Chat.Invoke(tab, uid, vars.VARS.Contact[uid].UserName, true); //chatIn(uid, vars.VARS.Contact[uid].UserName, true);
                        history1 getHist = new history1(getHistory);
                        IAsyncResult res1 = getHist.BeginInvoke(uid, 10, null, null);
                        vars.VARS.Chat.Invoke(show);
                    }

                    if (vars.VARS.NumbMass.ContainsKey(uid))
                        vars.VARS.NumbMass[uid].Add(mid);
                    else
                    {
                        vars.VARS.NumbMass.Add(uid, new List<uint>());
                        vars.VARS.NumbMass[uid].Add(mid);
                    }
                }
                foreach (uint item in vars.VARS.NumbMass.Keys)
                    markAsRead(item);
                show = new contactJob(vars.VARS.Chat.Activate);
                vars.VARS.Chat.Invoke(show);               
            }
        }

        #endregion

        #region Получение информации о новых отметках на сайте

        public Hashtable getStat()
        {
            string uri = "getCounters?";
            string s = vk_call(uri);

            Hashtable resp = (Hashtable)JSON.JsonDecode(s);
            Hashtable counters = (Hashtable)resp["response"];

            return counters;
        }

        #endregion

        #region Статус

        /// <summary>
        /// Установка статуса пользователя
        /// </summary>
        /// <param name="text">Текст статуса</param>

        public string setStatus(string text)
        {
            string uri = "status.set?format=JSON&text=" + text + "&uids=" + vars.VARS.Mid;

            string s = vk_call(uri);
            return s;
        }

        #endregion

        #region Стена

        /// <summary>
        /// Метод получает записи со стены
        /// </summary>
        /// <param name="count">Количество записей (не более 100)</param>
        /// <param name="filter">Чьи сообщения получать (owner, others, all)</param>
        /// <returns>Строка, содержащая статусы</returns>

        public string GetWall(int count, string filter)
        {
            string uri = "wall.get?count=" + count + "&filter=" + filter;
            string s = vk_call(uri);
            return s;
        }

        /// <summary>
        /// Метод создаёт запись на стене
        /// </summary>
        /// <param name="text">Текст записи</param>

        public string SetWallStatus(string text)
        {
            string uri = "wall.post?message=" + text;
            string s = vk_call(uri);
            return s;
        }
        
        /// <summary>
        /// Метод создаёт запись на стене
        /// </summary>
        /// <param name="text">Текст записи</param>
        /// <param name="attach">Имя файла (e.x. photo123456_884693)</param>
        /// <param name="uid">ID пользователя, которому надо отправить запись (0 - себе)</param>

        public string SetWallStatus(string text, string attach, uint uid)
        {
            string uri;
            if (uid != 0) // не себе
                uri = "wall.post?owner_id=" + uid + "&message=" + text + "&attachment=" + attach;
            else // себе
                uri = "wall.post?message=" + text + "&attachment=" + attach;
            string s = vk_call(uri);
            return s;
        }

        private string getAdressWall(uint uid)
        { // Получаем адрес для отправки изображения
            string s;
            string uri;
            if (uid != 0)
                uri = "photos.getWallUploadServer?" + "uid=" + uid;
            else
                uri = "photos.getWallUploadServer?";

            s = vk_call(uri);
            if (s != "" && s.IndexOf("error") == -1)
            {
                Hashtable jsonResp = (Hashtable)JSON.JsonDecode(s);
                Hashtable adress = (Hashtable)jsonResp["response"];
                s = Convert.ToString(adress["upload_url"]);
                return s;
            }
            else
                return "";
        }

        /// <summary>
        /// Загрузка файла на сервер
        /// </summary>
        /// <param name="uid">ID пользователя, которому надо загрузить файл (0 - себе)</param>
        /// <returns>Строка с именем загруженного файла</returns>

        public string UploadFile(uint uid, string path)
        {
            string url = getAdressWall(uid);
            string contenttype;

            WebResponse result = null;
            Stream newStream = null;
            StreamReader sr = null;
            string server = ""; // адрес сервера
            string photoName = ""; // имя фото
            string hash = ""; // хэш
            try
            {
                if (path == null)
                {
                    return "";
                }
                //----Формируем POST запрос------
                string boundary = "----------" + DateTime.Now.Ticks.ToString("x"); // строка разделитель для подразделов
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url); // создаём запрос на адрес
                req.ContentType = "multipart/form-data; boundary=" + boundary;
                req.Method = "POST"; // метод

                contenttype = path.IndexOf("jpg") == -1 ? path.IndexOf("gif") == -1 ?
                    (path.IndexOf("png") == -1 ? "" : "image/png") : "image/gif" : "image/jpg"; // какой тип изображения. При проверке на сервере несходство влияет на ответ

                StringBuilder sb = new StringBuilder(); // строкособиратель
                sb.Append("--");
                sb.Append(boundary);
                sb.Append("\r\n");
                sb.Append("Content-Disposition: form-data; name=");
                sb.Append("photo"); // название поля по регламенту API
                sb.Append("; filename=\"");
                sb.Append(Path.GetFileName(path));
                sb.Append("\"");
                sb.Append("\r\n");
                sb.Append("Content-Type: ");
                sb.Append(contenttype);
                sb.Append("\r\n");
                sb.Append("\r\n");

                string postHeader = sb.ToString(); // заголовок создан
                byte[] postHeaderBytes = Encoding.UTF8.GetBytes(postHeader); //считаем количество байт...

                byte[] boundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n"); // считаем...

                FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);

                long length = postHeaderBytes.Length + fileStream.Length + boundaryBytes.Length; // складываем заголовок, файл, разделитель

                req.ContentLength = length; // присваиваем длину запроса

                newStream = req.GetRequestStream(); // пытаемся получить поток

                newStream.Write(postHeaderBytes, 0, postHeaderBytes.Length); // пишем в поток заголовок

                byte[] buffer = new Byte[checked((uint)Math.Min(4096, (int)fileStream.Length))];
                int bytesRead = 0;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0) // пишем в поток файл
                    newStream.Write(buffer, 0, bytesRead);

                newStream.Write(boundaryBytes, 0, boundaryBytes.Length); // пишем в поток разделитель
                //---сформировали
                WebResponse resp = req.GetResponse(); // пытаемся получить ответ
                Stream s = resp.GetResponseStream(); // получаем поток в ответ
                sr = new StreamReader(s); //переводим
                Char[] read = new Char[256]; // в

                int count = sr.Read(read, 0, 256); // читаемый формат

                string strOut = "";

                while (count > 0) // записываем строку
                {
                    String str = new String(read, 0, count);
                    strOut += str;
                    count = sr.Read(read, 0, 256);
                }
                // парсим строку
                if (strOut.IndexOf("error") == -1)
                {
                    Hashtable respon = (Hashtable)JSON.JsonDecode(strOut);
                    server = Convert.ToString(respon["server"]);
                    photoName = Convert.ToString(respon["photo"]);
                    hash = Convert.ToString(respon["hash"]);

                    string uri;
                    if (uid != 0) // не себе на стену
                        uri = "photos.saveWallPhoto?server=" + server + "&photo=" + photoName + "&hash=" + hash + "&uid=" + uid;
                    else // себе
                        uri = "photos.saveWallPhoto?server=" + server + "&photo=" + photoName + "&hash=" + hash;
                    string strResp = vk_call(uri); // отправляем запрос на сохранения файла на сервере
                    if (strResp.IndexOf("error") == -1)
                    {
                        // парсим ответ
                        respon = (Hashtable)JSON.JsonDecode(strResp);
                        ArrayList arr = (ArrayList)respon["response"];
                        respon = (Hashtable)arr[0];
                        photoName = Convert.ToString(respon["id"]); // имя, которое необходимо указать при отсылке поста
                    }
                }

                return photoName;
            }

            catch (Exception exe)
            {
                GeneralMethods.WriteError(exe.Source, exe.Message, exe.TargetSite);
                return "";
            }

            finally
            {
                if (newStream != null)
                    newStream.Close();
                if (sr != null)
                    sr.Close();
                if (result != null)
                    result.Close();
            }
        }

        #endregion

        #region Документы

        public string UploadDocument(uint uid)
        {
            string uri = "document.Send";

            string s = vk_call(uri);

            Hashtable jsonResp = new Hashtable();
            jsonResp = (Hashtable)JSON.JsonDecode(s);
            ArrayList uploadString = (ArrayList)jsonResp["response"];

            string uploadString;
            return uploadString;
        }

        #endregion

        #region Установка флагов сообщениям

        /// <summary>
        /// Установка сообщениям флага "прочитан"
        /// </summary>
        /// <param name="uid">Номер пользователя</param>

        public void markAsRead(uint uid)
        {
            string mid = "";
            string separ;
            if (vars.VARS.NumbMass.Count != 0)
            {
                if (vars.VARS.NumbMass.ContainsKey(uid))
                {
                    if (vars.VARS.NumbMass[uid].Count == 1)
                        separ = "";
                    else separ = ",";

                    foreach (uint item in vars.VARS.NumbMass[uid])
                        mid = mid + separ + Convert.ToString(item); // формируем строку из номеров сообщений

                    string uri = "messages.markAsRead?format=JSON&mids=" + mid;

                    vk_call(uri);
                    vars.VARS.NumbMass[uid].Clear();
                }
            }
        }

        /// <summary>
        /// Установка сообщению флага "прочитан"
        /// </summary>
        /// <param name="mid">Номер сообщения</param>

        public void markAsReadMsg(uint mid)
        {
            string uri = "messages.markAsRead?format=JSON&mids=" + mid;

            vk_call(uri);
        }

        #endregion

        #region История сообщений

        /// <summary>
        /// Получение истории сообщений с пользователем
        /// </summary>
        /// <param name="uid">ID пользователя</param>
        /// <param name="count">Количество сообщений (не более 100 за раз)</param>

        public void getHistory(uint uid, int count)
        {
            string s,
                text; // Текст сообщения
            uint mid, // Номер сообщения
                outId; // ID отправителя
            double date; // Дата
            Hashtable jsonResp = new Hashtable(),
                data;

            string uri = "messages.getHistory?count=" + count + "&format=JSON&uid=" + uid;


            s = vk_call(uri);
            if (s != "" && s.IndexOf("error") == -1)
            {
                jsonResp = (Hashtable)JSON.JsonDecode(s);
                ArrayList msg = (ArrayList)jsonResp["response"];
                if (Convert.ToUInt32(msg[0]) < count)
                    count = Convert.ToInt32(msg[0]);
                history getHist = new history(GeneralMethods.printMsg);

                for (int i = count; i > 0; i--)      // Начинаем с конца, потому что сервер возвращает сообщения от текущей даты и далее
                {
                    data = (Hashtable)msg[i];
                    text = Convert.ToString(data["body"]);
                    mid = Convert.ToUInt32(data["mid"]);
                    outId = Convert.ToUInt32(data["from_id"]);
                    date = Convert.ToDouble(data["date"]);
                    if (!ChatForm.chat.textbox.ContainsKey(uid))
                        break;
                    ChatForm.chat.Invoke(getHist, uid, text, outId, date);
                }
            }
            else
                return;
        }

        public string getHistory(uint uid, int count, int offset)
        {
            string s;

            string uri = "messages.getHistory?count=" + count + "&format=JSON&uid=" + uid + "&offset=" + offset;

            s = vk_call(uri);

            return s;
        }

        #endregion

        #region Отправка сообщения
        
        /// <summary>
        /// Метод, отправляющий сообщение на сервер
        /// </summary>
        /// <param name="msg">Текст сообщения</param>
        /// <param name="id">ID получателя</param>
        /// <returns>True - сообщение отправлено, else - иначе</returns>

        public bool sendMsg(string msg, uint id)
        {
            string s = "";

            msg = msg.Replace("&", "amp;");

            string uri = "messages.send?format=JSON&message=" + msg + "&type=1&uid=" + id;
            s = vk_call(uri);

            if (!(s.IndexOf("response") == -1))
                return true;
            else return false;
        }

        #endregion
    }
}