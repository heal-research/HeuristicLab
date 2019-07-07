#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Algorithms.DataAnalysis.Views {
  partial class DensityTrackbar {
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
      this.radioButton = new System.Windows.Forms.RadioButton();
      this.trackBar = new System.Windows.Forms.TrackBar();
      this.chart = new HeuristicLab.Problems.DataAnalysis.Views.DensityChart();
      this.textBox = new System.Windows.Forms.TextBox();
      this.groupBox = new System.Windows.Forms.GroupBox();
      this.doubleLimitView = new HeuristicLab.Problems.DataAnalysis.Views.DoubleLimitView();
      this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.trackBar)).BeginInit();
      this.groupBox.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // radioButton
      // 
      this.radioButton.Location = new System.Drawing.Point(6, 11);
      this.radioButton.Name = "radioButton";
      this.radioButton.Size = new System.Drawing.Size(132, 17);
      this.radioButton.TabIndex = 0;
      this.radioButton.TabStop = true;
      this.radioButton.Text = "<Name>";
      this.radioButton.UseVisualStyleBackColor = true;
      this.radioButton.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
      // 
      // trackBar
      // 
      this.trackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.trackBar.BackColor = System.Drawing.Color.White;
      this.trackBar.LargeChange = 100;
      this.trackBar.Location = new System.Drawing.Point(144, 10);
      this.trackBar.Maximum = 1000;
      this.trackBar.Name = "trackBar";
      this.trackBar.RightToLeft = System.Windows.Forms.RightToLeft.No;
      this.trackBar.Size = new System.Drawing.Size(371, 45);
      this.trackBar.TabIndex = 1;
      this.trackBar.TickFrequency = 100;
      this.trackBar.Value = 500;
      this.trackBar.ValueChanged += new System.EventHandler(this.trackBar_ValueChanged);
      // 
      // chart
      // 
      this.chart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.chart.Location = new System.Drawing.Point(156, 35);
      this.chart.Name = "chart";
      this.chart.Size = new System.Drawing.Size(345, 20);
      this.chart.TabIndex = 3;
      // 
      // textBox
      // 
      this.textBox.Location = new System.Drawing.Point(6, 35);
      this.textBox.Name = "textBox";
      this.textBox.Size = new System.Drawing.Size(132, 20);
      this.textBox.TabIndex = 4;
      this.textBox.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_Validating);
      this.textBox.Validated += new System.EventHandler(this.textBox_Validated);
      // 
      // groupBox
      // 
      this.groupBox.Controls.Add(this.radioButton);
      this.groupBox.Controls.Add(this.doubleLimitView);
      this.groupBox.Controls.Add(this.chart);
      this.groupBox.Controls.Add(this.trackBar);
      this.groupBox.Controls.Add(this.textBox);
      this.groupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox.Location = new System.Drawing.Point(0, 0);
      this.groupBox.Name = "groupBox";
      this.groupBox.Size = new System.Drawing.Size(678, 62);
      this.groupBox.TabIndex = 5;
      this.groupBox.TabStop = false;
      // 
      // doubleLimitView
      // 
      this.doubleLimitView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.doubleLimitView.Caption = "DoubleLimit View";
      this.doubleLimitView.Content = null;
      this.doubleLimitView.Location = new System.Drawing.Point(521, 10);
      this.doubleLimitView.Name = "doubleLimitView";
      this.doubleLimitView.ReadOnly = false;
      this.doubleLimitView.Size = new System.Drawing.Size(151, 47);
      this.doubleLimitView.TabIndex = 2;
      // 
      // errorProvider
      // 
      this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
      this.errorProvider.ContainerControl = this;
      // 
      // DensityTrackbar
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.groupBox);
      this.Name = "DensityTrackbar";
      this.Size = new System.Drawing.Size(678, 62);
      ((System.ComponentModel.ISupportInitialize)(this.trackBar)).EndInit();
      this.groupBox.ResumeLayout(false);
      this.groupBox.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.RadioButton radioButton;
    private System.Windows.Forms.TrackBar trackBar;
    private Problems.DataAnalysis.Views.DoubleLimitView doubleLimitView;
    private HeuristicLab.Problems.DataAnalysis.Views.DensityChart chart;
    private System.Windows.Forms.TextBox textBox;
    private System.Windows.Forms.GroupBox groupBox;
    private System.Windows.Forms.ErrorProvider errorProvider;
  }
}
