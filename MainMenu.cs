using AI_maze;

namespace WinFormsApp1
{
    public partial class MainMenu : Form
    {
        private Player player;
        int complexity = 10;

        public MainMenu()
        {
            

            //инициализация всего
            InitializeComponent();
            InitializePlayer();
            SpawnButton();


            this.DoubleBuffered = true;
            this.KeyDown += MainMenuForm_KeyDown;
            this.BackColor = Color.Black;
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

        private void ComplexityChoice(object sender, EventArgs e)
        {

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
