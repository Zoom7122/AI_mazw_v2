using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace AI_maze
{
    public class Bot
    {
        public PictureBox PictureBox { get; private set; }
        public int Speed { get; set; } = 1;
        public bool IsMoving { get; private set; } = false;

        private Maze maze;
        private Timer movementTimer;
        private List<Point> path;
        private int currentPathIndex = 0;
        private int cellSize;
        private Point mazeOffset;

        private Color botColor = Color.Red;

        public Bot(Maze maze, int cellSize, Point mazeOffset)
        {
            this.maze = maze ?? throw new ArgumentNullException(nameof(maze));
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
            movementTimer.Interval = 100;
            movementTimer.Tick += (s, e) => MoveAlongPath();
        }

        public void StartMoving()
        {
            if (path != null && path.Count > 1 && !IsMoving)
            {
                IsMoving = true;
                movementTimer.Start();
            }
        }

        public void StopMoving()
        {
            IsMoving = false;
            movementTimer?.Stop();
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
                StopMoving();
                OnBotFinished?.Invoke(this, EventArgs.Empty);
            }
        }

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

        // Метод для отрисовки пути (опционально)
        public void DrawPath(Graphics graphics, Point offset, int cellSize)
        {
            if (path == null || path.Count < 2) return;

            using (Pen pathPen = new Pen(Color.Yellow, 2))
            {
                for (int i = 0; i < path.Count - 1; i++)
                {
                    Point current = path[i];
                    Point next = path[i + 1];

                    int currentX = offset.X + current.X * cellSize + cellSize / 2;
                    int currentY = offset.Y + current.Y * cellSize + cellSize / 2;
                    int nextX = offset.X + next.X * cellSize + cellSize / 2;
                    int nextY = offset.Y + next.Y * cellSize + cellSize / 2;

                    graphics.DrawLine(pathPen, currentX, currentY, nextX, nextY);
                }
            }
        }
    }
}