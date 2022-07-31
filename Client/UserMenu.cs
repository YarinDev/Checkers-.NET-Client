using Client.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class UserMenu : Form
    {
        private HttpClient client = new HttpClient();

        private String name, phone;
        Player p1 = new Player();
        Game game = new Game();
        Stopwatch stopWatch = new Stopwatch();
        private List<String> allGameMoves = new List<String>();
        GamesDataContext dbgm = new GamesDataContext();
        TableGames tg = new TableGames();
        TableGames tg2 = new TableGames();
        public UserMenu()
        {
            InitializeComponent();
        }
        public void initalizePlayer(Player player)
        {
            p1.Id = player.Id;
            p1.Name = player.Name;
            p1.Phone = player.Phone;
            p1.Num = player.Num;
            game.UserId = player.Id;
            game.Date = DateTime.Now;
            stopWatch.Start();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            TheGame theGame = new TheGame();
            theGame.initalizePlayer(p1);
            theGame.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            PlaybacksMenu playbacksMenu = new PlaybacksMenu();
            playbacksMenu.initalizePlayer(p1);
            playbacksMenu.Show();
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

    }
}
