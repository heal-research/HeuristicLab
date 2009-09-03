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

using System;
namespace HeuristicLab.GP {
  partial class GeneticProgrammingModelView {
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
      this.sizeLabel = new System.Windows.Forms.Label();
      this.sizeTextBox = new System.Windows.Forms.TextBox();
      this.heightLabel = new System.Windows.Forms.Label();
      this.heightTextBox = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // funTreeView
      // 
      this.funTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.funTreeView.LineColor = System.Drawing.Color.Black;
      this.funTreeView.Location = new System.Drawing.Point(0, 55);
      this.funTreeView.Size = new System.Drawing.Size(428, 429);
      // 
      // sizeLabel
      // 
      this.sizeLabel.AutoSize = true;
      this.sizeLabel.Location = new System.Drawing.Point(17, 6);
      this.sizeLabel.Name = "sizeLabel";
      this.sizeLabel.Size = new System.Drawing.Size(30, 13);
      this.sizeLabel.TabIndex = 1;
      this.sizeLabel.Text = "Size:";
      // 
      // sizeTextBox
      // 
      this.sizeTextBox.Location = new System.Drawing.Point(58, 3);
      this.sizeTextBox.Name = "sizeTextBox";
      this.sizeTextBox.ReadOnly = true;
      this.sizeTextBox.Size = new System.Drawing.Size(100, 20);
      this.sizeTextBox.TabIndex = 2;
      // 
      // heightLabel
      // 
      this.heightLabel.AutoSize = true;
      this.heightLabel.Location = new System.Drawing.Point(6, 32);
      this.heightLabel.Name = "heightLabel";
      this.heightLabel.Size = new System.Drawing.Size(41, 13);
      this.heightLabel.TabIndex = 3;
      this.heightLabel.Text = "Height:";
      // 
      // heightTextBox
      // 
      this.heightTextBox.Location = new System.Drawing.Point(58, 29);
      this.heightTextBox.Name = "heightTextBox";
      this.heightTextBox.ReadOnly = true;
      this.heightTextBox.Size = new System.Drawing.Size(100, 20);
      this.heightTextBox.TabIndex = 4;
      // 
      // GeneticProgrammingModelView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.Controls.Add(this.heightLabel);
      this.Controls.Add(this.heightTextBox);
      this.Controls.Add(this.sizeLabel);
      this.Controls.Add(this.sizeTextBox);
      this.Name = "GeneticProgrammingModelView";
      this.Size = new System.Drawing.Size(428, 487);
      this.Controls.SetChildIndex(this.sizeTextBox, 0);
      this.Controls.SetChildIndex(this.sizeLabel, 0);
      this.Controls.SetChildIndex(this.funTreeView, 0);
      this.Controls.SetChildIndex(this.heightTextBox, 0);
      this.Controls.SetChildIndex(this.heightLabel, 0);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    protected System.Windows.Forms.Label sizeLabel;
    protected System.Windows.Forms.TextBox sizeTextBox;
    protected System.Windows.Forms.Label heightLabel;
    protected System.Windows.Forms.TextBox heightTextBox;

  }
}
