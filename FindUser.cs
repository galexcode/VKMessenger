using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IMV
{
    public partial class FindUser : Form
    {
        uint ID = 0; // Айди выбранного пользователя

        public uint UserID
        {
            get
            {
                return ID;
            }
        }

        public FindUser()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            listBox1.Items.Clear(); // Очищаем лист контактов

            IEnumerable<string> findNames;
            findNames = Enumerable.Select(vars.VARS.Contact, n => n.Value.UserName); // Выбираем все имена пользователей
            findNames = Enumerable.Where(findNames, n => n.Contains(textBox1.Text)); // Ищем имена, которые включают заданную строку

            foreach (string item in findNames)
                listBox1.Items.Add(item); // Добавляем в лист контактов
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            button1_Click(sender, e); // 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count > 0) // Если был выбран контакт
            {
                IEnumerable<vk.profile> findUser;
                findUser = Enumerable.Select(vars.VARS.Contact, n => n.Value); // Выбираем все структуры профилей пользователей
                findUser = Enumerable.Where(findUser, n => n.UserName.Equals(listBox1.SelectedItem.ToString())); // Ищем точное совпадение с выбранной в листбоксе строкой

                foreach (vk.profile item in findUser)
                    ID = item.uid; // Берём АЙДИ этого пользователя
            }
            this.Hide();
        }
    }
}
