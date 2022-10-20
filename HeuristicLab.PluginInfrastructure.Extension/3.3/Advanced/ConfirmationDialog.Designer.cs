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
  partial class ConfirmationDialog {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfirmationDialog));
      this.informationTextBox = new System.Windows.Forms.RichTextBox();
      this.messageLabel = new System.Windows.Forms.Label();
      this.okButton = new System.Windows.Forms.Button();
      this.cancelButton = new System.Windows.Forms.Button();
      this.icon = new System.Windows.Forms.PictureBox();
      this.panel1 = new System.Windows.Forms.Panel();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.icon)).BeginInit();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // informationTextBox
      // 
      this.informationTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.informationTextBox.BackColor = System.Drawing.SystemColors.HighlightText;
      this.informationTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.informationTextBox.Location = new System.Drawing.Point(105, 46);
      this.informationTextBox.Name = "informationTextBox";
      this.informationTextBox.ReadOnly = true;
      this.informationTextBox.Size = new System.Drawing.Size(301, 181);
      this.informationTextBox.TabIndex = 0;
      this.informationTextBox.Text = "";
      // 
      // messageLabel
      // 
      this.messageLabel.AutoSize = true;
      this.messageLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.messageLabel.Location = new System.Drawing.Point(13, 13);
      this.messageLabel.Name = "messageLabel";
      this.messageLabel.Size = new System.Drawing.Size(0, 13);
      this.messageLabel.TabIndex = 1;
      // 
      // okButton
      // 
      this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.okButton.Location = new System.Drawing.Point(250, 14);
      this.okButton.Name = "okButton";
      this.okButton.Size = new System.Drawing.Size(75, 23);
      this.okButton.TabIndex = 2;
      this.okButton.Text = "&OK";
      this.toolTip.SetToolTip(this.okButton, "Confirm Action");
      this.okButton.UseVisualStyleBackColor = true;
      this.okButton.Click += new System.EventHandler(this.okButton_Click);
      // 
      // cancelButton
      // 
      this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.cancelButton.Location = new System.Drawing.Point(331, 14);
      this.cancelButton.Name = "cancelButton";
      this.cancelButton.Size = new System.Drawing.Size(75, 23);
      this.cancelButton.TabIndex = 3;
      this.cancelButton.Text = "&Cancel";
      this.toolTip.SetToolTip(this.cancelButton, "Cancel Action");
      this.cancelButton.UseVisualStyleBackColor = true;
      this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
      // 
      // icon
      // 
      this.icon.Location = new System.Drawing.Point(31, 66);
      this.icon.Name = "icon";
      this.icon.Size = new System.Drawing.Size(47, 50);
      this.icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
      this.icon.TabIndex = 4;
      this.icon.TabStop = false;
      // 
      // panel1
      // 
      this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.panel1.BackColor = System.Drawing.SystemColors.Control;
      this.panel1.Controls.Add(this.okButton);
      this.panel1.Controls.Add(this.cancelButton);
      this.panel1.Location = new System.Drawing.Point(0, 233);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(418, 50);
      this.panel1.TabIndex = 6;
      // 
      // ConfirmationDialog
      // 
      this.AcceptButton = this.okButton;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.BackColor = System.Drawing.SystemColors.HighlightText;
      this.CancelButton = this.cancelButton;
      this.ClientSize = new System.Drawing.Size(418, 282);
      this.Controls.Add(this.panel1);
      this.Controls.Add(this.icon);
      this.Controls.Add(this.messageLabel);
      this.Controls.Add(this.informationTextBox);
      this.Icon = HeuristicLab.PluginInfrastructure.Resources.HeuristicLab;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ConfirmationDialog";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "ConfirmationDialog";
      ((System.ComponentModel.ISupportInitialize)(this.icon)).EndInit();
      this.panel1.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.RichTextBox informationTextBox;
    private System.Windows.Forms.Label messageLabel;
    private System.Windows.Forms.Button okButton;
    private System.Windows.Forms.Button cancelButton;
    private System.Windows.Forms.PictureBox icon;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.ToolTip toolTip;
  }
}