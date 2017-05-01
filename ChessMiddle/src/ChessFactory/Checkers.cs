using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChessMiddle.ChessFactory
{
    //todo - 实现函数功能(井子棋已实现)
    class Checkers : IChess
    {
        /// <summary>
        /// 棋局还没结束.
        /// </summary>
        public char NOT_DONE { get => '~'; }

        private char[] _chess;
        private char[,] _chess2d;
        private int _width;
        private int _height;
        private char[] _role;


        public Checkers(int width, int height)
        {
            
            _chess = new char[width * height];
            _chess2d = new char[width, height];
            _width = width;
            _height = height;

            init();
        }

        private void init()
        {
            _role = new char[] { '1', '2' };
            int size = Width * Height;
            for (int i = 0; i < size; i++)
            {
                _chess[i] = '0';
            }
        }

        public int Width { get => _width; }

        public int Height { get => _height; }

        public char[] Role { get => _role; }

        public char[] Chess { get => _chess;  }

        public string Size { get => "" + Width + "-" + Height; }

        public bool DoChess(List<string> actionMove, char role)
        {
            return false;
        }

        public char GetResult()
        {
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                {
                    _chess2d[i, j] = _chess[i * Width + j];
                }

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    //if(chess2d[i,j]!=chess2d[])
                }
            }
            return 'x';
        }

        public List<string> DefaultDo(char role)
        {
            throw new NotImplementedException();
        }
    }
}
