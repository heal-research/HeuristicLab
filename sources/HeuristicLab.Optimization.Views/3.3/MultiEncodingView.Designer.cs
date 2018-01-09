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

namespace HeuristicLab.Optimization.Views {
  partial class MultiEncodingView {
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
      this.encodingsListView = new System.Windows.Forms.ListView();
      this.encodingNameColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.encodingTypeColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.addEncodingButton = new System.Windows.Forms.Button();
      this.removeEncodingButton = new System.Windows.Forms.Button();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.encodingDetailsGroupBox = new System.Windows.Forms.GroupBox();
      this.encodingDetailViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.encodingsGroupBox = new System.Windows.Forms.GroupBox();
      this.tabControl = new System.Windows.Forms.TabControl();
      this.encodingsTabPage = new System.Windows.Forms.TabPage();
      this.parametersTabPage = new System.Windows.Forms.TabPage();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.encodingDetailsGroupBox.SuspendLayout();
      this.encodingsGroupBox.SuspendLayout();
      this.tabControl.SuspendLayout();
      this.encodingsTabPage.SuspendLayout();
      this.parametersTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // parameterCollectionView
      // 
      this.parameterCollectionView.Location = new System.Drawing.Point(6, 6);
      this.parameterCollectionView.Size = new System.Drawing.Size(643, 433);
      this.parameterCollectionView.TabIndex = 0;
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Size = new System.Drawing.Size(583, 20);
      // 
      // infoLabel
      // 
      this.infoLabel.Location = new System.Drawing.Point(647, 3);
      // 
      // encodingsListView
      // 
      this.encodingsListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.encodingsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.encodingNameColumnHeader,
            this.encodingTypeColumnHeader});
      this.encodingsListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
      this.encodingsListView.HideSelection = false;
      this.encodingsListView.Location = new System.Drawing.Point(3, 33);
      this.encodingsListView.MultiSelect = false;
      this.encodingsListView.Name = "encodingsListView";
      this.encodingsListView.ShowGroups = false;
      this.encodingsListView.Size = new System.Drawing.Size(241, 378);
      this.encodingsListView.TabIndex = 2;
      this.encodingsListView.UseCompatibleStateImageBehavior = false;
      this.encodingsListView.View = System.Windows.Forms.View.Details;
      this.encodingsListView.SelectedIndexChanged += new System.EventHandler(this.encodingsListView_SelectedIndexChanged);
      // 
      // encodingNameColumnHeader
      // 
      this.encodingNameColumnHeader.Text = "Name";
      this.encodingNameColumnHeader.Width = 100;
      // 
      // encodingTypeColumnHeader
      // 
      this.encodingTypeColumnHeader.Text = "Type";
      this.encodingTypeColumnHeader.Width = 136;
      // 
      // addEncodingButton
      // 
      this.addEncodingButton.Location = new System.Drawing.Point(3, 3);
      this.addEncodingButton.Name = "addEncodingButton";
      this.addEncodingButton.Size = new System.Drawing.Size(24, 24);
      this.addEncodingButton.TabIndex = 0;
      this.addEncodingButton.Text = "+";
      this.addEncodingButton.UseVisualStyleBackColor = true;
      this.addEncodingButton.Click += new System.EventHandler(this.addEncodingButton_Click);
      // 
      // removeEncodingButton
      // 
      this.removeEncodingButton.Location = new System.Drawing.Point(33, 3);
      this.removeEncodingButton.Name = "removeEncodingButton";
      this.removeEncodingButton.Size = new System.Drawing.Size(24, 24);
      this.removeEncodingButton.TabIndex = 1;
      this.removeEncodingButton.Text = "x";
      this.removeEncodingButton.UseVisualStyleBackColor = true;
      this.removeEncodingButton.Click += new System.EventHandler(this.removeEncodingButton_Click);
      // 
      // splitContainer
      // 
      this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
      this.splitContainer.Location = new System.Drawing.Point(3, 16);
      this.splitContainer.Name = "splitContainer";
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.encodingsListView);
      this.splitContainer.Panel1.Controls.Add(this.removeEncodingButton);
      this.splitContainer.Panel1.Controls.Add(this.addEncodingButton);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.encodingDetailsGroupBox);
      this.splitContainer.Size = new System.Drawing.Size(637, 414);
      this.splitContainer.SplitterDistance = 247;
      this.splitContainer.TabIndex = 5;
      // 
      // encodingDetailsGroupBox
      // 
      this.encodingDetailsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.encodingDetailsGroupBox.Controls.Add(this.encodingDetailViewHost);
      this.encodingDetailsGroupBox.Location = new System.Drawing.Point(3, 27);
      this.encodingDetailsGroupBox.Name = "encodingDetailsGroupBox";
      this.encodingDetailsGroupBox.Size = new System.Drawing.Size(380, 384);
      this.encodingDetailsGroupBox.TabIndex = 0;
      this.encodingDetailsGroupBox.TabStop = false;
      this.encodingDetailsGroupBox.Text = "Details";
      // 
      // encodingDetailViewHost
      // 
      this.encodingDetailViewHost.Caption = "View";
      this.encodingDetailViewHost.Content = null;
      this.encodingDetailViewHost.Dock = System.Windows.Forms.DockStyle.Fill;
      this.encodingDetailViewHost.Enabled = false;
      this.encodingDetailViewHost.Location = new System.Drawing.Point(3, 16);
      this.encodingDetailViewHost.Name = "encodingDetailViewHost";
      this.encodingDetailViewHost.ReadOnly = false;
      this.encodingDetailViewHost.Size = new System.Drawing.Size(374, 365);
      this.encodingDetailViewHost.TabIndex = 0;
      this.encodingDetailViewHost.ViewsLabelVisible = true;
      this.encodingDetailViewHost.ViewType = null;
      // 
      // encodingsGroupBox
      // 
      this.encodingsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.encodingsGroupBox.Controls.Add(this.splitContainer);
      this.encodingsGroupBox.Location = new System.Drawing.Point(6, 6);
      this.encodingsGroupBox.Name = "encodingsGroupBox";
      this.encodingsGroupBox.Size = new System.Drawing.Size(643, 433);
      this.encodingsGroupBox.TabIndex = 0;
      this.encodingsGroupBox.TabStop = false;
      this.encodingsGroupBox.Text = "Encodings";
      // 
      // tabControl
      // 
      this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl.Controls.Add(this.encodingsTabPage);
      this.tabControl.Controls.Add(this.parametersTabPage);
      this.tabControl.Location = new System.Drawing.Point(0, 26);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(663, 471);
      this.tabControl.TabIndex = 3;
      // 
      // encodingsTabPage
      // 
      this.encodingsTabPage.Controls.Add(this.encodingsGroupBox);
      this.encodingsTabPage.Location = new System.Drawing.Point(4, 22);
      this.encodingsTabPage.Name = "encodingsTabPage";
      this.encodingsTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.encodingsTabPage.Size = new System.Drawing.Size(655, 445);
      this.encodingsTabPage.TabIndex = 0;
      this.encodingsTabPage.Text = "Encodings";
      this.encodingsTabPage.UseVisualStyleBackColor = true;
      // 
      // parametersTabPage
      // 
      this.parametersTabPage.Controls.Add(this.parameterCollectionView);
      this.parametersTabPage.Location = new System.Drawing.Point(4, 22);
      this.parametersTabPage.Name = "parametersTabPage";
      this.parametersTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.parametersTabPage.Size = new System.Drawing.Size(655, 445);
      this.parametersTabPage.TabIndex = 1;
      this.parametersTabPage.Text = "Parameters";
      this.parametersTabPage.UseVisualStyleBackColor = true;
      // 
      // MultiEncodingView
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.tabControl);
      this.Name = "MultiEncodingView";
      this.Size = new System.Drawing.Size(666, 497);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      this.Controls.SetChildIndex(this.infoLabel, 0);
      this.Controls.SetChildIndex(this.tabControl, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.encodingDetailsGroupBox.ResumeLayout(false);
      this.encodingsGroupBox.ResumeLayout(false);
      this.tabControl.ResumeLayout(false);
      this.encodingsTabPage.ResumeLayout(false);
      this.parametersTabPage.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ListView encodingsListView;
    private System.Windows.Forms.Button addEncodingButton;
    private System.Windows.Forms.Button removeEncodingButton;
    private System.Windows.Forms.SplitContainer splitContainer;
    private System.Windows.Forms.ColumnHeader encodingNameColumnHeader;
    private System.Windows.Forms.ColumnHeader encodingTypeColumnHeader;
    private System.Windows.Forms.GroupBox encodingDetailsGroupBox;
    private MainForm.WindowsForms.ViewHost encodingDetailViewHost;
    private System.Windows.Forms.GroupBox encodingsGroupBox;
    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TabPage encodingsTabPage;
    private System.Windows.Forms.TabPage parametersTabPage;
  }
}
