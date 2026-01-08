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

        private NumericUpDown portNumericUpDown;
        private TextBox sessionNameTextBox;
        private NumericUpDown saveGameIntervalNumericUpDown;
        private CheckBox startNewGameCheckBox;
        private CheckBox loadSavedGameCheckBox;
        private TextBox saveGameNameTextBox;
        private Button saveSettingsButton;
        private Button downloadSteamCmdButton;
        private Button startButton;
        private Button stopButton;
        private Button updateButton;
        private RichTextBox consoleTextBox;
        private Label portLabel;
        private Label sessionLabel;
        private Label saveIntervalLabel;
        private Label saveGameNameLabel;

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
			startNewGameCheckBox = new CheckBox();
			loadSavedGameCheckBox = new CheckBox();
			saveGameNameTextBox = new TextBox();
			saveSettingsButton = new Button();
			downloadSteamCmdButton = new Button();
			startButton = new Button();
			stopButton = new Button();
			updateButton = new Button();
			consoleTextBox = new RichTextBox();
			portLabel = new Label();
			sessionLabel = new Label();
			saveIntervalLabel = new Label();
			saveGameNameLabel = new Label();
			((ISupportInitialize)portNumericUpDown).BeginInit();
			((ISupportInitialize)saveGameIntervalNumericUpDown).BeginInit();
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
			// startNewGameCheckBox
			// 
			startNewGameCheckBox.AutoSize = true;
			startNewGameCheckBox.Checked = true;
			startNewGameCheckBox.CheckState = CheckState.Checked;
			startNewGameCheckBox.Location = new Point(12, 100);
			startNewGameCheckBox.Name = "startNewGameCheckBox";
			startNewGameCheckBox.Size = new Size(111, 19);
			startNewGameCheckBox.TabIndex = 6;
			startNewGameCheckBox.Text = "Start New Game";
			startNewGameCheckBox.UseVisualStyleBackColor = true;
			// 
			// loadSavedGameCheckBox
			// 
			loadSavedGameCheckBox.AutoSize = true;
			loadSavedGameCheckBox.Location = new Point(150, 100);
			loadSavedGameCheckBox.Name = "loadSavedGameCheckBox";
			loadSavedGameCheckBox.Size = new Size(120, 19);
			loadSavedGameCheckBox.TabIndex = 7;
			loadSavedGameCheckBox.Text = "Load Saved Game";
			loadSavedGameCheckBox.UseVisualStyleBackColor = true;
			// 
			// saveGameNameTextBox
			// 
			saveGameNameTextBox.Location = new Point(120, 126);
			saveGameNameTextBox.Name = "saveGameNameTextBox";
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
			// downloadSteamCmdButton
			// 
			downloadSteamCmdButton.Location = new Point(340, 44);
			downloadSteamCmdButton.Name = "downloadSteamCmdButton";
			downloadSteamCmdButton.Size = new Size(100, 25);
			downloadSteamCmdButton.TabIndex = 11;
			downloadSteamCmdButton.Text = "Download SteamCMD";
			downloadSteamCmdButton.UseVisualStyleBackColor = true;
			downloadSteamCmdButton.Click += DownloadSteamCmdButton_Click;
			// 
			// startButton
			// 
			startButton.Location = new Point(340, 76);
			startButton.Name = "startButton";
			startButton.Size = new Size(100, 25);
			startButton.TabIndex = 12;
			startButton.Text = "Start";
			startButton.UseVisualStyleBackColor = true;
			startButton.Click += StartButton_Click;
			// 
			// stopButton
			// 
			stopButton.Location = new Point(340, 108);
			stopButton.Name = "stopButton";
			stopButton.Size = new Size(100, 25);
			stopButton.TabIndex = 13;
			stopButton.Text = "Stop";
			stopButton.UseVisualStyleBackColor = true;
			stopButton.Click += StopButton_Click;
			// 
			// updateButton
			// 
			updateButton.Location = new Point(340, 140);
			updateButton.Name = "updateButton";
			updateButton.Size = new Size(100, 25);
			updateButton.TabIndex = 14;
			updateButton.Text = "Update";
			updateButton.UseVisualStyleBackColor = true;
			updateButton.Click += UpdateButton_Click;
			// 
			// consoleTextBox
			// 
			consoleTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			consoleTextBox.Location = new Point(12, 180);
			consoleTextBox.Name = "consoleTextBox";
			consoleTextBox.ReadOnly = true;
			consoleTextBox.Size = new Size(760, 370);
			consoleTextBox.TabIndex = 15;
			consoleTextBox.Text = "";
			consoleTextBox.WordWrap = false;
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
			saveGameNameLabel.Location = new Point(12, 129);
			saveGameNameLabel.Name = "saveGameNameLabel";
			saveGameNameLabel.Size = new Size(103, 15);
			saveGameNameLabel.TabIndex = 8;
			saveGameNameLabel.Text = "Save Game Name:";
			// 
			// MainForm
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(784, 561);
			Controls.Add(consoleTextBox);
			Controls.Add(updateButton);
			Controls.Add(stopButton);
			Controls.Add(startButton);
			Controls.Add(downloadSteamCmdButton);
			Controls.Add(saveSettingsButton);
			Controls.Add(saveGameNameTextBox);
			Controls.Add(saveGameNameLabel);
			Controls.Add(loadSavedGameCheckBox);
			Controls.Add(startNewGameCheckBox);
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
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion
	}
}