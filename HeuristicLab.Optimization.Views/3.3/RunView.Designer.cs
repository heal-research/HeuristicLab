#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  partial class RunView {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing) {
        if (components != null) components.Dispose();
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
      System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Results", System.Windows.Forms.HorizontalAlignment.Left);
      System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Parameters", System.Windows.Forms.HorizontalAlignment.Left);
      this.parametersResultsGroupBox = new System.Windows.Forms.GroupBox();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.listView = new System.Windows.Forms.ListView();
      this.nameColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.valueColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.imageList = new System.Windows.Forms.ImageList(this.components);
      this.detailsGroupBox = new System.Windows.Forms.GroupBox();
      this.viewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.showAlgorithmButton = new System.Windows.Forms.Button();
      this.changeColorButton = new System.Windows.Forms.Button();
      this.colorLabel = new System.Windows.Forms.Label();
      this.colorDialog = new System.Windows.Forms.ColorDialog();
      this.showDetailsCheckBox = new System.Windows.Forms.CheckBox();
      this.colorArea = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.parametersResultsGroupBox.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.detailsGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Location = new System.Drawing.Point(56, 0);
      this.nameTextBox.Size = new System.Drawing.Size(414, 20);
      // 
      // infoLabel
      // 
      this.infoLabel.Location = new System.Drawing.Point(476, 3);
      // 
      // parametersResultsGroupBox
      // 
      this.parametersResultsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.parametersResultsGroupBox.Controls.Add(this.splitContainer);
      this.parametersResultsGroupBox.Location = new System.Drawing.Point(0, 56);
      this.parametersResultsGroupBox.Name = "parametersResultsGroupBox";
      this.parametersResultsGroupBox.Size = new System.Drawing.Size(495, 268);
      this.parametersResultsGroupBox.TabIndex = 6;
      this.parametersResultsGroupBox.TabStop = false;
      this.parametersResultsGroupBox.Text = "Parameters && Results";
      // 
      // splitContainer
      // 
      this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer.Location = new System.Drawing.Point(3, 16);
      this.splitContainer.Name = "splitContainer";
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.listView);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.detailsGroupBox);
      this.splitContainer.Size = new System.Drawing.Size(489, 249);
      this.splitContainer.SplitterDistance = 177;
      this.splitContainer.TabIndex = 0;
      // 
      // listView
      // 
      this.listView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.nameColumnHeader,
            this.valueColumnHeader});
      this.listView.FullRowSelect = true;
      listViewGroup1.Header = "Results";
      listViewGroup1.Name = "resultsGroup";
      listViewGroup2.Header = "Parameters";
      listViewGroup2.Name = "parametersGroup";
      this.listView.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2});
      this.listView.HideSelection = false;
      this.listView.Location = new System.Drawing.Point(3, 3);
      this.listView.MultiSelect = false;
      this.listView.Name = "listView";
      this.listView.ShowItemToolTips = true;
      this.listView.Size = new System.Drawing.Size(171, 243);
      this.listView.SmallImageList = this.imageList;
      this.listView.Sorting = System.Windows.Forms.SortOrder.Ascending;
      this.listView.TabIndex = 0;
      this.listView.UseCompatibleStateImageBehavior = false;
      this.listView.View = System.Windows.Forms.View.Details;
      this.listView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.listView_ItemDrag);
      this.listView.SelectedIndexChanged += new System.EventHandler(this.listView_SelectedIndexChanged);
      this.listView.DoubleClick += new System.EventHandler(this.listView_DoubleClick);
      // 
      // nameColumnHeader
      // 
      this.nameColumnHeader.Text = "Name";
      // 
      // valueColumnHeader
      // 
      this.valueColumnHeader.Text = "Value";
      // 
      // imageList
      // 
      this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
      this.imageList.ImageSize = new System.Drawing.Size(16, 16);
      this.imageList.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // detailsGroupBox
      // 
      this.detailsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.detailsGroupBox.Controls.Add(this.viewHost);
      this.detailsGroupBox.Location = new System.Drawing.Point(3, 3);
      this.detailsGroupBox.Name = "detailsGroupBox";
      this.detailsGroupBox.Size = new System.Drawing.Size(302, 243);
      this.detailsGroupBox.TabIndex = 0;
      this.detailsGroupBox.TabStop = false;
      this.detailsGroupBox.Text = "Details";
      // 
      // viewHost
      // 
      this.viewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.viewHost.Caption = "View";
      this.viewHost.Content = null;
      this.viewHost.Enabled = false;
      this.viewHost.Location = new System.Drawing.Point(6, 19);
      this.viewHost.Name = "viewHost";
      this.viewHost.ReadOnly = true;
      this.viewHost.Size = new System.Drawing.Size(290, 218);
      this.viewHost.TabIndex = 0;
      this.viewHost.ViewsLabelVisible = true;
      this.viewHost.ViewType = null;
      // 
      // showAlgorithmButton
      // 
      this.showAlgorithmButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.showAlgorithmButton.Location = new System.Drawing.Point(0, 330);
      this.showAlgorithmButton.Name = "showAlgorithmButton";
      this.showAlgorithmButton.Size = new System.Drawing.Size(495, 23);
      this.showAlgorithmButton.TabIndex = 8;
      this.showAlgorithmButton.Text = "&Show Algorithm";
      this.toolTip.SetToolTip(this.showAlgorithmButton, "Show the algorithm which produced these results");
      this.showAlgorithmButton.UseVisualStyleBackColor = true;
      this.showAlgorithmButton.Click += new System.EventHandler(this.showAlgorithmButton_Click);
      // 
      // changeColorButton
      // 
      this.changeColorButton.Enabled = false;
      this.changeColorButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.changeColorButton.Location = new System.Drawing.Point(78, 26);
      this.changeColorButton.Name = "changeColorButton";
      this.changeColorButton.Size = new System.Drawing.Size(64, 24);
      this.changeColorButton.TabIndex = 5;
      this.changeColorButton.Text = "&Change...";
      this.changeColorButton.UseVisualStyleBackColor = true;
      this.changeColorButton.Click += new System.EventHandler(this.changeColorButton_Click);
      // 
      // colorLabel
      // 
      this.colorLabel.AutoSize = true;
      this.colorLabel.Location = new System.Drawing.Point(3, 32);
      this.colorLabel.Name = "colorLabel";
      this.colorLabel.Size = new System.Drawing.Size(34, 13);
      this.colorLabel.TabIndex = 3;
      this.colorLabel.Text = "C&olor:";
      // 
      // showDetailsCheckBox
      // 
      this.showDetailsCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.showDetailsCheckBox.Appearance = System.Windows.Forms.Appearance.Button;
      this.showDetailsCheckBox.Checked = true;
      this.showDetailsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
      this.showDetailsCheckBox.Image = HeuristicLab.Common.Resources.VSImageLibrary.Properties;
      this.showDetailsCheckBox.Location = new System.Drawing.Point(471, 26);
      this.showDetailsCheckBox.Name = "showDetailsCheckBox";
      this.showDetailsCheckBox.Size = new System.Drawing.Size(24, 24);
      this.showDetailsCheckBox.TabIndex = 7;
      this.toolTip.SetToolTip(this.showDetailsCheckBox, "Show/Hide Details");
      this.showDetailsCheckBox.UseVisualStyleBackColor = true;
      this.showDetailsCheckBox.CheckedChanged += new System.EventHandler(this.showDetailsCheckBox_CheckedChanged);
      // 
      // colorArea
      // 
      this.colorArea.BackColor = System.Drawing.Color.White;
      this.colorArea.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.colorArea.Location = new System.Drawing.Point(56, 31);
      this.colorArea.Name = "colorArea";
      this.colorArea.Size = new System.Drawing.Size(16, 16);
      this.colorArea.TabIndex = 4;
      // 
      // RunView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.colorArea);
      this.Controls.Add(this.showDetailsCheckBox);
      this.Controls.Add(this.colorLabel);
      this.Controls.Add(this.changeColorButton);
      this.Controls.Add(this.parametersResultsGroupBox);
      this.Controls.Add(this.showAlgorithmButton);
      this.Name = "RunView";
      this.Size = new System.Drawing.Size(495, 353);
      this.Controls.SetChildIndex(this.infoLabel, 0);
      this.Controls.SetChildIndex(this.showAlgorithmButton, 0);
      this.Controls.SetChildIndex(this.parametersResultsGroupBox, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.changeColorButton, 0);
      this.Controls.SetChildIndex(this.colorLabel, 0);
      this.Controls.SetChildIndex(this.showDetailsCheckBox, 0);
      this.Controls.SetChildIndex(this.colorArea, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.parametersResultsGroupBox.ResumeLayout(false);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.detailsGroupBox.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.GroupBox parametersResultsGroupBox;
    private System.Windows.Forms.SplitContainer splitContainer;
    private System.Windows.Forms.GroupBox detailsGroupBox;
    private System.Windows.Forms.ListView listView;
    private System.Windows.Forms.ColumnHeader nameColumnHeader;
    private System.Windows.Forms.ColumnHeader valueColumnHeader;
    private HeuristicLab.MainForm.WindowsForms.ViewHost viewHost;
    private System.Windows.Forms.ImageList imageList;
    private System.Windows.Forms.Button showAlgorithmButton;
    private System.Windows.Forms.Label colorLabel;
    private System.Windows.Forms.Button changeColorButton;
    private System.Windows.Forms.ColorDialog colorDialog;
    private System.Windows.Forms.CheckBox showDetailsCheckBox;
    private System.Windows.Forms.Label colorArea;

  }
}
