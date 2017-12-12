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

namespace HeuristicLab.DataPreprocessing.Views {
  partial class ScatterPlotSingleView {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScatterPlotSingleView));
      this.scatterPlotView = new HeuristicLab.Analysis.Views.ScatterPlotView();
      this.variablesGroupBox = new System.Windows.Forms.GroupBox();
      this.orderComboBox = new System.Windows.Forms.ComboBox();
      this.legendOrderLabel = new System.Windows.Forms.Label();
      this.useGradientCheckBox = new System.Windows.Forms.CheckBox();
      this.groupLabel = new System.Windows.Forms.Label();
      this.yLabel = new System.Windows.Forms.Label();
      this.xLabel = new System.Windows.Forms.Label();
      this.comboBoxGroup = new System.Windows.Forms.ComboBox();
      this.comboBoxYVariable = new System.Windows.Forms.ComboBox();
      this.comboBoxXVariable = new System.Windows.Forms.ComboBox();
      this.regressionGroupBox = new System.Windows.Forms.GroupBox();
      this.regressionTypeComboBox = new System.Windows.Forms.ComboBox();
      this.polynomialRegressionOrderNumericUpDown = new System.Windows.Forms.NumericUpDown();
      this.orderLabel = new System.Windows.Forms.Label();
      this.regressionTypeLabel = new System.Windows.Forms.Label();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.gradientPanel = new System.Windows.Forms.Panel();
      this.gradientPictureBox = new System.Windows.Forms.PictureBox();
      this.gradientMinimumLabel = new System.Windows.Forms.Label();
      this.gradientMaximumLabel = new System.Windows.Forms.Label();
      this.variablesGroupBox.SuspendLayout();
      this.regressionGroupBox.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.polynomialRegressionOrderNumericUpDown)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.gradientPanel.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.gradientPictureBox)).BeginInit();
      this.SuspendLayout();
      // 
      // scatterPlotView
      // 
      this.scatterPlotView.Caption = "View";
      this.scatterPlotView.Content = null;
      this.scatterPlotView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.scatterPlotView.Location = new System.Drawing.Point(0, 0);
      this.scatterPlotView.Name = "scatterPlotView";
      this.scatterPlotView.ReadOnly = false;
      this.scatterPlotView.ShowName = false;
      this.scatterPlotView.Size = new System.Drawing.Size(618, 517);
      this.scatterPlotView.TabIndex = 0;
      // 
      // variablesGroupBox
      // 
      this.variablesGroupBox.Controls.Add(this.orderComboBox);
      this.variablesGroupBox.Controls.Add(this.legendOrderLabel);
      this.variablesGroupBox.Controls.Add(this.useGradientCheckBox);
      this.variablesGroupBox.Controls.Add(this.groupLabel);
      this.variablesGroupBox.Controls.Add(this.yLabel);
      this.variablesGroupBox.Controls.Add(this.xLabel);
      this.variablesGroupBox.Controls.Add(this.comboBoxGroup);
      this.variablesGroupBox.Controls.Add(this.comboBoxYVariable);
      this.variablesGroupBox.Controls.Add(this.comboBoxXVariable);
      this.variablesGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
      this.variablesGroupBox.Location = new System.Drawing.Point(0, 0);
      this.variablesGroupBox.Name = "variablesGroupBox";
      this.variablesGroupBox.Size = new System.Drawing.Size(172, 178);
      this.variablesGroupBox.TabIndex = 1;
      this.variablesGroupBox.TabStop = false;
      this.variablesGroupBox.Text = "Variables";
      // 
      // orderComboBox
      // 
      this.orderComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.orderComboBox.FormattingEnabled = true;
      this.orderComboBox.Location = new System.Drawing.Point(51, 144);
      this.orderComboBox.Name = "orderComboBox";
      this.orderComboBox.Size = new System.Drawing.Size(115, 21);
      this.orderComboBox.TabIndex = 6;
      this.orderComboBox.SelectedIndexChanged += new System.EventHandler(this.orderComboBox_SelectedIndexChanged);
      // 
      // legendOrderLabel
      // 
      this.legendOrderLabel.AutoSize = true;
      this.legendOrderLabel.Location = new System.Drawing.Point(6, 149);
      this.legendOrderLabel.Name = "legendOrderLabel";
      this.legendOrderLabel.Size = new System.Drawing.Size(36, 13);
      this.legendOrderLabel.TabIndex = 5;
      this.legendOrderLabel.Text = "Order:";
      this.legendOrderLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // useGradientCheckBox
      // 
      this.useGradientCheckBox.AutoSize = true;
      this.useGradientCheckBox.Location = new System.Drawing.Point(51, 121);
      this.useGradientCheckBox.Name = "useGradientCheckBox";
      this.useGradientCheckBox.Size = new System.Drawing.Size(115, 17);
      this.useGradientCheckBox.TabIndex = 4;
      this.useGradientCheckBox.Text = "Use Color Gradient";
      this.useGradientCheckBox.UseVisualStyleBackColor = true;
      this.useGradientCheckBox.CheckedChanged += new System.EventHandler(this.useGradientCheckBox_CheckedChanged);
      // 
      // groupLabel
      // 
      this.groupLabel.AutoSize = true;
      this.groupLabel.Location = new System.Drawing.Point(6, 97);
      this.groupLabel.Name = "groupLabel";
      this.groupLabel.Size = new System.Drawing.Size(39, 13);
      this.groupLabel.TabIndex = 3;
      this.groupLabel.Text = "Group:";
      // 
      // yLabel
      // 
      this.yLabel.AutoSize = true;
      this.yLabel.Location = new System.Drawing.Point(6, 63);
      this.yLabel.Name = "yLabel";
      this.yLabel.Size = new System.Drawing.Size(17, 13);
      this.yLabel.TabIndex = 3;
      this.yLabel.Text = "Y:";
      // 
      // xLabel
      // 
      this.xLabel.AutoSize = true;
      this.xLabel.Location = new System.Drawing.Point(6, 29);
      this.xLabel.Name = "xLabel";
      this.xLabel.Size = new System.Drawing.Size(17, 13);
      this.xLabel.TabIndex = 2;
      this.xLabel.Text = "X:";
      // 
      // comboBoxGroup
      // 
      this.comboBoxGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.comboBoxGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.comboBoxGroup.FormattingEnabled = true;
      this.comboBoxGroup.Location = new System.Drawing.Point(51, 94);
      this.comboBoxGroup.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
      this.comboBoxGroup.Name = "comboBoxGroup";
      this.comboBoxGroup.Size = new System.Drawing.Size(115, 21);
      this.comboBoxGroup.TabIndex = 1;
      this.comboBoxGroup.SelectedIndexChanged += new System.EventHandler(this.comboBoxGroup_SelectedIndexChanged);
      // 
      // comboBoxYVariable
      // 
      this.comboBoxYVariable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.comboBoxYVariable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.comboBoxYVariable.FormattingEnabled = true;
      this.comboBoxYVariable.Location = new System.Drawing.Point(51, 60);
      this.comboBoxYVariable.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
      this.comboBoxYVariable.Name = "comboBoxYVariable";
      this.comboBoxYVariable.Size = new System.Drawing.Size(115, 21);
      this.comboBoxYVariable.TabIndex = 1;
      this.comboBoxYVariable.SelectedIndexChanged += new System.EventHandler(this.comboBoxYVariable_SelectedIndexChanged);
      // 
      // comboBoxXVariable
      // 
      this.comboBoxXVariable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.comboBoxXVariable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.comboBoxXVariable.FormattingEnabled = true;
      this.comboBoxXVariable.Location = new System.Drawing.Point(51, 26);
      this.comboBoxXVariable.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
      this.comboBoxXVariable.Name = "comboBoxXVariable";
      this.comboBoxXVariable.Size = new System.Drawing.Size(115, 21);
      this.comboBoxXVariable.TabIndex = 0;
      this.comboBoxXVariable.SelectedIndexChanged += new System.EventHandler(this.comboBoxXVariable_SelectedIndexChanged);
      // 
      // regressionGroupBox
      // 
      this.regressionGroupBox.Controls.Add(this.regressionTypeComboBox);
      this.regressionGroupBox.Controls.Add(this.polynomialRegressionOrderNumericUpDown);
      this.regressionGroupBox.Controls.Add(this.orderLabel);
      this.regressionGroupBox.Controls.Add(this.regressionTypeLabel);
      this.regressionGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
      this.regressionGroupBox.Location = new System.Drawing.Point(0, 178);
      this.regressionGroupBox.Name = "regressionGroupBox";
      this.regressionGroupBox.Size = new System.Drawing.Size(172, 78);
      this.regressionGroupBox.TabIndex = 4;
      this.regressionGroupBox.TabStop = false;
      this.regressionGroupBox.Text = "Regression";
      // 
      // regressionTypeComboBox
      // 
      this.regressionTypeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.regressionTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.regressionTypeComboBox.FormattingEnabled = true;
      this.regressionTypeComboBox.Location = new System.Drawing.Point(51, 19);
      this.regressionTypeComboBox.Name = "regressionTypeComboBox";
      this.regressionTypeComboBox.Size = new System.Drawing.Size(115, 21);
      this.regressionTypeComboBox.TabIndex = 14;
      this.regressionTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.regressionTypeComboBox_SelectedIndexChanged);
      // 
      // polynomialRegressionOrderNumericUpDown
      // 
      this.polynomialRegressionOrderNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.polynomialRegressionOrderNumericUpDown.Location = new System.Drawing.Point(51, 47);
      this.polynomialRegressionOrderNumericUpDown.Margin = new System.Windows.Forms.Padding(9, 3, 3, 3);
      this.polynomialRegressionOrderNumericUpDown.Maximum = new decimal(new int[] {
            6,
            0,
            0,
            0});
      this.polynomialRegressionOrderNumericUpDown.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
      this.polynomialRegressionOrderNumericUpDown.Name = "polynomialRegressionOrderNumericUpDown";
      this.polynomialRegressionOrderNumericUpDown.Size = new System.Drawing.Size(115, 20);
      this.polynomialRegressionOrderNumericUpDown.TabIndex = 15;
      this.polynomialRegressionOrderNumericUpDown.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
      this.polynomialRegressionOrderNumericUpDown.ValueChanged += new System.EventHandler(this.polynomialRegressionOrderNumericUpDown_ValueChanged);
      // 
      // orderLabel
      // 
      this.orderLabel.AutoSize = true;
      this.orderLabel.Location = new System.Drawing.Point(6, 49);
      this.orderLabel.Name = "orderLabel";
      this.orderLabel.Size = new System.Drawing.Size(36, 13);
      this.orderLabel.TabIndex = 16;
      this.orderLabel.Text = "Order:";
      // 
      // regressionTypeLabel
      // 
      this.regressionTypeLabel.AutoSize = true;
      this.regressionTypeLabel.Location = new System.Drawing.Point(6, 22);
      this.regressionTypeLabel.Name = "regressionTypeLabel";
      this.regressionTypeLabel.Size = new System.Drawing.Size(34, 13);
      this.regressionTypeLabel.TabIndex = 13;
      this.regressionTypeLabel.Text = "Type:";
      // 
      // splitContainer
      // 
      this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
      this.splitContainer.Location = new System.Drawing.Point(0, 0);
      this.splitContainer.Name = "splitContainer";
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.regressionGroupBox);
      this.splitContainer.Panel1.Controls.Add(this.variablesGroupBox);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.scatterPlotView);
      this.splitContainer.Panel2.Controls.Add(this.gradientPanel);
      this.splitContainer.Size = new System.Drawing.Size(863, 517);
      this.splitContainer.SplitterDistance = 172;
      this.splitContainer.TabIndex = 5;
      // 
      // gradientPanel
      // 
      this.gradientPanel.Controls.Add(this.gradientPictureBox);
      this.gradientPanel.Controls.Add(this.gradientMinimumLabel);
      this.gradientPanel.Controls.Add(this.gradientMaximumLabel);
      this.gradientPanel.Dock = System.Windows.Forms.DockStyle.Right;
      this.gradientPanel.Location = new System.Drawing.Point(618, 0);
      this.gradientPanel.Name = "gradientPanel";
      this.gradientPanel.Size = new System.Drawing.Size(69, 517);
      this.gradientPanel.TabIndex = 1;
      this.gradientPanel.Visible = false;
      // 
      // gradientPictureBox
      // 
      this.gradientPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.gradientPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.gradientPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("gradientPictureBox.Image")));
      this.gradientPictureBox.Location = new System.Drawing.Point(17, 29);
      this.gradientPictureBox.Name = "gradientPictureBox";
      this.gradientPictureBox.Size = new System.Drawing.Size(35, 460);
      this.gradientPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
      this.gradientPictureBox.TabIndex = 19;
      this.gradientPictureBox.TabStop = false;
      // 
      // gradientMinimumLabel
      // 
      this.gradientMinimumLabel.BackColor = System.Drawing.Color.Transparent;
      this.gradientMinimumLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.gradientMinimumLabel.Location = new System.Drawing.Point(0, 492);
      this.gradientMinimumLabel.Name = "gradientMinimumLabel";
      this.gradientMinimumLabel.Size = new System.Drawing.Size(69, 25);
      this.gradientMinimumLabel.TabIndex = 18;
      this.gradientMinimumLabel.Text = "0.0";
      this.gradientMinimumLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
      // 
      // gradientMaximumLabel
      // 
      this.gradientMaximumLabel.BackColor = System.Drawing.Color.Transparent;
      this.gradientMaximumLabel.Dock = System.Windows.Forms.DockStyle.Top;
      this.gradientMaximumLabel.Location = new System.Drawing.Point(0, 0);
      this.gradientMaximumLabel.Name = "gradientMaximumLabel";
      this.gradientMaximumLabel.Size = new System.Drawing.Size(69, 25);
      this.gradientMaximumLabel.TabIndex = 17;
      this.gradientMaximumLabel.Text = "1.0";
      this.gradientMaximumLabel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
      // 
      // ScatterPlotSingleView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.splitContainer);
      this.Name = "ScatterPlotSingleView";
      this.Size = new System.Drawing.Size(863, 517);
      this.variablesGroupBox.ResumeLayout(false);
      this.variablesGroupBox.PerformLayout();
      this.regressionGroupBox.ResumeLayout(false);
      this.regressionGroupBox.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.polynomialRegressionOrderNumericUpDown)).EndInit();
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.gradientPanel.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.gradientPictureBox)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private HeuristicLab.Analysis.Views.ScatterPlotView scatterPlotView;
    private System.Windows.Forms.GroupBox variablesGroupBox;
    private System.Windows.Forms.Label yLabel;
    private System.Windows.Forms.Label xLabel;
    private System.Windows.Forms.ComboBox comboBoxYVariable;
    private System.Windows.Forms.ComboBox comboBoxXVariable;
    private System.Windows.Forms.ComboBox comboBoxGroup;
    private System.Windows.Forms.Label groupLabel;
    private System.Windows.Forms.GroupBox regressionGroupBox;
    private System.Windows.Forms.ComboBox regressionTypeComboBox;
    private System.Windows.Forms.NumericUpDown polynomialRegressionOrderNumericUpDown;
    private System.Windows.Forms.Label orderLabel;
    private System.Windows.Forms.Label regressionTypeLabel;
    private System.Windows.Forms.SplitContainer splitContainer;
    private System.Windows.Forms.CheckBox useGradientCheckBox;
    private System.Windows.Forms.Panel gradientPanel;
    private System.Windows.Forms.Label gradientMinimumLabel;
    private System.Windows.Forms.Label gradientMaximumLabel;
    private System.Windows.Forms.PictureBox gradientPictureBox;
    private System.Windows.Forms.Label legendOrderLabel;
    private System.Windows.Forms.ComboBox orderComboBox;
  }
}
