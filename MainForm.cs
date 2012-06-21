using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Text;
using System.Windows.Forms;
using System.Web;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Linq;

namespace IMV
{
    public enum InternetConnectionState : int // необходимое перечисления для InternetGetConnectedState
    {
        INTERNET_CONNECTION_MODEM = 0x1,
        INTERNET_CONNECTION_LAN = 0x2,
        INTERNET_CONNECTION_PROXY = 0x4,
        INTERNET_RAS_INSTALLED = 0x10,
        INTERNET_CONNECTION_OFFLINE = 0x20,
        INTERNET_CONNECTION_CONFIGURED = 0x40
    }

    public partial class MainForm : Form
    {
        [DllImport("WININET", CharSet = CharSet.Auto)]
        static extern bool InternetGetConnectedState(ref InternetConnectionState lpdwFlags, int dwReserved); // проверка интернет соединения

        vk start = new vk();
        LongPoolServer serv = new LongPoolServer();

        Wait wt;
        bool wtcreate;
        ContextMenuStrip context;

        private DwmApi.MARGINS m_glassMargins;

        delegate void getEvent();
        delegate void getPhoto(List<uint> a, string size);
        delegate void NotifyEvent(string name, string text, uint ID);

        System.Threading.Timer timerKey, timerCount;
        System.Windows.Forms.Timer wait;
        TimerCallback getKey, getCounters;

        public MainForm()
        {
            InitializeComponent();
            context = new ContextMenuStrip();
            context.Items.Add("Настройки");
            context.Items.Add("Выход");
            context.Items[0].Name = "Settings";
            context.Items[1].Name = "Exit";
            context.ItemClicked += new ToolStripItemClickedEventHandler(context_ItemClicked);
            notifyIcon1.ContextMenuStrip = context;
            serv.NewStatus += new ChangeStatusEventHandler(serv_NewStatus);
            serv.NewMessage += new IncomingMessageEventHandler(serv_NewMessage);
        }

        void serv_NewMessage(IncomingMessageArgs info)
        {
            if ((info.flag & 32) == 32 && (info.flag & 2) != 2)
            {
                if (vars.VARS.Visual_notify && vars.VARS.Notify_income)
                {
                    string txt; // текст, который будет отображаться в окошке
                    if (info.text.Length > 20)
                    {
                        txt = info.text.Substring(0, 20);
                        txt += "...";
                    }
                    else
                        txt = info.text;

                    NotifyEvent ShowNotify = new NotifyEvent(ShowNotifyWindow);
                    this.Invoke(ShowNotify, vars.VARS.Contact[info.outID].UserName, txt, info.outID); // инвочим для того, чтобы она всплывала
                }
            }
        }

        void serv_NewStatus(ChangeStatusArgs info)
        {
            if (vars.VARS.Visual_notify && vars.VARS.Notify_online && info.flag == 1) // если визуальные оповещения включены и пользователь онлайн
            {
                NotifyEvent ShowNotify = new NotifyEvent(ShowNotifyWindow);
                this.Invoke(ShowNotify, vars.VARS.Contact[info.id].UserName, "В сети", info.id); // инвочим для того, чтобы она всплывала
            }
            if (vars.VARS.Visual_notify && vars.VARS.Notify_offline && info.flag == 0) // если визуальные оповещения включены и пользователь оффлайн
            {
                NotifyEvent ShowNotify = new NotifyEvent(ShowNotifyWindow);
                this.Invoke(ShowNotify, vars.VARS.Contact[info.id].UserName, "Не в сети", info.id);
            }
        }

        void ShowNotifyWindow(string name, string text, uint ID)
        {
            Notify noti = new Notify(name, text, ID);
            noti.ShowWindow();
            noti.ShowTime();
        }

        void context_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Name == "Settings") // если имя пункта меню равно...
            {
                Setting set = new Setting();
                set.Show();
            }
            if (e.ClickedItem.Name == "Exit")
            {
                if (vars.VARS.ExitVK)
                {
                    WebBrowser web = new WebBrowser();
                    web.Navigate("http://m.vkontakte.ru/logout");
                    string[] theCookies = System.IO.Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Cookies));
                    foreach (string currentFile in theCookies) // удаляем cookies с данными авторизации
                    {
                        try
                        {
                            if (currentFile.IndexOf("login.vk") != -1 || currentFile.IndexOf("vkontakte") != -1)
                                System.IO.File.Delete(currentFile);
                        }
                        catch (Exception exe)
                        {
                            GeneralMethods.WriteError(exe.Source, exe.Message, exe.TargetSite);
                        }
                    }
                }
                if (vars.VARS.SaveSettings) // если надо, то сохраняем настройки
                {
                    if (vars.VARS.Frequency) // если включена настройка сортировки по частоте
                    {
                        try
                        {
                            System.IO.FileStream stream = new System.IO.FileStream(vars.VARS.Directory + "frequency.dat", System.IO.FileMode.Create, System.IO.FileAccess.Write);
                            System.IO.StreamWriter writer = new System.IO.StreamWriter(stream, Encoding.UTF8);
                            foreach (uint id in vars.VARS.FrequencyUse.Keys)
                            {
                                writer.Write(id);
                                writer.Write(":");
                                writer.WriteLine(vars.VARS.FrequencyUse[id]);
                                writer.Flush();
                            }
                            writer.Close();
                            writer.Dispose();
                            stream.Dispose();
                        }
                        catch (Exception exe)
                        {
                            GeneralMethods.WriteError(exe.Source, exe.Message, exe.TargetSite);
                        }
                    }
                }
                else // если не сохраняем настройки, то удаляем ранее созданный файл
                {
                    try
                    {
                        System.IO.File.Delete(vars.VARS.Directory + "settings.cfg");
                    }
                    catch
                    {
                    }
                }
                Application.Exit();
            }
        }

        #region Glass
        /// <summary>
        /// Методы, реализующие эффект прозрачного стекла
        /// </summary>
        /// <param name="msg"></param>
        protected override void WndProc(ref Message msg)
        {
            base.WndProc(ref msg); // Вызываем базовый метод

            if (Environment.OSVersion.Version.Major >= 6.0) // Если версия видовс больше 6 (то есть виста и выше, то будем обрабатывать сообщения для прозрачности)
            {
                const int WM_DWMCOMPOSITIONCHANGED = 0x031E;

                switch (msg.Msg)
                {
                    case WM_DWMCOMPOSITIONCHANGED:
                        if (!DwmApi.DwmIsCompositionEnabled()) // если режим aero не включен, зануляем область стекла
                        {
                            m_glassMargins = null;
                        }
                        break;
                }
            }
        }

        private void ResetDwmBlurBehind()
        {
            if (DwmApi.DwmIsCompositionEnabled())
            {
                DwmApi.DWM_BLURBEHIND blur_behind = new DwmApi.DWM_BLURBEHIND();
                blur_behind.dwFlags = DwmApi.DWM_BLURBEHIND.DWM_BB_ENABLE | DwmApi.DWM_BLURBEHIND.DWM_BB_BLURREGION;
                blur_behind.fEnable = false;
                blur_behind.hRegionBlur = IntPtr.Zero;
                DwmApi.DwmEnableBlurBehindWindow(this.Handle, blur_behind); // включает эффект стекла
            }
        }


        private void OnClientArea()
        {
            ResetDwmBlurBehind();

            m_glassMargins = new DwmApi.MARGINS(0, 0, 0, 32); // устанавливаем отступы от края, которые будут прозрачными

            if (DwmApi.DwmIsCompositionEnabled()) DwmApi.DwmExtendFrameIntoClientArea(this.Handle, m_glassMargins); // если режим aero включён, задаём область где будет стекло

            this.Invalidate();
        }

        #endregion

        private void StartWork()
        {
            IntPtr hand = vars.VARS.Chat.Handle; // Инициализация окна чата, получение заголовка окна

            Thread checkVersion = new Thread(new ThreadStart(GeneralMethods.CheckNewVersion));
            checkVersion.Start();

            if (vars.VARS.GetOfflineMsg)
            {
                Thread newThrd = new Thread(new ThreadStart(start.messageGet)); // Получение списка контактов
                newThrd.Start();
            }

            getEvent Icon = new getEvent(start.getIcon);
            IAsyncResult res1 = Icon.BeginInvoke(null, null); // в асинхронном потоке грузим фотографии для контакт листа

            getEvent newEvent = new getEvent(serv.getLongPollServer);
            IAsyncResult res2 = newEvent.BeginInvoke(null, null); // в асинхронном потоке обрабатываем события о приходе сообщения и о статусах пользователей

            getCounters = new TimerCallback(UpdateCounters);
            timerCount = new System.Threading.Timer(getCounters, null, 0, 300000);

            if (vars.VARS.Expire != 0) // если время истечения ключа не равно 0, то
            {
                getKey = new TimerCallback(GetNewToken);
                timerKey = new System.Threading.Timer(getKey, null, (vars.VARS.Expire - 60) * 1000, (vars.VARS.Expire - 60) * 1000); // пускаем таймер обновлять каждые expire количество времени ключ token
            }

            if (vars.VARS.UpdateFriends) // если включена функция посика удалившихся друзей
            {
                Thread newThrd = new Thread(new ThreadStart(GeneralMethods.SearchFriends)); // Получение списка контактов
                newThrd.Start();
            }
        }

        private void GetNewToken(object state)
        {
            try
            {
                Invoke(new MethodInvoker(() =>
                                          {
                                              AccessForm acc = new AccessForm();
                                              acc.ShowDialog();
                                              if (acc.DialogResult == DialogResult.OK)
                                                  acc.Close();
                                              else
                                              {
                                                  MessageBox.Show("Сервер вас отключил!\nПриложение автоматически перезапустится\n и постарается восстановить связь.", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                                  Application.Exit();
                                              }
                                          }
                                          )); // инвочим метод, потому что находимся в другом потоке. Получаем новые данные сессии
                timerKey.Change((vars.VARS.Expire - 60) * 1000, (vars.VARS.Expire - 60) * 1000); // перезапускаем таймер с новым expire
            }

            catch (Exception exe)
            {
                GeneralMethods.WriteError(exe.Source, exe.Message, exe.TargetSite);
            }  
        }

        private void UpdateCounters(object state)
        {
            Hashtable couter = start.getStat();
            if (couter != null)
            {
                if (couter.ContainsKey("friends"))
                {
                    if (Convert.ToUInt32(couter["friends"]) > 0)
                    {
                        NotifyEvent ShowNotify = new NotifyEvent(ShowNotifyWindow);
                        this.Invoke(ShowNotify, "Новое событие!", "Новый друг", (uint)0);
                        getEvent ShowButton = new getEvent(this.ShowButton);
                        this.button5.Invoke(ShowButton);
                    }
                }
                if (couter.ContainsKey("photos"))
                {
                    if (Convert.ToUInt32(couter["photos"]) > 0)
                    {
                        NotifyEvent ShowNotify = new NotifyEvent(ShowNotifyWindow);
                        this.Invoke(ShowNotify, "Новое событие!", "Новая фотография", (uint)0);
                        getEvent ShowButton = new getEvent(this.ShowButton);
                        this.button5.Invoke(ShowButton);
                    }
                }
                if (couter.ContainsKey("videos"))
                {
                    if (Convert.ToUInt32(couter["videos"]) > 0)
                    {
                        NotifyEvent ShowNotify = new NotifyEvent(ShowNotifyWindow);
                        this.Invoke(ShowNotify, "Новое событие!", "Новое видео", (uint)0);
                        getEvent ShowButton = new getEvent(this.ShowButton);
                        this.button5.Invoke(ShowButton);
                    }
                }
                if (couter.ContainsKey("events"))
                {
                    if (Convert.ToUInt32(couter["events"]) > 0)
                    {
                        NotifyEvent ShowNotify = new NotifyEvent(ShowNotifyWindow);
                        this.Invoke(ShowNotify, "Новое событие!", "Новая встреча", (uint)0);
                        getEvent ShowButton = new getEvent(this.ShowButton);
                        this.button5.Invoke(ShowButton);
                    }
                }
            }
            else
            {
                getEvent HideButton = new getEvent(this.HideButton);
                this.button5.Invoke(HideButton);
            }
        }

        private void ShowButton()
        {
            this.button5.Visible = true;
        }

        private void HideButton()
        {
            this.button5.Visible = false;
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            myContactList1.FinishUpdate(); // обновляем настройки контакт листа
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (Environment.OSVersion.Version.Major >= 6.0) // если версия выше, делаём всё черным (черный GDI прозрачен)
            {
                if (DwmApi.DwmIsCompositionEnabled())
                {
                    button1.BackColor = Color.Black;
                    button2.BackColor = Color.Black;
                    button3.BackColor = Color.Black;
                    button4.BackColor = Color.Black;
                    button5.BackColor = Color.Black;
                    panel1.BackColor = Color.Black;
                    OnClientArea();
                }
            }

            if (!System.IO.Directory.Exists(vars.VARS.Directory)) // если директории каким-то образом не оказалось, создаём её и кидаем туда файл ошибок
            {
                System.IO.Directory.CreateDirectory(vars.VARS.Directory);
                System.IO.File.Create(vars.VARS.Directory + "errors.txt");
            }

            bool flag = true;
            InternetConnectionState flags = 0;
            bool InternetConnect = InternetGetConnectedState(ref flags, 0); // проверяем соединение с интернетом

            while (!InternetConnect) // пока не появится, будет цикл повторяться
            {
                if (flag)
                {
                    MessageBox.Show("Сервер временно недоступен!\nПрограмма подключится, когда появится соединение!", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    flag = false;
                    notifyIcon1.Visible = true;
                }
                Application.DoEvents();
                Thread.Sleep(2000);
                InternetConnect = InternetGetConnectedState(ref flags, 0);
            }

            AccessForm acc = new AccessForm(); // создаём форму авторизации
            if (acc.ShowDialog() == DialogResult.OK) // если ок, выполняем
            {
                GeneralMethods.tryGetSettings(); // читаем настройки
                if (!vars.VARS.ExitOnCloser) // устанавливаем режим сворачивания
                    MinimizeBox = false;
                if (!vars.VARS.Sound) // устанавливаем в верное положение картинку звука
                    button1.Image = global::IMV.Properties.Resources._1299773905_no_sound;
                wait = new System.Windows.Forms.Timer(); // создаём таймер
                wait.Interval = 4000;
                wait.Tick += new EventHandler(wait_Tick);
                wait.Start(); // запускаем таймер
                Thread newThrd = new Thread(new ThreadStart(start.getProfiles)); // Получение списка контактов
                newThrd.Start();
                while (newThrd.IsAlive) // приложение отправляет сообщения виндовс пока поток не завершился
                    Application.DoEvents();
                wait.Stop();
                wait.Dispose();
                if (wtcreate) // если открывалось окно ожидания, то уничтожаем его
                    wt.Dispose();
                acc.Dispose();
                GeneralMethods.AddItem(); // добавляем контакты в список
                if (vars.VARS.Frequency) // если включена настройка
                    GeneralMethods.tryGetFrequency(); // пытаемся загрузить частоту
                myContactList1.Sort(); // сортируем
                StartWork(); // Запуск "отлова" обновлений
            }
            else
                Application.Exit();

        }

        void wait_Tick(object sender, EventArgs e)
        {
            wt = new Wait(); // если больше 4 секунд грузится, то появляется окно ожидания
            wtcreate = true;
            wait.Stop();
            wt.Show();            
            //throw new NotImplementedException();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (vars.VARS.Sound)
            {
                vars.VARS.Sound = false;
                button1.Image = global::IMV.Properties.Resources._1299773905_no_sound;
            }
            else
            {
                vars.VARS.Sound = true;
                button1.Image = global::IMV.Properties.Resources._1299773922_sound;
            }            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            { // обновляем маленькие фотографии в списке контактов если кажется, что они устарели, например)
                vk start = new vk();
                List<uint> a = new List<uint>();
                foreach (KeyValuePair<uint, vk.profile> item in vars.VARS.Contact) // для кого грузим новые фотки (в данном случае для всех)
                    a.Add(item.Key);
                getPhoto newPhoto = new getPhoto(start.getPhoto);
                IAsyncResult res2 = newPhoto.BeginInvoke(a, "photo", null, null); // пускаем в асинхронном потоке
            }
            catch (Exception exe)
            {
                MessageBox.Show("Неизвестная ошибка!", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                GeneralMethods.WriteError(exe.Source, exe.Message, exe.TargetSite);
            }
        }

        private void MainForm_Deactivate(object sender, EventArgs e)
        {// сворачивание в трей
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                notifyIcon1.Visible = true;
            }
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        { // разворачивание окна из трея
            if ((!this.Visible) && (e.Button == MouseButtons.Left))
            {
                this.MinimizeBox = vars.VARS.ExitOnCloser;
                this.ShowInTaskbar = true;
                this.Show();
                this.WindowState = FormWindowState.Normal;
                notifyIcon1.Visible = false;
            }
        }

        private void MainForm_Activated(object sender, EventArgs e)
        {
            if (Environment.OSVersion.Version.Major >= 6.0)
                OnClientArea();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (vars.VARS.ExitOnCloser)
            {
                if (vars.VARS.ExitVK)
                {
                    WebBrowser web = new WebBrowser();
                    web.Navigate("http://m.vkontakte.ru/logout");
                    string[] theCookies = System.IO.Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Cookies));
                    foreach (string currentFile in theCookies) // удаляем cookies с данными авторизации
                    {
                        try
                        {
                            if (currentFile.IndexOf("login.vk") != -1 || currentFile.IndexOf("vkontakte") != -1)
                                System.IO.File.Delete(currentFile);
                        }
                        catch (Exception exe)
                        {
                            GeneralMethods.WriteError(exe.Source, exe.Message, exe.TargetSite);
                        }
                    }
                }
                if (vars.VARS.SaveSettings) // если сохраняем настройки
                {
                    if (vars.VARS.Frequency) // если надо, то записываем частоту использования
                    {
                        try
                        {
                            System.IO.FileStream stream = new System.IO.FileStream(vars.VARS.Directory + "frequency.dat", System.IO.FileMode.Create, System.IO.FileAccess.Write);
                            System.IO.StreamWriter writer = new System.IO.StreamWriter(stream, Encoding.UTF8);
                            foreach (uint id in vars.VARS.FrequencyUse.Keys)
                            {
                                writer.Write(id);
                                writer.Write(":");
                                writer.WriteLine(vars.VARS.FrequencyUse[id]);
                                writer.Flush();
                            }
                            writer.Close();
                            writer.Dispose();
                            stream.Dispose();
                        }
                        catch (Exception exe)
                        {
                            GeneralMethods.WriteError(exe.Source, exe.Message, exe.TargetSite);
                        }
                    }
                }
                else // иначе....
                {
                    try
                    { // удаляем файл настроек
                        System.IO.File.Delete(vars.VARS.Directory + "settings.cfg");
                    }
                    catch (Exception exe)
                    {
                        GeneralMethods.WriteError(exe.Source, exe.Message, exe.TargetSite);
                    }
                }
            }
            else // если на крестик указано в настройках сворачивать в трей
            {
                if (e.CloseReason != CloseReason.ApplicationExitCall &&
                    e.CloseReason != CloseReason.FormOwnerClosing &&
                    e.CloseReason != CloseReason.TaskManagerClosing && 
                    e.CloseReason != CloseReason.WindowsShutDown)
                {
                    e.Cancel = true;
                    this.Hide();
                    //this.ShowInTaskbar = false;
                    notifyIcon1.Visible = true;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {//показываем окно ввода статуса
            Status note = new Status();
            note.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {// показываем поле ввода имени
            textBox1.Visible = true;
            textBox1.Focus();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            myContactList1.Items.Clear();

            IEnumerable<vk.profile> findUser;
            findUser = Enumerable.Select(vars.VARS.Contact, n => n.Value); // Выбираем все структуры профилей пользователей
            findUser = Enumerable.Where(findUser, n => n.UserName.Contains(textBox1.Text)); // Ищем имена, которые включают заданную строку

            if (vars.VARS.ShowOffline != true)
                findUser = Enumerable.Where(findUser, n => n.online == true);

            foreach (vk.profile item in findUser) // Добавляем найденное в контакт лист
                myContactList1.Items.Add(item.uid);

            if (myContactList1.Items.Count != 0) // если не равно 0 количество найденных, обновляем настройки
                myContactList1.FinishUpdate();
            else
                myContactList1.FinishUpdate(); // если равно, то посылаем запрос об обновлении параметров

            if (textBox1.Text == "") // если строка поиска пуста, обновляем настроки
                myContactList1.FinishUpdate();
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {// скрываем поле ввода имени
            if (e.KeyCode == Keys.Escape)
            {
                textBox1.Visible = false;
                myContactList1.Sort();
                myContactList1.Invalidate();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://vkontakte.ru/id" + vars.VARS.Mid);
            button5.Visible = false;
        }
    }
}