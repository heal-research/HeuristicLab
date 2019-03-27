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

namespace HeuristicLab.Parameters.Views {
  partial class ScopeTreeLookupParameterView<T> {
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
      this.depthTextBox = new System.Windows.Forms.TextBox();
      this.depthLabel = new System.Windows.Forms.Label();
      this.actualNameTextBox = new System.Windows.Forms.TextBox();
      this.actualNameLabel = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // dataTypeLabel
      // 
      this.dataTypeLabel.Location = new System.Drawing.Point(3, 55);
      this.dataTypeLabel.TabIndex = 5;
      // 
      // dataTypeTextBox
      // 
      this.dataTypeTextBox.Location = new System.Drawing.Point(80, 52);
      this.dataTypeTextBox.Size = new System.Drawing.Size(306, 20);
      this.dataTypeTextBox.TabIndex = 6;
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Location = new System.Drawing.Point(80, 0);
      this.nameTextBox.Size = new System.Drawing.Size(281, 20);
      // 
      // infoLabel
      // 
      this.infoLabel.Location = new System.Drawing.Point(367, 3);
      // 
      // depthTextBox
      // 
      this.depthTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.errorProvider.SetIconAlignment(this.depthTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.depthTextBox, 2);
      this.depthTextBox.Location = new System.Drawing.Point(80, 78);
      this.depthTextBox.Name = "depthTextBox";
      this.depthTextBox.Size = new System.Drawing.Size(306, 20);
      this.depthTextBox.TabIndex = 8;
      this.depthTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.depthTextBox_KeyDown);
      this.depthTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.depthTextBox_Validating);
      this.depthTextBox.Validated += new System.EventHandler(this.depthTextBox_Validated);
      // 
      // depthLabel
      // 
      this.depthLabel.AutoSize = true;
      this.depthLabel.Location = new System.Drawing.Point(3, 81);
      this.depthLabel.Name = "depthLabel";
      this.depthLabel.Size = new System.Drawing.Size(39, 13);
      this.depthLabel.TabIndex = 7;
      this.depthLabel.Text = "&Depth:";
      // 
      // actualNameTextBox
      // 
      this.actualNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.actualNameTextBox.Location = new System.Drawing.Point(80, 26);
      this.actualNameTextBox.Name = "actualNameTextBox";
      this.actualNameTextBox.Size = new System.Drawing.Size(306, 20);
      this.actualNameTextBox.TabIndex = 4;
      this.actualNameTextBox.Validated += new System.EventHandler(this.actualNameTextBox_Validated);
      // 
      // actualNameLabel
      // 
      this.actualNameLabel.AutoSize = true;
      this.actualNameLabel.Location = new System.Drawing.Point(3, 29);
      this.actualNameLabel.Name = "actualNameLabel";
      this.actualNameLabel.Size = new System.Drawing.Size(71, 13);
      this.actualNameLabel.TabIndex = 3;
      this.actualNameLabel.Text = "&Actual Name:";
      // 
      // ScopeTreeLookupParameterView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.actualNameTextBox);
      this.Controls.Add(this.depthLabel);
      this.Controls.Add(this.depthTextBox);
      this.Controls.Add(this.actualNameLabel);
      this.Name = "ScopeTreeLookupParameterView";
      this.Size = new System.Drawing.Size(386, 103);
      this.Controls.SetChildIndex(this.dataTypeTextBox, 0);
      this.Controls.SetChildIndex(this.actualNameLabel, 0);
      this.Controls.SetChildIndex(this.depthTextBox, 0);
      this.Controls.SetChildIndex(this.dataTypeLabel, 0);
      this.Controls.SetChildIndex(this.infoLabel, 0);
      this.Controls.SetChildIndex(this.depthLabel, 0);
      this.Controls.SetChildIndex(this.actualNameTextBox, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    protected System.Windows.Forms.TextBox depthTextBox;
    protected System.Windows.Forms.Label depthLabel;
    protected System.Windows.Forms.TextBox actualNameTextBox;
    protected System.Windows.Forms.Label actualNameLabel;
  }
}
