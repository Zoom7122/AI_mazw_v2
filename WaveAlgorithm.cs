using System;
using System.Collections.Generic;
using System.Drawing;

namespace AI_maze
{
    public static class WaveAlgorithm
    {
        public static List<Point> FindPath(Maze maze, Point start, Point finish)
        {
            int[,] distance = new int[maze.Width, maze.Height];
            Point[,] previous = new Point[maze.Width, maze.Height];

            // Инициализация массивов
            for (int x = 0; x < maze.Width; x++)
            {
                for (int y = 0; y < maze.Height; y++)
                {
                    distance[x, y] = -1; // -1 означает непосещенную клетку
                    previous[x, y] = new Point(-1, -1);
                }
            }

            // Очередь для BFS
            Queue<Point> queue = new Queue<Point>();
            distance[start.X, start.Y] = 0;
            queue.Enqueue(start);

            // Направления движения: вверх, вправо, вниз, влево
            Point[] directions = new Point[]
            {
                new Point(0, -1),  // вверх
                new Point(1, 0),   // вправо
                new Point(0, 1),   // вниз
                new Point(-1, 0)   // влево
            };

            // BFS
            while (queue.Count > 0)
            {
                Point current = queue.Dequeue();

                // Если дошли до финиша, выходим
                if (current == finish)
                    break;

                // Проверяем всех соседей
                foreach (Point dir in directions)
                {
                    Point neighbor = new Point(current.X + dir.X, current.Y + dir.Y);

                    // Проверяем, что сосед в пределах лабиринта
                    if (neighbor.X >= 0 && neighbor.X < maze.Width &&
                        neighbor.Y >= 0 && neighbor.Y < maze.Height)
                    {
                        // Проверяем, можно ли пройти в этом направлении
                        if (CanMove(maze, current, neighbor) &&
                            distance[neighbor.X, neighbor.Y] == -1)
                        {
                            distance[neighbor.X, neighbor.Y] = distance[current.X, current.Y] + 1;
                            previous[neighbor.X, neighbor.Y] = current;
                            queue.Enqueue(neighbor);
                        }
                    }
                }
            }

            // Восстанавливаем путь от финиша к старту
            return ReconstructPath(previous, finish);
        }

        private static bool CanMove(Maze maze, Point from, Point to)
        {
            // Проверяем, что точки в пределах лабиринта
            if (from.X < 0 || from.X >= maze.Width || from.Y < 0 || from.Y >= maze.Height ||
                to.X < 0 || to.X >= maze.Width || to.Y < 0 || to.Y >= maze.Height)
                return false;

            Cell fromCell = maze.Cells[from.X, from.Y];
            Cell toCell = maze.Cells[to.X, to.Y];

            int dx = to.X - from.X;
            int dy = to.Y - from.Y;

            // Проверяем стены в зависимости от направления
            if (dx == 1) // Движение вправо
                return !fromCell.rightWall && !toCell.leftWall;
            else if (dx == -1) // Движение влево
                return !fromCell.leftWall && !toCell.rightWall;
            else if (dy == 1) // Движение вниз
                return !fromCell.downWall && !toCell.topWall;
            else if (dy == -1) // Движение вверх
                return !fromCell.topWall && !toCell.downWall;

            return false;
        }

        private static List<Point> ReconstructPath(Point[,] previous, Point finish)
        {
            List<Point> path = new List<Point>();
            Point current = finish;

            // Восстанавливаем путь от финиша к старту
            while (current.X != -1 && current.Y != -1)
            {
                path.Add(current);
                current = previous[current.X, current.Y];
            }

            // Разворачиваем путь, чтобы он был от старта к финишу
            path.Reverse();

            return path.Count > 1 ? path : null;
        }
    }
}