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

namespace HeuristicLab.Analysis.Statistics.Views {
  partial class StatisticalTestsView {
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
      this.stringConvertibleMatrixView = new HeuristicLab.Data.Views.StringConvertibleMatrixView();
      this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.openBoxPlotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.groupByLabel = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.resultComboBox = new System.Windows.Forms.ComboBox();
      this.lblPVal = new System.Windows.Forms.Label();
      this.pValTextBox = new System.Windows.Forms.TextBox();
      this.groupComboBox = new System.Windows.Forms.ComboBox();
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      this.splitContainer3 = new System.Windows.Forms.SplitContainer();
      this.pairwiseTestGroupBox = new System.Windows.Forms.GroupBox();
      this.pairwiseTextLabel = new System.Windows.Forms.Label();
      this.pairwiseStringConvertibleMatrixView = new HeuristicLab.Data.Views.StringConvertibleMatrixView();
      this.pairwiseLabel = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.equalDistsTextBox = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.groupCompComboBox = new System.Windows.Forms.ComboBox();
      this.allGroupTestGroupBox = new System.Windows.Forms.GroupBox();
      this.groupComTextLabel = new System.Windows.Forms.Label();
      this.groupCompLabel = new System.Windows.Forms.Label();
      this.normalityGroupBox = new System.Windows.Forms.GroupBox();
      this.normalityTextLabel = new System.Windows.Forms.Label();
      this.normalityStringConvertibleMatrixView = new HeuristicLab.Data.Views.StringConvertibleMatrixView();
      this.normalityLabel = new System.Windows.Forms.Label();
      this.selectDataGroupBox = new System.Windows.Forms.GroupBox();
      this.tabControl = new System.Windows.Forms.TabControl();
      this.tabPage1 = new System.Windows.Forms.TabPage();
      this.tabPage2 = new System.Windows.Forms.TabPage();
      this.histogramControl = new HeuristicLab.Analysis.Views.HistogramControl();
      this.splitContainer2 = new System.Windows.Forms.SplitContainer();
      this.contextMenuStrip1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
      this.splitContainer3.Panel1.SuspendLayout();
      this.splitContainer3.Panel2.SuspendLayout();
      this.splitContainer3.SuspendLayout();
      this.pairwiseTestGroupBox.SuspendLayout();
      this.allGroupTestGroupBox.SuspendLayout();
      this.normalityGroupBox.SuspendLayout();
      this.selectDataGroupBox.SuspendLayout();
      this.tabControl.SuspendLayout();
      this.tabPage1.SuspendLayout();
      this.tabPage2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
      this.splitContainer2.Panel1.SuspendLayout();
      this.splitContainer2.Panel2.SuspendLayout();
      this.splitContainer2.SuspendLayout();
      this.SuspendLayout();
      // 
      // stringConvertibleMatrixView
      // 
      this.stringConvertibleMatrixView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.stringConvertibleMatrixView.Caption = "StringConvertibleMatrix View";
      this.stringConvertibleMatrixView.Content = null;
      this.stringConvertibleMatrixView.ContextMenuStrip = this.contextMenuStrip1;
      this.stringConvertibleMatrixView.Location = new System.Drawing.Point(3, 3);
      this.stringConvertibleMatrixView.Name = "stringConvertibleMatrixView";
      this.stringConvertibleMatrixView.ReadOnly = false;
      this.stringConvertibleMatrixView.ShowRowsAndColumnsTextBox = false;
      this.stringConvertibleMatrixView.ShowStatisticalInformation = true;
      this.stringConvertibleMatrixView.Size = new System.Drawing.Size(567, 597);
      this.stringConvertibleMatrixView.TabIndex = 0;
      // 
      // contextMenuStrip1
      // 
      this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openBoxPlotToolStripMenuItem});
      this.contextMenuStrip1.Name = "contextMenuStrip1";
      this.contextMenuStrip1.Size = new System.Drawing.Size(175, 26);
      // 
      // openBoxPlotToolStripMenuItem
      // 
      this.openBoxPlotToolStripMenuItem.Name = "openBoxPlotToolStripMenuItem";
      this.openBoxPlotToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
      this.openBoxPlotToolStripMenuItem.Text = "Open BoxPlot View";
      this.openBoxPlotToolStripMenuItem.Click += new System.EventHandler(this.openBoxPlotToolStripMenuItem_Click);
      // 
      // groupByLabel
      // 
      this.groupByLabel.AutoSize = true;
      this.groupByLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupByLabel.Location = new System.Drawing.Point(3, 6);
      this.groupByLabel.Name = "groupByLabel";
      this.groupByLabel.Size = new System.Drawing.Size(53, 13);
      this.groupByLabel.TabIndex = 7;
      this.groupByLabel.Text = "Group by:";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(3, 6);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(40, 13);
      this.label1.TabIndex = 6;
      this.label1.Text = "Result:";
      // 
      // resultComboBox
      // 
      this.resultComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.resultComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.resultComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.resultComboBox.FormattingEnabled = true;
      this.resultComboBox.Location = new System.Drawing.Point(62, 3);
      this.resultComboBox.Name = "resultComboBox";
      this.resultComboBox.Size = new System.Drawing.Size(244, 21);
      this.resultComboBox.TabIndex = 5;
      this.resultComboBox.SelectedValueChanged += new System.EventHandler(this.resultComboBox_SelectedValueChanged);
      // 
      // lblPVal
      // 
      this.lblPVal.AutoSize = true;
      this.lblPVal.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblPVal.Location = new System.Drawing.Point(6, 39);
      this.lblPVal.Name = "lblPVal";
      this.lblPVal.Size = new System.Drawing.Size(46, 13);
      this.lblPVal.TabIndex = 12;
      this.lblPVal.Text = "p-Value:";
      this.lblPVal.TextAlign = System.Drawing.ContentAlignment.TopRight;
      // 
      // pValTextBox
      // 
      this.pValTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pValTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.pValTextBox.Location = new System.Drawing.Point(58, 36);
      this.pValTextBox.Name = "pValTextBox";
      this.pValTextBox.ReadOnly = true;
      this.pValTextBox.Size = new System.Drawing.Size(257, 20);
      this.pValTextBox.TabIndex = 13;
      // 
      // groupComboBox
      // 
      this.groupComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.groupComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.groupComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupComboBox.FormattingEnabled = true;
      this.groupComboBox.Location = new System.Drawing.Point(62, 3);
      this.groupComboBox.Name = "groupComboBox";
      this.groupComboBox.Size = new System.Drawing.Size(244, 21);
      this.groupComboBox.TabIndex = 14;
      this.groupComboBox.SelectedValueChanged += new System.EventHandler(this.groupComboBox_SelectedValueChanged);
      // 
      // splitContainer1
      // 
      this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.splitContainer1.Location = new System.Drawing.Point(6, 19);
      this.splitContainer1.Name = "splitContainer1";
      this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer1.Panel1
      // 
      this.splitContainer1.Panel1.Controls.Add(this.resultComboBox);
      this.splitContainer1.Panel1.Controls.Add(this.label1);
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.groupByLabel);
      this.splitContainer1.Panel2.Controls.Add(this.groupComboBox);
      this.splitContainer1.Size = new System.Drawing.Size(309, 54);
      this.splitContainer1.SplitterDistance = 25;
      this.splitContainer1.TabIndex = 18;
      // 
      // splitContainer3
      // 
      this.splitContainer3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.splitContainer3.Location = new System.Drawing.Point(6, 6);
      this.splitContainer3.Name = "splitContainer3";
      // 
      // splitContainer3.Panel1
      // 
      this.splitContainer3.Panel1.Controls.Add(this.stringConvertibleMatrixView);
      // 
      // splitContainer3.Panel2
      // 
      this.splitContainer3.Panel2.Controls.Add(this.splitContainer2);
      this.splitContainer3.Size = new System.Drawing.Size(910, 603);
      this.splitContainer3.SplitterDistance = 573;
      this.splitContainer3.TabIndex = 20;
      // 
      // pairwiseTestGroupBox
      // 
      this.pairwiseTestGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pairwiseTestGroupBox.Controls.Add(this.pairwiseTextLabel);
      this.pairwiseTestGroupBox.Controls.Add(this.pairwiseStringConvertibleMatrixView);
      this.pairwiseTestGroupBox.Controls.Add(this.pairwiseLabel);
      this.pairwiseTestGroupBox.Controls.Add(this.label3);
      this.pairwiseTestGroupBox.Controls.Add(this.equalDistsTextBox);
      this.pairwiseTestGroupBox.Controls.Add(this.label2);
      this.pairwiseTestGroupBox.Controls.Add(this.groupCompComboBox);
      this.pairwiseTestGroupBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.pairwiseTestGroupBox.Location = new System.Drawing.Point(3, 3);
      this.pairwiseTestGroupBox.Name = "pairwiseTestGroupBox";
      this.pairwiseTestGroupBox.Size = new System.Drawing.Size(321, 287);
      this.pairwiseTestGroupBox.TabIndex = 22;
      this.pairwiseTestGroupBox.TabStop = false;
      this.pairwiseTestGroupBox.Text = "4. Pairwise Test for Inequalities in Distributions";
      // 
      // pairwiseTextLabel
      // 
      this.pairwiseTextLabel.AutoSize = true;
      this.pairwiseTextLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.pairwiseTextLabel.Location = new System.Drawing.Point(28, 46);
      this.pairwiseTextLabel.Name = "pairwiseTextLabel";
      this.pairwiseTextLabel.Size = new System.Drawing.Size(165, 13);
      this.pairwiseTextLabel.TabIndex = 20;
      this.pairwiseTextLabel.Text = "Groups have different distribution:";
      // 
      // pairwiseStringConvertibleMatrixView
      // 
      this.pairwiseStringConvertibleMatrixView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pairwiseStringConvertibleMatrixView.Caption = "StringConvertibleMatrix View";
      this.pairwiseStringConvertibleMatrixView.Content = null;
      this.pairwiseStringConvertibleMatrixView.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.pairwiseStringConvertibleMatrixView.Location = new System.Drawing.Point(6, 97);
      this.pairwiseStringConvertibleMatrixView.Name = "pairwiseStringConvertibleMatrixView";
      this.pairwiseStringConvertibleMatrixView.ReadOnly = false;
      this.pairwiseStringConvertibleMatrixView.ShowRowsAndColumnsTextBox = false;
      this.pairwiseStringConvertibleMatrixView.ShowStatisticalInformation = false;
      this.pairwiseStringConvertibleMatrixView.Size = new System.Drawing.Size(309, 172);
      this.pairwiseStringConvertibleMatrixView.TabIndex = 19;
      // 
      // pairwiseLabel
      // 
      this.pairwiseLabel.AutoSize = true;
      this.pairwiseLabel.Location = new System.Drawing.Point(6, 44);
      this.pairwiseLabel.MaximumSize = new System.Drawing.Size(16, 16);
      this.pairwiseLabel.MinimumSize = new System.Drawing.Size(16, 16);
      this.pairwiseLabel.Name = "pairwiseLabel";
      this.pairwiseLabel.Size = new System.Drawing.Size(16, 16);
      this.pairwiseLabel.TabIndex = 18;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.Location = new System.Drawing.Point(6, 68);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(74, 13);
      this.label3.TabIndex = 18;
      this.label3.Text = "Equal Groups:";
      this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
      // 
      // equalDistsTextBox
      // 
      this.equalDistsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.equalDistsTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.equalDistsTextBox.Location = new System.Drawing.Point(86, 65);
      this.equalDistsTextBox.Name = "equalDistsTextBox";
      this.equalDistsTextBox.ReadOnly = true;
      this.equalDistsTextBox.Size = new System.Drawing.Size(229, 20);
      this.equalDistsTextBox.TabIndex = 18;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.Location = new System.Drawing.Point(6, 22);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(111, 13);
      this.label2.TabIndex = 18;
      this.label2.Text = "Group for comparison:";
      // 
      // groupCompComboBox
      // 
      this.groupCompComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.groupCompComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.groupCompComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupCompComboBox.FormattingEnabled = true;
      this.groupCompComboBox.Location = new System.Drawing.Point(123, 19);
      this.groupCompComboBox.Name = "groupCompComboBox";
      this.groupCompComboBox.Size = new System.Drawing.Size(192, 21);
      this.groupCompComboBox.TabIndex = 17;
      this.groupCompComboBox.SelectedValueChanged += new System.EventHandler(this.groupCompComboBox_SelectedValueChanged);
      // 
      // allGroupTestGroupBox
      // 
      this.allGroupTestGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.allGroupTestGroupBox.Controls.Add(this.groupComTextLabel);
      this.allGroupTestGroupBox.Controls.Add(this.groupCompLabel);
      this.allGroupTestGroupBox.Controls.Add(this.lblPVal);
      this.allGroupTestGroupBox.Controls.Add(this.pValTextBox);
      this.allGroupTestGroupBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.allGroupTestGroupBox.Location = new System.Drawing.Point(3, 237);
      this.allGroupTestGroupBox.Name = "allGroupTestGroupBox";
      this.allGroupTestGroupBox.Size = new System.Drawing.Size(321, 63);
      this.allGroupTestGroupBox.TabIndex = 21;
      this.allGroupTestGroupBox.TabStop = false;
      this.allGroupTestGroupBox.Text = "3. Kruskal Wallis Test for Inequalities in Distributions";
      // 
      // groupComTextLabel
      // 
      this.groupComTextLabel.AutoSize = true;
      this.groupComTextLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupComTextLabel.Location = new System.Drawing.Point(28, 16);
      this.groupComTextLabel.Name = "groupComTextLabel";
      this.groupComTextLabel.Size = new System.Drawing.Size(165, 13);
      this.groupComTextLabel.TabIndex = 19;
      this.groupComTextLabel.Text = "Groups have different distribution:";
      // 
      // groupCompLabel
      // 
      this.groupCompLabel.AutoSize = true;
      this.groupCompLabel.Location = new System.Drawing.Point(6, 16);
      this.groupCompLabel.MaximumSize = new System.Drawing.Size(16, 16);
      this.groupCompLabel.MinimumSize = new System.Drawing.Size(16, 16);
      this.groupCompLabel.Name = "groupCompLabel";
      this.groupCompLabel.Size = new System.Drawing.Size(16, 16);
      this.groupCompLabel.TabIndex = 17;
      // 
      // normalityGroupBox
      // 
      this.normalityGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.normalityGroupBox.Controls.Add(this.normalityTextLabel);
      this.normalityGroupBox.Controls.Add(this.normalityStringConvertibleMatrixView);
      this.normalityGroupBox.Controls.Add(this.normalityLabel);
      this.normalityGroupBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.normalityGroupBox.Location = new System.Drawing.Point(3, 88);
      this.normalityGroupBox.Name = "normalityGroupBox";
      this.normalityGroupBox.Size = new System.Drawing.Size(321, 143);
      this.normalityGroupBox.TabIndex = 20;
      this.normalityGroupBox.TabStop = false;
      this.normalityGroupBox.Text = "2. Jarque-Bera Test for Normal Distribution";
      // 
      // normalityTextLabel
      // 
      this.normalityTextLabel.AutoSize = true;
      this.normalityTextLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.normalityTextLabel.Location = new System.Drawing.Point(28, 16);
      this.normalityTextLabel.Name = "normalityTextLabel";
      this.normalityTextLabel.Size = new System.Drawing.Size(135, 13);
      this.normalityTextLabel.TabIndex = 18;
      this.normalityTextLabel.Text = "Data is normally distributed:";
      // 
      // normalityStringConvertibleMatrixView
      // 
      this.normalityStringConvertibleMatrixView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.normalityStringConvertibleMatrixView.Caption = "StringConvertibleMatrix View";
      this.normalityStringConvertibleMatrixView.Content = null;
      this.normalityStringConvertibleMatrixView.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.normalityStringConvertibleMatrixView.Location = new System.Drawing.Point(6, 35);
      this.normalityStringConvertibleMatrixView.Name = "normalityStringConvertibleMatrixView";
      this.normalityStringConvertibleMatrixView.ReadOnly = false;
      this.normalityStringConvertibleMatrixView.ShowRowsAndColumnsTextBox = false;
      this.normalityStringConvertibleMatrixView.ShowStatisticalInformation = false;
      this.normalityStringConvertibleMatrixView.Size = new System.Drawing.Size(306, 102);
      this.normalityStringConvertibleMatrixView.TabIndex = 17;
      // 
      // normalityLabel
      // 
      this.normalityLabel.AutoSize = true;
      this.normalityLabel.Location = new System.Drawing.Point(6, 16);
      this.normalityLabel.MaximumSize = new System.Drawing.Size(16, 16);
      this.normalityLabel.MinimumSize = new System.Drawing.Size(16, 16);
      this.normalityLabel.Name = "normalityLabel";
      this.normalityLabel.Size = new System.Drawing.Size(16, 16);
      this.normalityLabel.TabIndex = 16;
      // 
      // selectDataGroupBox
      // 
      this.selectDataGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.selectDataGroupBox.Controls.Add(this.splitContainer1);
      this.selectDataGroupBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.selectDataGroupBox.Location = new System.Drawing.Point(3, 3);
      this.selectDataGroupBox.Name = "selectDataGroupBox";
      this.selectDataGroupBox.Size = new System.Drawing.Size(321, 79);
      this.selectDataGroupBox.TabIndex = 19;
      this.selectDataGroupBox.TabStop = false;
      this.selectDataGroupBox.Text = "1. Select Data";
      // 
      // tabControl
      // 
      this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl.Controls.Add(this.tabPage1);
      this.tabControl.Controls.Add(this.tabPage2);
      this.tabControl.Location = new System.Drawing.Point(3, 3);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(930, 641);
      this.tabControl.TabIndex = 21;
      // 
      // tabPage1
      // 
      this.tabPage1.Controls.Add(this.splitContainer3);
      this.tabPage1.Location = new System.Drawing.Point(4, 22);
      this.tabPage1.Name = "tabPage1";
      this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage1.Size = new System.Drawing.Size(922, 615);
      this.tabPage1.TabIndex = 0;
      this.tabPage1.Text = "Tests";
      this.tabPage1.UseVisualStyleBackColor = true;
      // 
      // tabPage2
      // 
      this.tabPage2.Controls.Add(this.histogramControl);
      this.tabPage2.Location = new System.Drawing.Point(4, 22);
      this.tabPage2.Name = "tabPage2";
      this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage2.Size = new System.Drawing.Size(922, 615);
      this.tabPage2.TabIndex = 1;
      this.tabPage2.Text = "Histogram";
      this.tabPage2.UseVisualStyleBackColor = true;
      // 
      // histogramControl
      // 
      this.histogramControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.histogramControl.CalculateExactBins = false;
      this.histogramControl.IncrementNumberOfBins = 1;
      this.histogramControl.Location = new System.Drawing.Point(6, 6);
      this.histogramControl.MaximumNumberOfBins = 100000;
      this.histogramControl.MinimumNumberOfBins = 1;
      this.histogramControl.Name = "histogramControl";
      this.histogramControl.NumberOfBins = 10;
      this.histogramControl.ShowExactCheckbox = false;
      this.histogramControl.Size = new System.Drawing.Size(910, 603);
      this.histogramControl.TabIndex = 0;
      // 
      // splitContainer2
      // 
      this.splitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
      this.splitContainer2.IsSplitterFixed = true;
      this.splitContainer2.Location = new System.Drawing.Point(3, 3);
      this.splitContainer2.Name = "splitContainer2";
      this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer2.Panel1
      // 
      this.splitContainer2.Panel1.Controls.Add(this.selectDataGroupBox);
      this.splitContainer2.Panel1.Controls.Add(this.allGroupTestGroupBox);
      this.splitContainer2.Panel1.Controls.Add(this.normalityGroupBox);
      // 
      // splitContainer2.Panel2
      // 
      this.splitContainer2.Panel2.Controls.Add(this.pairwiseTestGroupBox);
      this.splitContainer2.Size = new System.Drawing.Size(327, 597);
      this.splitContainer2.SplitterDistance = 303;
      this.splitContainer2.SplitterWidth = 1;
      this.splitContainer2.TabIndex = 1;
      // 
      // StatisticalTestsView
      // 
      this.Controls.Add(this.tabControl);
      this.Name = "StatisticalTestsView";
      this.Size = new System.Drawing.Size(936, 647);
      this.contextMenuStrip1.ResumeLayout(false);
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel1.PerformLayout();
      this.splitContainer1.Panel2.ResumeLayout(false);
      this.splitContainer1.Panel2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
      this.splitContainer1.ResumeLayout(false);
      this.splitContainer3.Panel1.ResumeLayout(false);
      this.splitContainer3.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
      this.splitContainer3.ResumeLayout(false);
      this.pairwiseTestGroupBox.ResumeLayout(false);
      this.pairwiseTestGroupBox.PerformLayout();
      this.allGroupTestGroupBox.ResumeLayout(false);
      this.allGroupTestGroupBox.PerformLayout();
      this.normalityGroupBox.ResumeLayout(false);
      this.normalityGroupBox.PerformLayout();
      this.selectDataGroupBox.ResumeLayout(false);
      this.tabControl.ResumeLayout(false);
      this.tabPage1.ResumeLayout(false);
      this.tabPage2.ResumeLayout(false);
      this.splitContainer2.Panel1.ResumeLayout(false);
      this.splitContainer2.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
      this.splitContainer2.ResumeLayout(false);
      this.ResumeLayout(false);

    }
    #endregion

    private HeuristicLab.Data.Views.StringConvertibleMatrixView stringConvertibleMatrixView;
    private System.Windows.Forms.Label groupByLabel;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.ComboBox resultComboBox;
    private System.Windows.Forms.Label lblPVal;
    private System.Windows.Forms.TextBox pValTextBox;
    private System.Windows.Forms.ComboBox groupComboBox;
    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
    private System.Windows.Forms.ToolStripMenuItem openBoxPlotToolStripMenuItem;
    private System.Windows.Forms.SplitContainer splitContainer3;
    private System.Windows.Forms.GroupBox selectDataGroupBox;
    private System.Windows.Forms.GroupBox normalityGroupBox;
    private System.Windows.Forms.GroupBox pairwiseTestGroupBox;
    private System.Windows.Forms.GroupBox allGroupTestGroupBox;
    private System.Windows.Forms.Label normalityLabel;
    private System.Windows.Forms.Label groupCompLabel;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.ComboBox groupCompComboBox;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox equalDistsTextBox;
    private System.Windows.Forms.Label pairwiseLabel;
    private Data.Views.StringConvertibleMatrixView normalityStringConvertibleMatrixView;
    private Data.Views.StringConvertibleMatrixView pairwiseStringConvertibleMatrixView;
    private System.Windows.Forms.Label normalityTextLabel;
    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TabPage tabPage1;
    private System.Windows.Forms.TabPage tabPage2;
    private HeuristicLab.Analysis.Views.HistogramControl histogramControl;
    private System.Windows.Forms.Label pairwiseTextLabel;
    private System.Windows.Forms.Label groupComTextLabel;
    private System.Windows.Forms.SplitContainer splitContainer2;
  }
}
