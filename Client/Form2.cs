using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Linq.Expressions;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using Client.Model;
using System.Net.Http.Formatting;

namespace Client
{
    public partial class Form2 : Form
    {
        private HttpClient client = new HttpClient();
        Random r = new Random();
        Player p1 = new Player();
        Player p2 = new Player();
        int id;
        private Boolean computersTurn = false;
        private Boolean firstTime = false;
        private List<int[]> locations;
        private List<List<int[]>> blackCheckersLocationAndMoves;
        private List<int> blacksWithMovesIndexes;
        private int chosenCheckerIndex;


        public Form2()
        {
            InitializeComponent();
            Initialize();

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            client.BaseAddress = new Uri("https://localhost:44310/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

        }
        public void initalizePlayer(Player player)
        {

            p1.Id = player.Id;
            p1.Name = player.Name;
            p1.Phone = player.Phone;
            p1.Num = player.Num;

        }

        protected override void OnLoad(EventArgs e)
        {

        }


        private Button[,] Buttons = new Button[8, 8];
        //initializing board teams (8 vs 8 checkers)
        private Piece[,] Board = new Piece[8, 8];

        private PictureBox[] ImagesW = new PictureBox[12];
        private PictureBox[] ImagesB = new PictureBox[12];

        private Color FirstColour = Color.Azure;
        private Color SecondColour = Color.Black;
        private Color SelectedColour = Color.Orange;

        private Boolean selected = false;
        private int[] Piece_Selected;


        private void Initialize()
        {
            SetUpButtons();
            SetUpColours();
            SetUpSidePictures();
            IntialPositions();
            ShowPieces();
            Debug.WriteLine("Done");

            ImagesB[0].BackgroundImage = Properties.Resources.CheckerBlack1;
        }
        private void moveExample()
        {

            Board = Piece.Move(Board, 2, 3, 3, 2);
            UpdatePiecesTaken();
            SetUpColours();
            ShowPieces();
        }
        //happens when user is clicking on one of the board's squares, if the square is empty it means board[x,y] is null.
        private async void ClickAsync(object sender, EventArgs e)
        {

            Button Btn = (Button)sender;
            int x, y;

            //Subtract 48 to convert Char to Int
            x = Convert.ToInt16(Btn.Name[3]) - 48;
            y = Convert.ToInt16(Btn.Name[4]) - 48;

            if (firstTime != false)
            {
                /*locations = GetBlackCheckersLocations();
                blackCheckersLocationAndMoves = GetPossibleBlackMoves(locations);
                blacksWithMovesIndexes = onlyBlackWithMoves(blackCheckersLocationAndMoves);*/
                try
                {
                   chosenCheckerIndex = blacksWithMovesIndexes.ElementAt(p1.Num);
                }
                catch (ArgumentOutOfRangeException)
                {
                    Console.Write("white checkers player won!");
                }
                int[] checkerLocation = locations.ElementAt(chosenCheckerIndex);
                int[] moveToLocation = blackCheckersLocationAndMoves.ElementAt(chosenCheckerIndex).ElementAt(0);
                Console.WriteLine("Random Number from server after genrate random in range is: " + p1.Num);
                Board = Piece.Move(Board, checkerLocation[0], checkerLocation[1], moveToLocation[0], moveToLocation[1]);
                UpdatePiecesTaken();
                SetUpColours();
                ShowPieces();
                firstTime = false;
            }

            //if i have selected to move the same place i stand.
            if (selected && Piece_Selected[0] == x && Piece_Selected[1] == y)
            {
                selected = false;
                SetUpColours();
            }
            //if i have selected checker only(before selecting the move). than highlight the possible moves.
            else if (!selected)
            {
                if (Board[x, y] != null)
                {
                    selected = true;
                    Piece_Selected = new int[] { x, y };
                    Buttons[x, y].BackColor = SelectedColour;
                    Highlightpossiblemoves();
                }
            }
            //if i have selected a place to move
            else if (selected)
            {
                int from_x, from_y, to_x, to_y;

                if (Buttons[x, y].BackColor == SelectedColour)
                {
                    from_x = Piece_Selected[0];
                    from_y = Piece_Selected[1];

                    to_x = x;
                    to_y = y;

                    Board = Piece.Move(Board, from_x, from_y, to_x, to_y);
                    selected = false;

                    UpdatePiecesTaken();
                    SetUpColours();
                    ShowPieces();
                    firstTime = true;
                    computersTurn = true;

                    id = p1.Id;
                    locations = GetBlackCheckersLocations();
                    blackCheckersLocationAndMoves = GetPossibleBlackMoves(locations);
                    blacksWithMovesIndexes = onlyBlackWithMoves(blackCheckersLocationAndMoves);
                    await getPlayerWithRandAsync((int)blacksWithMovesIndexes.Count);

                }

            }

        }

        async Task<Player> GetPlayerAsync(string path)
        {
            var formatters = new List<MediaTypeFormatter>() {
    new JsonMediaTypeFormatter(),
    new XmlMediaTypeFormatter()
};

            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                p1 = await response.Content.ReadAsAsync<Player>(formatters);

            }


            return p1;
        }
        private async void getUpdatedPlayer()
        {
            p1 = await GetPlayerAsync("https://localhost:44310/api/TblUsers/" + p1.Id);

        }

        private async Task getPlayerWithRandAsync(int num)
        {
            await Task.Delay(3000);
            p1 = await GetPlayerAsync("https://localhost:44310/api/TblUsers/pname/1/" + num);

        }
        private async void SendBlacksCount(int num)

        {
            p1.Num = num;
            await UpdatePlayerAsync(p1);
        }
        async Task<Player> UpdatePlayerAsync(Player player)
        {
            HttpResponseMessage response = await client.PutAsJsonAsync(
                $"https://localhost:44310/api/TblUsers/{player.Id}", player);
            response.EnsureSuccessStatusCode();

            // Deserialize the updated product from the response body.
            player = await response.Content.ReadAsAsync<Player>();
            return player;
        }
        public void printArray<T>(IEnumerable<T> a)
        {
            foreach (var i in a)
            {
                Console.WriteLine(i);
            }
        }
        private void Highlightpossiblemoves()
        {
            int x, y;

            List<int[]> PossibleMoves = Piece.GetLegalMoves(Board, Piece_Selected[0], Piece_Selected[1]);

            foreach (int[] Move in PossibleMoves)
            {
                x = Move[0];
                y = Move[1];

                Buttons[x, y].BackColor = SelectedColour;
            }
        }

        private void SetUpButtons()
        {
            var bindingFlags = BindingFlags.Instance |
                               BindingFlags.NonPublic |
                               BindingFlags.Public;

            var regex = new Regex(
            "btn(\\d)(\\d)$",
            RegexOptions.CultureInvariant
            | RegexOptions.Compiled);

            this.GetType()
            .GetFields(bindingFlags)
            .ToList()
            .Where(fi => fi.FieldType.Name == "Button" && regex.IsMatch(fi.Name))
            .ToList()
            .ForEach(fi =>
            {
                var m = regex.Match(fi.Name);
                var x = Convert.ToInt16(m.Groups[1].Value);
                var y = Convert.ToInt16(m.Groups[2].Value);

                Buttons[x, y] = (Button)fi.GetValue(this);
            });

            foreach (Button btn in Buttons)
            {
                btn.Click += new System.EventHandler(this.ClickAsync);
                btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            }
        }

        private void SetUpSidePictures()
        {
            //White Pieces
            ImagesW[0] = PicWhite0;
            ImagesW[1] = PicWhite1;
            ImagesW[2] = PicWhite2;
            ImagesW[3] = PicWhite3;
            ImagesW[4] = PicWhite4;
            ImagesW[5] = PicWhite5;
            ImagesW[6] = PicWhite6;
            ImagesW[7] = PicWhite7;
            ImagesW[8] = PicWhite8;
            ImagesW[9] = PicWhite9;
            ImagesW[10] = PicWhite10;
            ImagesW[11] = PicWhite11;

            //Black Pieces
            ImagesB[0] = PicBlack0;
            ImagesB[1] = PicBlack1;
            ImagesB[2] = PicBlack2;
            ImagesB[3] = PicBlack3;
            ImagesB[4] = PicBlack4;
            ImagesB[5] = PicBlack5;
            ImagesB[6] = PicBlack6;
            ImagesB[7] = PicBlack7;
            ImagesB[8] = PicBlack8;
            ImagesB[9] = PicBlack9;
            ImagesB[10] = PicBlack10;
            ImagesB[11] = PicBlack11;
        }

        private void UpdatePiecesTaken()
        {
            int B = 12;
            int W = 12;

            foreach (Piece Piece in Board)
            {
                if (Piece != null)
                {
                    if (Piece.Colour == 0)
                    {
                        B--;
                    }
                    else if (Piece.Colour == 1)
                    {
                        W--;
                    }
                }
            }

            for (int i = 0; W > i; i++)
            {
                ImagesW[i].Visible = true;
            }

            for (int i = 0; B > i; i++)
            {
                ImagesB[i].Visible = true;
            }

        }

        private void SetUpColours()
        {
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Buttons[x, y].BackColor = (x + 1 + y + 1) % 2 == 0 ? FirstColour : SecondColour;
                }
            }
        }

        private void IntialPositions()
        {
            //Player 1
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if ((x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0))
                    {
                        Board[x, y] = new Piece(0);
                    }
                }
            }

            //Player 2
            for (int x = 5; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if ((x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0))
                    {
                        Board[x, y] = new Piece(1);
                    }
                }
            }
        }

        private void ShowPieces()
        {
            int colour;

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (Board[x, y] != null)
                    {
                        colour = Board[x, y].Colour;

                        switch (colour)
                        {
                            case 0: Buttons[x, y].BackgroundImage = Properties.Resources.CheckerBlack1; break;
                            case 1: Buttons[x, y].BackgroundImage = Properties.Resources.CheckerWhite1; break;
                        }
                    }
                    else
                    {
                        Buttons[x, y].BackgroundImage = null;
                    }
                }
            }
        }
        private List<int[]> GetBlackCheckersLocations()
        {
            List<int[]> blackCheckersLocations = new List<int[]>();
            int[] location;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (Board[i, j] != null && Board[i, j].Colour == 0)
                    {
                        location = new int[] { i, j };
                        blackCheckersLocations.Add(location);
                    }

                }

            }
            return blackCheckersLocations;
        }

        private List<List<int[]>> GetPossibleBlackMoves(List<int[]> blackCheckersLocations)
        {
            List<List<int[]>> PossibleMoves = new List<List<int[]>>();

            foreach (int[] location in blackCheckersLocations)
            {
                PossibleMoves.Add(Piece.GetLegalMoves(Board, location[0], location[1]));
            }
            return PossibleMoves;
        }
        private List<int> onlyBlackWithMoves(List<List<int[]>> blackCheckersLocationsAndMoves)
        {
            List<int> blacksWithMoves = new List<int>();

            foreach (List<int[]> black in blackCheckersLocationsAndMoves)
            {
                if (black.Count() > 0)
                {
                    blacksWithMoves.Add(blackCheckersLocationsAndMoves.IndexOf(black));
                }
            }
            Console.WriteLine(String.Join("; ", blacksWithMoves));  // "1; 2; 3"

            return blacksWithMoves;
        }

    }
}
