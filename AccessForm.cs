using System;
using System.Drawing;
using System.Windows.Forms;
using System.Web;
using System.Net;
using System.IO;
using System.Text;

namespace IMV
{
    public partial class AccessForm : Form
    {
        bool success = false, exit = false;
        short iter;
        public AccessForm()
        {
            InitializeComponent();
        }

        private string ifLoadLogin()
        {
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.webBrowser2.Dock = DockStyle.Fill;
            this.webBrowser2.Visible = true;

            while (this.Width <= 633 || this.Height <= 429)
            {
                System.Drawing.Size size = new Size(this.Width <= 633 ? this.Width + 6 : this.Width,
                    this.Height <= 429 ? this.Height + 5 : this.Height);
                if (iter % 2 == 0)
                {
                    Point point = new Point(this.Width <= 633 ? this.Location.X - 5 : this.Location.X,
                        this.Height <= 429 ? this.Location.Y - 4 : this.Location.Y);
                    this.Location = point;
                }
                this.Size = size;
                iter++;
            }

            webBrowser2.Navigate("https://api.vkontakte.ru/oauth/authorize?client_id=1964599&scope=13318&redirect_uri=https://api.vkontakte.ru/blank.html&response_type=token&display=popup");
            int count = 0;
            while (!success && !exit)
            {
                if (count > 500)
                {
                    Application.DoEvents();
                    count = 0;
                }
                count++;
            }
            return webBrowser2.Url.Fragment;
        }

        private void webBrowser1_NewWindow(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true; // Не даём открывать новые окна, если что-то ВКонтакте или в компоненте веббраузер залагает
        }

        private void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            string urlFragment; // фрагмент параметров авторизации

            if (e.Url.AbsolutePath == "/blank.html") // Если абсолютный урл равен, то авторизация прошла
                urlFragment = webBrowser1.Url.Fragment;
            else
                urlFragment = ifLoadLogin();

            if (urlFragment.IndexOf("error") != -1 || urlFragment == "") // Если авторизация прошла с ошибкой
            {
                this.DialogResult = DialogResult.Cancel;
                return;
            }

            if (urlFragment.IndexOf("error") == -1) // Если нет ошибок, берём токен, время жизни и наш айди
            {
                vars.VARS.Token = urlFragment.Substring(14, urlFragment.IndexOf("&") - 14);
                urlFragment = urlFragment.Remove(0, urlFragment.IndexOf("&") + 1);
                vars.VARS.Expire = Convert.ToUInt32(urlFragment.Substring(11, urlFragment.IndexOf("&") - 11));
                urlFragment = urlFragment.Remove(0, urlFragment.IndexOf("&"));
                vars.VARS.Mid = Convert.ToUInt32(urlFragment.Substring(9, urlFragment.Length - 9));
            }

            this.DialogResult = DialogResult.OK; // Возвращаем сообщение об успехе
        }

        private void webBrowser2_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            if (e.Url.AbsoluteUri == "http://login.vk.com/?act=login&soft=1")
                webBrowser1.Navigate("https://api.vkontakte.ru/oauth/authorize?client_id=1964599&scope=13318&redirect_uri=https://api.vkontakte.ru/blank.html&response_type=token&display=popup");
            if (e.Url.AbsolutePath == "/blank.html")// Если абсолют равен, то авторизация успешна
            {
                //uri = webBrowser1.Url.Fragment;
                //webBrowser1.Dispose();
                success = true;
            }
            if (e.Url.OriginalString.IndexOf("user_denied") != -1)
                Application.Exit();
        }

        private void AccessForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                exit = true;
                Application.Exit();
            }
        }
    }
}
