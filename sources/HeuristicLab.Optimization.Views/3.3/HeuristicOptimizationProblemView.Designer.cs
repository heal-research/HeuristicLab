#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  partial class HeuristicOptimizationProblemView {
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
      this.libraryLabel = new System.Windows.Forms.Label();
      this.problemInstanceProviderViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.problemInstanceProviderComboBox = new System.Windows.Forms.ComboBox();
      this.libraryInfoButton = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // parameterCollectionView
      // 
      this.parameterCollectionView.Location = new System.Drawing.Point(0, 58);
      this.parameterCollectionView.Size = new System.Drawing.Size(650, 394);
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Location = new System.Drawing.Point(50, 32);
      this.nameTextBox.Size = new System.Drawing.Size(578, 20);
      // 
      // nameLabel
      // 
      this.nameLabel.Location = new System.Drawing.Point(3, 35);
      // 
      // toolTip
      // 
      this.toolTip.AutoPopDelay = 10000;
      this.toolTip.InitialDelay = 100;
      this.toolTip.ReshowDelay = 100;
      // 
      // infoLabel
      // 
      this.infoLabel.Location = new System.Drawing.Point(634, 35);
      // 
      // libraryLabel
      // 
      this.libraryLabel.AutoSize = true;
      this.libraryLabel.Location = new System.Drawing.Point(3, 8);
      this.libraryLabel.Name = "libraryLabel";
      this.libraryLabel.Size = new System.Drawing.Size(41, 13);
      this.libraryLabel.TabIndex = 4;
      this.libraryLabel.Text = "Library:";
      // 
      // problemInstanceProviderViewHost
      // 
      this.problemInstanceProviderViewHost.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.problemInstanceProviderViewHost.Caption = "View";
      this.problemInstanceProviderViewHost.Content = null;
      this.problemInstanceProviderViewHost.Enabled = false;
      this.problemInstanceProviderViewHost.Location = new System.Drawing.Point(259, 5);
      this.problemInstanceProviderViewHost.Name = "problemInstanceProviderViewHost";
      this.problemInstanceProviderViewHost.ReadOnly = false;
      this.problemInstanceProviderViewHost.Size = new System.Drawing.Size(391, 21);
      this.problemInstanceProviderViewHost.TabIndex = 6;
      this.problemInstanceProviderViewHost.ViewsLabelVisible = false;
      this.problemInstanceProviderViewHost.ViewType = null;
      // 
      // problemInstanceProviderComboBox
      // 
      this.problemInstanceProviderComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.problemInstanceProviderComboBox.FormattingEnabled = true;
      this.problemInstanceProviderComboBox.Location = new System.Drawing.Point(50, 5);
      this.problemInstanceProviderComboBox.Name = "problemInstanceProviderComboBox";
      this.problemInstanceProviderComboBox.Size = new System.Drawing.Size(171, 21);
      this.problemInstanceProviderComboBox.TabIndex = 7;
      this.problemInstanceProviderComboBox.SelectedIndexChanged += new System.EventHandler(this.problemInstanceProviderComboBox_SelectedIndexChanged);
      this.problemInstanceProviderComboBox.DataSourceChanged += new System.EventHandler(this.comboBox_DataSourceChanged);
      // 
      // libraryInfoButton
      // 
      this.libraryInfoButton.Location = new System.Drawing.Point(227, 4);
      this.libraryInfoButton.Name = "libraryInfoButton";
      this.libraryInfoButton.Size = new System.Drawing.Size(26, 23);
      this.libraryInfoButton.TabIndex = 8;
      this.libraryInfoButton.Text = "Info";
      this.libraryInfoButton.UseVisualStyleBackColor = true;
      this.libraryInfoButton.Click += new System.EventHandler(this.libraryInfoButton_Click);
      // 
      // ProblemView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.problemInstanceProviderViewHost);
      this.Controls.Add(this.problemInstanceProviderComboBox);
      this.Controls.Add(this.libraryLabel);
      this.Controls.Add(this.libraryInfoButton);
      this.Name = "ProblemView";
      this.Size = new System.Drawing.Size(653, 455);
      this.Controls.SetChildIndex(this.libraryInfoButton, 0);
      this.Controls.SetChildIndex(this.libraryLabel, 0);
      this.Controls.SetChildIndex(this.problemInstanceProviderComboBox, 0);
      this.Controls.SetChildIndex(this.problemInstanceProviderViewHost, 0);
      this.Controls.SetChildIndex(this.infoLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.parameterCollectionView, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    protected System.Windows.Forms.Label libraryLabel;
    protected System.Windows.Forms.ComboBox problemInstanceProviderComboBox;
    protected MainForm.WindowsForms.ViewHost problemInstanceProviderViewHost;
    protected System.Windows.Forms.Button libraryInfoButton;

  }
}
