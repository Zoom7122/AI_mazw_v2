using AI_maze;
using System;
using System.Collections.Generic;
using System.Drawing;
using WinFormsApp1.Class;

namespace WinFormsApp1.Algoritms
{
    public static class WaveAlgorithm
    {
        
        public static List<Point> FindPath(Maze maze, Point start, Point finish)
        {
            // Проверяем валидность входных данных
            if (maze == null) return null;
            if (start.X < 0 || start.X >= maze.Width || start.Y < 0 || start.Y >= maze.Height ||
                finish.X < 0 || finish.X >= maze.Width || finish.Y < 0 || finish.Y >= maze.Height)
                return null;

            int[,] distance = new int[maze.Width, maze.Height];
            Point[,] previous = new Point[maze.Width, maze.Height];

            // Инициализация массивов
            for (int x = 0; x < maze.Width; x++)
            {
                for (int y = 0; y < maze.Height; y++)
                {
                    distance[x, y] = -1;
                    previous[x, y] = new Point(-1, -1);
                }
            }

            Queue<Point> queue = new Queue<Point>();
            distance[start.X, start.Y] = 0;
            queue.Enqueue(start);

            Point[] directions = new Point[]
            {
                new Point(0, -1), new Point(1, 0),
                new Point(0, 1), new Point(-1, 0)
            };

            while (queue.Count > 0)
            {
                Point current = queue.Dequeue();

                if (current.Equals(finish))
                    break;

                foreach (Point dir in directions)
                {
                    Point neighbor = new Point(current.X + dir.X, current.Y + dir.Y);

                    if (neighbor.X >= 0 && neighbor.X < maze.Width &&
                        neighbor.Y >= 0 && neighbor.Y < maze.Height &&
                        CanMove(maze, current, neighbor) &&
                        distance[neighbor.X, neighbor.Y] == -1)
                    {
                        distance[neighbor.X, neighbor.Y] = distance[current.X, current.Y] + 1;
                        previous[neighbor.X, neighbor.Y] = current;
                        queue.Enqueue(neighbor);
                    }
                }
            }

            return ReconstructPath(previous, finish);
        }

        private static bool CanMove(Maze maze, Point from, Point to)
        {
            if (from.X < 0 || from.X >= maze.Width || from.Y < 0 || from.Y >= maze.Height ||
                to.X < 0 || to.X >= maze.Width || to.Y < 0 || to.Y >= maze.Height)
                return false;

            Cell fromCell = maze.Cells[from.X, from.Y];
            Cell toCell = maze.Cells[to.X, to.Y];

            int dx = to.X - from.X;
            int dy = to.Y - from.Y;

            if (dx == 1) return !fromCell.rightWall && !toCell.leftWall;
            else if (dx == -1) return !fromCell.leftWall && !toCell.rightWall;
            else if (dy == 1) return !fromCell.downWall && !toCell.topWall;
            else if (dy == -1) return !fromCell.topWall && !toCell.downWall;

            return false;
        }

        private static List<Point> ReconstructPath(Point[,] previous, Point finish)
        {
            if (previous[finish.X, finish.Y].X == -1) return null;

            List<Point> path = new List<Point>();
            Point current = finish;

            while (current.X != -1 && current.Y != -1)
            {
                path.Add(current);
                current = previous[current.X, current.Y];
            }

            path.Reverse();
            return path.Count > 0 ? path : null;
        }
    }
}