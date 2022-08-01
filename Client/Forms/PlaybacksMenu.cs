using Client.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class PlaybacksMenu : Form
    {
        private BindingSource TableGamesBindingSource = new BindingSource();
        private GamesDataContext db = new GamesDataContext();
        Player p1 = new Player();
        int gameId = 0;

        public PlaybacksMenu()
        {
            InitializeComponent();
        }
        public void initalizePlayer(Player player)
        {
            p1.Id = player.Id;
            p1.Name = player.Name;
            p1.Phone = player.Phone;
            p1.Num = player.Num;

        }
        private void PlaybacksMenu_Load(object sender, EventArgs e)
        {
            var x =
                from g in db.TableGames
                where g.UserId == p1.Id
                select g;
            TableGamesBindingSource.DataSource = x;
            TableGamesDataGridView.DataSource = TableGamesBindingSource;
            TableGamesDataGridView.Columns["Moves"].Visible = false;
            TableGamesDataGridView.Columns["UserId"].Visible = false;

            TableGamesBindingNavigator.BindingSource = TableGamesBindingSource;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            gameId = Int32.Parse(textBox1.Text);

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            TheGame theGame = new TheGame();
            theGame.Show();
            await theGame.GamePlayback(gameId);
        }
    }
}
