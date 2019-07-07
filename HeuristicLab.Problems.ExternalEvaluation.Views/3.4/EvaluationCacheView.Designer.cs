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

namespace HeuristicLab.Problems.ExternalEvaluation.Views {
  partial class EvaluationCacheView {
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
      this.hits_sizeLabel = new System.Windows.Forms.Label();
      this.hits_sizeTextBox = new System.Windows.Forms.TextBox();
      this.clearButton = new System.Windows.Forms.Button();
      this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
      this.saveButton = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // parameterCollectionView
      // 
      this.parameterCollectionView.Location = new System.Drawing.Point(0, 52);
      this.parameterCollectionView.Size = new System.Drawing.Size(490, 301);
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      // 
      // hits_sizeLabel
      // 
      this.hits_sizeLabel.AutoSize = true;
      this.hits_sizeLabel.Location = new System.Drawing.Point(3, 29);
      this.hits_sizeLabel.Name = "hits_sizeLabel";
      this.hits_sizeLabel.Size = new System.Drawing.Size(53, 13);
      this.hits_sizeLabel.TabIndex = 4;
      this.hits_sizeLabel.Text = "Hits/Size:";
      // 
      // hits_sizeTextBox
      // 
      this.hits_sizeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.hits_sizeTextBox.Location = new System.Drawing.Point(59, 26);
      this.hits_sizeTextBox.Name = "hits_sizeTextBox";
      this.hits_sizeTextBox.ReadOnly = true;
      this.hits_sizeTextBox.Size = new System.Drawing.Size(319, 20);
      this.hits_sizeTextBox.TabIndex = 5;
      // 
      // clearButton
      // 
      this.clearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.clearButton.Location = new System.Drawing.Point(444, 24);
      this.clearButton.Name = "clearButton";
      this.clearButton.Size = new System.Drawing.Size(43, 23);
      this.clearButton.TabIndex = 6;
      this.clearButton.Text = "Clear";
      this.toolTip.SetToolTip(this.clearButton, "Clear");
      this.clearButton.UseVisualStyleBackColor = true;
      this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
      // 
      // saveFileDialog
      // 
      this.saveFileDialog.DefaultExt = "csv";
      this.saveFileDialog.Filter = "CSV Files|*.csv|All Files|*.*";
      this.saveFileDialog.RestoreDirectory = true;
      this.saveFileDialog.Title = "Save Cache";
      // 
      // saveButton
      // 
      this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.saveButton.Location = new System.Drawing.Point(384, 23);
      this.saveButton.Name = "saveButton";
      this.saveButton.Size = new System.Drawing.Size(54, 23);
      this.saveButton.TabIndex = 8;
      this.saveButton.Text = "Save...";
      this.saveButton.UseVisualStyleBackColor = true;
      this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
      // 
      // EvaluationCacheView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.hits_sizeTextBox);
      this.Controls.Add(this.hits_sizeLabel);
      this.Controls.Add(this.clearButton);
      this.Controls.Add(this.saveButton);
      this.Name = "EvaluationCacheView";
      this.Controls.SetChildIndex(this.saveButton, 0);
      this.Controls.SetChildIndex(this.clearButton, 0);
      this.Controls.SetChildIndex(this.hits_sizeLabel, 0);
      this.Controls.SetChildIndex(this.hits_sizeTextBox, 0);
      this.Controls.SetChildIndex(this.infoLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.parameterCollectionView, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label hits_sizeLabel;
    private System.Windows.Forms.TextBox hits_sizeTextBox;
    private System.Windows.Forms.Button clearButton;
    private System.Windows.Forms.SaveFileDialog saveFileDialog;
    private System.Windows.Forms.Button saveButton;
  }
}
