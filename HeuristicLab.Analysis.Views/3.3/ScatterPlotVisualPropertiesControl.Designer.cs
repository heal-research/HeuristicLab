#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

namespace HeuristicLab.Analysis.Views {
  partial class ScatterPlotVisualPropertiesControl {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      this.label1 = new System.Windows.Forms.Label();
      this.yAxisTitleTextBox = new System.Windows.Forms.TextBox();
      this.panel2 = new System.Windows.Forms.Panel();
      this.xAxisMaximumFixedRadioButton = new System.Windows.Forms.RadioButton();
      this.xAxisMaximumAutoRadioButton = new System.Windows.Forms.RadioButton();
      this.panel1 = new System.Windows.Forms.Panel();
      this.xAxisMinimumFixedRadioButton = new System.Windows.Forms.RadioButton();
      this.xAxisMinimumAutoRadioButton = new System.Windows.Forms.RadioButton();
      this.xAxisMinimumFixedTextBox = new System.Windows.Forms.TextBox();
      this.xAxisMaximumFixedTextBox = new System.Windows.Forms.TextBox();
      this.label12 = new System.Windows.Forms.Label();
      this.label9 = new System.Windows.Forms.Label();
      this.xAxisTitleTextBox = new System.Windows.Forms.TextBox();
      this.label11 = new System.Windows.Forms.Label();
      this.axisTabControl = new System.Windows.Forms.TabControl();
      this.xAxisTabPage = new System.Windows.Forms.TabPage();
      this.xAxisGridCheckBox = new System.Windows.Forms.CheckBox();
      this.label4 = new System.Windows.Forms.Label();
      this.yAxisTabPage = new System.Windows.Forms.TabPage();
      this.yAxisGridCheckBox = new System.Windows.Forms.CheckBox();
      this.label5 = new System.Windows.Forms.Label();
      this.panel6 = new System.Windows.Forms.Panel();
      this.yAxisMaximumFixedRadioButton = new System.Windows.Forms.RadioButton();
      this.yAxisMaximumAutoRadioButton = new System.Windows.Forms.RadioButton();
      this.panel5 = new System.Windows.Forms.Panel();
      this.yAxisMinimumAutoRadioButton = new System.Windows.Forms.RadioButton();
      this.yAxisMinimumFixedRadioButton = new System.Windows.Forms.RadioButton();
      this.yAxisMinimumFixedTextBox = new System.Windows.Forms.TextBox();
      this.yAxisMaximumFixedTextBox = new System.Windows.Forms.TextBox();
      this.label8 = new System.Windows.Forms.Label();
      this.label7 = new System.Windows.Forms.Label();
      this.titleTextBox = new System.Windows.Forms.TextBox();
      this.label15 = new System.Windows.Forms.Label();
      this.axisFontLabel = new System.Windows.Forms.Label();
      this.titleFontLabel = new System.Windows.Forms.Label();
      this.axisFontButton = new System.Windows.Forms.Button();
      this.label3 = new System.Windows.Forms.Label();
      this.titleFontButton = new System.Windows.Forms.Button();
      this.label2 = new System.Windows.Forms.Label();
      this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
      this.titleFontDialog = new System.Windows.Forms.FontDialog();
      this.axisFontDialog = new System.Windows.Forms.FontDialog();
      this.panel2.SuspendLayout();
      this.panel1.SuspendLayout();
      this.axisTabControl.SuspendLayout();
      this.xAxisTabPage.SuspendLayout();
      this.yAxisTabPage.SuspendLayout();
      this.panel6.SuspendLayout();
      this.panel5.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(6, 12);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(30, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "&Title:";
      // 
      // yAxisPrimaryTitleTextBox
      // 
      this.yAxisTitleTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.yAxisTitleTextBox.Location = new System.Drawing.Point(71, 9);
      this.yAxisTitleTextBox.Name = "yAxisPrimaryTitleTextBox";
      this.yAxisTitleTextBox.Size = new System.Drawing.Size(370, 20);
      this.yAxisTitleTextBox.TabIndex = 1;
      this.yAxisTitleTextBox.Validated += new System.EventHandler(this.yTitleTextBox_Validated);
      // 
      // panel2
      // 
      this.panel2.Controls.Add(this.xAxisMaximumFixedRadioButton);
      this.panel2.Controls.Add(this.xAxisMaximumAutoRadioButton);
      this.panel2.Location = new System.Drawing.Point(71, 61);
      this.panel2.Name = "panel2";
      this.panel2.Size = new System.Drawing.Size(106, 16);
      this.panel2.TabIndex = 6;
      // 
      // xAxisPrimaryMaximumFixedRadioButton
      // 
      this.xAxisMaximumFixedRadioButton.AutoSize = true;
      this.xAxisMaximumFixedRadioButton.Location = new System.Drawing.Point(53, 1);
      this.xAxisMaximumFixedRadioButton.Name = "xAxisPrimaryMaximumFixedRadioButton";
      this.xAxisMaximumFixedRadioButton.Size = new System.Drawing.Size(53, 17);
      this.xAxisMaximumFixedRadioButton.TabIndex = 1;
      this.xAxisMaximumFixedRadioButton.TabStop = true;
      this.xAxisMaximumFixedRadioButton.Text = "&Fixed:";
      this.xAxisMaximumFixedRadioButton.UseVisualStyleBackColor = true;
      this.xAxisMaximumFixedRadioButton.CheckedChanged += new System.EventHandler(this.xAxisMaximumRadioButton_CheckedChanged);
      // 
      // xAxisPrimaryMaximumAutoRadioButton
      // 
      this.xAxisMaximumAutoRadioButton.AutoSize = true;
      this.xAxisMaximumAutoRadioButton.Location = new System.Drawing.Point(0, 1);
      this.xAxisMaximumAutoRadioButton.Name = "xAxisPrimaryMaximumAutoRadioButton";
      this.xAxisMaximumAutoRadioButton.Size = new System.Drawing.Size(47, 17);
      this.xAxisMaximumAutoRadioButton.TabIndex = 0;
      this.xAxisMaximumAutoRadioButton.TabStop = true;
      this.xAxisMaximumAutoRadioButton.Text = "&Auto";
      this.xAxisMaximumAutoRadioButton.UseVisualStyleBackColor = true;
      this.xAxisMaximumAutoRadioButton.CheckedChanged += new System.EventHandler(this.xAxisMaximumRadioButton_CheckedChanged);
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.xAxisMinimumFixedRadioButton);
      this.panel1.Controls.Add(this.xAxisMinimumAutoRadioButton);
      this.panel1.Location = new System.Drawing.Point(71, 35);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(106, 16);
      this.panel1.TabIndex = 3;
      // 
      // xAxisPrimaryMinimumFixedRadioButton
      // 
      this.xAxisMinimumFixedRadioButton.AutoSize = true;
      this.xAxisMinimumFixedRadioButton.Location = new System.Drawing.Point(53, 1);
      this.xAxisMinimumFixedRadioButton.Name = "xAxisPrimaryMinimumFixedRadioButton";
      this.xAxisMinimumFixedRadioButton.Size = new System.Drawing.Size(53, 17);
      this.xAxisMinimumFixedRadioButton.TabIndex = 1;
      this.xAxisMinimumFixedRadioButton.TabStop = true;
      this.xAxisMinimumFixedRadioButton.Text = "&Fixed:";
      this.xAxisMinimumFixedRadioButton.UseVisualStyleBackColor = true;
      this.xAxisMinimumFixedRadioButton.CheckedChanged += new System.EventHandler(this.xAxisMinimumRadioButton_CheckedChanged);
      // 
      // xAxisPrimaryMinimumAutoRadioButton
      // 
      this.xAxisMinimumAutoRadioButton.AutoSize = true;
      this.xAxisMinimumAutoRadioButton.Location = new System.Drawing.Point(0, 1);
      this.xAxisMinimumAutoRadioButton.Name = "xAxisPrimaryMinimumAutoRadioButton";
      this.xAxisMinimumAutoRadioButton.Size = new System.Drawing.Size(47, 17);
      this.xAxisMinimumAutoRadioButton.TabIndex = 0;
      this.xAxisMinimumAutoRadioButton.TabStop = true;
      this.xAxisMinimumAutoRadioButton.Text = "&Auto";
      this.xAxisMinimumAutoRadioButton.UseVisualStyleBackColor = true;
      this.xAxisMinimumAutoRadioButton.CheckedChanged += new System.EventHandler(this.xAxisMinimumRadioButton_CheckedChanged);
      // 
      // xAxisPrimaryMinimumFixedTextBox
      // 
      this.xAxisMinimumFixedTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.xAxisMinimumFixedTextBox.Location = new System.Drawing.Point(196, 35);
      this.xAxisMinimumFixedTextBox.Name = "xAxisPrimaryMinimumFixedTextBox";
      this.xAxisMinimumFixedTextBox.Size = new System.Drawing.Size(245, 20);
      this.xAxisMinimumFixedTextBox.TabIndex = 4;
      this.xAxisMinimumFixedTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.xAxisMinimumFixedTextBox_Validating);
      // 
      // xAxisPrimaryMaximumFixedTextBox
      // 
      this.xAxisMaximumFixedTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.xAxisMaximumFixedTextBox.Location = new System.Drawing.Point(196, 61);
      this.xAxisMaximumFixedTextBox.Name = "xAxisPrimaryMaximumFixedTextBox";
      this.xAxisMaximumFixedTextBox.Size = new System.Drawing.Size(245, 20);
      this.xAxisMaximumFixedTextBox.TabIndex = 7;
      this.xAxisMaximumFixedTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.xAxisMaximumFixedTextBox_Validating);
      // 
      // label12
      // 
      this.label12.AutoSize = true;
      this.label12.Location = new System.Drawing.Point(6, 64);
      this.label12.Name = "label12";
      this.label12.Size = new System.Drawing.Size(54, 13);
      this.label12.TabIndex = 5;
      this.label12.Text = "&Maximum:";
      // 
      // label9
      // 
      this.label9.AutoSize = true;
      this.label9.Location = new System.Drawing.Point(6, 12);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(30, 13);
      this.label9.TabIndex = 0;
      this.label9.Text = "&Title:";
      // 
      // xAxisPrimaryTitleTextBox
      // 
      this.xAxisTitleTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.xAxisTitleTextBox.Location = new System.Drawing.Point(71, 9);
      this.xAxisTitleTextBox.Name = "xAxisPrimaryTitleTextBox";
      this.xAxisTitleTextBox.Size = new System.Drawing.Size(370, 20);
      this.xAxisTitleTextBox.TabIndex = 1;
      this.xAxisTitleTextBox.Validated += new System.EventHandler(this.xTitleTextBox_Validated);
      // 
      // label11
      // 
      this.label11.AutoSize = true;
      this.label11.Location = new System.Drawing.Point(6, 38);
      this.label11.Name = "label11";
      this.label11.Size = new System.Drawing.Size(51, 13);
      this.label11.TabIndex = 2;
      this.label11.Text = "&Minimum:";
      // 
      // axisTabControl
      // 
      this.axisTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.axisTabControl.Controls.Add(this.xAxisTabPage);
      this.axisTabControl.Controls.Add(this.yAxisTabPage);
      this.axisTabControl.Location = new System.Drawing.Point(0, 84);
      this.axisTabControl.Name = "axisTabControl";
      this.axisTabControl.SelectedIndex = 0;
      this.axisTabControl.Size = new System.Drawing.Size(455, 134);
      this.axisTabControl.TabIndex = 8;
      // 
      // xAxisTabPage
      // 
      this.xAxisTabPage.Controls.Add(this.xAxisGridCheckBox);
      this.xAxisTabPage.Controls.Add(this.label4);
      this.xAxisTabPage.Controls.Add(this.panel2);
      this.xAxisTabPage.Controls.Add(this.panel1);
      this.xAxisTabPage.Controls.Add(this.xAxisTitleTextBox);
      this.xAxisTabPage.Controls.Add(this.xAxisMinimumFixedTextBox);
      this.xAxisTabPage.Controls.Add(this.label11);
      this.xAxisTabPage.Controls.Add(this.xAxisMaximumFixedTextBox);
      this.xAxisTabPage.Controls.Add(this.label9);
      this.xAxisTabPage.Controls.Add(this.label12);
      this.xAxisTabPage.Location = new System.Drawing.Point(4, 22);
      this.xAxisTabPage.Name = "xAxisTabPage";
      this.xAxisTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.xAxisTabPage.Size = new System.Drawing.Size(447, 108);
      this.xAxisTabPage.TabIndex = 0;
      this.xAxisTabPage.Text = "X-Axis";
      this.xAxisTabPage.UseVisualStyleBackColor = true;
      // 
      // xAxisGridCheckBox
      // 
      this.xAxisGridCheckBox.AutoSize = true;
      this.xAxisGridCheckBox.Location = new System.Drawing.Point(71, 85);
      this.xAxisGridCheckBox.Name = "xAxisGridCheckBox";
      this.xAxisGridCheckBox.Size = new System.Drawing.Size(15, 14);
      this.xAxisGridCheckBox.TabIndex = 9;
      this.xAxisGridCheckBox.UseVisualStyleBackColor = true;
      this.xAxisGridCheckBox.CheckedChanged += new System.EventHandler(this.xAxisGridCheckBox_CheckedChanged);
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(6, 85);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(59, 13);
      this.label4.TabIndex = 8;
      this.label4.Text = "Show &Grid:";
      // 
      // yAxisTabPage
      // 
      this.yAxisTabPage.Controls.Add(this.yAxisGridCheckBox);
      this.yAxisTabPage.Controls.Add(this.label5);
      this.yAxisTabPage.Controls.Add(this.panel6);
      this.yAxisTabPage.Controls.Add(this.panel5);
      this.yAxisTabPage.Controls.Add(this.yAxisTitleTextBox);
      this.yAxisTabPage.Controls.Add(this.yAxisMinimumFixedTextBox);
      this.yAxisTabPage.Controls.Add(this.label1);
      this.yAxisTabPage.Controls.Add(this.yAxisMaximumFixedTextBox);
      this.yAxisTabPage.Controls.Add(this.label8);
      this.yAxisTabPage.Controls.Add(this.label7);
      this.yAxisTabPage.Location = new System.Drawing.Point(4, 22);
      this.yAxisTabPage.Name = "yAxisTabPage";
      this.yAxisTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.yAxisTabPage.Size = new System.Drawing.Size(447, 108);
      this.yAxisTabPage.TabIndex = 1;
      this.yAxisTabPage.Text = "Y-Axis";
      this.yAxisTabPage.UseVisualStyleBackColor = true;
      // 
      // yAxisGridCheckBox
      // 
      this.yAxisGridCheckBox.AutoSize = true;
      this.yAxisGridCheckBox.Location = new System.Drawing.Point(71, 85);
      this.yAxisGridCheckBox.Name = "yAxisGridCheckBox";
      this.yAxisGridCheckBox.Size = new System.Drawing.Size(15, 14);
      this.yAxisGridCheckBox.TabIndex = 10;
      this.yAxisGridCheckBox.UseVisualStyleBackColor = true;
      this.yAxisGridCheckBox.CheckedChanged += new System.EventHandler(this.yAxisGridCheckBox_CheckedChanged);
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(6, 85);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(59, 13);
      this.label5.TabIndex = 9;
      this.label5.Text = "Show &Grid:";
      // 
      // panel6
      // 
      this.panel6.Controls.Add(this.yAxisMaximumFixedRadioButton);
      this.panel6.Controls.Add(this.yAxisMaximumAutoRadioButton);
      this.panel6.Location = new System.Drawing.Point(71, 61);
      this.panel6.Name = "panel6";
      this.panel6.Size = new System.Drawing.Size(106, 20);
      this.panel6.TabIndex = 6;
      // 
      // yAxisPrimaryMaximumFixedRadioButton
      // 
      this.yAxisMaximumFixedRadioButton.AutoSize = true;
      this.yAxisMaximumFixedRadioButton.Location = new System.Drawing.Point(53, 1);
      this.yAxisMaximumFixedRadioButton.Name = "yAxisPrimaryMaximumFixedRadioButton";
      this.yAxisMaximumFixedRadioButton.Size = new System.Drawing.Size(53, 17);
      this.yAxisMaximumFixedRadioButton.TabIndex = 1;
      this.yAxisMaximumFixedRadioButton.TabStop = true;
      this.yAxisMaximumFixedRadioButton.Text = "&Fixed:";
      this.yAxisMaximumFixedRadioButton.UseVisualStyleBackColor = true;
      this.yAxisMaximumFixedRadioButton.CheckedChanged += new System.EventHandler(this.yAxisMaximumRadioButton_CheckedChanged);
      // 
      // yAxisPrimaryMaximumAutoRadioButton
      // 
      this.yAxisMaximumAutoRadioButton.AutoSize = true;
      this.yAxisMaximumAutoRadioButton.Location = new System.Drawing.Point(0, 1);
      this.yAxisMaximumAutoRadioButton.Name = "yAxisPrimaryMaximumAutoRadioButton";
      this.yAxisMaximumAutoRadioButton.Size = new System.Drawing.Size(47, 17);
      this.yAxisMaximumAutoRadioButton.TabIndex = 0;
      this.yAxisMaximumAutoRadioButton.TabStop = true;
      this.yAxisMaximumAutoRadioButton.Text = "&Auto";
      this.yAxisMaximumAutoRadioButton.UseVisualStyleBackColor = true;
      this.yAxisMaximumAutoRadioButton.CheckedChanged += new System.EventHandler(this.yAxisMaximumRadioButton_CheckedChanged);
      // 
      // panel5
      // 
      this.panel5.Controls.Add(this.yAxisMinimumAutoRadioButton);
      this.panel5.Controls.Add(this.yAxisMinimumFixedRadioButton);
      this.panel5.Location = new System.Drawing.Point(71, 35);
      this.panel5.Name = "panel5";
      this.panel5.Size = new System.Drawing.Size(106, 20);
      this.panel5.TabIndex = 3;
      // 
      // yAxisPrimaryMinimumAutoRadioButton
      // 
      this.yAxisMinimumAutoRadioButton.AutoSize = true;
      this.yAxisMinimumAutoRadioButton.Location = new System.Drawing.Point(0, 1);
      this.yAxisMinimumAutoRadioButton.Name = "yAxisPrimaryMinimumAutoRadioButton";
      this.yAxisMinimumAutoRadioButton.Size = new System.Drawing.Size(47, 17);
      this.yAxisMinimumAutoRadioButton.TabIndex = 0;
      this.yAxisMinimumAutoRadioButton.TabStop = true;
      this.yAxisMinimumAutoRadioButton.Text = "&Auto";
      this.yAxisMinimumAutoRadioButton.UseVisualStyleBackColor = true;
      this.yAxisMinimumAutoRadioButton.CheckedChanged += new System.EventHandler(this.yAxisMinimumRadioButton_CheckedChanged);
      // 
      // yAxisPrimaryMinimumFixedRadioButton
      // 
      this.yAxisMinimumFixedRadioButton.AutoSize = true;
      this.yAxisMinimumFixedRadioButton.Location = new System.Drawing.Point(53, 1);
      this.yAxisMinimumFixedRadioButton.Name = "yAxisPrimaryMinimumFixedRadioButton";
      this.yAxisMinimumFixedRadioButton.Size = new System.Drawing.Size(53, 17);
      this.yAxisMinimumFixedRadioButton.TabIndex = 1;
      this.yAxisMinimumFixedRadioButton.TabStop = true;
      this.yAxisMinimumFixedRadioButton.Text = "&Fixed:";
      this.yAxisMinimumFixedRadioButton.UseVisualStyleBackColor = true;
      this.yAxisMinimumFixedRadioButton.CheckedChanged += new System.EventHandler(this.yAxisMinimumRadioButton_CheckedChanged);
      // 
      // yAxisPrimaryMinimumFixedTextBox
      // 
      this.yAxisMinimumFixedTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.yAxisMinimumFixedTextBox.Location = new System.Drawing.Point(196, 35);
      this.yAxisMinimumFixedTextBox.Name = "yAxisPrimaryMinimumFixedTextBox";
      this.yAxisMinimumFixedTextBox.Size = new System.Drawing.Size(245, 20);
      this.yAxisMinimumFixedTextBox.TabIndex = 4;
      this.yAxisMinimumFixedTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.yAxisMinimumFixedTextBox_Validating);
      // 
      // yAxisPrimaryMaximumFixedTextBox
      // 
      this.yAxisMaximumFixedTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.yAxisMaximumFixedTextBox.Location = new System.Drawing.Point(196, 61);
      this.yAxisMaximumFixedTextBox.Name = "yAxisPrimaryMaximumFixedTextBox";
      this.yAxisMaximumFixedTextBox.Size = new System.Drawing.Size(245, 20);
      this.yAxisMaximumFixedTextBox.TabIndex = 7;
      this.yAxisMaximumFixedTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.yAxisMaximumFixedTextBox_Validating);
      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.Location = new System.Drawing.Point(6, 38);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(51, 13);
      this.label8.TabIndex = 2;
      this.label8.Text = "&Minimum:";
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(6, 64);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(54, 13);
      this.label7.TabIndex = 5;
      this.label7.Text = "&Maximum:";
      // 
      // titleTextBox
      // 
      this.titleTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.titleTextBox.Location = new System.Drawing.Point(66, 0);
      this.titleTextBox.Name = "titleTextBox";
      this.titleTextBox.Size = new System.Drawing.Size(389, 20);
      this.titleTextBox.TabIndex = 1;
      this.titleTextBox.Validated += new System.EventHandler(this.titleTextBox_Validated);
      // 
      // label15
      // 
      this.label15.AutoSize = true;
      this.label15.Location = new System.Drawing.Point(3, 3);
      this.label15.Name = "label15";
      this.label15.Size = new System.Drawing.Size(30, 13);
      this.label15.TabIndex = 0;
      this.label15.Text = "&Title:";
      // 
      // axisFontLabel
      // 
      this.axisFontLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.axisFontLabel.AutoSize = true;
      this.axisFontLabel.Location = new System.Drawing.Point(98, 60);
      this.axisFontLabel.Name = "axisFontLabel";
      this.axisFontLabel.Size = new System.Drawing.Size(43, 13);
      this.axisFontLabel.TabIndex = 7;
      this.axisFontLabel.Text = "( none )";
      // 
      // titleFontLabel
      // 
      this.titleFontLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.titleFontLabel.AutoSize = true;
      this.titleFontLabel.Location = new System.Drawing.Point(98, 31);
      this.titleFontLabel.Name = "titleFontLabel";
      this.titleFontLabel.Size = new System.Drawing.Size(43, 13);
      this.titleFontLabel.TabIndex = 4;
      this.titleFontLabel.Text = "( none )";
      // 
      // axisFontButton
      // 
      this.axisFontButton.Location = new System.Drawing.Point(66, 55);
      this.axisFontButton.Name = "axisFontButton";
      this.axisFontButton.Size = new System.Drawing.Size(26, 23);
      this.axisFontButton.TabIndex = 6;
      this.axisFontButton.Text = "...";
      this.axisFontButton.UseVisualStyleBackColor = true;
      this.axisFontButton.Click += new System.EventHandler(this.axisFontButton_Click);
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(3, 60);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(57, 13);
      this.label3.TabIndex = 5;
      this.label3.Text = "&Axes Font:";
      // 
      // titleFontButton
      // 
      this.titleFontButton.Location = new System.Drawing.Point(66, 26);
      this.titleFontButton.Name = "titleFontButton";
      this.titleFontButton.Size = new System.Drawing.Size(26, 23);
      this.titleFontButton.TabIndex = 3;
      this.titleFontButton.Text = "...";
      this.titleFontButton.UseVisualStyleBackColor = true;
      this.titleFontButton.Click += new System.EventHandler(this.titleFontButton_Click);
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(3, 31);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(54, 13);
      this.label2.TabIndex = 2;
      this.label2.Text = "&Title Font:";
      // 
      // errorProvider
      // 
      this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
      this.errorProvider.ContainerControl = this;
      // 
      // titleFontDialog
      // 
      this.titleFontDialog.Color = System.Drawing.SystemColors.ControlText;
      this.titleFontDialog.FontMustExist = true;
      this.titleFontDialog.ShowColor = true;
      // 
      // axisFontDialog
      // 
      this.axisFontDialog.Color = System.Drawing.SystemColors.ControlText;
      // 
      // ScatterPlotVisualPropertiesControl
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.axisFontLabel);
      this.Controls.Add(this.label15);
      this.Controls.Add(this.axisFontButton);
      this.Controls.Add(this.titleFontLabel);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.titleTextBox);
      this.Controls.Add(this.axisTabControl);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.titleFontButton);
      this.Name = "ScatterPlotVisualPropertiesControl";
      this.Size = new System.Drawing.Size(455, 220);
      this.panel2.ResumeLayout(false);
      this.panel2.PerformLayout();
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.axisTabControl.ResumeLayout(false);
      this.xAxisTabPage.ResumeLayout(false);
      this.xAxisTabPage.PerformLayout();
      this.yAxisTabPage.ResumeLayout(false);
      this.yAxisTabPage.PerformLayout();
      this.panel6.ResumeLayout(false);
      this.panel6.PerformLayout();
      this.panel5.ResumeLayout(false);
      this.panel5.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox yAxisTitleTextBox;
    private System.Windows.Forms.Label label12;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.TextBox xAxisTitleTextBox;
    private System.Windows.Forms.Label label11;
    private System.Windows.Forms.TextBox xAxisMaximumFixedTextBox;
    private System.Windows.Forms.RadioButton xAxisMaximumFixedRadioButton;
    private System.Windows.Forms.RadioButton xAxisMaximumAutoRadioButton;
    private System.Windows.Forms.TextBox xAxisMinimumFixedTextBox;
    private System.Windows.Forms.RadioButton xAxisMinimumFixedRadioButton;
    private System.Windows.Forms.RadioButton xAxisMinimumAutoRadioButton;
    private System.Windows.Forms.TabControl axisTabControl;
    private System.Windows.Forms.TabPage xAxisTabPage;
    private System.Windows.Forms.TabPage yAxisTabPage;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.TextBox yAxisMaximumFixedTextBox;
    private System.Windows.Forms.RadioButton yAxisMaximumFixedRadioButton;
    private System.Windows.Forms.RadioButton yAxisMaximumAutoRadioButton;
    private System.Windows.Forms.TextBox yAxisMinimumFixedTextBox;
    private System.Windows.Forms.RadioButton yAxisMinimumFixedRadioButton;
    private System.Windows.Forms.RadioButton yAxisMinimumAutoRadioButton;
    private System.Windows.Forms.ErrorProvider errorProvider;
    private System.Windows.Forms.FontDialog titleFontDialog;
    private System.Windows.Forms.Button titleFontButton;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Button axisFontButton;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label axisFontLabel;
    private System.Windows.Forms.Label titleFontLabel;
    private System.Windows.Forms.FontDialog axisFontDialog;
    private System.Windows.Forms.Label label15;
    private System.Windows.Forms.TextBox titleTextBox;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Panel panel2;
    private System.Windows.Forms.Panel panel6;
    private System.Windows.Forms.Panel panel5;
    private System.Windows.Forms.CheckBox xAxisGridCheckBox;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.CheckBox yAxisGridCheckBox;
    private System.Windows.Forms.Label label5;
  }
}
