using AI_maze;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormsApp1.Algoritms;
using WinFormsApp1.Class;

namespace WinFormsApp1
{
    public partial class GameForm : Form
    {
        private bool resultsShown = false;
        private Player player;
        private Maze maze;
        private Maze mazeForBot;
        private int cellSize = 40;
        private Point finishPoint;
        private Point finishPointForBot;
        private Bot bot;
        private int ComplexityMaze { get; set; }
        private TimerForGame gameTimer;

        private bool playerFinished = false;
        private bool botFinished = false;
        private int playerTimeSeconds = 0;
        private long botTimeMs = 0;

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
            CreateTimerObj();
        }

        public void CreateTimerObj()
        {
            gameTimer = new TimerForGame(this.ClientSize, this);
            gameTimer.CreateTimer();
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
                botFinished = true;
                botTimeMs = bot.GetElapsedTimeMs();

                // Отладочное сообщение
                Debug.WriteLine($"Бот дошел до финиша! Время: {botTimeMs} мс = {botTimeMs / 1000.0} сек");

                // Показываем сообщение, что бот дошел до финиша
                ShowTemporaryMessage($"Бот дошел до финиша!\nВремя: {TimeSpan.FromMilliseconds(botTimeMs):mm\\:ss\\.fff}");

                // Проверяем, дошел ли уже игрок
                CheckIfBothFinished();
            };
            bot.StartMoving();
        }

        private void CheckIfBothFinished()
        {
            // Проверяем, оба ли участника дошли до финиша
            if (playerFinished && botFinished && !resultsShown)
            {
                resultsShown = true;

                // Ждем немного, чтобы игрок увидел сообщение о финише бота
                Task.Delay(1000).ContinueWith(_ =>
                {
                    this.Invoke(new Action(ShowFinalResults));
                });
            }
        }
        private void ShowFinalResults()
        {
            // Создаем форму с результатами
            using (ResultForm resultForm = new ResultForm())
            {
                resultForm.PlayerTime = TimeSpan.FromSeconds(playerTimeSeconds);
                resultForm.BotTime = TimeSpan.FromMilliseconds(botTimeMs);

                // Определяем победителя
                if (playerTimeSeconds < (botTimeMs / 1000.0))
                {
                    resultForm.Winner = "Игрок";
                    resultForm.WinnerColor = Color.Blue;
                }
                else if (playerTimeSeconds > (botTimeMs / 1000.0))
                {
                    resultForm.Winner = "Бот";
                    resultForm.WinnerColor = Color.Red;
                }
                else
                {
                    resultForm.Winner = "Ничья";
                    resultForm.WinnerColor = Color.Gray;
                }

                var dialogResult = resultForm.ShowDialog();

                if (dialogResult == DialogResult.Retry)
                {
                    // Начать новую игру
                    this.DialogResult = DialogResult.Retry;
                    this.Close();
                }
                else
                {
                    this.Close();
                }
            }
        }

        private void ShowTemporaryMessage(string message)
        {
            // Создаем форму с сообщением
            Form messageForm = new Form();
            messageForm.Text = "Информация";
            messageForm.Size = new Size(300, 150);
            messageForm.StartPosition = FormStartPosition.CenterParent;
            messageForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            messageForm.MaximizeBox = false;
            messageForm.MinimizeBox = false;

            Label messageLabel = new Label();
            messageLabel.Text = message;
            messageLabel.Font = new Font("Arial", 10, FontStyle.Bold);
            messageLabel.ForeColor = Color.Black;
            messageLabel.Size = new Size(280, 80);
            messageLabel.Location = new Point(10, 20);
            messageLabel.TextAlign = ContentAlignment.MiddleCenter;
            messageForm.Controls.Add(messageLabel);

            Button okButton = new Button();
            okButton.Text = "OK";
            okButton.Size = new Size(80, 30);
            okButton.Location = new Point(110, 100);
            okButton.Click += (s, e) => messageForm.Close();
            messageForm.Controls.Add(okButton);

            messageForm.ShowDialog();
        }

        private void ShowResults()
        {
            // Получаем время бота в секундах
            double botTimeSeconds = botTimeMs / 1000.0;

            string winner = "";
            Color winnerColor = Color.Gray;

            // Определяем победителя
            if (playerTimeSeconds < botTimeSeconds)
            {
                winner = "Игрок";
                winnerColor = Color.Blue;
            }
            else if (playerTimeSeconds > botTimeSeconds)
            {
                winner = "Бот";
                winnerColor = Color.Red;
            }
            else
            {
                winner = "Ничья";
                winnerColor = Color.Gray;
            }

            // Создаем форму с результатами
            using (ResultForm resultForm = new ResultForm())
            {
                resultForm.PlayerTime = TimeSpan.FromSeconds(playerTimeSeconds);
                resultForm.BotTime = TimeSpan.FromMilliseconds(botTimeMs);
                resultForm.Winner = winner; // Устанавливаем победителя
                resultForm.WinnerColor = winnerColor; // Устанавливаем цвет победителя

                resultForm.ShowDialog();
            }

            // Закрываем игровую форму
            this.Close();
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
            DrawLabel(e.Graphics, "", panelOffset);
           
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

            Button saveButton = new Button();
            saveButton.Text = "Сохранить";
            saveButton.Size = new Size(100, 40);
            saveButton.Location = new Point(120, 10);
            saveButton.BackColor = Color.Green;
            saveButton.ForeColor = Color.White;
            saveButton.Font = new Font("Arial", 10, FontStyle.Bold);
            saveButton.FlatStyle = FlatStyle.Flat;
            saveButton.Click += (s, e) =>
            {
                using (SaveFileDialog saveDialog = new SaveFileDialog())
                {
                    saveDialog.Filter = "Файлы сохранения (*.save)|*.save|Все файлы (*.*)|*.*";
                    saveDialog.DefaultExt = "save";
                    saveDialog.Title = "Сохранить игру";

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        SaveGame(saveDialog.FileName);
                    }
                }
            };
            this.Controls.Add(saveButton);
            saveButton.BringToFront();

            Button loadButton = new Button();
            loadButton.Text = "Загрузить";
            loadButton.Size = new Size(100, 40);
            loadButton.Location = new Point(230, 10);
            loadButton.BackColor = Color.Blue;
            loadButton.ForeColor = Color.White;
            loadButton.Font = new Font("Arial", 10, FontStyle.Bold);
            loadButton.FlatStyle = FlatStyle.Flat;
            loadButton.Click += (s, e) =>
            {
                using (OpenFileDialog openDialog = new OpenFileDialog())
                {
                    openDialog.Filter = "Файлы сохранения (*.save)|*.save|Все файлы (*.*)|*.*";
                    openDialog.DefaultExt = "save";
                    openDialog.Title = "Загрузить игру";

                    if (openDialog.ShowDialog() == DialogResult.OK)
                    {
                        LoadGame(openDialog.FileName);
                    }
                }
            };
            this.Controls.Add(loadButton);
            loadButton.BringToFront();

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
            if (playerCellX == finishPoint.X && playerCellY == finishPoint.Y && !playerFinished)
            {
                playerFinished = true;
                playerTimeSeconds = gameTimer.GetElapsedSeconds();
                gameTimer.StopTimer();

                // Останавливаем движение игрока
                player.PictureBox.Enabled = false;

                // Отладочное сообщение
                Debug.WriteLine($"Игрок дошел до финиша! Время: {playerTimeSeconds} сек");

                // Показываем сообщение, что игрок дошел до финиша
                ShowTemporaryMessage($"Игрок дошел до финиша!\nВремя: {TimeSpan.FromSeconds(playerTimeSeconds):mm\\:ss\\.fff}");

                // Проверяем, дошел ли уже бот
                CheckIfBothFinished();
            }
        }

        private void GameForm_Load(object sender, EventArgs e)
        {
            
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
        private void SaveGame(string filePath)
        {
            try
            {
                var save = new GameSave
                {
                    Complexity = ComplexityMaze,
                    SaveDate = DateTime.Now,

                    // Сохраняем позицию игрока в клетках
                    PlayerPosition = new Point(
                        (player.PictureBox.Left - playerMazePanel.Location.X - 10) / cellSize,
                        (player.PictureBox.Top - playerMazePanel.Location.Y - 10) / cellSize
                    ),

                    // Сохраняем позицию бота в клетках
                    BotPosition = new Point(
                        (bot.PictureBox.Left - botMazePanel.Location.X - 10) / cellSize,
                        (bot.PictureBox.Top - botMazePanel.Location.Y - 10) / cellSize
                    ),

                    PlayerFinished = playerFinished,
                    BotFinished = botFinished,
                    PlayerTimeSeconds = playerTimeSeconds,
                    BotTimeMs = botTimeMs,
                    ElapsedSeconds = gameTimer?.GetElapsedSeconds() ?? 0,
                    IsBotMoving = bot?.IsMoving ?? false
                };

                // Сохраняем стены лабиринта как список
                save.MazeWalls = new List<CellWalls>();
                for (int x = 0; x < maze.Width; x++)
                {
                    for (int y = 0; y < maze.Height; y++)
                    {
                        var cell = maze.Cells[x, y];
                        save.MazeWalls.Add(new CellWalls
                        {
                            X = x,
                            Y = y,
                            TopWall = cell.topWall,
                            RightWall = cell.rightWall,
                            BottomWall = cell.downWall,
                            LeftWall = cell.leftWall
                        });
                    }
                }

                // Сериализуем в JSON
                var json = JsonSerializer.Serialize(save, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);

                MessageBox.Show("Игра сохранена!", "Сохранение", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadGame(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    MessageBox.Show("Файл сохранения не найден!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var json = File.ReadAllText(filePath);
                var save = JsonSerializer.Deserialize<GameSave>(json);

                if (save == null)
                {
                    MessageBox.Show("Неверный формат файла сохранения!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Останавливаем и удаляем старого бота полностью
                if (bot != null)
                {
                    bot.StopMoving();
                    bot.PictureBox.Dispose();
                    bot = null;
                }

                // Восстанавливаем сложность
                ComplexityMaze = save.Complexity;

                // Пересоздаем лабиринты
                InitializeMaze();

                // Восстанавливаем стены из списка
                foreach (var cellWalls in save.MazeWalls)
                {
                    if (cellWalls.X < maze.Width && cellWalls.Y < maze.Height)
                    {
                        var cell = maze.Cells[cellWalls.X, cellWalls.Y];
                        cell.topWall = cellWalls.TopWall;
                        cell.rightWall = cellWalls.RightWall;
                        cell.downWall = cellWalls.BottomWall;
                        cell.leftWall = cellWalls.LeftWall;

                        // Копируем в лабиринт бота
                        var botCell = mazeForBot.Cells[cellWalls.X, cellWalls.Y];
                        botCell.topWall = cellWalls.TopWall;
                        botCell.rightWall = cellWalls.RightWall;
                        botCell.downWall = cellWalls.BottomWall;
                        botCell.leftWall = cellWalls.LeftWall;
                    }
                }

                // Восстанавливаем позицию игрока
                player.PictureBox.Location = new Point(
                    playerMazePanel.Location.X + 10 + save.PlayerPosition.X * cellSize + cellSize / 4,
                    playerMazePanel.Location.Y + 10 + save.PlayerPosition.Y * cellSize + cellSize / 4
                );

                // Восстанавливаем состояние игры
                playerFinished = save.PlayerFinished;
                botFinished = save.BotFinished;
                playerTimeSeconds = save.PlayerTimeSeconds;
                botTimeMs = save.BotTimeMs;
                resultsShown = false;

                // Восстанавливаем таймер
                if (gameTimer != null)
                {
                    gameTimer.StopTimer();
                    // Здесь нужно установить время из сохранения
                    // Добавьте в TimerForGame метод SetTime:
                    // gameTimer.SetTime(save.ElapsedSeconds);
                }

                // Теперь создаем нового бота с правильной позицией
                // Нужно изменить InitializeBot, чтобы он принимал начальную позицию
                InitializeBotWithPosition(save.BotPosition.X, save.BotPosition.Y);

                if (save.IsBotMoving && !botFinished)
                {
                    bot.StartMoving();
                }

                // Обновляем отрисовку
                playerMazePanel.Invalidate();
                botMazePanel.Invalidate();

                MessageBox.Show($"Игра загружена!\nСохранено: {save.SaveDate}", "Загрузка",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void InitializeBotWithPosition(int startCellX, int startCellY)
        {
            if (mazeForBot == null) return;

            // Создаем бота с лабиринтом для бота
            Point botMazeOffset = new Point(10, 10);
            bot = new Bot(mazeForBot, cellSize, botMazeOffset);

            // Устанавливаем начальную позицию бота
            bot.PictureBox.Location = new Point(
                botMazePanel.Location.X + botMazeOffset.X + startCellX * cellSize + cellSize / 4,
                botMazePanel.Location.Y + botMazeOffset.Y + startCellY * cellSize + cellSize / 4
            );

            // Добавляем бота на панель бота
            botMazePanel.Controls.Add(bot.PictureBox);
            bot.PictureBox.BringToFront();

            // Пересчитываем путь от текущей позиции до финиша
            RecalculateBotPathFromPosition(startCellX, startCellY);

            bot.OnBotFinished += (s, e) =>
            {
                botFinished = true;
                botTimeMs = bot.GetElapsedTimeMs();
                Debug.WriteLine($"Бот дошел до финиша! Время: {botTimeMs} мс");
                ShowTemporaryMessage($"Бот дошел до финиша!\nВремя: {TimeSpan.FromMilliseconds(botTimeMs):mm\\:ss\\.fff}");
                CheckIfBothFinished();
            };
        }

        private void RecalculateBotPathFromPosition(int startCellX, int startCellY)
        {
            // Пересчитываем путь от сохраненной позиции до финиша
            var path = WaveAlgorithm.FindPath(mazeForBot,
                new Point(startCellX, startCellY),
                new Point(mazeForBot.Width - 1, mazeForBot.Height - 1));

            // Здесь нужно обновить путь бота
            // Вам нужно добавить метод в класс Bot для установки нового пути:
            // bot.SetPath(path);
        }
    }
}