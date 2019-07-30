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

namespace HeuristicLab.Optimization.Views {
  partial class RunCollectionColumnConstraintView {
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
      this.lblConstrainedMember = new System.Windows.Forms.Label();
      this.lblComparisonOperation = new System.Windows.Forms.Label();
      this.lblCompareValue = new System.Windows.Forms.Label();
      this.cmbConstraintOperation = new System.Windows.Forms.ComboBox();
      this.chbActive = new System.Windows.Forms.CheckBox();
      this.cmbConstraintColumn = new System.Windows.Forms.ComboBox();
      this.SuspendLayout();
      // 
      // lblConstrainedMember
      // 
      this.lblConstrainedMember.AutoSize = true;
      this.lblConstrainedMember.Location = new System.Drawing.Point(3, 6);
      this.lblConstrainedMember.Name = "lblConstrainedMember";
      this.lblConstrainedMember.Size = new System.Drawing.Size(107, 13);
      this.lblConstrainedMember.TabIndex = 0;
      this.lblConstrainedMember.Text = "Constrained Member:";
      // 
      // lblComparisonOperation
      // 
      this.lblComparisonOperation.AutoSize = true;
      this.lblComparisonOperation.Location = new System.Drawing.Point(3, 32);
      this.lblComparisonOperation.Name = "lblComparisonOperation";
      this.lblComparisonOperation.Size = new System.Drawing.Size(106, 13);
      this.lblComparisonOperation.TabIndex = 1;
      this.lblComparisonOperation.Text = "Constraint Operation:";
      // 
      // lblCompareValue
      // 
      this.lblCompareValue.AutoSize = true;
      this.lblCompareValue.Location = new System.Drawing.Point(3, 59);
      this.lblCompareValue.Name = "lblCompareValue";
      this.lblCompareValue.Size = new System.Drawing.Size(83, 13);
      this.lblCompareValue.TabIndex = 2;
      this.lblCompareValue.Text = "Constraint Data:";
      // 
      // cmbConstraintOperation
      // 
      this.cmbConstraintOperation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.cmbConstraintOperation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cmbConstraintOperation.FormattingEnabled = true;
      this.cmbConstraintOperation.Location = new System.Drawing.Point(127, 29);
      this.cmbConstraintOperation.Name = "cmbConstraintOperation";
      this.cmbConstraintOperation.Size = new System.Drawing.Size(246, 21);
      this.cmbConstraintOperation.TabIndex = 4;
      this.cmbConstraintOperation.SelectedIndexChanged += new System.EventHandler(this.cmbComparisonOperation_SelectedIndexChanged);
      // 
      // chbActive
      // 
      this.chbActive.AutoSize = true;
      this.chbActive.Location = new System.Drawing.Point(127, 82);
      this.chbActive.Name = "chbActive";
      this.chbActive.Size = new System.Drawing.Size(56, 17);
      this.chbActive.TabIndex = 7;
      this.chbActive.Text = "Active";
      this.chbActive.UseVisualStyleBackColor = true;
      this.chbActive.CheckedChanged += new System.EventHandler(this.chbActive_CheckedChanged);
      // 
      // cmbConstraintColumn
      // 
      this.cmbConstraintColumn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.cmbConstraintColumn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cmbConstraintColumn.FormattingEnabled = true;
      this.cmbConstraintColumn.Location = new System.Drawing.Point(127, 3);
      this.cmbConstraintColumn.Name = "cmbConstraintColumn";
      this.cmbConstraintColumn.Size = new System.Drawing.Size(246, 21);
      this.cmbConstraintColumn.TabIndex = 8;
      this.cmbConstraintColumn.SelectedIndexChanged += new System.EventHandler(this.cmbConstraintColumn_SelectedIndexChanged);
      // 
      // RunCollectionConstraintView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.cmbConstraintColumn);
      this.Controls.Add(this.chbActive);
      this.Controls.Add(this.cmbConstraintOperation);
      this.Controls.Add(this.lblCompareValue);
      this.Controls.Add(this.lblComparisonOperation);
      this.Controls.Add(this.lblConstrainedMember);
      this.Name = "RunCollectionConstraintView";
      this.Size = new System.Drawing.Size(376, 102);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label lblConstrainedMember;
    private System.Windows.Forms.Label lblComparisonOperation;
    private System.Windows.Forms.Label lblCompareValue;
    private System.Windows.Forms.CheckBox chbActive;
    protected System.Windows.Forms.ComboBox cmbConstraintColumn;
    protected System.Windows.Forms.ComboBox cmbConstraintOperation;
  }
}
