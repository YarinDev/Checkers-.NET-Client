﻿using Client.Model;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Login : Form
    {
        private HttpClient client = new HttpClient();
        private int id;
        private String name, phone;

        public Login()
        {
            InitializeComponent();
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
                MessageBox.Show("There is no user with these details!\nPlease Try again.");
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
