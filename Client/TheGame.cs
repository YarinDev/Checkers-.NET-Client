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
    public partial class TheGame : Form
    {
        private HttpClient client = new HttpClient();
        Player p1 = new Player();
        Game game = new Game();
        Stopwatch stopWatch = new Stopwatch();
        private List<String> allGameMoves = new List<String>();
        GamesDataContext dbgm = new GamesDataContext();
        TableGames tg = new TableGames();
        TableGames tg2 = new TableGames();


        private List<int[]> locations;
        private List<List<int[]>> blackCheckersLocationAndMoves;
        private List<int> blacksWithMovesIndexes;
        private int chosenCheckerIndex = 0;
        public static int playerScore = 0;
        public static int computerScore = 0;
        private int[] checkerLocation;
        private int[] moveToLocation;


        public TheGame()
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
            game.UserId = player.Id;
            game.Date = DateTime.Now;
            stopWatch.Start();
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

        private async Task GamePlayback(int gameIdForPlayback)
        {
            gameIdForPlayback = 3003;
            //get back client (exampe of GameId 3003) GameId and Moves
            tg2 = dbgm.TableGames.Where(game => game.GameId == gameIdForPlayback).Single();
            Console.WriteLine(tg2.GameId + " " + tg2.Moves);

            List<int> Moves = tg2.Moves.Split(',').Select(int.Parse).ToList();
            int numberOfSteps = Moves.Count / 4;
            int counter = 0;
            for (int i = 0; i < numberOfSteps; i++)
            {
                await Task.Delay(500);
                Board = Piece.Move(Board, Moves[counter], Moves[counter + 1], Moves[counter + 2], Moves[counter + 3]);
                UpdatePiecesTaken();
                SetUpColours();
                ShowPieces();
                counter += 4;
            }

        }

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


        //happens when user is clicking on one of the board's squares, if the square is empty it means board[x,y] is null.
        private async void ClickAsync(object sender, EventArgs e)
        {

            //GamePlayback(3003);

            endGame();

            Button Btn = (Button)sender;
            int x, y;

            //Subtract 48 to convert Char to Int
            x = Convert.ToInt16(Btn.Name[3]) - 48;
            y = Convert.ToInt16(Btn.Name[4]) - 48;

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
                    //String[] playerMove = { from_x.ToString(), from_y.ToString(), to_x.ToString(), to_y.ToString() };
                    allGameMoves.Add(from_x.ToString());
                    allGameMoves.Add(from_y.ToString());
                    allGameMoves.Add(to_x.ToString());
                    allGameMoves.Add(to_y.ToString());
                    selected = false;

                    UpdatePiecesTaken();
                    SetUpColours();
                    ShowPieces();


                    //gathring information of black checkers locations and possible moves.
                    locations = GetBlackCheckersLocations();
                    blackCheckersLocationAndMoves = GetPossibleBlackMoves(locations);
                    blacksWithMovesIndexes = onlyBlackWithMoves(blackCheckersLocationAndMoves);

                    //sending get request for server to get random num for making a random move.
                    await getPlayerWithRandAsync((int)blacksWithMovesIndexes.Count);

                    try
                    {
                        chosenCheckerIndex = blacksWithMovesIndexes.ElementAt(p1.Num);
                        checkerLocation = locations.ElementAt(chosenCheckerIndex);
                        moveToLocation = blackCheckersLocationAndMoves.ElementAt(chosenCheckerIndex).ElementAt(0);
                        Board = Piece.Move(Board, checkerLocation[0], checkerLocation[1], moveToLocation[0], moveToLocation[1]);

                        allGameMoves.Add(checkerLocation[0].ToString());
                        allGameMoves.Add(checkerLocation[1].ToString());
                        allGameMoves.Add(moveToLocation[0].ToString());
                        allGameMoves.Add(moveToLocation[1].ToString());
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        //end game method.
                        endGame();
                    }

                    UpdatePiecesTaken();
                    SetUpColours();
                    ShowPieces();
                    Console.WriteLine("player score is: " + playerScore);
                    Console.WriteLine("computer score is: " + computerScore);
                }

            }

        }

        private async void endGame()
        {


            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;
            game.Length = (int)ts.TotalSeconds;
            string str = String.Join(", ", allGameMoves);
            Console.WriteLine(str);
            if (playerScore >= computerScore)
            {
                game.Winner = p1.Name;
                Console.WriteLine("koko");

                MessageBox.Show(p1.Name + " won!");
                await CreateGameOnServer(game);

                //get most recent game GameId
                await LastGameIdAsync();
            }
            else
            {
                game.Winner = "Computer";
                MessageBox.Show("Computer won!");
                await CreateGameOnServer(game);
                //get most recent game GameId
                await LastGameIdAsync();

            }
            //post new game in client DB with GameId And list of all moves
            tg.GameId = game.GameId;
            tg.Moves = str;
            dbgm.TableGames.InsertOnSubmit(tg);
            dbgm.SubmitChanges();
            //get back client (exampe of first game) GameId and Moves
            tg2 = dbgm.TableGames.ToList().ElementAt(0);
            Console.WriteLine(tg2.GameId + " " + tg2.Moves);
            var combined = string.Join(", ", dbgm.TableGames.ToList());
            Console.WriteLine(combined);



        }
        private async Task LastGameIdAsync()
        {
            game = await GetLastGameAsync("https://localhost:44310/api/TblGames/getlast/1");

        }
        async Task<Game> GetLastGameAsync(string path)
        {
            var formatters = new List<MediaTypeFormatter>() {
    new JsonMediaTypeFormatter(),
    new XmlMediaTypeFormatter()
};

            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                game = await response.Content.ReadAsAsync<Game>(formatters);

            }

            return game;
        }


        private async Task CreateGameOnServer(Game game)
        {
            Uri response = await CreateGameAsync(game);
        }
        async Task<Uri> CreateGameAsync(Game game)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(
                "https://localhost:44310/api/TblGames", game);
            response.EnsureSuccessStatusCode();

            // return URI of the created resource.
            return response.Headers.Location;
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
        private async Task getPlayerWithRandAsync(int num)
        {
            await Task.Delay(500);
            p1 = await GetPlayerAsync("https://localhost:44310/api/TblUsers/pname/" + p1.Id + "/" + num);

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

            return blacksWithMoves;
        }
        private void moveExample()
        {

            Board = Piece.Move(Board, 2, 3, 3, 2);
            UpdatePiecesTaken();
            SetUpColours();
            ShowPieces();
        }
    }
}
