using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using SmartRoutines.UI.Core.Theme;
using Timer = System.Windows.Forms.Timer;

namespace SmartRoutines.UI.Controls.Common
{
    public partial class UC_Toast : SmartUserControl
    {
        private readonly Timer _timer;
        private int _displayTime = 5000; // 5 seconds

        public UC_Toast(string message)
        {
            InitializeComponent();
            SetupUI(message);

            _timer = new Timer { Interval = _displayTime };
            _timer.Tick += (s, e) => {
                _timer.Stop();
                FadeOutAndClose();
            };
        }

        private void SetupUI(string message)
        {
            this.Size = new Size(320, 60);
            this.BackColor = Color.Transparent;

            var pnlMain = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                FillColor = Color.FromArgb(20, 20, 24), // Elegant dark background
                BorderRadius = 12,
                BorderThickness = 1,
                BorderColor = SmartTheme.Success,
                ShadowDecoration = { Enabled = true, Color = Color.Black, Depth = 15, Shadow = new Padding(0, 0, 5, 5) }
            };

            var picIcon = new Guna2CirclePictureBox
            {
                Size = new Size(24, 24),
                Location = new Point(16, 18),
                FillColor = SmartTheme.Success,
                Image = SmartRoutines.UI.Core.Helper.IconLoader.GetIcon("success.png", 14),
                SizeMode = PictureBoxSizeMode.CenterImage,
                ImageRotate = 0F
            };

            var lblMsg = new Label
            {
                Text = message,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Location = new Point(50, 0),
                Size = new Size(250, 60),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };

            pnlMain.Controls.Add(picIcon);
            pnlMain.Controls.Add(lblMsg);
            this.Controls.Add(pnlMain);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            _timer.Start();
            
            // Initial animation: fade in (simple opactiy doesn't work well for controls, so we rely on visibility/position)
            this.Location = new Point(this.Location.X, this.Location.Y + 20);
            Timer anim = new Timer { Interval = 16 };
            int step = 0;
            anim.Tick += (s, ev) => {
                step++;
                this.Location = new Point(this.Location.X, this.Location.Y - 2);
                if (step >= 10) anim.Stop();
            };
            anim.Start();
        }

        private void FadeOutAndClose()
        {
            Timer anim = new Timer { Interval = 16 };
            int step = 0;
            anim.Tick += (s, ev) => {
                step++;
                this.Location = new Point(this.Location.X, this.Location.Y - 2);
                float opacity = 1.0f - (step / 10f); // Manual alpha imitation if possible, but simpler to just move and hide
                if (step >= 10)
                {
                    anim.Stop();
                    this.Parent?.Controls.Remove(this);
                    this.Dispose();
                }
            };
            anim.Start();
        }

        // Standard WinForms boilerplate for manual construction
        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "UC_Toast";
            this.ResumeLayout(false);
        }
    }
}
