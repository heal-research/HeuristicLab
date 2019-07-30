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


namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Views {
  partial class LaggedSymbolView {
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
      this.minTimeOffsetLabel = new System.Windows.Forms.Label();
      this.maxTimeOffsetLabel = new System.Windows.Forms.Label();
      this.minTimeOffsetTextBox = new System.Windows.Forms.TextBox();
      this.maxTimeOffsetTextBox = new System.Windows.Forms.TextBox();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // initialFrequencyLabel
      // 
      this.toolTip.SetToolTip(this.initialFrequencyLabel, "Relative frequency of the symbol in randomly created trees");
      // 
      // initialFrequencyTextBox
      // 
      this.errorProvider.SetIconAlignment(this.initialFrequencyTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.initialFrequencyTextBox.Size = new System.Drawing.Size(315, 20);
      // 
      // minimumArityTextBox
      // 
      this.errorProvider.SetIconAlignment(this.minimumArityTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.minimumArityTextBox.Size = new System.Drawing.Size(315, 20);
      // 
      // maximumArityTextBox
      // 
      this.errorProvider.SetIconAlignment(this.maximumArityTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.maximumArityTextBox.Size = new System.Drawing.Size(315, 20);
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Size = new System.Drawing.Size(290, 20);
      // 
      // infoLabel
      // 
      this.infoLabel.Location = new System.Drawing.Point(389, 3);
      // 
      // minTimeOffsetLabel
      // 
      this.minTimeOffsetLabel.AutoSize = true;
      this.minTimeOffsetLabel.Location = new System.Drawing.Point(3, 130);
      this.minTimeOffsetLabel.Name = "minTimeOffsetLabel";
      this.minTimeOffsetLabel.Size = new System.Drawing.Size(81, 13);
      this.minTimeOffsetLabel.TabIndex = 5;
      this.minTimeOffsetLabel.Text = "Min. time offset:";
      // 
      // maxTimeOffsetLabel
      // 
      this.maxTimeOffsetLabel.AutoSize = true;
      this.maxTimeOffsetLabel.Location = new System.Drawing.Point(3, 156);
      this.maxTimeOffsetLabel.Name = "maxTimeOffsetLabel";
      this.maxTimeOffsetLabel.Size = new System.Drawing.Size(84, 13);
      this.maxTimeOffsetLabel.TabIndex = 7;
      this.maxTimeOffsetLabel.Text = "Max. time offset:";
      // 
      // minTimeOffsetTextBox
      // 
      this.minTimeOffsetTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.minTimeOffsetTextBox.Location = new System.Drawing.Point(93, 127);
      this.minTimeOffsetTextBox.Name = "minTimeOffsetTextBox";
      this.minTimeOffsetTextBox.Size = new System.Drawing.Size(315, 20);
      this.minTimeOffsetTextBox.TabIndex = 6;
      this.minTimeOffsetTextBox.TextChanged += new System.EventHandler(this.minTimeOffsetTextBox_TextChanged);
      // 
      // maxTimeOffsetTextBox
      // 
      this.maxTimeOffsetTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.maxTimeOffsetTextBox.Location = new System.Drawing.Point(93, 153);
      this.maxTimeOffsetTextBox.Name = "maxTimeOffsetTextBox";
      this.maxTimeOffsetTextBox.Size = new System.Drawing.Size(315, 20);
      this.maxTimeOffsetTextBox.TabIndex = 8;
      this.maxTimeOffsetTextBox.TextChanged += new System.EventHandler(this.maxTimeOffsetTextBox_TextChanged);
      // 
      // LaggedSymbolView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.maxTimeOffsetTextBox);
      this.Controls.Add(this.minTimeOffsetTextBox);
      this.Controls.Add(this.maxTimeOffsetLabel);
      this.Controls.Add(this.minTimeOffsetLabel);
      this.Name = "LaggedSymbolView";
      this.Size = new System.Drawing.Size(408, 179);
      this.Controls.SetChildIndex(this.maximumArityLabel, 0);
      this.Controls.SetChildIndex(this.maximumArityTextBox, 0);
      this.Controls.SetChildIndex(this.minimumArityLabel, 0);
      this.Controls.SetChildIndex(this.minimumArityTextBox, 0);
      this.Controls.SetChildIndex(this.infoLabel, 0);
      this.Controls.SetChildIndex(this.initialFrequencyTextBox, 0);
      this.Controls.SetChildIndex(this.initialFrequencyLabel, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      this.Controls.SetChildIndex(this.minTimeOffsetLabel, 0);
      this.Controls.SetChildIndex(this.maxTimeOffsetLabel, 0);
      this.Controls.SetChildIndex(this.minTimeOffsetTextBox, 0);
      this.Controls.SetChildIndex(this.maxTimeOffsetTextBox, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label minTimeOffsetLabel;
    private System.Windows.Forms.Label maxTimeOffsetLabel;
    private System.Windows.Forms.TextBox minTimeOffsetTextBox;
    private System.Windows.Forms.TextBox maxTimeOffsetTextBox;

  }
}
