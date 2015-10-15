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

namespace HeuristicLab.Optimization.Views {
  partial class ProblemView {
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
      this.problemInstanceSplitContainer = new System.Windows.Forms.SplitContainer();
      this.libraryLabel = new System.Windows.Forms.Label();
      this.problemInstanceProviderComboBox = new System.Windows.Forms.ComboBox();
      this.problemInstanceProviderViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.problemInstanceSplitContainer)).BeginInit();
      this.problemInstanceSplitContainer.Panel1.SuspendLayout();
      this.problemInstanceSplitContainer.Panel2.SuspendLayout();
      this.problemInstanceSplitContainer.SuspendLayout();
      this.SuspendLayout();
      // 
      // parameterCollectionView
      // 
      this.parameterCollectionView.EnableBreadcrumbs = true;
      this.parameterCollectionView.Location = new System.Drawing.Point(6, 27);
      this.parameterCollectionView.Size = new System.Drawing.Size(501, 303);
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Location = new System.Drawing.Point(47, 1);
      this.nameTextBox.Size = new System.Drawing.Size(438, 20);
      // 
      // nameLabel
      // 
      this.nameLabel.Location = new System.Drawing.Point(3, 6);
      // 
      // infoLabel
      // 
      this.infoLabel.Location = new System.Drawing.Point(491, 4);
      // 
      // problemInstanceSplitContainer
      // 
      this.problemInstanceSplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.problemInstanceSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
      this.problemInstanceSplitContainer.IsSplitterFixed = true;
      this.problemInstanceSplitContainer.Location = new System.Drawing.Point(0, 0);
      this.problemInstanceSplitContainer.Name = "problemInstanceSplitContainer";
      this.problemInstanceSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // problemInstanceSplitContainer.Panel1
      // 
      this.problemInstanceSplitContainer.Panel1.Controls.Add(this.libraryLabel);
      this.problemInstanceSplitContainer.Panel1.Controls.Add(this.problemInstanceProviderComboBox);
      this.problemInstanceSplitContainer.Panel1.Controls.Add(this.problemInstanceProviderViewHost);
      this.problemInstanceSplitContainer.Panel1MinSize = 10;
      // 
      // problemInstanceSplitContainer.Panel2
      // 
      this.problemInstanceSplitContainer.Panel2.Controls.Add(this.nameLabel);
      this.problemInstanceSplitContainer.Panel2.Controls.Add(this.nameTextBox);
      this.problemInstanceSplitContainer.Panel2.Controls.Add(this.parameterCollectionView);
      this.problemInstanceSplitContainer.Panel2.Controls.Add(this.infoLabel);
      this.problemInstanceSplitContainer.Size = new System.Drawing.Size(511, 363);
      this.problemInstanceSplitContainer.SplitterDistance = 26;
      this.problemInstanceSplitContainer.TabIndex = 13;
      // 
      // libraryLabel
      // 
      this.libraryLabel.AutoSize = true;
      this.libraryLabel.Location = new System.Drawing.Point(3, 6);
      this.libraryLabel.Name = "libraryLabel";
      this.libraryLabel.Size = new System.Drawing.Size(41, 13);
      this.libraryLabel.TabIndex = 17;
      this.libraryLabel.Text = "Library:";
      // 
      // problemInstanceProviderComboBox
      // 
      this.problemInstanceProviderComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.problemInstanceProviderComboBox.FormattingEnabled = true;
      this.problemInstanceProviderComboBox.Location = new System.Drawing.Point(50, 2);
      this.problemInstanceProviderComboBox.Name = "problemInstanceProviderComboBox";
      this.problemInstanceProviderComboBox.Size = new System.Drawing.Size(208, 21);
      this.problemInstanceProviderComboBox.TabIndex = 18;
      this.problemInstanceProviderComboBox.SelectedIndexChanged += new System.EventHandler(this.problemInstanceProviderComboBox_SelectedIndexChanged);
      // 
      // problemInstanceProviderViewHost
      // 
      this.problemInstanceProviderViewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.problemInstanceProviderViewHost.Caption = "ProblemInstanceConsumerView";
      this.problemInstanceProviderViewHost.Content = null;
      this.problemInstanceProviderViewHost.Enabled = false;
      this.problemInstanceProviderViewHost.Location = new System.Drawing.Point(264, 1);
      this.problemInstanceProviderViewHost.Name = "problemInstanceProviderViewHost";
      this.problemInstanceProviderViewHost.ReadOnly = false;
      this.problemInstanceProviderViewHost.Size = new System.Drawing.Size(247, 23);
      this.problemInstanceProviderViewHost.TabIndex = 0;
      this.problemInstanceProviderViewHost.ViewsLabelVisible = false;
      this.problemInstanceProviderViewHost.ViewType = null;
      // 
      // ProblemView
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.problemInstanceSplitContainer);
      this.Name = "ProblemView";
      this.Size = new System.Drawing.Size(511, 363);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.problemInstanceSplitContainer.Panel1.ResumeLayout(false);
      this.problemInstanceSplitContainer.Panel1.PerformLayout();
      this.problemInstanceSplitContainer.Panel2.ResumeLayout(false);
      this.problemInstanceSplitContainer.Panel2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.problemInstanceSplitContainer)).EndInit();
      this.problemInstanceSplitContainer.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    protected System.Windows.Forms.SplitContainer problemInstanceSplitContainer;
    protected HeuristicLab.MainForm.WindowsForms.ViewHost problemInstanceProviderViewHost;
    protected System.Windows.Forms.Label libraryLabel;
    protected System.Windows.Forms.ComboBox problemInstanceProviderComboBox;
  }
}
