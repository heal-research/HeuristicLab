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

namespace HeuristicLab.Algorithms.DataAnalysis.Views {
  partial class OneFactorClassificationModelView {
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
      this.dataGridView = new System.Windows.Forms.DataGridView();
      this.variableValueColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.classcolumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.variableLabel = new System.Windows.Forms.Label();
      this.DefaultClassValueLabel = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
      this.SuspendLayout();
      // 
      // dataGridView
      // 
      this.dataGridView.AllowUserToAddRows = false;
      this.dataGridView.AllowUserToDeleteRows = false;
      this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.variableValueColumn,
            this.classcolumn});
      this.dataGridView.Location = new System.Drawing.Point(3, 32);
      this.dataGridView.Name = "dataGridView";
      this.dataGridView.ReadOnly = true;
      this.dataGridView.Size = new System.Drawing.Size(389, 230);
      this.dataGridView.TabIndex = 0;
      // 
      // intervalstart
      // 
      this.variableValueColumn.HeaderText = "Factor Value";
      this.variableValueColumn.Name = "Factor value";
      this.variableValueColumn.ReadOnly = true;
      // 
      // classcolumn
      // 
      this.classcolumn.HeaderText = "Class";
      this.classcolumn.Name = "classcolumn";
      this.classcolumn.ReadOnly = true;
      // 
      // variableLabel
      // 
      this.variableLabel.AutoSize = true;
      this.variableLabel.Location = new System.Drawing.Point(3, 10);
      this.variableLabel.Name = "variableLabel";
      this.variableLabel.Size = new System.Drawing.Size(48, 13);
      this.variableLabel.TabIndex = 1;
      this.variableLabel.Text = "Variable:";
      // 
      // DefaultClassValueLabel
      // 
      this.DefaultClassValueLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
      this.DefaultClassValueLabel.AutoSize = true;
      this.DefaultClassValueLabel.Location = new System.Drawing.Point(165, 10);
      this.DefaultClassValueLabel.Name = "DefaultClassValueLabel";
      this.DefaultClassValueLabel.Size = new System.Drawing.Size(118, 13);
      this.DefaultClassValueLabel.TabIndex = 2;
      this.DefaultClassValueLabel.Text = "Default class label:";
      // 
      // OneFactorClassificationModelView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.DefaultClassValueLabel);
      this.Controls.Add(this.variableLabel);
      this.Controls.Add(this.dataGridView);
      this.Name = "OneFactorClassificationModelView";
      this.Size = new System.Drawing.Size(395, 265);
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.DataGridView dataGridView;
    private System.Windows.Forms.Label variableLabel;
    private System.Windows.Forms.DataGridViewTextBoxColumn variableValueColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn classcolumn;
    private System.Windows.Forms.Label DefaultClassValueLabel;
  }
}
