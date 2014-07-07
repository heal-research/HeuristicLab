#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Optimizer {
  partial class StartPage {
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
      this.showStartPageCheckBox = new System.Windows.Forms.CheckBox();
      this.firstStepsRichTextBox = new System.Windows.Forms.RichTextBox();
      this.titleLabel = new System.Windows.Forms.Label();
      this.samplesGroupBox = new System.Windows.Forms.GroupBox();
      this.samplesListView = new System.Windows.Forms.ListView();
      this.nameColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.descriptionColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.imageList = new System.Windows.Forms.ImageList(this.components);
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.samplesGroupBox.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.SuspendLayout();
      // 
      // showStartPageCheckBox
      // 
      this.showStartPageCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.showStartPageCheckBox.AutoSize = true;
      this.showStartPageCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.showStartPageCheckBox.Location = new System.Drawing.Point(3, 620);
      this.showStartPageCheckBox.Name = "showStartPageCheckBox";
      this.showStartPageCheckBox.Size = new System.Drawing.Size(158, 17);
      this.showStartPageCheckBox.TabIndex = 1;
      this.showStartPageCheckBox.Text = "Show Start Page on Startup";
      this.showStartPageCheckBox.UseVisualStyleBackColor = true;
      this.showStartPageCheckBox.CheckedChanged += new System.EventHandler(this.showStartPageCheckBox_CheckedChanged);
      // 
      // firstStepsRichTextBox
      // 
      this.firstStepsRichTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.firstStepsRichTextBox.BackColor = System.Drawing.SystemColors.Control;
      this.firstStepsRichTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.firstStepsRichTextBox.Cursor = System.Windows.Forms.Cursors.Arrow;
      this.firstStepsRichTextBox.Location = new System.Drawing.Point(3, 33);
      this.firstStepsRichTextBox.Name = "firstStepsRichTextBox";
      this.firstStepsRichTextBox.ReadOnly = true;
      this.firstStepsRichTextBox.Size = new System.Drawing.Size(389, 581);
      this.firstStepsRichTextBox.TabIndex = 1;
      this.firstStepsRichTextBox.Text = "First Steps";
      this.firstStepsRichTextBox.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.firstStepsRichTextBox_LinkClicked);
      // 
      // titleLabel
      // 
      this.titleLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.titleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.titleLabel.Location = new System.Drawing.Point(-1, 0);
      this.titleLabel.Name = "titleLabel";
      this.titleLabel.Size = new System.Drawing.Size(393, 30);
      this.titleLabel.TabIndex = 0;
      this.titleLabel.Text = "Title";
      this.titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // samplesGroupBox
      // 
      this.samplesGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.samplesGroupBox.Controls.Add(this.samplesListView);
      this.samplesGroupBox.Location = new System.Drawing.Point(0, 3);
      this.samplesGroupBox.Name = "samplesGroupBox";
      this.samplesGroupBox.Size = new System.Drawing.Size(332, 637);
      this.samplesGroupBox.TabIndex = 0;
      this.samplesGroupBox.TabStop = false;
      this.samplesGroupBox.Text = "Samples";
      // 
      // samplesListView
      // 
      this.samplesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.nameColumnHeader,
            this.descriptionColumnHeader});
      this.samplesListView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.samplesListView.FullRowSelect = true;
      this.samplesListView.Location = new System.Drawing.Point(3, 16);
      this.samplesListView.MultiSelect = false;
      this.samplesListView.Name = "samplesListView";
      this.samplesListView.ShowItemToolTips = true;
      this.samplesListView.Size = new System.Drawing.Size(326, 618);
      this.samplesListView.SmallImageList = this.imageList;
      this.samplesListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
      this.samplesListView.TabIndex = 0;
      this.samplesListView.UseCompatibleStateImageBehavior = false;
      this.samplesListView.View = System.Windows.Forms.View.Details;
      this.samplesListView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.samplesListView_ItemDrag);
      this.samplesListView.DoubleClick += new System.EventHandler(this.samplesListView_DoubleClick);
      // 
      // nameColumnHeader
      // 
      this.nameColumnHeader.Text = "Name";
      this.nameColumnHeader.Width = 150;
      // 
      // descriptionColumnHeader
      // 
      this.descriptionColumnHeader.Text = "Description";
      this.descriptionColumnHeader.Width = 300;
      // 
      // imageList
      // 
      this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
      this.imageList.ImageSize = new System.Drawing.Size(16, 16);
      this.imageList.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // splitContainer
      // 
      this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer.Location = new System.Drawing.Point(0, 0);
      this.splitContainer.Name = "splitContainer";
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.titleLabel);
      this.splitContainer.Panel1.Controls.Add(this.showStartPageCheckBox);
      this.splitContainer.Panel1.Controls.Add(this.firstStepsRichTextBox);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.samplesGroupBox);
      this.splitContainer.Size = new System.Drawing.Size(728, 640);
      this.splitContainer.SplitterDistance = 392;
      this.splitContainer.TabIndex = 0;
      // 
      // StartPage
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.splitContainer);
      this.Name = "StartPage";
      this.Size = new System.Drawing.Size(728, 640);
      this.samplesGroupBox.ResumeLayout(false);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel1.PerformLayout();
      this.splitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.CheckBox showStartPageCheckBox;
    private System.Windows.Forms.RichTextBox firstStepsRichTextBox;
    private System.Windows.Forms.Label titleLabel;
    private System.Windows.Forms.GroupBox samplesGroupBox;
    private System.Windows.Forms.ListView samplesListView;
    private System.Windows.Forms.ColumnHeader nameColumnHeader;
    private System.Windows.Forms.ImageList imageList;
    private System.Windows.Forms.ColumnHeader descriptionColumnHeader;
    private System.Windows.Forms.SplitContainer splitContainer;
  }
}
