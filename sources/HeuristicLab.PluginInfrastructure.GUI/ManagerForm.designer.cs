#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.PluginInfrastructure.GUI {
  partial class ManagerForm {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if(disposing && (components != null)) {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManagerForm));
      this.menuStrip = new System.Windows.Forms.MenuStrip();
      this.pluginsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.managePluginSourcesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.installPluginFromFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
      this.installedPluginsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.installNewPluginsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
      this.refreshPluginListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.pluginTreeView = new System.Windows.Forms.TreeView();
      this.pluginIcons = new System.Windows.Forms.ImageList(this.components);
      this.infoTextBox = new System.Windows.Forms.RichTextBox();
      this.toolStrip = new System.Windows.Forms.ToolStrip();
      this.updateButton = new System.Windows.Forms.ToolStripButton();
      this.upgradeButton = new System.Windows.Forms.ToolStripButton();
      this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
      this.installButton = new System.Windows.Forms.ToolStripButton();
      this.removeButton = new System.Windows.Forms.ToolStripButton();
      this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.publishButton = new System.Windows.Forms.ToolStripButton();
      this.pluginContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.installMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.removeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.publishMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.menuStrip.SuspendLayout();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.toolStrip.SuspendLayout();
      this.pluginContextMenuStrip.SuspendLayout();
      this.SuspendLayout();
      // 
      // menuStrip
      // 
      this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pluginsToolStripMenuItem,
            this.helpToolStripMenuItem});
      this.menuStrip.Location = new System.Drawing.Point(0, 0);
      this.menuStrip.Name = "menuStrip";
      this.menuStrip.Size = new System.Drawing.Size(828, 24);
      this.menuStrip.TabIndex = 0;
      this.menuStrip.Text = "menuStrip";
      // 
      // pluginsToolStripMenuItem
      // 
      this.pluginsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.managePluginSourcesToolStripMenuItem,
            this.installPluginFromFileToolStripMenuItem,
            this.toolStripSeparator2,
            this.installedPluginsToolStripMenuItem,
            this.installNewPluginsToolStripMenuItem,
            this.toolStripSeparator3,
            this.refreshPluginListToolStripMenuItem,
            this.exitToolStripMenuItem});
      this.pluginsToolStripMenuItem.Name = "pluginsToolStripMenuItem";
      this.pluginsToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
      this.pluginsToolStripMenuItem.Text = "Plugins";
      // 
      // managePluginSourcesToolStripMenuItem
      // 
      this.managePluginSourcesToolStripMenuItem.Name = "managePluginSourcesToolStripMenuItem";
      this.managePluginSourcesToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
      this.managePluginSourcesToolStripMenuItem.Text = "Edit plugin sources...";
      this.managePluginSourcesToolStripMenuItem.Click += new System.EventHandler(this.managePluginSourcesToolStripMenuItem_Click);
      // 
      // installPluginFromFileToolStripMenuItem
      // 
      this.installPluginFromFileToolStripMenuItem.Name = "installPluginFromFileToolStripMenuItem";
      this.installPluginFromFileToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
      this.installPluginFromFileToolStripMenuItem.Text = "Install plugin from file...";
      this.installPluginFromFileToolStripMenuItem.Click += new System.EventHandler(this.installPluginFromFileToolStripMenuItem_Click);
      // 
      // toolStripSeparator2
      // 
      this.toolStripSeparator2.Name = "toolStripSeparator2";
      this.toolStripSeparator2.Size = new System.Drawing.Size(211, 6);
      // 
      // installedPluginsToolStripMenuItem
      // 
      this.installedPluginsToolStripMenuItem.Name = "installedPluginsToolStripMenuItem";
      this.installedPluginsToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
      this.installedPluginsToolStripMenuItem.Text = "Update";
      this.installedPluginsToolStripMenuItem.Click += new System.EventHandler(this.updateButton_Click);
      // 
      // installNewPluginsToolStripMenuItem
      // 
      this.installNewPluginsToolStripMenuItem.Name = "installNewPluginsToolStripMenuItem";
      this.installNewPluginsToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
      this.installNewPluginsToolStripMenuItem.Text = "Remove/Upgrade/Install...";
      this.installNewPluginsToolStripMenuItem.Click += new System.EventHandler(this.upgradeButton_Click);
      // 
      // toolStripSeparator3
      // 
      this.toolStripSeparator3.Name = "toolStripSeparator3";
      this.toolStripSeparator3.Size = new System.Drawing.Size(211, 6);
      // 
      // refreshPluginListToolStripMenuItem
      // 
      this.refreshPluginListToolStripMenuItem.Name = "refreshPluginListToolStripMenuItem";
      this.refreshPluginListToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
      this.refreshPluginListToolStripMenuItem.Text = "Refresh plugin list";
      this.refreshPluginListToolStripMenuItem.Click += new System.EventHandler(this.refreshPluginListToolStripMenuItem_Click);
      // 
      // exitToolStripMenuItem
      // 
      this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
      this.exitToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
      this.exitToolStripMenuItem.Text = "Close";
      this.exitToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
      // 
      // helpToolStripMenuItem
      // 
      this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
      this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
      this.helpToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
      this.helpToolStripMenuItem.Text = "Help";
      // 
      // aboutToolStripMenuItem
      // 
      this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
      this.aboutToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
      this.aboutToolStripMenuItem.Text = "About...";
      this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
      // 
      // splitContainer
      // 
      this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.splitContainer.Location = new System.Drawing.Point(0, 52);
      this.splitContainer.Name = "splitContainer";
      this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.pluginTreeView);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.infoTextBox);
      this.splitContainer.Size = new System.Drawing.Size(828, 473);
      this.splitContainer.SplitterDistance = 220;
      this.splitContainer.TabIndex = 1;
      // 
      // pluginTreeView
      // 
      this.pluginTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.pluginTreeView.ImageIndex = 0;
      this.pluginTreeView.ImageList = this.pluginIcons;
      this.pluginTreeView.Location = new System.Drawing.Point(0, 0);
      this.pluginTreeView.Name = "pluginTreeView";
      this.pluginTreeView.SelectedImageIndex = 0;
      this.pluginTreeView.Size = new System.Drawing.Size(828, 220);
      this.pluginTreeView.TabIndex = 0;
      this.pluginTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.pluginTreeView_NodeMouseClick);
      this.pluginTreeView.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.pluginTreeView_BeforeSelect);
      // 
      // pluginIcons
      // 
      this.pluginIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("pluginIcons.ImageStream")));
      this.pluginIcons.TransparentColor = System.Drawing.Color.Magenta;
      this.pluginIcons.Images.SetKeyName(0, "VSObject_Module.bmp");
      this.pluginIcons.Images.SetKeyName(1, "VSObject_Namespace.bmp");
      this.pluginIcons.Images.SetKeyName(2, "install.bmp");
      this.pluginIcons.Images.SetKeyName(3, "delete.bmp");
      this.pluginIcons.Images.SetKeyName(4, "genericInternet.bmp");
      // 
      // infoTextBox
      // 
      this.infoTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.infoTextBox.Location = new System.Drawing.Point(0, 0);
      this.infoTextBox.Name = "infoTextBox";
      this.infoTextBox.Size = new System.Drawing.Size(828, 249);
      this.infoTextBox.TabIndex = 0;
      this.infoTextBox.Text = "";
      // 
      // toolStrip
      // 
      this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.updateButton,
            this.upgradeButton,
            this.toolStripSeparator,
            this.installButton,
            this.removeButton,
            this.toolStripSeparator1,
            this.publishButton});
      this.toolStrip.Location = new System.Drawing.Point(0, 24);
      this.toolStrip.Name = "toolStrip";
      this.toolStrip.Size = new System.Drawing.Size(828, 25);
      this.toolStrip.TabIndex = 2;
      this.toolStrip.Text = "toolStrip";
      // 
      // updateButton
      // 
      this.updateButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.updateButton.Image = ((System.Drawing.Image)(resources.GetObject("updateButton.Image")));
      this.updateButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.updateButton.Name = "updateButton";
      this.updateButton.Size = new System.Drawing.Size(46, 22);
      this.updateButton.Text = "U&pdate";
      this.updateButton.Click += new System.EventHandler(this.updateButton_Click);
      // 
      // upgradeButton
      // 
      this.upgradeButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.upgradeButton.Image = ((System.Drawing.Image)(resources.GetObject("upgradeButton.Image")));
      this.upgradeButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.upgradeButton.Name = "upgradeButton";
      this.upgradeButton.Size = new System.Drawing.Size(140, 22);
      this.upgradeButton.Text = "Remove/&Upgrade/Install...";
      this.upgradeButton.Click += new System.EventHandler(this.upgradeButton_Click);
      // 
      // toolStripSeparator
      // 
      this.toolStripSeparator.Name = "toolStripSeparator";
      this.toolStripSeparator.Size = new System.Drawing.Size(6, 25);
      // 
      // installButton
      // 
      this.installButton.CheckOnClick = true;
      this.installButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.installButton.Image = ((System.Drawing.Image)(resources.GetObject("installButton.Image")));
      this.installButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.installButton.Name = "installButton";
      this.installButton.Size = new System.Drawing.Size(40, 22);
      this.installButton.Text = "&Install";
      this.installButton.Click += new System.EventHandler(this.installButton_Clicked);
      // 
      // removeButton
      // 
      this.removeButton.CheckOnClick = true;
      this.removeButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.removeButton.Image = ((System.Drawing.Image)(resources.GetObject("removeButton.Image")));
      this.removeButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.removeButton.Name = "removeButton";
      this.removeButton.Size = new System.Drawing.Size(50, 22);
      this.removeButton.Text = "&Remove";
      this.removeButton.Click += new System.EventHandler(this.removeButton_Clicked);
      // 
      // toolStripSeparator1
      // 
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
      // 
      // publishButton
      // 
      this.publishButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.publishButton.Image = ((System.Drawing.Image)(resources.GetObject("publishButton.Image")));
      this.publishButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.publishButton.Name = "publishButton";
      this.publishButton.Size = new System.Drawing.Size(44, 22);
      this.publishButton.Text = "Publish";
      this.publishButton.Click += new System.EventHandler(this.publishButton_Click);
      // 
      // pluginContextMenuStrip
      // 
      this.pluginContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.installMenuItem,
            this.removeMenuItem,
            this.publishMenuItem});
      this.pluginContextMenuStrip.Name = "pluginContextMenuStrip";
      this.pluginContextMenuStrip.Size = new System.Drawing.Size(125, 70);
      // 
      // installMenuItem
      // 
      this.installMenuItem.Name = "installMenuItem";
      this.installMenuItem.Size = new System.Drawing.Size(124, 22);
      this.installMenuItem.Text = "Install";
      this.installMenuItem.Click += new System.EventHandler(this.installButton_Clicked);
      // 
      // removeMenuItem
      // 
      this.removeMenuItem.Name = "removeMenuItem";
      this.removeMenuItem.Size = new System.Drawing.Size(124, 22);
      this.removeMenuItem.Text = "Remove";
      this.removeMenuItem.Click += new System.EventHandler(this.removeButton_Clicked);
      // 
      // publishMenuItem
      // 
      this.publishMenuItem.Name = "publishMenuItem";
      this.publishMenuItem.Size = new System.Drawing.Size(124, 22);
      this.publishMenuItem.Text = "Publish";
      this.publishMenuItem.Click += new System.EventHandler(this.publishButton_Click);
      // 
      // ManagerForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(828, 537);
      this.Controls.Add(this.toolStrip);
      this.Controls.Add(this.splitContainer);
      this.Controls.Add(this.menuStrip);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MainMenuStrip = this.menuStrip;
      this.Name = "ManagerForm";
      this.Text = "Heuristiclab Pluginmanager Console";
      this.menuStrip.ResumeLayout(false);
      this.menuStrip.PerformLayout();
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      this.splitContainer.ResumeLayout(false);
      this.toolStrip.ResumeLayout(false);
      this.toolStrip.PerformLayout();
      this.pluginContextMenuStrip.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.MenuStrip menuStrip;
    private System.Windows.Forms.ToolStripMenuItem pluginsToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem installedPluginsToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem installNewPluginsToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem managePluginSourcesToolStripMenuItem;
    private System.Windows.Forms.SplitContainer splitContainer;
    private System.Windows.Forms.TreeView pluginTreeView;
    private System.Windows.Forms.RichTextBox infoTextBox;
    private System.Windows.Forms.ToolStripMenuItem refreshPluginListToolStripMenuItem;
    private System.Windows.Forms.ToolStrip toolStrip;
    private System.Windows.Forms.ToolStripButton updateButton;
    private System.Windows.Forms.ToolStripButton upgradeButton;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
    private System.Windows.Forms.ToolStripButton installButton;
    private System.Windows.Forms.ToolStripButton removeButton;
    private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    private System.Windows.Forms.ToolStripButton publishButton;
    private System.Windows.Forms.ContextMenuStrip pluginContextMenuStrip;
    private System.Windows.Forms.ToolStripMenuItem installMenuItem;
    private System.Windows.Forms.ToolStripMenuItem removeMenuItem;
    private System.Windows.Forms.ToolStripMenuItem publishMenuItem;
    private System.Windows.Forms.ToolStripMenuItem installPluginFromFileToolStripMenuItem;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
    private System.Windows.Forms.ImageList pluginIcons;
  }
}

