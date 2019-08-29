#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views {
  partial class SymbolicExpressionGrammarSampleExpressionTreeView {
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
      this.sampleTreeGroupBox = new System.Windows.Forms.GroupBox();
      this.sampleTreeView = new HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views.GraphicalSymbolicExpressionTreeView();
      this.maxTreeLengthLabel = new System.Windows.Forms.Label();
      this.maxTreeDepthLabel = new System.Windows.Forms.Label();
      this.maxTreeLengthTextBox = new System.Windows.Forms.TextBox();
      this.maxTreeDepthTextBox = new System.Windows.Forms.TextBox();
      this.generateSampleTreeButton = new System.Windows.Forms.Button();
      this.treeCreatorComboBox = new System.Windows.Forms.ComboBox();
      this.treeCreatorLabel = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.sampleTreeGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Location = new System.Drawing.Point(93, 0);
      this.nameTextBox.Size = new System.Drawing.Size(319, 20);
      // 
      // infoLabel
      // 
      this.infoLabel.Location = new System.Drawing.Point(418, 3);
      // 
      // sampleTreeGroupBox
      // 
      this.sampleTreeGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.sampleTreeGroupBox.Controls.Add(this.sampleTreeView);
      this.sampleTreeGroupBox.Location = new System.Drawing.Point(3, 134);
      this.sampleTreeGroupBox.Name = "sampleTreeGroupBox";
      this.sampleTreeGroupBox.Size = new System.Drawing.Size(431, 265);
      this.sampleTreeGroupBox.TabIndex = 3;
      this.sampleTreeGroupBox.TabStop = false;
      this.sampleTreeGroupBox.Text = "Sample SymbolicExpressionTree";
      // 
      // sampleTreeView
      // 
      this.sampleTreeView.AllowDrop = true;
      this.sampleTreeView.Caption = "Graphical SymbolicExpressionTree View";
      this.sampleTreeView.Content = null;
      this.sampleTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.sampleTreeView.Location = new System.Drawing.Point(3, 16);
      this.sampleTreeView.Name = "sampleTreeView";
      this.sampleTreeView.ReadOnly = false;
      this.sampleTreeView.Size = new System.Drawing.Size(425, 246);
      this.sampleTreeView.TabIndex = 0;
      // 
      // maxTreeLengthLabel
      // 
      this.maxTreeLengthLabel.AutoSize = true;
      this.maxTreeLengthLabel.Location = new System.Drawing.Point(3, 29);
      this.maxTreeLengthLabel.Name = "maxTreeLengthLabel";
      this.maxTreeLengthLabel.Size = new System.Drawing.Size(88, 13);
      this.maxTreeLengthLabel.TabIndex = 4;
      this.maxTreeLengthLabel.Text = "Max TreeLength:";
      // 
      // maxTreeDepthLabel
      // 
      this.maxTreeDepthLabel.AutoSize = true;
      this.maxTreeDepthLabel.Location = new System.Drawing.Point(3, 55);
      this.maxTreeDepthLabel.Name = "maxTreeDepthLabel";
      this.maxTreeDepthLabel.Size = new System.Drawing.Size(84, 13);
      this.maxTreeDepthLabel.TabIndex = 5;
      this.maxTreeDepthLabel.Text = "Max TreeDepth:";
      // 
      // maxTreeLengthTextBox
      // 
      this.maxTreeLengthTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.maxTreeLengthTextBox.Location = new System.Drawing.Point(93, 26);
      this.maxTreeLengthTextBox.Name = "maxTreeLengthTextBox";
      this.maxTreeLengthTextBox.Size = new System.Drawing.Size(341, 20);
      this.maxTreeLengthTextBox.TabIndex = 6;
      this.maxTreeLengthTextBox.Text = "50";
      this.maxTreeLengthTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.maxTreeLengthTextBox_Validating);
      this.maxTreeLengthTextBox.Validated += new System.EventHandler(this.maxTreeLengthTextBox_Validated);
      // 
      // maxTreeDepthTextBox
      // 
      this.maxTreeDepthTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.maxTreeDepthTextBox.Location = new System.Drawing.Point(93, 52);
      this.maxTreeDepthTextBox.Name = "maxTreeDepthTextBox";
      this.maxTreeDepthTextBox.Size = new System.Drawing.Size(341, 20);
      this.maxTreeDepthTextBox.TabIndex = 7;
      this.maxTreeDepthTextBox.Text = "10";
      this.maxTreeDepthTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.maxTreeDepthTextBox_Validating);
      this.maxTreeDepthTextBox.Validated += new System.EventHandler(this.maxTreeDepthTextBox_Validated);
      // 
      // generateSampleTreeButton
      // 
      this.generateSampleTreeButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.generateSampleTreeButton.Location = new System.Drawing.Point(6, 105);
      this.generateSampleTreeButton.Name = "generateSampleTreeButton";
      this.generateSampleTreeButton.Size = new System.Drawing.Size(431, 23);
      this.generateSampleTreeButton.TabIndex = 8;
      this.generateSampleTreeButton.Text = "Generate SymbolicExpressionTree";
      this.generateSampleTreeButton.UseVisualStyleBackColor = true;
      this.generateSampleTreeButton.Click += new System.EventHandler(this.generateSampleTreeButton_Click);
      // 
      // treeCreatorComboBox
      // 
      this.treeCreatorComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.treeCreatorComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.treeCreatorComboBox.FormattingEnabled = true;
      this.treeCreatorComboBox.Location = new System.Drawing.Point(93, 78);
      this.treeCreatorComboBox.Name = "treeCreatorComboBox";
      this.treeCreatorComboBox.Size = new System.Drawing.Size(341, 21);
      this.treeCreatorComboBox.TabIndex = 9;
      this.treeCreatorComboBox.SelectedIndexChanged += new System.EventHandler(this.treeCreatorComboBox_SelectedIndexChanged);
      // 
      // treeCreatorLabel
      // 
      this.treeCreatorLabel.AutoSize = true;
      this.treeCreatorLabel.Location = new System.Drawing.Point(3, 81);
      this.treeCreatorLabel.Name = "treeCreatorLabel";
      this.treeCreatorLabel.Size = new System.Drawing.Size(69, 13);
      this.treeCreatorLabel.TabIndex = 10;
      this.treeCreatorLabel.Text = "Tree Creator:";
      // 
      // SymbolicExpressionGrammarSampleExpressionTreeView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.treeCreatorLabel);
      this.Controls.Add(this.sampleTreeGroupBox);
      this.Controls.Add(this.maxTreeDepthLabel);
      this.Controls.Add(this.maxTreeLengthTextBox);
      this.Controls.Add(this.treeCreatorComboBox);
      this.Controls.Add(this.maxTreeLengthLabel);
      this.Controls.Add(this.maxTreeDepthTextBox);
      this.Controls.Add(this.generateSampleTreeButton);
      this.Name = "SymbolicExpressionGrammarSampleExpressionTreeView";
      this.Size = new System.Drawing.Size(437, 402);
      this.Controls.SetChildIndex(this.generateSampleTreeButton, 0);
      this.Controls.SetChildIndex(this.maxTreeDepthTextBox, 0);
      this.Controls.SetChildIndex(this.maxTreeLengthLabel, 0);
      this.Controls.SetChildIndex(this.treeCreatorComboBox, 0);
      this.Controls.SetChildIndex(this.maxTreeLengthTextBox, 0);
      this.Controls.SetChildIndex(this.maxTreeDepthLabel, 0);
      this.Controls.SetChildIndex(this.sampleTreeGroupBox, 0);
      this.Controls.SetChildIndex(this.treeCreatorLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      this.Controls.SetChildIndex(this.infoLabel, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.sampleTreeGroupBox.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.GroupBox sampleTreeGroupBox;
    private System.Windows.Forms.Label maxTreeLengthLabel;
    private System.Windows.Forms.Label maxTreeDepthLabel;
    private System.Windows.Forms.TextBox maxTreeLengthTextBox;
    private System.Windows.Forms.TextBox maxTreeDepthTextBox;
    private System.Windows.Forms.Button generateSampleTreeButton;
    private GraphicalSymbolicExpressionTreeView sampleTreeView;
    private System.Windows.Forms.ComboBox treeCreatorComboBox;
    private System.Windows.Forms.Label treeCreatorLabel;
  }
}
