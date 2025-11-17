using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_maze
{
    public class Cell
    {
        public int X { get; }
        public int Y { get; }
        public bool isVisit = false;

        public bool topWall = true;
        public bool downWall = true;
        public bool leftWall = true;
        public bool rightWall = true;

        public Cell(int x, int y)
        {
            X = x;
            Y = y;
        }

    }
}
