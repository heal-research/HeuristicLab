#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
      this.hideRunsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.zoomButton = new System.Windows.Forms.RadioButton();
      this.selectButton = new System.Windows.Forms.RadioButton();
      this.radioButtonGroup = new System.Windows.Forms.GroupBox();
      this.colorRunsButton = new System.Windows.Forms.Button();
      this.colorDialog = new System.Windows.Forms.ColorDialog();
      this.tooltip = new System.Windows.Forms.ToolTip(this.components);
      this.colorXAxisButton = new System.Windows.Forms.Button();
      this.colorYAxisButton = new System.Windows.Forms.Button();
      this.transparencyTrackBar = new System.Windows.Forms.TrackBar();
      this.hideRunsButton = new System.Windows.Forms.Button();
      this.colorDialogButton = new System.Windows.Forms.Button();
      this.noRunsLabel = new System.Windows.Forms.Label();
      this.sizeTrackBar = new System.Windows.Forms.TrackBar();
      this.getDataAsMatrixToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.transparencyLabel = new System.Windows.Forms.Label();
      this.unhideAllRunToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.colorResetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      ((System.ComponentModel.ISupportInitialize)(this.xTrackBar)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.yTrackBar)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.chart)).BeginInit();
      this.radioButtonGroup.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.transparencyTrackBar)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.sizeTrackBar)).BeginInit();
      this.SuspendLayout();
      // 
      // yJitterLabel
      // 
      this.yJitterLabel.AutoSize = true;
      this.yJitterLabel.Location = new System.Drawing.Point(457, 6);
      this.yJitterLabel.Name = "yJitterLabel";
      this.yJitterLabel.Size = new System.Drawing.Size(32, 13);
      this.yJitterLabel.TabIndex = 13;
      this.yJitterLabel.Text = "Jitter:";
      // 
      // xJitterlabel
      // 
      this.xJitterlabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.xJitterlabel.AutoSize = true;
      this.xJitterlabel.Location = new System.Drawing.Point(893, 719);
      this.xJitterlabel.Name = "xJitterlabel";
      this.xJitterlabel.Size = new System.Drawing.Size(32, 13);
      this.xJitterlabel.TabIndex = 12;
      this.xJitterlabel.Text = "Jitter:";
      // 
      // xTrackBar
      // 
      this.xTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.xTrackBar.Enabled = false;
      this.xTrackBar.Location = new System.Drawing.Point(919, 715);
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
      this.xAxisLabel.Location = new System.Drawing.Point(439, 719);
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
      this.xAxisComboBox.DropDownWidth = 500;
      this.xAxisComboBox.FormattingEnabled = true;
      this.xAxisComboBox.Location = new System.Drawing.Point(460, 715);
      this.xAxisComboBox.Name = "xAxisComboBox";
      this.xAxisComboBox.Size = new System.Drawing.Size(400, 21);
      this.xAxisComboBox.TabIndex = 7;
      this.xAxisComboBox.Sorted = false;
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
      this.yAxisComboBox.DropDownWidth = 800;
      this.yAxisComboBox.FormattingEnabled = true;
      this.yAxisComboBox.Location = new System.Drawing.Point(24, 3);
      this.yAxisComboBox.Name = "yAxisComboBox";
      this.yAxisComboBox.Size = new System.Drawing.Size(400, 21);
      this.yAxisComboBox.TabIndex = 5;
      this.yAxisComboBox.Sorted = false;
      this.yAxisComboBox.SelectedValueChanged += new System.EventHandler(this.AxisComboBox_SelectedValueChanged);
      // 
      // yTrackBar
      // 
      this.yTrackBar.Enabled = false;
      this.yTrackBar.Location = new System.Drawing.Point(483, 3);
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
      this.sizeComboBox.DropDownWidth = 400;
      this.sizeComboBox.FormattingEnabled = true;
      this.sizeComboBox.Location = new System.Drawing.Point(613, 3);
      this.sizeComboBox.Name = "sizeComboBox";
      this.sizeComboBox.Size = new System.Drawing.Size(300, 21);
      this.sizeComboBox.Sorted = false;
      this.sizeComboBox.TabIndex = 14;
      this.sizeComboBox.SelectedValueChanged += new System.EventHandler(this.AxisComboBox_SelectedValueChanged);
      // 
      // sizeLabel
      // 
      this.sizeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.sizeLabel.AutoSize = true;
      this.sizeLabel.Location = new System.Drawing.Point(541, 6);
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
      this.chart.Location = new System.Drawing.Point(-1, 30);
      this.chart.Name = "chart";
      series1.ChartArea = "ChartArea1";
      series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
      series1.IsVisibleInLegend = false;
      series1.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
      series1.Name = "Bubbles";
      series1.YValuesPerPoint = 2;
      this.chart.Series.Add(series1);
      this.chart.Size = new System.Drawing.Size(988, 681);
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
      // hideRunsToolStripMenuItem
      // 
      this.hideRunsToolStripMenuItem.Name = "hideRunsToolStripMenuItem";
      this.hideRunsToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
      this.hideRunsToolStripMenuItem.Text = "Hide selected Runs";
      this.hideRunsToolStripMenuItem.Click += new System.EventHandler(this.hideRunsToolStripMenuItem_Click);
      // 
      // zoomButton
      // 
      this.zoomButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.zoomButton.AutoSize = true;
      this.zoomButton.Checked = true;
      this.zoomButton.Location = new System.Drawing.Point(6, 11);
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
      this.selectButton.Location = new System.Drawing.Point(61, 11);
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
      this.radioButtonGroup.Location = new System.Drawing.Point(6, 679);
      this.radioButtonGroup.Name = "radioButtonGroup";
      this.radioButtonGroup.Size = new System.Drawing.Size(122, 32);
      this.radioButtonGroup.TabIndex = 19;
      this.radioButtonGroup.TabStop = false;
      // 
      // colorRunsButton
      // 
      this.colorRunsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.colorRunsButton.Enabled = false;
      this.colorRunsButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.colorRunsButton.Location = new System.Drawing.Point(6, 715);
      this.colorRunsButton.Name = "colorRunsButton";
      this.colorRunsButton.Size = new System.Drawing.Size(21, 21);
      this.colorRunsButton.TabIndex = 20;
      this.tooltip.SetToolTip(this.colorRunsButton, "Color all selected runs");
      this.colorRunsButton.UseVisualStyleBackColor = true;
      this.colorRunsButton.Click += new System.EventHandler(this.colorRunsButton_Click);
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
      this.colorXAxisButton.Location = new System.Drawing.Point(866, 715);
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
      this.colorYAxisButton.Location = new System.Drawing.Point(430, 3);
      this.colorYAxisButton.Name = "colorYAxisButton";
      this.colorYAxisButton.Size = new System.Drawing.Size(21, 21);
      this.colorYAxisButton.TabIndex = 23;
      this.tooltip.SetToolTip(this.colorYAxisButton, "Color all runs according to their y-values");
      this.colorYAxisButton.UseVisualStyleBackColor = true;
      this.colorYAxisButton.Click += new System.EventHandler(this.colorYAxisButton_Click);
      // 
      // transparencyTrackBar
      // 
      this.transparencyTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.transparencyTrackBar.LargeChange = 16;
      this.transparencyTrackBar.Location = new System.Drawing.Point(177, 717);
      this.transparencyTrackBar.Maximum = 254;
      this.transparencyTrackBar.Name = "transparencyTrackBar";
      this.transparencyTrackBar.Size = new System.Drawing.Size(64, 45);
      this.transparencyTrackBar.TabIndex = 24;
      this.transparencyTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
      this.tooltip.SetToolTip(this.transparencyTrackBar, "Sets the transparency of the colors");
      this.transparencyTrackBar.ValueChanged += new System.EventHandler(this.transparencyTrackBar_ValueChanged);
      // 
      // hideRunsButton
      // 
      this.hideRunsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.hideRunsButton.Enabled = false;
      this.hideRunsButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.hideRunsButton.Location = new System.Drawing.Point(47, 715);
      this.hideRunsButton.Name = "hideRunsButton";
      this.hideRunsButton.Size = new System.Drawing.Size(43, 21);
      this.hideRunsButton.TabIndex = 26;
      this.hideRunsButton.Text = "Hide";
      this.tooltip.SetToolTip(this.hideRunsButton, "Hides all selected runs");
      this.hideRunsButton.UseVisualStyleBackColor = true;
      this.hideRunsButton.Click += new System.EventHandler(this.hideRunsButton_Click);
      // 
      // colorDialogButton
      // 
      this.colorDialogButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.colorDialogButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.colorDialogButton.Enabled = false;
      this.colorDialogButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.colorDialogButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.colorDialogButton.Location = new System.Drawing.Point(27, 715);
      this.colorDialogButton.Name = "colorDialogButton";
      this.colorDialogButton.Size = new System.Drawing.Size(14, 21);
      this.colorDialogButton.TabIndex = 25;
      this.colorDialogButton.Text = "v";
      this.tooltip.SetToolTip(this.colorDialogButton, "Choose color");
      this.colorDialogButton.UseVisualStyleBackColor = true;
      this.colorDialogButton.Click += new System.EventHandler(this.colorDialogButton_Click);
      // 
      // noRunsLabel
      // 
      this.noRunsLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.noRunsLabel.AutoSize = true;
      this.noRunsLabel.Location = new System.Drawing.Point(432, 350);
      this.noRunsLabel.Name = "noRunsLabel";
      this.noRunsLabel.Size = new System.Drawing.Size(138, 13);
      this.noRunsLabel.TabIndex = 21;
      this.noRunsLabel.Text = "No runs could be displayed.";
      // 
      // sizeTrackBar
      // 
      this.sizeTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.sizeTrackBar.Location = new System.Drawing.Point(919, 3);
      this.sizeTrackBar.Maximum = 20;
      this.sizeTrackBar.Minimum = -20;
      this.sizeTrackBar.Name = "sizeTrackBar";
      this.sizeTrackBar.Size = new System.Drawing.Size(64, 45);
      this.sizeTrackBar.TabIndex = 24;
      this.sizeTrackBar.TickFrequency = 20;
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
      // transparencyLabel
      // 
      this.transparencyLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.transparencyLabel.AutoSize = true;
      this.transparencyLabel.Location = new System.Drawing.Point(108, 719);
      this.transparencyLabel.Name = "transparencyLabel";
      this.transparencyLabel.Size = new System.Drawing.Size(75, 13);
      this.transparencyLabel.TabIndex = 12;
      this.transparencyLabel.Text = "Transparency:";
      // 
      // unhideAllRunToolStripMenuItem
      // 
      this.unhideAllRunToolStripMenuItem.Name = "unhideAllRunToolStripMenuItem";
      this.unhideAllRunToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
      this.unhideAllRunToolStripMenuItem.Text = "Unhide all";
      this.unhideAllRunToolStripMenuItem.Click += new System.EventHandler(this.unhideAllRunToolStripMenuItem_Click);
      // 
      // colorResetToolStripMenuItem
      // 
      this.colorResetToolStripMenuItem.Name = "colorResetToolStripMenuItem";
      this.colorResetToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
      this.colorResetToolStripMenuItem.Text = "Color reset";
      this.colorResetToolStripMenuItem.Click += new System.EventHandler(this.colorResetToolStripMenuItem_Click);
      // 
      // RunCollectionBubbleChartView
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.BackColor = System.Drawing.SystemColors.Window;
      this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.Controls.Add(this.hideRunsButton);
      this.Controls.Add(this.sizeTrackBar);
      this.Controls.Add(this.colorYAxisButton);
      this.Controls.Add(this.colorXAxisButton);
      this.Controls.Add(this.noRunsLabel);
      this.Controls.Add(this.colorRunsButton);
      this.Controls.Add(this.radioButtonGroup);
      this.Controls.Add(this.yJitterLabel);
      this.Controls.Add(this.transparencyLabel);
      this.Controls.Add(this.xJitterlabel);
      this.Controls.Add(this.xTrackBar);
      this.Controls.Add(this.xAxisLabel);
      this.Controls.Add(this.xAxisComboBox);
      this.Controls.Add(this.yAxisLabel);
      this.Controls.Add(this.yAxisComboBox);
      this.Controls.Add(this.yTrackBar);
      this.Controls.Add(this.colorDialogButton);
      this.Controls.Add(this.chart);
      this.Controls.Add(this.transparencyTrackBar);
      this.Controls.Add(this.sizeLabel);
      this.Controls.Add(this.sizeComboBox);
      this.Name = "RunCollectionBubbleChartView";
      this.Size = new System.Drawing.Size(986, 741);
      ((System.ComponentModel.ISupportInitialize)(this.xTrackBar)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.yTrackBar)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.chart)).EndInit();
      this.radioButtonGroup.ResumeLayout(false);
      this.radioButtonGroup.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.transparencyTrackBar)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.sizeTrackBar)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }
    #endregion

    private System.Windows.Forms.Label xAxisLabel;
    internal System.Windows.Forms.ComboBox xAxisComboBox;
    private System.Windows.Forms.Label yAxisLabel;
    internal System.Windows.Forms.ComboBox yAxisComboBox;
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
    private System.Windows.Forms.Button colorRunsButton;
    private System.Windows.Forms.ColorDialog colorDialog;
    private System.Windows.Forms.ToolTip tooltip;
    private System.Windows.Forms.Label noRunsLabel;
    private System.Windows.Forms.ToolStripMenuItem openBoxPlotViewToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem hideRunsToolStripMenuItem;
    private System.Windows.Forms.Button colorXAxisButton;
    private System.Windows.Forms.Button colorYAxisButton;
    private System.Windows.Forms.TrackBar sizeTrackBar;
    private System.Windows.Forms.ToolStripMenuItem getDataAsMatrixToolStripMenuItem;
    private System.Windows.Forms.TrackBar transparencyTrackBar;
    private System.Windows.Forms.Label transparencyLabel;
    private System.Windows.Forms.Button hideRunsButton;
    private System.Windows.Forms.ToolStripMenuItem unhideAllRunToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem colorResetToolStripMenuItem;
    private System.Windows.Forms.Button colorDialogButton;
  }
}
