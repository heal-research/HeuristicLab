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

namespace HeuristicLab.Analysis.Views {
  partial class HeatMapView {
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
      System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
      System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HeatMapView));
      this.chart = new HeuristicLab.Visualization.ChartControlsExtensions.EnhancedChart();
      this.colorsPictureBox = new System.Windows.Forms.PictureBox();
      this.minimumLabel = new System.Windows.Forms.Label();
      this.maximumLabel = new System.Windows.Forms.Label();
      this.grayscalesPictureBox = new System.Windows.Forms.PictureBox();
      this.grayscaleCheckBox = new System.Windows.Forms.CheckBox();
      ((System.ComponentModel.ISupportInitialize)(this.chart)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.colorsPictureBox)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.grayscalesPictureBox)).BeginInit();
      this.SuspendLayout();
      // 
      // chart
      // 
      this.chart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.chart.BorderlineColor = System.Drawing.Color.Black;
      this.chart.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
      chartArea1.AxisX.IntervalAutoMode = System.Windows.Forms.DataVisualization.Charting.IntervalAutoMode.VariableCount;
      chartArea1.AxisX.LabelAutoFitStyle = ((System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles)((System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.IncreaseFont | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.DecreaseFont)));
      chartArea1.AxisX.Title = "Column Index";
      chartArea1.AxisY.Title = "Row Index";
      chartArea1.CursorX.IsUserEnabled = true;
      chartArea1.CursorX.IsUserSelectionEnabled = true;
      chartArea1.CursorY.IsUserEnabled = true;
      chartArea1.CursorY.IsUserSelectionEnabled = true;
      chartArea1.Name = "Default";
      this.chart.ChartAreas.Add(chartArea1);
      this.chart.Location = new System.Drawing.Point(0, 0);
      this.chart.Name = "chart";
      this.chart.Size = new System.Drawing.Size(406, 463);
      this.chart.TabIndex = 0;
      this.chart.Text = "chart";
      title1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      title1.Name = "Default";
      title1.Text = "Heat Map";
      this.chart.Titles.Add(title1);
      // 
      // colorsPictureBox
      // 
      this.colorsPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.colorsPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.colorsPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("colorsPictureBox.Image")));
      this.colorsPictureBox.Location = new System.Drawing.Point(430, 28);
      this.colorsPictureBox.Name = "colorsPictureBox";
      this.colorsPictureBox.Size = new System.Drawing.Size(35, 393);
      this.colorsPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
      this.colorsPictureBox.TabIndex = 11;
      this.colorsPictureBox.TabStop = false;
      // 
      // minimumLabel
      // 
      this.minimumLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.minimumLabel.BackColor = System.Drawing.Color.Transparent;
      this.minimumLabel.Location = new System.Drawing.Point(412, 424);
      this.minimumLabel.Name = "minimumLabel";
      this.minimumLabel.Size = new System.Drawing.Size(73, 19);
      this.minimumLabel.TabIndex = 2;
      this.minimumLabel.Text = "0.0";
      this.minimumLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
      // 
      // maximumLabel
      // 
      this.maximumLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.maximumLabel.BackColor = System.Drawing.Color.Transparent;
      this.maximumLabel.Location = new System.Drawing.Point(412, 0);
      this.maximumLabel.Name = "maximumLabel";
      this.maximumLabel.Size = new System.Drawing.Size(73, 25);
      this.maximumLabel.TabIndex = 1;
      this.maximumLabel.Text = "1.0";
      this.maximumLabel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
      // 
      // grayscalesPictureBox
      // 
      this.grayscalesPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.grayscalesPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.grayscalesPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("grayscalesPictureBox.Image")));
      this.grayscalesPictureBox.Location = new System.Drawing.Point(430, 28);
      this.grayscalesPictureBox.Name = "grayscalesPictureBox";
      this.grayscalesPictureBox.Size = new System.Drawing.Size(35, 393);
      this.grayscalesPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
      this.grayscalesPictureBox.TabIndex = 15;
      this.grayscalesPictureBox.TabStop = false;
      this.grayscalesPictureBox.Visible = false;
      // 
      // grayscaleCheckBox
      // 
      this.grayscaleCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.grayscaleCheckBox.AutoSize = true;
      this.grayscaleCheckBox.Location = new System.Drawing.Point(412, 446);
      this.grayscaleCheckBox.Name = "grayscaleCheckBox";
      this.grayscaleCheckBox.Size = new System.Drawing.Size(73, 17);
      this.grayscaleCheckBox.TabIndex = 3;
      this.grayscaleCheckBox.Text = "Grayscale";
      this.grayscaleCheckBox.UseVisualStyleBackColor = true;
      this.grayscaleCheckBox.CheckedChanged += new System.EventHandler(this.grayscaledImagesCheckBox_CheckedChanged);
      // 
      // HeatMapView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.grayscaleCheckBox);
      this.Controls.Add(this.minimumLabel);
      this.Controls.Add(this.maximumLabel);
      this.Controls.Add(this.colorsPictureBox);
      this.Controls.Add(this.chart);
      this.Controls.Add(this.grayscalesPictureBox);
      this.Name = "HeatMapView";
      this.Size = new System.Drawing.Size(485, 463);
      ((System.ComponentModel.ISupportInitialize)(this.chart)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.colorsPictureBox)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.grayscalesPictureBox)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    protected HeuristicLab.Visualization.ChartControlsExtensions.EnhancedChart chart;
    protected System.Windows.Forms.PictureBox colorsPictureBox;
    protected System.Windows.Forms.Label minimumLabel;
    protected System.Windows.Forms.Label maximumLabel;
    protected System.Windows.Forms.PictureBox grayscalesPictureBox;
    protected System.Windows.Forms.CheckBox grayscaleCheckBox;
  }
}
