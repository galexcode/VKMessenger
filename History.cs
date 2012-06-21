using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace IMV
{
    public partial class History : Form
    {
        int offset = 0;
        uint uid;
        string temp = "";

        public History()
        {
            InitializeComponent();
            webBrowser1.Navigated += new WebBrowserNavigatedEventHandler(webBrowser1_Navigated);
        }

        void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            if (e.Url.Fragment == "#more") // Определяем, что делать
                getHistory(uid, offset += 40);
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Получение истории
        /// </summary>
        /// <param name="uid">ID пользователя</param>
        /// <param name="offset">Смещение для выборки сообщений</param>

        protected internal void getHistory(uint uid, int offset)
        {
            this.uid = uid;
            vk start = new vk();
            string s,
                text, // Текст сообщения
                name;
            uint mid, // Номер сообщения
                outId,
                readstate; // ID отправителя

            double date; // Дата
            Hashtable jsonResp = new Hashtable(),
                data;

            s = start.getHistory(uid, 40, offset);

            if (s != "" && s.IndexOf("error") == -1)
            {
                StringBuilder sb = new StringBuilder(); // Объявляем "собиралку" строки
                sb.Append("<html><head>" +
                "<script type='text/javascript' src='http://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js'></script>"
                + "</head><body><table style='font-size: 12px'><tr><td bgcolor=#E1E7ED><center><a href='#more' font-size=12px>Загрузить ещё</a></center></td></tr>");

                jsonResp = (Hashtable)JSON.JsonDecode(s);
                ArrayList msg = (ArrayList)jsonResp["response"];
                int min = msg.Count >= 40 ? 40 : msg.Count - 1; // По умолчанию загружаем по 40 фотографий, если их меньше, то загружаем сколько есть
                for (int i = min; i > 0; i--)
                {
                    data = (Hashtable)msg[i];
                    text = Convert.ToString(data["body"]);
                    mid = Convert.ToUInt32(data["mid"]);
                    outId = Convert.ToUInt32(data["from_id"]);
                    date = Convert.ToDouble(data["date"]);
                    readstate = Convert.ToUInt32(data["read_state"]);

                    if (outId == vars.VARS.Mid)
                        name = "Я";
                    else
                        name = vars.VARS.Contact[outId].UserName;

                    sb.Append("<tr><td bgcolor=");
                    if (readstate != 1) // Выделение непрочитанных сообщений
                        sb.Append("#E1E7ED name=");
                    else
                        sb.Append("#FFFFFF name=");
                    sb.Append(mid + ">");
                    sb.Append("<font color=");
                    if (outId == vars.VARS.Mid) // Выбор цвета имени
                        sb.Append("'0000FF'>");
                    else
                        sb.Append("'FA1C21'>");
                    sb.Append(name);
                    sb.Append(" (");
                    sb.Append(ChatForm.ConvertFromUnixTimestamp(date).ToShortTimeString());
                    sb.Append(" ");
                    sb.Append(ChatForm.ConvertFromUnixTimestamp(date).ToShortDateString());
                    sb.Append(")");
                    sb.Append("</font><br>");
                    sb.Append(text);
                    sb.Append("<br></td></tr>");
                }
                temp = webBrowser1.DocumentText.Replace("<html><head>" +
                "<script type='text/javascript' src='http://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js'></script>"
                + "</head><body><table style='font-size: 12px'><tr><td bgcolor=#E1E7ED><center><a href='#more' font-size=12px>Загрузить ещё</a></center></td></tr>", ""); // Удаляем фрагмент заголовка

                temp = temp.Replace("</table></body></html>", ""); // Удаляем фрагмент подвала
                sb.Append(temp); // Пристыковываем предыдущие сообщение к новым
                sb.Append("</table></body></html>"); // Добавляем подвал         

                webBrowser1.DocumentText = sb.ToString(); // Загружаем в веббраузер  
            }
            else
            {
                MessageBox.Show("Не удалось получить историю! Попробуйте позднее!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
    }
}
