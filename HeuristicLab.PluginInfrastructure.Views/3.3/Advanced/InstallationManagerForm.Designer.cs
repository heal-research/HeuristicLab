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
      this.components = new System.ComponentModel.Container();
      this.statusStrip = new System.Windows.Forms.StatusStrip();
      this.toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
      this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
      this.tabControl = new System.Windows.Forms.TabControl();
      this.localPluginsTabPage = new System.Windows.Forms.TabPage();
      this.localPluginsView = new HeuristicLab.PluginInfrastructure.Advanced.InstalledPluginsView();
      this.availablePluginsTabPage = new System.Windows.Forms.TabPage();
      this.remotePluginInstaller = new HeuristicLab.PluginInfrastructure.Advanced.AvailablePluginsView();
      this.uploadPluginsTabPage = new System.Windows.Forms.TabPage();
      this.pluginEditor = new HeuristicLab.PluginInfrastructure.Advanced.UploadPluginsView();
      this.manageProductsTabPage = new System.Windows.Forms.TabPage();
      this.productEditor = new HeuristicLab.PluginInfrastructure.Advanced.EditProductsView();
      this.logTabPage = new System.Windows.Forms.TabPage();
      this.logTextBox = new System.Windows.Forms.TextBox();
      this.menuStrip = new System.Windows.Forms.MenuStrip();
      this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.connectionSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
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
      this.statusStrip.Location = new System.Drawing.Point(0, 422);
      this.statusStrip.Name = "statusStrip";
      this.statusStrip.Size = new System.Drawing.Size(622, 22);
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
      this.tabControl.Size = new System.Drawing.Size(598, 392);
      this.tabControl.TabIndex = 16;
      this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
      // 
      // localPluginsTabPage
      // 
      this.localPluginsTabPage.Controls.Add(this.localPluginsView);
      this.localPluginsTabPage.Location = new System.Drawing.Point(4, 22);
      this.localPluginsTabPage.Name = "localPluginsTabPage";
      this.localPluginsTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.localPluginsTabPage.Size = new System.Drawing.Size(590, 366);
      this.localPluginsTabPage.TabIndex = 0;
      this.localPluginsTabPage.Text = "Installed Plugins";
      this.toolTip.SetToolTip(this.localPluginsTabPage, "Delete or update installed plugins");
      this.localPluginsTabPage.UseVisualStyleBackColor = true;
      // 
      // localPluginsView
      // 
      this.localPluginsView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.localPluginsView.InstallationManager = null;
      this.localPluginsView.Location = new System.Drawing.Point(6, 6);
      this.localPluginsView.Name = "localPluginsView";
      this.localPluginsView.PluginManager = null;
      this.localPluginsView.Size = new System.Drawing.Size(578, 354);
      this.localPluginsView.StatusView = null;
      this.localPluginsView.TabIndex = 0;
      // 
      // availablePluginsTabPage
      // 
      this.availablePluginsTabPage.Controls.Add(this.remotePluginInstaller);
      this.availablePluginsTabPage.Location = new System.Drawing.Point(4, 22);
      this.availablePluginsTabPage.Name = "availablePluginsTabPage";
      this.availablePluginsTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.availablePluginsTabPage.Size = new System.Drawing.Size(590, 366);
      this.availablePluginsTabPage.TabIndex = 1;
      this.availablePluginsTabPage.Text = "Available Plugins";
      this.toolTip.SetToolTip(this.availablePluginsTabPage, "Download and install new plugins");
      this.availablePluginsTabPage.UseVisualStyleBackColor = true;
      // 
      // remotePluginInstaller
      // 
      this.remotePluginInstaller.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.remotePluginInstaller.InstallationManager = null;
      this.remotePluginInstaller.Location = new System.Drawing.Point(6, 6);
      this.remotePluginInstaller.Name = "remotePluginInstaller";
      this.remotePluginInstaller.PluginManager = null;
      this.remotePluginInstaller.Size = new System.Drawing.Size(578, 354);
      this.remotePluginInstaller.StatusView = null;
      this.remotePluginInstaller.TabIndex = 14;
      // 
      // uploadPluginsTabPage
      // 
      this.uploadPluginsTabPage.Controls.Add(this.pluginEditor);
      this.uploadPluginsTabPage.Location = new System.Drawing.Point(4, 22);
      this.uploadPluginsTabPage.Name = "uploadPluginsTabPage";
      this.uploadPluginsTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.uploadPluginsTabPage.Size = new System.Drawing.Size(590, 366);
      this.uploadPluginsTabPage.TabIndex = 3;
      this.uploadPluginsTabPage.Text = "Upload Plugins";
      this.toolTip.SetToolTip(this.uploadPluginsTabPage, "Upload plugins");
      this.uploadPluginsTabPage.UseVisualStyleBackColor = true;
      // 
      // pluginEditor
      // 
      this.pluginEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.pluginEditor.Location = new System.Drawing.Point(6, 6);
      this.pluginEditor.Name = "pluginEditor";
      this.pluginEditor.PluginManager = null;
      this.pluginEditor.Size = new System.Drawing.Size(578, 354);
      this.pluginEditor.StatusView = null;
      this.pluginEditor.TabIndex = 0;
      // 
      // manageProductsTabPage
      // 
      this.manageProductsTabPage.Controls.Add(this.productEditor);
      this.manageProductsTabPage.Location = new System.Drawing.Point(4, 22);
      this.manageProductsTabPage.Name = "manageProductsTabPage";
      this.manageProductsTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.manageProductsTabPage.Size = new System.Drawing.Size(590, 366);
      this.manageProductsTabPage.TabIndex = 4;
      this.manageProductsTabPage.Text = "Manage Products";
      this.toolTip.SetToolTip(this.manageProductsTabPage, "Create and manage products");
      this.manageProductsTabPage.UseVisualStyleBackColor = true;
      // 
      // productEditor
      // 
      this.productEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.productEditor.Location = new System.Drawing.Point(6, 6);
      this.productEditor.Name = "productEditor";
      this.productEditor.Size = new System.Drawing.Size(578, 354);
      this.productEditor.StatusView = null;
      this.productEditor.TabIndex = 0;
      // 
      // logTabPage
      // 
      this.logTabPage.Controls.Add(this.logTextBox);
      this.logTabPage.Location = new System.Drawing.Point(4, 22);
      this.logTabPage.Name = "logTabPage";
      this.logTabPage.Size = new System.Drawing.Size(590, 366);
      this.logTabPage.TabIndex = 2;
      this.logTabPage.Text = "Log";
      this.toolTip.SetToolTip(this.logTabPage, "Show Log Messages");
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
      this.logTextBox.Size = new System.Drawing.Size(584, 360);
      this.logTextBox.TabIndex = 0;
      // 
      // menuStrip
      // 
      this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem});
      this.menuStrip.Location = new System.Drawing.Point(0, 0);
      this.menuStrip.Name = "menuStrip";
      this.menuStrip.Size = new System.Drawing.Size(622, 24);
      this.menuStrip.TabIndex = 17;
      this.menuStrip.Text = "menuStrip1";
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
      this.connectionSettingsToolStripMenuItem.Image = global::HeuristicLab.PluginInfrastructure.Resources.NetworkConnections;
      this.connectionSettingsToolStripMenuItem.Name = "connectionSettingsToolStripMenuItem";
      this.connectionSettingsToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
      this.connectionSettingsToolStripMenuItem.Text = "Connection Settings...";
      this.connectionSettingsToolStripMenuItem.Click += new System.EventHandler(this.connectionSettingsToolStripMenuItem_Click);
      // 
      // InstallationManagerForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.ClientSize = new System.Drawing.Size(622, 444);
      this.Controls.Add(this.tabControl);
      this.Controls.Add(this.statusStrip);
      this.Controls.Add(this.menuStrip);
      this.Icon = global::HeuristicLab.PluginInfrastructure.Resources.HeuristicLab;
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
    private AvailablePluginsView remotePluginInstaller;
    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TabPage localPluginsTabPage;
    private System.Windows.Forms.TabPage availablePluginsTabPage;
    private System.Windows.Forms.TabPage logTabPage;
    private System.Windows.Forms.TextBox logTextBox;
    private System.Windows.Forms.MenuStrip menuStrip;
    private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem connectionSettingsToolStripMenuItem;
    private System.Windows.Forms.TabPage uploadPluginsTabPage;
    private System.Windows.Forms.TabPage manageProductsTabPage;
    private UploadPluginsView pluginEditor;
    private EditProductsView productEditor;
    private InstalledPluginsView localPluginsView;
    private System.Windows.Forms.ToolTip toolTip;
  }
}