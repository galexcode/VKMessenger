namespace IMV
{
    partial class Setting
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.exitOnCloser = new System.Windows.Forms.CheckBox();
            this.updateFriends = new System.Windows.Forms.CheckBox();
            this.button3 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.OtherFolder = new System.Windows.Forms.CheckBox();
            this.SaveSettings = new System.Windows.Forms.CheckBox();
            this.Frequency = new System.Windows.Forms.CheckBox();
            this.OfflineMsg = new System.Windows.Forms.CheckBox();
            this.Offline = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.notify_offline = new System.Windows.Forms.CheckBox();
            this.notify_income = new System.Windows.Forms.CheckBox();
            this.notify_online = new System.Windows.Forms.CheckBox();
            this.visual_notify = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.userOffline = new System.Windows.Forms.CheckBox();
            this.userOnline = new System.Windows.Forms.CheckBox();
            this.out_msg = new System.Windows.Forms.CheckBox();
            this.incom_msg = new System.Windows.Forms.CheckBox();
            this.button7 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.exitVK = new System.Windows.Forms.CheckBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(266, 295);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "ОК";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(347, 295);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "Отмена";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(410, 277);
            this.tabControl1.TabIndex = 6;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(243)))), ((int)(((byte)(250)))));
            this.tabPage1.Controls.Add(this.exitVK);
            this.tabPage1.Controls.Add(this.exitOnCloser);
            this.tabPage1.Controls.Add(this.updateFriends);
            this.tabPage1.Controls.Add(this.button3);
            this.tabPage1.Controls.Add(this.textBox1);
            this.tabPage1.Controls.Add(this.OtherFolder);
            this.tabPage1.Controls.Add(this.SaveSettings);
            this.tabPage1.Controls.Add(this.Frequency);
            this.tabPage1.Controls.Add(this.OfflineMsg);
            this.tabPage1.Controls.Add(this.Offline);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(402, 251);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Общие";
            // 
            // exitOnCloser
            // 
            this.exitOnCloser.AutoSize = true;
            this.exitOnCloser.Location = new System.Drawing.Point(4, 169);
            this.exitOnCloser.Name = "exitOnCloser";
            this.exitOnCloser.Size = new System.Drawing.Size(267, 17);
            this.exitOnCloser.TabIndex = 8;
            this.exitOnCloser.Text = "Закрывать программу при нажатии на крестик";
            this.exitOnCloser.UseVisualStyleBackColor = true;
            // 
            // updateFriends
            // 
            this.updateFriends.AutoSize = true;
            this.updateFriends.Location = new System.Drawing.Point(4, 146);
            this.updateFriends.Name = "updateFriends";
            this.updateFriends.Size = new System.Drawing.Size(307, 17);
            this.updateFriends.TabIndex = 7;
            this.updateFriends.Text = "Оповещать при запуске об удалении кого-то из друзей";
            this.updateFriends.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(299, 119);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(26, 20);
            this.button3.TabIndex = 6;
            this.button3.Text = "...";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.Window;
            this.textBox1.Enabled = false;
            this.textBox1.Location = new System.Drawing.Point(24, 119);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(260, 20);
            this.textBox1.TabIndex = 5;
            // 
            // OtherFolder
            // 
            this.OtherFolder.AutoSize = true;
            this.OtherFolder.Location = new System.Drawing.Point(4, 96);
            this.OtherFolder.Name = "OtherFolder";
            this.OtherFolder.Size = new System.Drawing.Size(280, 17);
            this.OtherFolder.TabIndex = 4;
            this.OtherFolder.Text = "Использовать другую папку для хранения данных";
            this.OtherFolder.UseVisualStyleBackColor = true;
            this.OtherFolder.CheckedChanged += new System.EventHandler(this.OtherFolder_CheckedChanged);
            // 
            // SaveSettings
            // 
            this.SaveSettings.AutoSize = true;
            this.SaveSettings.Location = new System.Drawing.Point(4, 73);
            this.SaveSettings.Name = "SaveSettings";
            this.SaveSettings.Size = new System.Drawing.Size(276, 17);
            this.SaveSettings.TabIndex = 3;
            this.SaveSettings.Text = "Сохранять настройки при выходе из проложения";
            this.SaveSettings.UseVisualStyleBackColor = true;
            // 
            // Frequency
            // 
            this.Frequency.AutoSize = true;
            this.Frequency.Location = new System.Drawing.Point(4, 50);
            this.Frequency.Name = "Frequency";
            this.Frequency.Size = new System.Drawing.Size(256, 17);
            this.Frequency.TabIndex = 2;
            this.Frequency.Text = "Сортировать контакты по частоте переписки";
            this.Frequency.UseVisualStyleBackColor = true;
            // 
            // OfflineMsg
            // 
            this.OfflineMsg.AutoSize = true;
            this.OfflineMsg.Location = new System.Drawing.Point(4, 27);
            this.OfflineMsg.Name = "OfflineMsg";
            this.OfflineMsg.Size = new System.Drawing.Size(221, 17);
            this.OfflineMsg.TabIndex = 1;
            this.OfflineMsg.Text = "Загружать непрочитанные сообщения";
            this.OfflineMsg.UseVisualStyleBackColor = true;
            // 
            // Offline
            // 
            this.Offline.Location = new System.Drawing.Point(4, 4);
            this.Offline.Name = "Offline";
            this.Offline.Size = new System.Drawing.Size(223, 17);
            this.Offline.TabIndex = 0;
            this.Offline.Text = "Показывать оффлайн пользователей";
            this.Offline.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(243)))), ((int)(((byte)(250)))));
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(402, 251);
            this.tabPage2.TabIndex = 0;
            this.tabPage2.Text = "Оповещения";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.notify_offline);
            this.groupBox2.Controls.Add(this.notify_income);
            this.groupBox2.Controls.Add(this.notify_online);
            this.groupBox2.Controls.Add(this.visual_notify);
            this.groupBox2.Location = new System.Drawing.Point(3, 129);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(240, 119);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Визуальные оповещения";
            // 
            // notify_offline
            // 
            this.notify_offline.AutoSize = true;
            this.notify_offline.Location = new System.Drawing.Point(10, 66);
            this.notify_offline.Name = "notify_offline";
            this.notify_offline.Size = new System.Drawing.Size(148, 17);
            this.notify_offline.TabIndex = 2;
            this.notify_offline.Text = "Пользователь оффлайн";
            this.notify_offline.UseVisualStyleBackColor = true;
            this.notify_offline.Visible = false;
            // 
            // notify_income
            // 
            this.notify_income.AutoSize = true;
            this.notify_income.Location = new System.Drawing.Point(10, 89);
            this.notify_income.Name = "notify_income";
            this.notify_income.Size = new System.Drawing.Size(137, 17);
            this.notify_income.TabIndex = 2;
            this.notify_income.Text = "Входящее сообщение";
            this.notify_income.UseVisualStyleBackColor = true;
            this.notify_income.Visible = false;
            // 
            // notify_online
            // 
            this.notify_online.AutoSize = true;
            this.notify_online.Location = new System.Drawing.Point(10, 43);
            this.notify_online.Name = "notify_online";
            this.notify_online.Size = new System.Drawing.Size(138, 17);
            this.notify_online.TabIndex = 1;
            this.notify_online.Text = "Пользователь онлайн";
            this.notify_online.UseVisualStyleBackColor = true;
            this.notify_online.Visible = false;
            // 
            // visual_notify
            // 
            this.visual_notify.AutoSize = true;
            this.visual_notify.Location = new System.Drawing.Point(10, 20);
            this.visual_notify.Name = "visual_notify";
            this.visual_notify.Size = new System.Drawing.Size(141, 17);
            this.visual_notify.TabIndex = 0;
            this.visual_notify.Text = "Всплывающее окошко";
            this.visual_notify.UseVisualStyleBackColor = true;
            this.visual_notify.CheckedChanged += new System.EventHandler(this.visual_notify_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.userOffline);
            this.groupBox1.Controls.Add(this.userOnline);
            this.groupBox1.Controls.Add(this.out_msg);
            this.groupBox1.Controls.Add(this.incom_msg);
            this.groupBox1.Controls.Add(this.button7);
            this.groupBox1.Controls.Add(this.button6);
            this.groupBox1.Controls.Add(this.button5);
            this.groupBox1.Controls.Add(this.button4);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(240, 120);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Звуковые оповещения";
            // 
            // userOffline
            // 
            this.userOffline.AutoSize = true;
            this.userOffline.Location = new System.Drawing.Point(10, 91);
            this.userOffline.Name = "userOffline";
            this.userOffline.Size = new System.Drawing.Size(148, 17);
            this.userOffline.TabIndex = 15;
            this.userOffline.Text = "Пользователь оффлайн";
            this.userOffline.UseVisualStyleBackColor = true;
            // 
            // userOnline
            // 
            this.userOnline.AutoSize = true;
            this.userOnline.Location = new System.Drawing.Point(10, 67);
            this.userOnline.Name = "userOnline";
            this.userOnline.Size = new System.Drawing.Size(138, 17);
            this.userOnline.TabIndex = 14;
            this.userOnline.Text = "Пользователь онлайн";
            this.userOnline.UseVisualStyleBackColor = true;
            // 
            // out_msg
            // 
            this.out_msg.AutoSize = true;
            this.out_msg.Location = new System.Drawing.Point(10, 43);
            this.out_msg.Name = "out_msg";
            this.out_msg.Size = new System.Drawing.Size(144, 17);
            this.out_msg.TabIndex = 13;
            this.out_msg.Text = "Исходящее сообщение";
            this.out_msg.UseVisualStyleBackColor = true;
            // 
            // incom_msg
            // 
            this.incom_msg.AutoSize = true;
            this.incom_msg.Location = new System.Drawing.Point(10, 20);
            this.incom_msg.Name = "incom_msg";
            this.incom_msg.Size = new System.Drawing.Size(137, 17);
            this.incom_msg.TabIndex = 12;
            this.incom_msg.Text = "Входящее сообщение";
            this.incom_msg.UseVisualStyleBackColor = true;
            // 
            // button7
            // 
            this.button7.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button7.Location = new System.Drawing.Point(204, 90);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(30, 18);
            this.button7.TabIndex = 11;
            this.button7.Text = "...";
            this.button7.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button6
            // 
            this.button6.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button6.Location = new System.Drawing.Point(204, 66);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(30, 18);
            this.button6.TabIndex = 10;
            this.button6.Text = "...";
            this.button6.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button5
            // 
            this.button5.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button5.Location = new System.Drawing.Point(204, 42);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(30, 18);
            this.button5.TabIndex = 9;
            this.button5.Text = "...";
            this.button5.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button4
            // 
            this.button4.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button4.Location = new System.Drawing.Point(204, 19);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(30, 18);
            this.button4.TabIndex = 8;
            this.button4.Text = "...";
            this.button4.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // exitVK
            // 
            this.exitVK.AutoSize = true;
            this.exitVK.Location = new System.Drawing.Point(4, 192);
            this.exitVK.Name = "exitVK";
            this.exitVK.Size = new System.Drawing.Size(280, 17);
            this.exitVK.TabIndex = 9;
            this.exitVK.Text = "Выходить из контакта при выходе из приложения";
            this.exitVK.UseVisualStyleBackColor = true;
            // 
            // Setting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(243)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(434, 330);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Icon = global::IMV.Properties.Resources._1302885186_neverball;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Setting";
            this.Text = "Настройки";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.CheckBox Offline;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.CheckBox OtherFolder;
        private System.Windows.Forms.CheckBox SaveSettings;
        private System.Windows.Forms.CheckBox Frequency;
        private System.Windows.Forms.CheckBox OfflineMsg;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.CheckBox userOffline;
        private System.Windows.Forms.CheckBox userOnline;
        private System.Windows.Forms.CheckBox out_msg;
        private System.Windows.Forms.CheckBox incom_msg;
        private System.Windows.Forms.CheckBox updateFriends;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox visual_notify;
        private System.Windows.Forms.CheckBox notify_offline;
        private System.Windows.Forms.CheckBox notify_income;
        private System.Windows.Forms.CheckBox notify_online;
        private System.Windows.Forms.CheckBox exitOnCloser;
        private System.Windows.Forms.CheckBox exitVK;

    }
}