#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  partial class InstallationManagerForm {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InstallationManagerForm));
      this.statusStrip = new System.Windows.Forms.StatusStrip();
      this.toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
      this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
      this.removeButton = new System.Windows.Forms.Button();
      this.installButton = new System.Windows.Forms.Button();
      this.tabControl = new System.Windows.Forms.TabControl();
      this.localPluginsTabPage = new System.Windows.Forms.TabPage();
      this.updateButton = new System.Windows.Forms.Button();
      this.availablePluginsTabPage = new System.Windows.Forms.TabPage();
      this.refreshButton = new System.Windows.Forms.Button();
      this.uploadPluginsTabPage = new System.Windows.Forms.TabPage();
      this.manageProductsTabPage = new System.Windows.Forms.TabPage();
      this.logTabPage = new System.Windows.Forms.TabPage();
      this.logTextBox = new System.Windows.Forms.TextBox();
      this.menuStrip = new System.Windows.Forms.MenuStrip();
      this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.simpleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.advancedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.connectionSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.localPluginManagerView = new HeuristicLab.PluginInfrastructure.Advanced.LocalPluginManagerView();
      this.remotePluginInstaller = new HeuristicLab.PluginInfrastructure.Advanced.RemotePluginInstallerView();
      this.pluginEditor = new HeuristicLab.PluginInfrastructure.Advanced.PluginEditor();
      this.productEditor = new HeuristicLab.PluginInfrastructure.Advanced.ProductEditor();
      this.statusStrip.SuspendLayout();
      this.tabControl.SuspendLayout();
      this.localPluginsTabPage.SuspendLayout();
      this.availablePluginsTabPage.SuspendLayout();
      this.uploadPluginsTabPage.SuspendLayout();
      this.manageProductsTabPage.SuspendLayout();
      this.logTabPage.SuspendLayout();
      this.menuStrip.SuspendLayout();
      this.SuspendLayout();
      // 
      // statusStrip
      // 
      this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar,
            this.toolStripStatusLabel});
      this.statusStrip.Location = new System.Drawing.Point(0, 398);
      this.statusStrip.Name = "statusStrip";
      this.statusStrip.Size = new System.Drawing.Size(615, 22);
      this.statusStrip.TabIndex = 0;
      // 
      // toolStripProgressBar
      // 
      this.toolStripProgressBar.MarqueeAnimationSpeed = 30;
      this.toolStripProgressBar.Name = "toolStripProgressBar";
      this.toolStripProgressBar.Size = new System.Drawing.Size(100, 16);
      this.toolStripProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
      this.toolStripProgressBar.Visible = false;
      // 
      // toolStripStatusLabel
      // 
      this.toolStripStatusLabel.Name = "toolStripStatusLabel";
      this.toolStripStatusLabel.Size = new System.Drawing.Size(0, 17);
      // 
      // removeButton
      // 
      this.removeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.removeButton.Enabled = false;
      this.removeButton.Image = global::HeuristicLab.PluginInfrastructure.Properties.Resources.VS2008ImageLibrary_CommonElements_Actions_Remove;
      this.removeButton.Location = new System.Drawing.Point(103, 305);
      this.removeButton.Name = "removeButton";
      this.removeButton.Size = new System.Drawing.Size(110, 31);
      this.removeButton.TabIndex = 11;
      this.removeButton.Text = "Delete Selected";
      this.removeButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
      this.removeButton.UseVisualStyleBackColor = true;
      this.removeButton.Click += new System.EventHandler(this.removeLocalButton_Click);
      // 
      // installButton
      // 
      this.installButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.installButton.Enabled = false;
      this.installButton.Image = global::HeuristicLab.PluginInfrastructure.Properties.Resources.VS2008ImageLibrary_Objects_Install;
      this.installButton.Location = new System.Drawing.Point(84, 305);
      this.installButton.Name = "installButton";
      this.installButton.Size = new System.Drawing.Size(140, 31);
      this.installButton.TabIndex = 15;
      this.installButton.Text = "Install Selected Items";
      this.installButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
      this.installButton.UseVisualStyleBackColor = true;
      this.installButton.Click += new System.EventHandler(this.updateOrInstallButton_Click);
      // 
      // tabControl
      // 
      this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl.Controls.Add(this.localPluginsTabPage);
      this.tabControl.Controls.Add(this.availablePluginsTabPage);
      this.tabControl.Controls.Add(this.uploadPluginsTabPage);
      this.tabControl.Controls.Add(this.manageProductsTabPage);
      this.tabControl.Controls.Add(this.logTabPage);
      this.tabControl.Location = new System.Drawing.Point(12, 27);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(591, 368);
      this.tabControl.TabIndex = 16;
      this.tabControl.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabControl_Selected);
      // 
      // localPluginsTabPage
      // 
      this.localPluginsTabPage.Controls.Add(this.updateButton);
      this.localPluginsTabPage.Controls.Add(this.removeButton);
      this.localPluginsTabPage.Controls.Add(this.localPluginManagerView);
      this.localPluginsTabPage.Location = new System.Drawing.Point(4, 22);
      this.localPluginsTabPage.Name = "localPluginsTabPage";
      this.localPluginsTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.localPluginsTabPage.Size = new System.Drawing.Size(583, 342);
      this.localPluginsTabPage.TabIndex = 0;
      this.localPluginsTabPage.Text = "Installed Plugins";
      this.localPluginsTabPage.UseVisualStyleBackColor = true;
      // 
      // updateButton
      // 
      this.updateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.updateButton.Enabled = false;
      this.updateButton.Image = global::HeuristicLab.PluginInfrastructure.Properties.Resources.VS2008ImageLibrary_Objects_Internet;
      this.updateButton.Location = new System.Drawing.Point(6, 305);
      this.updateButton.Name = "updateButton";
      this.updateButton.Size = new System.Drawing.Size(91, 31);
      this.updateButton.TabIndex = 12;
      this.updateButton.Text = "Update All";
      this.updateButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
      this.updateButton.UseVisualStyleBackColor = true;
      this.updateButton.Click += new System.EventHandler(this.updateAllButton_Click);
      // 
      // availablePluginsTabPage
      // 
      this.availablePluginsTabPage.Controls.Add(this.remotePluginInstaller);
      this.availablePluginsTabPage.Controls.Add(this.refreshButton);
      this.availablePluginsTabPage.Controls.Add(this.installButton);
      this.availablePluginsTabPage.Location = new System.Drawing.Point(4, 22);
      this.availablePluginsTabPage.Name = "availablePluginsTabPage";
      this.availablePluginsTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.availablePluginsTabPage.Size = new System.Drawing.Size(583, 342);
      this.availablePluginsTabPage.TabIndex = 1;
      this.availablePluginsTabPage.Text = "Available Plugins";
      this.availablePluginsTabPage.UseVisualStyleBackColor = true;
      // 
      // refreshButton
      // 
      this.refreshButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.refreshButton.Image = global::HeuristicLab.PluginInfrastructure.Properties.Resources.VS2008ImageLibrary_Objects_Internet;
      this.refreshButton.Location = new System.Drawing.Point(6, 305);
      this.refreshButton.Name = "refreshButton";
      this.refreshButton.Size = new System.Drawing.Size(72, 31);
      this.refreshButton.TabIndex = 11;
      this.refreshButton.Text = "Refresh";
      this.refreshButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
      this.refreshButton.UseVisualStyleBackColor = true;
      this.refreshButton.Click += new System.EventHandler(this.refreshRemoteButton_Click);
      // 
      // uploadPluginsTabPage
      // 
      this.uploadPluginsTabPage.Controls.Add(this.pluginEditor);
      this.uploadPluginsTabPage.Location = new System.Drawing.Point(4, 22);
      this.uploadPluginsTabPage.Name = "uploadPluginsTabPage";
      this.uploadPluginsTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.uploadPluginsTabPage.Size = new System.Drawing.Size(583, 342);
      this.uploadPluginsTabPage.TabIndex = 3;
      this.uploadPluginsTabPage.Text = "Upload Plugins";
      this.uploadPluginsTabPage.UseVisualStyleBackColor = true;
      // 
      // manageProductsTabPage
      // 
      this.manageProductsTabPage.Controls.Add(this.productEditor);
      this.manageProductsTabPage.Location = new System.Drawing.Point(4, 22);
      this.manageProductsTabPage.Name = "manageProductsTabPage";
      this.manageProductsTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.manageProductsTabPage.Size = new System.Drawing.Size(583, 342);
      this.manageProductsTabPage.TabIndex = 4;
      this.manageProductsTabPage.Text = "Manage Products";
      this.manageProductsTabPage.UseVisualStyleBackColor = true;
      // 
      // logTabPage
      // 
      this.logTabPage.Controls.Add(this.logTextBox);
      this.logTabPage.Location = new System.Drawing.Point(4, 22);
      this.logTabPage.Name = "logTabPage";
      this.logTabPage.Size = new System.Drawing.Size(583, 342);
      this.logTabPage.TabIndex = 2;
      this.logTabPage.Text = "Log";
      this.logTabPage.UseVisualStyleBackColor = true;
      // 
      // logTextBox
      // 
      this.logTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.logTextBox.Location = new System.Drawing.Point(3, 3);
      this.logTextBox.Multiline = true;
      this.logTextBox.Name = "logTextBox";
      this.logTextBox.ReadOnly = true;
      this.logTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.logTextBox.Size = new System.Drawing.Size(577, 336);
      this.logTextBox.TabIndex = 0;
      // 
      // menuStrip
      // 
      this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewToolStripMenuItem,
            this.optionsToolStripMenuItem});
      this.menuStrip.Location = new System.Drawing.Point(0, 0);
      this.menuStrip.Name = "menuStrip";
      this.menuStrip.Size = new System.Drawing.Size(615, 24);
      this.menuStrip.TabIndex = 17;
      this.menuStrip.Text = "menuStrip1";
      // 
      // viewToolStripMenuItem
      // 
      this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.simpleToolStripMenuItem,
            this.advancedToolStripMenuItem});
      this.viewToolStripMenuItem.Enabled = false;
      this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
      this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
      this.viewToolStripMenuItem.Text = "View";
      // 
      // simpleToolStripMenuItem
      // 
      this.simpleToolStripMenuItem.Checked = true;
      this.simpleToolStripMenuItem.CheckOnClick = true;
      this.simpleToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
      this.simpleToolStripMenuItem.Name = "simpleToolStripMenuItem";
      this.simpleToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
      this.simpleToolStripMenuItem.Text = "Show Most Recent Plugins";
      this.simpleToolStripMenuItem.Click += new System.EventHandler(this.simpleToolStripMenuItem_Click);
      // 
      // advancedToolStripMenuItem
      // 
      this.advancedToolStripMenuItem.CheckOnClick = true;
      this.advancedToolStripMenuItem.Name = "advancedToolStripMenuItem";
      this.advancedToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
      this.advancedToolStripMenuItem.Text = "Show All Plugins";
      this.advancedToolStripMenuItem.Click += new System.EventHandler(this.advancedToolStripMenuItem_Click);
      // 
      // optionsToolStripMenuItem
      // 
      this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectionSettingsToolStripMenuItem});
      this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
      this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
      this.optionsToolStripMenuItem.Text = "Options";
      // 
      // connectionSettingsToolStripMenuItem
      // 
      this.connectionSettingsToolStripMenuItem.Image = global::HeuristicLab.PluginInfrastructure.Properties.Resources.VS2008ImageLibrary_Objects_NetworkConnections;
      this.connectionSettingsToolStripMenuItem.Name = "connectionSettingsToolStripMenuItem";
      this.connectionSettingsToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
      this.connectionSettingsToolStripMenuItem.Text = "Connection Settings...";
      this.connectionSettingsToolStripMenuItem.Click += new System.EventHandler(this.connectionSettingsToolStripMenuItem_Click);
      // 
      // localPluginManagerView
      // 
      this.localPluginManagerView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.localPluginManagerView.Location = new System.Drawing.Point(6, 6);
      this.localPluginManagerView.Name = "localPluginManagerView";
      this.localPluginManagerView.Plugins = null;
      this.localPluginManagerView.Size = new System.Drawing.Size(569, 293);
      this.localPluginManagerView.TabIndex = 0;
      this.localPluginManagerView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.localPluginManager_ItemChecked);
      // 
      // remotePluginInstaller
      // 
      this.remotePluginInstaller.AllPlugins = new HeuristicLab.PluginInfrastructure.IPluginDescription[0];
      this.remotePluginInstaller.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.remotePluginInstaller.Location = new System.Drawing.Point(6, 6);
      this.remotePluginInstaller.Name = "remotePluginInstaller";
      this.remotePluginInstaller.NewPlugins = new HeuristicLab.PluginInfrastructure.IPluginDescription[0];
      this.remotePluginInstaller.Products = new HeuristicLab.PluginInfrastructure.Advanced.DeploymentService.ProductDescription[0];
      this.remotePluginInstaller.ShowAllPlugins = false;
      this.remotePluginInstaller.Size = new System.Drawing.Size(571, 293);
      this.remotePluginInstaller.TabIndex = 14;
      this.remotePluginInstaller.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.remotePluginInstaller_ItemChecked);
      // 
      // pluginEditor
      // 
      this.pluginEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.pluginEditor.Location = new System.Drawing.Point(6, 6);
      this.pluginEditor.Name = "pluginEditor";
      this.pluginEditor.Size = new System.Drawing.Size(571, 330);
      this.pluginEditor.TabIndex = 0;
      // 
      // productEditor
      // 
      this.productEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.productEditor.Location = new System.Drawing.Point(6, 6);
      this.productEditor.Name = "productEditor";
      this.productEditor.Size = new System.Drawing.Size(571, 330);
      this.productEditor.TabIndex = 0;
      // 
      // InstallationManagerForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(615, 420);
      this.Controls.Add(this.tabControl);
      this.Controls.Add(this.statusStrip);
      this.Controls.Add(this.menuStrip);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MainMenuStrip = this.menuStrip;
      this.Name = "InstallationManagerForm";
      this.Text = "Plugin Manager";
      this.statusStrip.ResumeLayout(false);
      this.statusStrip.PerformLayout();
      this.tabControl.ResumeLayout(false);
      this.localPluginsTabPage.ResumeLayout(false);
      this.availablePluginsTabPage.ResumeLayout(false);
      this.uploadPluginsTabPage.ResumeLayout(false);
      this.manageProductsTabPage.ResumeLayout(false);
      this.logTabPage.ResumeLayout(false);
      this.logTabPage.PerformLayout();
      this.menuStrip.ResumeLayout(false);
      this.menuStrip.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.StatusStrip statusStrip;
    private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar;
    private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
    private LocalPluginManagerView localPluginManagerView;
    private RemotePluginInstallerView remotePluginInstaller;
    private System.Windows.Forms.Button removeButton;
    private System.Windows.Forms.Button installButton;
    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TabPage localPluginsTabPage;
    private System.Windows.Forms.TabPage availablePluginsTabPage;
    private System.Windows.Forms.TabPage logTabPage;
    private System.Windows.Forms.TextBox logTextBox;
    private System.Windows.Forms.Button refreshButton;
    private System.Windows.Forms.MenuStrip menuStrip;
    private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem simpleToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem advancedToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem connectionSettingsToolStripMenuItem;
    private System.Windows.Forms.Button updateButton;
    private System.Windows.Forms.TabPage uploadPluginsTabPage;
    private System.Windows.Forms.TabPage manageProductsTabPage;
    private PluginEditor pluginEditor;
    private ProductEditor productEditor;
  }
}