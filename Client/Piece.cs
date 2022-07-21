using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Piece
    {
        public int Colour { get; private set; }

        public Piece(int Team)
        {
            Colour = Team;
        }

        private static int[] Piece_taken = null;
        private static int[] TakePieceMove = null;

        //moving checker piece
        public static Piece[,] Move(Piece[,] Board, int from_X, int from_Y, int to_X, int to_Y)
        {
            List<int[]> PossibleMoves = GetLegalMoves(Board, from_X, from_Y);

            foreach (int[] Move in PossibleMoves)
            {
                if (Move[0] == to_X && Move[1] == to_Y)
                {
                    if (Piece_taken != null && TakePieceMove[0] == Move[0] && TakePieceMove[1] == Move[1])
                    {
                        //making the actual eating
                        if (Board[Piece_taken[0], Piece_taken[1]].Colour == 1)
                        {
                            Form2.computerScore++;
                        }
                        else
                        {
                            Form2.playerScore++;
                        }
                        Board[Piece_taken[0], Piece_taken[1]] = null;
                        Piece_taken = null;
                    }

                    Board[to_X, to_Y] = Board[from_X, from_Y];
                    Board[from_X, from_Y] = null;
                    break;
                }
            }

            return Board;
        }
        //checks availble legal moves of checker (Colour = 0 is for black checkers, Colour = 1 is for white checkers)
        public static List<int[]> GetLegalMoves(Piece[,] Board, int X, int Y)
        {
            List<int[]> PossibleMoves = new List<int[]>();
            int[] move;

            //Black checker
            if (Board[X, Y].Colour == 0)
            {
                //if black checker way to - down and left is possible on board
                if (Y - 1 >= 0 && X + 1 < 8)
                {
                    if (Board[X + 1, Y - 1] == null)
                    {
                        move = new int[] { X + 1, Y - 1 };
                        PossibleMoves.Add(move);
                    }

                    //possible to make an eat move
                    else if (Y - 2 >= 0 && X + 2 < 8)
                    {
                        if (Board[X + 2, Y - 2] == null && Board[X + 1, Y - 1].Colour == 1)
                        {
                            move = new int[] { X + 2, Y - 2 };
                            PossibleMoves.Add(move);

                            //pointing to the actual eating
                            Piece_taken = new int[] { X + 1, Y - 1 };
                            TakePieceMove = new int[] { X + 2, Y - 2 };
                        }
                    }
                }
                //if black checker way to - down and right is possible on board
                if (Y + 1 < 8 && X + 1 < 8)
                {
                    if (Board[X + 1, Y + 1] == null)
                    {
                        move = new int[] { X + 1, Y + 1 };
                        PossibleMoves.Add(move);
                    }

                    //possible to make an eat move
                    else if (Y + 2 < 8 && X + 2 < 8)
                    {
                        if (Board[X + 2, Y + 2] == null && Board[X + 1, Y + 1].Colour == 1)
                        {
                            move = new int[] { X + 2, Y + 2 };
                            PossibleMoves.Add(move);

                            Piece_taken = new int[] { X + 1, Y + 1 };
                            TakePieceMove = new int[] { X + 2, Y + 2 };
                        }
                    }
                }
            }
            //White checker
            else if (Board[X, Y].Colour == 1)
            {
                //if white checker way to - up and left is possible on board
                if (Y - 1 >= 0 && X - 1 >= 0)
                {
                    if (Board[X - 1, Y - 1] == null)
                    {
                        move = new int[] { X - 1, Y - 1 };
                        PossibleMoves.Add(move);
                    }

                    //possible to make an eat move
                    else if (Y - 2 >= 0 && X - 2 >= 0)
                    {
                        if (Board[X - 2, Y - 2] == null && Board[X - 1, Y - 1].Colour == 0)
                        {
                            move = new int[] { X - 2, Y - 2 };
                            PossibleMoves.Add(move);

                            //pointing to the actual eating
                            Piece_taken = new int[] { X - 1, Y - 1 };
                            TakePieceMove = new int[] { X - 2, Y - 2 };
                        }
                    }
                }
                //if white checker way to - up and right is possible on board
                if (Y + 1 < 8 && X - 1 >= 0)
                {
                    if (Board[X - 1, Y + 1] == null)
                    {
                        move = new int[] { X - 1, Y + 1 };
                        PossibleMoves.Add(move);
                    }

                    //possible to make an eat move
                    else if (Y + 2 < 8 && X - 2 >= 0)
                    {
                        if (Board[X - 2, Y + 2] == null && Board[X - 1, Y + 1].Colour == 0)
                        {
                            move = new int[] { X - 2, Y + 2 };
                            PossibleMoves.Add(move);

                            Piece_taken = new int[] { X - 1, Y + 1 };
                            TakePieceMove = new int[] { X - 2, Y + 2 };
                        }
                    }
                }
            }
            return PossibleMoves;
        }
    }
}
