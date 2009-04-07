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
  partial class NotConstraintView {
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
      this.subConstraintViewBase = new HeuristicLab.Core.ViewBase();
      this.subConstraintComboBox = new System.Windows.Forms.ComboBox();
      this.SuspendLayout();
      // 
      // subConstraintViewBase
      // 
      this.subConstraintViewBase.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.subConstraintViewBase.Caption = "View";
      this.subConstraintViewBase.Location = new System.Drawing.Point(0, 30);
      this.subConstraintViewBase.Name = "subConstraintViewBase";
      this.subConstraintViewBase.Size = new System.Drawing.Size(260, 206);
      this.subConstraintViewBase.TabIndex = 0;
      // 
      // subConstraintComboBox
      // 
      this.subConstraintComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.subConstraintComboBox.FormattingEnabled = true;
      this.subConstraintComboBox.Location = new System.Drawing.Point(0, 3);
      this.subConstraintComboBox.Name = "subConstraintComboBox";
      this.subConstraintComboBox.Size = new System.Drawing.Size(260, 21);
      this.subConstraintComboBox.TabIndex = 1;
      this.subConstraintComboBox.SelectedIndexChanged += new System.EventHandler(this.subConstraintComboBox_SelectedIndexChanged);
      // 
      // NotConstraintView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.subConstraintComboBox);
      this.Controls.Add(this.subConstraintViewBase);
      this.Name = "NotConstraintView";
      this.Size = new System.Drawing.Size(263, 239);
      this.ResumeLayout(false);

    }

    #endregion

    private HeuristicLab.Core.ViewBase subConstraintViewBase;
    private System.Windows.Forms.ComboBox subConstraintComboBox;
  }
}
