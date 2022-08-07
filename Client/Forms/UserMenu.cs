using Client.Model;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Client
{
    public partial class UserMenu : Form
    {

        Player p1 = new Player();
        Game game = new Game();
        Stopwatch stopWatch = new Stopwatch();

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

        private void button1_Click(object sender, EventArgs e)
        {
            TheGame theGame = new TheGame();
            theGame.initalizePlayer(p1);
            theGame.Show();
            MessageBox.Show("Your Details:\n" + "Id Number: " + p1.Id + "\n" + "Name: " + p1.Name + "\n" + "Phone: " + p1.Phone);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            PlaybacksMenu playbacksMenu = new PlaybacksMenu();
            playbacksMenu.initalizePlayer(p1);
            playbacksMenu.Show();
        }

    }
}
