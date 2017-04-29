using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChessMiddle.ChessFactory
{
    class jingziqi :IChess
    {
        private char[] _chess;
        private char[,] chess2d;
        private int _width;
        private int _height;
        private char[] _role;
        public jingziqi(int width,int height)
        {
            _role = new char[]{ 'x','o','-'};
            _chess = new char[width * height];
            chess2d = new char[width, height];
            _width = width;
            _height = height;

            init();
        }

        private void init()
        {
            int size = Width * Height;
            for(int i = 0; i<size;i++)
            {
                _chess[i] = '-';
            }
        }

        public int Width { get => _width;  }
        public int Height { get => _height;}
        public char[] Role { get => _role;  }
        public char[] Chess { get => _chess; }
        public string Size { get => "" + Width + "-" + Height; }

        /// <summary>
        /// 井子棋下子
        /// </summary>
        /// <param name="actionMove">包含棋步的序列</param>
        /// <param name="role">哪一方</param>
        /// <returns>是否合法</returns>
        public bool DoChess(List<string> actionMove, char role)
        {
            string position = actionMove[0];
            int x = int.Parse(position.Split(',')[0]);
            int y = int.Parse(position.Split(',')[1]);
            if (x <= 0 || x > Width || y <= 0 || y > Height)
                return false;
            if (role == '-' ||Array.IndexOf(Role, role) == -1)
                return false;

            int arrayPosition = (x - 1) * Width + y-1;
            //那个位置为空
            if(_chess[arrayPosition]=='-')
            {
                _chess[arrayPosition] = role;
                return true;
            }
            return false;
        }

        /// <summary>
        /// todo - 检测当前局势
        /// </summary>
        /// <returns></returns>
        public char GetSituation()
        {
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                {
                    chess2d[i, j] = _chess[i * Width + j];
                }

            for (int i=0;i<Width;i++)
            {
                for(int j=0;j<Height;j++)
                {
                    //if(chess2d[i,j]!=chess2d[])
                }
            }
            return 'x';
        }

        public void DefaultDo(char role)
        {
            throw new NotImplementedException();
        }
    }
}
