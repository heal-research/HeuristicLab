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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InstallationManagerForm));
      statusStrip = new System.Windows.Forms.StatusStrip();
      toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
      toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
      localPluginsView = new InstalledPluginsView();
      statusStrip.SuspendLayout();
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
      // localPluginsView
      // 
      localPluginsView.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
      localPluginsView.Location = new System.Drawing.Point(12, 12);
      localPluginsView.Name = "localPluginsView";
      localPluginsView.PluginManager = null;
      localPluginsView.Size = new System.Drawing.Size(598, 407);
      localPluginsView.StatusView = null;
      localPluginsView.TabIndex = 0;
      // 
      // InstallationManagerForm
      // 
      AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      ClientSize = new System.Drawing.Size(622, 444);
      Controls.Add(localPluginsView);
      Controls.Add(statusStrip);
      Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
      Name = "InstallationManagerForm";
      Text = "Plugin Manager";
      statusStrip.ResumeLayout(false);
      statusStrip.PerformLayout();
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private System.Windows.Forms.StatusStrip statusStrip;
    private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar;
    private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
    private InstalledPluginsView localPluginsView;
  }
}