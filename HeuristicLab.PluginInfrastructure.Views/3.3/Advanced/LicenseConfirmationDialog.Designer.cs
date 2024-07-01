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
namespace HeuristicLab.PluginInfrastructure.Advanced {
  partial class LicenseConfirmationDialog {
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
      this.richTextBox = new System.Windows.Forms.RichTextBox();
      this.acceptButton = new System.Windows.Forms.Button();
      this.rejectButton = new System.Windows.Forms.Button();
      this.licenseLabel = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.radioButtonGroupBox = new System.Windows.Forms.GroupBox();
      this.rejectRadioButton = new System.Windows.Forms.RadioButton();
      this.acceptRadioButton = new System.Windows.Forms.RadioButton();
      this.panel1 = new System.Windows.Forms.Panel();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.radioButtonGroupBox.SuspendLayout();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // richTextBox
      // 
      this.richTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.richTextBox.BackColor = System.Drawing.SystemColors.HighlightText;
      this.richTextBox.Location = new System.Drawing.Point(12, 48);
      this.richTextBox.Name = "richTextBox";
      this.richTextBox.ReadOnly = true;
      this.richTextBox.Size = new System.Drawing.Size(494, 287);
      this.richTextBox.TabIndex = 0;
      this.richTextBox.Text = "";
      // 
      // acceptButton
      // 
      this.acceptButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.acceptButton.Enabled = false;
      this.acceptButton.Location = new System.Drawing.Point(347, 53);
      this.acceptButton.Name = "acceptButton";
      this.acceptButton.Size = new System.Drawing.Size(75, 23);
      this.acceptButton.TabIndex = 1;
      this.acceptButton.Text = "Next >";
      this.toolTip.SetToolTip(this.acceptButton, "Accept license agreement and continue installation");
      this.acceptButton.UseVisualStyleBackColor = true;
      this.acceptButton.Click += new System.EventHandler(this.acceptButton_Click);
      // 
      // rejectButton
      // 
      this.rejectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.rejectButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.rejectButton.Location = new System.Drawing.Point(428, 53);
      this.rejectButton.Name = "rejectButton";
      this.rejectButton.Size = new System.Drawing.Size(79, 23);
      this.rejectButton.TabIndex = 2;
      this.rejectButton.Text = "Cancel";
      this.toolTip.SetToolTip(this.rejectButton, "Cancel installation");
      this.rejectButton.UseVisualStyleBackColor = true;
      this.rejectButton.Click += new System.EventHandler(this.rejectButton_Click);
      // 
      // licenseLabel
      // 
      this.licenseLabel.AutoSize = true;
      this.licenseLabel.Location = new System.Drawing.Point(12, 32);
      this.licenseLabel.Name = "licenseLabel";
      this.licenseLabel.Size = new System.Drawing.Size(241, 13);
      this.licenseLabel.TabIndex = 3;
      this.licenseLabel.Text = "Please read following license agreement carefully.";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(12, 9);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(115, 13);
      this.label1.TabIndex = 4;
      this.label1.Text = "License Agreement";
      // 
      // radioButtonGroupBox
      // 
      this.radioButtonGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.radioButtonGroupBox.Controls.Add(this.rejectRadioButton);
      this.radioButtonGroupBox.Controls.Add(this.acceptRadioButton);
      this.radioButtonGroupBox.Location = new System.Drawing.Point(13, 5);
      this.radioButtonGroupBox.Name = "radioButtonGroupBox";
      this.radioButtonGroupBox.Size = new System.Drawing.Size(281, 71);
      this.radioButtonGroupBox.TabIndex = 5;
      this.radioButtonGroupBox.TabStop = false;
      // 
      // rejectRadioButton
      // 
      this.rejectRadioButton.AutoSize = true;
      this.rejectRadioButton.Location = new System.Drawing.Point(6, 42);
      this.rejectRadioButton.Name = "rejectRadioButton";
      this.rejectRadioButton.Size = new System.Drawing.Size(264, 17);
      this.rejectRadioButton.TabIndex = 1;
      this.rejectRadioButton.TabStop = true;
      this.rejectRadioButton.Text = "I do not accept the terms in the license agreement.";
      this.rejectRadioButton.UseVisualStyleBackColor = true;
      this.rejectRadioButton.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
      // 
      // acceptRadioButton
      // 
      this.acceptRadioButton.AutoSize = true;
      this.acceptRadioButton.Location = new System.Drawing.Point(6, 19);
      this.acceptRadioButton.Name = "acceptRadioButton";
      this.acceptRadioButton.Size = new System.Drawing.Size(231, 17);
      this.acceptRadioButton.TabIndex = 0;
      this.acceptRadioButton.TabStop = true;
      this.acceptRadioButton.Text = "I accept the terms in the license agreement.";
      this.acceptRadioButton.UseVisualStyleBackColor = true;
      this.acceptRadioButton.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
      // 
      // panel1
      // 
      this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.panel1.BackColor = System.Drawing.SystemColors.Control;
      this.panel1.Controls.Add(this.acceptButton);
      this.panel1.Controls.Add(this.radioButtonGroupBox);
      this.panel1.Controls.Add(this.rejectButton);
      this.panel1.Location = new System.Drawing.Point(-1, 341);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(520, 88);
      this.panel1.TabIndex = 6;
      // 
      // LicenseConfirmationDialog
      // 
      this.AcceptButton = this.acceptButton;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.BackColor = System.Drawing.SystemColors.HighlightText;
      this.CancelButton = this.rejectButton;
      this.ClientSize = new System.Drawing.Size(518, 429);
      this.Controls.Add(this.panel1);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.licenseLabel);
      this.Controls.Add(this.richTextBox);
      this.Icon = HeuristicLab.PluginInfrastructure.Resources.HeuristicLab;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "LicenseConfirmationDialog";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.radioButtonGroupBox.ResumeLayout(false);
      this.radioButtonGroupBox.PerformLayout();
      this.panel1.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.RichTextBox richTextBox;
    private System.Windows.Forms.Button acceptButton;
    private System.Windows.Forms.Button rejectButton;
    private System.Windows.Forms.Label licenseLabel;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.GroupBox radioButtonGroupBox;
    private System.Windows.Forms.RadioButton rejectRadioButton;
    private System.Windows.Forms.RadioButton acceptRadioButton;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.ToolTip toolTip;
  }
}