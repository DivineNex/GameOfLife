using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameOfLife
{
    public partial class Form1 : Form
    {
        private int _currentGeneration = 0;
        private Graphics _graphics;
        private int _resolution;
        private bool[,] _field;
        private int _rows;
        private int _cols;
        private SolidBrush _brush;

        public Form1()
        {
            InitializeComponent();
        }

        private void StartGame()
        {
            if (gameTimer.Enabled)
                return;

            _currentGeneration = 0;
            Text = $"Current generation: {_currentGeneration}";

            nudResolution.Enabled = false;
            nudDensity.Enabled = false;
            _resolution = (int)nudResolution.Value;

            _rows = pictureBox1.Height / _resolution;
            _cols = pictureBox1.Width / _resolution;

            _field = new bool[_cols, _rows];
            _brush = new SolidBrush(Color.FromArgb(100, 87, 166));

            Random random = new Random();
            for (int x = 0; x < _cols; x++)
            {
                for (int y = 0; y < _rows; y++)
                {
                    _field[x, y] = random.Next((int)nudDensity.Value) == 0;
                }
            }

            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            _graphics = Graphics.FromImage(pictureBox1.Image);
            gameTimer.Start();
        }

        private void StopGame()
        {
            if (!gameTimer.Enabled)
                return;
            gameTimer.Stop();
            nudResolution.Enabled = true;
            nudDensity.Enabled = true;
        }

        private void NextGeneration()
        {
            _graphics.Clear(Color.FromArgb(157, 172, 255));

            var newField = new bool[_cols, _rows];

            for (int x = 0; x < _cols; x++)
            {
                for (int y = 0; y < _rows; y++)
                {
                    var neighborCount = CountNeighbors(x, y);
                    var hasLife = _field[x, y];

                    if (!hasLife && neighborCount == 3)
                       newField[x, y] = true;
                    else if (hasLife && neighborCount < 2 || neighborCount > 3)
                       newField[x, y] = false;
                    else
                       newField[x, y] = _field[x, y];

                    if (hasLife)
                       _graphics.FillRectangle(_brush, x * _resolution, y * _resolution, _resolution, _resolution);
                }
            }
            _field = newField;
            pictureBox1.Refresh();
            Text = $"Current generation: {++_currentGeneration}";
        }

        private int CountNeighbors(int x, int y)
        {
            int count = 0;

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    var col = (x + i + _cols) % _cols;
                    var row = (y + j + _rows) % _rows;

                    var isSelfChecking = col == x && row == y;
                    var hasLife = _field[col, row];

                    if (hasLife && !isSelfChecking)
                        count++;
                }
            }

            return count;
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void bStart_Click(object sender, EventArgs e)
        {
            StartGame();
        }

        private void bStop_Click(object sender, EventArgs e)
        {
            StopGame();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!gameTimer.Enabled)
                return;

            if (e.Button != MouseButtons.None)
            {
                var x = e.Location.X / _resolution;
                var y = e.Location.Y / _resolution;

                if (ValidateMousePosition(x, y))
                {
                    if (e.Button == MouseButtons.Left)
                        _field[x, y] = true;
                    else if (e.Button == MouseButtons.Right)
                        _field[x, y] = false;
                }
            }
        }

        private bool ValidateMousePosition(int x, int y)
        {
            return x >= 0 && y >= 0 && x < _cols && y < _rows;
        }
    }
}
