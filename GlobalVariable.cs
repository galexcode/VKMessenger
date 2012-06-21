using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IMV
{
    class vars
    {
        
        protected vars() 
        { }

        private sealed class varsCreate
        {
            private static readonly vars instance = new vars();
            public static vars Instance
            {
                get
                {
                    return instance;
                }
            }
        }

        public static vars VARS
        {
            get
            {
                return varsCreate.Instance;
            }
        }

        #region Настроки
        bool sound = true; // Проигрывать звуки или нет
        string token = ""; // Токен (уник ключ)
        uint expire = 0; // Время жизни токена
        uint mid = 0; // Айди пользователя
        bool saveSettings = true; // Сохранять ли настройки после выхода
        bool showOffline = true; // Показывать ли оффлайн пользователей
        bool getOfflineMsg = false; // Получать ли непрочитанные сообщения при входе
        bool frequency = false; // Сортировать по частоте использования
        bool otherFolder = false; // Сохранять данные приложения в другую папку
        bool exitOnCloser = true; // Выходить при нажатии на крестик
        string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\IMV\\"; // Папка для хранения файлов
        bool incoming_message_on = true; // Звук входящего сообщения (вкл/выкл)
        bool out_message_on = false; // Звук исходящего сообщения (вкл/выкл)
        bool user_online_on = false; // Звук для пользователя, ставшего онлайн (вкл/выкл)
        bool user_offline_on = false; // Звук для пользователя, ставшего оффлайн (вкл/выкл)
        string incoming_message = System.Windows.Forms.Application.StartupPath + "\\Sound\\inm.wav"; // Звук входящего сообщения (файл)
        string out_message = System.Windows.Forms.Application.StartupPath + "\\Sound\\outm.wav"; // Звук исходящего сообщения (файл)
        string user_online = System.Windows.Forms.Application.StartupPath + "\\Sound\\usronoff.wav"; // Звук для пользователя, ставшего онлайн (файл)
        string user_offline = System.Windows.Forms.Application.StartupPath + "\\Sound\\usronoff.wav"; // Звук для пользователя, ставшего оффлайн (файл)
        bool visual_notify = true; // Всплывашки (вкл/выкл)
        bool notify_online = true; // О входе в сеть
        bool notify_offline = false; // О выходе из сети
        bool notify_income = false; // О входящем сообщении
        bool updateFriends = false; // Проверять ли удилался кто-либо из друзей
        bool exitVK = true; // Выходить из контакта при выходе из приложения


        public bool Sound
        {
            get
            {
                return sound;
            }
            set
            {
                sound = value;
            }
        }

        public string Token
        {
            get
            {
                return token;
            }
            set
            {
                token = value;
            }
        }

        public uint Expire
        {
            get
            {
                return expire;
            }
            set
            {
                expire = value;
            }
        }

        public uint Mid
        {
            get
            {
                return mid;
            }
            set
            {
                mid = value;
            }
        }

        public bool SaveSettings
        {
            get
            {
                return saveSettings;
            }
            set
            {
                saveSettings = value;
            }
        }

        public bool ShowOffline
        {
            get
            {
                return showOffline;
            }
            set
            {
                showOffline = value;
            }
        }

        public bool GetOfflineMsg
        {
            get
            {
                return getOfflineMsg;
            }
            set
            {
                getOfflineMsg = value;
            }
        }

        public bool Frequency
        {
            get
            {
                return frequency;
            }
            set
            {
                frequency = value;
            }
        }

        public bool OtherFolder
        {
            get
            {
                return otherFolder;
            }
            set
            {
                otherFolder = value;
            }
        }

        public bool ExitOnCloser
        {
            get
            {
                return exitOnCloser;
            }
            set
            {
                exitOnCloser = value;
            }
        }

        public string Directory
        {
            get
            {
                return directory;
            }
            set
            {
                directory = value;
            }
        }

        public bool Incoming_message_on
        {
            get
            {
                return incoming_message_on;
            }
            set
            {
                incoming_message_on = value;
            }
        }

        public bool Out_message_on
        {
            get
            {
                return out_message_on;
            }
            set
            {
                out_message_on = value;
            }
        }

        public bool User_online_on
        {
            get
            {
                return user_online_on;
            }
            set
            {
                user_online_on = value;
            }
        }

        public bool User_offline_on
        {
            get
            {
                return user_offline_on;
            }
            set
            {
                user_offline_on = value;
            }
        }

        public string Incoming_message
        {
            get
            {
                return incoming_message;
            }
            set
            {
                incoming_message = value;
            }
        }

        public string Out_message
        {
            get
            {
                return out_message;
            }
            set
            {
                out_message = value;
            }
        }

        public string User_online
        {
            get
            {
                return user_online;
            }
            set
            {
                user_online = value;
            }
        }

        public string User_offline
        {
            get
            {
                return user_offline;
            }
            set
            {
                user_offline = value;
            }
        }

        public bool Visual_notify
        {
            get
            {
                return visual_notify;
            }
            set
            {
                visual_notify = value;
            }
        }

        public bool Notify_online
        {
            get
            {
                return notify_online;
            }
            set
            {
                notify_online = value;
            }
        }

        public bool Notify_offline
        {
            get
            {
                return notify_offline;
            }
            set
            {
                notify_offline = value;
            }
        }

        public bool Notify_income
        {
            get
            {
                return notify_income;
            }
            set
            {
                notify_income = value;
            }
        }

        public bool UpdateFriends
        {
            get
            {
                return updateFriends;
            }
            set
            {
                updateFriends = value;
            }
        }

        public bool ExitVK
        {
            get
            {
                return exitVK;
            }
            set
            {
                exitVK = value;
            }
        }
        #endregion

        #region Общие глобальные переменные

        System.Windows.Forms.ImageList smallPhoto = new System.Windows.Forms.ImageList(); // Маленькие изображения для контакт-листа и прочих нужд
        SortedDictionary<uint, vk.profile> contact = new SortedDictionary<uint, vk.profile>(); // Словарь всех пользователей
        Dictionary<uint, List<uint>> numbMass = new Dictionary<uint, List<uint>>(); // Номера непрочитанных сообщений
        Dictionary<uint, uint> frequencyUse = new Dictionary<uint, uint>(); // Частота использования контактов
        ChatForm chat = ChatForm.chat; // Объект чата

        public System.Windows.Forms.ImageList SmallPhoto
        {
            get
            {
                return smallPhoto;
            }
            set
            {
                smallPhoto = value;
            }
        }

        public SortedDictionary<uint, vk.profile> Contact
        {
            get
            {
                return contact;
            }
            set
            {
                contact = value;
            }
        }

        public Dictionary<uint, List<uint>> NumbMass
        {
            get
            {
                return numbMass;
            }
            set
            {
                numbMass = value;
            }
        }

        public Dictionary<uint, uint> FrequencyUse
        {
            get
            {
                return frequencyUse;
            }
            set
            {
                frequencyUse = value;
            }
        }

        public ChatForm Chat
        {
            get
            {
                return chat;
            }
        }
        #endregion

        #region Переменные программы

        int openNotify = 0;
        const string version = "1.14";

        public int OpenNotify
        {
            get
            {
                return openNotify;
            }
            set
            {
                openNotify = value;
            }
        }

        public string Version
        {
            get
            {
                return version;
            }
        }

        #endregion
    }       
}
