#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Clients.Hive.SlaveCore.Views {
  partial class SlaveMainViewBase {
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
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SlaveMainViewBase));
      this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
      this.contextMenuNotifyIcon = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.homepageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.mainTabControl = new System.Windows.Forms.TabControl();
      this.tabSlaveView = new System.Windows.Forms.TabPage();
      this.tabLog = new System.Windows.Forms.TabPage();
      this.btnAbout = new System.Windows.Forms.Button();
      this.slaveView = new HeuristicLab.Clients.Hive.SlaveCore.Views.SlaveView();
      this.logView = new HeuristicLab.Clients.Hive.SlaveCore.Views.LogView();
      this.contextMenuNotifyIcon.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.mainTabControl.SuspendLayout();
      this.tabSlaveView.SuspendLayout();
      this.tabLog.SuspendLayout();
      this.SuspendLayout();
      // 
      // closeToolStripMenuItem
      // 
      this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
      this.closeToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
      this.closeToolStripMenuItem.Text = "Close";
      this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
      // 
      // notifyIcon
      // 
      this.notifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
      this.notifyIcon.BalloonTipText = "A client for computing jobs sent from HeuristicLab Hive";
      this.notifyIcon.BalloonTipTitle = "HeuristicLab Hive Slave";
      this.notifyIcon.ContextMenuStrip = this.contextMenuNotifyIcon;
      this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
      this.notifyIcon.Text = "HeuristicLab Hive Slave";
      this.notifyIcon.Visible = true;
      this.notifyIcon.BalloonTipClicked += new System.EventHandler(this.notifyIcon_BalloonTipClicked);
      this.notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_Click);
      // 
      // contextMenuNotifyIcon
      // 
      this.contextMenuNotifyIcon.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.homepageToolStripMenuItem,
            this.toolStripSeparator,
            this.closeToolStripMenuItem});
      this.contextMenuNotifyIcon.Name = "contextMenuNotifyIcon";
      this.contextMenuNotifyIcon.Size = new System.Drawing.Size(210, 54);
      // 
      // homepageToolStripMenuItem
      // 
      this.homepageToolStripMenuItem.ForeColor = System.Drawing.Color.Blue;
      this.homepageToolStripMenuItem.Name = "homepageToolStripMenuItem";
      this.homepageToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
      this.homepageToolStripMenuItem.Text = "Visit dev.heuristiclab.com";
      this.homepageToolStripMenuItem.Click += new System.EventHandler(this.homepageToolStripMenuItem_Click);
      // 
      // toolStripSeparator
      // 
      this.toolStripSeparator.Name = "toolStripSeparator";
      this.toolStripSeparator.Size = new System.Drawing.Size(206, 6);
      // 
      // splitContainer
      // 
      this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.splitContainer.Location = new System.Drawing.Point(3, 3);
      this.splitContainer.Name = "splitContainer";
      this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.mainTabControl);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.btnAbout);
      this.splitContainer.Size = new System.Drawing.Size(647, 438);
      this.splitContainer.SplitterDistance = 392;
      this.splitContainer.TabIndex = 3;
      // 
      // mainTabControl
      // 
      this.mainTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.mainTabControl.Controls.Add(this.tabSlaveView);
      this.mainTabControl.Controls.Add(this.tabLog);
      this.mainTabControl.Location = new System.Drawing.Point(1, 1);
      this.mainTabControl.Margin = new System.Windows.Forms.Padding(1);
      this.mainTabControl.Name = "mainTabControl";
      this.mainTabControl.SelectedIndex = 0;
      this.mainTabControl.Size = new System.Drawing.Size(645, 390);
      this.mainTabControl.TabIndex = 2;
      // 
      // tabSlaveView
      // 
      this.tabSlaveView.Controls.Add(this.slaveView);
      this.tabSlaveView.Location = new System.Drawing.Point(4, 22);
      this.tabSlaveView.Name = "tabSlaveView";
      this.tabSlaveView.Padding = new System.Windows.Forms.Padding(3);
      this.tabSlaveView.Size = new System.Drawing.Size(637, 364);
      this.tabSlaveView.TabIndex = 0;
      this.tabSlaveView.Text = "Summary";
      this.tabSlaveView.UseVisualStyleBackColor = true;
      // 
      // tabLog
      // 
      this.tabLog.Controls.Add(this.logView);
      this.tabLog.Location = new System.Drawing.Point(4, 22);
      this.tabLog.Name = "tabLog";
      this.tabLog.Padding = new System.Windows.Forms.Padding(3);
      this.tabLog.Size = new System.Drawing.Size(637, 364);
      this.tabLog.TabIndex = 1;
      this.tabLog.Text = "Log";
      this.tabLog.UseVisualStyleBackColor = true;
      // 
      // btnAbout
      // 
      this.btnAbout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnAbout.Image = ((System.Drawing.Image)(resources.GetObject("btnAbout.Image")));
      this.btnAbout.Location = new System.Drawing.Point(618, 15);
      this.btnAbout.Name = "btnAbout";
      this.btnAbout.Size = new System.Drawing.Size(24, 24);
      this.btnAbout.TabIndex = 0;
      this.btnAbout.UseVisualStyleBackColor = true;
      this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
      // 
      // slaveView
      // 
      this.slaveView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.slaveView.Caption = "HeuristicLab Slave View";
      this.slaveView.Content = null;
      this.slaveView.Location = new System.Drawing.Point(6, 6);
      this.slaveView.Name = "slaveView";
      this.slaveView.ReadOnly = false;
      this.slaveView.Size = new System.Drawing.Size(625, 352);
      this.slaveView.TabIndex = 0;
      // 
      // logView
      // 
      this.logView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.logView.Caption = "LogView: Displays logged messages from the slave core.";
      this.logView.Content = null;
      this.logView.Location = new System.Drawing.Point(7, 7);
      this.logView.Name = "logView";
      this.logView.ReadOnly = false;
      this.logView.Size = new System.Drawing.Size(624, 351);
      this.logView.TabIndex = 0;
      // 
      // SlaveMainViewBase
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.splitContainer);
      this.Name = "SlaveMainViewBase";
      this.Size = new System.Drawing.Size(653, 444);
      this.contextMenuNotifyIcon.ResumeLayout(false);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.mainTabControl.ResumeLayout(false);
      this.tabSlaveView.ResumeLayout(false);
      this.tabLog.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TabPage tabLog;
    private LogView logView;
    private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
    private System.Windows.Forms.NotifyIcon notifyIcon;
    private System.Windows.Forms.ContextMenuStrip contextMenuNotifyIcon;
    private System.Windows.Forms.ToolStripMenuItem homepageToolStripMenuItem;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
    protected System.Windows.Forms.TabPage tabSlaveView;
    protected System.Windows.Forms.TabControl mainTabControl;
    protected SlaveView slaveView;
    protected System.Windows.Forms.SplitContainer splitContainer;
    private System.Windows.Forms.Button btnAbout;


  }
}
