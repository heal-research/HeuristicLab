#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
      System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
      System.Windows.Forms.DataVisualization.Charting.Title title1= new System.Windows.Forms.DataVisualization.Charting.Title();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HeatMapView));
      this.chart = new HeuristicLab.Visualization.ChartControlsExtensions.EnhancedChart();
      this.ColorsPictureBox = new System.Windows.Forms.PictureBox();
      this.ZeroLabel = new System.Windows.Forms.Label();
      this.OneLabel = new System.Windows.Forms.Label();
      this.GrayscalesPictureBox = new System.Windows.Forms.PictureBox();
      this.grayscaledImagesCheckBox = new System.Windows.Forms.CheckBox();
      ((System.ComponentModel.ISupportInitialize)(this.chart)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ColorsPictureBox)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.GrayscalesPictureBox)).BeginInit();
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
      chartArea1.AxisX.Title = "Solution Index";
      chartArea1.AxisY.Title = "Solution Index";
      chartArea1.CursorX.IsUserEnabled = true;
      chartArea1.CursorX.IsUserSelectionEnabled = true;
      chartArea1.CursorY.IsUserEnabled = true;
      chartArea1.CursorY.IsUserSelectionEnabled = true;
      chartArea1.Name = "Default";
      this.chart.ChartAreas.Add(chartArea1);
      legend1.Alignment = System.Drawing.StringAlignment.Center;
      legend1.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
      legend1.Name = "Default";
      this.chart.Legends.Add(legend1);
      this.chart.Location = new System.Drawing.Point(0, 0);
      this.chart.Name = "chart";
      this.chart.Size = new System.Drawing.Size(403, 335);
      this.chart.TabIndex = 0;
      this.chart.Text = "chart";
      title1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      title1.Name = "Default";
      title1.Text = "Solution Similarities";
      this.chart.Titles.Add(title1);
      // 
      // ColorsPictureBox
      // 
      this.ColorsPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.ColorsPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.ColorsPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("ColorsPictureBox.Image")));
      this.ColorsPictureBox.Location = new System.Drawing.Point(409, 23);
      this.ColorsPictureBox.Name = "ColorsPictureBox";
      this.ColorsPictureBox.Size = new System.Drawing.Size(35, 312);
      this.ColorsPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
      this.ColorsPictureBox.TabIndex = 11;
      this.ColorsPictureBox.TabStop = false;
      // 
      // ZeroLabel
      // 
      this.ZeroLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.ZeroLabel.AutoSize = true;
      this.ZeroLabel.BackColor = System.Drawing.Color.Transparent;
      this.ZeroLabel.Location = new System.Drawing.Point(416, 342);
      this.ZeroLabel.Name = "ZeroLabel";
      this.ZeroLabel.Size = new System.Drawing.Size(22, 13);
      this.ZeroLabel.TabIndex = 14;
      this.ZeroLabel.Text = "0.0";
      // 
      // OneLabel
      // 
      this.OneLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.OneLabel.AutoSize = true;
      this.OneLabel.BackColor = System.Drawing.Color.Transparent;
      this.OneLabel.Location = new System.Drawing.Point(416, 3);
      this.OneLabel.Name = "OneLabel";
      this.OneLabel.Size = new System.Drawing.Size(22, 13);
      this.OneLabel.TabIndex = 13;
      this.OneLabel.Text = "1.0";
      // 
      // GrayscalesPictureBox
      // 
      this.GrayscalesPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.GrayscalesPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.GrayscalesPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("GrayscalesPictureBox.Image")));
      this.GrayscalesPictureBox.Location = new System.Drawing.Point(409, 23);
      this.GrayscalesPictureBox.Name = "GrayscalesPictureBox";
      this.GrayscalesPictureBox.Size = new System.Drawing.Size(35, 312);
      this.GrayscalesPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
      this.GrayscalesPictureBox.TabIndex = 15;
      this.GrayscalesPictureBox.TabStop = false;
      this.GrayscalesPictureBox.Visible = false;
      // 
      // grayscaledImagesCheckBox
      // 
      this.grayscaledImagesCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.grayscaledImagesCheckBox.AutoSize = true;
      this.grayscaledImagesCheckBox.Location = new System.Drawing.Point(0, 338);
      this.grayscaledImagesCheckBox.Name = "grayscaledImagesCheckBox";
      this.grayscaledImagesCheckBox.Size = new System.Drawing.Size(115, 17);
      this.grayscaledImagesCheckBox.TabIndex = 16;
      this.grayscaledImagesCheckBox.Text = "Grayscaled images";
      this.grayscaledImagesCheckBox.UseVisualStyleBackColor = true;
      this.grayscaledImagesCheckBox.CheckedChanged += new System.EventHandler(this.grayscaledImagesCheckBox_CheckedChanged);
      // 
      // HeatmapView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.grayscaledImagesCheckBox);
      this.Controls.Add(this.ZeroLabel);
      this.Controls.Add(this.OneLabel);
      this.Controls.Add(this.ColorsPictureBox);
      this.Controls.Add(this.chart);
      this.Controls.Add(this.GrayscalesPictureBox);
      this.Name = "HeatmapView";
      this.Size = new System.Drawing.Size(444, 358);
      ((System.ComponentModel.ISupportInitialize)(this.chart)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ColorsPictureBox)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.GrayscalesPictureBox)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private HeuristicLab.Visualization.ChartControlsExtensions.EnhancedChart chart;
    private System.Windows.Forms.PictureBox ColorsPictureBox;
    private System.Windows.Forms.Label ZeroLabel;
    private System.Windows.Forms.Label OneLabel;
    private System.Windows.Forms.PictureBox GrayscalesPictureBox;
    private System.Windows.Forms.CheckBox grayscaledImagesCheckBox;
  }
}
