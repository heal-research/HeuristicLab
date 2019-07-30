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

namespace HeuristicLab.Data.Views {
  partial class BoolValueView {
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
      this.valueCheckBox = new System.Windows.Forms.CheckBox();
      this.SuspendLayout();
      // 
      // valueCheckBox
      // 
      this.valueCheckBox.AutoSize = true;
      this.valueCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.valueCheckBox.Location = new System.Drawing.Point(3, 0);
      this.valueCheckBox.Name = "valueCheckBox";
      this.valueCheckBox.Size = new System.Drawing.Size(56, 17);
      this.valueCheckBox.TabIndex = 0;
      this.valueCheckBox.Text = "&Value:";
      this.valueCheckBox.UseVisualStyleBackColor = true;
      this.valueCheckBox.CheckedChanged += new System.EventHandler(this.valueCheckBox_CheckedChanged);
      // 
      // BoolValueView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.valueCheckBox);
      this.Name = "BoolValueView";
      this.Size = new System.Drawing.Size(71, 27);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.CheckBox valueCheckBox;

  }
}
