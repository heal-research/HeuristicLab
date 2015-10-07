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
      this.smoothingToolStripMenuItem_Column = new System.Windows.Forms.ToolStripMenuItem();
      this.replaceValueOverSelectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.averageToolStripMenuItem_Selection = new System.Windows.Forms.ToolStripMenuItem();
      this.medianToolStripMenuItem_Selection = new System.Windows.Forms.ToolStripMenuItem();
      this.randomToolStripMenuItem_Selection = new System.Windows.Forms.ToolStripMenuItem();
      this.mostCommonToolStripMenuItem_Selection = new System.Windows.Forms.ToolStripMenuItem();
      this.btnSearch = new System.Windows.Forms.Button();
      this.btnReplace = new System.Windows.Forms.Button();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.addLineButton = new System.Windows.Forms.Button();
      this.addColumnButton = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.contextMenuCell.SuspendLayout();
      this.SuspendLayout();
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
      // btnApplySort
      // 
      this.btnApplySort.Location = new System.Drawing.Point(320, -1);
      this.btnApplySort.Name = "btnApplySort";
      this.btnApplySort.Size = new System.Drawing.Size(87, 23);
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
      this.contextMenuCell.Size = new System.Drawing.Size(225, 48);
      // 
      // replaceValueOverColumnToolStripMenuItem
      // 
      this.replaceValueOverColumnToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.averageToolStripMenuItem_Column,
            this.medianToolStripMenuItem_Column,
            this.randomToolStripMenuItem_Column,
            this.mostCommonToolStripMenuItem_Column,
            this.interpolationToolStripMenuItem_Column,
            this.smoothingToolStripMenuItem_Column});
      this.replaceValueOverColumnToolStripMenuItem.Name = "replaceValueOverColumnToolStripMenuItem";
      this.replaceValueOverColumnToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
      this.replaceValueOverColumnToolStripMenuItem.Text = "Replace Value over Column";
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
      // smoothingToolStripMenuItem_Column
      // 
      this.smoothingToolStripMenuItem_Column.Name = "smoothingToolStripMenuItem_Column";
      this.smoothingToolStripMenuItem_Column.Size = new System.Drawing.Size(155, 22);
      this.smoothingToolStripMenuItem_Column.Text = "Smoothing";
      this.smoothingToolStripMenuItem_Column.Click += new System.EventHandler(this.ReplaceWithSmoothing_Selection_Click);
      // 
      // replaceValueOverSelectionToolStripMenuItem
      // 
      this.replaceValueOverSelectionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.averageToolStripMenuItem_Selection,
            this.medianToolStripMenuItem_Selection,
            this.randomToolStripMenuItem_Selection,
            this.mostCommonToolStripMenuItem_Selection});
      this.replaceValueOverSelectionToolStripMenuItem.Name = "replaceValueOverSelectionToolStripMenuItem";
      this.replaceValueOverSelectionToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
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
      this.btnSearch.Location = new System.Drawing.Point(173, -1);
      this.btnSearch.Name = "btnSearch";
      this.btnSearch.Size = new System.Drawing.Size(53, 23);
      this.btnSearch.TabIndex = 8;
      this.btnSearch.Text = "Search";
      this.toolTip.SetToolTip(this.btnSearch, "Opens the Search dialog");
      this.btnSearch.UseVisualStyleBackColor = true;
      this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
      // 
      // btnReplace
      // 
      this.btnReplace.Location = new System.Drawing.Point(232, -1);
      this.btnReplace.Name = "btnReplace";
      this.btnReplace.Size = new System.Drawing.Size(55, 23);
      this.btnReplace.TabIndex = 9;
      this.btnReplace.Text = "Replace";
      this.toolTip.SetToolTip(this.btnReplace, "Opens the Search & Replace dialog");
      this.btnReplace.UseVisualStyleBackColor = true;
      this.btnReplace.Click += new System.EventHandler(this.btnReplace_Click);
      // 
      // addLineButton
      // 
      this.addLineButton.Location = new System.Drawing.Point(173, 25);
      this.addLineButton.Name = "addLineButton";
      this.addLineButton.Size = new System.Drawing.Size(114, 23);
      this.addLineButton.TabIndex = 10;
      this.addLineButton.Text = "Add Line";
      this.addLineButton.UseVisualStyleBackColor = true;
      this.addLineButton.Click += new System.EventHandler(this.addLineButton_Click);
      // 
      // addColumnButton
      // 
      this.addColumnButton.Location = new System.Drawing.Point(293, 25);
      this.addColumnButton.Name = "addColumnButton";
      this.addColumnButton.Size = new System.Drawing.Size(114, 23);
      this.addColumnButton.TabIndex = 10;
      this.addColumnButton.Text = "Add Column";
      this.addColumnButton.UseVisualStyleBackColor = true;
      this.addColumnButton.Click += new System.EventHandler(this.addColumnButton_Click);
      // 
      // DataGridContentView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.addColumnButton);
      this.Controls.Add(this.addLineButton);
      this.Controls.Add(this.btnReplace);
      this.Controls.Add(this.btnSearch);
      this.Controls.Add(this.btnApplySort);
      this.Name = "DataGridContentView";
      this.Controls.SetChildIndex(this.statisticsTextBox, 0);
      this.Controls.SetChildIndex(this.rowsLabel, 0);
      this.Controls.SetChildIndex(this.columnsLabel, 0);
      this.Controls.SetChildIndex(this.rowsTextBox, 0);
      this.Controls.SetChildIndex(this.columnsTextBox, 0);
      this.Controls.SetChildIndex(this.btnApplySort, 0);
      this.Controls.SetChildIndex(this.btnSearch, 0);
      this.Controls.SetChildIndex(this.btnReplace, 0);
      this.Controls.SetChildIndex(this.addLineButton, 0);
      this.Controls.SetChildIndex(this.addColumnButton, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.contextMenuCell.ResumeLayout(false);
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
    private System.Windows.Forms.ToolStripMenuItem smoothingToolStripMenuItem_Column;
    private System.Windows.Forms.Button addLineButton;
    private System.Windows.Forms.Button addColumnButton;
  }
}