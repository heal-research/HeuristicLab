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

namespace HeuristicLab.DataPreprocessing.Views {
  partial class DataGridContentView {
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
      this.btnApplySort = new System.Windows.Forms.Button();
      this.contextMenuCell = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.replaceValueOverColumnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.averageToolStripMenuItem_Column = new System.Windows.Forms.ToolStripMenuItem();
      this.medianToolStripMenuItem_Column = new System.Windows.Forms.ToolStripMenuItem();
      this.randomToolStripMenuItem_Column = new System.Windows.Forms.ToolStripMenuItem();
      this.mostCommonToolStripMenuItem_Column = new System.Windows.Forms.ToolStripMenuItem();
      this.interpolationToolStripMenuItem_Column = new System.Windows.Forms.ToolStripMenuItem();
      this.replaceValueOverSelectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.averageToolStripMenuItem_Selection = new System.Windows.Forms.ToolStripMenuItem();
      this.medianToolStripMenuItem_Selection = new System.Windows.Forms.ToolStripMenuItem();
      this.randomToolStripMenuItem_Selection = new System.Windows.Forms.ToolStripMenuItem();
      this.mostCommonToolStripMenuItem_Selection = new System.Windows.Forms.ToolStripMenuItem();
      this.btnSearch = new System.Windows.Forms.Button();
      this.btnReplace = new System.Windows.Forms.Button();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.shuffleWithinPartitionsCheckBox = new System.Windows.Forms.CheckBox();
      this.addRowButton = new System.Windows.Forms.Button();
      this.addColumnButton = new System.Windows.Forms.Button();
      this.renameColumnsButton = new System.Windows.Forms.Button();
      this.showVariablesGroupBox = new System.Windows.Forms.GroupBox();
      this.shuffleAllButton = new System.Windows.Forms.Button();
      this.checkInputsTargetButton = new System.Windows.Forms.Button();
      this.uncheckAllButton = new System.Windows.Forms.Button();
      this.checkAllButton = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.contextMenuCell.SuspendLayout();
      this.showVariablesGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // rowsLabel
      // 
      this.rowsLabel.Size = new System.Drawing.Size(55, 13);
      this.rowsLabel.Text = "Datarows:";
      // 
      // rowsTextBox
      // 
      this.rowsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
      this.errorProvider.SetIconAlignment(this.rowsTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.rowsTextBox, 2);
      this.rowsTextBox.Size = new System.Drawing.Size(71, 20);
      // 
      // columnsTextBox
      // 
      this.columnsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
      this.columnsTextBox.Size = new System.Drawing.Size(71, 20);
      // 
      // columnsLabel
      // 
      this.columnsLabel.Size = new System.Drawing.Size(53, 13);
      this.columnsLabel.Text = "Variables:";
      // 
      // statisticsTextBox
      // 
      this.statisticsTextBox.Size = new System.Drawing.Size(421, 13);
      // 
      // btnApplySort
      // 
      this.btnApplySort.Location = new System.Drawing.Point(228, 0);
      this.btnApplySort.Name = "btnApplySort";
      this.btnApplySort.Size = new System.Drawing.Size(104, 23);
      this.btnApplySort.TabIndex = 7;
      this.btnApplySort.Text = "Apply Sort";
      this.toolTip.SetToolTip(this.btnApplySort, "The current sorting is applied on the data itself.");
      this.btnApplySort.UseVisualStyleBackColor = true;
      this.btnApplySort.Click += new System.EventHandler(this.btnApplySort_Click);
      // 
      // contextMenuCell
      // 
      this.contextMenuCell.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.replaceValueOverColumnToolStripMenuItem,
            this.replaceValueOverSelectionToolStripMenuItem});
      this.contextMenuCell.Name = "contextMenuCell";
      this.contextMenuCell.Size = new System.Drawing.Size(224, 48);
      // 
      // replaceValueOverColumnToolStripMenuItem
      // 
      this.replaceValueOverColumnToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.averageToolStripMenuItem_Column,
            this.medianToolStripMenuItem_Column,
            this.randomToolStripMenuItem_Column,
            this.mostCommonToolStripMenuItem_Column,
            this.interpolationToolStripMenuItem_Column});
      this.replaceValueOverColumnToolStripMenuItem.Name = "replaceValueOverColumnToolStripMenuItem";
      this.replaceValueOverColumnToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
      this.replaceValueOverColumnToolStripMenuItem.Text = "Replace Value over Variable";
      // 
      // averageToolStripMenuItem_Column
      // 
      this.averageToolStripMenuItem_Column.Name = "averageToolStripMenuItem_Column";
      this.averageToolStripMenuItem_Column.Size = new System.Drawing.Size(155, 22);
      this.averageToolStripMenuItem_Column.Text = "Average";
      this.averageToolStripMenuItem_Column.Click += new System.EventHandler(this.ReplaceWithAverage_Column_Click);
      // 
      // medianToolStripMenuItem_Column
      // 
      this.medianToolStripMenuItem_Column.Name = "medianToolStripMenuItem_Column";
      this.medianToolStripMenuItem_Column.Size = new System.Drawing.Size(155, 22);
      this.medianToolStripMenuItem_Column.Text = "Median";
      this.medianToolStripMenuItem_Column.Click += new System.EventHandler(this.ReplaceWithMedian_Column_Click);
      // 
      // randomToolStripMenuItem_Column
      // 
      this.randomToolStripMenuItem_Column.Name = "randomToolStripMenuItem_Column";
      this.randomToolStripMenuItem_Column.Size = new System.Drawing.Size(155, 22);
      this.randomToolStripMenuItem_Column.Text = "Random";
      this.randomToolStripMenuItem_Column.Click += new System.EventHandler(this.ReplaceWithRandom_Column_Click);
      // 
      // mostCommonToolStripMenuItem_Column
      // 
      this.mostCommonToolStripMenuItem_Column.Name = "mostCommonToolStripMenuItem_Column";
      this.mostCommonToolStripMenuItem_Column.Size = new System.Drawing.Size(155, 22);
      this.mostCommonToolStripMenuItem_Column.Text = "Most Common";
      this.mostCommonToolStripMenuItem_Column.Click += new System.EventHandler(this.ReplaceWithMostCommon_Column_Click);
      // 
      // interpolationToolStripMenuItem_Column
      // 
      this.interpolationToolStripMenuItem_Column.Name = "interpolationToolStripMenuItem_Column";
      this.interpolationToolStripMenuItem_Column.Size = new System.Drawing.Size(155, 22);
      this.interpolationToolStripMenuItem_Column.Text = "Interpolation";
      this.interpolationToolStripMenuItem_Column.Click += new System.EventHandler(this.ReplaceWithInterpolation_Column_Click);
      // 
      // replaceValueOverSelectionToolStripMenuItem
      // 
      this.replaceValueOverSelectionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.averageToolStripMenuItem_Selection,
            this.medianToolStripMenuItem_Selection,
            this.randomToolStripMenuItem_Selection,
            this.mostCommonToolStripMenuItem_Selection});
      this.replaceValueOverSelectionToolStripMenuItem.Name = "replaceValueOverSelectionToolStripMenuItem";
      this.replaceValueOverSelectionToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
      this.replaceValueOverSelectionToolStripMenuItem.Text = "Replace Value over Selection";
      // 
      // averageToolStripMenuItem_Selection
      // 
      this.averageToolStripMenuItem_Selection.Name = "averageToolStripMenuItem_Selection";
      this.averageToolStripMenuItem_Selection.Size = new System.Drawing.Size(155, 22);
      this.averageToolStripMenuItem_Selection.Text = "Average";
      this.averageToolStripMenuItem_Selection.Click += new System.EventHandler(this.ReplaceWithAverage_Selection_Click);
      // 
      // medianToolStripMenuItem_Selection
      // 
      this.medianToolStripMenuItem_Selection.Name = "medianToolStripMenuItem_Selection";
      this.medianToolStripMenuItem_Selection.Size = new System.Drawing.Size(155, 22);
      this.medianToolStripMenuItem_Selection.Text = "Median";
      this.medianToolStripMenuItem_Selection.Click += new System.EventHandler(this.ReplaceWithMedian_Selection_Click);
      // 
      // randomToolStripMenuItem_Selection
      // 
      this.randomToolStripMenuItem_Selection.Name = "randomToolStripMenuItem_Selection";
      this.randomToolStripMenuItem_Selection.Size = new System.Drawing.Size(155, 22);
      this.randomToolStripMenuItem_Selection.Text = "Random";
      this.randomToolStripMenuItem_Selection.Click += new System.EventHandler(this.ReplaceWithRandom_Selection_Click);
      // 
      // mostCommonToolStripMenuItem_Selection
      // 
      this.mostCommonToolStripMenuItem_Selection.Name = "mostCommonToolStripMenuItem_Selection";
      this.mostCommonToolStripMenuItem_Selection.Size = new System.Drawing.Size(155, 22);
      this.mostCommonToolStripMenuItem_Selection.Text = "Most Common";
      this.mostCommonToolStripMenuItem_Selection.Click += new System.EventHandler(this.ReplaceWithMostCommon_Selection_Click);
      // 
      // btnSearch
      // 
      this.btnSearch.Location = new System.Drawing.Point(167, 0);
      this.btnSearch.Name = "btnSearch";
      this.btnSearch.Size = new System.Drawing.Size(55, 23);
      this.btnSearch.TabIndex = 8;
      this.btnSearch.Text = "Search";
      this.toolTip.SetToolTip(this.btnSearch, "Opens the Search dialog");
      this.btnSearch.UseVisualStyleBackColor = true;
      this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
      // 
      // btnReplace
      // 
      this.btnReplace.Location = new System.Drawing.Point(167, 26);
      this.btnReplace.Name = "btnReplace";
      this.btnReplace.Size = new System.Drawing.Size(55, 23);
      this.btnReplace.TabIndex = 9;
      this.btnReplace.Text = "Replace";
      this.toolTip.SetToolTip(this.btnReplace, "Opens the Search & Replace dialog");
      this.btnReplace.UseVisualStyleBackColor = true;
      this.btnReplace.Click += new System.EventHandler(this.btnReplace_Click);
      // 
      // shuffleWithinPartitionsCheckBox
      // 
      this.shuffleWithinPartitionsCheckBox.AutoSize = true;
      this.shuffleWithinPartitionsCheckBox.Location = new System.Drawing.Point(444, 30);
      this.shuffleWithinPartitionsCheckBox.Name = "shuffleWithinPartitionsCheckBox";
      this.shuffleWithinPartitionsCheckBox.Size = new System.Drawing.Size(102, 17);
      this.shuffleWithinPartitionsCheckBox.TabIndex = 20;
      this.shuffleWithinPartitionsCheckBox.Text = "Within Partitions";
      this.toolTip.SetToolTip(this.shuffleWithinPartitionsCheckBox, "If checked, the Training and Test partitions are shuffled separately. Otherwise a" +
        "ll data is shuffled.");
      this.shuffleWithinPartitionsCheckBox.UseVisualStyleBackColor = true;
      // 
      // addRowButton
      // 
      this.addRowButton.Location = new System.Drawing.Point(338, 26);
      this.addRowButton.Name = "addRowButton";
      this.addRowButton.Size = new System.Drawing.Size(83, 23);
      this.addRowButton.TabIndex = 10;
      this.addRowButton.Text = "Add Datarow";
      this.addRowButton.UseVisualStyleBackColor = true;
      this.addRowButton.Click += new System.EventHandler(this.addRowButton_Click);
      // 
      // addColumnButton
      // 
      this.addColumnButton.Location = new System.Drawing.Point(338, 0);
      this.addColumnButton.Name = "addColumnButton";
      this.addColumnButton.Size = new System.Drawing.Size(83, 23);
      this.addColumnButton.TabIndex = 10;
      this.addColumnButton.Text = "Add Variable";
      this.addColumnButton.UseVisualStyleBackColor = true;
      this.addColumnButton.Click += new System.EventHandler(this.addColumnButton_Click);
      // 
      // renameColumnsButton
      // 
      this.renameColumnsButton.Location = new System.Drawing.Point(228, 26);
      this.renameColumnsButton.Name = "renameColumnsButton";
      this.renameColumnsButton.Size = new System.Drawing.Size(104, 23);
      this.renameColumnsButton.TabIndex = 11;
      this.renameColumnsButton.Text = "Rename Variables";
      this.renameColumnsButton.UseVisualStyleBackColor = true;
      this.renameColumnsButton.Click += new System.EventHandler(this.renameColumnsButton_Click);
      // 
      // showVariablesGroupBox
      // 
      this.showVariablesGroupBox.Controls.Add(this.checkInputsTargetButton);
      this.showVariablesGroupBox.Controls.Add(this.uncheckAllButton);
      this.showVariablesGroupBox.Controls.Add(this.checkAllButton);
      this.showVariablesGroupBox.Location = new System.Drawing.Point(564, 0);
      this.showVariablesGroupBox.Name = "showVariablesGroupBox";
      this.showVariablesGroupBox.Size = new System.Drawing.Size(97, 49);
      this.showVariablesGroupBox.TabIndex = 17;
      this.showVariablesGroupBox.TabStop = false;
      this.showVariablesGroupBox.Text = "Show Variables";
      // 
      // shuffleAllButton
      // 
      this.shuffleAllButton.Location = new System.Drawing.Point(444, 0);
      this.shuffleAllButton.Name = "shuffleAllButton";
      this.shuffleAllButton.Size = new System.Drawing.Size(102, 23);
      this.shuffleAllButton.TabIndex = 19;
      this.shuffleAllButton.Text = "Shuffle";
      this.shuffleAllButton.UseVisualStyleBackColor = true;
      this.shuffleAllButton.Click += new System.EventHandler(this.shuffleAllButton_Click);
      // 
      // checkInputsTargetButton
      // 
      this.checkInputsTargetButton.Image = global::HeuristicLab.DataPreprocessing.Views.PreprocessingIcons.Inputs;
      this.checkInputsTargetButton.Location = new System.Drawing.Point(36, 19);
      this.checkInputsTargetButton.Name = "checkInputsTargetButton";
      this.checkInputsTargetButton.Size = new System.Drawing.Size(24, 24);
      this.checkInputsTargetButton.TabIndex = 14;
      this.toolTip.SetToolTip(this.checkInputsTargetButton, "Select Inputs & Target");
      this.checkInputsTargetButton.UseVisualStyleBackColor = true;
      this.checkInputsTargetButton.Click += new System.EventHandler(this.checkInputsTargetButton_Click);
      // 
      // uncheckAllButton
      // 
      this.uncheckAllButton.Image = global::HeuristicLab.DataPreprocessing.Views.PreprocessingIcons.None;
      this.uncheckAllButton.Location = new System.Drawing.Point(66, 19);
      this.uncheckAllButton.Name = "uncheckAllButton";
      this.uncheckAllButton.Size = new System.Drawing.Size(24, 24);
      this.uncheckAllButton.TabIndex = 12;
      this.toolTip.SetToolTip(this.uncheckAllButton, "Select None");
      this.uncheckAllButton.UseVisualStyleBackColor = true;
      this.uncheckAllButton.Click += new System.EventHandler(this.uncheckAllButton_Click);
      // 
      // checkAllButton
      // 
      this.checkAllButton.Image = global::HeuristicLab.DataPreprocessing.Views.PreprocessingIcons.All;
      this.checkAllButton.Location = new System.Drawing.Point(6, 19);
      this.checkAllButton.Name = "checkAllButton";
      this.checkAllButton.Size = new System.Drawing.Size(24, 24);
      this.checkAllButton.TabIndex = 13;
      this.toolTip.SetToolTip(this.checkAllButton, "Select All");
      this.checkAllButton.UseVisualStyleBackColor = true;
      this.checkAllButton.Click += new System.EventHandler(this.checkAllButton_Click);
      // 
      // DataGridContentView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.shuffleWithinPartitionsCheckBox);
      this.Controls.Add(this.shuffleAllButton);
      this.Controls.Add(this.showVariablesGroupBox);
      this.Controls.Add(this.renameColumnsButton);
      this.Controls.Add(this.addColumnButton);
      this.Controls.Add(this.addRowButton);
      this.Controls.Add(this.btnReplace);
      this.Controls.Add(this.btnSearch);
      this.Controls.Add(this.btnApplySort);
      this.Name = "DataGridContentView";
      this.Controls.SetChildIndex(this.btnApplySort, 0);
      this.Controls.SetChildIndex(this.btnSearch, 0);
      this.Controls.SetChildIndex(this.btnReplace, 0);
      this.Controls.SetChildIndex(this.addRowButton, 0);
      this.Controls.SetChildIndex(this.addColumnButton, 0);
      this.Controls.SetChildIndex(this.renameColumnsButton, 0);
      this.Controls.SetChildIndex(this.statisticsTextBox, 0);
      this.Controls.SetChildIndex(this.rowsLabel, 0);
      this.Controls.SetChildIndex(this.columnsLabel, 0);
      this.Controls.SetChildIndex(this.rowsTextBox, 0);
      this.Controls.SetChildIndex(this.columnsTextBox, 0);
      this.Controls.SetChildIndex(this.showVariablesGroupBox, 0);
      this.Controls.SetChildIndex(this.shuffleAllButton, 0);
      this.Controls.SetChildIndex(this.shuffleWithinPartitionsCheckBox, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.contextMenuCell.ResumeLayout(false);
      this.showVariablesGroupBox.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button btnApplySort;
    private System.Windows.Forms.ContextMenuStrip contextMenuCell;
    private System.Windows.Forms.ToolStripMenuItem replaceValueOverColumnToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem averageToolStripMenuItem_Column;
    private System.Windows.Forms.ToolStripMenuItem medianToolStripMenuItem_Column;
    private System.Windows.Forms.ToolStripMenuItem randomToolStripMenuItem_Column;
    private System.Windows.Forms.ToolStripMenuItem mostCommonToolStripMenuItem_Column;
    private System.Windows.Forms.ToolStripMenuItem interpolationToolStripMenuItem_Column;
    private System.Windows.Forms.ToolStripMenuItem replaceValueOverSelectionToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem averageToolStripMenuItem_Selection;
    private System.Windows.Forms.ToolStripMenuItem medianToolStripMenuItem_Selection;
    private System.Windows.Forms.ToolStripMenuItem randomToolStripMenuItem_Selection;
    private System.Windows.Forms.ToolStripMenuItem mostCommonToolStripMenuItem_Selection;
    private System.Windows.Forms.Button btnSearch;
    private System.Windows.Forms.Button btnReplace;
    private System.Windows.Forms.ToolTip toolTip;
    private System.Windows.Forms.Button addRowButton;
    private System.Windows.Forms.Button addColumnButton;
    private System.Windows.Forms.Button renameColumnsButton;
    private System.Windows.Forms.GroupBox showVariablesGroupBox;
    private System.Windows.Forms.Button checkInputsTargetButton;
    private System.Windows.Forms.Button uncheckAllButton;
    private System.Windows.Forms.Button checkAllButton;
    private System.Windows.Forms.Button shuffleAllButton;
    private System.Windows.Forms.CheckBox shuffleWithinPartitionsCheckBox;
  }
}