﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;


namespace ChessMiddle.ChessFactory
{
    class Xytq : IChess
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
            Dictionary<List<string>, char[,]> Next = NextLayout(role, _chessLayout);
            int moveableCount = Next.Keys.Count;

            int randomI = new Random().Next(0, moveableCount);
            return Next.ElementAt(randomI).Key;
        }

        public bool DoChess(List<string> actionMove, char role)
        {
            Dictionary<List<string>, char[,]> next = NextLayout(role, _chessLayout);
            foreach (List<string> ableMove in next.Keys)
            {
                if (actionMove.Equals(ableMove))
                {   
                    _chessLayout = next[ableMove];
                    return true;
                }
            }

            return false;
        }

        public char GetResult()
        {
            Dictionary<List<string>, char[,]> RoleFirstNext = NextLayout(Role[0], _chessLayout);
            if(RoleFirstNext.Keys.Count == 0)
            {
                return Role[1];
            }

            Dictionary<List<string>, char[,]> RoleSecondNext = NextLayout(Role[1], _chessLayout);
            if (RoleSecondNext.Keys.Count == 0)
            {
                return Role[0];
            }

            if(RoleFirstNext.Keys.Count!=0 && RoleSecondNext.Keys.Count != 0)
            {
                return NOT_DONE;
            }

            return DRAW;
        }


        /// <summary>
        /// 返回棋局的下一个可行解.包含解的解的步骤和棋局
        /// </summary>
        /// <param name="player"></param>
        /// <param name="layout"></param>
        /// <returns></returns>
        public Dictionary<List<string>, char[,]> NextLayout(char player, char[,] layout)
        {

            //add all possible position
            Dictionary<List<string>, char[,]> layoutList = new Dictionary<List<string>, char[,]>();
            List<string> nullList = new List<string>();
            nullList.Add("");
            layoutList.Add(nullList, layout);
            //add all empty position
            Dictionary<List<string>, char[,]> emptyList = new Dictionary<List<string>, char[,]>();
            //add all rival position
            Dictionary<List<string>, char[,]> rivalList = new Dictionary<List<string>, char[,]>();


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

                                List<string> actionMove = new List<string>();
                                actionMove.Add("" + i + "," + j + "-" + (i + nextLine) + "," + (j - 1));
                                //add to list
                                emptyList.Add(actionMove, t);

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

                                        List<string> actionMove = new List<string>();
                                        actionMove.Add("" + i + "," + j + "--" + (i + 2 * nextLine) + "," + (j - 2));

                                        continueJump(t, i + 2 * nextLine, j - 2, player, rival, rivalList, actionMove);
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

                                List<string> actionMove = new List<string>();
                                actionMove.Add("" + i + "," + j + "-" + (i + nextLine) + "," + (j + 1));
                                //add to list
                                emptyList.Add(actionMove, t);

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

                                        List<string> actionMove = new List<string>();
                                        actionMove.Add("" + i + "," + j + "--" + (i + 2 * nextLine) + "," + (j + 2));

                                        continueJump(t, i + 2 * nextLine, j + 2, player, rival, rivalList, actionMove);
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

                                List<string> actionMove = new List<string>();
                                actionMove.Add("" + i + "," + j + "-" + (i - nextLine) + "," + (j - 1));
                                //add to list
                                emptyList.Add(actionMove, t);
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

                                        List<string> actionMove = new List<string>();
                                        actionMove.Add("" + i + "," + j + "--" + (i - 2 * nextLine) + "," + (j - 2));

                                        continueJump(t, i - 2 * nextLine, j - 2, player, rival, rivalList, actionMove);
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

                                List<string> actionMove = new List<string>();
                                actionMove.Add("" + i + "," + j + "-" + (i - nextLine) + "," + (j + 1));
                                //add to list
                                emptyList.Add(actionMove, t);

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

                                        List<string> actionMove = new List<string>();
                                        actionMove.Add("" + i + "," + j + "--" + (i - 2 * nextLine) + "," + (j + 2));

                                        continueJump(t, i - 2 * nextLine, j + 2, player, rival, rivalList, actionMove);
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
                foreach (List<string> am in emptyList.Keys)
                {
                    layoutList.Add(am, emptyList[am]);
                }
            }
            else
            {
                foreach (List<string> am in rivalList.Keys)
                {
                    layoutList.Add(am, rivalList[am]);
                }
            }

            return layoutList;
        }

        public void continueJump(char[,] layout, int x, int y, char player, int rival, Dictionary<List<string>, char[,]> a, List<string> actionMove)
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

                    List<string> newActionMove = Clone<string>(actionMove);
                    newActionMove.Add("" + x + "," + y + "--" + (x - 2) + "," + (y - 2));
                    continueJump(t, x - 2, y - 2, player, rival, a, newActionMove);
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

                    List<string> newActionMove = Clone<string>(actionMove);
                    newActionMove.Add("" + x + "," + y + "--" + (x - 2) + "," + (y + 2));
                    continueJump(t, x - 2, y + 2, player, rival, a, newActionMove);
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

                    List<string> newActionMove = Clone<string>(actionMove);
                    newActionMove.Add("" + x + "," + y + "--" + (x + 2) + "," + (y - 2));
                    continueJump(t, x + 2, y + 2, player, rival, a, newActionMove);
                }
                if (x + 2 < LENGTH && y + 2 < LENGTH && layout[x + 1, y + 1] == rival && layout[x + 2, y + 2] == EMPTY)
                {
                    flag = 1;
                    char[,] t = new char[LENGTH, LENGTH];

                    deepClone(t, layout);
                    t[x + 2, y + 2] = player;
                    t[x + 1, y + 1] = EMPTY;
                    t[x, y] = EMPTY;

                    List<string> newActionMove = Clone<string>(actionMove);
                    newActionMove.Add("" + x + "," + y + "--" + (x + 2) + "," + (y + 2));
                    continueJump(t, x + 2, y + 2, player, rival, a, newActionMove);
                }

            }

            if (0 == flag)
            {
                a.Add(actionMove, layout);
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

        /// <summary>
        /// Clones the specified list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List">The list.</param>
        /// <returns>List{``0}.</returns>
        public static List<T> Clone<T>(object List)
        {
            using (Stream objectStream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, List);
                objectStream.Seek(0, SeekOrigin.Begin);
                return formatter.Deserialize(objectStream) as List<T>;
            }
        }

        public void print(char[,] layout, List<string> am)
        {
            int LENGTH = _width;
            for (int i = 0; i < LENGTH; i++)
            {
                Console.Write("|");
                for (int j = 0; j < LENGTH; j++)
                {
                    if (layout[i, j] == EMPTY)
                    {
                        Console.Write(" " + "|");
                    }
                    if (layout[i, j] == WHITE)
                    {
                        Console.Write("a" + "|");
                    }
                    if (layout[i, j] == WHITE_KING)
                    {
                        Console.Write("A" + "|");
                    }
                    if (layout[i, j] == BLACK)
                    {
                        Console.Write("b" + "|");
                    }
                    if (layout[i, j] == BLACK_KING)
                    {
                        Console.Write("B" + "|");
                    }
                }
                Console.WriteLine();
                Console.WriteLine("-----------------");
            }
            foreach (string a in am)
                Console.WriteLine(a);

            Console.WriteLine();
            Console.WriteLine("----------this is line------------");
            Console.WriteLine();
        }

    }
}
