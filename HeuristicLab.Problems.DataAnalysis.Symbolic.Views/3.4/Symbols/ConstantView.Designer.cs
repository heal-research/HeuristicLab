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
  partial class ConstantView {
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
      this.valueLabel = new System.Windows.Forms.Label();
      this.valueTextBox = new System.Windows.Forms.TextBox();
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
      this.initialFrequencyTextBox.Size = new System.Drawing.Size(280, 20);
      // 
      // minimumArityLabel
      // 
      this.toolTip.SetToolTip(this.minimumArityLabel, "The minimum arity of the symbol");
      // 
      // maximumArityLabel
      // 
      this.toolTip.SetToolTip(this.maximumArityLabel, "The maximum arity of the symbol");
      // 
      // minimumArityTextBox
      // 
      this.errorProvider.SetIconAlignment(this.minimumArityTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.minimumArityTextBox.Size = new System.Drawing.Size(280, 20);
      // 
      // maximumArityTextBox
      // 
      this.errorProvider.SetIconAlignment(this.maximumArityTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.maximumArityTextBox.Size = new System.Drawing.Size(280, 20);
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Size = new System.Drawing.Size(280, 20);
      // 
      // infoLabel
      // 
      this.infoLabel.Location = new System.Drawing.Point(379, 3);
      // 
      // valueLabel
      // 
      this.valueLabel.AutoSize = true;
      this.valueLabel.Location = new System.Drawing.Point(3, 130);
      this.valueLabel.Name = "valueLabel";
      this.valueLabel.Size = new System.Drawing.Size(37, 13);
      this.valueLabel.TabIndex = 0;
      this.valueLabel.Text = "Value:";
      this.toolTip.SetToolTip(this.valueLabel, "The value of the constant.");
      // 
      // valueTextBox
      // 
      this.valueTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.errorProvider.SetIconAlignment(this.valueTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.valueTextBox.Location = new System.Drawing.Point(93, 127);
      this.valueTextBox.Name = "valueTextBox";
      this.valueTextBox.Size = new System.Drawing.Size(280, 20);
      this.valueTextBox.TabIndex = 1;
      this.toolTip.SetToolTip(this.valueTextBox, "The value of the constant.");
      this.valueTextBox.TextChanged += new System.EventHandler(this.valueTextBox_TextChanged);
      // 
      // ConstantView
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.valueLabel);
      this.Controls.Add(this.valueTextBox);
      this.Name = "ConstantView";
      this.Size = new System.Drawing.Size(398, 168);
      this.Controls.SetChildIndex(this.valueTextBox, 0);
      this.Controls.SetChildIndex(this.valueLabel, 0);
      this.Controls.SetChildIndex(this.enabledCheckBox, 0);
      this.Controls.SetChildIndex(this.initialFrequencyLabel, 0);
      this.Controls.SetChildIndex(this.initialFrequencyTextBox, 0);
      this.Controls.SetChildIndex(this.maximumArityLabel, 0);
      this.Controls.SetChildIndex(this.maximumArityTextBox, 0);
      this.Controls.SetChildIndex(this.minimumArityLabel, 0);
      this.Controls.SetChildIndex(this.minimumArityTextBox, 0);
      this.Controls.SetChildIndex(this.infoLabel, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label valueLabel;
    private System.Windows.Forms.TextBox valueTextBox;
  }
}
