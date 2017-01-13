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

namespace HeuristicLab.Analysis.Views {
  partial class DataRowVisualPropertiesControl {
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
      this.chartTypeComboBox = new System.Windows.Forms.ComboBox();
      this.colorDialog = new System.Windows.Forms.ColorDialog();
      this.colorButton = new System.Windows.Forms.Button();
      this.startIndexZeroCheckBox = new System.Windows.Forms.CheckBox();
      this.binsNumericUpDown = new System.Windows.Forms.NumericUpDown();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.commonGroupBox = new System.Windows.Forms.GroupBox();
      this.isVisibleInLegendCheckBox = new System.Windows.Forms.CheckBox();
      this.label10 = new System.Windows.Forms.Label();
      this.displayNameTextBox = new System.Windows.Forms.TextBox();
      this.axisGroupBox = new System.Windows.Forms.GroupBox();
      this.panel2 = new System.Windows.Forms.Panel();
      this.yAxisSecondaryRadioButton = new System.Windows.Forms.RadioButton();
      this.yAxisPrimaryRadioButton = new System.Windows.Forms.RadioButton();
      this.panel1 = new System.Windows.Forms.Panel();
      this.xAxisPrimaryRadioButton = new System.Windows.Forms.RadioButton();
      this.xAxisSecondaryRadioButton = new System.Windows.Forms.RadioButton();
      this.label5 = new System.Windows.Forms.Label();
      this.label6 = new System.Windows.Forms.Label();
      this.label9 = new System.Windows.Forms.Label();
      this.histoGramGroupBox = new System.Windows.Forms.GroupBox();
      this.binsExactRadioButton = new System.Windows.Forms.RadioButton();
      this.binsApproximatelyRadioButton = new System.Windows.Forms.RadioButton();
      this.lineChartGroupBox = new System.Windows.Forms.GroupBox();
      this.lineWidthNumericUpDown = new System.Windows.Forms.NumericUpDown();
      this.label4 = new System.Windows.Forms.Label();
      this.label7 = new System.Windows.Forms.Label();
      this.lineStyleComboBox = new System.Windows.Forms.ComboBox();
      this.label8 = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.binsNumericUpDown)).BeginInit();
      this.commonGroupBox.SuspendLayout();
      this.axisGroupBox.SuspendLayout();
      this.panel2.SuspendLayout();
      this.panel1.SuspendLayout();
      this.histoGramGroupBox.SuspendLayout();
      this.lineChartGroupBox.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.lineWidthNumericUpDown)).BeginInit();
      this.SuspendLayout();
      // 
      // chartTypeComboBox
      // 
      this.chartTypeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.chartTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.chartTypeComboBox.FormattingEnabled = true;
      this.chartTypeComboBox.Location = new System.Drawing.Point(102, 45);
      this.chartTypeComboBox.Name = "chartTypeComboBox";
      this.chartTypeComboBox.Size = new System.Drawing.Size(219, 21);
      this.chartTypeComboBox.TabIndex = 3;
      this.chartTypeComboBox.SelectedValueChanged += new System.EventHandler(this.chartTypeComboBox_SelectedValueChanged);
      // 
      // colorDialog
      // 
      this.colorDialog.Color = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
      // 
      // colorButton
      // 
      this.colorButton.BackColor = System.Drawing.SystemColors.Control;
      this.colorButton.Location = new System.Drawing.Point(102, 72);
      this.colorButton.Name = "colorButton";
      this.colorButton.Size = new System.Drawing.Size(23, 23);
      this.colorButton.TabIndex = 5;
      this.colorButton.UseVisualStyleBackColor = false;
      this.colorButton.Click += new System.EventHandler(this.colorButton_Click);
      // 
      // startIndexZeroCheckBox
      // 
      this.startIndexZeroCheckBox.AutoSize = true;
      this.startIndexZeroCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.startIndexZeroCheckBox.Location = new System.Drawing.Point(81, 46);
      this.startIndexZeroCheckBox.Name = "startIndexZeroCheckBox";
      this.startIndexZeroCheckBox.Size = new System.Drawing.Size(15, 14);
      this.startIndexZeroCheckBox.TabIndex = 3;
      this.startIndexZeroCheckBox.UseVisualStyleBackColor = true;
      this.startIndexZeroCheckBox.CheckedChanged += new System.EventHandler(this.startIndexZeroCheckBox_CheckedChanged);
      // 
      // binsNumericUpDown
      // 
      this.binsNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.binsNumericUpDown.Location = new System.Drawing.Point(57, 19);
      this.binsNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.binsNumericUpDown.Name = "binsNumericUpDown";
      this.binsNumericUpDown.Size = new System.Drawing.Size(97, 20);
      this.binsNumericUpDown.TabIndex = 1;
      this.binsNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.binsNumericUpDown.ValueChanged += new System.EventHandler(this.binsNumericUpDown_ValueChanged);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(6, 48);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(34, 13);
      this.label1.TabIndex = 2;
      this.label1.Text = "&Type:";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(6, 77);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(34, 13);
      this.label2.TabIndex = 4;
      this.label2.Text = "&Color:";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(6, 21);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(30, 13);
      this.label3.TabIndex = 0;
      this.label3.Text = "&Bins:";
      // 
      // commonGroupBox
      // 
      this.commonGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.commonGroupBox.Controls.Add(this.isVisibleInLegendCheckBox);
      this.commonGroupBox.Controls.Add(this.label10);
      this.commonGroupBox.Controls.Add(this.displayNameTextBox);
      this.commonGroupBox.Controls.Add(this.axisGroupBox);
      this.commonGroupBox.Controls.Add(this.colorButton);
      this.commonGroupBox.Controls.Add(this.label9);
      this.commonGroupBox.Controls.Add(this.label1);
      this.commonGroupBox.Controls.Add(this.label2);
      this.commonGroupBox.Controls.Add(this.chartTypeComboBox);
      this.commonGroupBox.Location = new System.Drawing.Point(0, 0);
      this.commonGroupBox.Name = "commonGroupBox";
      this.commonGroupBox.Size = new System.Drawing.Size(327, 199);
      this.commonGroupBox.TabIndex = 0;
      this.commonGroupBox.TabStop = false;
      this.commonGroupBox.Text = "Common";
      // 
      // isVisibleInLegendCheckBox
      // 
      this.isVisibleInLegendCheckBox.AutoSize = true;
      this.isVisibleInLegendCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.isVisibleInLegendCheckBox.Location = new System.Drawing.Point(102, 104);
      this.isVisibleInLegendCheckBox.Name = "isVisibleInLegendCheckBox";
      this.isVisibleInLegendCheckBox.Size = new System.Drawing.Size(15, 14);
      this.isVisibleInLegendCheckBox.TabIndex = 8;
      this.isVisibleInLegendCheckBox.UseVisualStyleBackColor = true;
      this.isVisibleInLegendCheckBox.CheckedChanged += new System.EventHandler(this.isVisibleInLegendCheckBox_CheckedChanged);
      // 
      // label10
      // 
      this.label10.AutoSize = true;
      this.label10.Location = new System.Drawing.Point(6, 104);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(90, 13);
      this.label10.TabIndex = 7;
      this.label10.Text = "&Visible in Legend:";
      // 
      // displayNameTextBox
      // 
      this.displayNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.displayNameTextBox.Location = new System.Drawing.Point(102, 19);
      this.displayNameTextBox.Name = "displayNameTextBox";
      this.displayNameTextBox.Size = new System.Drawing.Size(219, 20);
      this.displayNameTextBox.TabIndex = 1;
      this.displayNameTextBox.Validated += new System.EventHandler(this.displayNameTextBox_Validated);
      // 
      // axisGroupBox
      // 
      this.axisGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.axisGroupBox.Controls.Add(this.panel2);
      this.axisGroupBox.Controls.Add(this.panel1);
      this.axisGroupBox.Controls.Add(this.label5);
      this.axisGroupBox.Controls.Add(this.label6);
      this.axisGroupBox.Location = new System.Drawing.Point(6, 126);
      this.axisGroupBox.Name = "axisGroupBox";
      this.axisGroupBox.Size = new System.Drawing.Size(315, 67);
      this.axisGroupBox.TabIndex = 6;
      this.axisGroupBox.TabStop = false;
      this.axisGroupBox.Text = "Axes";
      // 
      // panel2
      // 
      this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.panel2.Controls.Add(this.yAxisSecondaryRadioButton);
      this.panel2.Controls.Add(this.yAxisPrimaryRadioButton);
      this.panel2.Location = new System.Drawing.Point(51, 42);
      this.panel2.Name = "panel2";
      this.panel2.Size = new System.Drawing.Size(258, 17);
      this.panel2.TabIndex = 7;
      // 
      // yAxisSecondaryRadioButton
      // 
      this.yAxisSecondaryRadioButton.AutoSize = true;
      this.yAxisSecondaryRadioButton.Location = new System.Drawing.Point(65, 0);
      this.yAxisSecondaryRadioButton.Name = "yAxisSecondaryRadioButton";
      this.yAxisSecondaryRadioButton.Size = new System.Drawing.Size(76, 17);
      this.yAxisSecondaryRadioButton.TabIndex = 5;
      this.yAxisSecondaryRadioButton.TabStop = true;
      this.yAxisSecondaryRadioButton.Text = "&Secondary";
      this.yAxisSecondaryRadioButton.UseVisualStyleBackColor = true;
      this.yAxisSecondaryRadioButton.CheckedChanged += new System.EventHandler(this.yAxisRadioButton_CheckedChanged);
      // 
      // yAxisPrimaryRadioButton
      // 
      this.yAxisPrimaryRadioButton.AutoSize = true;
      this.yAxisPrimaryRadioButton.Location = new System.Drawing.Point(0, 0);
      this.yAxisPrimaryRadioButton.Name = "yAxisPrimaryRadioButton";
      this.yAxisPrimaryRadioButton.Size = new System.Drawing.Size(59, 17);
      this.yAxisPrimaryRadioButton.TabIndex = 4;
      this.yAxisPrimaryRadioButton.TabStop = true;
      this.yAxisPrimaryRadioButton.Text = "&Primary";
      this.yAxisPrimaryRadioButton.UseVisualStyleBackColor = true;
      this.yAxisPrimaryRadioButton.CheckedChanged += new System.EventHandler(this.yAxisRadioButton_CheckedChanged);
      // 
      // panel1
      // 
      this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.panel1.Controls.Add(this.xAxisPrimaryRadioButton);
      this.panel1.Controls.Add(this.xAxisSecondaryRadioButton);
      this.panel1.Location = new System.Drawing.Point(51, 19);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(258, 17);
      this.panel1.TabIndex = 6;
      // 
      // xAxisPrimaryRadioButton
      // 
      this.xAxisPrimaryRadioButton.AutoSize = true;
      this.xAxisPrimaryRadioButton.Location = new System.Drawing.Point(0, 0);
      this.xAxisPrimaryRadioButton.Name = "xAxisPrimaryRadioButton";
      this.xAxisPrimaryRadioButton.Size = new System.Drawing.Size(59, 17);
      this.xAxisPrimaryRadioButton.TabIndex = 1;
      this.xAxisPrimaryRadioButton.TabStop = true;
      this.xAxisPrimaryRadioButton.Text = "&Primary";
      this.xAxisPrimaryRadioButton.UseVisualStyleBackColor = true;
      this.xAxisPrimaryRadioButton.CheckedChanged += new System.EventHandler(this.xAxisRadioButton_CheckedChanged);
      // 
      // xAxisSecondaryRadioButton
      // 
      this.xAxisSecondaryRadioButton.AutoSize = true;
      this.xAxisSecondaryRadioButton.Location = new System.Drawing.Point(65, 0);
      this.xAxisSecondaryRadioButton.Name = "xAxisSecondaryRadioButton";
      this.xAxisSecondaryRadioButton.Size = new System.Drawing.Size(76, 17);
      this.xAxisSecondaryRadioButton.TabIndex = 2;
      this.xAxisSecondaryRadioButton.TabStop = true;
      this.xAxisSecondaryRadioButton.Text = "&Secondary";
      this.xAxisSecondaryRadioButton.UseVisualStyleBackColor = true;
      this.xAxisSecondaryRadioButton.CheckedChanged += new System.EventHandler(this.xAxisRadioButton_CheckedChanged);
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(6, 44);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(39, 13);
      this.label5.TabIndex = 3;
      this.label5.Text = "&Y-Axis:";
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(6, 21);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(39, 13);
      this.label6.TabIndex = 0;
      this.label6.Text = "&X-Axis:";
      // 
      // label9
      // 
      this.label9.AutoSize = true;
      this.label9.Location = new System.Drawing.Point(6, 22);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(38, 13);
      this.label9.TabIndex = 0;
      this.label9.Text = "&Name:";
      // 
      // histoGramGroupBox
      // 
      this.histoGramGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.histoGramGroupBox.Controls.Add(this.binsExactRadioButton);
      this.histoGramGroupBox.Controls.Add(this.binsApproximatelyRadioButton);
      this.histoGramGroupBox.Controls.Add(this.binsNumericUpDown);
      this.histoGramGroupBox.Controls.Add(this.label3);
      this.histoGramGroupBox.Location = new System.Drawing.Point(0, 305);
      this.histoGramGroupBox.Name = "histoGramGroupBox";
      this.histoGramGroupBox.Size = new System.Drawing.Size(327, 47);
      this.histoGramGroupBox.TabIndex = 2;
      this.histoGramGroupBox.TabStop = false;
      this.histoGramGroupBox.Text = "Histogram Properties";
      // 
      // binsExactRadioButton
      // 
      this.binsExactRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.binsExactRadioButton.AutoSize = true;
      this.binsExactRadioButton.Location = new System.Drawing.Point(269, 19);
      this.binsExactRadioButton.Name = "binsExactRadioButton";
      this.binsExactRadioButton.Size = new System.Drawing.Size(52, 17);
      this.binsExactRadioButton.TabIndex = 3;
      this.binsExactRadioButton.TabStop = true;
      this.binsExactRadioButton.Text = "&Exact";
      this.binsExactRadioButton.UseVisualStyleBackColor = true;
      this.binsExactRadioButton.CheckedChanged += new System.EventHandler(this.binNumberRadioButton_CheckedChanged);
      // 
      // binsApproximatelyRadioButton
      // 
      this.binsApproximatelyRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.binsApproximatelyRadioButton.AutoSize = true;
      this.binsApproximatelyRadioButton.Location = new System.Drawing.Point(173, 19);
      this.binsApproximatelyRadioButton.Name = "binsApproximatelyRadioButton";
      this.binsApproximatelyRadioButton.Size = new System.Drawing.Size(90, 17);
      this.binsApproximatelyRadioButton.TabIndex = 2;
      this.binsApproximatelyRadioButton.TabStop = true;
      this.binsApproximatelyRadioButton.Text = "&Approximately";
      this.binsApproximatelyRadioButton.UseVisualStyleBackColor = true;
      this.binsApproximatelyRadioButton.CheckedChanged += new System.EventHandler(this.binNumberRadioButton_CheckedChanged);
      // 
      // lineChartGroupBox
      // 
      this.lineChartGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.lineChartGroupBox.Controls.Add(this.lineWidthNumericUpDown);
      this.lineChartGroupBox.Controls.Add(this.label4);
      this.lineChartGroupBox.Controls.Add(this.startIndexZeroCheckBox);
      this.lineChartGroupBox.Controls.Add(this.label7);
      this.lineChartGroupBox.Controls.Add(this.lineStyleComboBox);
      this.lineChartGroupBox.Controls.Add(this.label8);
      this.lineChartGroupBox.Location = new System.Drawing.Point(0, 205);
      this.lineChartGroupBox.Name = "lineChartGroupBox";
      this.lineChartGroupBox.Size = new System.Drawing.Size(327, 94);
      this.lineChartGroupBox.TabIndex = 1;
      this.lineChartGroupBox.TabStop = false;
      this.lineChartGroupBox.Text = "Line Properties";
      // 
      // lineWidthNumericUpDown
      // 
      this.lineWidthNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.lineWidthNumericUpDown.Location = new System.Drawing.Point(82, 66);
      this.lineWidthNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.lineWidthNumericUpDown.Name = "lineWidthNumericUpDown";
      this.lineWidthNumericUpDown.Size = new System.Drawing.Size(239, 20);
      this.lineWidthNumericUpDown.TabIndex = 5;
      this.lineWidthNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.lineWidthNumericUpDown.ValueChanged += new System.EventHandler(this.lineWidthNumericUpDown_ValueChanged);
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(6, 68);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(38, 13);
      this.label4.TabIndex = 4;
      this.label4.Text = "&Width:";
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(6, 46);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(70, 13);
      this.label7.TabIndex = 2;
      this.label7.Text = "Start &Index 0:";
      // 
      // lineStyleComboBox
      // 
      this.lineStyleComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.lineStyleComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.lineStyleComboBox.FormattingEnabled = true;
      this.lineStyleComboBox.Location = new System.Drawing.Point(82, 19);
      this.lineStyleComboBox.Name = "lineStyleComboBox";
      this.lineStyleComboBox.Size = new System.Drawing.Size(239, 21);
      this.lineStyleComboBox.TabIndex = 1;
      this.lineStyleComboBox.SelectedValueChanged += new System.EventHandler(this.lineStyleComboBox_SelectedValueChanged);
      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.Location = new System.Drawing.Point(6, 22);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(33, 13);
      this.label8.TabIndex = 0;
      this.label8.Text = "&Style:";
      // 
      // DataRowVisualPropertiesControl
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.lineChartGroupBox);
      this.Controls.Add(this.histoGramGroupBox);
      this.Controls.Add(this.commonGroupBox);
      this.Name = "DataRowVisualPropertiesControl";
      this.Size = new System.Drawing.Size(327, 356);
      ((System.ComponentModel.ISupportInitialize)(this.binsNumericUpDown)).EndInit();
      this.commonGroupBox.ResumeLayout(false);
      this.commonGroupBox.PerformLayout();
      this.axisGroupBox.ResumeLayout(false);
      this.axisGroupBox.PerformLayout();
      this.panel2.ResumeLayout(false);
      this.panel2.PerformLayout();
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.histoGramGroupBox.ResumeLayout(false);
      this.histoGramGroupBox.PerformLayout();
      this.lineChartGroupBox.ResumeLayout(false);
      this.lineChartGroupBox.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.lineWidthNumericUpDown)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ComboBox chartTypeComboBox;
    private System.Windows.Forms.ColorDialog colorDialog;
    private System.Windows.Forms.Button colorButton;
    private System.Windows.Forms.CheckBox startIndexZeroCheckBox;
    private System.Windows.Forms.NumericUpDown binsNumericUpDown;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.GroupBox commonGroupBox;
    private System.Windows.Forms.GroupBox histoGramGroupBox;
    private System.Windows.Forms.RadioButton xAxisSecondaryRadioButton;
    private System.Windows.Forms.RadioButton yAxisSecondaryRadioButton;
    private System.Windows.Forms.RadioButton xAxisPrimaryRadioButton;
    private System.Windows.Forms.RadioButton yAxisPrimaryRadioButton;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.GroupBox lineChartGroupBox;
    private System.Windows.Forms.NumericUpDown lineWidthNumericUpDown;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.RadioButton binsExactRadioButton;
    private System.Windows.Forms.RadioButton binsApproximatelyRadioButton;
    private System.Windows.Forms.ComboBox lineStyleComboBox;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.GroupBox axisGroupBox;
    private System.Windows.Forms.TextBox displayNameTextBox;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.Panel panel2;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Label label10;
    private System.Windows.Forms.CheckBox isVisibleInLegendCheckBox;
  }
}
