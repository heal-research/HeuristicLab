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
namespace HeuristicLab.Optimization.Views {
  partial class RunCollectionRLDView {
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
      this.dataTableComboBox = new System.Windows.Forms.ComboBox();
      this.dataTableLabel = new System.Windows.Forms.Label();
      this.groupLabel = new System.Windows.Forms.Label();
      this.groupComboBox = new System.Windows.Forms.ComboBox();
      this.targetLogScalingCheckBox = new System.Windows.Forms.CheckBox();
      this.targetsTextBox = new System.Windows.Forms.TextBox();
      this.targetsLabel = new System.Windows.Forms.Label();
      this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
      this.addTargetsAsResultButton = new System.Windows.Forms.Button();
      this.generateTargetsButton = new System.Windows.Forms.Button();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.budgetsLabel = new System.Windows.Forms.Label();
      this.budgetsTextBox = new System.Windows.Forms.TextBox();
      this.addBudgetsAsResultButton = new System.Windows.Forms.Button();
      this.aggregateTargetsCheckBox = new System.Windows.Forms.CheckBox();
      this.tabControl = new System.Windows.Forms.TabControl();
      this.byTargetTabPage = new System.Windows.Forms.TabPage();
      this.relativeOrAbsoluteComboBox = new System.Windows.Forms.ComboBox();
      this.targetChart = new HeuristicLab.Visualization.ChartControlsExtensions.EnhancedChart();
      this.showLabelsCheckBox = new System.Windows.Forms.CheckBox();
      this.markerCheckBox = new System.Windows.Forms.CheckBox();
      this.boundShadingCheckBox = new System.Windows.Forms.CheckBox();
      this.byCostTabPage = new System.Windows.Forms.TabPage();
      this.byCostViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.budgetLogScalingCheckBox = new System.Windows.Forms.CheckBox();
      this.generateBudgetsButton = new System.Windows.Forms.Button();
      this.byTableTabPage = new System.Windows.Forms.TabPage();
      this.ertTableView = new HeuristicLab.Data.Views.StringConvertibleMatrixView();
      this.problemComboBox = new System.Windows.Forms.ComboBox();
      this.problemLabel = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.tabControl.SuspendLayout();
      this.byTargetTabPage.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.targetChart)).BeginInit();
      this.byCostTabPage.SuspendLayout();
      this.byTableTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // dataTableComboBox
      // 
      this.dataTableComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.dataTableComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.dataTableComboBox.FormattingEnabled = true;
      this.dataTableComboBox.Location = new System.Drawing.Point(69, 57);
      this.dataTableComboBox.Name = "dataTableComboBox";
      this.dataTableComboBox.Size = new System.Drawing.Size(582, 21);
      this.dataTableComboBox.TabIndex = 5;
      this.dataTableComboBox.SelectedIndexChanged += new System.EventHandler(this.dataTableComboBox_SelectedIndexChanged);
      // 
      // dataTableLabel
      // 
      this.dataTableLabel.AutoSize = true;
      this.dataTableLabel.Location = new System.Drawing.Point(3, 60);
      this.dataTableLabel.Name = "dataTableLabel";
      this.dataTableLabel.Size = new System.Drawing.Size(60, 13);
      this.dataTableLabel.TabIndex = 4;
      this.dataTableLabel.Text = "DataTable:";
      // 
      // groupLabel
      // 
      this.groupLabel.AutoSize = true;
      this.groupLabel.Location = new System.Drawing.Point(3, 6);
      this.groupLabel.Name = "groupLabel";
      this.groupLabel.Size = new System.Drawing.Size(39, 13);
      this.groupLabel.TabIndex = 0;
      this.groupLabel.Text = "Group:";
      // 
      // groupComboBox
      // 
      this.groupComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.groupComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.groupComboBox.FormattingEnabled = true;
      this.groupComboBox.Location = new System.Drawing.Point(69, 3);
      this.groupComboBox.Name = "groupComboBox";
      this.groupComboBox.Size = new System.Drawing.Size(582, 21);
      this.groupComboBox.TabIndex = 1;
      this.groupComboBox.SelectedIndexChanged += new System.EventHandler(this.groupComboBox_SelectedIndexChanged);
      // 
      // targetLogScalingCheckBox
      // 
      this.targetLogScalingCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.targetLogScalingCheckBox.AutoSize = true;
      this.targetLogScalingCheckBox.Location = new System.Drawing.Point(9, 364);
      this.targetLogScalingCheckBox.Name = "targetLogScalingCheckBox";
      this.targetLogScalingCheckBox.Size = new System.Drawing.Size(112, 17);
      this.targetLogScalingCheckBox.TabIndex = 6;
      this.targetLogScalingCheckBox.Text = "logarithmic scaling";
      this.targetLogScalingCheckBox.UseVisualStyleBackColor = true;
      this.targetLogScalingCheckBox.CheckedChanged += new System.EventHandler(this.logScalingCheckBox_CheckedChanged);
      // 
      // targetsTextBox
      // 
      this.targetsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.targetsTextBox.Location = new System.Drawing.Point(59, 8);
      this.targetsTextBox.Name = "targetsTextBox";
      this.targetsTextBox.Size = new System.Drawing.Size(204, 20);
      this.targetsTextBox.TabIndex = 1;
      this.toolTip.SetToolTip(this.targetsTextBox, "The order of the targets is important, first to-hit targets\r\nshould be given firs" +
        "t. The sequence should be monotonous.\r\n\r\nTargets should be separated by semicolo" +
        "n, tab or space.");
      this.targetsTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.targetsTextBox_Validating);
      // 
      // targetsLabel
      // 
      this.targetsLabel.AutoSize = true;
      this.targetsLabel.Location = new System.Drawing.Point(6, 11);
      this.targetsLabel.Name = "targetsLabel";
      this.targetsLabel.Size = new System.Drawing.Size(46, 13);
      this.targetsLabel.TabIndex = 0;
      this.targetsLabel.Text = "Targets:";
      // 
      // errorProvider
      // 
      this.errorProvider.ContainerControl = this;
      // 
      // addTargetsAsResultButton
      // 
      this.addTargetsAsResultButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.addTargetsAsResultButton.Location = new System.Drawing.Point(551, 6);
      this.addTargetsAsResultButton.Name = "addTargetsAsResultButton";
      this.addTargetsAsResultButton.Size = new System.Drawing.Size(89, 23);
      this.addTargetsAsResultButton.TabIndex = 4;
      this.addTargetsAsResultButton.Text = "Add as Result";
      this.addTargetsAsResultButton.UseVisualStyleBackColor = true;
      this.addTargetsAsResultButton.Click += new System.EventHandler(this.addTargetsAsResultButton_Click);
      // 
      // generateTargetsButton
      // 
      this.generateTargetsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.generateTargetsButton.Location = new System.Drawing.Point(456, 6);
      this.generateTargetsButton.Name = "generateTargetsButton";
      this.generateTargetsButton.Size = new System.Drawing.Size(89, 23);
      this.generateTargetsButton.TabIndex = 3;
      this.generateTargetsButton.Text = "Generate...";
      this.generateTargetsButton.UseVisualStyleBackColor = true;
      this.generateTargetsButton.Click += new System.EventHandler(this.generateTargetsButton_Click);
      // 
      // budgetsLabel
      // 
      this.budgetsLabel.AutoSize = true;
      this.budgetsLabel.Location = new System.Drawing.Point(6, 11);
      this.budgetsLabel.Name = "budgetsLabel";
      this.budgetsLabel.Size = new System.Drawing.Size(49, 13);
      this.budgetsLabel.TabIndex = 1;
      this.budgetsLabel.Text = "Budgets:";
      // 
      // budgetsTextBox
      // 
      this.budgetsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.budgetsTextBox.Location = new System.Drawing.Point(59, 8);
      this.budgetsTextBox.Name = "budgetsTextBox";
      this.budgetsTextBox.Size = new System.Drawing.Size(391, 20);
      this.budgetsTextBox.TabIndex = 6;
      this.budgetsTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.budgetsTextBox_Validating);
      // 
      // addBudgetsAsResultButton
      // 
      this.addBudgetsAsResultButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.addBudgetsAsResultButton.Location = new System.Drawing.Point(551, 6);
      this.addBudgetsAsResultButton.Name = "addBudgetsAsResultButton";
      this.addBudgetsAsResultButton.Size = new System.Drawing.Size(89, 23);
      this.addBudgetsAsResultButton.TabIndex = 7;
      this.addBudgetsAsResultButton.Text = "Add as Result";
      this.addBudgetsAsResultButton.UseVisualStyleBackColor = true;
      this.addBudgetsAsResultButton.Click += new System.EventHandler(this.addBudgetsAsResultButton_Click);
      // 
      // aggregateTargetsCheckBox
      // 
      this.aggregateTargetsCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.aggregateTargetsCheckBox.AutoSize = true;
      this.aggregateTargetsCheckBox.Checked = true;
      this.aggregateTargetsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
      this.aggregateTargetsCheckBox.Location = new System.Drawing.Point(374, 10);
      this.aggregateTargetsCheckBox.Name = "aggregateTargetsCheckBox";
      this.aggregateTargetsCheckBox.Size = new System.Drawing.Size(74, 17);
      this.aggregateTargetsCheckBox.TabIndex = 2;
      this.aggregateTargetsCheckBox.Text = "aggregate";
      this.aggregateTargetsCheckBox.UseVisualStyleBackColor = true;
      this.aggregateTargetsCheckBox.CheckedChanged += new System.EventHandler(this.aggregateTargetsCheckBox_CheckedChanged);
      // 
      // tabControl
      // 
      this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl.Controls.Add(this.byTargetTabPage);
      this.tabControl.Controls.Add(this.byCostTabPage);
      this.tabControl.Controls.Add(this.byTableTabPage);
      this.tabControl.Location = new System.Drawing.Point(0, 84);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(654, 413);
      this.tabControl.TabIndex = 6;
      // 
      // byTargetTabPage
      // 
      this.byTargetTabPage.Controls.Add(this.relativeOrAbsoluteComboBox);
      this.byTargetTabPage.Controls.Add(this.targetChart);
      this.byTargetTabPage.Controls.Add(this.showLabelsCheckBox);
      this.byTargetTabPage.Controls.Add(this.markerCheckBox);
      this.byTargetTabPage.Controls.Add(this.boundShadingCheckBox);
      this.byTargetTabPage.Controls.Add(this.targetLogScalingCheckBox);
      this.byTargetTabPage.Controls.Add(this.targetsLabel);
      this.byTargetTabPage.Controls.Add(this.aggregateTargetsCheckBox);
      this.byTargetTabPage.Controls.Add(this.targetsTextBox);
      this.byTargetTabPage.Controls.Add(this.generateTargetsButton);
      this.byTargetTabPage.Controls.Add(this.addTargetsAsResultButton);
      this.byTargetTabPage.Location = new System.Drawing.Point(4, 22);
      this.byTargetTabPage.Name = "byTargetTabPage";
      this.byTargetTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.byTargetTabPage.Size = new System.Drawing.Size(646, 387);
      this.byTargetTabPage.TabIndex = 0;
      this.byTargetTabPage.Text = "Performance by Target";
      this.byTargetTabPage.UseVisualStyleBackColor = true;
      // 
      // relativeOrAbsoluteComboBox
      // 
      this.relativeOrAbsoluteComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.relativeOrAbsoluteComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.relativeOrAbsoluteComboBox.FormattingEnabled = true;
      this.relativeOrAbsoluteComboBox.Items.AddRange(new object[] {
            "relative",
            "absolute"});
      this.relativeOrAbsoluteComboBox.Location = new System.Drawing.Point(269, 8);
      this.relativeOrAbsoluteComboBox.Name = "relativeOrAbsoluteComboBox";
      this.relativeOrAbsoluteComboBox.Size = new System.Drawing.Size(99, 21);
      this.relativeOrAbsoluteComboBox.TabIndex = 8;
      this.relativeOrAbsoluteComboBox.SelectedIndexChanged += new System.EventHandler(this.relativeOrAbsoluteComboBox_SelectedIndexChanged);
      // 
      // targetChart
      // 
      this.targetChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      chartArea2.AxisX.IsStartedFromZero = false;
      chartArea2.AxisX.MinorGrid.Enabled = true;
      chartArea2.AxisX.MinorGrid.LineColor = System.Drawing.Color.WhiteSmoke;
      chartArea2.AxisX.MinorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
      chartArea2.AxisY.Maximum = 1D;
      chartArea2.AxisY.Minimum = 0D;
      chartArea2.AxisY.MinorGrid.Enabled = true;
      chartArea2.AxisY.MinorGrid.LineColor = System.Drawing.Color.WhiteSmoke;
      chartArea2.AxisY.MinorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
      chartArea2.Name = "ChartArea1";
      this.targetChart.ChartAreas.Add(chartArea2);
      legend2.Alignment = System.Drawing.StringAlignment.Center;
      legend2.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
      legend2.Name = "Legend1";
      this.targetChart.Legends.Add(legend2);
      this.targetChart.Location = new System.Drawing.Point(6, 34);
      this.targetChart.Name = "targetChart";
      this.targetChart.Size = new System.Drawing.Size(634, 324);
      this.targetChart.SuppressExceptions = true;
      this.targetChart.TabIndex = 7;
      this.targetChart.Text = "enhancedChart1";
      this.targetChart.CustomizeLegend += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.CustomizeLegendEventArgs>(this.chart_CustomizeLegend);
      this.targetChart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.chart_MouseDown);
      this.targetChart.MouseMove += new System.Windows.Forms.MouseEventHandler(this.chart_MouseMove);
      // 
      // showLabelsCheckBox
      // 
      this.showLabelsCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.showLabelsCheckBox.AutoSize = true;
      this.showLabelsCheckBox.Checked = true;
      this.showLabelsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
      this.showLabelsCheckBox.Location = new System.Drawing.Point(329, 364);
      this.showLabelsCheckBox.Name = "showLabelsCheckBox";
      this.showLabelsCheckBox.Size = new System.Drawing.Size(81, 17);
      this.showLabelsCheckBox.TabIndex = 6;
      this.showLabelsCheckBox.Text = "show labels";
      this.showLabelsCheckBox.UseVisualStyleBackColor = true;
      this.showLabelsCheckBox.CheckedChanged += new System.EventHandler(this.showLabelsCheckBox_CheckedChanged);
      // 
      // markerCheckBox
      // 
      this.markerCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.markerCheckBox.AutoSize = true;
      this.markerCheckBox.Location = new System.Drawing.Point(229, 364);
      this.markerCheckBox.Name = "markerCheckBox";
      this.markerCheckBox.Size = new System.Drawing.Size(91, 17);
      this.markerCheckBox.TabIndex = 6;
      this.markerCheckBox.Text = "show markers";
      this.markerCheckBox.UseVisualStyleBackColor = true;
      this.markerCheckBox.CheckedChanged += new System.EventHandler(this.markerCheckBox_CheckedChanged);
      // 
      // boundShadingCheckBox
      // 
      this.boundShadingCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.boundShadingCheckBox.AutoSize = true;
      this.boundShadingCheckBox.Checked = true;
      this.boundShadingCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
      this.boundShadingCheckBox.Location = new System.Drawing.Point(127, 364);
      this.boundShadingCheckBox.Name = "boundShadingCheckBox";
      this.boundShadingCheckBox.Size = new System.Drawing.Size(96, 17);
      this.boundShadingCheckBox.TabIndex = 6;
      this.boundShadingCheckBox.Text = "bound shading";
      this.boundShadingCheckBox.UseVisualStyleBackColor = true;
      this.boundShadingCheckBox.CheckedChanged += new System.EventHandler(this.boundShadingCheckBox_CheckedChanged);
      // 
      // byCostTabPage
      // 
      this.byCostTabPage.Controls.Add(this.byCostViewHost);
      this.byCostTabPage.Controls.Add(this.budgetLogScalingCheckBox);
      this.byCostTabPage.Controls.Add(this.generateBudgetsButton);
      this.byCostTabPage.Controls.Add(this.budgetsLabel);
      this.byCostTabPage.Controls.Add(this.addBudgetsAsResultButton);
      this.byCostTabPage.Controls.Add(this.budgetsTextBox);
      this.byCostTabPage.Location = new System.Drawing.Point(4, 22);
      this.byCostTabPage.Name = "byCostTabPage";
      this.byCostTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.byCostTabPage.Size = new System.Drawing.Size(646, 387);
      this.byCostTabPage.TabIndex = 1;
      this.byCostTabPage.Text = "Performance by Cost";
      this.byCostTabPage.UseVisualStyleBackColor = true;
      // 
      // byCostViewHost
      // 
      this.byCostViewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.byCostViewHost.Caption = "View";
      this.byCostViewHost.Content = null;
      this.byCostViewHost.Enabled = false;
      this.byCostViewHost.Location = new System.Drawing.Point(6, 34);
      this.byCostViewHost.Name = "byCostViewHost";
      this.byCostViewHost.ReadOnly = false;
      this.byCostViewHost.Size = new System.Drawing.Size(634, 324);
      this.byCostViewHost.TabIndex = 12;
      this.byCostViewHost.ViewsLabelVisible = true;
      this.byCostViewHost.ViewType = null;
      // 
      // budgetLogScalingCheckBox
      // 
      this.budgetLogScalingCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.budgetLogScalingCheckBox.AutoSize = true;
      this.budgetLogScalingCheckBox.Location = new System.Drawing.Point(9, 364);
      this.budgetLogScalingCheckBox.Name = "budgetLogScalingCheckBox";
      this.budgetLogScalingCheckBox.Size = new System.Drawing.Size(112, 17);
      this.budgetLogScalingCheckBox.TabIndex = 11;
      this.budgetLogScalingCheckBox.Text = "logarithmic scaling";
      this.budgetLogScalingCheckBox.UseVisualStyleBackColor = true;
      this.budgetLogScalingCheckBox.CheckedChanged += new System.EventHandler(this.logScalingCheckBox_CheckedChanged);
      // 
      // generateBudgetsButton
      // 
      this.generateBudgetsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.generateBudgetsButton.Location = new System.Drawing.Point(456, 6);
      this.generateBudgetsButton.Name = "generateBudgetsButton";
      this.generateBudgetsButton.Size = new System.Drawing.Size(89, 23);
      this.generateBudgetsButton.TabIndex = 9;
      this.generateBudgetsButton.Text = "Generate...";
      this.generateBudgetsButton.UseVisualStyleBackColor = true;
      this.generateBudgetsButton.Click += new System.EventHandler(this.generateBudgetsButton_Click);
      // 
      // byTableTabPage
      // 
      this.byTableTabPage.Controls.Add(this.ertTableView);
      this.byTableTabPage.Location = new System.Drawing.Point(4, 22);
      this.byTableTabPage.Name = "byTableTabPage";
      this.byTableTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.byTableTabPage.Size = new System.Drawing.Size(646, 387);
      this.byTableTabPage.TabIndex = 2;
      this.byTableTabPage.Text = "Expected Runtime Tables";
      this.byTableTabPage.UseVisualStyleBackColor = true;
      // 
      // ertTableView
      // 
      this.ertTableView.Caption = "StringConvertibleMatrix View";
      this.ertTableView.Content = null;
      this.ertTableView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ertTableView.Location = new System.Drawing.Point(3, 3);
      this.ertTableView.Name = "ertTableView";
      this.ertTableView.ReadOnly = false;
      this.ertTableView.ShowRowsAndColumnsTextBox = false;
      this.ertTableView.ShowStatisticalInformation = false;
      this.ertTableView.Size = new System.Drawing.Size(640, 381);
      this.ertTableView.TabIndex = 0;
      // 
      // problemComboBox
      // 
      this.problemComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.problemComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.problemComboBox.FormattingEnabled = true;
      this.problemComboBox.Location = new System.Drawing.Point(69, 30);
      this.problemComboBox.Name = "problemComboBox";
      this.problemComboBox.Size = new System.Drawing.Size(582, 21);
      this.problemComboBox.TabIndex = 3;
      this.problemComboBox.SelectedIndexChanged += new System.EventHandler(this.problemComboBox_SelectedIndexChanged);
      // 
      // problemLabel
      // 
      this.problemLabel.AutoSize = true;
      this.problemLabel.Location = new System.Drawing.Point(3, 33);
      this.problemLabel.Name = "problemLabel";
      this.problemLabel.Size = new System.Drawing.Size(48, 13);
      this.problemLabel.TabIndex = 2;
      this.problemLabel.Text = "Problem:";
      // 
      // RunCollectionRLDView
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.tabControl);
      this.Controls.Add(this.groupComboBox);
      this.Controls.Add(this.groupLabel);
      this.Controls.Add(this.problemLabel);
      this.Controls.Add(this.dataTableLabel);
      this.Controls.Add(this.problemComboBox);
      this.Controls.Add(this.dataTableComboBox);
      this.Name = "RunCollectionRLDView";
      this.Size = new System.Drawing.Size(654, 497);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.tabControl.ResumeLayout(false);
      this.byTargetTabPage.ResumeLayout(false);
      this.byTargetTabPage.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.targetChart)).EndInit();
      this.byCostTabPage.ResumeLayout(false);
      this.byCostTabPage.PerformLayout();
      this.byTableTabPage.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ComboBox dataTableComboBox;
    private System.Windows.Forms.Label dataTableLabel;
    private System.Windows.Forms.Label groupLabel;
    private System.Windows.Forms.ComboBox groupComboBox;
    private System.Windows.Forms.CheckBox targetLogScalingCheckBox;
    private System.Windows.Forms.TextBox targetsTextBox;
    private System.Windows.Forms.Label targetsLabel;
    protected System.Windows.Forms.ErrorProvider errorProvider;
    private System.Windows.Forms.Button addTargetsAsResultButton;
    private System.Windows.Forms.Button generateTargetsButton;
    private System.Windows.Forms.ToolTip toolTip;
    private System.Windows.Forms.Button addBudgetsAsResultButton;
    private System.Windows.Forms.TextBox budgetsTextBox;
    private System.Windows.Forms.Label budgetsLabel;
    private System.Windows.Forms.CheckBox aggregateTargetsCheckBox;
    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TabPage byTargetTabPage;
    private System.Windows.Forms.TabPage byCostTabPage;
    private System.Windows.Forms.TabPage byTableTabPage;
    private System.Windows.Forms.Button generateBudgetsButton;
    private System.Windows.Forms.CheckBox budgetLogScalingCheckBox;
    private System.Windows.Forms.Label problemLabel;
    private System.Windows.Forms.ComboBox problemComboBox;
    private Data.Views.StringConvertibleMatrixView ertTableView;
    private Visualization.ChartControlsExtensions.EnhancedChart targetChart;
    private MainForm.WindowsForms.ViewHost byCostViewHost;
    private System.Windows.Forms.CheckBox boundShadingCheckBox;
    private System.Windows.Forms.CheckBox markerCheckBox;
    private System.Windows.Forms.ComboBox relativeOrAbsoluteComboBox;
    private System.Windows.Forms.CheckBox showLabelsCheckBox;
  }
}
