using SmartRoutines.UI.Core.Theme;
using SmartRoutines.Core.Interfaces.Logic;
using SmartRoutines.Core.DTOs;
using System;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using SmartRoutines.UI.Controls.AddRoutine;
using SmartRoutines.UI.Controls.AddRoutine.UC_Step3.ActionMainParts;
using Guna.UI2.WinForms;

namespace SmartRoutines.UI.Controls.AddRoutine.UC_Step3
{
    public partial class UC_ActionsMain : SmartUserControl
    {
        private UC_WizardStepper _stepper = null!;
        private UC_Step1_Identity _step1 = null!;
        private UC_Step2_Trigger _step2 = null!;
        // _step3 is declared in the designer partial; do not redeclare here.

        private Guna2Button _btnBack = null!;
        private Guna2Button _btnNext = null!;
        private Guna2Button _btnCancel = null!;

        private readonly IRoutineService _routineService;
        private int _currentStep = 1;

        public UC_ActionsMain(IRoutineService routineService)
        {
            _routineService = routineService;
            InitializeComponent();
            InitializeWizard();
        }

        private void InitializeWizard()
        {
            this.BackColor = SmartTheme.Background;

            // 1. Stepper
            _stepper = new UC_WizardStepper { Dock = DockStyle.Fill };
            pnlStepper.Controls.Add(_stepper);
            pnlStepper.Padding = new Padding(40, 0, 40, 0);

            // 2. Steps
            _step1 = new UC_Step1_Identity();
            _step2 = new UC_Step2_Trigger();
            // _step3 is created by the designer (added to pnlContent in InitializeComponent)
            _step3 = new UC_Step3_Actions();

            // 3. Footer Buttons
            _btnCancel = CreateFooterButton("Cancel", SmartTheme.Surface2, false);
            _btnBack = CreateFooterButton("< Back", SmartTheme.Surface3, false);
            _btnNext = CreateFooterButton("Next >", SmartTheme.Primary, true);

            _btnCancel.Click += (s, e) => CancelWizard();
            _btnBack.Click += (s, e) => GoBack();
            _btnNext.Click += (s, e) => GoNext();

            var flowButtons = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(0, 15, 40, 0),
                BackColor = Color.Transparent
            };
            flowButtons.Controls.Add(_btnNext);
            flowButtons.Controls.Add(_btnBack);
            flowButtons.Controls.Add(_btnCancel);
            pnlFooter.Controls.Add(flowButtons);

            ShowStep(1);
        }

        private Guna2Button CreateFooterButton(string text, Color color, bool isPrimary)
        {
            return new Guna2Button
            {
                Text = text,
                Size = new Size(120, 40),
                FillColor = color,
                ForeColor = Color.White,
                Font = isPrimary ? SmartTheme.FontBodyBold : SmartTheme.FontBody,
                BorderRadius = 8,
                Cursor = Cursors.Hand,
                Margin = new Padding(10, 0, 0, 0),
                Animated = true
            };
        }

        private void ShowStep(int step)
        {
            _currentStep = step;
            pnlContent.Controls.Clear();

            Control stepControl = step switch
            {
                1 => _step1,
                2 => _step2,
                3 => _step3,
                _ => _step1
            };

            stepControl.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(stepControl);

            _stepper.SetStep(step);
            _btnBack.Visible = (step > 1);
            _btnNext.Text = (step == 3) ? "✓ Finish" : "Next >";
            _btnNext.FillColor = (step == 3) ? SmartTheme.Success : SmartTheme.Primary;
        }

        private void GoNext()
        {
            if (_currentStep == 1)
            {
                if (!_step1.ValidateIdentity()) return;
                _stepper.MarkStepComplete(1);
                ShowStep(2);
            }
            else if (_currentStep == 2)
            {
                _stepper.MarkStepComplete(2);
                ShowStep(3);
            }
            else if (_currentStep == 3)
            {
                FinishWizard();
            }
        }

        private void GoBack()
        {
            ShowStep(_currentStep - 1);
        }

        private void CancelWizard()
        {
            ReturnToDashboard();
        }

        private async void FinishWizard()
        {
            string name = _step1.RoutineName;
            string desc = _step1.Description;

            var dto = new UpsertRoutineDto
            {
                Name = name,
                Description = desc,
                IconPath = _step1.SelectedIconKey,
                TriggerType = _step2.SelectedTriggerType,
                TriggerConfig = _step2.GetTriggerConfigJson(),
                Actions = _step3.Actions.Select((block, index) => block.ToDto(index + 1)).ToList()
            };

            try
            {
                await _routineService.SaveAsync(dto);

                if (this.ParentForm is SmartRoutines.UI.Forms.FrmMain main)
                {
                    // 1. Show success toast
                    main.ShowToast($"Routine '{name}' created successfully");

                    // 2. Signal the dashboard to reload data on next navigation
                    main.RequestDashboardRefresh();

                    // 3. Go home
                    main.DisplayPage<UC_Dashboard>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving routine: " + ex.Message);
            }
        }

        private void ReturnToDashboard()
        {
            if (this.ParentForm is SmartRoutines.UI.Forms.FrmMain main)
            {
                main.DisplayPage<UC_Dashboard>();
            }
        }
    }
}
