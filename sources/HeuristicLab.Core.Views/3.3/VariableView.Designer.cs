#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Core.Views {
  partial class VariableView {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (chooseItemDialog != null) chooseItemDialog.Dispose();
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
      this.nameLabel = new System.Windows.Forms.Label();
      this.nameTextBox = new System.Windows.Forms.TextBox();
      this.valueLabel = new System.Windows.Forms.Label();
      this.valueTextBox = new System.Windows.Forms.TextBox();
      this.editorGroupBox = new System.Windows.Forms.GroupBox();
      this.setVariableValueButton = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // nameLabel
      // 
      this.nameLabel.AutoSize = true;
      this.nameLabel.Location = new System.Drawing.Point(3, 3);
      this.nameLabel.Name = "nameLabel";
      this.nameLabel.Size = new System.Drawing.Size(38, 13);
      this.nameLabel.TabIndex = 0;
      this.nameLabel.Text = "&Name:";
      // 
      // nameTextBox
      // 
      this.nameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.nameTextBox.Location = new System.Drawing.Point(50, 0);
      this.nameTextBox.Name = "nameTextBox";
      this.nameTextBox.Size = new System.Drawing.Size(153, 20);
      this.nameTextBox.TabIndex = 1;
      this.nameTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.nameTextBox_Validating);
      // 
      // valueLabel
      // 
      this.valueLabel.AutoSize = true;
      this.valueLabel.Location = new System.Drawing.Point(3, 29);
      this.valueLabel.Name = "valueLabel";
      this.valueLabel.Size = new System.Drawing.Size(37, 13);
      this.valueLabel.TabIndex = 2;
      this.valueLabel.Text = "&Value:";
      // 
      // valueTextBox
      // 
      this.valueTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.valueTextBox.Location = new System.Drawing.Point(50, 26);
      this.valueTextBox.Name = "valueTextBox";
      this.valueTextBox.ReadOnly = true;
      this.valueTextBox.Size = new System.Drawing.Size(99, 20);
      this.valueTextBox.TabIndex = 3;
      // 
      // editorGroupBox
      // 
      this.editorGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.editorGroupBox.Location = new System.Drawing.Point(0, 52);
      this.editorGroupBox.Name = "editorGroupBox";
      this.editorGroupBox.Size = new System.Drawing.Size(203, 102);
      this.editorGroupBox.TabIndex = 5;
      this.editorGroupBox.TabStop = false;
      this.editorGroupBox.Text = "&Editor:";
      // 
      // setVariableValueButton
      // 
      this.setVariableValueButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.setVariableValueButton.Location = new System.Drawing.Point(155, 26);
      this.setVariableValueButton.Name = "setVariableValueButton";
      this.setVariableValueButton.Size = new System.Drawing.Size(45, 20);
      this.setVariableValueButton.TabIndex = 4;
      this.setVariableValueButton.Text = "Set...";
      this.setVariableValueButton.UseVisualStyleBackColor = true;
      this.setVariableValueButton.Click += new System.EventHandler(this.setVariableValueButton_Click);
      // 
      // VariableView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.setVariableValueButton);
      this.Controls.Add(this.editorGroupBox);
      this.Controls.Add(this.valueTextBox);
      this.Controls.Add(this.nameTextBox);
      this.Controls.Add(this.valueLabel);
      this.Controls.Add(this.nameLabel);
      this.Name = "VariableView";
      this.Size = new System.Drawing.Size(203, 154);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label nameLabel;
    private System.Windows.Forms.TextBox nameTextBox;
    private System.Windows.Forms.Label valueLabel;
    private System.Windows.Forms.TextBox valueTextBox;
    private System.Windows.Forms.GroupBox editorGroupBox;
    private System.Windows.Forms.Button setVariableValueButton;
  }
}
