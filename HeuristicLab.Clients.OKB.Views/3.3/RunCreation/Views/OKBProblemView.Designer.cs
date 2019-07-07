#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Clients.OKB.RunCreation {
  partial class OKBProblemView {
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
      this.problemComboBox = new System.Windows.Forms.ComboBox();
      this.problemLabel = new System.Windows.Forms.Label();
      this.refreshButton = new System.Windows.Forms.Button();
      this.cloneProblemButton = new System.Windows.Forms.Button();
      this.tabControl = new System.Windows.Forms.TabControl();
      this.parametersTabPage = new System.Windows.Forms.TabPage();
      this.parameterCollectionView = new HeuristicLab.Core.Views.ParameterCollectionView();
      this.solutionsTabPage = new System.Windows.Forms.TabPage();
      this.flaAnalysisTabPage = new System.Windows.Forms.TabPage();
      this.flaSplitContainer = new System.Windows.Forms.SplitContainer();
      this.calculateButton = new System.Windows.Forms.Button();
      this.characteristicsMatrixView = new HeuristicLab.Data.Views.StringConvertibleMatrixView();
      this.uploadCharacteristicsButton = new System.Windows.Forms.Button();
      this.downloadCharacteristicsButton = new System.Windows.Forms.Button();
      this.solutionsViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.refreshSolutionsButton = new System.Windows.Forms.Button();
      this.uploadSolutionsButton = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.tabControl.SuspendLayout();
      this.parametersTabPage.SuspendLayout();
      this.solutionsTabPage.SuspendLayout();
      this.flaAnalysisTabPage.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.flaSplitContainer)).BeginInit();
      this.flaSplitContainer.Panel1.SuspendLayout();
      this.flaSplitContainer.Panel2.SuspendLayout();
      this.flaSplitContainer.SuspendLayout();
      this.SuspendLayout();
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Location = new System.Drawing.Point(72, 27);
      this.nameTextBox.Size = new System.Drawing.Size(640, 20);
      this.nameTextBox.TabIndex = 5;
      // 
      // nameLabel
      // 
      this.nameLabel.Location = new System.Drawing.Point(3, 30);
      this.nameLabel.TabIndex = 4;
      // 
      // infoLabel
      // 
      this.infoLabel.Location = new System.Drawing.Point(723, 30);
      this.infoLabel.TabIndex = 6;
      // 
      // problemComboBox
      // 
      this.problemComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.problemComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.problemComboBox.FormattingEnabled = true;
      this.problemComboBox.Location = new System.Drawing.Point(72, 0);
      this.problemComboBox.Name = "problemComboBox";
      this.problemComboBox.Size = new System.Drawing.Size(610, 21);
      this.problemComboBox.TabIndex = 1;
      this.problemComboBox.SelectedValueChanged += new System.EventHandler(this.problemComboBox_SelectedValueChanged);
      // 
      // problemLabel
      // 
      this.problemLabel.AutoSize = true;
      this.problemLabel.Location = new System.Drawing.Point(3, 3);
      this.problemLabel.Name = "problemLabel";
      this.problemLabel.Size = new System.Drawing.Size(48, 13);
      this.problemLabel.TabIndex = 0;
      this.problemLabel.Text = "&Problem:";
      // 
      // refreshButton
      // 
      this.refreshButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.refreshButton.Location = new System.Drawing.Point(718, -1);
      this.refreshButton.Name = "refreshButton";
      this.refreshButton.Size = new System.Drawing.Size(24, 24);
      this.refreshButton.TabIndex = 3;
      this.toolTip.SetToolTip(this.refreshButton, "Refresh Problems");
      this.refreshButton.UseVisualStyleBackColor = true;
      this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
      // 
      // cloneProblemButton
      // 
      this.cloneProblemButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.cloneProblemButton.Location = new System.Drawing.Point(688, -1);
      this.cloneProblemButton.Name = "cloneProblemButton";
      this.cloneProblemButton.Size = new System.Drawing.Size(24, 24);
      this.cloneProblemButton.TabIndex = 2;
      this.toolTip.SetToolTip(this.cloneProblemButton, "Clone Problem");
      this.cloneProblemButton.UseVisualStyleBackColor = true;
      this.cloneProblemButton.Click += new System.EventHandler(this.cloneProblemButton_Click);
      // 
      // tabControl
      // 
      this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl.Controls.Add(this.parametersTabPage);
      this.tabControl.Controls.Add(this.solutionsTabPage);
      this.tabControl.Controls.Add(this.flaAnalysisTabPage);
      this.tabControl.Location = new System.Drawing.Point(6, 53);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(736, 473);
      this.tabControl.TabIndex = 8;
      // 
      // parametersTabPage
      // 
      this.parametersTabPage.Controls.Add(this.parameterCollectionView);
      this.parametersTabPage.Location = new System.Drawing.Point(4, 22);
      this.parametersTabPage.Name = "parametersTabPage";
      this.parametersTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.parametersTabPage.Size = new System.Drawing.Size(728, 447);
      this.parametersTabPage.TabIndex = 0;
      this.parametersTabPage.Text = "Parameters";
      this.parametersTabPage.UseVisualStyleBackColor = true;
      // 
      // parameterCollectionView
      // 
      this.parameterCollectionView.AllowEditingOfHiddenParameters = false;
      this.parameterCollectionView.Caption = "ParameterCollection View";
      this.parameterCollectionView.Content = null;
      this.parameterCollectionView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.parameterCollectionView.Location = new System.Drawing.Point(3, 3);
      this.parameterCollectionView.Name = "parameterCollectionView";
      this.parameterCollectionView.ReadOnly = true;
      this.parameterCollectionView.ShowDetails = true;
      this.parameterCollectionView.Size = new System.Drawing.Size(722, 441);
      this.parameterCollectionView.TabIndex = 8;
      // 
      // solutionsTabPage
      // 
      this.solutionsTabPage.Controls.Add(this.uploadSolutionsButton);
      this.solutionsTabPage.Controls.Add(this.refreshSolutionsButton);
      this.solutionsTabPage.Controls.Add(this.solutionsViewHost);
      this.solutionsTabPage.Location = new System.Drawing.Point(4, 22);
      this.solutionsTabPage.Name = "solutionsTabPage";
      this.solutionsTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.solutionsTabPage.Size = new System.Drawing.Size(728, 447);
      this.solutionsTabPage.TabIndex = 3;
      this.solutionsTabPage.Text = "Solutions";
      this.solutionsTabPage.UseVisualStyleBackColor = true;
      // 
      // flaAnalysisTabPage
      // 
      this.flaAnalysisTabPage.Controls.Add(this.flaSplitContainer);
      this.flaAnalysisTabPage.Location = new System.Drawing.Point(4, 22);
      this.flaAnalysisTabPage.Name = "flaAnalysisTabPage";
      this.flaAnalysisTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.flaAnalysisTabPage.Size = new System.Drawing.Size(728, 447);
      this.flaAnalysisTabPage.TabIndex = 2;
      this.flaAnalysisTabPage.Text = "Fitness Landscape Analysis";
      this.flaAnalysisTabPage.UseVisualStyleBackColor = true;
      // 
      // flaSplitContainer
      // 
      this.flaSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.flaSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
      this.flaSplitContainer.Location = new System.Drawing.Point(3, 3);
      this.flaSplitContainer.Name = "flaSplitContainer";
      // 
      // flaSplitContainer.Panel1
      // 
      this.flaSplitContainer.Panel1.Controls.Add(this.calculateButton);
      // 
      // flaSplitContainer.Panel2
      // 
      this.flaSplitContainer.Panel2.Controls.Add(this.characteristicsMatrixView);
      this.flaSplitContainer.Panel2.Controls.Add(this.uploadCharacteristicsButton);
      this.flaSplitContainer.Panel2.Controls.Add(this.downloadCharacteristicsButton);
      this.flaSplitContainer.Size = new System.Drawing.Size(722, 441);
      this.flaSplitContainer.SplitterDistance = 363;
      this.flaSplitContainer.TabIndex = 7;
      // 
      // calculateButton
      // 
      this.calculateButton.Location = new System.Drawing.Point(3, 3);
      this.calculateButton.Name = "calculateButton";
      this.calculateButton.Size = new System.Drawing.Size(26, 23);
      this.calculateButton.TabIndex = 8;
      this.calculateButton.Text = "Calculate";
      this.calculateButton.UseVisualStyleBackColor = true;
      this.calculateButton.Click += new System.EventHandler(this.calculateButton_Click);
      // 
      // characteristicsMatrixView
      // 
      this.characteristicsMatrixView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.characteristicsMatrixView.Caption = "StringConvertibleMatrix View";
      this.characteristicsMatrixView.Content = null;
      this.characteristicsMatrixView.Location = new System.Drawing.Point(3, 32);
      this.characteristicsMatrixView.Name = "characteristicsMatrixView";
      this.characteristicsMatrixView.ReadOnly = false;
      this.characteristicsMatrixView.ShowRowsAndColumnsTextBox = false;
      this.characteristicsMatrixView.ShowStatisticalInformation = false;
      this.characteristicsMatrixView.Size = new System.Drawing.Size(349, 406);
      this.characteristicsMatrixView.TabIndex = 4;
      // 
      // uploadCharacteristicsButton
      // 
      this.uploadCharacteristicsButton.Location = new System.Drawing.Point(35, 3);
      this.uploadCharacteristicsButton.Name = "uploadCharacteristicsButton";
      this.uploadCharacteristicsButton.Size = new System.Drawing.Size(26, 23);
      this.uploadCharacteristicsButton.TabIndex = 5;
      this.uploadCharacteristicsButton.Text = "Upload";
      this.uploadCharacteristicsButton.UseVisualStyleBackColor = true;
      this.uploadCharacteristicsButton.Click += new System.EventHandler(this.uploadCharacteristicsButton_Click);
      // 
      // downloadCharacteristicsButton
      // 
      this.downloadCharacteristicsButton.Location = new System.Drawing.Point(3, 3);
      this.downloadCharacteristicsButton.Name = "downloadCharacteristicsButton";
      this.downloadCharacteristicsButton.Size = new System.Drawing.Size(26, 23);
      this.downloadCharacteristicsButton.TabIndex = 6;
      this.downloadCharacteristicsButton.Text = "Download";
      this.downloadCharacteristicsButton.UseVisualStyleBackColor = true;
      this.downloadCharacteristicsButton.Click += new System.EventHandler(this.downloadCharacteristicsButton_Click);
      // 
      // solutionsViewHost
      // 
      this.solutionsViewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.solutionsViewHost.Caption = "View";
      this.solutionsViewHost.Content = null;
      this.solutionsViewHost.Enabled = false;
      this.solutionsViewHost.Location = new System.Drawing.Point(0, 35);
      this.solutionsViewHost.Name = "solutionsViewHost";
      this.solutionsViewHost.ReadOnly = false;
      this.solutionsViewHost.Size = new System.Drawing.Size(728, 412);
      this.solutionsViewHost.TabIndex = 0;
      this.solutionsViewHost.ViewsLabelVisible = true;
      this.solutionsViewHost.ViewType = null;
      // 
      // refreshSolutionsButton
      // 
      this.refreshSolutionsButton.Location = new System.Drawing.Point(6, 6);
      this.refreshSolutionsButton.Name = "refreshSolutionsButton";
      this.refreshSolutionsButton.Size = new System.Drawing.Size(26, 23);
      this.refreshSolutionsButton.TabIndex = 1;
      this.refreshSolutionsButton.Text = "Refresh";
      this.refreshSolutionsButton.UseVisualStyleBackColor = true;
      this.refreshSolutionsButton.Click += new System.EventHandler(this.refreshSolutionsButton_Click);
      // 
      // uploadSolutionsButton
      // 
      this.uploadSolutionsButton.Location = new System.Drawing.Point(38, 6);
      this.uploadSolutionsButton.Name = "uploadSolutionsButton";
      this.uploadSolutionsButton.Size = new System.Drawing.Size(26, 23);
      this.uploadSolutionsButton.TabIndex = 1;
      this.uploadSolutionsButton.Text = "Upload";
      this.uploadSolutionsButton.UseVisualStyleBackColor = true;
      this.uploadSolutionsButton.Click += new System.EventHandler(this.uploadSolutionsButton_Click);
      // 
      // OKBProblemView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tabControl);
      this.Controls.Add(this.problemComboBox);
      this.Controls.Add(this.problemLabel);
      this.Controls.Add(this.cloneProblemButton);
      this.Controls.Add(this.refreshButton);
      this.Name = "OKBProblemView";
      this.Size = new System.Drawing.Size(742, 526);
      this.Controls.SetChildIndex(this.refreshButton, 0);
      this.Controls.SetChildIndex(this.cloneProblemButton, 0);
      this.Controls.SetChildIndex(this.problemLabel, 0);
      this.Controls.SetChildIndex(this.problemComboBox, 0);
      this.Controls.SetChildIndex(this.tabControl, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.infoLabel, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.tabControl.ResumeLayout(false);
      this.parametersTabPage.ResumeLayout(false);
      this.solutionsTabPage.ResumeLayout(false);
      this.flaAnalysisTabPage.ResumeLayout(false);
      this.flaSplitContainer.Panel1.ResumeLayout(false);
      this.flaSplitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.flaSplitContainer)).EndInit();
      this.flaSplitContainer.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ComboBox problemComboBox;
    private System.Windows.Forms.Label problemLabel;
    private System.Windows.Forms.Button refreshButton;
    private System.Windows.Forms.Button cloneProblemButton;
    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TabPage parametersTabPage;
    private Core.Views.ParameterCollectionView parameterCollectionView;
    private System.Windows.Forms.TabPage flaAnalysisTabPage;
    private System.Windows.Forms.Button calculateButton;
    private System.Windows.Forms.SplitContainer flaSplitContainer;
    private Data.Views.StringConvertibleMatrixView characteristicsMatrixView;
    private System.Windows.Forms.Button uploadCharacteristicsButton;
    private System.Windows.Forms.Button downloadCharacteristicsButton;
    private System.Windows.Forms.TabPage solutionsTabPage;
    private MainForm.WindowsForms.ViewHost solutionsViewHost;
    private System.Windows.Forms.Button uploadSolutionsButton;
    private System.Windows.Forms.Button refreshSolutionsButton;


  }
}
