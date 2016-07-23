#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
      this.tabSlaves = new System.Windows.Forms.TabPage();
      this.imageListUsers = new System.Windows.Forms.ImageList(this.components);
      this.resourcesView = new HeuristicLab.Clients.Hive.Administrator.Views.ResourcesView();
      this.tabAdmin.SuspendLayout();
      this.tabSlaves.SuspendLayout();
      this.SuspendLayout();
      // 
      // tabAdmin
      // 
      this.tabAdmin.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.tabAdmin.Controls.Add(this.tabSlaves);
      this.tabAdmin.Location = new System.Drawing.Point(3, 0);
      this.tabAdmin.Name = "tabAdmin";
      this.tabAdmin.SelectedIndex = 0;
      this.tabAdmin.Size = new System.Drawing.Size(742, 546);
      this.tabAdmin.TabIndex = 0;
      // 
      // tabSlaves
      // 
      this.tabSlaves.Controls.Add(this.resourcesView);
      this.tabSlaves.Location = new System.Drawing.Point(4, 22);
      this.tabSlaves.Name = "tabSlaves";
      this.tabSlaves.Padding = new System.Windows.Forms.Padding(3);
      this.tabSlaves.Size = new System.Drawing.Size(734, 520);
      this.tabSlaves.TabIndex = 3;
      this.tabSlaves.Text = "Slaves";
      this.tabSlaves.UseVisualStyleBackColor = true;
      // 
      // imageListUsers
      // 
      this.imageListUsers.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
      this.imageListUsers.ImageSize = new System.Drawing.Size(16, 16);
      this.imageListUsers.TransparentColor = System.Drawing.Color.Transparent;
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
      // HiveAdministrationView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.tabAdmin);
      this.Name = "HiveAdministrationView";
      this.Size = new System.Drawing.Size(745, 546);
      this.tabAdmin.ResumeLayout(false);
      this.tabSlaves.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.ComponentModel.IContainer components;
    private System.Windows.Forms.ImageList imageListUsers;
    private System.Windows.Forms.TabPage tabSlaves;

  }
}
