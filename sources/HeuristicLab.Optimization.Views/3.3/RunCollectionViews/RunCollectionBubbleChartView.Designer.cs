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

namespace HeuristicLab.Optimization.Views {
  partial class RunCollectionBubbleChartView {
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
      System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
      System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RunCollectionBubbleChartView));
      this.yJitterLabel = new System.Windows.Forms.Label();
      this.xJitterlabel = new System.Windows.Forms.Label();
      this.xTrackBar = new System.Windows.Forms.TrackBar();
      this.xAxisLabel = new System.Windows.Forms.Label();
      this.xAxisComboBox = new System.Windows.Forms.ComboBox();
      this.yAxisLabel = new System.Windows.Forms.Label();
      this.yAxisComboBox = new System.Windows.Forms.ComboBox();
      this.yTrackBar = new System.Windows.Forms.TrackBar();
      this.sizeComboBox = new System.Windows.Forms.ComboBox();
      this.sizeLabel = new System.Windows.Forms.Label();
      this.chart = new HeuristicLab.Visualization.ChartControlsExtensions.EnhancedChart();
      this.openBoxPlotViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.hideRunToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.zoomButton = new System.Windows.Forms.RadioButton();
      this.selectButton = new System.Windows.Forms.RadioButton();
      this.radioButtonGroup = new System.Windows.Forms.GroupBox();
      this.colorButton = new System.Windows.Forms.Button();
      this.colorDialog = new System.Windows.Forms.ColorDialog();
      this.tooltip = new System.Windows.Forms.ToolTip(this.components);
      this.colorXAxisButton = new System.Windows.Forms.Button();
      this.colorYAxisButton = new System.Windows.Forms.Button();
      this.noRunsLabel = new System.Windows.Forms.Label();
      this.sizeTrackBar = new System.Windows.Forms.TrackBar();
      this.getDataAsMatrixToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      ((System.ComponentModel.ISupportInitialize)(this.xTrackBar)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.yTrackBar)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.chart)).BeginInit();
      this.radioButtonGroup.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.sizeTrackBar)).BeginInit();
      this.SuspendLayout();
      // 
      // yJitterLabel
      // 
      this.yJitterLabel.AutoSize = true;
      this.yJitterLabel.Location = new System.Drawing.Point(278, 6);
      this.yJitterLabel.Name = "yJitterLabel";
      this.yJitterLabel.Size = new System.Drawing.Size(32, 13);
      this.yJitterLabel.TabIndex = 13;
      this.yJitterLabel.Text = "Jitter:";
      // 
      // xJitterlabel
      // 
      this.xJitterlabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.xJitterlabel.AutoSize = true;
      this.xJitterlabel.Location = new System.Drawing.Point(749, 472);
      this.xJitterlabel.Name = "xJitterlabel";
      this.xJitterlabel.Size = new System.Drawing.Size(32, 13);
      this.xJitterlabel.TabIndex = 12;
      this.xJitterlabel.Text = "Jitter:";
      // 
      // xTrackBar
      // 
      this.xTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.xTrackBar.Enabled = false;
      this.xTrackBar.Location = new System.Drawing.Point(787, 469);
      this.xTrackBar.Maximum = 100;
      this.xTrackBar.Name = "xTrackBar";
      this.xTrackBar.Size = new System.Drawing.Size(64, 45);
      this.xTrackBar.TabIndex = 11;
      this.xTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
      this.xTrackBar.ValueChanged += new System.EventHandler(this.jitterTrackBar_ValueChanged);
      // 
      // xAxisLabel
      // 
      this.xAxisLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.xAxisLabel.AutoSize = true;
      this.xAxisLabel.Location = new System.Drawing.Point(471, 472);
      this.xAxisLabel.Name = "xAxisLabel";
      this.xAxisLabel.Size = new System.Drawing.Size(15, 13);
      this.xAxisLabel.TabIndex = 8;
      this.xAxisLabel.Text = "x:";
      // 
      // xAxisComboBox
      // 
      this.xAxisComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.xAxisComboBox.BackColor = System.Drawing.SystemColors.ControlLightLight;
      this.xAxisComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.xAxisComboBox.FormattingEnabled = true;
      this.xAxisComboBox.Location = new System.Drawing.Point(492, 469);
      this.xAxisComboBox.Name = "xAxisComboBox";
      this.xAxisComboBox.Size = new System.Drawing.Size(221, 21);
      this.xAxisComboBox.TabIndex = 7;
      this.xAxisComboBox.SelectedValueChanged += new System.EventHandler(this.AxisComboBox_SelectedValueChanged);
      // 
      // yAxisLabel
      // 
      this.yAxisLabel.AutoSize = true;
      this.yAxisLabel.Location = new System.Drawing.Point(3, 6);
      this.yAxisLabel.Name = "yAxisLabel";
      this.yAxisLabel.Size = new System.Drawing.Size(15, 13);
      this.yAxisLabel.TabIndex = 6;
      this.yAxisLabel.Text = "y:";
      // 
      // yAxisComboBox
      // 
      this.yAxisComboBox.BackColor = System.Drawing.SystemColors.ControlLightLight;
      this.yAxisComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.yAxisComboBox.FormattingEnabled = true;
      this.yAxisComboBox.Location = new System.Drawing.Point(24, 3);
      this.yAxisComboBox.Name = "yAxisComboBox";
      this.yAxisComboBox.Size = new System.Drawing.Size(221, 21);
      this.yAxisComboBox.TabIndex = 5;
      this.yAxisComboBox.SelectedValueChanged += new System.EventHandler(this.AxisComboBox_SelectedValueChanged);
      // 
      // yTrackBar
      // 
      this.yTrackBar.Enabled = false;
      this.yTrackBar.Location = new System.Drawing.Point(312, 3);
      this.yTrackBar.Maximum = 100;
      this.yTrackBar.Name = "yTrackBar";
      this.yTrackBar.Size = new System.Drawing.Size(59, 45);
      this.yTrackBar.TabIndex = 10;
      this.yTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
      this.yTrackBar.ValueChanged += new System.EventHandler(this.jitterTrackBar_ValueChanged);
      // 
      // sizeComboBox
      // 
      this.sizeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.sizeComboBox.BackColor = System.Drawing.SystemColors.ControlLightLight;
      this.sizeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.sizeComboBox.FormattingEnabled = true;
      this.sizeComboBox.Location = new System.Drawing.Point(560, 4);
      this.sizeComboBox.Name = "sizeComboBox";
      this.sizeComboBox.Size = new System.Drawing.Size(221, 21);
      this.sizeComboBox.TabIndex = 14;
      this.sizeComboBox.SelectedValueChanged += new System.EventHandler(this.AxisComboBox_SelectedValueChanged);
      // 
      // sizeLabel
      // 
      this.sizeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.sizeLabel.AutoSize = true;
      this.sizeLabel.Location = new System.Drawing.Point(489, 7);
      this.sizeLabel.Name = "sizeLabel";
      this.sizeLabel.Size = new System.Drawing.Size(66, 13);
      this.sizeLabel.TabIndex = 15;
      this.sizeLabel.Text = "Bubble Size:";
      // 
      // chart
      // 
      this.chart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      chartArea1.Name = "ChartArea1";
      this.chart.ChartAreas.Add(chartArea1);
      this.chart.Location = new System.Drawing.Point(6, 30);
      this.chart.Name = "chart";
      series1.ChartArea = "ChartArea1";
      series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
      series1.IsVisibleInLegend = false;
      series1.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
      series1.Name = "Bubbles";
      series1.YValuesPerPoint = 2;
      this.chart.Series.Add(series1);
      this.chart.Size = new System.Drawing.Size(843, 425);
      this.chart.TabIndex = 16;
      this.chart.Text = "chart";
      this.chart.AxisViewChanged += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.ViewEventArgs>(this.chart_AxisViewChanged);
      this.chart.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.chart_MouseDoubleClick);
      this.chart.MouseMove += new System.Windows.Forms.MouseEventHandler(this.chart_MouseMove);
      this.chart.MouseUp += new System.Windows.Forms.MouseEventHandler(this.chart_MouseUp);
      // 
      // openBoxPlotViewToolStripMenuItem
      // 
      this.openBoxPlotViewToolStripMenuItem.Name = "openBoxPlotViewToolStripMenuItem";
      this.openBoxPlotViewToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
      this.openBoxPlotViewToolStripMenuItem.Text = "Open BoxPlot View";
      this.openBoxPlotViewToolStripMenuItem.Click += new System.EventHandler(this.openBoxPlotViewToolStripMenuItem_Click);
      // 
      // hideRunToolStripMenuItem
      // 
      this.hideRunToolStripMenuItem.Name = "hideRunToolStripMenuItem";
      this.hideRunToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
      this.hideRunToolStripMenuItem.Text = "Hide Run";
      this.hideRunToolStripMenuItem.Click += new System.EventHandler(this.hideRunToolStripMenuItem_Click);
      // 
      // zoomButton
      // 
      this.zoomButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.zoomButton.AutoSize = true;
      this.zoomButton.Checked = true;
      this.zoomButton.Location = new System.Drawing.Point(6, 10);
      this.zoomButton.Name = "zoomButton";
      this.zoomButton.Size = new System.Drawing.Size(52, 17);
      this.zoomButton.TabIndex = 17;
      this.zoomButton.TabStop = true;
      this.zoomButton.Text = "Zoom";
      this.zoomButton.UseVisualStyleBackColor = true;
      this.zoomButton.CheckedChanged += new System.EventHandler(this.zoomButton_CheckedChanged);
      // 
      // selectButton
      // 
      this.selectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.selectButton.AutoSize = true;
      this.selectButton.Location = new System.Drawing.Point(64, 9);
      this.selectButton.Name = "selectButton";
      this.selectButton.Size = new System.Drawing.Size(55, 17);
      this.selectButton.TabIndex = 18;
      this.selectButton.Text = "Select";
      this.selectButton.UseVisualStyleBackColor = true;
      // 
      // radioButtonGroup
      // 
      this.radioButtonGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.radioButtonGroup.Controls.Add(this.zoomButton);
      this.radioButtonGroup.Controls.Add(this.selectButton);
      this.radioButtonGroup.Location = new System.Drawing.Point(3, 461);
      this.radioButtonGroup.Name = "radioButtonGroup";
      this.radioButtonGroup.Size = new System.Drawing.Size(135, 32);
      this.radioButtonGroup.TabIndex = 19;
      this.radioButtonGroup.TabStop = false;
      // 
      // colorButton
      // 
      this.colorButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.colorButton.Enabled = false;
      this.colorButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.colorButton.Location = new System.Drawing.Point(144, 469);
      this.colorButton.Name = "colorButton";
      this.colorButton.Size = new System.Drawing.Size(64, 21);
      this.colorButton.TabIndex = 20;
      this.colorButton.Text = "Color";
      this.colorButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.colorButton.UseVisualStyleBackColor = true;
      this.colorButton.Click += new System.EventHandler(this.colorButton_Click);
      // 
      // colorDialog
      // 
      this.colorDialog.AllowFullOpen = false;
      this.colorDialog.FullOpen = true;
      // 
      // colorXAxisButton
      // 
      this.colorXAxisButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.colorXAxisButton.Enabled = false;
      this.colorXAxisButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.DisplayInColorVertical;
      this.colorXAxisButton.Location = new System.Drawing.Point(719, 469);
      this.colorXAxisButton.Name = "colorXAxisButton";
      this.colorXAxisButton.Size = new System.Drawing.Size(21, 21);
      this.colorXAxisButton.TabIndex = 22;
      this.tooltip.SetToolTip(this.colorXAxisButton, "Color all runs according to their x-values");
      this.colorXAxisButton.UseVisualStyleBackColor = true;
      this.colorXAxisButton.Click += new System.EventHandler(this.colorXAxisButton_Click);
      // 
      // colorYAxisButton
      // 
      this.colorYAxisButton.Enabled = false;
      this.colorYAxisButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.DisplayInColor;
      this.colorYAxisButton.Location = new System.Drawing.Point(251, 3);
      this.colorYAxisButton.Name = "colorYAxisButton";
      this.colorYAxisButton.Size = new System.Drawing.Size(21, 21);
      this.colorYAxisButton.TabIndex = 23;
      this.tooltip.SetToolTip(this.colorYAxisButton, "Color all runs according to their y-values");
      this.colorYAxisButton.UseVisualStyleBackColor = true;
      this.colorYAxisButton.Click += new System.EventHandler(this.colorYAxisButton_Click);
      // 
      // noRunsLabel
      // 
      this.noRunsLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.noRunsLabel.AutoSize = true;
      this.noRunsLabel.Location = new System.Drawing.Point(366, 228);
      this.noRunsLabel.Name = "noRunsLabel";
      this.noRunsLabel.Size = new System.Drawing.Size(138, 13);
      this.noRunsLabel.TabIndex = 21;
      this.noRunsLabel.Text = "No runs could be displayed.";
      // 
      // sizeTrackBar
      // 
      this.sizeTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.sizeTrackBar.Location = new System.Drawing.Point(787, 3);
      this.sizeTrackBar.Maximum = 20;
      this.sizeTrackBar.Minimum = -20;
      this.sizeTrackBar.Name = "sizeTrackBar";
      this.sizeTrackBar.Size = new System.Drawing.Size(64, 45);
      this.sizeTrackBar.TabIndex = 24;
      this.sizeTrackBar.TickFrequency = 25;
      this.sizeTrackBar.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
      this.sizeTrackBar.ValueChanged += new System.EventHandler(this.sizeTrackBar_ValueChanged);
      // 
      // getDataAsMatrixToolStripMenuItem
      // 
      this.getDataAsMatrixToolStripMenuItem.Name = "getDataAsMatrixToolStripMenuItem";
      this.getDataAsMatrixToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
      this.getDataAsMatrixToolStripMenuItem.Text = "Get Data as Matrix";
      this.getDataAsMatrixToolStripMenuItem.Click += new System.EventHandler(this.getDataAsMatrixToolStripMenuItem_Click);
      // 
      // RunCollectionBubbleChartView
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.BackColor = System.Drawing.SystemColors.Window;
      this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.Controls.Add(this.sizeTrackBar);
      this.Controls.Add(this.colorYAxisButton);
      this.Controls.Add(this.colorXAxisButton);
      this.Controls.Add(this.noRunsLabel);
      this.Controls.Add(this.colorButton);
      this.Controls.Add(this.radioButtonGroup);
      this.Controls.Add(this.chart);
      this.Controls.Add(this.sizeLabel);
      this.Controls.Add(this.sizeComboBox);
      this.Controls.Add(this.yJitterLabel);
      this.Controls.Add(this.xJitterlabel);
      this.Controls.Add(this.xTrackBar);
      this.Controls.Add(this.xAxisLabel);
      this.Controls.Add(this.xAxisComboBox);
      this.Controls.Add(this.yAxisLabel);
      this.Controls.Add(this.yAxisComboBox);
      this.Controls.Add(this.yTrackBar);
      this.Name = "RunCollectionBubbleChartView";
      this.Size = new System.Drawing.Size(854, 496);
      ((System.ComponentModel.ISupportInitialize)(this.xTrackBar)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.yTrackBar)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.chart)).EndInit();
      this.radioButtonGroup.ResumeLayout(false);
      this.radioButtonGroup.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.sizeTrackBar)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }
    #endregion

    private System.Windows.Forms.Label xAxisLabel;
    private System.Windows.Forms.ComboBox xAxisComboBox;
    private System.Windows.Forms.Label yAxisLabel;
    private System.Windows.Forms.ComboBox yAxisComboBox;
    private System.Windows.Forms.TrackBar yTrackBar;
    private System.Windows.Forms.TrackBar xTrackBar;
    private System.Windows.Forms.Label xJitterlabel;
    private System.Windows.Forms.Label yJitterLabel;
    private System.Windows.Forms.ComboBox sizeComboBox;
    private System.Windows.Forms.Label sizeLabel;
    private HeuristicLab.Visualization.ChartControlsExtensions.EnhancedChart chart;
    private System.Windows.Forms.RadioButton zoomButton;
    private System.Windows.Forms.RadioButton selectButton;
    private System.Windows.Forms.GroupBox radioButtonGroup;
    private System.Windows.Forms.Button colorButton;
    private System.Windows.Forms.ColorDialog colorDialog;
    private System.Windows.Forms.ToolTip tooltip;
    private System.Windows.Forms.Label noRunsLabel;
    private System.Windows.Forms.ToolStripMenuItem openBoxPlotViewToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem hideRunToolStripMenuItem;
    private System.Windows.Forms.Button colorXAxisButton;
    private System.Windows.Forms.Button colorYAxisButton;
    private System.Windows.Forms.TrackBar sizeTrackBar;
    private System.Windows.Forms.ToolStripMenuItem getDataAsMatrixToolStripMenuItem;
  }
}
