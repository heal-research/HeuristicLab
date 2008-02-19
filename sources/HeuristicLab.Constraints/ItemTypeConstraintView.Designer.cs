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
  partial class ItemTypeConstraintView {
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
      this.typeTextBox = new System.Windows.Forms.TextBox();
      this.setButton = new System.Windows.Forms.Button();
      this.typeLabel = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // typeTextBox
      // 
      this.typeTextBox.Location = new System.Drawing.Point(45, 3);
      this.typeTextBox.Name = "typeTextBox";
      this.typeTextBox.Size = new System.Drawing.Size(189, 20);
      this.typeTextBox.TabIndex = 0;
      // 
      // setButton
      // 
      this.setButton.Location = new System.Drawing.Point(240, 1);
      this.setButton.Name = "setButton";
      this.setButton.Size = new System.Drawing.Size(44, 23);
      this.setButton.TabIndex = 1;
      this.setButton.Text = "Set";
      this.setButton.UseVisualStyleBackColor = true;
      this.setButton.Click += new System.EventHandler(this.setButton_Click);
      // 
      // typeLabel
      // 
      this.typeLabel.AutoSize = true;
      this.typeLabel.Location = new System.Drawing.Point(5, 6);
      this.typeLabel.Name = "typeLabel";
      this.typeLabel.Size = new System.Drawing.Size(34, 13);
      this.typeLabel.TabIndex = 2;
      this.typeLabel.Text = "Type:";
      // 
      // ItemTypeConstraintView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.typeLabel);
      this.Controls.Add(this.setButton);
      this.Controls.Add(this.typeTextBox);
      this.Name = "ItemTypeConstraintView";
      this.Size = new System.Drawing.Size(287, 26);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox typeTextBox;
    private System.Windows.Forms.Button setButton;
    private System.Windows.Forms.Label typeLabel;
  }
}
