#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
      System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("Results", System.Windows.Forms.HorizontalAlignment.Left);
      System.Windows.Forms.ListViewGroup listViewGroup4 = new System.Windows.Forms.ListViewGroup("Parameters", System.Windows.Forms.HorizontalAlignment.Left);
      this.parametersResultsGroupBox = new System.Windows.Forms.GroupBox();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.listView = new System.Windows.Forms.ListView();
      this.nameColumnHeader = new System.Windows.Forms.ColumnHeader();
      this.valueColumnHeader = new System.Windows.Forms.ColumnHeader();
      this.imageList = new System.Windows.Forms.ImageList(this.components);
      this.detailsGroupBox = new System.Windows.Forms.GroupBox();
      this.viewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.showAlgorithmButton = new System.Windows.Forms.Button();
      this.changeColorButton = new System.Windows.Forms.Button();
      this.colorLabel = new System.Windows.Forms.Label();
      this.colorPictureBox = new System.Windows.Forms.PictureBox();
      this.colorDialog = new System.Windows.Forms.ColorDialog();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.parametersResultsGroupBox.SuspendLayout();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.detailsGroupBox.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.colorPictureBox)).BeginInit();
      this.SuspendLayout();
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Size = new System.Drawing.Size(423, 20);
      // 
      // descriptionTextBox
      // 
      this.descriptionTextBox.Size = new System.Drawing.Size(423, 20);
      // 
      // parametersResultsGroupBox
      // 
      this.parametersResultsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.parametersResultsGroupBox.Controls.Add(this.splitContainer);
      this.parametersResultsGroupBox.Location = new System.Drawing.Point(0, 79);
      this.parametersResultsGroupBox.Name = "parametersResultsGroupBox";
      this.parametersResultsGroupBox.Size = new System.Drawing.Size(495, 245);
      this.parametersResultsGroupBox.TabIndex = 4;
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
      this.splitContainer.Size = new System.Drawing.Size(489, 226);
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
      listViewGroup3.Header = "Results";
      listViewGroup3.Name = "resultsGroup";
      listViewGroup4.Header = "Parameters";
      listViewGroup4.Name = "parametersGroup";
      this.listView.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup3,
            listViewGroup4});
      this.listView.HideSelection = false;
      this.listView.Location = new System.Drawing.Point(3, 3);
      this.listView.MultiSelect = false;
      this.listView.Name = "listView";
      this.listView.ShowItemToolTips = true;
      this.listView.Size = new System.Drawing.Size(171, 220);
      this.listView.SmallImageList = this.imageList;
      this.listView.Sorting = System.Windows.Forms.SortOrder.Ascending;
      this.listView.TabIndex = 0;
      this.listView.UseCompatibleStateImageBehavior = false;
      this.listView.View = System.Windows.Forms.View.Details;
      this.listView.SelectedIndexChanged += new System.EventHandler(this.listView_SelectedIndexChanged);
      this.listView.DoubleClick += new System.EventHandler(this.listView_DoubleClick);
      this.listView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.listView_ItemDrag);
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
      this.detailsGroupBox.Size = new System.Drawing.Size(302, 220);
      this.detailsGroupBox.TabIndex = 0;
      this.detailsGroupBox.TabStop = false;
      this.detailsGroupBox.Text = "Details";
      // 
      // viewHost
      // 
      this.viewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.viewHost.Content = null;
      this.viewHost.Location = new System.Drawing.Point(6, 19);
      this.viewHost.Name = "viewHost";
      this.viewHost.ReadOnly = true;
      this.viewHost.Size = new System.Drawing.Size(290, 195);
      this.viewHost.TabIndex = 0;
      this.viewHost.ViewType = null;
      // 
      // showAlgorithmButton
      // 
      this.showAlgorithmButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.showAlgorithmButton.Location = new System.Drawing.Point(0, 330);
      this.showAlgorithmButton.Name = "showAlgorithmButton";
      this.showAlgorithmButton.Size = new System.Drawing.Size(495, 23);
      this.showAlgorithmButton.TabIndex = 5;
      this.showAlgorithmButton.Text = "&Show Algorithm";
      this.toolTip.SetToolTip(this.showAlgorithmButton, "Show the algorithm which produced these results");
      this.showAlgorithmButton.UseVisualStyleBackColor = true;
      this.showAlgorithmButton.Click += new System.EventHandler(this.showAlgorithmButton_Click);
      // 
      // changeColorButton
      // 
      this.changeColorButton.Enabled = false;
      this.changeColorButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.changeColorButton.Location = new System.Drawing.Point(94, 52);
      this.changeColorButton.Name = "changeColorButton";
      this.changeColorButton.Size = new System.Drawing.Size(64, 21);
      this.changeColorButton.TabIndex = 21;
      this.changeColorButton.Text = "Change";
      this.changeColorButton.UseVisualStyleBackColor = true;
      this.changeColorButton.Click += new System.EventHandler(this.changeColorButton_Click);
      // 
      // colorLabel
      // 
      this.colorLabel.AutoSize = true;
      this.colorLabel.Location = new System.Drawing.Point(3, 56);
      this.colorLabel.Name = "colorLabel";
      this.colorLabel.Size = new System.Drawing.Size(34, 13);
      this.colorLabel.TabIndex = 22;
      this.colorLabel.Text = "Color:";
      // 
      // colorPictureBox
      // 
      this.colorPictureBox.Location = new System.Drawing.Point(73, 54);
      this.colorPictureBox.Name = "colorPictureBox";
      this.colorPictureBox.Size = new System.Drawing.Size(17, 17);
      this.colorPictureBox.TabIndex = 23;
      this.colorPictureBox.TabStop = false;
      // 
      // RunView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.colorPictureBox);
      this.Controls.Add(this.colorLabel);
      this.Controls.Add(this.changeColorButton);
      this.Controls.Add(this.parametersResultsGroupBox);
      this.Controls.Add(this.showAlgorithmButton);
      this.Name = "RunView";
      this.Size = new System.Drawing.Size(495, 353);
      this.Controls.SetChildIndex(this.showAlgorithmButton, 0);
      this.Controls.SetChildIndex(this.parametersResultsGroupBox, 0);
      this.Controls.SetChildIndex(this.descriptionLabel, 0);
      this.Controls.SetChildIndex(this.descriptionTextBox, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.changeColorButton, 0);
      this.Controls.SetChildIndex(this.colorLabel, 0);
      this.Controls.SetChildIndex(this.colorPictureBox, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.parametersResultsGroupBox.ResumeLayout(false);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      this.splitContainer.ResumeLayout(false);
      this.detailsGroupBox.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.colorPictureBox)).EndInit();
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
    private System.Windows.Forms.PictureBox colorPictureBox;
    private System.Windows.Forms.ColorDialog colorDialog;

  }
}
