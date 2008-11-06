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

namespace HeuristicLab.Hive.Client.Console {
  partial class HiveClientConsole {
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
      this.msClientConsole = new System.Windows.Forms.MenuStrip();
      this.tsmiConsole = new System.Windows.Forms.ToolStripMenuItem();
      this.tsmiExit = new System.Windows.Forms.ToolStripMenuItem();
      this.tcClientConsole = new System.Windows.Forms.TabControl();
      this.tpConnection = new System.Windows.Forms.TabPage();
      this.btnDisconnect = new System.Windows.Forms.Button();
      this.btnConnect = new System.Windows.Forms.Button();
      this.gbUser = new System.Windows.Forms.GroupBox();
      this.tbUuid = new System.Windows.Forms.TextBox();
      this.lblUuid = new System.Windows.Forms.Label();
      this.rtbInfoClient = new System.Windows.Forms.RichTextBox();
      this.gbManager = new System.Windows.Forms.GroupBox();
      this.lblPort = new System.Windows.Forms.Label();
      this.lblIp = new System.Windows.Forms.Label();
      this.tbPort = new System.Windows.Forms.TextBox();
      this.tbIp = new IPAddressTextBox();
      this.tabPage2 = new System.Windows.Forms.TabPage();
      this.msClientConsole.SuspendLayout();
      this.tcClientConsole.SuspendLayout();
      this.tpConnection.SuspendLayout();
      this.gbUser.SuspendLayout();
      this.gbManager.SuspendLayout();
      this.SuspendLayout();
      // 
      // msClientConsole
      // 
      this.msClientConsole.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiConsole});
      this.msClientConsole.Location = new System.Drawing.Point(0, 0);
      this.msClientConsole.Name = "msClientConsole";
      this.msClientConsole.Size = new System.Drawing.Size(434, 24);
      this.msClientConsole.TabIndex = 0;
      this.msClientConsole.Text = "menuStrip1";
      // 
      // tsmiConsole
      // 
      this.tsmiConsole.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiExit});
      this.tsmiConsole.Name = "tsmiConsole";
      this.tsmiConsole.Size = new System.Drawing.Size(82, 20);
      this.tsmiConsole.Text = "Clientconsole";
      // 
      // tsmiExit
      // 
      this.tsmiExit.Name = "tsmiExit";
      this.tsmiExit.Size = new System.Drawing.Size(103, 22);
      this.tsmiExit.Text = "Exit";
      this.tsmiExit.Click += new System.EventHandler(this.tsmiExit_Click);
      // 
      // tcClientConsole
      // 
      this.tcClientConsole.Controls.Add(this.tpConnection);
      this.tcClientConsole.Controls.Add(this.tabPage2);
      this.tcClientConsole.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tcClientConsole.Location = new System.Drawing.Point(0, 24);
      this.tcClientConsole.Name = "tcClientConsole";
      this.tcClientConsole.SelectedIndex = 0;
      this.tcClientConsole.Size = new System.Drawing.Size(434, 481);
      this.tcClientConsole.TabIndex = 1;
      // 
      // tpConnection
      // 
      this.tpConnection.Controls.Add(this.btnDisconnect);
      this.tpConnection.Controls.Add(this.btnConnect);
      this.tpConnection.Controls.Add(this.gbUser);
      this.tpConnection.Controls.Add(this.rtbInfoClient);
      this.tpConnection.Controls.Add(this.gbManager);
      this.tpConnection.Location = new System.Drawing.Point(4, 22);
      this.tpConnection.Name = "tpConnection";
      this.tpConnection.Padding = new System.Windows.Forms.Padding(3);
      this.tpConnection.RightToLeft = System.Windows.Forms.RightToLeft.No;
      this.tpConnection.Size = new System.Drawing.Size(426, 455);
      this.tpConnection.TabIndex = 0;
      this.tpConnection.Text = "Connection";
      this.tpConnection.UseVisualStyleBackColor = true;
      // 
      // btnDisconnect
      // 
      this.btnDisconnect.Enabled = false;
      this.btnDisconnect.Location = new System.Drawing.Point(213, 182);
      this.btnDisconnect.Name = "btnDisconnect";
      this.btnDisconnect.Size = new System.Drawing.Size(134, 23);
      this.btnDisconnect.TabIndex = 4;
      this.btnDisconnect.Text = "Disconnect";
      this.btnDisconnect.UseVisualStyleBackColor = true;
      this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
      // 
      // btnConnect
      // 
      this.btnConnect.Location = new System.Drawing.Point(66, 182);
      this.btnConnect.Name = "btnConnect";
      this.btnConnect.Size = new System.Drawing.Size(131, 23);
      this.btnConnect.TabIndex = 3;
      this.btnConnect.Text = "Connect";
      this.btnConnect.UseVisualStyleBackColor = true;
      this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
      // 
      // gbUser
      // 
      this.gbUser.Controls.Add(this.tbUuid);
      this.gbUser.Controls.Add(this.lblUuid);
      this.gbUser.Location = new System.Drawing.Point(67, 117);
      this.gbUser.Name = "gbUser";
      this.gbUser.Size = new System.Drawing.Size(281, 51);
      this.gbUser.TabIndex = 2;
      this.gbUser.TabStop = false;
      this.gbUser.Text = "User";
      // 
      // tbUuid
      // 
      this.tbUuid.Location = new System.Drawing.Point(56, 22);
      this.tbUuid.Name = "tbUuid";
      this.tbUuid.Size = new System.Drawing.Size(209, 20);
      this.tbUuid.TabIndex = 2;
      // 
      // lblUuid
      // 
      this.lblUuid.AutoSize = true;
      this.lblUuid.Location = new System.Drawing.Point(15, 25);
      this.lblUuid.Name = "lblUuid";
      this.lblUuid.Size = new System.Drawing.Size(34, 13);
      this.lblUuid.TabIndex = 0;
      this.lblUuid.Text = "UUID";
      // 
      // rtbInfoClient
      // 
      this.rtbInfoClient.Location = new System.Drawing.Point(18, 218);
      this.rtbInfoClient.Name = "rtbInfoClient";
      this.rtbInfoClient.Size = new System.Drawing.Size(387, 229);
      this.rtbInfoClient.TabIndex = 1;
      this.rtbInfoClient.Text = "";
      // 
      // gbManager
      // 
      this.gbManager.Controls.Add(this.lblPort);
      this.gbManager.Controls.Add(this.lblIp);
      this.gbManager.Controls.Add(this.tbPort);
      this.gbManager.Controls.Add(this.tbIp);
      this.gbManager.Location = new System.Drawing.Point(66, 19);
      this.gbManager.Name = "gbManager";
      this.gbManager.Size = new System.Drawing.Size(283, 82);
      this.gbManager.TabIndex = 0;
      this.gbManager.TabStop = false;
      this.gbManager.Text = "Manager";
      // 
      // lblPort
      // 
      this.lblPort.AutoSize = true;
      this.lblPort.Location = new System.Drawing.Point(16, 54);
      this.lblPort.Name = "lblPort";
      this.lblPort.Size = new System.Drawing.Size(26, 13);
      this.lblPort.TabIndex = 3;
      this.lblPort.Text = "Port";
      // 
      // lblIp
      // 
      this.lblIp.AutoSize = true;
      this.lblIp.Location = new System.Drawing.Point(16, 26);
      this.lblIp.Name = "lblIp";
      this.lblIp.Size = new System.Drawing.Size(17, 13);
      this.lblIp.TabIndex = 2;
      this.lblIp.Text = "IP";
      // 
      // tbPort
      // 
      this.tbPort.Location = new System.Drawing.Point(57, 47);
      this.tbPort.Name = "tbPort";
      this.tbPort.Size = new System.Drawing.Size(209, 20);
      this.tbPort.TabIndex = 1;
      // 
      // tbIp
      // 
      this.tbIp.Location = new System.Drawing.Point(57, 19);
      this.tbIp.Name = "tbIp";
      this.tbIp.Size = new System.Drawing.Size(211, 20);
      this.tbIp.TabIndex = 0;
      // 
      // tabPage2
      // 
      this.tabPage2.Location = new System.Drawing.Point(4, 22);
      this.tabPage2.Name = "tabPage2";
      this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage2.Size = new System.Drawing.Size(426, 455);
      this.tabPage2.TabIndex = 1;
      this.tabPage2.Text = "tabPage2";
      this.tabPage2.UseVisualStyleBackColor = true;
      // 
      // HiveClientConsole
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(434, 505);
      this.Controls.Add(this.tcClientConsole);
      this.Controls.Add(this.msClientConsole);
      this.MainMenuStrip = this.msClientConsole;
      this.Name = "HiveClientConsole";
      this.Text = "Client Console";
      this.msClientConsole.ResumeLayout(false);
      this.msClientConsole.PerformLayout();
      this.tcClientConsole.ResumeLayout(false);
      this.tpConnection.ResumeLayout(false);
      this.gbUser.ResumeLayout(false);
      this.gbUser.PerformLayout();
      this.gbManager.ResumeLayout(false);
      this.gbManager.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.MenuStrip msClientConsole;
    private System.Windows.Forms.ToolStripMenuItem tsmiConsole;
    private System.Windows.Forms.TabControl tcClientConsole;
    private System.Windows.Forms.TabPage tpConnection;
    private System.Windows.Forms.TabPage tabPage2;
    private System.Windows.Forms.RichTextBox rtbInfoClient;
    private System.Windows.Forms.GroupBox gbManager;
    private System.Windows.Forms.TextBox tbPort;
    private IPAddressTextBox tbIp;
    private System.Windows.Forms.Label lblIp;
    private System.Windows.Forms.GroupBox gbUser;
    private System.Windows.Forms.Label lblUuid;
    private System.Windows.Forms.Label lblPort;
    private System.Windows.Forms.Button btnDisconnect;
    private System.Windows.Forms.Button btnConnect;
    private System.Windows.Forms.TextBox tbUuid;
    private System.Windows.Forms.ToolStripMenuItem tsmiExit;
  }
}

