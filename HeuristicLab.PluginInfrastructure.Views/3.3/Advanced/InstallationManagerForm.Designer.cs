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
      components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InstallationManagerForm));
      statusStrip = new System.Windows.Forms.StatusStrip();
      toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
      toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
      tabControl = new System.Windows.Forms.TabControl();
      localPluginsTabPage = new System.Windows.Forms.TabPage();
      localPluginsView = new InstalledPluginsView();
      logTabPage = new System.Windows.Forms.TabPage();
      logTextBox = new System.Windows.Forms.TextBox();
      menuStrip = new System.Windows.Forms.MenuStrip();
      optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      connectionSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      toolTip = new System.Windows.Forms.ToolTip(components);
      statusStrip.SuspendLayout();
      tabControl.SuspendLayout();
      localPluginsTabPage.SuspendLayout();
      logTabPage.SuspendLayout();
      menuStrip.SuspendLayout();
      SuspendLayout();
      // 
      // statusStrip
      // 
      statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripProgressBar, toolStripStatusLabel });
      statusStrip.Location = new System.Drawing.Point(0, 422);
      statusStrip.Name = "statusStrip";
      statusStrip.Size = new System.Drawing.Size(622, 22);
      statusStrip.TabIndex = 0;
      // 
      // toolStripProgressBar
      // 
      toolStripProgressBar.MarqueeAnimationSpeed = 30;
      toolStripProgressBar.Name = "toolStripProgressBar";
      toolStripProgressBar.Size = new System.Drawing.Size(100, 16);
      toolStripProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
      toolStripProgressBar.Visible = false;
      // 
      // toolStripStatusLabel
      // 
      toolStripStatusLabel.Name = "toolStripStatusLabel";
      toolStripStatusLabel.Size = new System.Drawing.Size(0, 17);
      // 
      // tabControl
      // 
      tabControl.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
      tabControl.Controls.Add(localPluginsTabPage);
      tabControl.Controls.Add(logTabPage);
      tabControl.Location = new System.Drawing.Point(12, 27);
      tabControl.Name = "tabControl";
      tabControl.SelectedIndex = 0;
      tabControl.Size = new System.Drawing.Size(598, 392);
      tabControl.TabIndex = 16;
      tabControl.SelectedIndexChanged += tabControl_SelectedIndexChanged;
      // 
      // localPluginsTabPage
      // 
      localPluginsTabPage.Controls.Add(localPluginsView);
      localPluginsTabPage.Location = new System.Drawing.Point(4, 24);
      localPluginsTabPage.Name = "localPluginsTabPage";
      localPluginsTabPage.Padding = new System.Windows.Forms.Padding(3);
      localPluginsTabPage.Size = new System.Drawing.Size(590, 364);
      localPluginsTabPage.TabIndex = 0;
      localPluginsTabPage.Text = "Installed Plugins";
      toolTip.SetToolTip(localPluginsTabPage, "Delete or update installed plugins");
      localPluginsTabPage.UseVisualStyleBackColor = true;
      // 
      // localPluginsView
      // 
      localPluginsView.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
      localPluginsView.Location = new System.Drawing.Point(6, 6);
      localPluginsView.Name = "localPluginsView";
      localPluginsView.PluginManager = null;
      localPluginsView.Size = new System.Drawing.Size(578, 354);
      localPluginsView.StatusView = null;
      localPluginsView.TabIndex = 0;
      // 
      // logTabPage
      // 
      logTabPage.Controls.Add(logTextBox);
      logTabPage.Location = new System.Drawing.Point(4, 24);
      logTabPage.Name = "logTabPage";
      logTabPage.Size = new System.Drawing.Size(590, 364);
      logTabPage.TabIndex = 2;
      logTabPage.Text = "Log";
      toolTip.SetToolTip(logTabPage, "Show Log Messages");
      logTabPage.UseVisualStyleBackColor = true;
      // 
      // logTextBox
      // 
      logTextBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
      logTextBox.Location = new System.Drawing.Point(3, 3);
      logTextBox.Multiline = true;
      logTextBox.Name = "logTextBox";
      logTextBox.ReadOnly = true;
      logTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      logTextBox.Size = new System.Drawing.Size(584, 360);
      logTextBox.TabIndex = 0;
      // 
      // menuStrip
      // 
      menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { optionsToolStripMenuItem });
      menuStrip.Location = new System.Drawing.Point(0, 0);
      menuStrip.Name = "menuStrip";
      menuStrip.Size = new System.Drawing.Size(622, 24);
      menuStrip.TabIndex = 17;
      menuStrip.Text = "menuStrip1";
      // 
      // optionsToolStripMenuItem
      // 
      optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { connectionSettingsToolStripMenuItem });
      optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
      optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
      optionsToolStripMenuItem.Text = "Options";
      // 
      // connectionSettingsToolStripMenuItem
      // 
      connectionSettingsToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("connectionSettingsToolStripMenuItem.Image");
      connectionSettingsToolStripMenuItem.Name = "connectionSettingsToolStripMenuItem";
      connectionSettingsToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
      connectionSettingsToolStripMenuItem.Text = "Connection Settings...";
      connectionSettingsToolStripMenuItem.Click += connectionSettingsToolStripMenuItem_Click;
      // 
      // InstallationManagerForm
      // 
      AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      ClientSize = new System.Drawing.Size(622, 444);
      Controls.Add(tabControl);
      Controls.Add(statusStrip);
      Controls.Add(menuStrip);
      Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
      MainMenuStrip = menuStrip;
      Name = "InstallationManagerForm";
      Text = "Plugin Manager";
      statusStrip.ResumeLayout(false);
      statusStrip.PerformLayout();
      tabControl.ResumeLayout(false);
      localPluginsTabPage.ResumeLayout(false);
      logTabPage.ResumeLayout(false);
      logTabPage.PerformLayout();
      menuStrip.ResumeLayout(false);
      menuStrip.PerformLayout();
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private System.Windows.Forms.StatusStrip statusStrip;
    private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar;
    private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TabPage logTabPage;
    private System.Windows.Forms.TextBox logTextBox;
    private System.Windows.Forms.MenuStrip menuStrip;
    private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem connectionSettingsToolStripMenuItem;
    private System.Windows.Forms.ToolTip toolTip;
    private System.Windows.Forms.TabPage localPluginsTabPage;
    private InstalledPluginsView localPluginsView;
  }
}