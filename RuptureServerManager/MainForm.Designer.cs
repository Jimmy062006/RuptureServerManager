using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace RuptureServerManager
{
	/// <summary>
	/// Partial class for MainForm that contains UI component definitions. All
	/// modifications to the UI layout belong here so that designer tools or
	/// manual changes can be kept separate from the logic in MainForm.cs.
	/// </summary>
	public partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private IContainer components = null;

		// UI control fields
		private NumericUpDown portNumericUpDown;
		private TextBox sessionNameTextBox;
		private NumericUpDown saveGameIntervalNumericUpDown;
		private CheckBox startNewGameCheckBox;
		private CheckBox loadSavedGameCheckBox;
		private TextBox saveGameNameTextBox;
		private Button saveSettingsButton;
		private Button startButton;
		private Button stopButton;
		private Button updateButton;
		private Label portLabel;
		private Label sessionLabel;
		private Label saveIntervalLabel;
		private Label saveGameNameLabel;

		// Renamed update controls
		private Label updateIntervalLabel;
		private CheckBox autoUpdateCheckBox;
		private GroupBox updateSettingsGroupBox;
		private TextBox updateIntervalTextBox;

		// New password controls
		private GroupBox passwordGroupBox;
		private Label adminPasswordLabel;
		private TextBox adminPasswordTextBox;
		private Button setAdminPasswordButton;
		private Label playerPasswordLabel;
		private TextBox playerPasswordTextBox;
		private Button setPlayerPasswordButton;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

        #region Windows Form Designer generated code

        /// <summary>
        /// Initialize UI components and layout. This method contains all code
        /// required to create and arrange controls on the form. It is invoked
        /// by the constructor in MainForm.cs.
        /// </summary>
        private void InitializeComponent()
        {
            portNumericUpDown = new NumericUpDown();
            sessionNameTextBox = new TextBox();
            saveGameIntervalNumericUpDown = new NumericUpDown();
            saveGameNameTextBox = new TextBox();
            saveSettingsButton = new Button();
            startButton = new Button();
            stopButton = new Button();
            updateButton = new Button();
            portLabel = new Label();
            sessionLabel = new Label();
            saveIntervalLabel = new Label();
            saveGameNameLabel = new Label();
            updateIntervalLabel = new Label();
            autoUpdateCheckBox = new CheckBox();
            updateSettingsGroupBox = new GroupBox();
            updateIntervalTextBox = new TextBox();
            passwordGroupBox = new GroupBox();
            setPlayerPasswordButton = new Button();
            playerPasswordTextBox = new TextBox();
            playerPasswordLabel = new Label();
            setAdminPasswordButton = new Button();
            adminPasswordTextBox = new TextBox();
            adminPasswordLabel = new Label();
            rtbConsoleLog = new RichTextBox();
            ((ISupportInitialize)portNumericUpDown).BeginInit();
            ((ISupportInitialize)saveGameIntervalNumericUpDown).BeginInit();
            updateSettingsGroupBox.SuspendLayout();
            passwordGroupBox.SuspendLayout();
            SuspendLayout();
            // 
            // portNumericUpDown
            // 
            portNumericUpDown.Location = new Point(120, 12);
            portNumericUpDown.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            portNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            portNumericUpDown.Name = "portNumericUpDown";
            portNumericUpDown.Size = new Size(120, 23);
            portNumericUpDown.TabIndex = 1;
            portNumericUpDown.Value = new decimal(new int[] { 7777, 0, 0, 0 });
            // 
            // sessionNameTextBox
            // 
            sessionNameTextBox.Location = new Point(120, 41);
            sessionNameTextBox.MaxLength = 20;
            sessionNameTextBox.Name = "sessionNameTextBox";
            sessionNameTextBox.Size = new Size(200, 23);
            sessionNameTextBox.TabIndex = 3;
            // 
            // saveGameIntervalNumericUpDown
            // 
            saveGameIntervalNumericUpDown.Location = new Point(120, 71);
            saveGameIntervalNumericUpDown.Maximum = new decimal(new int[] { 86400, 0, 0, 0 });
            saveGameIntervalNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            saveGameIntervalNumericUpDown.Name = "saveGameIntervalNumericUpDown";
            saveGameIntervalNumericUpDown.Size = new Size(120, 23);
            saveGameIntervalNumericUpDown.TabIndex = 5;
            saveGameIntervalNumericUpDown.Value = new decimal(new int[] { 300, 0, 0, 0 });
            // 
            // saveGameNameTextBox
            // 
            saveGameNameTextBox.Location = new Point(120, 102);
            saveGameNameTextBox.Name = "saveGameNameTextBox";
            saveGameNameTextBox.ReadOnly = true;
            saveGameNameTextBox.Size = new Size(200, 23);
            saveGameNameTextBox.TabIndex = 9;
            // 
            // saveSettingsButton
            // 
            saveSettingsButton.Location = new Point(340, 12);
            saveSettingsButton.Name = "saveSettingsButton";
            saveSettingsButton.Size = new Size(100, 25);
            saveSettingsButton.TabIndex = 10;
            saveSettingsButton.Text = "Save Settings";
            saveSettingsButton.UseVisualStyleBackColor = true;
            saveSettingsButton.Click += SaveSettingsButton_Click;
            // 
            // startButton
            // 
            startButton.Location = new Point(340, 75);
            startButton.Name = "startButton";
            startButton.Size = new Size(100, 25);
            startButton.TabIndex = 12;
            startButton.Text = "Start Server";
            startButton.UseVisualStyleBackColor = true;
            startButton.Click += StartButton_Click;
            // 
            // stopButton
            // 
            stopButton.Location = new Point(340, 107);
            stopButton.Name = "stopButton";
            stopButton.Size = new Size(100, 25);
            stopButton.TabIndex = 13;
            stopButton.Text = "Stop Server";
            stopButton.UseVisualStyleBackColor = true;
            stopButton.Click += StopButton_Click;
            // 
            // updateButton
            // 
            updateButton.Location = new Point(340, 44);
            updateButton.Name = "updateButton";
            updateButton.Size = new Size(100, 25);
            updateButton.TabIndex = 14;
            updateButton.Text = "Update Server";
            updateButton.UseVisualStyleBackColor = true;
            updateButton.Click += UpdateButton_Click;
            // 
            // portLabel
            // 
            portLabel.AutoSize = true;
            portLabel.Location = new Point(12, 15);
            portLabel.Name = "portLabel";
            portLabel.Size = new Size(32, 15);
            portLabel.TabIndex = 0;
            portLabel.Text = "Port:";
            // 
            // sessionLabel
            // 
            sessionLabel.AutoSize = true;
            sessionLabel.Location = new Point(12, 44);
            sessionLabel.Name = "sessionLabel";
            sessionLabel.Size = new Size(84, 15);
            sessionLabel.TabIndex = 2;
            sessionLabel.Text = "Session Name:";
            // 
            // saveIntervalLabel
            // 
            saveIntervalLabel.AutoSize = true;
            saveIntervalLabel.Location = new Point(12, 74);
            saveIntervalLabel.Name = "saveIntervalLabel";
            saveIntervalLabel.Size = new Size(92, 15);
            saveIntervalLabel.TabIndex = 4;
            saveIntervalLabel.Text = "Save Interval (s):";
            // 
            // saveGameNameLabel
            // 
            saveGameNameLabel.AutoSize = true;
            saveGameNameLabel.Location = new Point(12, 105);
            saveGameNameLabel.Name = "saveGameNameLabel";
            saveGameNameLabel.Size = new Size(103, 15);
            saveGameNameLabel.TabIndex = 8;
            saveGameNameLabel.Text = "Save Game Name:";
            // 
            // updateIntervalLabel
            // 
            updateIntervalLabel.AutoSize = true;
            updateIntervalLabel.Location = new Point(20, 50);
            updateIntervalLabel.Name = "updateIntervalLabel";
            updateIntervalLabel.Size = new Size(119, 15);
            updateIntervalLabel.TabIndex = 17;
            updateIntervalLabel.Text = "Check Interval (Mins)";
            // 
            // autoUpdateCheckBox
            // 
            autoUpdateCheckBox.AutoSize = true;
            autoUpdateCheckBox.CheckAlign = ContentAlignment.MiddleRight;
            autoUpdateCheckBox.Location = new Point(20, 25);
            autoUpdateCheckBox.Name = "autoUpdateCheckBox";
            autoUpdateCheckBox.Size = new Size(93, 19);
            autoUpdateCheckBox.TabIndex = 18;
            autoUpdateCheckBox.Text = "Auto Update";
            autoUpdateCheckBox.UseVisualStyleBackColor = true;
            // 
            // updateSettingsGroupBox
            // 
            updateSettingsGroupBox.Controls.Add(updateIntervalTextBox);
            updateSettingsGroupBox.Controls.Add(autoUpdateCheckBox);
            updateSettingsGroupBox.Controls.Add(updateIntervalLabel);
            updateSettingsGroupBox.Location = new Point(446, 15);
            updateSettingsGroupBox.Name = "updateSettingsGroupBox";
            updateSettingsGroupBox.Size = new Size(200, 100);
            updateSettingsGroupBox.TabIndex = 19;
            updateSettingsGroupBox.TabStop = false;
            updateSettingsGroupBox.Text = "Update Settings";
            // 
            // updateIntervalTextBox
            // 
            updateIntervalTextBox.Location = new Point(145, 47);
            updateIntervalTextBox.Name = "updateIntervalTextBox";
            updateIntervalTextBox.Size = new Size(44, 23);
            updateIntervalTextBox.TabIndex = 19;
            updateIntervalTextBox.KeyPress += UpdateInterval_KeyPress;
            // 
            // passwordGroupBox
            // 
            passwordGroupBox.Controls.Add(setPlayerPasswordButton);
            passwordGroupBox.Controls.Add(playerPasswordTextBox);
            passwordGroupBox.Controls.Add(playerPasswordLabel);
            passwordGroupBox.Controls.Add(setAdminPasswordButton);
            passwordGroupBox.Controls.Add(adminPasswordTextBox);
            passwordGroupBox.Controls.Add(adminPasswordLabel);
            passwordGroupBox.Location = new Point(652, 15);
            passwordGroupBox.Name = "passwordGroupBox";
            passwordGroupBox.Size = new Size(200, 180);
            passwordGroupBox.TabIndex = 20;
            passwordGroupBox.TabStop = false;
            passwordGroupBox.Text = "Passwords";
            // 
            // setPlayerPasswordButton
            // 
            setPlayerPasswordButton.Location = new Point(12, 152);
            setPlayerPasswordButton.Name = "setPlayerPasswordButton";
            setPlayerPasswordButton.Size = new Size(85, 23);
            setPlayerPasswordButton.TabIndex = 5;
            setPlayerPasswordButton.Text = "Set Player";
            setPlayerPasswordButton.UseVisualStyleBackColor = true;
            setPlayerPasswordButton.Click += SetPasswordButton_Click;
            // 
            // playerPasswordTextBox
            // 
            playerPasswordTextBox.Location = new Point(12, 126);
            playerPasswordTextBox.Name = "playerPasswordTextBox";
            playerPasswordTextBox.Size = new Size(176, 23);
            playerPasswordTextBox.TabIndex = 4;
            playerPasswordTextBox.UseSystemPasswordChar = true;
            // 
            // playerPasswordLabel
            // 
            playerPasswordLabel.AutoSize = true;
            playerPasswordLabel.Location = new Point(12, 108);
            playerPasswordLabel.Name = "playerPasswordLabel";
            playerPasswordLabel.Size = new Size(92, 15);
            playerPasswordLabel.TabIndex = 3;
            playerPasswordLabel.Text = "Player Password";
            // 
            // setAdminPasswordButton
            // 
            setAdminPasswordButton.Location = new Point(12, 72);
            setAdminPasswordButton.Name = "setAdminPasswordButton";
            setAdminPasswordButton.Size = new Size(85, 23);
            setAdminPasswordButton.TabIndex = 2;
            setAdminPasswordButton.Text = "Set Admin";
            setAdminPasswordButton.UseVisualStyleBackColor = true;
            setAdminPasswordButton.Click += SetPasswordButton_Click;
            // 
            // adminPasswordTextBox
            // 
            adminPasswordTextBox.Location = new Point(12, 46);
            adminPasswordTextBox.Name = "adminPasswordTextBox";
            adminPasswordTextBox.Size = new Size(176, 23);
            adminPasswordTextBox.TabIndex = 1;
            adminPasswordTextBox.UseSystemPasswordChar = true;
            // 
            // adminPasswordLabel
            // 
            adminPasswordLabel.AutoSize = true;
            adminPasswordLabel.Location = new Point(12, 28);
            adminPasswordLabel.Name = "adminPasswordLabel";
            adminPasswordLabel.Size = new Size(96, 15);
            adminPasswordLabel.TabIndex = 0;
            adminPasswordLabel.Text = "Admin Password";
            // 
            // rtbConsoleLog
            // 
            rtbConsoleLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            rtbConsoleLog.Location = new Point(12, 201);
            rtbConsoleLog.Name = "rtbConsoleLog";
            rtbConsoleLog.Size = new Size(840, 348);
            rtbConsoleLog.TabIndex = 21;
            rtbConsoleLog.Text = "";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(862, 561);
            Controls.Add(rtbConsoleLog);
            Controls.Add(updateSettingsGroupBox);
            Controls.Add(passwordGroupBox);
            Controls.Add(updateButton);
            Controls.Add(stopButton);
            Controls.Add(startButton);
            Controls.Add(saveSettingsButton);
            Controls.Add(saveGameNameTextBox);
            Controls.Add(saveGameNameLabel);
            Controls.Add(saveGameIntervalNumericUpDown);
            Controls.Add(saveIntervalLabel);
            Controls.Add(sessionNameTextBox);
            Controls.Add(sessionLabel);
            Controls.Add(portNumericUpDown);
            Controls.Add(portLabel);
            MinimumSize = new Size(800, 600);
            Name = "MainForm";
            Text = "Rupture Server Manager";
            Load += MainForm_Load;
            ((ISupportInitialize)portNumericUpDown).EndInit();
            ((ISupportInitialize)saveGameIntervalNumericUpDown).EndInit();
            updateSettingsGroupBox.ResumeLayout(false);
            updateSettingsGroupBox.PerformLayout();
            passwordGroupBox.ResumeLayout(false);
            passwordGroupBox.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RichTextBox rtbConsoleLog;
    }
}