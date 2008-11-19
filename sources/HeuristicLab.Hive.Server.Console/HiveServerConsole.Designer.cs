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

namespace HeuristicLab.Hive.Server.Console {
  partial class HiveServerConsole {
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
      this.msServerConsole = new System.Windows.Forms.MenuStrip();
      this.tsmiConsole = new System.Windows.Forms.ToolStripMenuItem();
      this.tsmiExit = new System.Windows.Forms.ToolStripMenuItem();
      this.gbManager = new System.Windows.Forms.GroupBox();
      this.lblPort = new System.Windows.Forms.Label();
      this.lblIp = new System.Windows.Forms.Label();
      this.tbPort = new System.Windows.Forms.TextBox();
      this.tbIp = new HeuristicLab.Hive.Server.Console.IPAddressTextBox();
      this.btnLogin = new System.Windows.Forms.Button();
      this.msServerConsole.SuspendLayout();
      this.gbManager.SuspendLayout();
      this.SuspendLayout();
      // 
      // msServerConsole
      // 
      this.msServerConsole.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiConsole});
      this.msServerConsole.Location = new System.Drawing.Point(0, 0);
      this.msServerConsole.Name = "msServerConsole";
      this.msServerConsole.Size = new System.Drawing.Size(311, 24);
      this.msServerConsole.TabIndex = 0;
      this.msServerConsole.Text = "menuStrip1";
      // 
      // tsmiConsole
      // 
      this.tsmiConsole.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiExit});
      this.tsmiConsole.Name = "tsmiConsole";
      this.tsmiConsole.Size = new System.Drawing.Size(87, 20);
      this.tsmiConsole.Text = "Serverconsole";
      // 
      // tsmiExit
      // 
      this.tsmiExit.Name = "tsmiExit";
      this.tsmiExit.Size = new System.Drawing.Size(152, 22);
      this.tsmiExit.Text = "Exit";
      this.tsmiExit.Click += new System.EventHandler(this.tsmiExit_Click);
      // 
      // gbManager
      // 
      this.gbManager.Controls.Add(this.lblPort);
      this.gbManager.Controls.Add(this.lblIp);
      this.gbManager.Controls.Add(this.tbPort);
      this.gbManager.Controls.Add(this.tbIp);
      this.gbManager.Location = new System.Drawing.Point(12, 37);
      this.gbManager.Name = "gbManager";
      this.gbManager.Size = new System.Drawing.Size(283, 82);
      this.gbManager.TabIndex = 1;
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
      // btnLogin
      // 
      this.btnLogin.Location = new System.Drawing.Point(179, 127);
      this.btnLogin.Name = "btnLogin";
      this.btnLogin.Size = new System.Drawing.Size(116, 23);
      this.btnLogin.TabIndex = 2;
      this.btnLogin.Text = "Login";
      this.btnLogin.UseVisualStyleBackColor = true;
      this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
      // 
      // HiveServerConsole
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(311, 162);
      this.Controls.Add(this.btnLogin);
      this.Controls.Add(this.gbManager);
      this.Controls.Add(this.msServerConsole);
      this.MainMenuStrip = this.msServerConsole;
      this.Name = "HiveServerConsole";
      this.Text = "Server Console";
      this.msServerConsole.ResumeLayout(false);
      this.msServerConsole.PerformLayout();
      this.gbManager.ResumeLayout(false);
      this.gbManager.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.MenuStrip msServerConsole;
    private System.Windows.Forms.ToolStripMenuItem tsmiConsole;
    private System.Windows.Forms.ToolStripMenuItem tsmiExit;
    private System.Windows.Forms.GroupBox gbManager;
    private System.Windows.Forms.Label lblPort;
    private System.Windows.Forms.Label lblIp;
    private System.Windows.Forms.TextBox tbPort;
    private IPAddressTextBox tbIp;
    private System.Windows.Forms.Button btnLogin;
  }
}

