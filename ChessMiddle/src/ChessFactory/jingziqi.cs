using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChessMiddle.ChessFactory
{
    class jingziqi : IChess
    {
        private char NOTHING = '-';
        /// <summary>
        /// 棋局还没结束.
        /// </summary>
        public char NOT_DONE { get => '~'; }
        /// <summary>
        /// 平局
        /// </summary>
        public char DRAW { get => 'd';  }

        private char[] _chess;
        private char[,] _chess2d;
        private int _width;
        private int _height;
        private char[] _role;
        public jingziqi(int width, int height)
        {
            _role = new char[] { 'x', 'o', NOTHING };
            _chess = new char[width * height];
            _chess2d = new char[width, height];
            _width = width;
            _height = height;

            init();
        }

        private void init()
        {
            int size = Width * Height;
            for (int i = 0; i < size; i++)
            {
                _chess[i] = NOTHING;
            }

            for (int i = 0; i < _width; i++)
                for (int j = 0; j < _height; j++)
                {
                    _chess2d[i, j] = NOTHING;
                }
        }

        public int Width { get => _width; }
        public int Height { get => _height; }
        public char[] Role { get => _role; }
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
            if (role == NOTHING || Array.IndexOf(Role, role) == -1)
                return false;

            int arrayPosition = (x - 1) * Width + y - 1;
            //那个位置为空
            if (_chess[arrayPosition] == NOTHING)
            {
                _chess[arrayPosition] = role;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获得当前棋局的结果
        /// </summary>
        /// <returns>代表当前结果的字符</returns>
        public char GetResult()
        {
            int size = Width * Height;
            for (int i = 0; i < size; i++)
            {
                if (_chess[i] == NOTHING)
                    return NOT_DONE;
            }

            //检测某列是否有胜者
            bool isVertical = true;
            for (int i = 0; i < _width; i++)
            {
                for (int j = 0; j < _width - 1; j++)
                {
                    if (_chess2d[j, i] != _chess2d[j+1, i])
                        isVertical = false;
                }
                if (isVertical == true)
                    return _chess2d[i, 0];
                
            }

            //检测某行是否有胜者
            bool isHorizontal = true;
            for (int i = 0; i < _height; i++)
            {
                for (int j = 0; j < _height - 1; j++)
                {
                    if (_chess2d[i, j] != _chess2d[i, j + 1])
                        isHorizontal = false;
                }
                if (isHorizontal == true)
                    return _chess2d[i, 0];
            }

            for(int i=0;i<_width-1;i++)
            {
                bool isSlash = true;
                if (_chess2d[i, i] != _chess2d[i + 1, i + 1])
                    isSlash = false;
                if (isSlash == true)
                    return _chess2d[0, 0];
            }

            for (int i = _width-1; i > 0 ; i--)
            {
                bool isSlash = true;
                if (_chess2d[i, Width-i-1] != _chess2d[i -1, Width - i - 2])
                    isSlash = false;
                if (isSlash == true)
                    return _chess2d[0, _width - 1];
            }

            return DRAW;
        }

        /// <summary>
        /// 执行默认步
        /// </summary>
        /// <param name="role">执行者</param>
        /// <returns>默认步的动作序列</returns>
        public List<string> DefaultDo(char role)
        {
            List<string> action = new List<string>();
            for(int i=0;i<=_width;i++)
            {
                for(int j=0;j<_height;j++)
                {
                    if (_chess2d[i, j] == NOTHING)
                    {
                        //example
                        //在(1,2)的位置加一个x
                        //+/1,2/x
                        action.Add("+/" + i + "," + j + "/" + role);
                        break;
                    }
                }
            }
            return action;
        }
    }
}
