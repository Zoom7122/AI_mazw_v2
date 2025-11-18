using AI_maze;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormsApp1.Class;

namespace WinFormsApp1
{
    public partial class GameForm : Form
    {
        private Player player;
        private Maze maze;
        private Maze mazeForBot;
        private int cellSize = 40;
        private Point finishPoint;
        private Point finishPointForBot;
        private Bot bot;
        private int ComplexityMaze { get; set; }

        // Панели для лабиринтов
        private DoubleBufferedPanel playerMazePanel;
        private DoubleBufferedPanel botMazePanel;

        public GameForm(int ComplexityMaze)
        {
            this.ComplexityMaze = ComplexityMaze;

            InitializeComponent();

            InitializePanels(this.ComplexityMaze = ComplexityMaze);


            InitializeMaze();
            InitializePlayer();
            SpawnButton();
            InitializeBot();

            // Настройка формы
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.KeyPreview = true;
            this.BackColor = Color.Black;
            this.DoubleBuffered = true;

            // Подписываемся на события
            this.KeyDown += GameForm_KeyDown;
        }

        private void InitializeBot()
        {
            if (mazeForBot == null) return;

            // Создаем бота с лабиринтом для бота
            Point botMazeOffset = new Point(10, 10);
            bot = new Bot(mazeForBot, cellSize, botMazeOffset);

            // Добавляем бота на панель бота
            botMazePanel.Controls.Add(bot.PictureBox);
            bot.PictureBox.BringToFront();

            bot.OnBotFinished += (s, e) =>
            {
                MessageBox.Show("Бот прошел лабиринт!", "Результат");
            };

            bot.StartMoving();
        }

        private void BotMazePanel_Paint(object sender, PaintEventArgs e)
        {
            // Очищаем панель
            e.Graphics.Clear(Color.White);

            // Отрисовываем лабиринт с небольшим отступом от краев панели
            Point panelOffset = new Point(10, 10);
            mazeForBot.Draw(e.Graphics, cellSize, panelOffset);

            // Отрисовываем финиш
            DrawFinish(e.Graphics, panelOffset, finishPointForBot, Color.Blue);

            // Подпись для лабиринта бота
            DrawLabel(e.Graphics, "Бот", panelOffset);

            // Если хочешь отрисовать путь бота, добавь:
            // bot?.DrawPath(e.Graphics, panelOffset, cellSize);
        }

        // При закрытии формы останавливаем бота
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            bot?.StopMoving();
            base.OnFormClosed(e);
        }

        private void InitializePanels(int complixity)
        {

            int shiftX, shiftY = 200;
            if (complixity == 15)
                shiftX = 500;
            else shiftX = 250;
            // Рассчитываем размер панели на основе размера лабиринта
            int panelSize = ComplexityMaze * cellSize + 50; // +50 для отступов

            // Панель для игрока (левая половина экрана)
            playerMazePanel = new DoubleBufferedPanel();
            playerMazePanel.Size = new Size(panelSize, panelSize);
            playerMazePanel.Location = new Point(
                (this.ClientSize.Width / 2 - panelSize) / 2 + 150,
                (this.ClientSize.Height - panelSize) / 2 + shiftY
            );
            playerMazePanel.BackColor = Color.White;
            playerMazePanel.BorderStyle = BorderStyle.FixedSingle;
            playerMazePanel.Paint += PlayerMazePanel_Paint;
            this.Controls.Add(playerMazePanel);

            // Панель для бота (правая половина экрана)
            botMazePanel = new DoubleBufferedPanel();
            botMazePanel.Size = new Size(panelSize, panelSize);
            botMazePanel.Location = new Point(
                this.ClientSize.Width / 2 + (this.ClientSize.Width / 2 - panelSize) / 2 + shiftX, 
                (this.ClientSize.Height - panelSize) / 2 + shiftY
            );
            botMazePanel.BackColor = Color.White;
            botMazePanel.BorderStyle = BorderStyle.FixedSingle;
            botMazePanel.Paint += BotMazePanel_Paint;
            this.Controls.Add(botMazePanel);
        }

        private void InitializeMaze()
        {
            maze = new Maze(ComplexityMaze, ComplexityMaze);
            maze.Kill(0, 0);

            mazeForBot = new Maze(ComplexityMaze, ComplexityMaze);
            mazeForBot.Kill(0, 0);

            // Устанавливаем финиш в правый нижний угол
            finishPoint = new Point(maze.Width - 1, maze.Height - 1);
            finishPointForBot = new Point(mazeForBot.Width - 1, mazeForBot.Height - 1);
        }

        private void InitializePlayer()
        {
            player = new Player();

            // Размер игрока пропорционально клетке лабиринта
            player.PictureBox.Size = new Size(cellSize / 2, cellSize / 2);

            // Позиционируем игрока в начале лабиринта на панели игрока
            player.PictureBox.Location = new Point(
                playerMazePanel.Location.X + cellSize / 4 + 10,
                playerMazePanel.Location.Y + cellSize / 4 + 10
            );

            this.Controls.Add(player.PictureBox);
            player.PictureBox.BringToFront();
        }

        private void SpawnButton()
        {
            Button exitButton = new Button();
            exitButton.Text = "Выход";
            exitButton.Size = new Size(100, 40);
            exitButton.Location = new Point(
                10,10
            );
            exitButton.BackColor = Color.Red;
            exitButton.ForeColor = Color.White;
            exitButton.Font = new Font("Arial", 10, FontStyle.Bold);
            exitButton.FlatStyle = FlatStyle.Flat;
            exitButton.FlatAppearance.BorderSize = 2;
            exitButton.FlatAppearance.BorderColor = Color.White;
            exitButton.Click += Exit_clic;
            this.Controls.Add(exitButton);
            exitButton.BringToFront();
        }

        // Отрисовка лабиринта игрока на его панели
        private void PlayerMazePanel_Paint(object sender, PaintEventArgs e)
        {
            // Очищаем панель
            e.Graphics.Clear(Color.White);

            // Отрисовываем лабиринт с небольшим отступом от краев панели
            Point panelOffset = new Point(10, 10);
            maze.Draw(e.Graphics, cellSize, panelOffset);

            // Отрисовываем финиш
            DrawFinish(e.Graphics, panelOffset, finishPoint, Color.Green);

            // Подпись для лабиринта игрока
            //DrawLabel(e.Graphics, "Игрок", panelOffset);
        }

        //private void BotMazePanel_Paint(object sender, PaintEventArgs e)
        //{
        //    // Очищаем панель
        //    e.Graphics.Clear(Color.White);

        //    // Отрисовываем лабиринт с небольшим отступом от краев панели
        //    Point panelOffset = new Point(10, 10);
        //    mazeForBot.Draw(e.Graphics, cellSize, panelOffset);

        //    // Отрисовываем финиш
        //    DrawFinish(e.Graphics, panelOffset, finishPointForBot, Color.Blue);

        //    // Подпись для лабиринта бота
        //    //DrawLabel(e.Graphics, "Бот", panelOffset);
        //}

        private void DrawFinish(Graphics graphics, Point offset, Point finish, Color color)
        {
            int x = finish.X * cellSize + offset.X + 5;
            int y = finish.Y * cellSize + offset.Y + 5;
            int size = cellSize - 10;

            using (Brush brush = new SolidBrush(color))
            {
                graphics.FillRectangle(brush, x, y, size, size);
            }
        }

        private void DrawLabel(Graphics graphics, string text, Point offset)
        {
            Font labelFont = new Font("Arial", 12, FontStyle.Bold);
            SizeF textSize = graphics.MeasureString(text, labelFont);

            PointF labelPos = new PointF(
                250, 500
            );

            graphics.DrawString(text, labelFont, Brushes.Black, labelPos);
        }

        private void Exit_clic(object sender, EventArgs e)
        {
            this.Close();
        }

        private void GameForm_KeyDown(object sender, KeyEventArgs e)
        {
            int deltaX = 0, deltaY = 0;

            switch (e.KeyCode)
            {
                case Keys.W: deltaY = -player.Speed; break;
                case Keys.S: deltaY = player.Speed; break;
                case Keys.A: deltaX = -player.Speed; break;
                case Keys.D: deltaX = player.Speed; break;
                case Keys.Escape: this.Close(); break;
            }

            if (!WillCollide(deltaX, deltaY))
            {
                player.Move(deltaX, deltaY);
                CheckWinCondition();
            }

            // Обновляем отрисовку панели игрока
            playerMazePanel.Invalidate();
        }

        private bool WillCollide(int deltaX, int deltaY)
        {
            Rectangle futureBounds = new Rectangle(
                player.PictureBox.Left + deltaX,
                player.PictureBox.Top + deltaY,
                player.PictureBox.Width,
                player.PictureBox.Height
            );

            // Проверяем границы панели игрока
            if (!playerMazePanel.Bounds.Contains(futureBounds))
                return true;

            // Проверяем столкновение с каждой клеткой лабиринта
            for (int x = 0; x < maze.Width; x++)
            {
                for (int y = 0; y < maze.Height; y++)
                {
                    Cell cell = maze.Cells[x, y];
                    if (CheckCellCollision(cell, futureBounds, playerMazePanel.Location))
                        return true;
                }
            }

            return false;
        }

        private bool CheckCellCollision(Cell cell, Rectangle playerBounds, Point panelLocation)
        {
            // Смещение внутри панели
            Point panelOffset = new Point(10, 10);
            int cellX = cell.X * cellSize + panelLocation.X + panelOffset.X;
            int cellY = cell.Y * cellSize + panelLocation.Y + panelOffset.Y;

            // Создаем прямоугольники для каждой стены
            if (cell.topWall)
            {
                Rectangle topWall = new Rectangle(cellX, cellY, cellSize, 2);
                if (playerBounds.IntersectsWith(topWall))
                    return true;
            }

            if (cell.downWall)
            {
                Rectangle downWall = new Rectangle(cellX, cellY + cellSize - 2, cellSize, 2);
                if (playerBounds.IntersectsWith(downWall))
                    return true;
            }

            if (cell.leftWall)
            {
                Rectangle leftWall = new Rectangle(cellX, cellY, 2, cellSize);
                if (playerBounds.IntersectsWith(leftWall))
                    return true;
            }

            if (cell.rightWall)
            {
                Rectangle rightWall = new Rectangle(cellX + cellSize - 2, cellY, 2, cellSize);
                if (playerBounds.IntersectsWith(rightWall))
                    return true;
            }

            return false;
        }

        private void CheckWinCondition()
        {
            // Смещение внутри панели
            Point panelOffset = new Point(10, 10);

            // Определяем клетку, в которой находится игрок
            int playerCellX = (player.PictureBox.Left - playerMazePanel.Location.X - panelOffset.X + cellSize / 2) / cellSize;
            int playerCellY = (player.PictureBox.Top - playerMazePanel.Location.Y - panelOffset.Y + cellSize / 2) / cellSize;

            // Проверяем, дошел ли игрок до финиша
            if (playerCellX == finishPoint.X && playerCellY == finishPoint.Y)
            {
                MessageBox.Show("Поздравляю! Вы прошли лабиринт!", "Победа!");
                this.Close();
            }
        }

        private void GameForm_Load(object sender, EventArgs e)
        {
            // Дополнительная инициализация при загрузке формы
        }

        private void GameForm_Resize(object sender, EventArgs e)
        {
            // При изменении размера формы пересчитываем позиции панелей
            if (playerMazePanel != null && botMazePanel != null)
            {
                int panelSize = ComplexityMaze * cellSize + 20;

                playerMazePanel.Location = new Point(
                    (this.ClientSize.Width / 2 - panelSize) / 2,
                    (this.ClientSize.Height - panelSize) / 2
                );

                botMazePanel.Location = new Point(
                    this.ClientSize.Width / 2 + (this.ClientSize.Width / 2 - panelSize) / 2,
                    (this.ClientSize.Height - panelSize) / 2
                );

                Control exitButton = this.Controls.OfType<Button>().FirstOrDefault();
                if (exitButton != null)
                {
                    exitButton.Location = new Point(this.ClientSize.Width - 120, 20);
                }
            }
        }
        public class DoubleBufferedPanel : Panel
        {
            public DoubleBufferedPanel()
            {
                this.DoubleBuffered = true;
                this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                             ControlStyles.UserPaint |
                             ControlStyles.OptimizedDoubleBuffer, true);
            }
        }
    }
}