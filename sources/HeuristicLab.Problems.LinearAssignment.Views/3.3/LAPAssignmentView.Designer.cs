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
      this.assignmentView = new HeuristicLab.Data.Views.StringConvertibleArrayView();
      this.qualityView = new HeuristicLab.Data.Views.StringConvertibleValueView();
      this.SuspendLayout();
      // 
      // assignmentView
      // 
      this.assignmentView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.assignmentView.Caption = "StringConvertibleArray View";
      this.assignmentView.Content = null;
      this.assignmentView.Location = new System.Drawing.Point(0, 30);
      this.assignmentView.Name = "assignmentView";
      this.assignmentView.ReadOnly = true;
      this.assignmentView.Size = new System.Drawing.Size(422, 130);
      this.assignmentView.TabIndex = 0;
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
      this.qualityView.Size = new System.Drawing.Size(422, 21);
      this.qualityView.TabIndex = 1;
      // 
      // LAPAssignmentView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.qualityView);
      this.Controls.Add(this.assignmentView);
      this.Name = "LAPAssignmentView";
      this.Size = new System.Drawing.Size(422, 160);
      this.ResumeLayout(false);

    }

    #endregion

    private Data.Views.StringConvertibleArrayView assignmentView;
    private Data.Views.StringConvertibleValueView qualityView;
  }
}
