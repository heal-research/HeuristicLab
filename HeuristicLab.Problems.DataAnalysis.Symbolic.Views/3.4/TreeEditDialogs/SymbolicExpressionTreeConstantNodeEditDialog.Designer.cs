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

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Views {
  partial class ConstantNodeEditDialog {
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
      this.oldValueTextBox = new System.Windows.Forms.TextBox();
      this.newValueTextBox = new System.Windows.Forms.TextBox();
      this.newValueLabel = new System.Windows.Forms.Label();
      this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
      this.okButton = new System.Windows.Forms.Button();
      this.cancelButton = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // originalValueLabel
      // 
      this.originalValueLabel.AutoSize = true;
      this.originalValueLabel.Location = new System.Drawing.Point(12, 9);
      this.originalValueLabel.Name = "originalValueLabel";
      this.originalValueLabel.Size = new System.Drawing.Size(72, 13);
      this.originalValueLabel.TabIndex = 2;
      this.originalValueLabel.Text = "Original Value";
      // 
      // oldValueTextBox
      // 
      this.oldValueTextBox.Location = new System.Drawing.Point(106, 6);
      this.oldValueTextBox.Name = "oldValueTextBox";
      this.oldValueTextBox.ReadOnly = true;
      this.oldValueTextBox.Size = new System.Drawing.Size(131, 20);
      this.oldValueTextBox.TabIndex = 3;
      // 
      // newValueTextBox
      // 
      this.newValueTextBox.Location = new System.Drawing.Point(106, 32);
      this.newValueTextBox.Name = "newValueTextBox";
      this.newValueTextBox.Size = new System.Drawing.Size(131, 20);
      this.newValueTextBox.TabIndex = 0;
      this.newValueTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.childControl_KeyDown);
      this.newValueTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.newValueTextBox_Validating);
      this.newValueTextBox.Validated += new System.EventHandler(this.newValueTextBox_Validated);
      // 
      // newValueLabel
      // 
      this.newValueLabel.AutoSize = true;
      this.newValueLabel.Location = new System.Drawing.Point(12, 35);
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
      // okButton
      // 
      this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.okButton.Location = new System.Drawing.Point(81, 66);
      this.okButton.Name = "okButton";
      this.okButton.Size = new System.Drawing.Size(75, 23);
      this.okButton.TabIndex = 8;
      this.okButton.Text = "OK";
      this.okButton.UseVisualStyleBackColor = true;
      this.okButton.Click += new System.EventHandler(this.okButton_Click);
      // 
      // cancelButton
      // 
      this.cancelButton.CausesValidation = false;
      this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.cancelButton.Location = new System.Drawing.Point(162, 66);
      this.cancelButton.Name = "cancelButton";
      this.cancelButton.Size = new System.Drawing.Size(75, 23);
      this.cancelButton.TabIndex = 9;
      this.cancelButton.Text = "Cancel";
      this.cancelButton.UseVisualStyleBackColor = true;
      this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
      // 
      // ConstantNodeEditDialog
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoSize = true;
      this.CancelButton = this.cancelButton;
      this.ClientSize = new System.Drawing.Size(248, 101);
      this.ControlBox = false;
      this.Controls.Add(this.cancelButton);
      this.Controls.Add(this.okButton);
      this.Controls.Add(this.newValueLabel);
      this.Controls.Add(this.newValueTextBox);
      this.Controls.Add(this.oldValueTextBox);
      this.Controls.Add(this.originalValueLabel);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ConstantNodeEditDialog";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Edit constant value";
      this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ConstantNodeEditDialog_KeyDown);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label originalValueLabel;
    private System.Windows.Forms.TextBox oldValueTextBox;
    private System.Windows.Forms.Label newValueLabel;
    private System.Windows.Forms.ErrorProvider errorProvider;
    private System.Windows.Forms.Button cancelButton;
    private System.Windows.Forms.Button okButton;
    public System.Windows.Forms.TextBox newValueTextBox;
  }
}
