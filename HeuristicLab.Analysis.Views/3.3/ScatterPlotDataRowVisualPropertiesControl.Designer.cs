#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  partial class ScatterPlotDataRowVisualPropertiesControl {
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
      this.pointStyleComboBox = new System.Windows.Forms.ComboBox();
      this.colorDialog = new System.Windows.Forms.ColorDialog();
      this.colorButton = new System.Windows.Forms.Button();
      this.isVisibleInLegendCheckBox = new System.Windows.Forms.CheckBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.pointSizeNumericUpDown = new System.Windows.Forms.NumericUpDown();
      this.label3 = new System.Windows.Forms.Label();
      this.displayNameTextBox = new System.Windows.Forms.TextBox();
      this.label9 = new System.Windows.Forms.Label();
      this.label7 = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.pointSizeNumericUpDown)).BeginInit();
      this.SuspendLayout();
      // 
      // pointStyleComboBox
      // 
      this.pointStyleComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pointStyleComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.pointStyleComboBox.FormattingEnabled = true;
      this.pointStyleComboBox.Location = new System.Drawing.Point(99, 34);
      this.pointStyleComboBox.Name = "pointStyleComboBox";
      this.pointStyleComboBox.Size = new System.Drawing.Size(240, 21);
      this.pointStyleComboBox.TabIndex = 3;
      this.pointStyleComboBox.SelectedValueChanged += new System.EventHandler(this.pointStyleComboBox_SelectedValueChanged);
      // 
      // colorDialog
      // 
      this.colorDialog.Color = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
      // 
      // colorButton
      // 
      this.colorButton.BackColor = System.Drawing.SystemColors.Control;
      this.colorButton.Location = new System.Drawing.Point(99, 61);
      this.colorButton.Name = "colorButton";
      this.colorButton.Size = new System.Drawing.Size(23, 23);
      this.colorButton.TabIndex = 5;
      this.colorButton.UseVisualStyleBackColor = false;
      this.colorButton.Click += new System.EventHandler(this.colorButton_Click);
      // 
      // isVisibleInLegendCheckBox
      // 
      this.isVisibleInLegendCheckBox.AutoSize = true;
      this.isVisibleInLegendCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.isVisibleInLegendCheckBox.Location = new System.Drawing.Point(99, 116);
      this.isVisibleInLegendCheckBox.Name = "isVisibleInLegendCheckBox";
      this.isVisibleInLegendCheckBox.Size = new System.Drawing.Size(15, 14);
      this.isVisibleInLegendCheckBox.TabIndex = 3;
      this.isVisibleInLegendCheckBox.UseVisualStyleBackColor = true;
      this.isVisibleInLegendCheckBox.CheckedChanged += new System.EventHandler(this.isVisibleInLegendCheckBox_CheckedChanged);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(3, 37);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(60, 13);
      this.label1.TabIndex = 2;
      this.label1.Text = "&Point Style:";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(3, 66);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(61, 13);
      this.label2.TabIndex = 4;
      this.label2.Text = "Point &Color:";
      // 
      // pointSizeNumericUpDown
      // 
      this.pointSizeNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pointSizeNumericUpDown.Location = new System.Drawing.Point(99, 90);
      this.pointSizeNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.pointSizeNumericUpDown.Name = "pointSizeNumericUpDown";
      this.pointSizeNumericUpDown.Size = new System.Drawing.Size(240, 20);
      this.pointSizeNumericUpDown.TabIndex = 5;
      this.pointSizeNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.pointSizeNumericUpDown.ValueChanged += new System.EventHandler(this.pointSizeNumericUpDown_ValueChanged);
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(3, 92);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(57, 13);
      this.label3.TabIndex = 6;
      this.label3.Text = "Point &Size:";
      // 
      // displayNameTextBox
      // 
      this.displayNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.displayNameTextBox.Location = new System.Drawing.Point(99, 8);
      this.displayNameTextBox.Name = "displayNameTextBox";
      this.displayNameTextBox.Size = new System.Drawing.Size(240, 20);
      this.displayNameTextBox.TabIndex = 1;
      this.displayNameTextBox.Validated += new System.EventHandler(this.displayNameTextBox_Validated);
      // 
      // label9
      // 
      this.label9.AutoSize = true;
      this.label9.Location = new System.Drawing.Point(3, 11);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(38, 13);
      this.label9.TabIndex = 0;
      this.label9.Text = "&Name:";
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(3, 116);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(90, 13);
      this.label7.TabIndex = 2;
      this.label7.Text = "&Visible in Legend:";
      // 
      // ScatterPlotDataRowVisualPropertiesControl
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.pointSizeNumericUpDown);
      this.Controls.Add(this.isVisibleInLegendCheckBox);
      this.Controls.Add(this.label9);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.pointStyleComboBox);
      this.Controls.Add(this.label7);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.displayNameTextBox);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.colorButton);
      this.Name = "ScatterPlotDataRowVisualPropertiesControl";
      this.Size = new System.Drawing.Size(342, 135);
      ((System.ComponentModel.ISupportInitialize)(this.pointSizeNumericUpDown)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ComboBox pointStyleComboBox;
    private System.Windows.Forms.ColorDialog colorDialog;
    private System.Windows.Forms.Button colorButton;
    private System.Windows.Forms.CheckBox isVisibleInLegendCheckBox;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.NumericUpDown pointSizeNumericUpDown;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.TextBox displayNameTextBox;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.Label label3;
  }
}
