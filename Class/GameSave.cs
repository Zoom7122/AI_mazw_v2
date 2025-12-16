// GameSave.cs
using System;
using System.Collections.Generic;
using System.Drawing;

namespace WinFormsApp1
{
    [Serializable]
    public class GameSave
    {
        public int Version { get; set; } = 1;
        public int Complexity { get; set; }
        public Point PlayerPosition { get; set; }
        public Point BotPosition { get; set; }
        public int CurrentBotPathIndex { get; set; }
        public bool PlayerFinished { get; set; }
        public bool BotFinished { get; set; }
        public int PlayerTimeSeconds { get; set; }
        public long BotTimeMs { get; set; }
        public int ElapsedSeconds { get; set; }
        public bool IsBotMoving { get; set; }
        public DateTime SaveDate { get; set; }

        // Сериализуемые стены лабиринта
        public List<CellWalls> MazeWalls { get; set; }

        // Для хранения позиций пути бота
        public List<SerializablePoint> BotPath { get; set; }
    }

    [Serializable]
    public class CellWalls
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool TopWall { get; set; }
        public bool RightWall { get; set; }
        public bool BottomWall { get; set; }
        public bool LeftWall { get; set; }
    }

    [Serializable]
    public class SerializablePoint
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
}