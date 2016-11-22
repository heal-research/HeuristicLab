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

namespace HeuristicLab.DataPreprocessing.Views {
  partial class StatisticsView {
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
      this.lblRows = new System.Windows.Forms.Label();
      this.txtRows = new System.Windows.Forms.Label();
      this.lblColumns = new System.Windows.Forms.Label();
      this.lblMissingValuesTotal = new System.Windows.Forms.Label();
      this.txtColumns = new System.Windows.Forms.Label();
      this.txtNumericColumns = new System.Windows.Forms.Label();
      this.txtNominalColumns = new System.Windows.Forms.Label();
      this.lblNumericColumns = new System.Windows.Forms.Label();
      this.lblNominalColumns = new System.Windows.Forms.Label();
      this.txtMissingValuesTotal = new System.Windows.Forms.Label();
      this.dataGridView = new System.Windows.Forms.DataGridView();
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
      this.SuspendLayout();
      // 
      // lblRows
      // 
      this.lblRows.AutoSize = true;
      this.lblRows.Location = new System.Drawing.Point(3, 11);
      this.lblRows.Name = "lblRows";
      this.lblRows.Size = new System.Drawing.Size(34, 13);
      this.lblRows.TabIndex = 0;
      this.lblRows.Text = "Rows";
      // 
      // txtRows
      // 
      this.txtRows.AutoSize = true;
      this.txtRows.Location = new System.Drawing.Point(115, 11);
      this.txtRows.Name = "txtRows";
      this.txtRows.Size = new System.Drawing.Size(19, 13);
      this.txtRows.TabIndex = 1;
      this.txtRows.Text = "12";
      // 
      // lblColumns
      // 
      this.lblColumns.AutoSize = true;
      this.lblColumns.Location = new System.Drawing.Point(3, 31);
      this.lblColumns.Name = "lblColumns";
      this.lblColumns.Size = new System.Drawing.Size(47, 13);
      this.lblColumns.TabIndex = 2;
      this.lblColumns.Text = "Columns";
      // 
      // lblMissingValuesTotal
      // 
      this.lblMissingValuesTotal.AutoSize = true;
      this.lblMissingValuesTotal.Location = new System.Drawing.Point(3, 92);
      this.lblMissingValuesTotal.Name = "lblMissingValuesTotal";
      this.lblMissingValuesTotal.Size = new System.Drawing.Size(100, 13);
      this.lblMissingValuesTotal.TabIndex = 3;
      this.lblMissingValuesTotal.Text = "Missing Values total";
      // 
      // txtColumns
      // 
      this.txtColumns.AutoSize = true;
      this.txtColumns.Location = new System.Drawing.Point(115, 31);
      this.txtColumns.Name = "txtColumns";
      this.txtColumns.Size = new System.Drawing.Size(25, 13);
      this.txtColumns.TabIndex = 1;
      this.txtColumns.Text = "123";
      // 
      // txtNumericColumns
      // 
      this.txtNumericColumns.AutoSize = true;
      this.txtNumericColumns.Location = new System.Drawing.Point(115, 51);
      this.txtNumericColumns.Name = "txtNumericColumns";
      this.txtNumericColumns.Size = new System.Drawing.Size(25, 13);
      this.txtNumericColumns.TabIndex = 1;
      this.txtNumericColumns.Text = "456";
      // 
      // txtNominalColumns
      // 
      this.txtNominalColumns.AutoSize = true;
      this.txtNominalColumns.Location = new System.Drawing.Point(115, 72);
      this.txtNominalColumns.Name = "txtNominalColumns";
      this.txtNominalColumns.Size = new System.Drawing.Size(25, 13);
      this.txtNominalColumns.TabIndex = 1;
      this.txtNominalColumns.Text = "789";
      // 
      // lblNumericColumns
      // 
      this.lblNumericColumns.AutoSize = true;
      this.lblNumericColumns.Location = new System.Drawing.Point(3, 51);
      this.lblNumericColumns.Name = "lblNumericColumns";
      this.lblNumericColumns.Size = new System.Drawing.Size(89, 13);
      this.lblNumericColumns.TabIndex = 3;
      this.lblNumericColumns.Text = "Numeric Columns";
      // 
      // lblNominalColumns
      // 
      this.lblNominalColumns.AutoSize = true;
      this.lblNominalColumns.Location = new System.Drawing.Point(3, 72);
      this.lblNominalColumns.Name = "lblNominalColumns";
      this.lblNominalColumns.Size = new System.Drawing.Size(88, 13);
      this.lblNominalColumns.TabIndex = 3;
      this.lblNominalColumns.Text = "Nominal Columns";
      // 
      // txtMissingValuesTotal
      // 
      this.txtMissingValuesTotal.AutoSize = true;
      this.txtMissingValuesTotal.Location = new System.Drawing.Point(115, 92);
      this.txtMissingValuesTotal.Name = "txtMissingValuesTotal";
      this.txtMissingValuesTotal.Size = new System.Drawing.Size(25, 13);
      this.txtMissingValuesTotal.TabIndex = 1;
      this.txtMissingValuesTotal.Text = "102";
      // 
      // dataGridView
      // 
      this.dataGridView.AllowUserToAddRows = false;
      this.dataGridView.AllowUserToDeleteRows = false;
      this.dataGridView.AllowUserToOrderColumns = true;
      this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dataGridView.Location = new System.Drawing.Point(5, 117);
      this.dataGridView.Name = "dataGridView";
      this.dataGridView.ReadOnly = true;
      this.dataGridView.RowHeadersWidth = 80;
      this.dataGridView.Size = new System.Drawing.Size(530, 278);
      this.dataGridView.TabIndex = 4;
      this.dataGridView.VirtualMode = true;
      this.dataGridView.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dataGridView_CellValueNeeded);
      // 
      // StatisticsView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.dataGridView);
      this.Controls.Add(this.lblNominalColumns);
      this.Controls.Add(this.lblNumericColumns);
      this.Controls.Add(this.lblMissingValuesTotal);
      this.Controls.Add(this.txtMissingValuesTotal);
      this.Controls.Add(this.txtNominalColumns);
      this.Controls.Add(this.lblColumns);
      this.Controls.Add(this.txtNumericColumns);
      this.Controls.Add(this.txtColumns);
      this.Controls.Add(this.txtRows);
      this.Controls.Add(this.lblRows);
      this.Name = "StatisticsView";
      this.Size = new System.Drawing.Size(549, 408);
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label lblRows;
    private System.Windows.Forms.Label txtRows;
    private System.Windows.Forms.Label lblColumns;
    private System.Windows.Forms.Label lblMissingValuesTotal;
    private System.Windows.Forms.Label txtColumns;
    private System.Windows.Forms.Label txtNumericColumns;
    private System.Windows.Forms.Label txtNominalColumns;
    private System.Windows.Forms.Label lblNumericColumns;
    private System.Windows.Forms.Label lblNominalColumns;
    private System.Windows.Forms.Label txtMissingValuesTotal;
    private System.Windows.Forms.DataGridView dataGridView;
  }
}
