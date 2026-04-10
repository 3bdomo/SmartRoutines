using Guna.UI2.WinForms;
using System.ComponentModel;

namespace SmartRoutines.UI.Core.Theme
{

    public class SmartUserControl : UserControl
    {
        protected Guna2Transition _transition;

        protected bool IsInDesignMode =>
            DesignMode ||
            LicenseManager.UsageMode == LicenseUsageMode.Designtime ||
            (Site?.DesignMode ?? false);

        public SmartUserControl()
        {
            this.BackColor = SmartTheme.Background;
            this.Font = SmartTheme.FontBody;
            this.ForeColor = SmartTheme.TextPrimary;
            this.DoubleBuffered = true;

            _transition = new Guna2Transition();
            _transition.AnimationType = Guna.UI2.AnimatorNS.AnimationType.Transparent;
            _transition.Interval = 10;
            _transition.MaxAnimationTime = 500;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsInDesignMode)
            {
                this.Visible = false;
                _transition.ShowSync(this);
            }
        }
    }
}
