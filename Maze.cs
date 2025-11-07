using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_maze
{
    public class Maze
    {

        public int Height, Width;

        public Cell[,] Cells { get; private set; }

        Random rnd = new Random();
        public Pen wallPen = new Pen(Color.Black, 2);
        public Maze(int Height, int Width)
        {
            this.Height = Height;
            this.Width = Width;
            Cells = new Cell[Width, Height];

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Cells[x, y] = new Cell(x, y);
                }
            }
        }

        public void Kill(int startX, int startY)
        {

            Cell cell = Cells[startX, startY];
            cell.isVisit = true;
            List<Cell> listUnvisitNeight = GetUnVisitNeighbors(cell);
            if (listUnvisitNeight.Count > 0)
            {
                int rand = rnd.Next(0, listUnvisitNeight.Count);
                Cell randFromListCell = listUnvisitNeight[rand];
                RemoveWall(cell, randFromListCell);
                Kill(randFromListCell.X, randFromListCell.Y);
            }
            else
            {
                Hunt();
            }

        }
        public void Hunt()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Cell cell = Cells[x, y];
                    List<Cell> list = GetVisitNeightborn(cell);
                    if (cell.isVisit == false && list.Count > 0)
                    {
                        int rand = rnd.Next(0, list.Count);
                        Cell randFromListCell = list[rand];
                        RemoveWall(cell, randFromListCell);
                        Kill(cell.X, cell.Y);
                        return;
                    }
                }
            }
        }

        List<Cell> GetVisitNeightborn(Cell cell)
        {
            List<Cell> neighbors = new List<Cell>();

            if (cell.Y > 0)
            {
                Cell topN = Cells[cell.X, cell.Y - 1];
                if (topN.isVisit == true) neighbors.Add(topN);

            }
            if (cell.Y < Height - 1)
            {
                Cell down = Cells[cell.X, cell.Y + 1];
                if (down.isVisit == true) neighbors.Add(down);
            }
            if (cell.X > 0)
            {
                Cell left = Cells[cell.X - 1, cell.Y];
                if (left.isVisit == true) neighbors.Add(left);
            }
            if (cell.X < Width - 1)
            {
                Cell right = Cells[cell.X + 1, cell.Y];
                if (right.isVisit == true) neighbors.Add(right);
            }

            return neighbors;
        }


        void RemoveWall(Cell current, Cell Check)
        {
            int divX = current.X - Check.X;
            int divY = current.Y - Check.Y;
            if (divX == 1)
            {
                current.leftWall = false;
                Check.rightWall = false;
            }
            else if (divX == -1)
            {
                current.rightWall = false;
                Check.leftWall = false;
            }
            else if (divY == 1)
            {
                current.topWall = false;
                Check.downWall = false;
            }
            else if (divY == -1)
            {
                current.downWall = false;
                Check.topWall = false;
            }

        }

        private List<Cell> GetUnVisitNeighbors(Cell cell)
        {
            List<Cell> neighbors = new List<Cell>();

            if (cell.Y > 0)
            {
                Cell topN = Cells[cell.X, cell.Y - 1];
                if (topN.isVisit == false) neighbors.Add(topN);

            }
            if (cell.Y < Height - 1)
            {
                Cell down = Cells[cell.X, cell.Y + 1];
                if (down.isVisit == false) neighbors.Add(down);
            }
            if (cell.X > 0)
            {
                Cell left = Cells[cell.X - 1, cell.Y];
                if (left.isVisit == false) neighbors.Add(left);
            }
            if (cell.X < Width - 1)
            {
                Cell right = Cells[cell.X + 1, cell.Y];
                if (right.isVisit == false) neighbors.Add(right);
            }

            return neighbors;
        }

        public void Draw(Graphics graphics, int cellSize, Point offset)
        {
            graphics.Clear(Color.White);

            using (Pen pen = new Pen(Color.Black, 2))
            {
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        Cell cell = Cells[x, y];
                        DrawCell(graphics, pen, cell, cellSize, offset);
                    }
                }
            }
        }

        void DrawCell(Graphics graphics, Pen pen, Cell cell, int cellSize, Point offset)
        {
            int x = cell.X * cellSize + offset.X;
            int y = cell.Y * cellSize + offset.Y;

            if (cell.topWall)
                graphics.DrawLine(pen, x, y, x + cellSize, y);

            if (cell.downWall)
                graphics.DrawLine(pen, x, y + cellSize, x + cellSize, y + cellSize);

            if (cell.leftWall)
                graphics.DrawLine(pen, x, y, x, y + cellSize);

            if (cell.rightWall)
                graphics.DrawLine(pen, x + cellSize, y, x + cellSize, y + cellSize);
        }

    }

}
