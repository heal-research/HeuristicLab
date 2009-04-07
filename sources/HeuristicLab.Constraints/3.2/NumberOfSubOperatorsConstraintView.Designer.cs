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
  partial class NumberOfSubOperatorsConstraintView {
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
      this.minDataView = new HeuristicLab.Data.IntDataView();
      this.maxDataView = new HeuristicLab.Data.IntDataView();
      this.minLabel = new System.Windows.Forms.Label();
      this.maxLabel = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // minDataView
      // 
      this.minDataView.Caption = "View";
      this.minDataView.IntData = null;
      this.minDataView.Location = new System.Drawing.Point(36, 3);
      this.minDataView.Name = "minDataView";
      this.minDataView.Size = new System.Drawing.Size(265, 26);
      this.minDataView.TabIndex = 0;
      // 
      // maxDataView
      // 
      this.maxDataView.Caption = "View";
      this.maxDataView.IntData = null;
      this.maxDataView.Location = new System.Drawing.Point(36, 32);
      this.maxDataView.Name = "maxDataView";
      this.maxDataView.Size = new System.Drawing.Size(265, 26);
      this.maxDataView.TabIndex = 1;
      // 
      // minLabel
      // 
      this.minLabel.AutoSize = true;
      this.minLabel.Location = new System.Drawing.Point(3, 3);
      this.minLabel.Name = "minLabel";
      this.minLabel.Size = new System.Drawing.Size(27, 13);
      this.minLabel.TabIndex = 2;
      this.minLabel.Text = "Min:";
      // 
      // maxLabel
      // 
      this.maxLabel.AutoSize = true;
      this.maxLabel.Location = new System.Drawing.Point(3, 32);
      this.maxLabel.Name = "maxLabel";
      this.maxLabel.Size = new System.Drawing.Size(30, 13);
      this.maxLabel.TabIndex = 3;
      this.maxLabel.Text = "Max:";
      // 
      // NumberOfSubOperatorsConstraintView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.maxLabel);
      this.Controls.Add(this.minLabel);
      this.Controls.Add(this.maxDataView);
      this.Controls.Add(this.minDataView);
      this.Name = "NumberOfSubOperatorsConstraintView";
      this.Size = new System.Drawing.Size(306, 61);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private HeuristicLab.Data.IntDataView minDataView;
    private HeuristicLab.Data.IntDataView maxDataView;
    private System.Windows.Forms.Label minLabel;
    private System.Windows.Forms.Label maxLabel;
  }
}
