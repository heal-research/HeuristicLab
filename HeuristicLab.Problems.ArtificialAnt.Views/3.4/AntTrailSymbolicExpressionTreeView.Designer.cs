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

namespace HeuristicLab.Problems.ArtificialAnt.Views {
  partial class AntTrailSymbolicExpressionTreeView {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing) {
        if (components != null) components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      this.animationTimer = new System.Windows.Forms.Timer(this.components);
      this.symExpressionGroupBox = new System.Windows.Forms.GroupBox();
      this.SuspendLayout();
      // 
      // animationTimer
      // 
      this.animationTimer.Interval = 15;
      // 
      // symExpressionGroupBox
      // 
      this.symExpressionGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.symExpressionGroupBox.Location = new System.Drawing.Point(3, 3);
      this.symExpressionGroupBox.Name = "symExpressionGroupBox";
      this.symExpressionGroupBox.Size = new System.Drawing.Size(386, 304);
      this.symExpressionGroupBox.TabIndex = 0;
      this.symExpressionGroupBox.TabStop = false;
      this.symExpressionGroupBox.Text = "Symbolic Expression Tree";
      // 
      // AntTrailSymbolicExpressionTreeView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.symExpressionGroupBox);
      this.Name = "AntTrailSymbolicExpressionTreeView";
      this.Size = new System.Drawing.Size(392, 310);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Timer animationTimer;
    private System.Windows.Forms.GroupBox symExpressionGroupBox;


  }
}
