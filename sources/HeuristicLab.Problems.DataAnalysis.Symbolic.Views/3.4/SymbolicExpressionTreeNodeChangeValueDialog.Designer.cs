#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  partial class ValueChangeDialog {
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

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      this.originalValueLabel = new System.Windows.Forms.Label();
      this.originalValueTextBox = new System.Windows.Forms.TextBox();
      this.newValueTextBox = new System.Windows.Forms.TextBox();
      this.newValueLabel = new System.Windows.Forms.Label();
      this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // originalValueLabel
      // 
      this.originalValueLabel.AutoSize = true;
      this.originalValueLabel.Location = new System.Drawing.Point(14, 15);
      this.originalValueLabel.Name = "originalValueLabel";
      this.originalValueLabel.Size = new System.Drawing.Size(72, 13);
      this.originalValueLabel.TabIndex = 2;
      this.originalValueLabel.Text = "Original Value";
      // 
      // originalValueTextBox
      // 
      this.originalValueTextBox.Location = new System.Drawing.Point(91, 12);
      this.originalValueTextBox.Name = "originalValueTextBox";
      this.originalValueTextBox.ReadOnly = true;
      this.originalValueTextBox.Size = new System.Drawing.Size(100, 20);
      this.originalValueTextBox.TabIndex = 3;
      // 
      // newValueTextBox
      // 
      this.newValueTextBox.Location = new System.Drawing.Point(91, 38);
      this.newValueTextBox.Name = "newValueTextBox";
      this.newValueTextBox.Size = new System.Drawing.Size(100, 20);
      this.newValueTextBox.TabIndex = 0;
      this.newValueTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.newValueTextBox_KeyDown);
      this.newValueTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.newValueTextBox_Validating);
      this.newValueTextBox.Validated += new System.EventHandler(this.newValueTextBox_Validated);
      // 
      // newValueLabel
      // 
      this.newValueLabel.AutoSize = true;
      this.newValueLabel.Location = new System.Drawing.Point(14, 41);
      this.newValueLabel.Name = "newValueLabel";
      this.newValueLabel.Size = new System.Drawing.Size(59, 13);
      this.newValueLabel.TabIndex = 5;
      this.newValueLabel.Text = "New Value";
      // 
      // errorProvider
      // 
      this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
      this.errorProvider.ContainerControl = this;
      this.errorProvider.RightToLeft = true;
      // 
      // ValueChangeDialog
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoSize = true;
      this.ClientSize = new System.Drawing.Size(209, 71);
      this.Controls.Add(this.newValueLabel);
      this.Controls.Add(this.newValueTextBox);
      this.Controls.Add(this.originalValueTextBox);
      this.Controls.Add(this.originalValueLabel);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ValueChangeDialog";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Change Value or Weight";
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label originalValueLabel;
    private System.Windows.Forms.TextBox originalValueTextBox;
    private System.Windows.Forms.TextBox newValueTextBox;
    private System.Windows.Forms.Label newValueLabel;
    private System.Windows.Forms.ErrorProvider errorProvider;
  }
}
