using WinFormsApp1.Class;

namespace WinFormsApp1
{
    public partial class MainMenu : Form
    {
        private bool isMenuVisible = false;
        private Panel menuPanel;
        private Player player;
        int complexity = 10;
        private TimerForGame gameTimer;

        public MainMenu()
        {
            //инициализация всего
            InitializeComponent();
            InitializePlayer();
            SpawnButton();
            InitializeMenuPanel();


            this.DoubleBuffered = true;
            this.KeyDown += MainMenuForm_KeyDown;
            this.BackColor = Color.Black;

            CreateTimerObj();
        } 

        public void CreateTimerObj()
        {
            gameTimer = new TimerForGame(this.ClientSize, this);
            gameTimer.CreateTimer();
        }

        private void SpawnButton()
        {
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.KeyPreview = true;
            this.BackColor = Color.Black;

            Button startButton = new Button();
            startButton.Text = "Играть";
            startButton.Size = new Size(
                (int)(this.ClientSize.Width * 0.30),
                (int)(this.ClientSize.Height * 0.10)
            );
            startButton.Location = new Point(
                (this.ClientSize.Width - startButton.Width) / 2,
                (int)(this.ClientSize.Height * 0.4)
            );
            startButton.BackColor = Color.DarkBlue;
            startButton.ForeColor = Color.White;
            float fontSize = startButton.Height * 0.3f;
            startButton.Font = new Font("Arial", fontSize, FontStyle.Bold);
            startButton.FlatStyle = FlatStyle.Flat;
            startButton.FlatAppearance.BorderSize = 2;
            startButton.FlatAppearance.BorderColor = Color.White;
            startButton.Click += StartGame;
            this.Controls.Add(startButton);

            Button exitButton = new Button();
            exitButton.Text = "Выход";
            exitButton.Size = startButton.Size;
            exitButton.Location = new Point(
                (this.ClientSize.Width - exitButton.Width) / 2,
                (int)(this.ClientSize.Height * 0.55)
            );
            exitButton.BackColor = Color.DarkRed;
            exitButton.ForeColor = Color.White;
            exitButton.Font = new Font("Arial", fontSize, FontStyle.Bold);
            exitButton.FlatStyle = FlatStyle.Flat;
            exitButton.FlatAppearance.BorderSize = 2;
            exitButton.FlatAppearance.BorderColor = Color.White;

            exitButton.Click += Exit_clic;
            this.Controls.Add(exitButton);

            Button complexityChoice = new Button();
            complexityChoice.Text = "Выбор сложности";
            complexityChoice.Size = startButton.Size;
            complexityChoice.Location = new Point(
                (this.ClientSize.Width - exitButton.Width) / 2 + 200,
                (int)(this.ClientSize.Height * 0.55) + 200
            );
            complexityChoice.BackColor = Color.DarkRed;
            complexityChoice.ForeColor = Color.White;
            complexityChoice.Font = new Font("Arial", fontSize, FontStyle.Bold);
            complexityChoice.FlatStyle = FlatStyle.Flat;
            complexityChoice.FlatAppearance.BorderSize = 2;
            complexityChoice.FlatAppearance.BorderColor = Color.White;
            complexityChoice.Click += ComplexityChoice;
            this.Controls.Add(complexityChoice);
        }

        private void InitializeMenuPanel()
        {
            // Создаем панель меню
            menuPanel = new Panel();
            menuPanel.Size = new Size(300, 200);
            menuPanel.Location = new Point(
                (this.ClientSize.Width - 300) / 2,
                (this.ClientSize.Height - 200) / 2
            );
            menuPanel.BackColor = Color.DarkSlateBlue;
            menuPanel.BorderStyle = BorderStyle.FixedSingle;
            menuPanel.Visible = false; // Изначально скрыта

            // Добавляем заголовок
            Label titleLabel = new Label();
            titleLabel.Text = "Выбор сложности";
            titleLabel.Font = new Font("Arial", 14, FontStyle.Bold);
            titleLabel.ForeColor = Color.White;
            titleLabel.Size = new Size(280, 30);
            titleLabel.Location = new Point(10, 10);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            menuPanel.Controls.Add(titleLabel);

            // Кнопка "Легко"
            Button easyButton = new Button();
            easyButton.Text = "Легко (5x5)";
            easyButton.Size = new Size(200, 40);
            easyButton.Location = new Point(50, 50);
            easyButton.BackColor = Color.Green;
            easyButton.ForeColor = Color.White;
            easyButton.Font = new Font("Arial", 10, FontStyle.Bold);
            easyButton.Click += (s, e) =>
            {
                complexity = 5;
                HideMenu();
                MessageBox.Show("Выбрана легкая сложность!");
            };
            menuPanel.Controls.Add(easyButton);

            // Кнопка "Средне"
            Button mediumButton = new Button();
            mediumButton.Text = "Средне (10x10)";
            mediumButton.Size = new Size(200, 40);
            mediumButton.Location = new Point(50, 100);
            mediumButton.BackColor = Color.Orange;
            mediumButton.ForeColor = Color.White;
            mediumButton.Font = new Font("Arial", 10, FontStyle.Bold);
            mediumButton.Click += (s, e) =>
            {
                complexity = 10;
                HideMenu();
                MessageBox.Show("Выбрана средняя сложность!");
            };
            menuPanel.Controls.Add(mediumButton);

            // Кнопка "Сложно"
            Button hardButton = new Button();
            hardButton.Text = "Сложно (15x15)";
            hardButton.Size = new Size(200, 40);
            hardButton.Location = new Point(50, 150);
            hardButton.BackColor = Color.Red;
            hardButton.ForeColor = Color.White;
            hardButton.Font = new Font("Arial", 10, FontStyle.Bold);
            hardButton.Click += (s, e) =>
            {
                complexity = 15;
                HideMenu();
                MessageBox.Show("Выбрана сложная сложность!");
            };
            menuPanel.Controls.Add(hardButton);

            // Кнопка закрытия меню
            Button closeButton = new Button();
            closeButton.Text = "?";
            closeButton.Size = new Size(30, 30);
            closeButton.Location = new Point(260, 5);
            closeButton.BackColor = Color.DarkRed;
            closeButton.ForeColor = Color.White;
            closeButton.Font = new Font("Arial", 12, FontStyle.Bold);
            closeButton.Click += (s, e) => HideMenu();
            menuPanel.Controls.Add(closeButton);

            this.Controls.Add(menuPanel);
            menuPanel.BringToFront();
        }

        private void ShowMenu()
        {
            isMenuVisible = true;
            menuPanel.Visible = true;
            menuPanel.BringToFront();

            // Затемняем фон
            foreach (Control control in this.Controls)
            {
                if (control != menuPanel)
                    control.Enabled = false;
            }
        }

        private void HideMenu()
        {
            isMenuVisible = false;
            menuPanel.Visible = false;

            // Восстанавливаем фон
            foreach (Control control in this.Controls)
            {
                control.Enabled = true;
            }
        }

        private void ComplexityChoice(object sender, EventArgs e)
        {
            ShowMenu();
        }

        private void StartGame(object sender, EventArgs eventArgs)
        {
            GameForm gameForm = new GameForm(complexity);
            gameForm.Show();

            // Скрываем текущую форму (меню)
            this.Hide();

            // Обработчик закрытия игровой формы
            gameForm.FormClosed += (s, args) =>
            {
                this.Show(); // Показываем меню снова при закрытии игры
            };
        }
        private void Exit_clic(object srnder, EventArgs e)
        {
            this.Close();
        }

        private void InitializePlayer()
        {
            player = new Player();
            this.Controls.Add(player.PictureBox);
            player.PictureBox.BringToFront(); // Чтобы был поверх других элементов
            ResizePlayer(); // Первоначальная установка размера

        }
        private void ResizePlayer()
        {
            int playerSize = (int)(this.ClientSize.Width * 0.08);
            player.PictureBox.Size = new Size(playerSize, playerSize);

            player.PictureBox.Location = new Point(
                (int)(this.ClientSize.Width * 0.1),
                (int)(this.ClientSize.Height * 0.8)
            );
        }

        private void MainMenuForm_KeyDown(object sender, KeyEventArgs e)
        {
            int deltaX = 0, deltaY = 0;

            switch (e.KeyCode)
            {
                case Keys.W: deltaY = -player.Speed; break;
                case Keys.S: deltaY = player.Speed; break;
                case Keys.A: deltaX = -player.Speed; break;
                case Keys.D: deltaX = player.Speed; break;
            }

            player.Move(deltaX, deltaY);
            player.CheckBoundaries(this);
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {

        }
    }

}
