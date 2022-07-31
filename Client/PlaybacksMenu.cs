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
        public PlaybacksMenu()
        {
            InitializeComponent();
        }

        private void PlaybacksMenu_Load(object sender, EventArgs e)
        {
            TableGamesBindingSource.DataSource = db.TableGames;
            TableGamesDataGridView.DataSource = TableGamesBindingSource;
            TableGamesBindingNavigator.BindingSource = TableGamesBindingSource;
        }
    }
}
