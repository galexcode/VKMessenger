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
    public partial class DeleteFriends : Form
    {
        Dictionary<uint, string> list = new Dictionary<uint, string>();

        public DeleteFriends()
        {
            InitializeComponent();
        }

        public void AddList(Dictionary<uint, string> list)
        {
            this.list = list;

            foreach (KeyValuePair<uint,string> item in list)
                listBox1.Items.Add(item.Value); // Добавляем всех удалившихся в список
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            IEnumerable<uint> user;
            user = Enumerable.Select(list, n => n.Key);
            user = Enumerable.Where(user, n => list[n].Contains(listBox1.SelectedItem.ToString()));

            foreach (uint item in user)
                System.Diagnostics.Process.Start("http://vkontakte.ru/id" + item); // Запускаем браузер по умолчанию и переходим на страницу
        }

        private void DeleteFriends_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                System.IO.File.Delete(vars.VARS.Directory + "contact.last"); // Удаляем ненужный список
            }
            catch { }
        }
    }
}
