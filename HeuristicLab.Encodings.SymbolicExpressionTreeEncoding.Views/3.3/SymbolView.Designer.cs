#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views {
  partial class SymbolView {
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
      this.initialFrequencyLabel = new System.Windows.Forms.Label();
      this.initialFrequencyTextBox = new System.Windows.Forms.TextBox();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Location = new System.Drawing.Point(117, 0);
      this.nameTextBox.Size = new System.Drawing.Size(200, 20);
      // 
      // descriptionTextBox
      // 
      this.errorProvider.SetIconAlignment(this.descriptionTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.descriptionTextBox.Location = new System.Drawing.Point(117, 26);
      this.descriptionTextBox.Size = new System.Drawing.Size(200, 20);
      // 
      // initialFrequencyLabel
      // 
      this.initialFrequencyLabel.AutoSize = true;
      this.initialFrequencyLabel.Location = new System.Drawing.Point(3, 56);
      this.initialFrequencyLabel.Name = "initialFrequencyLabel";
      this.initialFrequencyLabel.Size = new System.Drawing.Size(84, 13);
      this.initialFrequencyLabel.TabIndex = 4;
      this.initialFrequencyLabel.Text = "Initial frequency:";
      this.toolTip.SetToolTip(this.initialFrequencyLabel, "Relative frequency of the symbol in randomly created trees");
      // 
      // initialFrequencyTextBox
      // 
      this.initialFrequencyTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.errorProvider.SetIconAlignment(this.initialFrequencyTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.initialFrequencyTextBox.Location = new System.Drawing.Point(117, 53);
      this.initialFrequencyTextBox.Name = "initialFrequencyTextBox";
      this.initialFrequencyTextBox.Size = new System.Drawing.Size(200, 20);
      this.initialFrequencyTextBox.TabIndex = 5;
      this.initialFrequencyTextBox.TextChanged += new System.EventHandler(this.initialFrequencyTextBox_TextChanged);
      // 
      // SymbolView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.initialFrequencyTextBox);
      this.Controls.Add(this.initialFrequencyLabel);
      this.Name = "SymbolView";
      this.Size = new System.Drawing.Size(320, 79);
      this.Controls.SetChildIndex(this.initialFrequencyLabel, 0);
      this.Controls.SetChildIndex(this.initialFrequencyTextBox, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.descriptionLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      this.Controls.SetChildIndex(this.descriptionTextBox, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    protected System.Windows.Forms.Label initialFrequencyLabel;
    protected System.Windows.Forms.TextBox initialFrequencyTextBox;

  }
}
