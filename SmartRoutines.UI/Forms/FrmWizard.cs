
using System;
using System.Drawing;
using System.Windows.Forms;		
using SmartRoutines.Core.Interfaces.Logic;
using SmartRoutines.UI.Controls;
using SmartRoutines.UI.Controls.AddRoutine;
using SmartRoutines.UI.Controls.AddRoutine.UC_Step3;
using SmartRoutines.UI.Core.Theme;

namespace SmartRoutines.UI.Forms;

public class FrmWizard : Form
{
	private readonly IRoutineService _routineService;

	private Panel pnlHeader = null!;
	private Panel pnlStepper = null!;
	private Panel pnlSeparator = null!;
	private Panel pnlContent = null!;
	private Panel pnlBottom = null!;

	private Button btnBack = null!;
	private Button btnNext = null!;
	private Button btnClose = null!;

	private UC_WizardStepper ucStepper = null!;
	private UC_Step1_Identity step1 = null!;
	private UC_Step2_Trigger step2 = null!;
	private UC_ActionsMain step3 = null!;

	private int _currentStep = 1;

	public FrmWizard(IRoutineService routineService)
	{
		_routineService = routineService;
		ConfigureForm();
		InitializeUI();
	}

	private void ConfigureForm()
	{
		this.Text = "Create New Routine";
		this.Size = new Size(780, 720);
		this.MinimumSize = new Size(780, 720);
		this.StartPosition = FormStartPosition.CenterScreen;
		this.BackColor = SmartTheme.Background;
		this.ForeColor = Color.White;
		this.DoubleBuffered = true;
		this.FormBorderStyle = FormBorderStyle.None; 
	}

	private void InitializeUI()
	{
		pnlHeader = new Panel
		{
			Dock = DockStyle.Top,
			Height = 60,
			BackColor = SmartTheme.Background
		};

		var lblTitle = new Label
		{
			Text = "Create New Routine",
			Font = new Font("Segoe UI", 14f, FontStyle.Bold),
			ForeColor = Color.White,
			AutoSize = true,
			Location = new Point(24, 18)
		};

		btnClose = new Button
		{
			Text = "\u00D7",
			Font = new Font("Segoe UI", 18f),
			ForeColor = Color.Gray,
			BackColor = Color.Transparent,
			FlatStyle = FlatStyle.Flat,
			Size = new Size(40, 40),
			Cursor = Cursors.Hand,
			Anchor = AnchorStyles.Top | AnchorStyles.Right
		};
		btnClose.FlatAppearance.BorderSize = 0;
		btnClose.Click += (s, e) => this.Close();

		pnlHeader.Controls.Add(lblTitle);
		pnlHeader.Controls.Add(btnClose);
		this.Controls.Add(pnlHeader);

		pnlStepper = new Panel
		{
			Dock = DockStyle.Top,
			Height = 70,
			Padding = new Padding(24, 0, 24, 0)
		};
		ucStepper = new UC_WizardStepper { Dock = DockStyle.Fill };
		pnlStepper.Controls.Add(ucStepper);
		this.Controls.Add(pnlStepper);

		pnlSeparator = new Panel
		{
			Dock = DockStyle.Top,
			Height = 1,
			BackColor = Color.FromArgb(45, 45, 45)
		};
		this.Controls.Add(pnlSeparator);

		pnlBottom = new Panel
		{
			Dock = DockStyle.Bottom,
			Height = 70,
			Padding = new Padding(24, 15, 24, 15)
		};

		btnBack = new Button
		{
			Text = "< Back",
			Size = new Size(100, 38),
			BackColor = SmartTheme.Surface3,
			ForeColor = Color.White,
			FlatStyle = FlatStyle.Flat,
			Visible = false,
			Location = new Point(24, 15)
		};
		btnBack.Click += (s, e) => GoBack();

		btnNext = new Button
		{
			Text = "Next >",
			Size = new Size(130, 38),
			BackColor = SmartTheme.Primary,
			ForeColor = Color.White,
			FlatStyle = FlatStyle.Flat,
			Font = new Font("Segoe UI", 9f, FontStyle.Bold),
			Anchor = AnchorStyles.Right 
		};
		btnNext.Click += (s, e) => GoNext();

		pnlBottom.Controls.Add(btnBack);
		pnlBottom.Controls.Add(btnNext);
		this.Controls.Add(pnlBottom);

		pnlContent = new Panel
		{
			Dock = DockStyle.Fill,
			Padding = new Padding(24, 10, 24, 10)
		};
		this.Controls.Add(pnlContent);

		step1 = new UC_Step1_Identity();
		step2 = new UC_Step2_Trigger();
		step3 = new UC_ActionsMain(_routineService);
		this.Load += (s, e) => {
			btnNext.Location = new Point(pnlBottom.Width - btnNext.Width - 24, 15);
			btnClose.Location = new Point(pnlHeader.Width - 50, 10);
		};
		pnlBottom.Resize += (s, e) => btnNext.Left = pnlBottom.Width - btnNext.Width - 24;

		ShowStep(1);
	}

	private void ShowStep(int step)
	{
		pnlContent.Controls.Clear();
		_currentStep = step;
		Control active = step == 1 ? step1 : step == 2 ? step2 : (Control)step3;
		active.Dock = DockStyle.Fill;
		pnlContent.Controls.Add(active);

		btnBack.Visible = step > 1;
		btnNext.Text = step == 3 ? "\u2713 Finish & Save" : "Next >";
		btnNext.BackColor = step == 3 ? Color.FromArgb(46, 204, 113) : SmartTheme.Primary;

		ucStepper.SetStep(step);
	}

	private void GoNext()
	{
		if (_currentStep == 1)
		{
			if (!step1.ValidateIdentity()) return;
			ucStepper.MarkStepComplete(1);
			ShowStep(2);
		}
		else if (_currentStep == 2)
		{
			ucStepper.MarkStepComplete(2);
			ShowStep(3);
		}
		else if (_currentStep == 3)
		{
			OnFinish();
		}
	}

	private void GoBack() => ShowStep(_currentStep - 1);

	private void OnFinish()
	{
		MessageBox.Show($"Routine '{step1.RoutineName}' created successfully!", "Success");
		this.DialogResult = DialogResult.OK;
		this.Close();
	}
}