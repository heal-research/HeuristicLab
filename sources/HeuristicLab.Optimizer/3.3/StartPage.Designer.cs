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
      this.loadingPanel = new System.Windows.Forms.Panel();
      this.loadingProgressBar = new System.Windows.Forms.ProgressBar();
      this.loadingLabel = new System.Windows.Forms.Label();
      this.samplesListView = new System.Windows.Forms.ListView();
      this.nameColumnHeader = new System.Windows.Forms.ColumnHeader();
      this.descriptionColumnHeader = new System.Windows.Forms.ColumnHeader();
      this.imageList = new System.Windows.Forms.ImageList(this.components);
      this.samplesGroupBox.SuspendLayout();
      this.loadingPanel.SuspendLayout();
      this.SuspendLayout();
      // 
      // showStartPageCheckBox
      // 
      this.showStartPageCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.showStartPageCheckBox.AutoSize = true;
      this.showStartPageCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.showStartPageCheckBox.Location = new System.Drawing.Point(3, 542);
      this.showStartPageCheckBox.Name = "showStartPageCheckBox";
      this.showStartPageCheckBox.Size = new System.Drawing.Size(158, 17);
      this.showStartPageCheckBox.TabIndex = 3;
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
      this.firstStepsRichTextBox.Size = new System.Drawing.Size(760, 319);
      this.firstStepsRichTextBox.TabIndex = 1;
      this.firstStepsRichTextBox.Text = "First Steps";
      this.firstStepsRichTextBox.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.firstStepsRichTextBox_LinkClicked);
      // 
      // titleLabel
      // 
      this.titleLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.titleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.titleLabel.Location = new System.Drawing.Point(3, 0);
      this.titleLabel.Name = "titleLabel";
      this.titleLabel.Size = new System.Drawing.Size(760, 30);
      this.titleLabel.TabIndex = 0;
      this.titleLabel.Text = "Title";
      this.titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // samplesGroupBox
      // 
      this.samplesGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.samplesGroupBox.Controls.Add(this.loadingPanel);
      this.samplesGroupBox.Controls.Add(this.samplesListView);
      this.samplesGroupBox.Location = new System.Drawing.Point(3, 358);
      this.samplesGroupBox.Name = "samplesGroupBox";
      this.samplesGroupBox.Size = new System.Drawing.Size(760, 178);
      this.samplesGroupBox.TabIndex = 2;
      this.samplesGroupBox.TabStop = false;
      this.samplesGroupBox.Text = "Samples";
      // 
      // loadingPanel
      // 
      this.loadingPanel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
      this.loadingPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.loadingPanel.Controls.Add(this.loadingProgressBar);
      this.loadingPanel.Controls.Add(this.loadingLabel);
      this.loadingPanel.Enabled = false;
      this.loadingPanel.Location = new System.Drawing.Point(205, 74);
      this.loadingPanel.Name = "loadingPanel";
      this.loadingPanel.Size = new System.Drawing.Size(350, 62);
      this.loadingPanel.TabIndex = 1;
      // 
      // loadingProgressBar
      // 
      this.loadingProgressBar.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.loadingProgressBar.Location = new System.Drawing.Point(101, 19);
      this.loadingProgressBar.Name = "loadingProgressBar";
      this.loadingProgressBar.Size = new System.Drawing.Size(229, 23);
      this.loadingProgressBar.Step = 1;
      this.loadingProgressBar.TabIndex = 1;
      // 
      // loadingLabel
      // 
      this.loadingLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.loadingLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.loadingLabel.Location = new System.Drawing.Point(12, 19);
      this.loadingLabel.Name = "loadingLabel";
      this.loadingLabel.Size = new System.Drawing.Size(83, 23);
      this.loadingLabel.TabIndex = 0;
      this.loadingLabel.Text = "Loading ...";
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
      this.samplesListView.Size = new System.Drawing.Size(754, 159);
      this.samplesListView.SmallImageList = this.imageList;
      this.samplesListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
      this.samplesListView.TabIndex = 0;
      this.samplesListView.UseCompatibleStateImageBehavior = false;
      this.samplesListView.View = System.Windows.Forms.View.Details;
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
      // StartPage
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.firstStepsRichTextBox);
      this.Controls.Add(this.samplesGroupBox);
      this.Controls.Add(this.titleLabel);
      this.Controls.Add(this.showStartPageCheckBox);
      this.Name = "StartPage";
      this.Size = new System.Drawing.Size(766, 562);
      this.samplesGroupBox.ResumeLayout(false);
      this.loadingPanel.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

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
    private System.Windows.Forms.Panel loadingPanel;
    private System.Windows.Forms.ProgressBar loadingProgressBar;
    private System.Windows.Forms.Label loadingLabel;
  }
}
