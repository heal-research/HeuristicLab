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

namespace HeuristicLab.Problems.LinearAssignment.Views {
  partial class LAPAssignmentView {
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
      this.qualityView = new HeuristicLab.Data.Views.StringConvertibleValueView();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.assignmentDataGridView = new System.Windows.Forms.DataGridView();
      this.RowsColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.Columns = new System.Windows.Forms.DataGridViewTextBoxColumn();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.assignmentDataGridView)).BeginInit();
      this.SuspendLayout();
      // 
      // qualityView
      // 
      this.qualityView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.qualityView.Caption = "StringConvertibleValue View";
      this.qualityView.Content = null;
      this.qualityView.LabelVisible = true;
      this.qualityView.Location = new System.Drawing.Point(0, 3);
      this.qualityView.Name = "qualityView";
      this.qualityView.ReadOnly = true;
      this.qualityView.Size = new System.Drawing.Size(413, 21);
      this.qualityView.TabIndex = 1;
      // 
      // splitContainer
      // 
      this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
      this.splitContainer.Location = new System.Drawing.Point(0, 30);
      this.splitContainer.Name = "splitContainer";
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.assignmentDataGridView);
      this.splitContainer.Size = new System.Drawing.Size(413, 324);
      this.splitContainer.SplitterDistance = 250;
      this.splitContainer.TabIndex = 2;
      // 
      // assignmentDataGridView
      // 
      this.assignmentDataGridView.AllowUserToAddRows = false;
      this.assignmentDataGridView.AllowUserToDeleteRows = false;
      this.assignmentDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.assignmentDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.assignmentDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.RowsColumn,
            this.Columns});
      this.assignmentDataGridView.Location = new System.Drawing.Point(3, 3);
      this.assignmentDataGridView.Name = "assignmentDataGridView";
      this.assignmentDataGridView.ReadOnly = true;
      this.assignmentDataGridView.Size = new System.Drawing.Size(244, 318);
      this.assignmentDataGridView.TabIndex = 0;
      // 
      // RowsColumn
      // 
      this.RowsColumn.HeaderText = "Rows";
      this.RowsColumn.Name = "RowsColumn";
      this.RowsColumn.ReadOnly = true;
      // 
      // Columns
      // 
      this.Columns.HeaderText = "Columns";
      this.Columns.Name = "Columns";
      this.Columns.ReadOnly = true;
      // 
      // LAPAssignmentView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.splitContainer);
      this.Controls.Add(this.qualityView);
      this.Name = "LAPAssignmentView";
      this.Size = new System.Drawing.Size(413, 354);
      this.splitContainer.Panel1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.assignmentDataGridView)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private Data.Views.StringConvertibleValueView qualityView;
    private System.Windows.Forms.SplitContainer splitContainer;
    private System.Windows.Forms.DataGridView assignmentDataGridView;
    private System.Windows.Forms.DataGridViewTextBoxColumn RowsColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn Columns;
  }
}
