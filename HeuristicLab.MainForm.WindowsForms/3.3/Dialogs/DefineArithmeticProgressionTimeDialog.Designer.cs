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

namespace HeuristicLab.MainForm.WindowsForms {
  partial class DefineArithmeticTimeSpanProgressionDialog {
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
      this.components = new System.ComponentModel.Container();
      this.stepSizeTextBox = new System.Windows.Forms.TextBox();
      this.maximumTextBox = new System.Windows.Forms.TextBox();
      this.minimumTextBox = new System.Windows.Forms.TextBox();
      this.stepSizeLabel = new System.Windows.Forms.Label();
      this.maximumLabel = new System.Windows.Forms.Label();
      this.minimumLabel = new System.Windows.Forms.Label();
      this.cancelButton = new System.Windows.Forms.Button();
      this.okButton = new System.Windows.Forms.Button();
      this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // stepSizeTextBox
      // 
      this.stepSizeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.stepSizeTextBox.Location = new System.Drawing.Point(75, 64);
      this.stepSizeTextBox.Name = "stepSizeTextBox";
      this.stepSizeTextBox.Size = new System.Drawing.Size(184, 20);
      this.stepSizeTextBox.TabIndex = 11;
      this.stepSizeTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_Validating);
      // 
      // maximumTextBox
      // 
      this.maximumTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.maximumTextBox.Location = new System.Drawing.Point(75, 38);
      this.maximumTextBox.Name = "maximumTextBox";
      this.maximumTextBox.Size = new System.Drawing.Size(184, 20);
      this.maximumTextBox.TabIndex = 9;
      this.maximumTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_Validating);
      // 
      // minimumTextBox
      // 
      this.minimumTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.minimumTextBox.Location = new System.Drawing.Point(75, 12);
      this.minimumTextBox.Name = "minimumTextBox";
      this.minimumTextBox.Size = new System.Drawing.Size(184, 20);
      this.minimumTextBox.TabIndex = 7;
      this.minimumTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_Validating);
      // 
      // stepSizeLabel
      // 
      this.stepSizeLabel.AutoSize = true;
      this.stepSizeLabel.Location = new System.Drawing.Point(15, 67);
      this.stepSizeLabel.Name = "stepSizeLabel";
      this.stepSizeLabel.Size = new System.Drawing.Size(53, 13);
      this.stepSizeLabel.TabIndex = 10;
      this.stepSizeLabel.Text = "Step size:";
      // 
      // maximumLabel
      // 
      this.maximumLabel.AutoSize = true;
      this.maximumLabel.Location = new System.Drawing.Point(15, 41);
      this.maximumLabel.Name = "maximumLabel";
      this.maximumLabel.Size = new System.Drawing.Size(54, 13);
      this.maximumLabel.TabIndex = 8;
      this.maximumLabel.Text = "Maximum:";
      // 
      // minimumLabel
      // 
      this.minimumLabel.AutoSize = true;
      this.minimumLabel.Location = new System.Drawing.Point(15, 15);
      this.minimumLabel.Name = "minimumLabel";
      this.minimumLabel.Size = new System.Drawing.Size(51, 13);
      this.minimumLabel.TabIndex = 6;
      this.minimumLabel.Text = "Minimum:";
      // 
      // cancelButton
      // 
      this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.cancelButton.Location = new System.Drawing.Point(184, 96);
      this.cancelButton.Name = "cancelButton";
      this.cancelButton.Size = new System.Drawing.Size(75, 23);
      this.cancelButton.TabIndex = 12;
      this.cancelButton.Text = "&Cancel";
      this.cancelButton.UseVisualStyleBackColor = true;
      // 
      // okButton
      // 
      this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.okButton.Location = new System.Drawing.Point(103, 96);
      this.okButton.Name = "okButton";
      this.okButton.Size = new System.Drawing.Size(75, 23);
      this.okButton.TabIndex = 12;
      this.okButton.Text = "&OK";
      this.okButton.UseVisualStyleBackColor = true;
      // 
      // errorProvider
      // 
      this.errorProvider.ContainerControl = this;
      // 
      // DefineArithmeticProgressionDialog
      // 
      this.AcceptButton = this.okButton;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.cancelButton;
      this.ClientSize = new System.Drawing.Size(271, 131);
      this.Controls.Add(this.okButton);
      this.Controls.Add(this.cancelButton);
      this.Controls.Add(this.stepSizeTextBox);
      this.Controls.Add(this.maximumTextBox);
      this.Controls.Add(this.minimumTextBox);
      this.Controls.Add(this.stepSizeLabel);
      this.Controls.Add(this.maximumLabel);
      this.Controls.Add(this.minimumLabel);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "DefineArithmeticProgressionDialog";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Generate Values";
      this.TopMost = true;
      this.Load += new System.EventHandler(this.DefineArithmeticTimeSpanProgressionDialog_Load);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox stepSizeTextBox;
    private System.Windows.Forms.TextBox maximumTextBox;
    private System.Windows.Forms.TextBox minimumTextBox;
    private System.Windows.Forms.Label stepSizeLabel;
    private System.Windows.Forms.Label maximumLabel;
    private System.Windows.Forms.Label minimumLabel;
    private System.Windows.Forms.Button cancelButton;
    private System.Windows.Forms.Button okButton;
    private System.Windows.Forms.ErrorProvider errorProvider;
  }
}