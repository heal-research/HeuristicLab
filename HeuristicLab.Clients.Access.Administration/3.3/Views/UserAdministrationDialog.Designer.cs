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

namespace HeuristicLab.Clients.Access.Administration {
  partial class UserAdministrationDialog {
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
      this.refreshableUserListView = new HeuristicLab.Clients.Access.Administration.RefreshableUserListView();
      this.mainTabControl = new System.Windows.Forms.TabControl();
      this.usersTabPage = new System.Windows.Forms.TabPage();
      this.UserGroupsTabPage = new System.Windows.Forms.TabPage();
      this.refreshableUserGroupListView = new HeuristicLab.Clients.Access.Administration.RefreshableUserGroupListView();
      this.rolesTabPage = new System.Windows.Forms.TabPage();
      this.refreshableRoleListView = new HeuristicLab.Clients.Access.Administration.RefreshableRoleListView();
      this.mainTabControl.SuspendLayout();
      this.usersTabPage.SuspendLayout();
      this.UserGroupsTabPage.SuspendLayout();
      this.rolesTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // refreshableUserListView
      // 
      this.refreshableUserListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.refreshableUserListView.Caption = "View";
      this.refreshableUserListView.Content = null;
      this.refreshableUserListView.Location = new System.Drawing.Point(6, 6);
      this.refreshableUserListView.Name = "refreshableUserListView";
      this.refreshableUserListView.ReadOnly = false;
      this.refreshableUserListView.Size = new System.Drawing.Size(921, 527);
      this.refreshableUserListView.TabIndex = 0;
      // 
      // mainTabControl
      // 
      this.mainTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.mainTabControl.Controls.Add(this.usersTabPage);
      this.mainTabControl.Controls.Add(this.UserGroupsTabPage);
      this.mainTabControl.Controls.Add(this.rolesTabPage);
      this.mainTabControl.Location = new System.Drawing.Point(12, 12);
      this.mainTabControl.Name = "mainTabControl";
      this.mainTabControl.SelectedIndex = 0;
      this.mainTabControl.Size = new System.Drawing.Size(941, 565);
      this.mainTabControl.TabIndex = 1;
      // 
      // usersTabPage
      // 
      this.usersTabPage.Controls.Add(this.refreshableUserListView);
      this.usersTabPage.Location = new System.Drawing.Point(4, 22);
      this.usersTabPage.Name = "usersTabPage";
      this.usersTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.usersTabPage.Size = new System.Drawing.Size(933, 539);
      this.usersTabPage.TabIndex = 0;
      this.usersTabPage.Text = "Users";
      this.usersTabPage.UseVisualStyleBackColor = true;
      // 
      // UserGroupsTabPage
      // 
      this.UserGroupsTabPage.Controls.Add(this.refreshableUserGroupListView);
      this.UserGroupsTabPage.Location = new System.Drawing.Point(4, 22);
      this.UserGroupsTabPage.Name = "UserGroupsTabPage";
      this.UserGroupsTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.UserGroupsTabPage.Size = new System.Drawing.Size(774, 376);
      this.UserGroupsTabPage.TabIndex = 1;
      this.UserGroupsTabPage.Text = "Groups";
      this.UserGroupsTabPage.UseVisualStyleBackColor = true;
      // 
      // refreshableUserGroupListView
      // 
      this.refreshableUserGroupListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.refreshableUserGroupListView.Caption = "View";
      this.refreshableUserGroupListView.Content = null;
      this.refreshableUserGroupListView.Location = new System.Drawing.Point(6, 6);
      this.refreshableUserGroupListView.Name = "refreshableUserGroupListView";
      this.refreshableUserGroupListView.ReadOnly = false;
      this.refreshableUserGroupListView.Size = new System.Drawing.Size(762, 364);
      this.refreshableUserGroupListView.TabIndex = 0;
      // 
      // rolesTabPage
      // 
      this.rolesTabPage.Controls.Add(this.refreshableRoleListView);
      this.rolesTabPage.Location = new System.Drawing.Point(4, 22);
      this.rolesTabPage.Name = "rolesTabPage";
      this.rolesTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.rolesTabPage.Size = new System.Drawing.Size(774, 376);
      this.rolesTabPage.TabIndex = 2;
      this.rolesTabPage.Text = "Roles";
      this.rolesTabPage.UseVisualStyleBackColor = true;
      // 
      // refreshableRoleListView
      // 
      this.refreshableRoleListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.refreshableRoleListView.Caption = "View";
      this.refreshableRoleListView.Content = null;
      this.refreshableRoleListView.Location = new System.Drawing.Point(6, 6);
      this.refreshableRoleListView.Name = "refreshableRoleListView";
      this.refreshableRoleListView.ReadOnly = false;
      this.refreshableRoleListView.Size = new System.Drawing.Size(762, 364);
      this.refreshableRoleListView.TabIndex = 0;
      // 
      // UserAdministrationDialog
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(965, 589);
      this.Controls.Add(this.mainTabControl);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "UserAdministrationDialog";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "User Administration";
      this.Load += new System.EventHandler(this.UserAdministrationDialog_Load);
      this.mainTabControl.ResumeLayout(false);
      this.usersTabPage.ResumeLayout(false);
      this.UserGroupsTabPage.ResumeLayout(false);
      this.rolesTabPage.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private RefreshableUserListView refreshableUserListView;
    private System.Windows.Forms.TabControl mainTabControl;
    private System.Windows.Forms.TabPage usersTabPage;
    private System.Windows.Forms.TabPage UserGroupsTabPage;
    private RefreshableUserGroupListView refreshableUserGroupListView;
    private System.Windows.Forms.TabPage rolesTabPage;
    private RefreshableRoleListView refreshableRoleListView;
  }
}