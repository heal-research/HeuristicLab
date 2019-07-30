#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  partial class ScatterPlotMultiView {
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
      this.frameTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
      this.columnHeaderScrollPanel = new System.Windows.Forms.Panel();
      this.columnHeaderTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
      this.rowHeaderScrollPanel = new System.Windows.Forms.Panel();
      this.rowHeaderTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
      this.bodyScrollPanel = new System.Windows.Forms.Panel();
      this.bodyTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
      this.sizeGroupBox = new System.Windows.Forms.GroupBox();
      this.heightNumericUpDown = new System.Windows.Forms.NumericUpDown();
      this.widthNumericUpDown = new System.Windows.Forms.NumericUpDown();
      this.heightLabel = new System.Windows.Forms.Label();
      this.widthLabel = new System.Windows.Forms.Label();
      this.lockAspectCheckBox = new System.Windows.Forms.CheckBox();
      this.pointSizeNumericUpDown = new System.Windows.Forms.NumericUpDown();
      this.pointSizeLabel = new System.Windows.Forms.Label();
      this.regressionGroupBox = new System.Windows.Forms.GroupBox();
      this.regressionTypeComboBox = new System.Windows.Forms.ComboBox();
      this.polynomialRegressionOrderNumericUpDown = new System.Windows.Forms.NumericUpDown();
      this.orderLabel = new System.Windows.Forms.Label();
      this.regressionTyleLabel = new System.Windows.Forms.Label();
      this.groupingOptionsBox = new System.Windows.Forms.GroupBox();
      this.legendGroupBox = new System.Windows.Forms.GroupBox();
      this.legendCheckbox = new System.Windows.Forms.CheckBox();
      this.legendOrderComboBox = new System.Windows.Forms.ComboBox();
      this.legendOrderLabel = new System.Windows.Forms.Label();
      this.aggregationLabel = new System.Windows.Forms.Label();
      this.aggregationComboBox = new System.Windows.Forms.ComboBox();
      this.groupingComboBox = new System.Windows.Forms.ComboBox();
      this.opacityLabel = new System.Windows.Forms.Label();
      this.pointOpacityNumericUpDown = new System.Windows.Forms.NumericUpDown();
      this.pointsGroupBox = new System.Windows.Forms.GroupBox();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.frameTableLayoutPanel.SuspendLayout();
      this.columnHeaderScrollPanel.SuspendLayout();
      this.rowHeaderScrollPanel.SuspendLayout();
      this.bodyScrollPanel.SuspendLayout();
      this.sizeGroupBox.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.heightNumericUpDown)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.widthNumericUpDown)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.pointSizeNumericUpDown)).BeginInit();
      this.regressionGroupBox.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.polynomialRegressionOrderNumericUpDown)).BeginInit();
      this.groupingOptionsBox.SuspendLayout();
      this.legendGroupBox.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pointOpacityNumericUpDown)).BeginInit();
      this.pointsGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // splitContainer
      // 
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.sizeGroupBox);
      this.splitContainer.Panel1.Controls.Add(this.pointsGroupBox);
      this.splitContainer.Panel1.Controls.Add(this.groupingOptionsBox);
      this.splitContainer.Panel1.Controls.Add(this.regressionGroupBox);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.frameTableLayoutPanel);
      this.splitContainer.Size = new System.Drawing.Size(589, 451);
      // 
      // frameTableLayoutPanel
      // 
      this.frameTableLayoutPanel.ColumnCount = 2;
      this.frameTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
      this.frameTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.frameTableLayoutPanel.Controls.Add(this.columnHeaderScrollPanel, 1, 0);
      this.frameTableLayoutPanel.Controls.Add(this.rowHeaderScrollPanel, 0, 1);
      this.frameTableLayoutPanel.Controls.Add(this.bodyScrollPanel, 1, 1);
      this.frameTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.frameTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
      this.frameTableLayoutPanel.Name = "frameTableLayoutPanel";
      this.frameTableLayoutPanel.RowCount = 2;
      this.frameTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
      this.frameTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.frameTableLayoutPanel.Size = new System.Drawing.Size(405, 451);
      this.frameTableLayoutPanel.TabIndex = 0;
      // 
      // columnHeaderScrollPanel
      // 
      this.columnHeaderScrollPanel.Controls.Add(this.columnHeaderTableLayoutPanel);
      this.columnHeaderScrollPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.columnHeaderScrollPanel.Location = new System.Drawing.Point(40, 0);
      this.columnHeaderScrollPanel.Margin = new System.Windows.Forms.Padding(0);
      this.columnHeaderScrollPanel.Name = "columnHeaderScrollPanel";
      this.columnHeaderScrollPanel.Size = new System.Drawing.Size(365, 40);
      this.columnHeaderScrollPanel.TabIndex = 3;
      // 
      // columnHeaderTableLayoutPanel
      // 
      this.columnHeaderTableLayoutPanel.AutoSize = true;
      this.columnHeaderTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.columnHeaderTableLayoutPanel.ColumnCount = 2;
      this.columnHeaderTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.columnHeaderTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.columnHeaderTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Left;
      this.columnHeaderTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
      this.columnHeaderTableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
      this.columnHeaderTableLayoutPanel.Name = "columnHeaderTableLayoutPanel";
      this.columnHeaderTableLayoutPanel.RowCount = 1;
      this.columnHeaderTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.columnHeaderTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.columnHeaderTableLayoutPanel.Size = new System.Drawing.Size(0, 40);
      this.columnHeaderTableLayoutPanel.TabIndex = 1;
      // 
      // rowHeaderScrollPanel
      // 
      this.rowHeaderScrollPanel.Controls.Add(this.rowHeaderTableLayoutPanel);
      this.rowHeaderScrollPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.rowHeaderScrollPanel.Location = new System.Drawing.Point(0, 40);
      this.rowHeaderScrollPanel.Margin = new System.Windows.Forms.Padding(0);
      this.rowHeaderScrollPanel.Name = "rowHeaderScrollPanel";
      this.rowHeaderScrollPanel.Size = new System.Drawing.Size(40, 411);
      this.rowHeaderScrollPanel.TabIndex = 4;
      // 
      // rowHeaderTableLayoutPanel
      // 
      this.rowHeaderTableLayoutPanel.AutoSize = true;
      this.rowHeaderTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.rowHeaderTableLayoutPanel.ColumnCount = 1;
      this.rowHeaderTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.rowHeaderTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.rowHeaderTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Top;
      this.rowHeaderTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
      this.rowHeaderTableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
      this.rowHeaderTableLayoutPanel.Name = "rowHeaderTableLayoutPanel";
      this.rowHeaderTableLayoutPanel.RowCount = 2;
      this.rowHeaderTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.rowHeaderTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.rowHeaderTableLayoutPanel.Size = new System.Drawing.Size(40, 0);
      this.rowHeaderTableLayoutPanel.TabIndex = 2;
      // 
      // bodyScrollPanel
      // 
      this.bodyScrollPanel.AutoScroll = true;
      this.bodyScrollPanel.Controls.Add(this.bodyTableLayoutPanel);
      this.bodyScrollPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.bodyScrollPanel.Location = new System.Drawing.Point(40, 40);
      this.bodyScrollPanel.Margin = new System.Windows.Forms.Padding(0);
      this.bodyScrollPanel.Name = "bodyScrollPanel";
      this.bodyScrollPanel.Size = new System.Drawing.Size(365, 411);
      this.bodyScrollPanel.TabIndex = 5;
      this.bodyScrollPanel.Scroll += new System.Windows.Forms.ScrollEventHandler(this.bodyScrollPanel_Scroll);
      // 
      // bodyTableLayoutPanel
      // 
      this.bodyTableLayoutPanel.AutoSize = true;
      this.bodyTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.bodyTableLayoutPanel.ColumnCount = 2;
      this.bodyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.bodyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.bodyTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
      this.bodyTableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
      this.bodyTableLayoutPanel.Name = "bodyTableLayoutPanel";
      this.bodyTableLayoutPanel.RowCount = 2;
      this.bodyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.bodyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.bodyTableLayoutPanel.Size = new System.Drawing.Size(0, 0);
      this.bodyTableLayoutPanel.TabIndex = 0;
      // 
      // sizeGroupBox
      // 
      this.sizeGroupBox.Controls.Add(this.heightNumericUpDown);
      this.sizeGroupBox.Controls.Add(this.widthNumericUpDown);
      this.sizeGroupBox.Controls.Add(this.heightLabel);
      this.sizeGroupBox.Controls.Add(this.widthLabel);
      this.sizeGroupBox.Controls.Add(this.lockAspectCheckBox);
      this.sizeGroupBox.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.sizeGroupBox.Location = new System.Drawing.Point(0, 78);
      this.sizeGroupBox.Name = "sizeGroupBox";
      this.sizeGroupBox.Size = new System.Drawing.Size(180, 98);
      this.sizeGroupBox.TabIndex = 5;
      this.sizeGroupBox.TabStop = false;
      this.sizeGroupBox.Text = "Chart Size";
      // 
      // heightNumericUpDown
      // 
      this.heightNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.heightNumericUpDown.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
      this.heightNumericUpDown.Location = new System.Drawing.Point(50, 46);
      this.heightNumericUpDown.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
      this.heightNumericUpDown.Name = "heightNumericUpDown";
      this.heightNumericUpDown.Size = new System.Drawing.Size(124, 20);
      this.heightNumericUpDown.TabIndex = 4;
      this.heightNumericUpDown.Value = new decimal(new int[] {
            225,
            0,
            0,
            0});
      this.heightNumericUpDown.ValueChanged += new System.EventHandler(this.heightNumericUpDown_ValueChanged);
      // 
      // widthNumericUpDown
      // 
      this.widthNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.widthNumericUpDown.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
      this.widthNumericUpDown.Location = new System.Drawing.Point(50, 20);
      this.widthNumericUpDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
      this.widthNumericUpDown.Name = "widthNumericUpDown";
      this.widthNumericUpDown.Size = new System.Drawing.Size(124, 20);
      this.widthNumericUpDown.TabIndex = 3;
      this.widthNumericUpDown.Value = new decimal(new int[] {
            300,
            0,
            0,
            0});
      this.widthNumericUpDown.ValueChanged += new System.EventHandler(this.widthNumericUpDown_ValueChanged);
      // 
      // heightLabel
      // 
      this.heightLabel.AutoSize = true;
      this.heightLabel.Location = new System.Drawing.Point(6, 48);
      this.heightLabel.Name = "heightLabel";
      this.heightLabel.Size = new System.Drawing.Size(41, 13);
      this.heightLabel.TabIndex = 2;
      this.heightLabel.Text = "Height:";
      // 
      // widthLabel
      // 
      this.widthLabel.AutoSize = true;
      this.widthLabel.Location = new System.Drawing.Point(6, 22);
      this.widthLabel.Name = "widthLabel";
      this.widthLabel.Size = new System.Drawing.Size(38, 13);
      this.widthLabel.TabIndex = 1;
      this.widthLabel.Text = "Width:";
      // 
      // lockAspectCheckBox
      // 
      this.lockAspectCheckBox.AutoSize = true;
      this.lockAspectCheckBox.Checked = true;
      this.lockAspectCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
      this.lockAspectCheckBox.Location = new System.Drawing.Point(50, 73);
      this.lockAspectCheckBox.Name = "lockAspectCheckBox";
      this.lockAspectCheckBox.Size = new System.Drawing.Size(114, 17);
      this.lockAspectCheckBox.TabIndex = 6;
      this.lockAspectCheckBox.Text = "Lock Aspect Ratio";
      this.lockAspectCheckBox.UseVisualStyleBackColor = true;
      // 
      // pointSizeNumericUpDown
      // 
      this.pointSizeNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pointSizeNumericUpDown.Location = new System.Drawing.Point(58, 15);
      this.pointSizeNumericUpDown.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
      this.pointSizeNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.pointSizeNumericUpDown.Name = "pointSizeNumericUpDown";
      this.pointSizeNumericUpDown.Size = new System.Drawing.Size(116, 20);
      this.pointSizeNumericUpDown.TabIndex = 8;
      this.pointSizeNumericUpDown.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
      this.pointSizeNumericUpDown.ValueChanged += new System.EventHandler(this.pointSizeNumericUpDown_ValueChanged);
      // 
      // pointSizeLabel
      // 
      this.pointSizeLabel.AutoSize = true;
      this.pointSizeLabel.Location = new System.Drawing.Point(6, 17);
      this.pointSizeLabel.Name = "pointSizeLabel";
      this.pointSizeLabel.Size = new System.Drawing.Size(30, 13);
      this.pointSizeLabel.TabIndex = 7;
      this.pointSizeLabel.Text = "Size:";
      // 
      // regressionGroupBox
      // 
      this.regressionGroupBox.Controls.Add(this.regressionTypeComboBox);
      this.regressionGroupBox.Controls.Add(this.polynomialRegressionOrderNumericUpDown);
      this.regressionGroupBox.Controls.Add(this.orderLabel);
      this.regressionGroupBox.Controls.Add(this.regressionTyleLabel);
      this.regressionGroupBox.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.regressionGroupBox.Location = new System.Drawing.Point(0, 376);
      this.regressionGroupBox.Name = "regressionGroupBox";
      this.regressionGroupBox.Size = new System.Drawing.Size(180, 75);
      this.regressionGroupBox.TabIndex = 3;
      this.regressionGroupBox.TabStop = false;
      this.regressionGroupBox.Text = "Regression";
      // 
      // regressionTypeComboBox
      // 
      this.regressionTypeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.regressionTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.regressionTypeComboBox.FormattingEnabled = true;
      this.regressionTypeComboBox.Location = new System.Drawing.Point(67, 19);
      this.regressionTypeComboBox.Name = "regressionTypeComboBox";
      this.regressionTypeComboBox.Size = new System.Drawing.Size(107, 21);
      this.regressionTypeComboBox.TabIndex = 14;
      this.regressionTypeComboBox.SelectedValueChanged += new System.EventHandler(this.regressionTypeComboBox_SelectedValueChanged);
      // 
      // polynomialRegressionOrderNumericUpDown
      // 
      this.polynomialRegressionOrderNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.polynomialRegressionOrderNumericUpDown.Location = new System.Drawing.Point(67, 45);
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
      this.polynomialRegressionOrderNumericUpDown.Size = new System.Drawing.Size(107, 20);
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
      this.orderLabel.Location = new System.Drawing.Point(6, 47);
      this.orderLabel.Name = "orderLabel";
      this.orderLabel.Size = new System.Drawing.Size(36, 13);
      this.orderLabel.TabIndex = 16;
      this.orderLabel.Text = "Order:";
      // 
      // regressionTyleLabel
      // 
      this.regressionTyleLabel.AutoSize = true;
      this.regressionTyleLabel.Location = new System.Drawing.Point(6, 22);
      this.regressionTyleLabel.Name = "regressionTyleLabel";
      this.regressionTyleLabel.Size = new System.Drawing.Size(34, 13);
      this.regressionTyleLabel.TabIndex = 13;
      this.regressionTyleLabel.Text = "Type:";
      // 
      // groupingOptionsBox
      // 
      this.groupingOptionsBox.Controls.Add(this.legendGroupBox);
      this.groupingOptionsBox.Controls.Add(this.aggregationLabel);
      this.groupingOptionsBox.Controls.Add(this.aggregationComboBox);
      this.groupingOptionsBox.Controls.Add(this.groupingComboBox);
      this.groupingOptionsBox.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.groupingOptionsBox.Location = new System.Drawing.Point(0, 245);
      this.groupingOptionsBox.Name = "groupingOptionsBox";
      this.groupingOptionsBox.Size = new System.Drawing.Size(180, 131);
      this.groupingOptionsBox.TabIndex = 8;
      this.groupingOptionsBox.TabStop = false;
      this.groupingOptionsBox.Text = "Grouping";
      // 
      // legendGroupBox
      // 
      this.legendGroupBox.Controls.Add(this.legendCheckbox);
      this.legendGroupBox.Controls.Add(this.legendOrderComboBox);
      this.legendGroupBox.Controls.Add(this.legendOrderLabel);
      this.legendGroupBox.Enabled = false;
      this.legendGroupBox.Location = new System.Drawing.Point(6, 73);
      this.legendGroupBox.Name = "legendGroupBox";
      this.legendGroupBox.Size = new System.Drawing.Size(167, 50);
      this.legendGroupBox.TabIndex = 7;
      this.legendGroupBox.TabStop = false;
      this.legendGroupBox.Text = "Legend";
      // 
      // legendCheckbox
      // 
      this.legendCheckbox.AutoSize = true;
      this.legendCheckbox.Checked = true;
      this.legendCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
      this.legendCheckbox.Location = new System.Drawing.Point(56, -1);
      this.legendCheckbox.Name = "legendCheckbox";
      this.legendCheckbox.Size = new System.Drawing.Size(56, 17);
      this.legendCheckbox.TabIndex = 0;
      this.legendCheckbox.Text = "Visible";
      this.legendCheckbox.UseVisualStyleBackColor = true;
      this.legendCheckbox.CheckedChanged += new System.EventHandler(this.legendCheckbox_CheckedChanged);
      // 
      // legendOrderComboBox
      // 
      this.legendOrderComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.legendOrderComboBox.FormattingEnabled = true;
      this.legendOrderComboBox.Location = new System.Drawing.Point(49, 19);
      this.legendOrderComboBox.Name = "legendOrderComboBox";
      this.legendOrderComboBox.Size = new System.Drawing.Size(109, 21);
      this.legendOrderComboBox.TabIndex = 5;
      this.legendOrderComboBox.SelectedIndexChanged += new System.EventHandler(this.legendOrderComboBox_SelectedIndexChanged);
      // 
      // legendOrderLabel
      // 
      this.legendOrderLabel.AutoSize = true;
      this.legendOrderLabel.Location = new System.Drawing.Point(7, 22);
      this.legendOrderLabel.Name = "legendOrderLabel";
      this.legendOrderLabel.Size = new System.Drawing.Size(36, 13);
      this.legendOrderLabel.TabIndex = 4;
      this.legendOrderLabel.Text = "Order:";
      this.legendOrderLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // aggregationLabel
      // 
      this.aggregationLabel.AutoSize = true;
      this.aggregationLabel.Enabled = false;
      this.aggregationLabel.Location = new System.Drawing.Point(5, 49);
      this.aggregationLabel.Name = "aggregationLabel";
      this.aggregationLabel.Size = new System.Drawing.Size(67, 13);
      this.aggregationLabel.TabIndex = 3;
      this.aggregationLabel.Text = "Aggregation:";
      // 
      // aggregationComboBox
      // 
      this.aggregationComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.aggregationComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.aggregationComboBox.Enabled = false;
      this.aggregationComboBox.FormattingEnabled = true;
      this.aggregationComboBox.Location = new System.Drawing.Point(79, 46);
      this.aggregationComboBox.Name = "aggregationComboBox";
      this.aggregationComboBox.Size = new System.Drawing.Size(96, 21);
      this.aggregationComboBox.TabIndex = 2;
      this.aggregationComboBox.SelectedIndexChanged += new System.EventHandler(this.aggregationComboBox_SelectedIndexChanged);
      // 
      // groupingComboBox
      // 
      this.groupingComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.groupingComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.groupingComboBox.FormattingEnabled = true;
      this.groupingComboBox.Location = new System.Drawing.Point(6, 19);
      this.groupingComboBox.Name = "groupingComboBox";
      this.groupingComboBox.Size = new System.Drawing.Size(169, 21);
      this.groupingComboBox.TabIndex = 1;
      this.groupingComboBox.SelectedIndexChanged += new System.EventHandler(this.groupingComboBox_SelectedIndexChanged);
      // 
      // opacityLabel
      // 
      this.opacityLabel.AutoSize = true;
      this.opacityLabel.Location = new System.Drawing.Point(6, 43);
      this.opacityLabel.Name = "opacityLabel";
      this.opacityLabel.Size = new System.Drawing.Size(46, 13);
      this.opacityLabel.TabIndex = 7;
      this.opacityLabel.Text = "Opacity:";
      // 
      // pointOpacityNumericUpDown
      // 
      this.pointOpacityNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pointOpacityNumericUpDown.DecimalPlaces = 2;
      this.pointOpacityNumericUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
      this.pointOpacityNumericUpDown.Location = new System.Drawing.Point(58, 41);
      this.pointOpacityNumericUpDown.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.pointOpacityNumericUpDown.Name = "pointOpacityNumericUpDown";
      this.pointOpacityNumericUpDown.Size = new System.Drawing.Size(115, 20);
      this.pointOpacityNumericUpDown.TabIndex = 8;
      this.pointOpacityNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.pointOpacityNumericUpDown.ValueChanged += new System.EventHandler(this.pointOpacityNumericUpDown_ValueChanged);
      // 
      // pointsGroupBox
      // 
      this.pointsGroupBox.Controls.Add(this.pointOpacityNumericUpDown);
      this.pointsGroupBox.Controls.Add(this.pointSizeLabel);
      this.pointsGroupBox.Controls.Add(this.pointSizeNumericUpDown);
      this.pointsGroupBox.Controls.Add(this.opacityLabel);
      this.pointsGroupBox.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.pointsGroupBox.Location = new System.Drawing.Point(0, 176);
      this.pointsGroupBox.Name = "pointsGroupBox";
      this.pointsGroupBox.Size = new System.Drawing.Size(180, 69);
      this.pointsGroupBox.TabIndex = 4;
      this.pointsGroupBox.TabStop = false;
      this.pointsGroupBox.Text = "Points";
      // 
      // ScatterPlotMultiView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Name = "ScatterPlotMultiView";
      this.Size = new System.Drawing.Size(589, 451);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.frameTableLayoutPanel.ResumeLayout(false);
      this.columnHeaderScrollPanel.ResumeLayout(false);
      this.columnHeaderScrollPanel.PerformLayout();
      this.rowHeaderScrollPanel.ResumeLayout(false);
      this.rowHeaderScrollPanel.PerformLayout();
      this.bodyScrollPanel.ResumeLayout(false);
      this.bodyScrollPanel.PerformLayout();
      this.sizeGroupBox.ResumeLayout(false);
      this.sizeGroupBox.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.heightNumericUpDown)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.widthNumericUpDown)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.pointSizeNumericUpDown)).EndInit();
      this.regressionGroupBox.ResumeLayout(false);
      this.regressionGroupBox.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.polynomialRegressionOrderNumericUpDown)).EndInit();
      this.groupingOptionsBox.ResumeLayout(false);
      this.groupingOptionsBox.PerformLayout();
      this.legendGroupBox.ResumeLayout(false);
      this.legendGroupBox.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pointOpacityNumericUpDown)).EndInit();
      this.pointsGroupBox.ResumeLayout(false);
      this.pointsGroupBox.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel frameTableLayoutPanel;
    private System.Windows.Forms.TableLayoutPanel columnHeaderTableLayoutPanel;
    private System.Windows.Forms.TableLayoutPanel rowHeaderTableLayoutPanel;
    private System.Windows.Forms.TableLayoutPanel bodyTableLayoutPanel;
    private System.Windows.Forms.Panel columnHeaderScrollPanel;
    private System.Windows.Forms.Panel rowHeaderScrollPanel;
    private System.Windows.Forms.Panel bodyScrollPanel;
    private System.Windows.Forms.GroupBox sizeGroupBox;
    private System.Windows.Forms.Label heightLabel;
    private System.Windows.Forms.Label widthLabel;
    private System.Windows.Forms.GroupBox regressionGroupBox;
    private System.Windows.Forms.ComboBox regressionTypeComboBox;
    private System.Windows.Forms.NumericUpDown polynomialRegressionOrderNumericUpDown;
    private System.Windows.Forms.Label orderLabel;
    private System.Windows.Forms.Label regressionTyleLabel;
    private System.Windows.Forms.GroupBox groupingOptionsBox;
    private System.Windows.Forms.ComboBox groupingComboBox;
    private System.Windows.Forms.NumericUpDown widthNumericUpDown;
    private System.Windows.Forms.NumericUpDown heightNumericUpDown;
    private System.Windows.Forms.CheckBox lockAspectCheckBox;
    private System.Windows.Forms.NumericUpDown pointSizeNumericUpDown;
    private System.Windows.Forms.Label pointSizeLabel;
    private System.Windows.Forms.Label aggregationLabel;
    private System.Windows.Forms.ComboBox aggregationComboBox;
    private System.Windows.Forms.GroupBox pointsGroupBox;
    private System.Windows.Forms.NumericUpDown pointOpacityNumericUpDown;
    private System.Windows.Forms.Label opacityLabel;
    private System.Windows.Forms.Label legendOrderLabel;
    private System.Windows.Forms.ComboBox legendOrderComboBox;
    private System.Windows.Forms.GroupBox legendGroupBox;
    private System.Windows.Forms.CheckBox legendCheckbox;
  }
}
