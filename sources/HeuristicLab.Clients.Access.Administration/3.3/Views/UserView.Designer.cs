#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Clients.Access.Administration {
  partial class UserView {
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
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.userNameTextBox = new System.Windows.Forms.TextBox();
      this.fullNameTextBox = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.emailTextBox = new System.Windows.Forms.TextBox();
      this.idTextBox = new System.Windows.Forms.TextBox();
      this.label5 = new System.Windows.Forms.Label();
      this.label6 = new System.Windows.Forms.Label();
      this.lastActivityTextBox = new System.Windows.Forms.TextBox();
      this.lastLoginTextBox = new System.Windows.Forms.TextBox();
      this.resetPasswordButton = new System.Windows.Forms.Button();
      this.refreshableRoleSelectionListView = new HeuristicLab.Clients.Access.Administration.RefreshableRoleSelectionListView();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(3, 6);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(63, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "User Name:";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(3, 32);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(57, 13);
      this.label2.TabIndex = 1;
      this.label2.Text = "Full Name:";
      // 
      // userNameTextBox
      // 
      this.userNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.userNameTextBox.Location = new System.Drawing.Point(76, 3);
      this.userNameTextBox.Name = "userNameTextBox";
      this.userNameTextBox.Size = new System.Drawing.Size(746, 20);
      this.userNameTextBox.TabIndex = 2;
      this.userNameTextBox.TextChanged += new System.EventHandler(this.userNameTextBox_TextChanged);
      // 
      // fullNameTextBox
      // 
      this.fullNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.fullNameTextBox.Location = new System.Drawing.Point(76, 29);
      this.fullNameTextBox.Name = "fullNameTextBox";
      this.fullNameTextBox.Size = new System.Drawing.Size(746, 20);
      this.fullNameTextBox.TabIndex = 3;
      this.fullNameTextBox.TextChanged += new System.EventHandler(this.fullNameTextBox_TextChanged);
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(3, 58);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(39, 13);
      this.label3.TabIndex = 4;
      this.label3.Text = "E-Mail:";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(5, 84);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(19, 13);
      this.label4.TabIndex = 5;
      this.label4.Text = "Id:";
      // 
      // emailTextBox
      // 
      this.emailTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.emailTextBox.Location = new System.Drawing.Point(76, 55);
      this.emailTextBox.Name = "emailTextBox";
      this.emailTextBox.Size = new System.Drawing.Size(746, 20);
      this.emailTextBox.TabIndex = 6;
      this.emailTextBox.TextChanged += new System.EventHandler(this.emailTextBox_TextChanged);
      // 
      // idTextBox
      // 
      this.idTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.idTextBox.Enabled = false;
      this.idTextBox.Location = new System.Drawing.Point(76, 81);
      this.idTextBox.Name = "idTextBox";
      this.idTextBox.Size = new System.Drawing.Size(746, 20);
      this.idTextBox.TabIndex = 7;
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(3, 110);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(67, 13);
      this.label5.TabIndex = 8;
      this.label5.Text = "Last Activity:";
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(3, 136);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(59, 13);
      this.label6.TabIndex = 9;
      this.label6.Text = "Last Login:";
      // 
      // lastActivityTextBox
      // 
      this.lastActivityTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.lastActivityTextBox.Enabled = false;
      this.lastActivityTextBox.Location = new System.Drawing.Point(76, 107);
      this.lastActivityTextBox.Name = "lastActivityTextBox";
      this.lastActivityTextBox.Size = new System.Drawing.Size(746, 20);
      this.lastActivityTextBox.TabIndex = 10;
      // 
      // lastLoginTextBox
      // 
      this.lastLoginTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.lastLoginTextBox.Enabled = false;
      this.lastLoginTextBox.Location = new System.Drawing.Point(76, 133);
      this.lastLoginTextBox.Name = "lastLoginTextBox";
      this.lastLoginTextBox.Size = new System.Drawing.Size(746, 20);
      this.lastLoginTextBox.TabIndex = 11;
      // 
      // resetPasswordButton
      // 
      this.resetPasswordButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.resetPasswordButton.Location = new System.Drawing.Point(8, 417);
      this.resetPasswordButton.Name = "resetPasswordButton";
      this.resetPasswordButton.Size = new System.Drawing.Size(96, 23);
      this.resetPasswordButton.TabIndex = 12;
      this.resetPasswordButton.Text = "Reset Password";
      this.resetPasswordButton.UseVisualStyleBackColor = true;
      this.resetPasswordButton.Click += new System.EventHandler(this.resetPasswordButton_Click);
      // 
      // refreshableRoleSelectionListView
      // 
      this.refreshableRoleSelectionListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.refreshableRoleSelectionListView.Caption = "View";
      this.refreshableRoleSelectionListView.Content = null;
      this.refreshableRoleSelectionListView.CurrentUser = null;
      this.refreshableRoleSelectionListView.Location = new System.Drawing.Point(8, 159);
      this.refreshableRoleSelectionListView.Name = "refreshableRoleSelectionListView";
      this.refreshableRoleSelectionListView.ReadOnly = false;
      this.refreshableRoleSelectionListView.Size = new System.Drawing.Size(814, 252);
      this.refreshableRoleSelectionListView.TabIndex = 13;
      // 
      // UserView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.refreshableRoleSelectionListView);
      this.Controls.Add(this.resetPasswordButton);
      this.Controls.Add(this.lastLoginTextBox);
      this.Controls.Add(this.lastActivityTextBox);
      this.Controls.Add(this.label6);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.idTextBox);
      this.Controls.Add(this.emailTextBox);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.fullNameTextBox);
      this.Controls.Add(this.userNameTextBox);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Name = "UserView";
      this.Size = new System.Drawing.Size(825, 447);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox userNameTextBox;
    private System.Windows.Forms.TextBox fullNameTextBox;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.TextBox emailTextBox;
    private System.Windows.Forms.TextBox idTextBox;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.TextBox lastActivityTextBox;
    private System.Windows.Forms.TextBox lastLoginTextBox;
    private System.Windows.Forms.Button resetPasswordButton;
    private RefreshableRoleSelectionListView refreshableRoleSelectionListView;
  }
}
