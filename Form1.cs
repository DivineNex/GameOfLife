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
        private GameEngine gameEngine;
        private Graphics _graphics;
        private int _resolution;
        private SolidBrush _brush;
        private bool _isGrid;
        private bool _isHeatmapEnabled;

        public Form1()
        {
            InitializeComponent();
        }

        private void StartGame()
        {
            if (timerGame.Enabled)
                return;

            timerGame.Interval = (int)nudInterval.Value;
            timerFPS.Start();

            nudResolution.Enabled = false;
            nudDensity.Enabled = false;
            _resolution = (int)nudResolution.Value;

            gameEngine = new GameEngine
            (
                rows: pictureBox1.Height / _resolution,
                cols: pictureBox1.Width / _resolution,
                density: (int)nudDensity.Minimum + (int)nudDensity.Maximum - (int)nudDensity.Value
            );

            Text = $"Current generation: {gameEngine.CurrentGeneration}";

            _brush = new SolidBrush(Color.FromArgb(100, 87, 166));

            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            _graphics = Graphics.FromImage(pictureBox1.Image);
            timerGame.Start();
        }

        private void StopGame()
        {
            if (!timerGame.Enabled)
                return;

            timerFPS.Stop();
            timerGame.Stop();
            nudResolution.Enabled = true;
            nudDensity.Enabled = true;
        }

        private void DrawGeneration()
        {
            _graphics.Clear(Color.FromArgb(157, 172, 255));

            var field = gameEngine.GetCurrentGeneration();

            for (int x = 0; x < field.GetLength(0); x++)
            {
                for (int y = 0; y < field.GetLength(1); y++)
                {
                    if (!_isHeatmapEnabled)
                    {
                        _brush.Color = Color.FromArgb(100, 87, 166);
                        if (field[x, y].isAlive)
                        {
                            if (_isGrid)
                                _graphics.FillRectangle(_brush, x * _resolution, y * _resolution, _resolution - 1, _resolution - 1);
                            else
                                _graphics.FillRectangle(_brush, x * _resolution, y * _resolution, _resolution, _resolution);
                        }
                    }
                    else
                    {
                        switch (field[x,y].neighborCount)
                        {
                            case 0:
                                _brush.Color = Color.FromArgb(49, 105, 209);
                                break;
                            case 1:
                                _brush.Color = Color.FromArgb(76, 151, 177);
                                break;
                            case 2:
                                _brush.Color = Color.FromArgb(130, 155, 64);
                                break;
                            case 3:
                                _brush.Color = Color.FromArgb(208, 200, 55);
                                break;
                            case 4:
                                _brush.Color = Color.FromArgb(242, 212, 53);
                                break;
                            case 5:
                                _brush.Color = Color.FromArgb(196, 137, 34);
                                break;
                            case 6:
                                _brush.Color = Color.FromArgb(193, 111, 28);
                                break;
                            case 7:
                                _brush.Color = Color.FromArgb(190, 61, 15);
                                break;
                            case 8:
                                _brush.Color = Color.FromArgb(187, 25, 7);
                                break;
                        }

                        if (_isGrid)
                            _graphics.FillRectangle(_brush, x * _resolution, y * _resolution, _resolution - 1, _resolution - 1);
                        else
                            _graphics.FillRectangle(_brush, x * _resolution, y * _resolution, _resolution, _resolution);
                    }

                }
            }

            pictureBox1.Refresh();
            Text = $"Current generation: {gameEngine.CurrentGeneration}";
            gameEngine.NextGeneration();
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            DrawGeneration();
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
            if (!timerGame.Enabled)
                return;

            if (e.Button != MouseButtons.None)
            {
                var x = e.Location.X / _resolution;
                var y = e.Location.Y / _resolution;

                if (e.Button == MouseButtons.Left)
                    gameEngine.AddCell(x, y);
                else if (e.Button == MouseButtons.Right)
                    gameEngine.RemoveCell(x, y);
            }
        }

        private void cbIsGridDrawing_CheckedChanged(object sender, EventArgs e)
        {
            if (timerGame.Enabled)
                _isGrid = cbIsGridDrawing.Checked;
        }

        private void timerFPS_Tick(object sender, EventArgs e)
        {
            labelFPS.Text = $"FPS (GPS): {gameEngine.GetFps()}";
            labelFPS.Update();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            timerGame.Interval = (int)nudInterval.Value;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (timerGame.Enabled)
                _isHeatmapEnabled = cbHeatmapMode.Checked;
        }
    }
}
