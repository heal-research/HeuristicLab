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

namespace HeuristicLab.Clients.Access.Views {
  partial class ClientRegistrationDialog {
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
      this.btnCollectInformation = new System.Windows.Forms.Button();
      this.richTextBox1 = new System.Windows.Forms.RichTextBox();
      this.progressBar = new System.Windows.Forms.ProgressBar();
      this.btnRegisterClient = new System.Windows.Forms.Button();
      this.btnCancel = new System.Windows.Forms.Button();
      this.clientView = new HeuristicLab.Clients.Access.Views.ClientView();
      this.SuspendLayout();
      // 
      // btnCollectInformation
      // 
      this.btnCollectInformation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btnCollectInformation.Location = new System.Drawing.Point(12, 220);
      this.btnCollectInformation.Name = "btnCollectInformation";
      this.btnCollectInformation.Size = new System.Drawing.Size(111, 23);
      this.btnCollectInformation.TabIndex = 1;
      this.btnCollectInformation.Text = "Collect Information";
      this.btnCollectInformation.UseVisualStyleBackColor = true;
      this.btnCollectInformation.Click += new System.EventHandler(this.btnCollectInformation_Click);
      // 
      // richTextBox1
      // 
      this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.richTextBox1.BackColor = System.Drawing.SystemColors.Control;
      this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.richTextBox1.Location = new System.Drawing.Point(12, 12);
      this.richTextBox1.Name = "richTextBox1";
      this.richTextBox1.Size = new System.Drawing.Size(769, 202);
      this.richTextBox1.TabIndex = 2;
      this.richTextBox1.Text = "";
      this.richTextBox1.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.richTextBox1_LinkClicked);
      // 
      // progressBar
      // 
      this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.progressBar.Location = new System.Drawing.Point(129, 220);
      this.progressBar.Name = "progressBar";
      this.progressBar.Size = new System.Drawing.Size(651, 23);
      this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
      this.progressBar.TabIndex = 4;
      this.progressBar.Visible = false;
      // 
      // btnRegisterClient
      // 
      this.btnRegisterClient.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btnRegisterClient.Enabled = false;
      this.btnRegisterClient.Location = new System.Drawing.Point(12, 595);
      this.btnRegisterClient.Name = "btnRegisterClient";
      this.btnRegisterClient.Size = new System.Drawing.Size(88, 23);
      this.btnRegisterClient.TabIndex = 5;
      this.btnRegisterClient.Text = "Register client";
      this.btnRegisterClient.UseVisualStyleBackColor = true;
      this.btnRegisterClient.Click += new System.EventHandler(this.btnRegisterClient_Click);
      // 
      // btnCancel
      // 
      this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnCancel.Location = new System.Drawing.Point(705, 595);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(75, 23);
      this.btnCancel.TabIndex = 6;
      this.btnCancel.Text = "Close";
      this.btnCancel.UseVisualStyleBackColor = true;
      // 
      // clientView
      // 
      this.clientView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.clientView.Caption = "Client View";
      this.clientView.Content = null;
      this.clientView.Location = new System.Drawing.Point(11, 249);
      this.clientView.Name = "clientView";
      this.clientView.ReadOnly = false;
      this.clientView.Size = new System.Drawing.Size(769, 340);
      this.clientView.TabIndex = 3;
      // 
      // ClientRegistrationDialog
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.btnCancel;
      this.ClientSize = new System.Drawing.Size(793, 631);
      this.Controls.Add(this.btnCancel);
      this.Controls.Add(this.btnRegisterClient);
      this.Controls.Add(this.progressBar);
      this.Controls.Add(this.clientView);
      this.Controls.Add(this.richTextBox1);
      this.Controls.Add(this.btnCollectInformation);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ClientRegistrationDialog";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Client Registration";
      this.TopMost = true;
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button btnCollectInformation;
    private System.Windows.Forms.RichTextBox richTextBox1;
    private ClientView clientView;
    private System.Windows.Forms.ProgressBar progressBar;
    private System.Windows.Forms.Button btnRegisterClient;
    private System.Windows.Forms.Button btnCancel;

  }
}