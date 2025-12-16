using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1.Class
{
    public class TimerForGame
    {
        private Label timeLabel;
        private System.Windows.Forms.Timer gameTimer;
        private int elapsedSeconds = 0;
        Size _clientSize;
        Form _form;

        public TimerForGame(Size ClientSize, Form form) 
        {
            _clientSize= ClientSize;
            _form= form;
        }

        public void StopTimer()
        {
            gameTimer.Stop();
        }

        public int GetElapsedSeconds()
        {
            return elapsedSeconds;
        }

        public void CreateTimer()
        {
            timeLabel = new Label();
            timeLabel.Text = "00:00";
            timeLabel.Size = new Size(80, 30);
            timeLabel.Font = new Font("Consolas", 14, FontStyle.Bold);
            timeLabel.ForeColor = Color.White;
            timeLabel.BackColor = Color.Black;
            timeLabel.TextAlign = ContentAlignment.MiddleCenter;
            timeLabel.BorderStyle = BorderStyle.FixedSingle;
            timeLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            timeLabel.Location = new Point(
                _clientSize.Width - timeLabel.Width - 20,
                20
            );
            _form.Controls.Add(timeLabel);

            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = 1000;
            gameTimer.Tick += GameTimerTick;
            StartTimer();

        }

        public void GameTimerTick(object sender, EventArgs e)
        {

            elapsedSeconds++;
            TimeSpan time = TimeSpan.FromSeconds(elapsedSeconds);
            timeLabel.Text = string.Format("{0:00}:{1:00}", (int)time.TotalMinutes, time.Seconds);
        }

        public void StartTimer()
        {

            elapsedSeconds = 0;
            timeLabel.Text = "00:00";
            gameTimer.Start();

        }
    }
}
