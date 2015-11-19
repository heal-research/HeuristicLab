#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Visualization.ChartControlsExtensions;

namespace HeuristicLab.Analysis.Views {
  partial class HistogramControl {
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
      System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
      System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
      System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
      this.chart = new HeuristicLab.Visualization.ChartControlsExtensions.EnhancedChart();
      this.binsNumericUpDown = new System.Windows.Forms.NumericUpDown();
      this.label1 = new System.Windows.Forms.Label();
      this.exactCheckBox = new System.Windows.Forms.CheckBox();
      this.label2 = new System.Windows.Forms.Label();
      this.bandwidthNumericUpDown = new System.Windows.Forms.NumericUpDown();
      this.noDataLabel = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.chart)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.binsNumericUpDown)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.bandwidthNumericUpDown)).BeginInit();
      this.SuspendLayout();
      // 
      // chart
      // 
      this.chart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      chartArea2.Name = "ChartArea1";
      this.chart.ChartAreas.Add(chartArea2);
      legend2.Alignment = System.Drawing.StringAlignment.Center;
      legend2.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
      legend2.Name = "Default";
      this.chart.Legends.Add(legend2);
      this.chart.Location = new System.Drawing.Point(0, 27);
      this.chart.Name = "chart";
      series2.ChartArea = "ChartArea1";
      series2.Legend = "Default";
      series2.Name = "Series1";
      this.chart.Series.Add(series2);
      this.chart.Size = new System.Drawing.Size(465, 336);
      this.chart.TabIndex = 0;
      // 
      // binsNumericUpDown
      // 
      this.binsNumericUpDown.Location = new System.Drawing.Point(91, 3);
      this.binsNumericUpDown.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
      this.binsNumericUpDown.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
      this.binsNumericUpDown.Name = "binsNumericUpDown";
      this.binsNumericUpDown.Size = new System.Drawing.Size(61, 20);
      this.binsNumericUpDown.TabIndex = 1;
      this.binsNumericUpDown.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
      this.binsNumericUpDown.ValueChanged += new System.EventHandler(this.binsNumericUpDown_ValueChanged);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(3, 5);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(82, 13);
      this.label1.TabIndex = 2;
      this.label1.Text = "Number of Bins:";
      // 
      // exactCheckBox
      // 
      this.exactCheckBox.AutoSize = true;
      this.exactCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.exactCheckBox.Location = new System.Drawing.Point(291, 4);
      this.exactCheckBox.Name = "exactCheckBox";
      this.exactCheckBox.Size = new System.Drawing.Size(56, 17);
      this.exactCheckBox.TabIndex = 3;
      this.exactCheckBox.Text = "Exact:";
      this.exactCheckBox.UseVisualStyleBackColor = true;
      this.exactCheckBox.CheckedChanged += new System.EventHandler(this.exactCheckBox_CheckedChanged);
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(158, 5);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(60, 13);
      this.label2.TabIndex = 5;
      this.label2.Text = "Bandwidth:";
      // 
      // bandwidthNumericUpDown
      // 
      this.bandwidthNumericUpDown.DecimalPlaces = 2;
      this.bandwidthNumericUpDown.Location = new System.Drawing.Point(224, 3);
      this.bandwidthNumericUpDown.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
      this.bandwidthNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            851968});
      this.bandwidthNumericUpDown.Name = "bandwidthNumericUpDown";
      this.bandwidthNumericUpDown.Size = new System.Drawing.Size(61, 20);
      this.bandwidthNumericUpDown.TabIndex = 4;
      this.bandwidthNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.bandwidthNumericUpDown.ValueChanged += new System.EventHandler(this.bandwidthNumericUpDown_ValueChanged);
      // 
      // noDataLabel
      // 
      this.noDataLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.noDataLabel.AutoSize = true;
      this.noDataLabel.Location = new System.Drawing.Point(163, 175);
      this.noDataLabel.Name = "noDataLabel";
      this.noDataLabel.Size = new System.Drawing.Size(139, 13);
      this.noDataLabel.TabIndex = 22;
      this.noDataLabel.Text = "No data could be displayed.";
      // 
      // HistogramControl
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.noDataLabel);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.bandwidthNumericUpDown);
      this.Controls.Add(this.exactCheckBox);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.binsNumericUpDown);
      this.Controls.Add(this.chart);
      this.Name = "HistogramControl";
      this.Size = new System.Drawing.Size(465, 363);
      ((System.ComponentModel.ISupportInitialize)(this.chart)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.binsNumericUpDown)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.bandwidthNumericUpDown)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private EnhancedChart chart;
    private System.Windows.Forms.NumericUpDown binsNumericUpDown;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.CheckBox exactCheckBox;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.NumericUpDown bandwidthNumericUpDown;
    private System.Windows.Forms.Label noDataLabel;
  }
}
