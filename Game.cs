using ChessAI.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessAI
{
    class Game
    {
        private Board Board;

        public Game()
        {
            Board = new Board();
        }

        public Board GetBoard()
        {
            return Board;
        }
    }
}
