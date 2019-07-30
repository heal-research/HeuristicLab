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

namespace HeuristicLab.Optimizer {
  partial class MainFormTypeSelectionDialog {
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
      this.cancelButton = new System.Windows.Forms.Button();
      this.okButton = new System.Windows.Forms.Button();
      this.groupBox = new System.Windows.Forms.GroupBox();
      this.rbSingleDocumentMainForm = new System.Windows.Forms.RadioButton();
      this.rbMultipleDocumentMainForm = new System.Windows.Forms.RadioButton();
      this.rbDockingMainForm = new System.Windows.Forms.RadioButton();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.labelRestart = new System.Windows.Forms.Label();
      this.groupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // cancelButton
      // 
      this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.cancelButton.Location = new System.Drawing.Point(173, 167);
      this.cancelButton.Name = "cancelButton";
      this.cancelButton.Size = new System.Drawing.Size(75, 23);
      this.cancelButton.TabIndex = 9;
      this.cancelButton.Text = "&Cancel";
      this.cancelButton.UseVisualStyleBackColor = true;
      // 
      // okButton
      // 
      this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.okButton.Location = new System.Drawing.Point(92, 167);
      this.okButton.Name = "okButton";
      this.okButton.Size = new System.Drawing.Size(75, 23);
      this.okButton.TabIndex = 8;
      this.okButton.Text = "&OK";
      this.okButton.UseVisualStyleBackColor = true;
      this.okButton.Click += new System.EventHandler(this.okButton_Click);
      // 
      // groupBox
      // 
      this.groupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.groupBox.Controls.Add(this.rbSingleDocumentMainForm);
      this.groupBox.Controls.Add(this.rbMultipleDocumentMainForm);
      this.groupBox.Controls.Add(this.rbDockingMainForm);
      this.groupBox.Location = new System.Drawing.Point(12, 12);
      this.groupBox.Name = "groupBox";
      this.groupBox.Size = new System.Drawing.Size(234, 93);
      this.groupBox.TabIndex = 10;
      this.groupBox.TabStop = false;
      this.groupBox.Text = "MainForm Types";
      // 
      // rbSingleDocumentMainForm
      // 
      this.rbSingleDocumentMainForm.AutoSize = true;
      this.rbSingleDocumentMainForm.Location = new System.Drawing.Point(6, 65);
      this.rbSingleDocumentMainForm.Name = "rbSingleDocumentMainForm";
      this.rbSingleDocumentMainForm.Size = new System.Drawing.Size(106, 17);
      this.rbSingleDocumentMainForm.TabIndex = 2;
      this.rbSingleDocumentMainForm.TabStop = true;
      this.rbSingleDocumentMainForm.Text = "&Single Document";
      this.toolTip.SetToolTip(this.rbSingleDocumentMainForm, "Creates for each open document an own window");
      this.rbSingleDocumentMainForm.UseVisualStyleBackColor = true;
      // 
      // rbMultipleDocumentMainForm
      // 
      this.rbMultipleDocumentMainForm.AutoSize = true;
      this.rbMultipleDocumentMainForm.Location = new System.Drawing.Point(6, 42);
      this.rbMultipleDocumentMainForm.Name = "rbMultipleDocumentMainForm";
      this.rbMultipleDocumentMainForm.Size = new System.Drawing.Size(113, 17);
      this.rbMultipleDocumentMainForm.TabIndex = 1;
      this.rbMultipleDocumentMainForm.TabStop = true;
      this.rbMultipleDocumentMainForm.Text = "&Multiple Document";
      this.toolTip.SetToolTip(this.rbMultipleDocumentMainForm, "Shows for each open document an own, nested window");
      this.rbMultipleDocumentMainForm.UseVisualStyleBackColor = true;
      // 
      // rbDockingMainForm
      // 
      this.rbDockingMainForm.AutoSize = true;
      this.rbDockingMainForm.Location = new System.Drawing.Point(6, 19);
      this.rbDockingMainForm.Name = "rbDockingMainForm";
      this.rbDockingMainForm.Size = new System.Drawing.Size(65, 17);
      this.rbDockingMainForm.TabIndex = 0;
      this.rbDockingMainForm.TabStop = true;
      this.rbDockingMainForm.Text = "&Docking";
      this.toolTip.SetToolTip(this.rbDockingMainForm, "Displays open documents as tab pages. \r\nIf you have resizing issues in the user i" +
              "nterface, please try the other options.");
      this.rbDockingMainForm.UseVisualStyleBackColor = true;
      // 
      // labelRestart
      // 
      this.labelRestart.AutoSize = true;
      this.labelRestart.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.labelRestart.ForeColor = System.Drawing.Color.Red;
      this.labelRestart.Location = new System.Drawing.Point(9, 123);
      this.labelRestart.Name = "labelRestart";
      this.labelRestart.Size = new System.Drawing.Size(241, 26);
      this.labelRestart.TabIndex = 11;
      this.labelRestart.Text = "Please note that you have to restart the \r\nOptimizer for the changes to take effe" +
          "ct. ";
      // 
      // MainFormTypeSelectionDialog
      // 
      this.AcceptButton = this.okButton;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.CancelButton = this.cancelButton;
      this.ClientSize = new System.Drawing.Size(260, 202);
      this.Controls.Add(this.labelRestart);
      this.Controls.Add(this.groupBox);
      this.Controls.Add(this.cancelButton);
      this.Controls.Add(this.okButton);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "MainFormTypeSelectionDialog";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Change MainForm Type";
      this.Load += new System.EventHandler(this.MainFormTypeSelectionDialog_Load);
      this.groupBox.ResumeLayout(false);
      this.groupBox.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button cancelButton;
    private System.Windows.Forms.Button okButton;
    private System.Windows.Forms.GroupBox groupBox;
    private System.Windows.Forms.RadioButton rbMultipleDocumentMainForm;
    private System.Windows.Forms.RadioButton rbDockingMainForm;
    private System.Windows.Forms.ToolTip toolTip;
    private System.Windows.Forms.RadioButton rbSingleDocumentMainForm;
    private System.Windows.Forms.Label labelRestart;
  }
}