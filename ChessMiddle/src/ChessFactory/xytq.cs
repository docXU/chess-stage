using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ChessMiddle.ChessFactory
{
    public class Xytq : IChess
    {

        public char EMPTY { get => '0'; }
        public char NOT_DONE { get => '~'; }
        public char DRAW { get => 'd'; }

        private char[,] _chessLayout;
        private int _width;
        private int _height;
        private char[] _role;

        private char BLACK;
        private char BLACK_KING;
        private char WHITE;
        private char WHITE_KING;

        public int Width { get => _width; }
        public int Height { get => _height; }
        public char[] Role { get => _role; }
        public char[,] ChessLayout { get => _chessLayout; }
        public string Size => Width + "-" + Height;

        public Xytq()
        {
            _width = 8;
            _height = 8;
            _role = new char[] { 'a', 'b' };
            BLACK = 'b';
            BLACK_KING = 'B';
            WHITE = 'a';
            WHITE_KING = 'A';

            init();
        }

        public Xytq(char[,] layout)
        {
            _width = 8;
            _height = 8;
            _role = new char[] { 'a', 'b' };
            BLACK = 'b';
            BLACK_KING = 'B';
            WHITE = 'a';
            WHITE_KING = 'A';

            _chessLayout = layout;
        }

        private void init()
        {
            _chessLayout = new char[8, 8]{
                {'0','a','0','a','0','a','0','a' },
                {'a','0','a','0','a','0','a','0' },
                {'0','a','0','a','0','a','0','a' },
                {'0','0','0','0','0','0','0','0' },
                {'0','0','0','0','0','0','0','0' },
                {'b','0','b','0','b','0','b','0' },
                {'0','b','0','b','0','b','0','b' },
                {'b','0','b','0','b','0','b','0' }
            };
        }

        public List<string> DefaultDo(char role)
        {
            throw new NotImplementedException();
        }

        public bool DoChess(List<string> actionMove, char role)
        {
            throw new NotImplementedException();
        }

        public char GetResult()
        {
            throw new NotImplementedException();
        }

        public List<char[,]> NextLayout(char player, char[,] layout)
        {

            //add all possible position
            List<char[,]> layoutList = new List<char[,]>();
            layoutList.Add(layout);
            //add all empty position
            List<char[,]> emptyList = new List<char[,]>();
            //add all rival position
            List<char[,]> rivalList = new List<char[,]>();

            char rival = WHITE;
            int nextLine = -1;
            char playerKing = BLACK_KING;
            char rivalKing = WHITE_KING;
            if (player == WHITE)
            {
                rival = BLACK;
                nextLine = 1;
                playerKing = WHITE_KING;
                rivalKing = BLACK_KING;
            }

            int LENGTH = _width;
            for (int i = 0; i < _width; i++)
            {
                for (int j = 0; j < _height; j++)
                {
                    if (layout[i, j] == player || layout[i, j] == playerKing)
                    {
                        if (layout[i, j] == playerKing)
                        {
                            player = playerKing;
                        }
                        //left positon
                        if (i + nextLine < _width && i + nextLine >= 0 && j - 1 >= 0)
                        {
                            //deep clone
                            if (layout[i + nextLine, j - 1] == EMPTY)
                            {
                                char[,] t = new char[LENGTH, LENGTH];

                                deepClone(t, layout);

                                t[i + nextLine, j - 1] = player;
                                t[i, j] = EMPTY;
                                //add to list
                                emptyList.Add(t);
                            }
                            if (layout[i + nextLine, j - 1] == rival || layout[i + nextLine, j - 1] == rivalKing)
                            {
                                if (i + 2 * nextLine < LENGTH && i + 2 * nextLine >= 0 && j - 2 >= 0)
                                {
                                    if (layout[i + 2 * nextLine, j - 2] == EMPTY)
                                    {
                                        char[,] t = new char[LENGTH, LENGTH];

                                        deepClone(t, layout);

                                        t[i + 2 * nextLine, j - 2] = player;
                                        t[i + nextLine, j - 1] = EMPTY;
                                        t[i, j] = EMPTY;

                                        continueJump(t, i + 2 * nextLine, j - 2, player, rival, rivalList);
                                        //rivalList.add(t);
                                    }
                                }
                            }
                        }

                        //right position
                        if (i + nextLine < LENGTH && i + nextLine >= 0 && j + 1 < LENGTH)
                        {
                            if (layout[i + nextLine, j + 1] == EMPTY)
                            {
                                //get next layout
                                char[,] t = new char[LENGTH, LENGTH];

                                deepClone(t, layout);

                                t[i + nextLine, j + 1] = player;
                                t[i, j] = EMPTY;
                                //add to list
                                emptyList.Add(t);
                            }
                            if (layout[i + nextLine, j + 1] == rival || layout[i + nextLine, j + 1] == rivalKing)
                            {
                                if (i + 2 * nextLine < LENGTH && i + 2 * nextLine >= 0 && j + 2 < LENGTH)
                                {
                                    if (layout[i + 2 * nextLine, j + 2] == EMPTY)
                                    {
                                        char[,] t = new char[LENGTH, LENGTH];

                                        deepClone(t, layout);

                                        t[i + 2 * nextLine, j + 2] = player;
                                        t[i + nextLine, j + 1] = EMPTY;
                                        t[i, j] = EMPTY;

                                        continueJump(t, i + 2 * nextLine, j + 2, player, rival, rivalList);
                                        //rivalList.add(t);
                                    }
                                }
                            }
                        }
                    }

                    if (layout[i, j] == playerKing)
                    {
                        //left positon
                        if (i - nextLine < LENGTH && i - nextLine >= 0 && j - 1 >= 0)
                        {
                            //deep clone
                            if (layout[i - nextLine, j - 1] == EMPTY)
                            {
                                char[,] t = new char[LENGTH, LENGTH];

                                deepClone(t, layout);

                                t[i - nextLine, j - 1] = player;
                                t[i, j] = EMPTY;
                                //add to list
                                emptyList.Add(t);
                            }
                            if (layout[i - nextLine, j - 1] == rival || layout[i - nextLine, j - 1] == rivalKing)
                            {
                                if (i - 2 * nextLine < LENGTH && i - 2 * nextLine >= 0 && j - 2 >= 0)
                                {
                                    if (layout[i - 2 * nextLine, j - 2] == EMPTY)

                                    {
                                        char[,] t = new char[LENGTH, LENGTH];

                                        deepClone(t, layout);
                                        t[i - 2 * nextLine, j - 2] = player;
                                        t[i - nextLine, j - 1] = EMPTY;
                                        t[i, j] = EMPTY;

                                        continueJump(t, i - 2 * nextLine, j - 2, player, rival, rivalList);
                                        //rivalList.add(t);
                                    }
                                }
                            }
                        }

                        //right position
                        if (i - nextLine < LENGTH && i - nextLine >= 0 && j + 1 < LENGTH)
                        {
                            if (layout[i - nextLine, j + 1] == EMPTY)
                            {
                                //get next layout
                                char[,] t = new char[LENGTH, LENGTH];

                                deepClone(t, layout);

                                t[i - nextLine, j + 1] = player;
                                t[i, j] = EMPTY;
                                //add to list
                                emptyList.Add(t);
                            }
                            if (layout[i - nextLine, j + 1] == rival || layout[i - nextLine, j + 1] == rivalKing)
                            {
                                if (i - 2 * nextLine < LENGTH && i - 2 * nextLine >= 0 && j + 2 < LENGTH)
                                {
                                    if (layout[i - 2 * nextLine, j + 2] == EMPTY)

                                    {
                                        char[,] t = new char[LENGTH, LENGTH];

                                        deepClone(t, layout);
                                        t[i - 2 * nextLine, j + 2] = player;
                                        t[i - nextLine, j + 1] = EMPTY;
                                        t[i, j] = EMPTY;

                                        continueJump(t, i - 2 * nextLine, j + 2, player, rival, rivalList);
                                        //rivalList.add(t);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //not exist compulsory layout
            if (rivalList.Count == 0)
            {
                layoutList.AddRange(emptyList);
            }
            else
            {
                layoutList.AddRange(rivalList);
            }

            return layoutList;
        }

        public void continueJump(char[,] layout, int x, int y, char player, int rival, List<char[,]> a)
        {
            int flag = 0;
            int LENGTH = _width;
            if (player == BLACK || player == WHITE_KING || player == BLACK_KING)
            {
                if (x - 2 >= 0 && y - 2 >= 0 && layout[x - 1, y - 1] == rival && layout[x - 2, y - 2] == EMPTY)
                {
                    flag = 1;
                    char[,] t = new char[LENGTH, LENGTH];

                    deepClone(t, layout);
                    t[x - 2, y - 2] = player;
                    t[x - 1, y - 1] = EMPTY;
                    t[x, y] = EMPTY;

                    continueJump(t, x - 2, y - 2, player, rival, a);
                    //a.add(t);
                }
                if (x - 2 >= 0 && y + 2 < LENGTH && layout[x - 1, y + 1] == rival && layout[x - 2, y + 2] == EMPTY)
                {
                    flag = 1;
                    char[,] t = new char[LENGTH, LENGTH];

                    deepClone(t, layout);
                    t[x - 2, y + 2] = player;
                    t[x - 1, y + 1] = EMPTY;
                    t[x, y] = EMPTY;

                    continueJump(t, x - 2, y + 2, player, rival, a);
                    //a.add(t);
                }
            }
            if (player == WHITE || player == WHITE_KING || player == BLACK_KING)
            {
                if (x + 2 < LENGTH && y - 2 >= 0 && layout[x + 1, y - 1] == rival && layout[x + 2, y - 2] == EMPTY)
                {
                    flag = 1;
                    char[,] t = new char[LENGTH, LENGTH];

                    deepClone(t, layout);
                    t[x + 2, y - 2] = player;
                    t[x + 1, y - 1] = EMPTY;
                    t[x, y] = EMPTY;

                    continueJump(t, x + 2, y + 2, player, rival, a);
                }
                if (x + 2 < LENGTH && y + 2 < LENGTH && layout[x + 1, y + 1] == rival && layout[x + 2, y + 2] == EMPTY)
                {
                    flag = 1;
                    char[,] t = new char[LENGTH, LENGTH];

                    deepClone(t, layout);
                    t[x + 2, y + 2] = player;
                    t[x + 1, y + 1] = EMPTY;
                    t[x, y] = EMPTY;

                    continueJump(t, x + 2, y + 2, player, rival, a);
                }
            }

            if (0 == flag)
            {
                a.Add(layout);
            }
            return;
        }

        public void deepClone(char[,] a, char[,] b)
        {
            int LENGTH = _width;
            for (int i = 0; i < LENGTH; i++)
            {
                for (int j = 0; j < LENGTH; j++)
                {
                    a[i, j] = b[i, j];
                }
            }
        }

        public void print(char[,] layout)
        {
            int LENGTH = _width;
            for (int i = 0; i < LENGTH; i++)
            {
                Console.Write("|");
                for (int j = 0; j < LENGTH; j++)
                {
                    if (layout[i,j] == EMPTY)
                    {
                        Console.Write(" " + "|");
                    }
                    if (layout[i,j] == WHITE)
                    {
                        Console.Write("o" + "|");
                    }
                    if (layout[i,j] == WHITE_KING)
                    {
                        Console.Write("@" + "|");
                    }
                    if (layout[i,j] == BLACK)
                    {
                        Console.Write("*" + "|");
                    }
                    if (layout[i,j] == BLACK_KING)
                    {
                        Console.Write("&" + "|");
                    }
                }
                Console.WriteLine();
                Console.WriteLine("----------------------");

            }
            Console.WriteLine();
            Console.WriteLine("----------this is line------------");
            Console.WriteLine();
        }


    }
}
