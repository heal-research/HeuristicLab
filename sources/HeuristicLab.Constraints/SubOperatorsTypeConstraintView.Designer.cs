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

namespace HeuristicLab.Constraints {
  partial class SubOperatorsTypeConstraintView {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if(disposing && (components != null)) {
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
      this.indexDataView = new HeuristicLab.Data.IntDataView();
      this.indexLabel = new System.Windows.Forms.Label();
      this.allSubOperatorsTypeConstraintView = new HeuristicLab.Constraints.AllSubOperatorsTypeConstraintView();
      this.SuspendLayout();
      // 
      // indexDataView
      // 
      this.indexDataView.Caption = "View";
      this.indexDataView.IntData = null;
      this.indexDataView.Location = new System.Drawing.Point(109, 3);
      this.indexDataView.Name = "indexDataView";
      this.indexDataView.Size = new System.Drawing.Size(265, 26);
      this.indexDataView.TabIndex = 1;
      // 
      // indexLabel
      // 
      this.indexLabel.AutoSize = true;
      this.indexLabel.Location = new System.Drawing.Point(4, 4);
      this.indexLabel.Name = "indexLabel";
      this.indexLabel.Size = new System.Drawing.Size(99, 13);
      this.indexLabel.TabIndex = 2;
      this.indexLabel.Text = "Sub-operator index:";
      // 
      // allSubOperatorsTypeConstraintView
      // 
      this.allSubOperatorsTypeConstraintView.Caption = "View";
      this.allSubOperatorsTypeConstraintView.Location = new System.Drawing.Point(0, 35);
      this.allSubOperatorsTypeConstraintView.Name = "allSubOperatorsTypeConstraintView";
      this.allSubOperatorsTypeConstraintView.Size = new System.Drawing.Size(492, 337);
      this.allSubOperatorsTypeConstraintView.TabIndex = 3;
      // 
      // SubOperatorsTypeConstraintView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.allSubOperatorsTypeConstraintView);
      this.Controls.Add(this.indexLabel);
      this.Controls.Add(this.indexDataView);
      this.Name = "SubOperatorsTypeConstraintView";
      this.Size = new System.Drawing.Size(496, 377);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private HeuristicLab.Data.IntDataView indexDataView;
    private System.Windows.Forms.Label indexLabel;
    private AllSubOperatorsTypeConstraintView allSubOperatorsTypeConstraintView;
  }
}
