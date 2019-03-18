#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Optimizer {
  partial class ChangeNestingLevelDialog {
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

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.okButton = new System.Windows.Forms.Button();
      this.cancelButton = new System.Windows.Forms.Button();
      this.nestingLevelNumericUpDownn = new System.Windows.Forms.NumericUpDown();
      this.nestingLevelLabel = new System.Windows.Forms.Label();
      this.infoLabel = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.nestingLevelNumericUpDownn)).BeginInit();
      this.SuspendLayout();
      // 
      // okButton
      // 
      this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.okButton.Location = new System.Drawing.Point(124, 92);
      this.okButton.Name = "okButton";
      this.okButton.Size = new System.Drawing.Size(75, 23);
      this.okButton.TabIndex = 0;
      this.okButton.Text = "&OK";
      this.okButton.UseVisualStyleBackColor = true;
      this.okButton.Click += new System.EventHandler(this.okButton_Click);
      // 
      // cancelButton
      // 
      this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.cancelButton.Location = new System.Drawing.Point(205, 92);
      this.cancelButton.Name = "cancelButton";
      this.cancelButton.Size = new System.Drawing.Size(75, 23);
      this.cancelButton.TabIndex = 1;
      this.cancelButton.Text = "&Cancel";
      this.cancelButton.UseVisualStyleBackColor = true;
      // 
      // nestingLevelNumericUpDownn
      // 
      this.nestingLevelNumericUpDownn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.nestingLevelNumericUpDownn.Location = new System.Drawing.Point(147, 12);
      this.nestingLevelNumericUpDownn.Name = "nestingLevelNumericUpDownn";
      this.nestingLevelNumericUpDownn.Size = new System.Drawing.Size(133, 20);
      this.nestingLevelNumericUpDownn.TabIndex = 2;
      // 
      // nestingLevelLabel
      // 
      this.nestingLevelLabel.AutoSize = true;
      this.nestingLevelLabel.Location = new System.Drawing.Point(12, 14);
      this.nestingLevelLabel.Name = "nestingLevelLabel";
      this.nestingLevelLabel.Size = new System.Drawing.Size(116, 13);
      this.nestingLevelLabel.TabIndex = 3;
      this.nestingLevelLabel.Text = "Maximum nesting level:";
      // 
      // infoLabel
      // 
      this.infoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.infoLabel.Location = new System.Drawing.Point(12, 46);
      this.infoLabel.Name = "infoLabel";
      this.infoLabel.Size = new System.Drawing.Size(268, 43);
      this.infoLabel.TabIndex = 4;
      this.infoLabel.Text = "A high nesting level of controls can cause problems. A maximum level of 35 is rec" +
    "ommended.";
      // 
      // ChangeNestingLevelDialog
      // 
      this.AcceptButton = this.okButton;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.cancelButton;
      this.ClientSize = new System.Drawing.Size(292, 127);
      this.Controls.Add(this.infoLabel);
      this.Controls.Add(this.nestingLevelLabel);
      this.Controls.Add(this.nestingLevelNumericUpDownn);
      this.Controls.Add(this.cancelButton);
      this.Controls.Add(this.okButton);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ChangeNestingLevelDialog";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Change Nesting Level";
      this.TopMost = true;
      this.Load += new System.EventHandler(this.ChangeNestingLevelDialog_Load);
      ((System.ComponentModel.ISupportInitialize)(this.nestingLevelNumericUpDownn)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button okButton;
    private System.Windows.Forms.Button cancelButton;
    private System.Windows.Forms.NumericUpDown nestingLevelNumericUpDownn;
    private System.Windows.Forms.Label nestingLevelLabel;
    private System.Windows.Forms.Label infoLabel;
  }
}