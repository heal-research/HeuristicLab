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

namespace HeuristicLab.Hive.Server.ServerConsole {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HiveServerConsole));
      this.gbConfiguration = new System.Windows.Forms.GroupBox();
      this.lblPort = new System.Windows.Forms.Label();
      this.lblIp = new System.Windows.Forms.Label();
      this.tbPort = new System.Windows.Forms.TextBox();
      this.tbIp = new System.Windows.Forms.TextBox();
      this.btnLogin = new System.Windows.Forms.Button();
      this.gpUser = new System.Windows.Forms.GroupBox();
      this.tbUserName = new System.Windows.Forms.TextBox();
      this.lblPwd = new System.Windows.Forms.Label();
      this.lblUsername = new System.Windows.Forms.Label();
      this.tbPwd = new System.Windows.Forms.TextBox();
      this.lblError = new System.Windows.Forms.Label();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.btnCancel = new System.Windows.Forms.Button();
      this.gbConfiguration.SuspendLayout();
      this.gpUser.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.SuspendLayout();
      // 
      // gbConfiguration
      // 
      this.gbConfiguration.Controls.Add(this.lblPort);
      this.gbConfiguration.Controls.Add(this.lblIp);
      this.gbConfiguration.Controls.Add(this.tbPort);
      this.gbConfiguration.Controls.Add(this.tbIp);
      this.gbConfiguration.Location = new System.Drawing.Point(188, 100);
      this.gbConfiguration.Name = "gbConfiguration";
      this.gbConfiguration.Size = new System.Drawing.Size(283, 82);
      this.gbConfiguration.TabIndex = 4;
      this.gbConfiguration.TabStop = false;
      this.gbConfiguration.Text = "Configuration";
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
      this.tbPort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.HiveServerConsole_KeyPress);
      // 
      // tbIp
      // 
      this.tbIp.Location = new System.Drawing.Point(77, 19);
      this.tbIp.Name = "tbIp";
      this.tbIp.Size = new System.Drawing.Size(191, 20);
      this.tbIp.TabIndex = 5;
      this.tbIp.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.HiveServerConsole_KeyPress);
      // 
      // btnLogin
      // 
      this.btnLogin.Location = new System.Drawing.Point(355, 193);
      this.btnLogin.Name = "btnLogin";
      this.btnLogin.Size = new System.Drawing.Size(116, 23);
      this.btnLogin.TabIndex = 7;
      this.btnLogin.Text = "Login";
      this.btnLogin.UseVisualStyleBackColor = true;
      this.btnLogin.Click += new System.EventHandler(this.BtnLogin_Click);
      this.btnLogin.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.HiveServerConsole_KeyPress);
      // 
      // gpUser
      // 
      this.gpUser.Controls.Add(this.tbUserName);
      this.gpUser.Controls.Add(this.lblPwd);
      this.gpUser.Controls.Add(this.lblUsername);
      this.gpUser.Controls.Add(this.tbPwd);
      this.gpUser.Location = new System.Drawing.Point(188, 12);
      this.gpUser.Name = "gpUser";
      this.gpUser.Size = new System.Drawing.Size(283, 82);
      this.gpUser.TabIndex = 1;
      this.gpUser.TabStop = false;
      this.gpUser.Text = "User";
      // 
      // tbUserName
      // 
      this.tbUserName.Location = new System.Drawing.Point(77, 19);
      this.tbUserName.Name = "tbUserName";
      this.tbUserName.Size = new System.Drawing.Size(189, 20);
      this.tbUserName.TabIndex = 2;
      this.tbUserName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.HiveServerConsole_KeyPress);
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
      this.tbPwd.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.HiveServerConsole_KeyPress);
      // 
      // lblError
      // 
      this.lblError.AutoSize = true;
      this.lblError.ForeColor = System.Drawing.Color.Red;
      this.lblError.Location = new System.Drawing.Point(12, 193);
      this.lblError.Name = "lblError";
      this.lblError.Size = new System.Drawing.Size(0, 13);
      this.lblError.TabIndex = 5;
      // 
      // pictureBox1
      // 
      this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
      this.pictureBox1.Location = new System.Drawing.Point(12, 12);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(170, 170);
      this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
      this.pictureBox1.TabIndex = 8;
      this.pictureBox1.TabStop = false;
      // 
      // btnCancel
      // 
      this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnCancel.Location = new System.Drawing.Point(259, 193);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(90, 23);
      this.btnCancel.TabIndex = 9;
      this.btnCancel.Text = "&Cancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
      // 
      // HiveServerConsole
      // 
      this.AcceptButton = this.btnLogin;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.Control;
      this.CancelButton = this.btnCancel;
      this.ClientSize = new System.Drawing.Size(482, 226);
      this.Controls.Add(this.btnCancel);
      this.Controls.Add(this.pictureBox1);
      this.Controls.Add(this.lblError);
      this.Controls.Add(this.gpUser);
      this.Controls.Add(this.btnLogin);
      this.Controls.Add(this.gbConfiguration);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.Name = "HiveServerConsole";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Server Console Logon";
      this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.HiveServerConsole_KeyPress);
      this.gbConfiguration.ResumeLayout(false);
      this.gbConfiguration.PerformLayout();
      this.gpUser.ResumeLayout(false);
      this.gpUser.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.GroupBox gbConfiguration;
    private System.Windows.Forms.Label lblPort;
    private System.Windows.Forms.Label lblIp;
    private System.Windows.Forms.TextBox tbPort;
    private System.Windows.Forms.TextBox tbIp;
    private System.Windows.Forms.Button btnLogin;
    private System.Windows.Forms.GroupBox gpUser;
    private System.Windows.Forms.TextBox tbUserName;
    private System.Windows.Forms.Label lblPwd;
    private System.Windows.Forms.Label lblUsername;
    private System.Windows.Forms.TextBox tbPwd;
    private System.Windows.Forms.Label lblError;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.Button btnCancel;
  }
}

