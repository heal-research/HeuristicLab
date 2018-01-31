#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Clients.Access.Views {
  partial class LightweightUserInformationView {
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
      this.changePasswordButton = new System.Windows.Forms.Button();
      this.label3 = new System.Windows.Forms.Label();
      this.fullNameTextBox = new System.Windows.Forms.TextBox();
      this.label4 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.userNameTextBox = new System.Windows.Forms.TextBox();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.rolesListView = new System.Windows.Forms.ListView();
      this.columnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.imageList = new System.Windows.Forms.ImageList(this.components);
      this.groupsListView = new System.Windows.Forms.ListView();
      this.groupsColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.label5 = new System.Windows.Forms.Label();
      this.emailTextBox = new System.Windows.Forms.TextBox();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.SuspendLayout();
      // 
      // changePasswordButton
      // 
      this.changePasswordButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.changePasswordButton.Location = new System.Drawing.Point(316, 231);
      this.changePasswordButton.Name = "changePasswordButton";
      this.changePasswordButton.Size = new System.Drawing.Size(112, 23);
      this.changePasswordButton.TabIndex = 19;
      this.changePasswordButton.Text = "Change Password";
      this.changePasswordButton.UseVisualStyleBackColor = true;
      this.changePasswordButton.Click += new System.EventHandler(this.changePasswordButton_Click);
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(3, 0);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(37, 13);
      this.label3.TabIndex = 17;
      this.label3.Text = "Roles:";
      // 
      // fullNameTextBox
      // 
      this.fullNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.fullNameTextBox.Location = new System.Drawing.Point(69, 29);
      this.fullNameTextBox.Name = "fullNameTextBox";
      this.fullNameTextBox.Size = new System.Drawing.Size(359, 20);
      this.fullNameTextBox.TabIndex = 16;
      this.fullNameTextBox.TextChanged += new System.EventHandler(this.fullNameTextBox_TextChanged);
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(3, 0);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(44, 13);
      this.label4.TabIndex = 18;
      this.label4.Text = "Groups:";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(0, 6);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(63, 13);
      this.label1.TabIndex = 11;
      this.label1.Text = "User Name:";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(0, 32);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(57, 13);
      this.label2.TabIndex = 12;
      this.label2.Text = "Full Name:";
      // 
      // userNameTextBox
      // 
      this.userNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.userNameTextBox.Location = new System.Drawing.Point(69, 3);
      this.userNameTextBox.Name = "userNameTextBox";
      this.userNameTextBox.ReadOnly = true;
      this.userNameTextBox.Size = new System.Drawing.Size(359, 20);
      this.userNameTextBox.TabIndex = 15;
      // 
      // splitContainer
      // 
      this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.splitContainer.Location = new System.Drawing.Point(3, 81);
      this.splitContainer.Name = "splitContainer";
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.rolesListView);
      this.splitContainer.Panel1.Controls.Add(this.label3);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.groupsListView);
      this.splitContainer.Panel2.Controls.Add(this.label4);
      this.splitContainer.Size = new System.Drawing.Size(425, 144);
      this.splitContainer.SplitterDistance = 211;
      this.splitContainer.TabIndex = 20;
      // 
      // rolesListView
      // 
      this.rolesListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.rolesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader});
      this.rolesListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
      this.rolesListView.HideSelection = false;
      this.rolesListView.Location = new System.Drawing.Point(3, 16);
      this.rolesListView.Name = "rolesListView";
      this.rolesListView.Size = new System.Drawing.Size(205, 125);
      this.rolesListView.SmallImageList = this.imageList;
      this.rolesListView.TabIndex = 18;
      this.rolesListView.UseCompatibleStateImageBehavior = false;
      this.rolesListView.View = System.Windows.Forms.View.Details;
      // 
      // columnHeader
      // 
      this.columnHeader.Width = 100;
      // 
      // imageList
      // 
      this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
      this.imageList.ImageSize = new System.Drawing.Size(16, 16);
      this.imageList.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // groupsListView
      // 
      this.groupsListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.groupsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.groupsColumnHeader});
      this.groupsListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
      this.groupsListView.HideSelection = false;
      this.groupsListView.Location = new System.Drawing.Point(3, 16);
      this.groupsListView.Name = "groupsListView";
      this.groupsListView.Size = new System.Drawing.Size(204, 125);
      this.groupsListView.SmallImageList = this.imageList;
      this.groupsListView.TabIndex = 19;
      this.groupsListView.UseCompatibleStateImageBehavior = false;
      this.groupsListView.View = System.Windows.Forms.View.Details;
      // 
      // groupsColumnHeader
      // 
      this.groupsColumnHeader.Width = 100;
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(0, 58);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(39, 13);
      this.label5.TabIndex = 21;
      this.label5.Text = "E-Mail:";
      // 
      // emailTextBox
      // 
      this.emailTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.emailTextBox.Location = new System.Drawing.Point(69, 55);
      this.emailTextBox.Name = "emailTextBox";
      this.emailTextBox.Size = new System.Drawing.Size(359, 20);
      this.emailTextBox.TabIndex = 22;
      this.emailTextBox.TextChanged += new System.EventHandler(this.emailTextBox_TextChanged);
      // 
      // LightweightUserInformationView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.emailTextBox);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.splitContainer);
      this.Controls.Add(this.changePasswordButton);
      this.Controls.Add(this.fullNameTextBox);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.userNameTextBox);
      this.Name = "LightweightUserInformationView";
      this.Size = new System.Drawing.Size(431, 257);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel1.PerformLayout();
      this.splitContainer.Panel2.ResumeLayout(false);
      this.splitContainer.Panel2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button changePasswordButton;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox fullNameTextBox;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox userNameTextBox;
    private System.Windows.Forms.SplitContainer splitContainer;
    private System.Windows.Forms.ListView rolesListView;
    private System.Windows.Forms.ListView groupsListView;
    private System.Windows.Forms.ImageList imageList;
    private System.Windows.Forms.ColumnHeader columnHeader;
    private System.Windows.Forms.ColumnHeader groupsColumnHeader;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.TextBox emailTextBox;
  }
}
