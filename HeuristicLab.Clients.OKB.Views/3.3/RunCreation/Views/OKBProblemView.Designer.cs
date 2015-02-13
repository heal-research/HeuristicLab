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
      this.parameterCollectionView = new HeuristicLab.Core.Views.ParameterCollectionView();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Location = new System.Drawing.Point(72, 27);
      this.nameTextBox.Size = new System.Drawing.Size(604, 20);
      this.nameTextBox.TabIndex = 5;
      // 
      // nameLabel
      // 
      this.nameLabel.Location = new System.Drawing.Point(3, 30);
      this.nameLabel.TabIndex = 4;
      // 
      // infoLabel
      // 
      this.infoLabel.Location = new System.Drawing.Point(687, 30);
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
      this.problemComboBox.Size = new System.Drawing.Size(574, 21);
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
      this.refreshButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Refresh;
      this.refreshButton.Location = new System.Drawing.Point(682, -1);
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
      this.cloneProblemButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Clone;
      this.cloneProblemButton.Location = new System.Drawing.Point(652, -1);
      this.cloneProblemButton.Name = "cloneProblemButton";
      this.cloneProblemButton.Size = new System.Drawing.Size(24, 24);
      this.cloneProblemButton.TabIndex = 2;
      this.toolTip.SetToolTip(this.cloneProblemButton, "Clone Problem");
      this.cloneProblemButton.UseVisualStyleBackColor = true;
      this.cloneProblemButton.Click += new System.EventHandler(this.cloneProblemButton_Click);
      // 
      // parameterCollectionView
      // 
      this.parameterCollectionView.AllowEditingOfHiddenParameters = false;
      this.parameterCollectionView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.parameterCollectionView.Caption = "ParameterCollection View";
      this.parameterCollectionView.Content = null;
      this.parameterCollectionView.Location = new System.Drawing.Point(0, 53);
      this.parameterCollectionView.Name = "parameterCollectionView";
      this.parameterCollectionView.ReadOnly = false;
      this.parameterCollectionView.Size = new System.Drawing.Size(706, 340);
      this.parameterCollectionView.TabIndex = 7;
      // 
      // OKBProblemView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.problemComboBox);
      this.Controls.Add(this.parameterCollectionView);
      this.Controls.Add(this.problemLabel);
      this.Controls.Add(this.cloneProblemButton);
      this.Controls.Add(this.refreshButton);
      this.Name = "OKBProblemView";
      this.Size = new System.Drawing.Size(706, 393);
      this.Controls.SetChildIndex(this.refreshButton, 0);
      this.Controls.SetChildIndex(this.cloneProblemButton, 0);
      this.Controls.SetChildIndex(this.problemLabel, 0);
      this.Controls.SetChildIndex(this.parameterCollectionView, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.infoLabel, 0);
      this.Controls.SetChildIndex(this.problemComboBox, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ComboBox problemComboBox;
    private System.Windows.Forms.Label problemLabel;
    private System.Windows.Forms.Button refreshButton;
    private System.Windows.Forms.Button cloneProblemButton;
    private HeuristicLab.Core.Views.ParameterCollectionView parameterCollectionView;


  }
}
