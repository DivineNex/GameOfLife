using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Forms;
using System.Threading;

namespace GameOfLife
{
    public class GameEngine
    {
        public uint CurrentGeneration { get; private set; }
        private bool[,] _field;
        private readonly int _rows;
        private readonly int _cols;
        private DateTime _lastCheckTime = DateTime.Now;
        private long _frameCount = 0;

        public GameEngine(int rows, int cols, int density)
        {
            _rows = rows;
            _cols = cols;
            _field = new bool[cols, rows];
            Random _random = new Random();

            for (int x = 0; x < _cols; x++)
            {
                for (int y = 0; y < _rows; y++)
                {
                    _field[x, y] = _random.Next(density) == 0;
                }
            }
        }

        public void NextGeneration()
        {
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
                }
            }
            _field = newField;
            CurrentGeneration++;
            OnMapUpdated();
        }

        public bool[,] GetCurrentGeneration()
        {
            var result = new bool[_cols, _rows];
            for (int x = 0; x < _cols; x++)
            {
                for (int y = 0; y < _rows; y++)
                {
                    result[x, y] = _field[x, y];
                }
            }
            return result;
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

        private bool ValidateCellPosition(int x, int y)
        {
            return x >= 0 && y >= 0 && x < _cols && y < _rows;
        }

        private void UpdateCell(int x, int y, bool state)
        {
            if (ValidateCellPosition(x, y))
            {
                _field[x, y] = state;
            }
        }

        public void AddCell(int x, int y) => UpdateCell(x, y, state: true);

        public void RemoveCell(int x, int y) => UpdateCell(x, y, state: false);

        private void OnMapUpdated()
        {
            Interlocked.Increment(ref _frameCount);
        }

        public int GetFps()
        {
            double secondsElapsed = (DateTime.Now - _lastCheckTime).TotalSeconds;
            long count = Interlocked.Exchange(ref _frameCount, 0);
            double fps = count / secondsElapsed;
            _lastCheckTime = DateTime.Now;
            return (int)fps;
        }
    }
}
