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

namespace HeuristicLab.Encodings.PermutationEncoding.Views {
  partial class PermutationView {
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
      this.permutationTypeView = new HeuristicLab.Encodings.PermutationEncoding.Views.PermutationTypeView();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // lengthTextBox
      // 
      this.errorProvider.SetIconAlignment(this.lengthTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.lengthTextBox, 2);
      // 
      // permutationTypeView
      // 
      this.permutationTypeView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.permutationTypeView.Caption = "PermutationType View";
      this.permutationTypeView.Content = null;
      this.permutationTypeView.Location = new System.Drawing.Point(6, 26);
      this.permutationTypeView.Name = "permutationTypeView";
      this.permutationTypeView.ReadOnly = false;
      this.permutationTypeView.Size = new System.Drawing.Size(418, 29);
      this.permutationTypeView.TabIndex = 3;
      // 
      // PermutationView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.permutationTypeView);
      this.Name = "PermutationView";
      this.Controls.SetChildIndex(this.lengthLabel, 0);
      this.Controls.SetChildIndex(this.lengthTextBox, 0);
      this.Controls.SetChildIndex(this.permutationTypeView, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private PermutationTypeView permutationTypeView;

  }
}
