using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChessMiddle.ChessFactory
{
    class xytq : IChess
    {
        private char[,] _chess;
        public int Width => throw new NotImplementedException();

        public int Height => throw new NotImplementedException();

        public char[] Role => throw new NotImplementedException();

        public char[,] ChessLayout { get => _chess; }

        public string Size => throw new NotImplementedException();

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
    }
}
