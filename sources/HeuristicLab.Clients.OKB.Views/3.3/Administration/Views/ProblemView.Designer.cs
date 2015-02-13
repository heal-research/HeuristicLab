#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Clients.OKB.Administration {
  partial class ProblemView {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProblemView));
      this.platformLabel = new System.Windows.Forms.Label();
      this.platformComboBox = new System.Windows.Forms.ComboBox();
      this.problemClassLabel = new System.Windows.Forms.Label();
      this.problemClassComboBox = new System.Windows.Forms.ComboBox();
      this.tabControl = new System.Windows.Forms.TabControl();
      this.usersTabPage = new System.Windows.Forms.TabPage();
      this.storeUsersButton = new System.Windows.Forms.Button();
      this.problemUserView = new HeuristicLab.Clients.Access.Views.RefreshableLightweightUserView();
      this.dataTabPage = new System.Windows.Forms.TabPage();
      this.noViewAvailableLabel = new System.Windows.Forms.Label();
      this.dataViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.saveFileButton = new System.Windows.Forms.Button();
      this.openFileButton = new System.Windows.Forms.Button();
      this.newDataButton = new System.Windows.Forms.Button();
      this.storeDataButton = new System.Windows.Forms.Button();
      this.refreshDataButton = new System.Windows.Forms.Button();
      this.dataTypeNameLabel = new System.Windows.Forms.Label();
      this.dataTypeGroupBox = new System.Windows.Forms.GroupBox();
      this.dataTypeTypeNameTextBox = new System.Windows.Forms.TextBox();
      this.dataTypeNameTextBox = new System.Windows.Forms.TextBox();
      this.dataTypeTypeNameLabel = new System.Windows.Forms.Label();
      this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
      this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
      this.tabControl.SuspendLayout();
      this.usersTabPage.SuspendLayout();
      this.dataTabPage.SuspendLayout();
      this.dataTypeGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // nameTextBox
      // 
      this.nameTextBox.Location = new System.Drawing.Point(90, 29);
      this.nameTextBox.Size = new System.Drawing.Size(543, 20);
      // 
      // descriptionTextBox
      // 
      this.descriptionTextBox.Location = new System.Drawing.Point(90, 55);
      this.descriptionTextBox.Size = new System.Drawing.Size(543, 20);
      // 
      // storeButton
      // 
      this.toolTip.SetToolTip(this.storeButton, "Store Data");
      // 
      // platformLabel
      // 
      this.platformLabel.AutoSize = true;
      this.platformLabel.Location = new System.Drawing.Point(3, 84);
      this.platformLabel.Name = "platformLabel";
      this.platformLabel.Size = new System.Drawing.Size(48, 13);
      this.platformLabel.TabIndex = 5;
      this.platformLabel.Text = "&Platform:";
      // 
      // platformComboBox
      // 
      this.platformComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.platformComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.platformComboBox.FormattingEnabled = true;
      this.platformComboBox.Location = new System.Drawing.Point(90, 81);
      this.platformComboBox.Name = "platformComboBox";
      this.platformComboBox.Size = new System.Drawing.Size(543, 21);
      this.platformComboBox.TabIndex = 6;
      this.platformComboBox.SelectedValueChanged += new System.EventHandler(this.platformComboBox_SelectedValueChanged);
      // 
      // problemClassLabel
      // 
      this.problemClassLabel.AutoSize = true;
      this.problemClassLabel.Location = new System.Drawing.Point(3, 111);
      this.problemClassLabel.Name = "problemClassLabel";
      this.problemClassLabel.Size = new System.Drawing.Size(76, 13);
      this.problemClassLabel.TabIndex = 7;
      this.problemClassLabel.Text = "&Problem Class:";
      // 
      // problemClassComboBox
      // 
      this.problemClassComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.problemClassComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.problemClassComboBox.FormattingEnabled = true;
      this.problemClassComboBox.Location = new System.Drawing.Point(90, 108);
      this.problemClassComboBox.Name = "problemClassComboBox";
      this.problemClassComboBox.Size = new System.Drawing.Size(543, 21);
      this.problemClassComboBox.TabIndex = 8;
      this.problemClassComboBox.SelectedValueChanged += new System.EventHandler(this.problemClassComboBox_SelectedValueChanged);
      // 
      // tabControl
      // 
      this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl.Controls.Add(this.usersTabPage);
      this.tabControl.Controls.Add(this.dataTabPage);
      this.tabControl.Location = new System.Drawing.Point(0, 239);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(633, 206);
      this.tabControl.TabIndex = 10;
      // 
      // usersTabPage
      // 
      this.usersTabPage.Controls.Add(this.storeUsersButton);
      this.usersTabPage.Controls.Add(this.problemUserView);
      this.usersTabPage.Location = new System.Drawing.Point(4, 22);
      this.usersTabPage.Name = "usersTabPage";
      this.usersTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.usersTabPage.Size = new System.Drawing.Size(625, 180);
      this.usersTabPage.TabIndex = 0;
      this.usersTabPage.Text = "Authorized Users and Groups";
      this.usersTabPage.UseVisualStyleBackColor = true;
      // 
      // storeUsersButton
      // 
      this.storeUsersButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.PublishToWeb;
      this.storeUsersButton.Location = new System.Drawing.Point(38, 9);
      this.storeUsersButton.Name = "storeUsersButton";
      this.storeUsersButton.Size = new System.Drawing.Size(24, 24);
      this.storeUsersButton.TabIndex = 1;
      this.toolTip.SetToolTip(this.storeUsersButton, "Store Authorized Users");
      this.storeUsersButton.UseVisualStyleBackColor = true;
      this.storeUsersButton.Click += new System.EventHandler(this.storeUsersButton_Click);
      // 
      // problemUserView
      // 
      this.problemUserView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.problemUserView.Caption = "RefreshableLightweightUser View";
      this.problemUserView.Content = null;
      this.problemUserView.FetchSelectedUsers = null;
      this.problemUserView.Location = new System.Drawing.Point(6, 6);
      this.problemUserView.Name = "problemUserView";
      this.problemUserView.ReadOnly = false;
      this.problemUserView.Size = new System.Drawing.Size(613, 168);
      this.problemUserView.TabIndex = 2;
      this.problemUserView.SelectedUsersChanged += new System.EventHandler(this.problemUserView_SelectedUsersChanged);
      // 
      // dataTabPage
      // 
      this.dataTabPage.Controls.Add(this.noViewAvailableLabel);
      this.dataTabPage.Controls.Add(this.dataViewHost);
      this.dataTabPage.Controls.Add(this.saveFileButton);
      this.dataTabPage.Controls.Add(this.openFileButton);
      this.dataTabPage.Controls.Add(this.newDataButton);
      this.dataTabPage.Controls.Add(this.storeDataButton);
      this.dataTabPage.Controls.Add(this.refreshDataButton);
      this.dataTabPage.Location = new System.Drawing.Point(4, 22);
      this.dataTabPage.Name = "dataTabPage";
      this.dataTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.dataTabPage.Size = new System.Drawing.Size(625, 180);
      this.dataTabPage.TabIndex = 1;
      this.dataTabPage.Text = "Platform-Specific Problem Data";
      this.dataTabPage.UseVisualStyleBackColor = true;
      // 
      // noViewAvailableLabel
      // 
      this.noViewAvailableLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.noViewAvailableLabel.AutoSize = true;
      this.noViewAvailableLabel.Location = new System.Drawing.Point(265, 84);
      this.noViewAvailableLabel.Name = "noViewAvailableLabel";
      this.noViewAvailableLabel.Size = new System.Drawing.Size(94, 13);
      this.noViewAvailableLabel.TabIndex = 6;
      this.noViewAvailableLabel.Text = "No view available.";
      // 
      // dataViewHost
      // 
      this.dataViewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.dataViewHost.Caption = "View";
      this.dataViewHost.Content = null;
      this.dataViewHost.Enabled = false;
      this.dataViewHost.Location = new System.Drawing.Point(6, 36);
      this.dataViewHost.Name = "dataViewHost";
      this.dataViewHost.ReadOnly = false;
      this.dataViewHost.Size = new System.Drawing.Size(613, 138);
      this.dataViewHost.TabIndex = 5;
      this.dataViewHost.ViewsLabelVisible = true;
      this.dataViewHost.ViewType = null;
      // 
      // saveFileButton
      // 
      this.saveFileButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Save;
      this.saveFileButton.Location = new System.Drawing.Point(126, 6);
      this.saveFileButton.Name = "saveFileButton";
      this.saveFileButton.Size = new System.Drawing.Size(24, 24);
      this.saveFileButton.TabIndex = 4;
      this.toolTip.SetToolTip(this.saveFileButton, "Save Problem Data into File");
      this.saveFileButton.UseVisualStyleBackColor = true;
      this.saveFileButton.Click += new System.EventHandler(this.saveFileButton_Click);
      // 
      // openFileButton
      // 
      this.openFileButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Open;
      this.openFileButton.Location = new System.Drawing.Point(96, 6);
      this.openFileButton.Name = "openFileButton";
      this.openFileButton.Size = new System.Drawing.Size(24, 24);
      this.openFileButton.TabIndex = 3;
      this.toolTip.SetToolTip(this.openFileButton, "Load Problem Data from File");
      this.openFileButton.UseVisualStyleBackColor = true;
      this.openFileButton.Click += new System.EventHandler(this.openFileButton_Click);
      // 
      // newDataButton
      // 
      this.newDataButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.NewDocument;
      this.newDataButton.Location = new System.Drawing.Point(66, 6);
      this.newDataButton.Name = "newDataButton";
      this.newDataButton.Size = new System.Drawing.Size(24, 24);
      this.newDataButton.TabIndex = 2;
      this.toolTip.SetToolTip(this.newDataButton, "Create New Problem Data");
      this.newDataButton.UseVisualStyleBackColor = true;
      this.newDataButton.Click += new System.EventHandler(this.newDataButton_Click);
      // 
      // storeDataButton
      // 
      this.storeDataButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.PublishToWeb;
      this.storeDataButton.Location = new System.Drawing.Point(36, 6);
      this.storeDataButton.Name = "storeDataButton";
      this.storeDataButton.Size = new System.Drawing.Size(24, 24);
      this.storeDataButton.TabIndex = 1;
      this.toolTip.SetToolTip(this.storeDataButton, "Store Problem Data");
      this.storeDataButton.UseVisualStyleBackColor = true;
      this.storeDataButton.Click += new System.EventHandler(this.storeDataButton_Click);
      // 
      // refreshDataButton
      // 
      this.refreshDataButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Refresh;
      this.refreshDataButton.Location = new System.Drawing.Point(6, 6);
      this.refreshDataButton.Name = "refreshDataButton";
      this.refreshDataButton.Size = new System.Drawing.Size(24, 24);
      this.refreshDataButton.TabIndex = 0;
      this.toolTip.SetToolTip(this.refreshDataButton, "Refresh Problem Data");
      this.refreshDataButton.UseVisualStyleBackColor = true;
      this.refreshDataButton.Click += new System.EventHandler(this.refreshDataButton_Click);
      // 
      // dataTypeNameLabel
      // 
      this.dataTypeNameLabel.AutoSize = true;
      this.dataTypeNameLabel.Location = new System.Drawing.Point(6, 22);
      this.dataTypeNameLabel.Name = "dataTypeNameLabel";
      this.dataTypeNameLabel.Size = new System.Drawing.Size(38, 13);
      this.dataTypeNameLabel.TabIndex = 0;
      this.dataTypeNameLabel.Text = "&Name:";
      // 
      // dataTypeGroupBox
      // 
      this.dataTypeGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.dataTypeGroupBox.Controls.Add(this.dataTypeTypeNameTextBox);
      this.dataTypeGroupBox.Controls.Add(this.dataTypeNameTextBox);
      this.dataTypeGroupBox.Controls.Add(this.dataTypeTypeNameLabel);
      this.dataTypeGroupBox.Controls.Add(this.dataTypeNameLabel);
      this.dataTypeGroupBox.Location = new System.Drawing.Point(0, 146);
      this.dataTypeGroupBox.Name = "dataTypeGroupBox";
      this.dataTypeGroupBox.Size = new System.Drawing.Size(633, 77);
      this.dataTypeGroupBox.TabIndex = 9;
      this.dataTypeGroupBox.TabStop = false;
      this.dataTypeGroupBox.Text = "Data Type";
      // 
      // dataTypeTypeNameTextBox
      // 
      this.dataTypeTypeNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.dataTypeTypeNameTextBox.Location = new System.Drawing.Point(90, 45);
      this.dataTypeTypeNameTextBox.Name = "dataTypeTypeNameTextBox";
      this.dataTypeTypeNameTextBox.Size = new System.Drawing.Size(537, 20);
      this.dataTypeTypeNameTextBox.TabIndex = 3;
      this.toolTip.SetToolTip(this.dataTypeTypeNameTextBox, "Machine Readable Data Type Name (e.g. Assembly Qualified Name)");
      this.dataTypeTypeNameTextBox.TextChanged += new System.EventHandler(this.dataTypeTypeNameTextBox_TextChanged);
      // 
      // dataTypeNameTextBox
      // 
      this.dataTypeNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.dataTypeNameTextBox.Location = new System.Drawing.Point(90, 19);
      this.dataTypeNameTextBox.Name = "dataTypeNameTextBox";
      this.dataTypeNameTextBox.Size = new System.Drawing.Size(537, 20);
      this.dataTypeNameTextBox.TabIndex = 1;
      this.toolTip.SetToolTip(this.dataTypeNameTextBox, "Human Readable Data Type Name");
      this.dataTypeNameTextBox.TextChanged += new System.EventHandler(this.dataTypeNameTextBox_TextChanged);
      // 
      // dataTypeTypeNameLabel
      // 
      this.dataTypeTypeNameLabel.AutoSize = true;
      this.dataTypeTypeNameLabel.Location = new System.Drawing.Point(6, 48);
      this.dataTypeTypeNameLabel.Name = "dataTypeTypeNameLabel";
      this.dataTypeTypeNameLabel.Size = new System.Drawing.Size(65, 13);
      this.dataTypeTypeNameLabel.TabIndex = 2;
      this.dataTypeTypeNameLabel.Text = "&Type Name:";
      // 
      // openFileDialog
      // 
      this.openFileDialog.FileName = "data";
      this.openFileDialog.Filter = "All Files (*.*)|*.*";
      this.openFileDialog.Title = "Load Problem Data";
      // 
      // saveFileDialog
      // 
      this.saveFileDialog.FileName = "data";
      this.saveFileDialog.Filter = "All Files (*.*)|*.*";
      this.saveFileDialog.Title = "Save Problem Data";
      // 
      // ProblemView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.Controls.Add(this.dataTypeGroupBox);
      this.Controls.Add(this.tabControl);
      this.Controls.Add(this.platformComboBox);
      this.Controls.Add(this.platformLabel);
      this.Controls.Add(this.problemClassComboBox);
      this.Controls.Add(this.problemClassLabel);
      this.Name = "ProblemView";
      this.Size = new System.Drawing.Size(633, 445);
      this.Controls.SetChildIndex(this.problemClassLabel, 0);
      this.Controls.SetChildIndex(this.problemClassComboBox, 0);
      this.Controls.SetChildIndex(this.platformLabel, 0);
      this.Controls.SetChildIndex(this.platformComboBox, 0);
      this.Controls.SetChildIndex(this.tabControl, 0);
      this.Controls.SetChildIndex(this.dataTypeGroupBox, 0);
      this.Controls.SetChildIndex(this.storeButton, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.descriptionLabel, 0);
      this.Controls.SetChildIndex(this.descriptionTextBox, 0);
      this.tabControl.ResumeLayout(false);
      this.usersTabPage.ResumeLayout(false);
      this.dataTabPage.ResumeLayout(false);
      this.dataTabPage.PerformLayout();
      this.dataTypeGroupBox.ResumeLayout(false);
      this.dataTypeGroupBox.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label platformLabel;
    private System.Windows.Forms.ComboBox platformComboBox;
    private System.Windows.Forms.Label problemClassLabel;
    private System.Windows.Forms.ComboBox problemClassComboBox;
    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TabPage usersTabPage;
    private System.Windows.Forms.Button storeUsersButton;
    private System.Windows.Forms.TabPage dataTabPage;
    private System.Windows.Forms.Label dataTypeNameLabel;
    private System.Windows.Forms.GroupBox dataTypeGroupBox;
    private System.Windows.Forms.TextBox dataTypeTypeNameTextBox;
    private System.Windows.Forms.TextBox dataTypeNameTextBox;
    private System.Windows.Forms.Label dataTypeTypeNameLabel;
    private System.Windows.Forms.Button storeDataButton;
    private System.Windows.Forms.Button refreshDataButton;
    private System.Windows.Forms.Button openFileButton;
    private System.Windows.Forms.Button newDataButton;
    private System.Windows.Forms.Button saveFileButton;
    private System.Windows.Forms.OpenFileDialog openFileDialog;
    private System.Windows.Forms.SaveFileDialog saveFileDialog;
    private MainForm.WindowsForms.ViewHost dataViewHost;
    private System.Windows.Forms.Label noViewAvailableLabel;
    private Access.Views.RefreshableLightweightUserView problemUserView;

  }
}
