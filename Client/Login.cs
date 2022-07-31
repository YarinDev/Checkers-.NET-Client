using Client.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Login : Form
    {
        private HttpClient client = new HttpClient();
        private BindingSource TblUsersBindingSource = new BindingSource();
        private int id;
        private String name, phone;

        public Login()
        {
            InitializeComponent();
            /*            sendNumToServer(5);
            */
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void GetNumFromServer(int num)
        {
            using (var client = new HttpClient())
            {
                var endPoint = new Uri("https://localhost:44310/");
                var result = client.GetAsync(endPoint).Result;
                var json = result.Content.ReadAsStringAsync().Result;
            }
        }




        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Verify that the pressed key isn't CTRL or any non-numeric digit
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // If you want, you can allow decimal (float) numbers
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Character Validation(only Character allowed)
            if (Char.IsControl(e.KeyChar) != true && Char.IsNumber(e.KeyChar) == true)
            {
                e.Handled = true;
            }

        }
        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Verify that the pressed key isn't CTRL or any non-numeric digit
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // If you want, you can allow decimal (float) numbers
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
            phone = textBox3.Text;

        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            id = Int32.Parse(textBox1.Text);

        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            name = textBox2.Text;

        }
        private void textBox3_TextChanged(object sender, EventArgs e)
        {

            phone = textBox3.Text;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            Player p1 = await GetPlayerAsync("api/TblUsers/" + id);

            name = name.ToLower();
            phone = phone.ToLower();
            p1.Name = p1.Name.ToLower().Trim();
            p1.Phone = p1.Phone.ToLower().Trim();

            /*  TheGame f2 = new TheGame();
              f2.initalizePlayer(p1);
              f2.Show();*/

            if (id == p1.Id && String.Equals(name, p1.Name) && String.Equals(phone, p1.Phone))
            {
                MessageBox.Show("Correct!");

                //Running the Menu for user and passing his information for later forms.   
                UserMenu menu = new UserMenu();
                menu.initalizePlayer(p1);
                menu.Show();
               
            }
            else
            {
                MessageBox.Show("There is no user with the entered details!");
            }
        }

        async Task<Player> GetPlayerAsync(string path)
        {

            Player players = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                players = await response.Content.ReadAsAsync<Player>();

            }


            return players;
        }



        private void StartGame_Load(object sender, EventArgs e)
        {

            client.BaseAddress = new Uri("https://localhost:44310/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
