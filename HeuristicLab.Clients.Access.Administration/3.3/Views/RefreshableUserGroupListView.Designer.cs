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

namespace HeuristicLab.Clients.Access.Administration {
  partial class RefreshableUserGroupListView {
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RefreshableUserGroupListView));
      this.userGroupListView = new HeuristicLab.Clients.Access.Administration.UserGroupListView();
      this.storeButton = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // userGroupListView
      // 
      this.userGroupListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.userGroupListView.Caption = "UserGroupList View";
      this.userGroupListView.Content = null;
      this.userGroupListView.Location = new System.Drawing.Point(3, 33);
      this.userGroupListView.Name = "userGroupListView";
      this.userGroupListView.ReadOnly = false;
      this.userGroupListView.Size = new System.Drawing.Size(559, 375);
      this.userGroupListView.TabIndex = 2;
      // 
      // storeButton
      // 
      this.storeButton.Enabled = false;
      this.storeButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Save;
      this.storeButton.Location = new System.Drawing.Point(33, 3);
      this.storeButton.Name = "storeButton";
      this.storeButton.Size = new System.Drawing.Size(24, 24);
      this.storeButton.TabIndex = 4;
      this.storeButton.UseVisualStyleBackColor = true;
      this.storeButton.Click += new System.EventHandler(this.storeButton_Click);
      // 
      // RefreshableUserGroupListView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.storeButton);
      this.Controls.Add(this.userGroupListView);
      this.Name = "RefreshableUserGroupListView";
      this.Size = new System.Drawing.Size(565, 411);
      this.Controls.SetChildIndex(this.userGroupListView, 0);
      this.Controls.SetChildIndex(this.refreshButton, 0);
      this.Controls.SetChildIndex(this.storeButton, 0);
      this.ResumeLayout(false);

    }

    #endregion

    private UserGroupListView userGroupListView;
    private System.Windows.Forms.Button storeButton;

  }
}
