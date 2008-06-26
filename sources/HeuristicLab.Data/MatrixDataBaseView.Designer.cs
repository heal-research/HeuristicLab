#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System;
namespace HeuristicLab.Data {
  partial class MatrixDataBaseView {
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
      this.rowsLabel = new System.Windows.Forms.Label();
      this.rowsTextBox = new System.Windows.Forms.TextBox();
      this.dataGridView = new System.Windows.Forms.DataGridView();
      this.columnsTextBox = new System.Windows.Forms.TextBox();
      this.columnsLabel = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
      this.SuspendLayout();
      // 
      // rowsLabel
      // 
      this.rowsLabel.AutoSize = true;
      this.rowsLabel.Location = new System.Drawing.Point(3, 4);
      this.rowsLabel.Name = "rowsLabel";
      this.rowsLabel.Size = new System.Drawing.Size(37, 13);
      this.rowsLabel.TabIndex = 0;
      this.rowsLabel.Text = "Rows:";
      // 
      // rowsTextBox
      // 
      this.rowsTextBox.AcceptsReturn = true;
      this.rowsTextBox.Location = new System.Drawing.Point(39, 1);
      this.rowsTextBox.Name = "rowsTextBox";
      this.rowsTextBox.Size = new System.Drawing.Size(110, 20);
      this.rowsTextBox.TabIndex = 1;
      this.rowsTextBox.Text = "0";
      this.rowsTextBox.Validated += new System.EventHandler(this.textBox_Validated);
      this.rowsTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_KeyDown);
      this.rowsTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_Validating);
      // 
      // dataGridView
      // 
      this.dataGridView.AllowUserToAddRows = false;
      this.dataGridView.AllowUserToDeleteRows = false;
      this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dataGridView.ColumnHeadersVisible = false;
      this.dataGridView.Location = new System.Drawing.Point(3, 27);
      this.dataGridView.Name = "dataGridView";
      this.dataGridView.Size = new System.Drawing.Size(383, 293);
      this.dataGridView.TabIndex = 2;
      this.dataGridView.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dataGridView_CellValidating);
      // 
      // columnsTextBox
      // 
      this.columnsTextBox.AcceptsReturn = true;
      this.columnsTextBox.Location = new System.Drawing.Point(215, 1);
      this.columnsTextBox.Name = "columnsTextBox";
      this.columnsTextBox.Size = new System.Drawing.Size(110, 20);
      this.columnsTextBox.TabIndex = 4;
      this.columnsTextBox.Text = "0";
      this.columnsTextBox.Validated += new System.EventHandler(this.textBox_Validated);
      this.columnsTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_KeyDown);
      this.columnsTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_Validating);
      // 
      // columnsLabel
      // 
      this.columnsLabel.AutoSize = true;
      this.columnsLabel.Location = new System.Drawing.Point(159, 4);
      this.columnsLabel.Name = "columnsLabel";
      this.columnsLabel.Size = new System.Drawing.Size(50, 13);
      this.columnsLabel.TabIndex = 3;
      this.columnsLabel.Text = "Columns:";
      // 
      // MatrixDataBaseView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.columnsTextBox);
      this.Controls.Add(this.columnsLabel);
      this.Controls.Add(this.dataGridView);
      this.Controls.Add(this.rowsTextBox);
      this.Controls.Add(this.rowsLabel);
      this.Name = "MatrixDataBaseView";
      this.Size = new System.Drawing.Size(391, 323);
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label rowsLabel;
    private System.Windows.Forms.TextBox rowsTextBox;
    protected System.Windows.Forms.DataGridView dataGridView;
    private System.Windows.Forms.TextBox columnsTextBox;
    private System.Windows.Forms.Label columnsLabel;
  }
}
