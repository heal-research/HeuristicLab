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

namespace HeuristicLab.Optimization.Views {
  partial class RunCollectionParameterAnalysisView {
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
      System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
      System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
      System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
      System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
      this.chart = new HeuristicLab.Visualization.ChartControlsExtensions.EnhancedChart();
      this.stepSizeLabel = new System.Windows.Forms.Label();
      this.stepSizeTextBox = new System.Windows.Forms.TextBox();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.logScalingCheckBox = new System.Windows.Forms.CheckBox();
      this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
      this.groupsTreeView = new System.Windows.Forms.TreeView();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.groupsGroupBox = new System.Windows.Forms.GroupBox();
      this.removeGroupButton = new System.Windows.Forms.Button();
      this.addGroupButton = new System.Windows.Forms.Button();
      this.parametersGroupBox = new System.Windows.Forms.GroupBox();
      this.parametersTreeView = new System.Windows.Forms.TreeView();
      this.dataRowsGroupBox = new System.Windows.Forms.GroupBox();
      this.minMaxCheckBox = new System.Windows.Forms.CheckBox();
      this.medianCheckBox = new System.Windows.Forms.CheckBox();
      this.quartilesCheckBox = new System.Windows.Forms.CheckBox();
      this.averageCheckBox = new System.Windows.Forms.CheckBox();
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      ((System.ComponentModel.ISupportInitialize)(this.chart)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.groupsGroupBox.SuspendLayout();
      this.parametersGroupBox.SuspendLayout();
      this.dataRowsGroupBox.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.SuspendLayout();
      // 
      // chart
      // 
      this.chart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      chartArea1.AxisX.IsStartedFromZero = false;
      chartArea1.AxisX.LabelStyle.Format = "N0";
      chartArea1.AxisX.MinorGrid.Enabled = true;
      chartArea1.AxisX.MinorGrid.LineColor = System.Drawing.Color.WhiteSmoke;
      chartArea1.AxisX.MinorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
      chartArea1.AxisX.ScaleBreakStyle.Enabled = true;
      chartArea1.AxisX.Title = "Evaluated Solutions";
      chartArea1.AxisY.LabelStyle.Format = "P0";
      chartArea1.AxisY.MinorGrid.Enabled = true;
      chartArea1.AxisY.MinorGrid.LineColor = System.Drawing.Color.WhiteSmoke;
      chartArea1.AxisY.MinorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
      chartArea1.AxisY.Title = "Relative Distance to Best Known Quality";
      chartArea1.CursorX.Interval = 0D;
      chartArea1.CursorX.IsUserSelectionEnabled = true;
      chartArea1.CursorY.Interval = 0D;
      chartArea1.CursorY.IsUserSelectionEnabled = true;
      chartArea1.Name = "chartArea";
      this.chart.ChartAreas.Add(chartArea1);
      this.chart.Cursor = System.Windows.Forms.Cursors.Default;
      legend1.Alignment = System.Drawing.StringAlignment.Center;
      legend1.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
      legend1.Name = "legend";
      this.chart.Legends.Add(legend1);
      this.chart.Location = new System.Drawing.Point(3, 0);
      this.chart.Name = "chart";
      series1.ChartArea = "chartArea";
      series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
      series1.Legend = "legend";
      series1.Name = "Series1";
      this.chart.Series.Add(series1);
      this.chart.Size = new System.Drawing.Size(735, 852);
      this.chart.TabIndex = 0;
      this.chart.Text = "chart";
      title1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      title1.Name = "title";
      title1.Text = "Title";
      this.chart.Titles.Add(title1);
      this.chart.CustomizeLegend += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.CustomizeLegendEventArgs>(this.chart_CustomizeLegend);
      this.chart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.chart_MouseDown);
      this.chart.MouseMove += new System.Windows.Forms.MouseEventHandler(this.chart_MouseMove);
      // 
      // stepSizeLabel
      // 
      this.stepSizeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.stepSizeLabel.AutoSize = true;
      this.stepSizeLabel.Location = new System.Drawing.Point(3, 831);
      this.stepSizeLabel.Name = "stepSizeLabel";
      this.stepSizeLabel.Size = new System.Drawing.Size(55, 13);
      this.stepSizeLabel.TabIndex = 1;
      this.stepSizeLabel.Text = "&Step Size:";
      // 
      // stepSizeTextBox
      // 
      this.stepSizeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.stepSizeTextBox.Location = new System.Drawing.Point(82, 828);
      this.stepSizeTextBox.Name = "stepSizeTextBox";
      this.stepSizeTextBox.Size = new System.Drawing.Size(100, 20);
      this.stepSizeTextBox.TabIndex = 2;
      this.toolTip.SetToolTip(this.stepSizeTextBox, "The step size of evaluated solutions on the x-axis.");
      this.stepSizeTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.stepSizeTextBox_KeyDown);
      this.stepSizeTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.stepSizeTextBox_Validating);
      this.stepSizeTextBox.Validated += new System.EventHandler(this.stepSizeTextBox_Validated);
      // 
      // logScalingCheckBox
      // 
      this.logScalingCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.logScalingCheckBox.AutoSize = true;
      this.logScalingCheckBox.Location = new System.Drawing.Point(206, 830);
      this.logScalingCheckBox.Name = "logScalingCheckBox";
      this.logScalingCheckBox.Size = new System.Drawing.Size(118, 17);
      this.logScalingCheckBox.TabIndex = 3;
      this.logScalingCheckBox.Text = "&Logarithmic Scaling";
      this.toolTip.SetToolTip(this.logScalingCheckBox, "Logarithmic scaling on x-axis.");
      this.logScalingCheckBox.UseVisualStyleBackColor = true;
      this.logScalingCheckBox.CheckedChanged += new System.EventHandler(this.logScalingCheckBox_CheckedChanged);
      // 
      // errorProvider
      // 
      this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
      this.errorProvider.ContainerControl = this;
      // 
      // groupsTreeView
      // 
      this.groupsTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.groupsTreeView.HideSelection = false;
      this.groupsTreeView.Location = new System.Drawing.Point(6, 49);
      this.groupsTreeView.Name = "groupsTreeView";
      this.groupsTreeView.ShowRootLines = false;
      this.groupsTreeView.Size = new System.Drawing.Size(255, 524);
      this.groupsTreeView.TabIndex = 2;
      this.groupsTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.groupsTreeView_AfterSelect);
      // 
      // splitContainer
      // 
      this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer.Location = new System.Drawing.Point(0, 0);
      this.splitContainer.Name = "splitContainer";
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.splitContainer1);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.dataRowsGroupBox);
      this.splitContainer.Panel2.Controls.Add(this.stepSizeTextBox);
      this.splitContainer.Panel2.Controls.Add(this.stepSizeLabel);
      this.splitContainer.Panel2.Controls.Add(this.logScalingCheckBox);
      this.splitContainer.Panel2.Controls.Add(this.chart);
      this.splitContainer.Size = new System.Drawing.Size(1101, 851);
      this.splitContainer.SplitterDistance = 273;
      this.splitContainer.TabIndex = 0;
      // 
      // groupsGroupBox
      // 
      this.groupsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.groupsGroupBox.Controls.Add(this.removeGroupButton);
      this.groupsGroupBox.Controls.Add(this.groupsTreeView);
      this.groupsGroupBox.Controls.Add(this.addGroupButton);
      this.groupsGroupBox.Location = new System.Drawing.Point(3, 4);
      this.groupsGroupBox.Name = "groupsGroupBox";
      this.groupsGroupBox.Size = new System.Drawing.Size(267, 579);
      this.groupsGroupBox.TabIndex = 0;
      this.groupsGroupBox.TabStop = false;
      this.groupsGroupBox.Text = "&Groups";
      // 
      // removeGroupButton
      // 
      this.removeGroupButton.Enabled = false;
      this.removeGroupButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Remove;
      this.removeGroupButton.Location = new System.Drawing.Point(36, 19);
      this.removeGroupButton.Name = "removeGroupButton";
      this.removeGroupButton.Size = new System.Drawing.Size(24, 24);
      this.removeGroupButton.TabIndex = 1;
      this.removeGroupButton.UseVisualStyleBackColor = true;
      this.removeGroupButton.Click += new System.EventHandler(this.removeGroupButton_Click);
      // 
      // addGroupButton
      // 
      this.addGroupButton.Enabled = false;
      this.addGroupButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Add;
      this.addGroupButton.Location = new System.Drawing.Point(6, 19);
      this.addGroupButton.Name = "addGroupButton";
      this.addGroupButton.Size = new System.Drawing.Size(24, 24);
      this.addGroupButton.TabIndex = 0;
      this.addGroupButton.UseVisualStyleBackColor = true;
      this.addGroupButton.Click += new System.EventHandler(this.addGroupButton_Click);
      // 
      // parametersGroupBox
      // 
      this.parametersGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.parametersGroupBox.Controls.Add(this.parametersTreeView);
      this.parametersGroupBox.Location = new System.Drawing.Point(3, 3);
      this.parametersGroupBox.Name = "parametersGroupBox";
      this.parametersGroupBox.Size = new System.Drawing.Size(267, 255);
      this.parametersGroupBox.TabIndex = 0;
      this.parametersGroupBox.TabStop = false;
      this.parametersGroupBox.Text = "&Parameters";
      // 
      // parametersTreeView
      // 
      this.parametersTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.parametersTreeView.HideSelection = false;
      this.parametersTreeView.Location = new System.Drawing.Point(6, 19);
      this.parametersTreeView.Name = "parametersTreeView";
      this.parametersTreeView.ShowRootLines = false;
      this.parametersTreeView.Size = new System.Drawing.Size(255, 230);
      this.parametersTreeView.TabIndex = 0;
      this.parametersTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.parametersTreeView_AfterSelect);
      // 
      // dataRowsGroupBox
      // 
      this.dataRowsGroupBox.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.dataRowsGroupBox.Controls.Add(this.minMaxCheckBox);
      this.dataRowsGroupBox.Controls.Add(this.medianCheckBox);
      this.dataRowsGroupBox.Controls.Add(this.quartilesCheckBox);
      this.dataRowsGroupBox.Controls.Add(this.averageCheckBox);
      this.dataRowsGroupBox.Location = new System.Drawing.Point(744, 395);
      this.dataRowsGroupBox.Name = "dataRowsGroupBox";
      this.dataRowsGroupBox.Size = new System.Drawing.Size(77, 111);
      this.dataRowsGroupBox.TabIndex = 4;
      this.dataRowsGroupBox.TabStop = false;
      this.dataRowsGroupBox.Text = "&Data Rows";
      // 
      // minMaxCheckBox
      // 
      this.minMaxCheckBox.AutoSize = true;
      this.minMaxCheckBox.Location = new System.Drawing.Point(6, 19);
      this.minMaxCheckBox.Name = "minMaxCheckBox";
      this.minMaxCheckBox.Size = new System.Drawing.Size(68, 17);
      this.minMaxCheckBox.TabIndex = 0;
      this.minMaxCheckBox.Text = "&Min/Max";
      this.minMaxCheckBox.UseVisualStyleBackColor = true;
      this.minMaxCheckBox.CheckedChanged += new System.EventHandler(this.dataRowCheckBox_CheckedChanged);
      // 
      // medianCheckBox
      // 
      this.medianCheckBox.AutoSize = true;
      this.medianCheckBox.Checked = true;
      this.medianCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
      this.medianCheckBox.Location = new System.Drawing.Point(6, 88);
      this.medianCheckBox.Name = "medianCheckBox";
      this.medianCheckBox.Size = new System.Drawing.Size(61, 17);
      this.medianCheckBox.TabIndex = 3;
      this.medianCheckBox.Text = "M&edian";
      this.medianCheckBox.UseVisualStyleBackColor = true;
      this.medianCheckBox.CheckedChanged += new System.EventHandler(this.dataRowCheckBox_CheckedChanged);
      // 
      // quartilesCheckBox
      // 
      this.quartilesCheckBox.AutoSize = true;
      this.quartilesCheckBox.Checked = true;
      this.quartilesCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
      this.quartilesCheckBox.Location = new System.Drawing.Point(6, 42);
      this.quartilesCheckBox.Name = "quartilesCheckBox";
      this.quartilesCheckBox.Size = new System.Drawing.Size(59, 17);
      this.quartilesCheckBox.TabIndex = 1;
      this.quartilesCheckBox.Text = "&Q1/Q3";
      this.quartilesCheckBox.UseVisualStyleBackColor = true;
      this.quartilesCheckBox.CheckedChanged += new System.EventHandler(this.dataRowCheckBox_CheckedChanged);
      // 
      // averageCheckBox
      // 
      this.averageCheckBox.AutoSize = true;
      this.averageCheckBox.Location = new System.Drawing.Point(6, 65);
      this.averageCheckBox.Name = "averageCheckBox";
      this.averageCheckBox.Size = new System.Drawing.Size(66, 17);
      this.averageCheckBox.TabIndex = 2;
      this.averageCheckBox.Text = "&Average";
      this.averageCheckBox.UseVisualStyleBackColor = true;
      this.averageCheckBox.CheckedChanged += new System.EventHandler(this.dataRowCheckBox_CheckedChanged);
      // 
      // splitContainer1
      // 
      this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer1.Location = new System.Drawing.Point(0, 0);
      this.splitContainer1.Name = "splitContainer1";
      this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer1.Panel1
      // 
      this.splitContainer1.Panel1.Controls.Add(this.parametersGroupBox);
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.groupsGroupBox);
      this.splitContainer1.Size = new System.Drawing.Size(273, 851);
      this.splitContainer1.SplitterDistance = 261;
      this.splitContainer1.TabIndex = 0;
      // 
      // RunCollectionParameterAnalysisView
      // 
      this.BackColor = System.Drawing.SystemColors.Window;
      this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.Controls.Add(this.splitContainer);
      this.Name = "RunCollectionParameterAnalysisView";
      this.Size = new System.Drawing.Size(1101, 851);
      ((System.ComponentModel.ISupportInitialize)(this.chart)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      this.splitContainer.Panel2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.groupsGroupBox.ResumeLayout(false);
      this.parametersGroupBox.ResumeLayout(false);
      this.dataRowsGroupBox.ResumeLayout(false);
      this.dataRowsGroupBox.PerformLayout();
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
      this.splitContainer1.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private Visualization.ChartControlsExtensions.EnhancedChart chart;
    private System.Windows.Forms.Label stepSizeLabel;
    private System.Windows.Forms.TextBox stepSizeTextBox;
    private System.Windows.Forms.ToolTip toolTip;
    private System.Windows.Forms.ErrorProvider errorProvider;
    private System.Windows.Forms.CheckBox logScalingCheckBox;
    private System.Windows.Forms.SplitContainer splitContainer;
    private System.Windows.Forms.TreeView groupsTreeView;
    private System.Windows.Forms.GroupBox parametersGroupBox;
    private System.Windows.Forms.TreeView parametersTreeView;
    private System.Windows.Forms.CheckBox medianCheckBox;
    private System.Windows.Forms.CheckBox averageCheckBox;
    private System.Windows.Forms.CheckBox quartilesCheckBox;
    private System.Windows.Forms.CheckBox minMaxCheckBox;
    private System.Windows.Forms.GroupBox dataRowsGroupBox;
    private System.Windows.Forms.GroupBox groupsGroupBox;
    private System.Windows.Forms.Button removeGroupButton;
    private System.Windows.Forms.Button addGroupButton;
    private System.Windows.Forms.SplitContainer splitContainer1;
  }
}
