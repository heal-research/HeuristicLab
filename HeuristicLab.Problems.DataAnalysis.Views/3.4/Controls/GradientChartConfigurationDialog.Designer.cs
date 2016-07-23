#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.DataAnalysis.Views {
  partial class GradientChartConfigurationDialog {
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

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      this.xAxisGroupBox = new System.Windows.Forms.GroupBox();
      this.maxXTextBox = new System.Windows.Forms.TextBox();
      this.xAutomaticCheckBox = new System.Windows.Forms.CheckBox();
      this.minXTextBox = new System.Windows.Forms.TextBox();
      this.maxXLabel = new System.Windows.Forms.Label();
      this.minXLabel = new System.Windows.Forms.Label();
      this.yAxisGroupBox = new System.Windows.Forms.GroupBox();
      this.maxYTextBox = new System.Windows.Forms.TextBox();
      this.yAutomaticCheckBox = new System.Windows.Forms.CheckBox();
      this.minYTextBox = new System.Windows.Forms.TextBox();
      this.maxYLabel = new System.Windows.Forms.Label();
      this.minYLabel = new System.Windows.Forms.Label();
      this.miscGroupBox = new System.Windows.Forms.GroupBox();
      this.StepsNumericUpDown = new System.Windows.Forms.NumericUpDown();
      this.stepsLabel = new System.Windows.Forms.Label();
      this.applyButton = new System.Windows.Forms.Button();
      this.cancelButton = new System.Windows.Forms.Button();
      this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
      this.xAxisGroupBox.SuspendLayout();
      this.yAxisGroupBox.SuspendLayout();
      this.miscGroupBox.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.StepsNumericUpDown)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // xAxisGroupBox
      // 
      this.xAxisGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.xAxisGroupBox.Controls.Add(this.maxXTextBox);
      this.xAxisGroupBox.Controls.Add(this.xAutomaticCheckBox);
      this.xAxisGroupBox.Controls.Add(this.minXTextBox);
      this.xAxisGroupBox.Controls.Add(this.maxXLabel);
      this.xAxisGroupBox.Controls.Add(this.minXLabel);
      this.xAxisGroupBox.Location = new System.Drawing.Point(12, 12);
      this.xAxisGroupBox.Name = "xAxisGroupBox";
      this.xAxisGroupBox.Size = new System.Drawing.Size(234, 78);
      this.xAxisGroupBox.TabIndex = 1;
      this.xAxisGroupBox.TabStop = false;
      this.xAxisGroupBox.Text = "X-Axis";
      // 
      // maxXTextBox
      // 
      this.maxXTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.maxXTextBox.Location = new System.Drawing.Point(61, 45);
      this.maxXTextBox.Name = "maxXTextBox";
      this.maxXTextBox.Size = new System.Drawing.Size(167, 20);
      this.maxXTextBox.TabIndex = 3;
      this.maxXTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.numberTextBox_Validating);
      this.maxXTextBox.Validated += new System.EventHandler(this.numberTextBox_Validated);
      // 
      // xAutomaticCheckBox
      // 
      this.xAutomaticCheckBox.AutoSize = true;
      this.xAutomaticCheckBox.Location = new System.Drawing.Point(48, -1);
      this.xAutomaticCheckBox.Name = "xAutomaticCheckBox";
      this.xAutomaticCheckBox.Size = new System.Drawing.Size(73, 17);
      this.xAutomaticCheckBox.TabIndex = 1;
      this.xAutomaticCheckBox.Text = "Automatic";
      this.xAutomaticCheckBox.UseVisualStyleBackColor = true;
      this.xAutomaticCheckBox.CheckedChanged += new System.EventHandler(this.automaticCheckBox_CheckedChanged);
      // 
      // minXTextBox
      // 
      this.minXTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.minXTextBox.Location = new System.Drawing.Point(61, 19);
      this.minXTextBox.Name = "minXTextBox";
      this.minXTextBox.Size = new System.Drawing.Size(167, 20);
      this.minXTextBox.TabIndex = 2;
      this.minXTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.numberTextBox_Validating);
      this.minXTextBox.Validated += new System.EventHandler(this.numberTextBox_Validated);
      // 
      // maxXLabel
      // 
      this.maxXLabel.AutoSize = true;
      this.maxXLabel.Location = new System.Drawing.Point(7, 48);
      this.maxXLabel.Name = "maxXLabel";
      this.maxXLabel.Size = new System.Drawing.Size(27, 13);
      this.maxXLabel.TabIndex = 1;
      this.maxXLabel.Text = "Max";
      // 
      // minXLabel
      // 
      this.minXLabel.AutoSize = true;
      this.minXLabel.Location = new System.Drawing.Point(7, 22);
      this.minXLabel.Name = "minXLabel";
      this.minXLabel.Size = new System.Drawing.Size(24, 13);
      this.minXLabel.TabIndex = 0;
      this.minXLabel.Text = "Min";
      // 
      // yAxisGroupBox
      // 
      this.yAxisGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.yAxisGroupBox.Controls.Add(this.maxYTextBox);
      this.yAxisGroupBox.Controls.Add(this.yAutomaticCheckBox);
      this.yAxisGroupBox.Controls.Add(this.minYTextBox);
      this.yAxisGroupBox.Controls.Add(this.maxYLabel);
      this.yAxisGroupBox.Controls.Add(this.minYLabel);
      this.yAxisGroupBox.Location = new System.Drawing.Point(13, 96);
      this.yAxisGroupBox.Name = "yAxisGroupBox";
      this.yAxisGroupBox.Size = new System.Drawing.Size(234, 75);
      this.yAxisGroupBox.TabIndex = 2;
      this.yAxisGroupBox.TabStop = false;
      this.yAxisGroupBox.Text = "Y-Axis";
      // 
      // maxYTextBox
      // 
      this.maxYTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.maxYTextBox.Location = new System.Drawing.Point(61, 45);
      this.maxYTextBox.Name = "maxYTextBox";
      this.maxYTextBox.Size = new System.Drawing.Size(167, 20);
      this.maxYTextBox.TabIndex = 3;
      this.maxYTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.numberTextBox_Validating);
      this.maxYTextBox.Validated += new System.EventHandler(this.numberTextBox_Validated);
      // 
      // yAutomaticCheckBox
      // 
      this.yAutomaticCheckBox.AutoSize = true;
      this.yAutomaticCheckBox.Location = new System.Drawing.Point(47, -1);
      this.yAutomaticCheckBox.Name = "yAutomaticCheckBox";
      this.yAutomaticCheckBox.Size = new System.Drawing.Size(73, 17);
      this.yAutomaticCheckBox.TabIndex = 1;
      this.yAutomaticCheckBox.Text = "Automatic";
      this.yAutomaticCheckBox.UseVisualStyleBackColor = true;
      this.yAutomaticCheckBox.CheckedChanged += new System.EventHandler(this.automaticCheckBox_CheckedChanged);
      // 
      // minYTextBox
      // 
      this.minYTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.minYTextBox.Location = new System.Drawing.Point(61, 19);
      this.minYTextBox.Name = "minYTextBox";
      this.minYTextBox.Size = new System.Drawing.Size(167, 20);
      this.minYTextBox.TabIndex = 2;
      this.minYTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.numberTextBox_Validating);
      this.minYTextBox.Validated += new System.EventHandler(this.numberTextBox_Validated);
      // 
      // maxYLabel
      // 
      this.maxYLabel.AutoSize = true;
      this.maxYLabel.Location = new System.Drawing.Point(7, 48);
      this.maxYLabel.Name = "maxYLabel";
      this.maxYLabel.Size = new System.Drawing.Size(27, 13);
      this.maxYLabel.TabIndex = 1;
      this.maxYLabel.Text = "Max";
      // 
      // minYLabel
      // 
      this.minYLabel.AutoSize = true;
      this.minYLabel.Location = new System.Drawing.Point(7, 22);
      this.minYLabel.Name = "minYLabel";
      this.minYLabel.Size = new System.Drawing.Size(24, 13);
      this.minYLabel.TabIndex = 0;
      this.minYLabel.Text = "Min";
      // 
      // miscGroupBox
      // 
      this.miscGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.miscGroupBox.Controls.Add(this.StepsNumericUpDown);
      this.miscGroupBox.Controls.Add(this.stepsLabel);
      this.miscGroupBox.Location = new System.Drawing.Point(13, 177);
      this.miscGroupBox.Name = "miscGroupBox";
      this.miscGroupBox.Size = new System.Drawing.Size(233, 53);
      this.miscGroupBox.TabIndex = 4;
      this.miscGroupBox.TabStop = false;
      // 
      // StepsNumericUpDown
      // 
      this.StepsNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.StepsNumericUpDown.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
      this.StepsNumericUpDown.Location = new System.Drawing.Point(61, 20);
      this.StepsNumericUpDown.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
      this.StepsNumericUpDown.Name = "StepsNumericUpDown";
      this.StepsNumericUpDown.Size = new System.Drawing.Size(166, 20);
      this.StepsNumericUpDown.TabIndex = 4;
      // 
      // stepsLabel
      // 
      this.stepsLabel.AutoSize = true;
      this.stepsLabel.Location = new System.Drawing.Point(6, 22);
      this.stepsLabel.Name = "stepsLabel";
      this.stepsLabel.Size = new System.Drawing.Size(34, 13);
      this.stepsLabel.TabIndex = 1;
      this.stepsLabel.Text = "Steps";
      // 
      // applyButton
      // 
      this.applyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.applyButton.Location = new System.Drawing.Point(91, 239);
      this.applyButton.Name = "applyButton";
      this.applyButton.Size = new System.Drawing.Size(75, 23);
      this.applyButton.TabIndex = 5;
      this.applyButton.Text = "Apply";
      this.applyButton.UseVisualStyleBackColor = true;
      this.applyButton.Click += new System.EventHandler(this.okButton_Click);
      // 
      // cancelButton
      // 
      this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.cancelButton.Location = new System.Drawing.Point(172, 239);
      this.cancelButton.Name = "cancelButton";
      this.cancelButton.Size = new System.Drawing.Size(75, 23);
      this.cancelButton.TabIndex = 6;
      this.cancelButton.Text = "Cancel";
      this.cancelButton.UseVisualStyleBackColor = true;
      // 
      // errorProvider
      // 
      this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
      this.errorProvider.ContainerControl = this;
      // 
      // GradientChartConfigurationDialog
      // 
      this.AcceptButton = this.applyButton;
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.CancelButton = this.cancelButton;
      this.ClientSize = new System.Drawing.Size(258, 272);
      this.Controls.Add(this.cancelButton);
      this.Controls.Add(this.applyButton);
      this.Controls.Add(this.miscGroupBox);
      this.Controls.Add(this.xAxisGroupBox);
      this.Controls.Add(this.yAxisGroupBox);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "GradientChartConfigurationDialog";
      this.ShowIcon = false;
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
      this.Text = "Configure Gradient Chart";
      this.TopMost = true;
      this.Shown += new System.EventHandler(this.GradientChartConfigurationDialog_Shown);
      this.xAxisGroupBox.ResumeLayout(false);
      this.xAxisGroupBox.PerformLayout();
      this.yAxisGroupBox.ResumeLayout(false);
      this.yAxisGroupBox.PerformLayout();
      this.miscGroupBox.ResumeLayout(false);
      this.miscGroupBox.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.StepsNumericUpDown)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox xAxisGroupBox;
    private System.Windows.Forms.TextBox maxXTextBox;
    private System.Windows.Forms.TextBox minXTextBox;
    private System.Windows.Forms.Label maxXLabel;
    private System.Windows.Forms.Label minXLabel;
    private System.Windows.Forms.GroupBox yAxisGroupBox;
    private System.Windows.Forms.TextBox maxYTextBox;
    private System.Windows.Forms.CheckBox yAutomaticCheckBox;
    private System.Windows.Forms.TextBox minYTextBox;
    private System.Windows.Forms.Label maxYLabel;
    private System.Windows.Forms.Label minYLabel;
    private System.Windows.Forms.GroupBox miscGroupBox;
    private System.Windows.Forms.Label stepsLabel;
    private System.Windows.Forms.Button applyButton;
    private System.Windows.Forms.Button cancelButton;
    private System.Windows.Forms.CheckBox xAutomaticCheckBox;
    private System.Windows.Forms.NumericUpDown StepsNumericUpDown;
    private System.Windows.Forms.ErrorProvider errorProvider;
  }
}