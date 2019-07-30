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

namespace HeuristicLab.Clients.Hive.Administrator.Views {
  partial class HiveAdministratorView {
    private System.Windows.Forms.TabControl tabAdmin;
    private HeuristicLab.Clients.Hive.Administrator.Views.ResourcesView resourcesView;

    #region Component Designer generated code
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      this.tabAdmin = new System.Windows.Forms.TabControl();
      this.tabResources = new System.Windows.Forms.TabPage();
      this.resourcesView = new HeuristicLab.Clients.Hive.Administrator.Views.ResourcesView();
      this.tabProjects = new System.Windows.Forms.TabPage();
      this.imageListUsers = new System.Windows.Forms.ImageList(this.components);
      this.projectsView = new HeuristicLab.Clients.Hive.Administrator.Views.ProjectsView();
      this.tabAdmin.SuspendLayout();
      this.tabResources.SuspendLayout();
      this.tabProjects.SuspendLayout();
      this.SuspendLayout();
      // 
      // tabAdmin
      // 
      this.tabAdmin.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tabAdmin.Controls.Add(this.tabResources);
      this.tabAdmin.Controls.Add(this.tabProjects);
      this.tabAdmin.Location = new System.Drawing.Point(3, 0);
      this.tabAdmin.Name = "tabAdmin";
      this.tabAdmin.SelectedIndex = 0;
      this.tabAdmin.Size = new System.Drawing.Size(742, 546);
      this.tabAdmin.TabIndex = 0;
      // 
      // tabResources
      // 
      this.tabResources.Controls.Add(this.resourcesView);
      this.tabResources.Location = new System.Drawing.Point(4, 22);
      this.tabResources.Name = "tabResources";
      this.tabResources.Padding = new System.Windows.Forms.Padding(3);
      this.tabResources.Size = new System.Drawing.Size(734, 520);
      this.tabResources.TabIndex = 3;
      this.tabResources.Text = "Resources";
      this.tabResources.UseVisualStyleBackColor = true;
      // 
      // resourcesView
      // 
      this.resourcesView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.resourcesView.Caption = "ResourcesView";
      this.resourcesView.Content = null;
      this.resourcesView.Location = new System.Drawing.Point(0, 0);
      this.resourcesView.Name = "resourcesView";
      this.resourcesView.ReadOnly = false;
      this.resourcesView.Size = new System.Drawing.Size(734, 520);
      this.resourcesView.TabIndex = 0;
      // 
      // tabProjects
      // 
      this.tabProjects.Controls.Add(this.projectsView);
      this.tabProjects.Location = new System.Drawing.Point(4, 22);
      this.tabProjects.Name = "tabProjects";
      this.tabProjects.Size = new System.Drawing.Size(734, 520);
      this.tabProjects.TabIndex = 4;
      this.tabProjects.Text = "Projects";
      this.tabProjects.UseVisualStyleBackColor = true;
      // 
      // imageListUsers
      // 
      this.imageListUsers.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
      this.imageListUsers.ImageSize = new System.Drawing.Size(16, 16);
      this.imageListUsers.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // projectsView
      // 
      this.projectsView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.projectsView.Caption = "Resources View";
      this.projectsView.Content = null;
      this.projectsView.Location = new System.Drawing.Point(0, 0);
      this.projectsView.Name = "projectsView";
      this.projectsView.ReadOnly = false;
      this.projectsView.Size = new System.Drawing.Size(734, 520);
      this.projectsView.TabIndex = 0;
      // 
      // HiveAdministratorView
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.tabAdmin);
      this.Name = "HiveAdministratorView";
      this.Size = new System.Drawing.Size(745, 546);
      this.tabAdmin.ResumeLayout(false);
      this.tabResources.ResumeLayout(false);
      this.tabProjects.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.ComponentModel.IContainer components;
    private System.Windows.Forms.ImageList imageListUsers;
    private System.Windows.Forms.TabPage tabResources;
    private System.Windows.Forms.TabPage tabProjects;
    private ProjectsView projectsView;
  }
}
