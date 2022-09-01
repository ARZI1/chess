using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessAI.Engine
{
    class Board
    {
        private static readonly ulong FILE_A = 0x0101010101010101;
        private static readonly ulong FILE_B = 0x202020202020202;
        private static readonly ulong FILE_C = 0x0404040404040404;
        private static readonly ulong FILE_D = 0x0808080808080808;
        private static readonly ulong FILE_E = 0x1010101010101010;
        private static readonly ulong FILE_F = 0x2020202020202020;
        private static readonly ulong FILE_G = 0x4040404040404040;
        private static readonly ulong FILE_H = 0x8080808080808080;

        private static readonly ulong RANK_1 = 0x00000000000000FF;
        private static readonly ulong RANK_2 = 0x000000000000FF00;
        private static readonly ulong RANK_3 = 0x0000000000FF0000;
        private static readonly ulong RANK_4 = 0x00000000FF000000;
        private static readonly ulong RANK_5 = 0x000000FF00000000;
        private static readonly ulong RANK_6 = 0x0000FF0000000000;
        private static readonly ulong RANK_7 = 0x00FF000000000000;
        private static readonly ulong RANK_8 = 0xFF00000000000000;

        private static readonly int PIECE_NONE   = -1;
        private static readonly int PAWN_WHITE   = 0;
        private static readonly int KNIGHT_WHITE = 1;
        private static readonly int BISHOP_WHITE = 2;
        private static readonly int ROOK_WHITE   = 3;
        private static readonly int QUEEN_WHITE  = 4;
        private static readonly int KING_WHITE   = 5;
        private static readonly int PAWN_BLACK   = 6;
        private static readonly int KINGHT_BLACK = 7;
        private static readonly int BISHOP_BLACK = 8;
        private static readonly int ROOK_BLACK   = 9;
        private static readonly int QUEEN_BLACK  = 10;
        private static readonly int KING_BLACK   = 11;

        private ulong Occupied;
        private ulong WhiteOccupied;
        private ulong BlackOccupied;

        private int[] Pieces; // stores the integer value of pieces on the board
        private ulong[] PieceBoards; // stores the bitboard of pieces

        public Board()
        {
            Pieces = new int[64];
            for (int i = 0; 64 > i; i++)
                Pieces[i] = PIECE_NONE;

            PieceBoards = new ulong[12];
        }

        public void Print()
        {
            for (int y = 0; 8 > y; y++)
            {
                for (int x = 0; 8 > x; x++)
                {
                    int index = 8 * (7 - y) + x;
                    if (Pieces[index] == PIECE_NONE)
                        Console.Write(' ');
                    else
                        Console.Write(Pieces[index]);

                    if (x != 7)
                        Console.Write(" | ");
                }
                Console.WriteLine();
            }
        }

        /* ------------------------------ */
        // Piece information
        /* ------------------------------ */
        
        public bool IsPieceAt(int index)
        {
            return (Occupied & (1ul << index)) != 0;
        }

        public int GetPieceAt(int index)
        {
            return Pieces[index];
        }

        private bool IsWhitePiece(int index)
        {
            return Pieces[index] < PAWN_BLACK;
        }

        public int[] GetPieces()
        {
            return Pieces;
        }

        /* ------------------------------ */
        // Piece movement
        /* ------------------------------ */

        /**
         * Moves a piece to a new location
         * We assume there is a piece to be moved
         * param from - the piece's origin
         * param to - the piece's destination
         */
        public void MovePiece(int from, int to)
        {
            bool capture = IsPieceAt(to);
            bool white = IsWhitePiece(from);


            // update occupied data

            Occupied |= 1ul << to;
            Occupied &= ~(1ul << from);
            if (white)
            {
                WhiteOccupied |= 1ul << to;
                WhiteOccupied &= ~(1ul << from);
                if (capture)
                    BlackOccupied &= ~(1ul << from);
            }
            else
            {
                BlackOccupied |= 1ul << to;
                BlackOccupied &= ~(1ul << from);
                if (capture)
                    WhiteOccupied &= ~(1ul << from);
            }


            // update piece data

            // remove pieces from board
            PieceBoards[Pieces[from]] &= ~(1ul << from);
            if (capture)
                PieceBoards[Pieces[to]] &= ~(1ul << to);


            // add piece to new location
            Pieces[to] = Pieces[from];
            Pieces[from] = PIECE_NONE;
            PieceBoards[Pieces[to]] |= 1ul << to;
        }

        public void SetPiece(int piece, int loc)
        {
            if (piece == PIECE_NONE)
                return;

            Occupied |= 1ul << loc;
            if (IsWhitePiece(piece))
                WhiteOccupied |= 1ul << loc;
            else
                BlackOccupied |= 1ul << loc;

            Pieces[loc] = piece;
            PieceBoards[piece] |= 1ul << loc;
        }

        /* ------------------------------ */
        // Miscellaneous
        /* ------------------------------ */

        /**
         * Loads a FEN string's data 
         */
        public void LoadFen(string FEN)
        {
            string[] parts = FEN.Split(' ');

            string[] ranks = parts[0].Split('/');
            for (int rank = 0; 8 > rank; rank++)
            {
                for (int row = 0; 8 > row; row++)
                {
                    char c = ranks[7 - rank][row];
                    if (char.IsLetter(c))
                        SetPiece(CharToPiece(c), 8 * rank + row);
                    else
                        row += c - '0';
                }
            }
        }

        private int CharToPiece(Char c)
        {
            int piece = char.IsLower(c) ? PAWN_BLACK : PAWN_WHITE;
            switch(Char.ToUpper(c))
            {
                case 'P':
                    piece += PAWN_WHITE;
                    break;
                case 'N':
                    piece += KNIGHT_WHITE;
                    break;
                case 'B':
                    piece += BISHOP_WHITE;
                    break;
                case 'R':
                    piece += ROOK_WHITE;
                    break;
                case 'Q':
                    piece += QUEEN_WHITE;
                    break;
                case 'K':
                    piece += KING_WHITE;
                    break;
            }

            return piece;
        }
    }
}
