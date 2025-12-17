using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class ResultForm : Form
    {
        public TimeSpan PlayerTime { get; set; }
        public TimeSpan BotTime { get; set; }
        public string Winner { get; set; } = "";
        public Color WinnerColor { get; set; } = Color.Gray;

        private Label playerTimeLabel;
        private Label botTimeLabel;
        private Label diffLabel;
        private Label winnerLabel;

        public ResultForm()
        {
            InitializeForm();
            SetupControls();
        }

        private void InitializeForm()
        {
            // Настройки формы
            this.Text = "Результаты игры";
            this.Size = new Size(600, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.AutoScaleMode = AutoScaleMode.Font;
        }

        private void SetupControls()
        {
            // Заголовок
            Label titleLabel = new Label();
            titleLabel.Text = "ИТОГИ ГОНКИ";
            titleLabel.Font = new Font("Arial", 24, FontStyle.Bold);
            titleLabel.ForeColor = Color.DarkBlue;
            titleLabel.Size = new Size(400, 50);
            titleLabel.Location = new Point(25, 20);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(titleLabel);

            // Панель для результатов игрока
            Panel playerPanel = new Panel();
            playerPanel.Size = new Size(400, 70);
            playerPanel.Location = new Point(25, 90);
            playerPanel.BackColor = Color.LightBlue;
            playerPanel.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(playerPanel);

            Label playerTitle = new Label();
            playerTitle.Text = "ИГРОК";
            playerTitle.Font = new Font("Arial", 14, FontStyle.Bold);
            playerTitle.ForeColor = Color.DarkBlue;
            playerTitle.Size = new Size(100, 30);
            playerTitle.Location = new Point(10, 20);
            playerTitle.TextAlign = ContentAlignment.MiddleLeft;
            playerPanel.Controls.Add(playerTitle);

            playerTimeLabel = new Label();
            playerTimeLabel.Font = new Font("Consolas", 18, FontStyle.Bold);
            playerTimeLabel.ForeColor = Color.Black;
            playerTimeLabel.Size = new Size(200, 30);
            playerTimeLabel.Location = new Point(180, 20);
            playerTimeLabel.TextAlign = ContentAlignment.MiddleRight;
            playerPanel.Controls.Add(playerTimeLabel);

            // Панель для результатов бота
            Panel botPanel = new Panel();
            botPanel.Size = new Size(400, 70);
            botPanel.Location = new Point(25, 170);
            botPanel.BackColor = Color.LightCoral;
            botPanel.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(botPanel);

            Label botTitle = new Label();
            botTitle.Text = "БОТ";
            botTitle.Font = new Font("Arial", 14, FontStyle.Bold);
            botTitle.ForeColor = Color.DarkRed;
            botTitle.Size = new Size(100, 30);
            botTitle.Location = new Point(10, 20);
            botTitle.TextAlign = ContentAlignment.MiddleLeft;
            botPanel.Controls.Add(botTitle);

            botTimeLabel = new Label();
            botTimeLabel.Font = new Font("Consolas", 18, FontStyle.Bold);
            botTimeLabel.ForeColor = Color.Black;
            botTimeLabel.Size = new Size(200, 30);
            botTimeLabel.Location = new Point(180, 20);
            botTimeLabel.TextAlign = ContentAlignment.MiddleRight;
            botPanel.Controls.Add(botTimeLabel);

            // Разница во времени
            diffLabel = new Label();
            diffLabel.Font = new Font("Arial", 12, FontStyle.Bold);
            diffLabel.Size = new Size(400, 30);
            diffLabel.Location = new Point(25, 250);
            diffLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(diffLabel);

            // Победитель
            winnerLabel = new Label();
            winnerLabel.Font = new Font("Arial", 16, FontStyle.Bold);
            winnerLabel.Size = new Size(400, 40);
            winnerLabel.Location = new Point(25, 290);
            winnerLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(winnerLabel);

            // Кнопка новой игры
            Button newGameButton = new Button();
            newGameButton.Text = "НОВАЯ ИГРА";
            newGameButton.Size = new Size(150, 40);
            newGameButton.Location = new Point(75, 340);
            newGameButton.BackColor = Color.Green;
            newGameButton.ForeColor = Color.White;
            newGameButton.Font = new Font("Arial", 12, FontStyle.Bold);
            newGameButton.FlatStyle = FlatStyle.Flat;
            newGameButton.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.Retry;
                this.Close();
            };
            this.Controls.Add(newGameButton);

            // Кнопка выхода в меню
            Button menuButton = new Button();
            menuButton.Text = "В МЕНЮ";
            menuButton.Size = new Size(150, 40);
            menuButton.Location = new Point(225, 340);
            menuButton.BackColor = Color.Blue;
            menuButton.ForeColor = Color.White;
            menuButton.Font = new Font("Arial", 12, FontStyle.Bold);
            menuButton.FlatStyle = FlatStyle.Flat;
            menuButton.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            };
            this.Controls.Add(menuButton);

            // Обновляем данные после создания элементов
            this.Load += (s, e) => UpdateResults();
        }

        private void UpdateResults()
        {
            // Обновляем время игрока
            playerTimeLabel.Text = PlayerTime.ToString(@"mm\:ss\.fff");

            // Обновляем время бота
            botTimeLabel.Text = BotTime.ToString(@"mm\:ss\.fff");

            // Разница во времени
            TimeSpan difference = PlayerTime - BotTime;
            string diffText = "";
            Color diffColor = Color.Black;

            if (difference.TotalMilliseconds > 0)
            {
                diffText = $"Бот быстрее на: {difference:mm\\:ss\\.fff}";
                diffColor = Color.DarkRed;
            }
            else if (difference.TotalMilliseconds < 0)
            {
                diffText = $"Игрок быстрее на: {-difference:mm\\:ss\\.fff}";
                diffColor = Color.DarkBlue;
            }
            else
            {
                diffText = "Время одинаковое!";
                diffColor = Color.DarkGreen;
            }

            diffLabel.Text = diffText;
            diffLabel.ForeColor = diffColor;

            // Победитель
            string winnerText = string.IsNullOrEmpty(Winner) ? "ПОБЕДИТЕЛЬ НЕ ОПРЕДЕЛЕН" : $"ПОБЕДИТЕЛЬ: {Winner.ToUpper()}";
            winnerLabel.Text = winnerText;
            winnerLabel.ForeColor = WinnerColor;
        }
    }
}