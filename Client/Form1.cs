using Client.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Form1 : Form
    {
        private HttpClient client = new HttpClient();
        private BindingSource TblUsersBindingSource = new BindingSource();



        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            IEnumerable<Player> p = await GetPlayerAsync("api/TblUsers/pname/yarin");
            TblUsersBindingSource.DataSource = p;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            TblDataGridView.DataSource = TblUsersBindingSource;

            client.BaseAddress = new Uri("https://localhost:44310/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }
        async Task<IEnumerable<Player>> GetPlayerAsync(string path)
        {

            IEnumerable<Player> player = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                player = await response.Content.ReadAsAsync <IEnumerable<Player>>();
            }
            return player;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
