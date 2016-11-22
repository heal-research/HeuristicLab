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

namespace HeuristicLab.Clients.OKB.Query {
  partial class QueryView {
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
      this.refreshResultsButton = new System.Windows.Forms.Button();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.runCollectionView = new HeuristicLab.Optimization.Views.RunCollectionView();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.filterTabControl = new System.Windows.Forms.TabControl();
      this.filterTabPage = new System.Windows.Forms.TabPage();
      this.filtersGroupBox = new System.Windows.Forms.GroupBox();
      this.filterPanel = new System.Windows.Forms.Panel();
      this.refreshFiltersButton = new System.Windows.Forms.Button();
      this.constraintsTabPage = new System.Windows.Forms.TabPage();
      this.deselectAllButton = new System.Windows.Forms.Button();
      this.selectAllButton = new System.Windows.Forms.Button();
      this.constraintsCheckedListBox = new System.Windows.Forms.CheckedListBox();
      this.resultsGroupBox = new System.Windows.Forms.GroupBox();
      this.includeBinaryValuesCheckBox = new System.Windows.Forms.CheckBox();
      this.resultsInfoPanel = new System.Windows.Forms.Panel();
      this.abortButton = new System.Windows.Forms.Button();
      this.resultsProgressBar = new System.Windows.Forms.ProgressBar();
      this.resultsInfoLabel = new System.Windows.Forms.Label();
      this.filtersInfoPanel = new System.Windows.Forms.Panel();
      this.filtersInfoLabel = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.filterTabControl.SuspendLayout();
      this.filterTabPage.SuspendLayout();
      this.filtersGroupBox.SuspendLayout();
      this.constraintsTabPage.SuspendLayout();
      this.resultsGroupBox.SuspendLayout();
      this.resultsInfoPanel.SuspendLayout();
      this.filtersInfoPanel.SuspendLayout();
      this.SuspendLayout();
      // 
      // refreshResultsButton
      // 
      this.refreshResultsButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Refresh;
      this.refreshResultsButton.Location = new System.Drawing.Point(6, 19);
      this.refreshResultsButton.Name = "refreshResultsButton";
      this.refreshResultsButton.Size = new System.Drawing.Size(24, 24);
      this.refreshResultsButton.TabIndex = 0;
      this.toolTip.SetToolTip(this.refreshResultsButton, "Refresh Results");
      this.refreshResultsButton.UseVisualStyleBackColor = true;
      this.refreshResultsButton.Click += new System.EventHandler(this.refreshResultsButton_Click);
      // 
      // runCollectionView
      // 
      this.runCollectionView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.runCollectionView.Caption = "RunCollection View";
      this.runCollectionView.Content = null;
      this.runCollectionView.Location = new System.Drawing.Point(6, 49);
      this.runCollectionView.Name = "runCollectionView";
      this.runCollectionView.ReadOnly = false;
      this.runCollectionView.Size = new System.Drawing.Size(826, 223);
      this.runCollectionView.TabIndex = 3;
      // 
      // splitContainer
      // 
      this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.splitContainer.Location = new System.Drawing.Point(0, 0);
      this.splitContainer.Name = "splitContainer";
      this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.filterTabControl);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.resultsGroupBox);
      this.splitContainer.Size = new System.Drawing.Size(838, 567);
      this.splitContainer.SplitterDistance = 282;
      this.splitContainer.TabIndex = 0;
      // 
      // filterTabControl
      // 
      this.filterTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.filterTabControl.Controls.Add(this.filterTabPage);
      this.filterTabControl.Controls.Add(this.constraintsTabPage);
      this.filterTabControl.Location = new System.Drawing.Point(3, 3);
      this.filterTabControl.Name = "filterTabControl";
      this.filterTabControl.SelectedIndex = 0;
      this.filterTabControl.Size = new System.Drawing.Size(832, 276);
      this.filterTabControl.TabIndex = 1;
      // 
      // filterTabPage
      // 
      this.filterTabPage.Controls.Add(this.filtersGroupBox);
      this.filterTabPage.Location = new System.Drawing.Point(4, 22);
      this.filterTabPage.Name = "filterTabPage";
      this.filterTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.filterTabPage.Size = new System.Drawing.Size(824, 250);
      this.filterTabPage.TabIndex = 0;
      this.filterTabPage.Text = "Filters";
      this.filterTabPage.UseVisualStyleBackColor = true;
      // 
      // filtersGroupBox
      // 
      this.filtersGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.filtersGroupBox.Controls.Add(this.filterPanel);
      this.filtersGroupBox.Controls.Add(this.refreshFiltersButton);
      this.filtersGroupBox.Location = new System.Drawing.Point(6, 6);
      this.filtersGroupBox.Name = "filtersGroupBox";
      this.filtersGroupBox.Size = new System.Drawing.Size(812, 238);
      this.filtersGroupBox.TabIndex = 0;
      this.filtersGroupBox.TabStop = false;
      this.filtersGroupBox.Text = "Filters";
      // 
      // filterPanel
      // 
      this.filterPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.filterPanel.Location = new System.Drawing.Point(6, 19);
      this.filterPanel.Name = "filterPanel";
      this.filterPanel.Size = new System.Drawing.Size(770, 213);
      this.filterPanel.TabIndex = 1;
      // 
      // refreshFiltersButton
      // 
      this.refreshFiltersButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.refreshFiltersButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Refresh;
      this.refreshFiltersButton.Location = new System.Drawing.Point(782, 21);
      this.refreshFiltersButton.Name = "refreshFiltersButton";
      this.refreshFiltersButton.Size = new System.Drawing.Size(24, 24);
      this.refreshFiltersButton.TabIndex = 0;
      this.refreshFiltersButton.UseVisualStyleBackColor = true;
      this.refreshFiltersButton.Click += new System.EventHandler(this.refreshFiltersButton_Click);
      // 
      // constraintsTabPage
      // 
      this.constraintsTabPage.Controls.Add(this.deselectAllButton);
      this.constraintsTabPage.Controls.Add(this.selectAllButton);
      this.constraintsTabPage.Controls.Add(this.constraintsCheckedListBox);
      this.constraintsTabPage.Location = new System.Drawing.Point(4, 22);
      this.constraintsTabPage.Name = "constraintsTabPage";
      this.constraintsTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.constraintsTabPage.Size = new System.Drawing.Size(824, 250);
      this.constraintsTabPage.TabIndex = 1;
      this.constraintsTabPage.Text = "Limit Downloaded Values";
      this.constraintsTabPage.UseVisualStyleBackColor = true;
      // 
      // deselectAllButton
      // 
      this.deselectAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.deselectAllButton.Location = new System.Drawing.Point(88, 221);
      this.deselectAllButton.Name = "deselectAllButton";
      this.deselectAllButton.Size = new System.Drawing.Size(75, 23);
      this.deselectAllButton.TabIndex = 2;
      this.deselectAllButton.Text = "Deselect all";
      this.deselectAllButton.UseVisualStyleBackColor = true;
      this.deselectAllButton.Click += new System.EventHandler(this.deselectAllButton_Click);
      // 
      // selectAllButton
      // 
      this.selectAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.selectAllButton.Location = new System.Drawing.Point(6, 221);
      this.selectAllButton.Name = "selectAllButton";
      this.selectAllButton.Size = new System.Drawing.Size(75, 23);
      this.selectAllButton.TabIndex = 1;
      this.selectAllButton.Text = "Select all";
      this.selectAllButton.UseVisualStyleBackColor = true;
      this.selectAllButton.Click += new System.EventHandler(this.selectAllButton_Click);
      // 
      // constraintsCheckedListBox
      // 
      this.constraintsCheckedListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.constraintsCheckedListBox.CheckOnClick = true;
      this.constraintsCheckedListBox.FormattingEnabled = true;
      this.constraintsCheckedListBox.Location = new System.Drawing.Point(6, 6);
      this.constraintsCheckedListBox.Name = "constraintsCheckedListBox";
      this.constraintsCheckedListBox.Size = new System.Drawing.Size(812, 214);
      this.constraintsCheckedListBox.Sorted = true;
      this.constraintsCheckedListBox.TabIndex = 0;
      // 
      // resultsGroupBox
      // 
      this.resultsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.resultsGroupBox.Controls.Add(this.includeBinaryValuesCheckBox);
      this.resultsGroupBox.Controls.Add(this.refreshResultsButton);
      this.resultsGroupBox.Controls.Add(this.runCollectionView);
      this.resultsGroupBox.Location = new System.Drawing.Point(0, 3);
      this.resultsGroupBox.Name = "resultsGroupBox";
      this.resultsGroupBox.Size = new System.Drawing.Size(838, 278);
      this.resultsGroupBox.TabIndex = 0;
      this.resultsGroupBox.TabStop = false;
      this.resultsGroupBox.Text = "Results";
      // 
      // includeBinaryValuesCheckBox
      // 
      this.includeBinaryValuesCheckBox.AutoSize = true;
      this.includeBinaryValuesCheckBox.Checked = true;
      this.includeBinaryValuesCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
      this.includeBinaryValuesCheckBox.Location = new System.Drawing.Point(53, 24);
      this.includeBinaryValuesCheckBox.Name = "includeBinaryValuesCheckBox";
      this.includeBinaryValuesCheckBox.Size = new System.Drawing.Size(128, 17);
      this.includeBinaryValuesCheckBox.TabIndex = 1;
      this.includeBinaryValuesCheckBox.Text = "&Include Binary Values";
      this.includeBinaryValuesCheckBox.UseVisualStyleBackColor = true;
      // 
      // resultsInfoPanel
      // 
      this.resultsInfoPanel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
      this.resultsInfoPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.resultsInfoPanel.Controls.Add(this.abortButton);
      this.resultsInfoPanel.Controls.Add(this.resultsProgressBar);
      this.resultsInfoPanel.Controls.Add(this.resultsInfoLabel);
      this.resultsInfoPanel.Location = new System.Drawing.Point(238, 339);
      this.resultsInfoPanel.Name = "resultsInfoPanel";
      this.resultsInfoPanel.Size = new System.Drawing.Size(362, 87);
      this.resultsInfoPanel.TabIndex = 1;
      this.resultsInfoPanel.Visible = false;
      // 
      // abortButton
      // 
      this.abortButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.abortButton.Location = new System.Drawing.Point(3, 58);
      this.abortButton.Name = "abortButton";
      this.abortButton.Size = new System.Drawing.Size(354, 23);
      this.abortButton.TabIndex = 2;
      this.abortButton.Text = "&Abort";
      this.abortButton.UseVisualStyleBackColor = true;
      this.abortButton.Click += new System.EventHandler(this.abortButton_Click);
      // 
      // resultsProgressBar
      // 
      this.resultsProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.resultsProgressBar.Enabled = false;
      this.resultsProgressBar.Location = new System.Drawing.Point(3, 29);
      this.resultsProgressBar.Name = "resultsProgressBar";
      this.resultsProgressBar.Size = new System.Drawing.Size(354, 23);
      this.resultsProgressBar.Step = 1;
      this.resultsProgressBar.TabIndex = 1;
      // 
      // resultsInfoLabel
      // 
      this.resultsInfoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.resultsInfoLabel.Enabled = false;
      this.resultsInfoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.resultsInfoLabel.Location = new System.Drawing.Point(3, 3);
      this.resultsInfoLabel.Name = "resultsInfoLabel";
      this.resultsInfoLabel.Size = new System.Drawing.Size(354, 23);
      this.resultsInfoLabel.TabIndex = 0;
      this.resultsInfoLabel.Text = "Loading ...";
      this.resultsInfoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // filtersInfoPanel
      // 
      this.filtersInfoPanel.Anchor = System.Windows.Forms.AnchorStyles.Top;
      this.filtersInfoPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.filtersInfoPanel.Controls.Add(this.filtersInfoLabel);
      this.filtersInfoPanel.Enabled = false;
      this.filtersInfoPanel.Location = new System.Drawing.Point(238, 136);
      this.filtersInfoPanel.Name = "filtersInfoPanel";
      this.filtersInfoPanel.Size = new System.Drawing.Size(362, 33);
      this.filtersInfoPanel.TabIndex = 0;
      this.filtersInfoPanel.Visible = false;
      // 
      // filtersInfoLabel
      // 
      this.filtersInfoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.filtersInfoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.filtersInfoLabel.Location = new System.Drawing.Point(3, 4);
      this.filtersInfoLabel.Name = "filtersInfoLabel";
      this.filtersInfoLabel.Size = new System.Drawing.Size(354, 23);
      this.filtersInfoLabel.TabIndex = 0;
      this.filtersInfoLabel.Text = "Loading Filters ...";
      this.filtersInfoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // QueryView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.filtersInfoPanel);
      this.Controls.Add(this.resultsInfoPanel);
      this.Controls.Add(this.splitContainer);
      this.Name = "QueryView";
      this.Size = new System.Drawing.Size(838, 567);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.filterTabControl.ResumeLayout(false);
      this.filterTabPage.ResumeLayout(false);
      this.filtersGroupBox.ResumeLayout(false);
      this.constraintsTabPage.ResumeLayout(false);
      this.resultsGroupBox.ResumeLayout(false);
      this.resultsGroupBox.PerformLayout();
      this.resultsInfoPanel.ResumeLayout(false);
      this.filtersInfoPanel.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button refreshResultsButton;
    private System.Windows.Forms.ToolTip toolTip;
    private HeuristicLab.Optimization.Views.RunCollectionView runCollectionView;
    private System.Windows.Forms.SplitContainer splitContainer;
    private System.Windows.Forms.GroupBox filtersGroupBox;
    private System.Windows.Forms.GroupBox resultsGroupBox;
    private System.Windows.Forms.Panel filterPanel;
    private System.Windows.Forms.Panel resultsInfoPanel;
    private System.Windows.Forms.ProgressBar resultsProgressBar;
    private System.Windows.Forms.Label resultsInfoLabel;
    private System.Windows.Forms.Button abortButton;
    private System.Windows.Forms.Panel filtersInfoPanel;
    private System.Windows.Forms.Label filtersInfoLabel;
    private System.Windows.Forms.Button refreshFiltersButton;
    private System.Windows.Forms.CheckBox includeBinaryValuesCheckBox;
    private System.Windows.Forms.TabControl filterTabControl;
    private System.Windows.Forms.TabPage filterTabPage;
    private System.Windows.Forms.TabPage constraintsTabPage;
    private System.Windows.Forms.Button deselectAllButton;
    private System.Windows.Forms.Button selectAllButton;
    private System.Windows.Forms.CheckedListBox constraintsCheckedListBox;

  }
}
