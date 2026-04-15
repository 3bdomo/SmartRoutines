using System.Drawing.Drawing2D;
using SmartRoutines.UI.Core.Theme;



namespace SmartRoutines.UI.Controls.AddRoutine;

/// <summary>
/// Wizard stepper control showing 3 steps with progress indicators.
/// </summary>
public class UC_WizardStepper : SmartUserControl
{
    private int _currentStep = 1;
    private readonly int[] _completedSteps = [0, 0, 0];

    /// <summary>
    /// Gets or sets the current step number (1-3).
    /// </summary>
    public int CurrentStep
    {
        get => _currentStep;
        set => SetStep(value);
    }

    /// <summary>
    /// Initializes a new instance of the UC_WizardStepper control.
    /// </summary>
    public UC_WizardStepper()
    {
        Size = new Size(400, 60);
        BackColor = SmartTheme.Background;
        DoubleBuffered = true;
    }

    /// <summary>
    /// Sets the current step and redraws the stepper.
    /// </summary>
    /// <param name="step">The step number (1-3)</param>
    public void SetStep(int step)
    {
        _currentStep = Math.Clamp(step, 1, 3);
        Invalidate();
    }

    /// <summary>
    /// Marks a step as completed.
    /// </summary>
    /// <param name="step">The step number to mark as complete</param>
    public void MarkStepComplete(int step)
    {
        if (step >= 1 && step <= 3)
        {
            _completedSteps[step - 1] = 1;
            Invalidate();
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        string[] labels = { "Identity", "Trigger", "Actions" };
        int circleSize = 32;
        int totalWidth = Width;
        int spacing = (totalWidth - 3 * circleSize) / 4;
        int y = (Height - circleSize - 20) / 2; // vertically center circles+labels

        int[] cx =
        [
            spacing + circleSize / 2,
            spacing * 2 + circleSize + circleSize / 2,
            spacing * 3 + circleSize * 2 + circleSize / 2
        ];

        // Draw connectors first (behind circles)
        for (int i = 0; i < 2; i++)
        {
            int x1 = cx[i] + circleSize / 2 + 2;
            int x2 = cx[i + 1] - circleSize / 2 - 2;
            int lineY = y + circleSize / 2;
            bool completed = _currentStep > i + 1;
            using var pen = new Pen(completed ? Color.FromArgb(0, 120, 212) : Color.FromArgb(68, 68, 68), 2);
            g.DrawLine(pen, x1, lineY, x2, lineY);
        }

        // Draw circles
        for (int i = 0; i < 3; i++)
        {
            int rx = cx[i] - circleSize / 2;
            var rect = new Rectangle(rx, y, circleSize, circleSize);
            bool isActive = _currentStep == i + 1;
            bool isDone = _completedSteps[i] == 1;

            if (isActive || isDone)
            {
                using var brush = new SolidBrush(Color.FromArgb(0, 120, 212));
                g.FillEllipse(brush, rect);
            }
            else
            {
                using var brush = new SolidBrush(Color.FromArgb(30, 30, 30));
                g.FillEllipse(brush, rect);
                using var pen = new Pen(Color.FromArgb(68, 68, 68), 1.5f);
                g.DrawEllipse(pen, rect);
            }

            if (isDone)
            {
                // Draw checkmark
                using var pen = new Pen(Color.White, 2f);
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                int cx2 = cx[i];
                int cy2 = y + circleSize / 2;
                var points = new Point[]
                {
                    new(cx2 - 7, cy2),
                    new(cx2 - 2, cy2 + 5),
                    new(cx2 + 7, cy2 - 5)
                };
                g.DrawLines(pen, points);
            }
            else
            {
                // Draw number
                using var font = new Font("Segoe UI", 9f, FontStyle.Bold);
                using var brush = new SolidBrush(isActive ? Color.White : Color.FromArgb(120, 120, 120));
                var numStr = (i + 1).ToString();
                var sz = g.MeasureString(numStr, font);
                g.DrawString(numStr, font, brush,
                    cx[i] - sz.Width / 2,
                    y + circleSize / 2 - sz.Height / 2);
            }

            // Draw label below circle
            using var lblFont = new Font("Segoe UI", 8f);
            using var lblBrush = new SolidBrush(_currentStep >= i + 1 ? Color.White : Color.FromArgb(100, 100, 100));
            var lblSz = g.MeasureString(labels[i], lblFont);
            g.DrawString(labels[i], lblFont, lblBrush,
                cx[i] - lblSz.Width / 2,
                y + circleSize + 4);
        }
    }
}
