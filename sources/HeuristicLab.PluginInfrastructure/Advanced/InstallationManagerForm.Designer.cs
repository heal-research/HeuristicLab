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
      this.statusStrip = new System.Windows.Forms.StatusStrip();
      this.toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
      this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
      this.removeButton = new System.Windows.Forms.Button();
      this.installButton = new System.Windows.Forms.Button();
      this.tabControl = new System.Windows.Forms.TabControl();
      this.localPluginsTabPage = new System.Windows.Forms.TabPage();
      this.localPluginManager = new HeuristicLab.PluginInfrastructure.Advanced.LocalPluginManager();
      this.remotePluginsTabPage = new System.Windows.Forms.TabPage();
      this.remotePluginInstaller = new HeuristicLab.PluginInfrastructure.Advanced.RemotePluginInstaller();
      this.logTabPage = new System.Windows.Forms.TabPage();
      this.logTextBox = new System.Windows.Forms.TextBox();
      this.refreshButton = new System.Windows.Forms.Button();
      this.editConnectionButton = new System.Windows.Forms.Button();
      this.statusStrip.SuspendLayout();
      this.tabControl.SuspendLayout();
      this.localPluginsTabPage.SuspendLayout();
      this.remotePluginsTabPage.SuspendLayout();
      this.logTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // statusStrip
      // 
      this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar,
            this.toolStripStatusLabel});
      this.statusStrip.Location = new System.Drawing.Point(0, 612);
      this.statusStrip.Name = "statusStrip";
      this.statusStrip.Size = new System.Drawing.Size(606, 22);
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
      this.removeButton.Location = new System.Drawing.Point(6, 557);
      this.removeButton.Name = "removeButton";
      this.removeButton.Size = new System.Drawing.Size(109, 23);
      this.removeButton.TabIndex = 11;
      this.removeButton.Text = "Remove Plugins";
      this.removeButton.UseVisualStyleBackColor = true;
      this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
      // 
      // installButton
      // 
      this.installButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.installButton.Enabled = false;
      this.installButton.Location = new System.Drawing.Point(6, 557);
      this.installButton.Name = "installButton";
      this.installButton.Size = new System.Drawing.Size(132, 23);
      this.installButton.TabIndex = 15;
      this.installButton.Text = "Download and Install";
      this.installButton.UseVisualStyleBackColor = true;
      this.installButton.Click += new System.EventHandler(this.updateButton_Click);
      // 
      // tabControl
      // 
      this.tabControl.Controls.Add(this.localPluginsTabPage);
      this.tabControl.Controls.Add(this.remotePluginsTabPage);
      this.tabControl.Controls.Add(this.logTabPage);
      this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabControl.Location = new System.Drawing.Point(0, 0);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(606, 612);
      this.tabControl.TabIndex = 16;
      // 
      // localPluginsTabPage
      // 
      this.localPluginsTabPage.Controls.Add(this.removeButton);
      this.localPluginsTabPage.Controls.Add(this.localPluginManager);
      this.localPluginsTabPage.Location = new System.Drawing.Point(4, 22);
      this.localPluginsTabPage.Name = "localPluginsTabPage";
      this.localPluginsTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.localPluginsTabPage.Size = new System.Drawing.Size(598, 586);
      this.localPluginsTabPage.TabIndex = 0;
      this.localPluginsTabPage.Text = "Installed Plugins";
      this.localPluginsTabPage.UseVisualStyleBackColor = true;
      // 
      // localPluginManager
      // 
      this.localPluginManager.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.localPluginManager.Location = new System.Drawing.Point(6, 6);
      this.localPluginManager.Name = "localPluginManager";
      this.localPluginManager.Plugins = null;
      this.localPluginManager.Size = new System.Drawing.Size(584, 545);
      this.localPluginManager.TabIndex = 0;
      this.localPluginManager.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.localPluginManager_ItemChecked);
      // 
      // remotePluginsTabPage
      // 
      this.remotePluginsTabPage.Controls.Add(this.editConnectionButton);
      this.remotePluginsTabPage.Controls.Add(this.remotePluginInstaller);
      this.remotePluginsTabPage.Controls.Add(this.refreshButton);
      this.remotePluginsTabPage.Controls.Add(this.installButton);
      this.remotePluginsTabPage.Location = new System.Drawing.Point(4, 22);
      this.remotePluginsTabPage.Name = "remotePluginsTabPage";
      this.remotePluginsTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.remotePluginsTabPage.Size = new System.Drawing.Size(598, 586);
      this.remotePluginsTabPage.TabIndex = 1;
      this.remotePluginsTabPage.Text = "Remote Plugins";
      this.remotePluginsTabPage.UseVisualStyleBackColor = true;
      // 
      // remotePluginInstaller
      // 
      this.remotePluginInstaller.AllPlugins = new HeuristicLab.PluginInfrastructure.IPluginDescription[0];
      this.remotePluginInstaller.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.remotePluginInstaller.Location = new System.Drawing.Point(6, 35);
      this.remotePluginInstaller.Name = "remotePluginInstaller";
      this.remotePluginInstaller.NewPlugins = new HeuristicLab.PluginInfrastructure.IPluginDescription[0];
      this.remotePluginInstaller.Products = new HeuristicLab.PluginInfrastructure.Advanced.DeploymentService.ProductDescription[0];
      this.remotePluginInstaller.Size = new System.Drawing.Size(584, 516);
      this.remotePluginInstaller.TabIndex = 14;
      this.remotePluginInstaller.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.remotePluginInstaller_ItemChecked);
      // 
      // logTabPage
      // 
      this.logTabPage.Controls.Add(this.logTextBox);
      this.logTabPage.Location = new System.Drawing.Point(4, 22);
      this.logTabPage.Name = "logTabPage";
      this.logTabPage.Size = new System.Drawing.Size(598, 586);
      this.logTabPage.TabIndex = 2;
      this.logTabPage.Text = "Log";
      this.logTabPage.UseVisualStyleBackColor = true;
      // 
      // logTextBox
      // 
      this.logTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.logTextBox.Location = new System.Drawing.Point(0, 0);
      this.logTextBox.Multiline = true;
      this.logTextBox.Name = "logTextBox";
      this.logTextBox.ReadOnly = true;
      this.logTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.logTextBox.Size = new System.Drawing.Size(598, 586);
      this.logTextBox.TabIndex = 0;
      // 
      // refreshButton
      // 
      this.refreshButton.Location = new System.Drawing.Point(8, 6);
      this.refreshButton.Name = "refreshButton";
      this.refreshButton.Size = new System.Drawing.Size(75, 23);
      this.refreshButton.TabIndex = 11;
      this.refreshButton.Text = "Refresh";
      this.refreshButton.UseVisualStyleBackColor = true;
      this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
      // 
      // editConnectionButton
      // 
      this.editConnectionButton.Location = new System.Drawing.Point(89, 6);
      this.editConnectionButton.Name = "editConnectionButton";
      this.editConnectionButton.Size = new System.Drawing.Size(108, 23);
      this.editConnectionButton.TabIndex = 16;
      this.editConnectionButton.Text = "Edit Connection...";
      this.editConnectionButton.UseVisualStyleBackColor = true;
      this.editConnectionButton.Click += new System.EventHandler(this.editConnectionButton_Click);
      // 
      // InstallationManagerForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(606, 634);
      this.Controls.Add(this.tabControl);
      this.Controls.Add(this.statusStrip);
      this.Name = "InstallationManagerForm";
      this.Text = "InstallationManager";
      this.statusStrip.ResumeLayout(false);
      this.statusStrip.PerformLayout();
      this.tabControl.ResumeLayout(false);
      this.localPluginsTabPage.ResumeLayout(false);
      this.remotePluginsTabPage.ResumeLayout(false);
      this.logTabPage.ResumeLayout(false);
      this.logTabPage.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.StatusStrip statusStrip;
    private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar;
    private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
    private LocalPluginManager localPluginManager;
    private RemotePluginInstaller remotePluginInstaller;
    private System.Windows.Forms.Button removeButton;
    private System.Windows.Forms.Button installButton;
    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TabPage localPluginsTabPage;
    private System.Windows.Forms.TabPage remotePluginsTabPage;
    private System.Windows.Forms.TabPage logTabPage;
    private System.Windows.Forms.TextBox logTextBox;
    private System.Windows.Forms.Button editConnectionButton;
    private System.Windows.Forms.Button refreshButton;
  }
}