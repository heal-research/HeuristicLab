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
      this.tbIp = new System.Windows.Forms.TextBox();
      this.btnLogin = new System.Windows.Forms.Button();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.tbUserName = new System.Windows.Forms.TextBox();
      this.lblPwd = new System.Windows.Forms.Label();
      this.lblUsername = new System.Windows.Forms.Label();
      this.tbPwd = new System.Windows.Forms.TextBox();
      this.lblError = new System.Windows.Forms.Label();
      this.msServerConsole.SuspendLayout();
      this.gbManager.SuspendLayout();
      this.groupBox1.SuspendLayout();
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
      this.tsmiExit.Size = new System.Drawing.Size(103, 22);
      this.tsmiExit.Text = "Exit";
      this.tsmiExit.Click += new System.EventHandler(this.tsmiExit_Click);
      // 
      // gbManager
      // 
      this.gbManager.Controls.Add(this.lblPort);
      this.gbManager.Controls.Add(this.lblIp);
      this.gbManager.Controls.Add(this.tbPort);
      this.gbManager.Controls.Add(this.tbIp);
      this.gbManager.Location = new System.Drawing.Point(16, 124);
      this.gbManager.Name = "gbManager";
      this.gbManager.Size = new System.Drawing.Size(283, 82);
      this.gbManager.TabIndex = 4;
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
      this.tbPort.Location = new System.Drawing.Point(77, 47);
      this.tbPort.Name = "tbPort";
      this.tbPort.Size = new System.Drawing.Size(189, 20);
      this.tbPort.TabIndex = 6;
      // 
      // tbIp
      // 
      this.tbIp.Location = new System.Drawing.Point(77, 19);
      this.tbIp.Name = "tbIp";
      this.tbIp.Size = new System.Drawing.Size(191, 20);
      this.tbIp.TabIndex = 5;
      // 
      // btnLogin
      // 
      this.btnLogin.Location = new System.Drawing.Point(183, 212);
      this.btnLogin.Name = "btnLogin";
      this.btnLogin.Size = new System.Drawing.Size(116, 23);
      this.btnLogin.TabIndex = 7;
      this.btnLogin.Text = "Login";
      this.btnLogin.UseVisualStyleBackColor = true;
      this.btnLogin.Click += new System.EventHandler(this.BtnLogin_Click);
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.tbUserName);
      this.groupBox1.Controls.Add(this.lblPwd);
      this.groupBox1.Controls.Add(this.lblUsername);
      this.groupBox1.Controls.Add(this.tbPwd);
      this.groupBox1.Location = new System.Drawing.Point(16, 36);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(283, 82);
      this.groupBox1.TabIndex = 1;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Manager";
      // 
      // tbUserName
      // 
      this.tbUserName.Location = new System.Drawing.Point(77, 19);
      this.tbUserName.Name = "tbUserName";
      this.tbUserName.Size = new System.Drawing.Size(189, 20);
      this.tbUserName.TabIndex = 2;
      // 
      // lblPwd
      // 
      this.lblPwd.AutoSize = true;
      this.lblPwd.Location = new System.Drawing.Point(16, 54);
      this.lblPwd.Name = "lblPwd";
      this.lblPwd.Size = new System.Drawing.Size(53, 13);
      this.lblPwd.TabIndex = 3;
      this.lblPwd.Text = "Password";
      // 
      // lblUsername
      // 
      this.lblUsername.AutoSize = true;
      this.lblUsername.Location = new System.Drawing.Point(16, 26);
      this.lblUsername.Name = "lblUsername";
      this.lblUsername.Size = new System.Drawing.Size(55, 13);
      this.lblUsername.TabIndex = 2;
      this.lblUsername.Text = "Username";
      // 
      // tbPwd
      // 
      this.tbPwd.Location = new System.Drawing.Point(77, 47);
      this.tbPwd.Name = "tbPwd";
      this.tbPwd.Size = new System.Drawing.Size(189, 20);
      this.tbPwd.TabIndex = 3;
      this.tbPwd.UseSystemPasswordChar = true;
      // 
      // lblError
      // 
      this.lblError.AutoSize = true;
      this.lblError.ForeColor = System.Drawing.Color.Red;
      this.lblError.Location = new System.Drawing.Point(16, 238);
      this.lblError.Name = "lblError";
      this.lblError.Size = new System.Drawing.Size(0, 13);
      this.lblError.TabIndex = 5;
      // 
      // HiveServerConsole
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(311, 263);
      this.Controls.Add(this.lblError);
      this.Controls.Add(this.groupBox1);
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
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
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
    private System.Windows.Forms.TextBox tbIp;
    private System.Windows.Forms.Button btnLogin;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.TextBox tbUserName;
    private System.Windows.Forms.Label lblPwd;
    private System.Windows.Forms.Label lblUsername;
    private System.Windows.Forms.TextBox tbPwd;
    private System.Windows.Forms.Label lblError;
  }
}

