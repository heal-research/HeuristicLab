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

using System.Windows.Forms;

namespace HeuristicLab.Optimization.Views {
  partial class RunCollectionContentConstraintView {
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
      this.chbActive = new System.Windows.Forms.CheckBox();
      this.runsView = new HeuristicLab.Optimization.Views.RunCollectionContentConstraintView.RunSetView();
      this.SuspendLayout();
      // 
      // chbActive
      // 
      this.chbActive.AutoSize = true;
      this.chbActive.Location = new System.Drawing.Point(3, 3);
      this.chbActive.Name = "chbActive";
      this.chbActive.Size = new System.Drawing.Size(56, 17);
      this.chbActive.TabIndex = 7;
      this.chbActive.Text = "Active";
      this.chbActive.UseVisualStyleBackColor = true;
      this.chbActive.CheckedChanged += new System.EventHandler(this.chbActive_CheckedChanged);
      // 
      // operatorSetView1
      // 
      this.runsView.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
      this.runsView.Caption = "Runs View";
      this.runsView.Content = null;
      this.runsView.Location = new System.Drawing.Point(3, 26);
      this.runsView.Name = "runsView";
      this.runsView.ReadOnly = false;
      this.runsView.Size = new System.Drawing.Size(309, 323);
      this.runsView.TabIndex = 8;
      // 
      // RunCollectionContentConstraintView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.runsView);
      this.Controls.Add(this.chbActive);
      this.Name = "RunCollectionContentConstraintView";
      this.Size = new System.Drawing.Size(315, 352);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.CheckBox chbActive;
    private RunSetView runsView;
  }
}
