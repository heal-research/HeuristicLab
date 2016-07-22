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

namespace HeuristicLab.Clients.OKB.RunCreation {
  partial class OKBExperimentUploadView {
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
      DisposeSpecific();
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      this.btnUpload = new System.Windows.Forms.Button();
      this.dataGridView = new System.Windows.Forms.DataGridView();
      this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.setColumnToThisValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.clearButton = new System.Windows.Forms.Button();
      this.RunNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.AlgorithmNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.AlgorithmTypeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.OKBAlgorithmColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
      this.ProblemNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.ProblemTypeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.OKBProblemColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
      this.UploadColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
      this.contextMenu.SuspendLayout();
      this.SuspendLayout();
      // 
      // btnUpload
      // 
      this.btnUpload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btnUpload.Location = new System.Drawing.Point(4, 200);
      this.btnUpload.Name = "btnUpload";
      this.btnUpload.Size = new System.Drawing.Size(89, 23);
      this.btnUpload.TabIndex = 1;
      this.btnUpload.Text = "Upload Runs";
      this.btnUpload.UseVisualStyleBackColor = true;
      this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
      // 
      // dataGridView
      // 
      this.dataGridView.AllowDrop = true;
      this.dataGridView.AllowUserToAddRows = false;
      this.dataGridView.AllowUserToDeleteRows = false;
      this.dataGridView.AllowUserToOrderColumns = true;
      this.dataGridView.AllowUserToResizeRows = false;
      this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
      this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.RunNameColumn,
            this.AlgorithmNameColumn,
            this.AlgorithmTypeColumn,
            this.OKBAlgorithmColumn,
            this.ProblemNameColumn,
            this.ProblemTypeColumn,
            this.OKBProblemColumn,
            this.UploadColumn});
      this.dataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
      this.dataGridView.Location = new System.Drawing.Point(3, 3);
      this.dataGridView.Name = "dataGridView";
      this.dataGridView.Size = new System.Drawing.Size(460, 191);
      this.dataGridView.TabIndex = 2;
      this.dataGridView.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView_CellMouseClick);
      this.dataGridView.DragDrop += new System.Windows.Forms.DragEventHandler(this.dataGridView_DragDrop);
      this.dataGridView.DragEnter += new System.Windows.Forms.DragEventHandler(this.dataGridView_DragEnter);
      // 
      // contextMenu
      // 
      this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setColumnToThisValueToolStripMenuItem});
      this.contextMenu.Name = "contextMenu";
      this.contextMenu.Size = new System.Drawing.Size(202, 26);
      // 
      // setColumnToThisValueToolStripMenuItem
      // 
      this.setColumnToThisValueToolStripMenuItem.Name = "setColumnToThisValueToolStripMenuItem";
      this.setColumnToThisValueToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
      this.setColumnToThisValueToolStripMenuItem.Text = "Set column to this value";
      this.setColumnToThisValueToolStripMenuItem.Click += new System.EventHandler(this.setColumnToThisValueToolStripMenuItem_Click);
      // 
      // clearButton
      // 
      this.clearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.clearButton.Location = new System.Drawing.Point(387, 199);
      this.clearButton.Name = "clearButton";
      this.clearButton.Size = new System.Drawing.Size(75, 23);
      this.clearButton.TabIndex = 3;
      this.clearButton.Text = "Clear Runs";
      this.clearButton.UseVisualStyleBackColor = true;
      this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
      // 
      // RunNameColumn
      // 
      this.RunNameColumn.HeaderText = "Run Name";
      this.RunNameColumn.Name = "RunNameColumn";
      this.RunNameColumn.ReadOnly = true;
      // 
      // AlgorithmNameColumn
      // 
      this.AlgorithmNameColumn.HeaderText = "Algorithm Name";
      this.AlgorithmNameColumn.Name = "AlgorithmNameColumn";
      this.AlgorithmNameColumn.ReadOnly = true;
      // 
      // AlgorithmTypeColumn
      // 
      this.AlgorithmTypeColumn.HeaderText = "Algorithm Type";
      this.AlgorithmTypeColumn.Name = "AlgorithmTypeColumn";
      this.AlgorithmTypeColumn.ReadOnly = true;
      // 
      // OKBAlgorithmColumn
      // 
      this.OKBAlgorithmColumn.HeaderText = "OKB Algorithm";
      this.OKBAlgorithmColumn.Name = "OKBAlgorithmColumn";
      // 
      // ProblemNameColumn
      // 
      this.ProblemNameColumn.HeaderText = "Problem Name";
      this.ProblemNameColumn.Name = "ProblemNameColumn";
      this.ProblemNameColumn.ReadOnly = true;
      this.ProblemNameColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
      this.ProblemNameColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      // 
      // ProblemTypeColumn
      // 
      this.ProblemTypeColumn.HeaderText = "Problem Type";
      this.ProblemTypeColumn.Name = "ProblemTypeColumn";
      this.ProblemTypeColumn.ReadOnly = true;
      this.ProblemTypeColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
      this.ProblemTypeColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      // 
      // OKBProblemColumn
      // 
      this.OKBProblemColumn.HeaderText = "OKB Problem";
      this.OKBProblemColumn.Name = "OKBProblemColumn";
      // 
      // UploadColumn
      // 
      this.UploadColumn.HeaderText = "Upload?";
      this.UploadColumn.Name = "UploadColumn";
      this.UploadColumn.ToolTipText = "Whether the run should be uploaded or not.";
      // 
      // OKBExperimentUploadView
      // 
      this.AllowDrop = true;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.clearButton);
      this.Controls.Add(this.dataGridView);
      this.Controls.Add(this.btnUpload);
      this.Name = "OKBExperimentUploadView";
      this.Size = new System.Drawing.Size(466, 229);
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
      this.contextMenu.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button btnUpload;
    private System.Windows.Forms.DataGridView dataGridView;
    private System.Windows.Forms.ContextMenuStrip contextMenu;
    private System.Windows.Forms.ToolStripMenuItem setColumnToThisValueToolStripMenuItem;
    private System.Windows.Forms.Button clearButton;
    private System.Windows.Forms.DataGridViewTextBoxColumn RunNameColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn AlgorithmNameColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn AlgorithmTypeColumn;
    private System.Windows.Forms.DataGridViewComboBoxColumn OKBAlgorithmColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn ProblemNameColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn ProblemTypeColumn;
    private System.Windows.Forms.DataGridViewComboBoxColumn OKBProblemColumn;
    private System.Windows.Forms.DataGridViewCheckBoxColumn UploadColumn;
  }
}
