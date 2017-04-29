using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChessMiddle.ChessFactory
{
    //todo - 实现函数功能
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
            return '1';
        }

        public List<string> DefaultDo(char role)
        {
            throw new NotImplementedException();
        }
    }
}
