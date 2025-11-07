using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace AI_maze
{
    public class Bot
    {
        public PictureBox PictureBox { get; private set; }
        public int Speed { get; set; } = 5;
        public bool IsMoving { get; private set; } = false;

        private Maze maze;
        private Timer movementTimer;
        private List<Point> path;
        private int currentPathIndex = 0;
        private int cellSize;
        private Point mazeOffset;

        // Цвет бота (отличный от игрока)
        private Color botColor = Color.Red;

        public Bot(Maze maze, int cellSize, Point mazeOffset)
        {
            this.maze = maze;
            this.cellSize = cellSize;
            this.mazeOffset = mazeOffset;

            InitializeBot();
            FindPath();
            SetupMovementTimer();
        }

        private void InitializeBot()
        {
            PictureBox = new PictureBox();
            PictureBox.Size = new Size(cellSize / 2, cellSize / 2);
            PictureBox.BackColor = botColor;

            // Устанавливаем бота в начало лабиринта
            SetPosition(0, 0);
        }

        private void SetPosition(int cellX, int cellY)
        {
            PictureBox.Location = new Point(
                mazeOffset.X + cellX * cellSize + cellSize / 4,
                mazeOffset.Y + cellY * cellSize + cellSize / 4
            );
        }

        private void FindPath()
        {
            // Используем волновой алгоритм для поиска пути от (0,0) до финиша
            path = WaveAlgorithm.FindPath(maze, new Point(0, 0),
                new Point(maze.Width - 1, maze.Height - 1));

            if (path != null && path.Count > 0)
            {
                currentPathIndex = 0;
                SetPosition(path[0].X, path[0].Y);
            }
        }

        private void SetupMovementTimer()
        {
            movementTimer = new Timer();
            movementTimer.Interval = 100; // Интервал движения (можно регулировать скорость)
            movementTimer.Tick += (s, e) => MoveAlongPath();
        }

        public void StartMoving()
        {
            if (path != null && path.Count > 1)
            {
                IsMoving = true;
                movementTimer.Start();
            }
        }

        public void StopMoving()
        {
            IsMoving = false;
            movementTimer.Stop();
        }

        private void MoveAlongPath()
        {
            if (currentPathIndex < path.Count - 1)
            {
                currentPathIndex++;
                Point nextCell = path[currentPathIndex];
                SetPosition(nextCell.X, nextCell.Y);
            }
            else
            {
                // Бот дошел до финиша
                StopMoving();
                OnBotFinished?.Invoke(this, EventArgs.Empty);
            }
        }

        // Событие, когда бот дошел до финиша
        public event EventHandler OnBotFinished;

        public void Reset()
        {
            StopMoving();
            currentPathIndex = 0;
            if (path != null && path.Count > 0)
            {
                SetPosition(path[0].X, path[0].Y);
            }
        }
    }
}