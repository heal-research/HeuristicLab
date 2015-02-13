#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  partial class ClientInformationDialog {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClientInformationDialog));
      this.clientView = new HeuristicLab.Clients.Access.Views.ClientView();
      this.refreshButton = new System.Windows.Forms.Button();
      this.okButton = new System.Windows.Forms.Button();
      this.registerClientButton = new System.Windows.Forms.Button();
      this.infoLabel = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // clientView
      // 
      this.clientView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.clientView.Caption = "Client View";
      this.clientView.Content = null;
      this.clientView.Location = new System.Drawing.Point(12, 42);
      this.clientView.Name = "clientView";
      this.clientView.ReadOnly = false;
      this.clientView.Size = new System.Drawing.Size(653, 315);
      this.clientView.TabIndex = 0;
      // 
      // refreshButton
      // 
      this.refreshButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Refresh;
      this.refreshButton.Location = new System.Drawing.Point(12, 12);
      this.refreshButton.Name = "refreshButton";
      this.refreshButton.Size = new System.Drawing.Size(24, 24);
      this.refreshButton.TabIndex = 1;
      this.refreshButton.UseVisualStyleBackColor = true;
      this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
      // 
      // okButton
      // 
      this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.okButton.Location = new System.Drawing.Point(590, 363);
      this.okButton.Name = "okButton";
      this.okButton.Size = new System.Drawing.Size(75, 23);
      this.okButton.TabIndex = 2;
      this.okButton.Text = "OK";
      this.okButton.UseVisualStyleBackColor = true;
      this.okButton.Click += new System.EventHandler(this.okButton_Click);
      // 
      // registerClientButton
      // 
      this.registerClientButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.registerClientButton.Location = new System.Drawing.Point(13, 362);
      this.registerClientButton.Name = "registerClientButton";
      this.registerClientButton.Size = new System.Drawing.Size(84, 23);
      this.registerClientButton.TabIndex = 3;
      this.registerClientButton.Text = "Register client";
      this.registerClientButton.UseVisualStyleBackColor = true;
      this.registerClientButton.Visible = false;
      this.registerClientButton.Click += new System.EventHandler(this.registerClientButton_Click);
      // 
      // infoLabel
      // 
      this.infoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.infoLabel.AutoSize = true;
      this.infoLabel.Location = new System.Drawing.Point(103, 367);
      this.infoLabel.Name = "infoLabel";
      this.infoLabel.Size = new System.Drawing.Size(434, 13);
      this.infoLabel.TabIndex = 4;
      this.infoLabel.Text = "Your client isn\'t registered yet. Click on the Register client button to use addi" +
          "tional services.";
      this.infoLabel.Visible = false;
      // 
      // ClientInformationDialog
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(677, 398);
      this.Controls.Add(this.infoLabel);
      this.Controls.Add(this.registerClientButton);
      this.Controls.Add(this.okButton);
      this.Controls.Add(this.refreshButton);
      this.Controls.Add(this.clientView);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ClientInformationDialog";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Client Information";
      this.TopMost = true;
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private ClientView clientView;
    private System.Windows.Forms.Button refreshButton;
    private System.Windows.Forms.Button okButton;
    private System.Windows.Forms.Button registerClientButton;
    private System.Windows.Forms.Label infoLabel;
  }
}