#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Operators.Views {
  partial class OperatorView {
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
      this.breakpointLabel = new System.Windows.Forms.Label();
      this.breakpointCheckBox = new System.Windows.Forms.CheckBox();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // parameterCollectionView
      // 
      this.parameterCollectionView.Location = new System.Drawing.Point(0, 72);
      this.parameterCollectionView.Size = new System.Drawing.Size(490, 281);
      this.parameterCollectionView.TabIndex = 6;
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      // 
      // breakpointLabel
      // 
      this.breakpointLabel.AutoSize = true;
      this.breakpointLabel.Location = new System.Drawing.Point(3, 52);
      this.breakpointLabel.Name = "breakpointLabel";
      this.breakpointLabel.Size = new System.Drawing.Size(61, 13);
      this.breakpointLabel.TabIndex = 4;
      this.breakpointLabel.Text = "&Breakpoint:";
      // 
      // breakpointCheckBox
      // 
      this.breakpointCheckBox.AutoSize = true;
      this.breakpointCheckBox.Location = new System.Drawing.Point(72, 52);
      this.breakpointCheckBox.Name = "breakpointCheckBox";
      this.breakpointCheckBox.Size = new System.Drawing.Size(15, 14);
      this.breakpointCheckBox.TabIndex = 5;
      this.breakpointCheckBox.UseVisualStyleBackColor = true;
      this.breakpointCheckBox.CheckedChanged += new System.EventHandler(this.breakpointCheckBox_CheckedChanged);
      this.toolTip.SetToolTip(this.breakpointCheckBox, "Check if an engine should stop execution each time after this operator has been processed.");
      // 
      // OperatorView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.breakpointCheckBox);
      this.Controls.Add(this.breakpointLabel);
      this.Name = "OperatorView";
      this.Controls.SetChildIndex(this.breakpointLabel, 0);
      this.Controls.SetChildIndex(this.breakpointCheckBox, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.descriptionLabel, 0);
      this.Controls.SetChildIndex(this.descriptionTextBox, 0);
      this.Controls.SetChildIndex(this.parameterCollectionView, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    protected System.Windows.Forms.Label breakpointLabel;
    protected System.Windows.Forms.CheckBox breakpointCheckBox;

  }
}
