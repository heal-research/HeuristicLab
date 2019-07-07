#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  partial class UserInformationDialog {
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
      this.closeButton = new System.Windows.Forms.Button();
      this.refreshableLightweightUserInformationView = new HeuristicLab.Clients.Access.Views.RefreshableLightweightAccessClientInformationView();
      this.SuspendLayout();
      // 
      // closeButton
      // 
      this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.closeButton.Location = new System.Drawing.Point(16, 245);
      this.closeButton.Name = "closeButton";
      this.closeButton.Size = new System.Drawing.Size(75, 23);
      this.closeButton.TabIndex = 1;
      this.closeButton.Text = "OK";
      this.closeButton.UseVisualStyleBackColor = true;
      this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
      // 
      // refreshableLightweightUserInformationView
      // 
      this.refreshableLightweightUserInformationView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.refreshableLightweightUserInformationView.Caption = "";
      this.refreshableLightweightUserInformationView.Content = null;
      this.refreshableLightweightUserInformationView.Location = new System.Drawing.Point(12, 12);
      this.refreshableLightweightUserInformationView.Name = "refreshableLightweightUserInformationView";
      this.refreshableLightweightUserInformationView.ReadOnly = false;
      this.refreshableLightweightUserInformationView.Size = new System.Drawing.Size(362, 262);
      this.refreshableLightweightUserInformationView.TabIndex = 0;
      // 
      // UserInformationDialog
      // 
      this.AcceptButton = this.closeButton;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(386, 286);
      this.Controls.Add(this.closeButton);
      this.Controls.Add(this.refreshableLightweightUserInformationView);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "UserInformationDialog";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "User Information";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UserInformationDialog_FormClosing);
      this.ResumeLayout(false);

    }

    #endregion

    private RefreshableLightweightAccessClientInformationView refreshableLightweightUserInformationView;
    private System.Windows.Forms.Button closeButton;
  }
}