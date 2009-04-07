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

namespace HeuristicLab.Data {
  partial class BoolDataView {
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
      this.dataCheckBox = new System.Windows.Forms.CheckBox();
      this.SuspendLayout();
      // 
      // dataCheckBox
      // 
      this.dataCheckBox.AutoSize = true;
      this.dataCheckBox.Location = new System.Drawing.Point(0, 0);
      this.dataCheckBox.Name = "dataCheckBox";
      this.dataCheckBox.Size = new System.Drawing.Size(15, 14);
      this.dataCheckBox.TabIndex = 1;
      this.dataCheckBox.UseVisualStyleBackColor = true;
      this.dataCheckBox.CheckedChanged += new System.EventHandler(this.dataCheckBox_CheckedChanged);
      // 
      // BoolDataView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.dataCheckBox);
      this.Name = "BoolDataView";
      this.Size = new System.Drawing.Size(19, 18);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.CheckBox dataCheckBox;

  }
}
