
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp1.Class
{
    public class Player
    {
        public PictureBox PictureBox { get; private set; }
        public bool IsFacingRight { get; private set; } = true;
        public int Speed { get; set; } = 5;

        private Image originalImage;
        private Image flippedImage;

        public Player()
        {
            PictureBox = new PictureBox();
            PictureBox.Size = new Size(50, 50);
            PictureBox.Location = new Point(100, 100);
            PictureBox.BackColor = Color.Blue;

            LoadTextures();
        }

        private void LoadTextures()
        {
            // Получаем путь к корневой папке проекта
            string projectRoot = GetProjectRootDirectory();
            string texturesPath = Path.Combine(projectRoot, "Textures");

            string originalPath = Path.Combine(texturesPath, "minecraft-stev.gif");
            string flippedPath = Path.Combine(texturesPath, "minecraft-stevFlip.gif");

            if (File.Exists(originalPath) && File.Exists(flippedPath))
            {
                try
                {
                    originalImage = Image.FromFile(originalPath);
                    flippedImage = Image.FromFile(flippedPath);
                    PictureBox.Image = originalImage;
                    PictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                    PictureBox.BackColor = Color.Transparent;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show($"Файлы не найдены:\n{originalPath}\n{flippedPath}");
            }
        }

        private string GetProjectRootDirectory()
        {
            string currentDir = Directory.GetCurrentDirectory();
            DirectoryInfo dir = new DirectoryInfo(currentDir);

            // Поднимаемся до папки WinFormsApp1
            for (int i = 0; i < 3; i++)
            {
                if (dir.Parent != null)
                    dir = dir.Parent;
            }

            return dir.FullName;
        }

        public void Move(int deltaX, int deltaY)
        {
            PictureBox.Left += deltaX;
            PictureBox.Top += deltaY;

            if (deltaX < 0 && IsFacingRight)
            {
                Flip();
            }
            else if (deltaX > 0 && !IsFacingRight)
            {
                Flip();
            }
        }

        public void Flip()
        {
            IsFacingRight = !IsFacingRight;

            if (originalImage != null && flippedImage != null)
            {
                PictureBox.Image = IsFacingRight ? originalImage : flippedImage;
            }
        }

        public void CheckBoundaries(Form form)
        {
            if (PictureBox.Top < 0)
                PictureBox.Top = 0;
            if (PictureBox.Top > form.ClientSize.Height - PictureBox.Height)
                PictureBox.Top = form.ClientSize.Height - PictureBox.Height;
            if (PictureBox.Left < 0)
                PictureBox.Left = 0;
            if (PictureBox.Left > form.ClientSize.Width - PictureBox.Width)
                PictureBox.Left = form.ClientSize.Width - PictureBox.Width;
        }
    }
}