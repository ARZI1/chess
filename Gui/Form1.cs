using ChessAI.Engine;
using ChessAI.Gui;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessAI
{
    public partial class Chess : Form
    {
        //TODO: fix holding piece icon background deformity
        private Image[] IconImgs;

        private Game Game;

        private Panel[] ChessSquares;
        private PictureBox[] PieceIcons;

        private int HoldingPiece;
        private int HoldingOrigin;
        private static readonly int NULL_ORIGIN = -1;
        private PictureBox HoldingIcon;

        public Chess()
        {
            Game = Program.Game;

            InitializeComponent();
            CreateChessBoard();
            LoadIcons();

            HoldingPiece = -1;
            HoldingOrigin = NULL_ORIGIN;
            
            SyncPeicesFromBoard();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void IconFollowMouse(Object sender, EventArgs e)
        {
            if (!IsHoldingPiece())
                return;

            Point pos = PointToClient(MousePosition);
            HoldingIcon.Location = new Point(pos.X - 30, pos.Y - 30);
        }


        private void OnClick(Object sender, EventArgs e)
        {
            // event arguments contain the mouse position relative to the original panel 
            Point mousePos = PointToClient(MousePosition);
            int pieceIndex = GetSquareIndexFromMouse(mousePos);

            //TODO: fix out of bounds for board boarder
            if (!(64 > pieceIndex && pieceIndex >= 0)) // out of sqares' bounds
            {
                PutBackHolding();
                return;
            }

            if (IsHoldingPiece())
            {
                if (!PlacePiece(pieceIndex))
                {
                    PutBackHolding();
                }
            } 
            else
            {
                PickUpPiece(pieceIndex);
            }
        }

        private void PickUpPiece(int from)
        {
            int piece = GetBoard().GetPieceAt(from);

            if (!IsPiece(piece))
                return;

            HoldingPiece = piece;
            HoldingOrigin = from;

            RemoveIcon(ChessSquares[from]);

            HoldingIcon.Image = IconImgs[piece];
            HoldingIcon.Refresh();
            HoldingIcon.Visible = true;
        }

        private bool PlacePiece(int to)
        {
            if (to == HoldingOrigin)
                return false;

            if (false) // TODO: add canmove check
                return false;

            GetBoard().MovePiece(HoldingOrigin, to);
            SetIcon(ChessSquares[to], IconImgs[HoldingPiece]);

            HoldingPiece = -1;
            HoldingOrigin = NULL_ORIGIN;

            HoldingIcon.Image = null;
            HoldingIcon.Refresh();
            HoldingIcon.Visible = false;

            return true;
        }

        private void PutBackHolding()
        {
            if (!IsHoldingPiece())
                return;

            SetIcon(ChessSquares[HoldingOrigin], IconImgs[HoldingPiece]);

            HoldingPiece = -1;
            HoldingOrigin = NULL_ORIGIN;

            HoldingIcon.Image = null;
            HoldingIcon.Refresh();
            HoldingIcon.Visible = false;
        }

        private bool IsHoldingPiece()
        {
            return IsPiece(HoldingPiece);
        }

        private int GetSquareIndexFromMouse(Point point)
        {
            return (8 * (7 - point.Y / 64)) + ((point.X - 64) / 64);
        }

        private void SetIcon(Panel sqaure, Image iconImage)
        {
            int index = (int)sqaure.Tag;
            PictureBox icon = PieceIcons[index];

            icon.Image = iconImage;
            icon.Refresh();
            icon.Visible = true;
        }

        private void RemoveIcon(Panel sqaure)
        {
            int index = (int)sqaure.Tag;
            PictureBox icon = PieceIcons[index];

            icon.Image = null;
            icon.Refresh();
            icon.Visible = false;
        }

        private void SyncPeicesFromBoard()
        {
            int[] Pieces = GetBoard().GetPieces();
            for (int i = 0; 64 > i; i++)
            {
                if (IsPiece(Pieces[i]))
                    SetIcon(ChessSquares[i], IconImgs[Pieces[i]]);
                else
                    RemoveIcon(ChessSquares[i]);
            }
        }

        private Board GetBoard()
        {
            return Game.GetBoard();
        }

        private bool IsPiece(int piece)
        {
            return piece != -1;
        }

        private void CreateChessBoard()
        {
            // border position information
            for (int y = 0; 8 > y; y++) // ranks
            {
                Label label = new Label();

                label.Text = $"{8 - y}";
                label.TextAlign = ContentAlignment.MiddleCenter;
                label.Font = new Font("Arial", 20);
                label.BackColor = Color.DarkGray;

                label.Width = 64;
                label.Height = 64;
                label.Location = new Point(0, y * label.Height);

                this.Controls.Add(label);
            }

            for (int x = 1; 8 >= x; x++) // files
            {
                Label label = new Label();

                label.Text = $"{(char)('A' + x - 1)}";
                label.TextAlign = ContentAlignment.MiddleCenter;
                label.Font = new Font("Arial", 20);
                label.BackColor = Color.DarkGray;

                label.Width = 64;
                label.Height = 64;
                label.Location = new Point(x * label.Width, 8 * label.Height);

                this.Controls.Add(label);
            }

            // Actual squares
            ChessSquares = new Panel[64];
            for (int x = 0; 8 > x; x++)
            {
                for (int y = 0; 8 > y; y++)
                {
                    int index = 8 * (7 - y) + x;
                    Panel panel = new Panel();

                    panel.Tag = index;

                    panel.Width = 64;
                    panel.Height = 64;
                    panel.Location = new Point((x + 1) * panel.Width, y * panel.Height);

                    panel.BackColor = (x + y) % 2 == 0 ? Color.FromArgb(208, 223, 244) : Color.FromArgb(75, 100, 138);

                    panel.MouseDown += OnClick;
                    panel.MouseUp += OnClick;
                    panel.MouseMove += IconFollowMouse;

                    ChessSquares[index] = panel;
                    this.Controls.Add(panel);
                }
            }

            // Initialize icons
            PieceIcons = new PictureBox[64];
            for (int x = 0; 8 > x; x++)
            {
                for (int y = 0; 8 > y; y++)
                {
                    int index = 8 * (7 - y) + x;
                    PictureBox icon = new PictureBox();

                    icon.Visible = false;

                    icon.MouseDown += OnClick;
                    icon.MouseUp += OnClick;
                    icon.MouseMove += IconFollowMouse;

                    PieceIcons[index] = icon;
                    ChessSquares[index].Controls.Add(icon);
                }
            }

            HoldingIcon = new MovingIcon();

            HoldingIcon.BackColor = Color.Transparent;
            HoldingIcon.Visible = false;

            HoldingIcon.MouseDown += OnClick;
            HoldingIcon.MouseUp += OnClick;
            HoldingIcon.MouseMove += IconFollowMouse;

            this.Controls.Add(HoldingIcon);
            HoldingIcon.BringToFront();
        }

        private void LoadIcons()
        {
            IconImgs = new Image[12];
            IconImgs[0] = Image.FromFile("../../Assets/Pieces/PawnWhite.png");
            IconImgs[1] = Image.FromFile("../../Assets/Pieces/KnightWhite.png");
            IconImgs[2] = Image.FromFile("../../Assets/Pieces/BishopWhite.png");
            IconImgs[3] = Image.FromFile("../../Assets/Pieces/RookWhite.png");
            IconImgs[4] = Image.FromFile("../../Assets/Pieces/QueenWhite.png");
            IconImgs[5] = Image.FromFile("../../Assets/Pieces/KingWhite.png");
            IconImgs[6] = Image.FromFile("../../Assets/Pieces/PawnBlack.png");
            IconImgs[7] = Image.FromFile("../../Assets/Pieces/KnightBlack.png");
            IconImgs[8] = Image.FromFile("../../Assets/Pieces/BishopBlack.png");
            IconImgs[9] = Image.FromFile("../../Assets/Pieces/RookBlack.png");
            IconImgs[10] = Image.FromFile("../../Assets/Pieces/QueenBlack.png");
            IconImgs[11] = Image.FromFile("../../Assets/Pieces/KingBlack.png");
        }
    }
}
