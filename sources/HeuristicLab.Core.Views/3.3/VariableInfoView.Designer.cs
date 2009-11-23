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
  partial class VariableInfoView {
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
      this.actualNameLabel = new System.Windows.Forms.Label();
      this.actualNameTextBox = new System.Windows.Forms.TextBox();
      this.dataTypeLabel = new System.Windows.Forms.Label();
      this.dataTypeTextBox = new System.Windows.Forms.TextBox();
      this.descriptionLabel = new System.Windows.Forms.Label();
      this.descriptionTextBox = new System.Windows.Forms.TextBox();
      this.kindLabel = new System.Windows.Forms.Label();
      this.kindTextBox = new System.Windows.Forms.TextBox();
      this.formalNameTextBox = new System.Windows.Forms.TextBox();
      this.formalNameLabel = new System.Windows.Forms.Label();
      this.localLabel = new System.Windows.Forms.Label();
      this.localCheckBox = new System.Windows.Forms.CheckBox();
      this.SuspendLayout();
      // 
      // actualNameLabel
      // 
      this.actualNameLabel.AutoSize = true;
      this.actualNameLabel.Location = new System.Drawing.Point(3, 3);
      this.actualNameLabel.Name = "actualNameLabel";
      this.actualNameLabel.Size = new System.Drawing.Size(71, 13);
      this.actualNameLabel.TabIndex = 0;
      this.actualNameLabel.Text = "&Actual Name:";
      // 
      // actualNameTextBox
      // 
      this.actualNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.actualNameTextBox.Location = new System.Drawing.Point(81, 0);
      this.actualNameTextBox.Name = "actualNameTextBox";
      this.actualNameTextBox.Size = new System.Drawing.Size(188, 20);
      this.actualNameTextBox.TabIndex = 1;
      this.actualNameTextBox.Validated += new System.EventHandler(this.actualNameTextBox_Validated);
      // 
      // dataTypeLabel
      // 
      this.dataTypeLabel.AutoSize = true;
      this.dataTypeLabel.Location = new System.Drawing.Point(3, 55);
      this.dataTypeLabel.Name = "dataTypeLabel";
      this.dataTypeLabel.Size = new System.Drawing.Size(34, 13);
      this.dataTypeLabel.TabIndex = 4;
      this.dataTypeLabel.Text = "&Type:";
      // 
      // dataTypeTextBox
      // 
      this.dataTypeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.dataTypeTextBox.Location = new System.Drawing.Point(81, 52);
      this.dataTypeTextBox.Name = "dataTypeTextBox";
      this.dataTypeTextBox.ReadOnly = true;
      this.dataTypeTextBox.Size = new System.Drawing.Size(188, 20);
      this.dataTypeTextBox.TabIndex = 5;
      // 
      // descriptionLabel
      // 
      this.descriptionLabel.AutoSize = true;
      this.descriptionLabel.Location = new System.Drawing.Point(3, 127);
      this.descriptionLabel.Name = "descriptionLabel";
      this.descriptionLabel.Size = new System.Drawing.Size(63, 13);
      this.descriptionLabel.TabIndex = 10;
      this.descriptionLabel.Text = "&Description:";
      // 
      // descriptionTextBox
      // 
      this.descriptionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.descriptionTextBox.Location = new System.Drawing.Point(81, 124);
      this.descriptionTextBox.Multiline = true;
      this.descriptionTextBox.Name = "descriptionTextBox";
      this.descriptionTextBox.ReadOnly = true;
      this.descriptionTextBox.Size = new System.Drawing.Size(188, 85);
      this.descriptionTextBox.TabIndex = 11;
      this.descriptionTextBox.Validated += new System.EventHandler(this.actualNameTextBox_Validated);
      // 
      // kindLabel
      // 
      this.kindLabel.AutoSize = true;
      this.kindLabel.Location = new System.Drawing.Point(3, 81);
      this.kindLabel.Name = "kindLabel";
      this.kindLabel.Size = new System.Drawing.Size(31, 13);
      this.kindLabel.TabIndex = 6;
      this.kindLabel.Text = "&Kind:";
      // 
      // kindTextBox
      // 
      this.kindTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.kindTextBox.Location = new System.Drawing.Point(81, 78);
      this.kindTextBox.Name = "kindTextBox";
      this.kindTextBox.ReadOnly = true;
      this.kindTextBox.Size = new System.Drawing.Size(188, 20);
      this.kindTextBox.TabIndex = 7;
      this.kindTextBox.Validated += new System.EventHandler(this.actualNameTextBox_Validated);
      // 
      // formalNameTextBox
      // 
      this.formalNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.formalNameTextBox.Location = new System.Drawing.Point(81, 26);
      this.formalNameTextBox.Name = "formalNameTextBox";
      this.formalNameTextBox.ReadOnly = true;
      this.formalNameTextBox.Size = new System.Drawing.Size(188, 20);
      this.formalNameTextBox.TabIndex = 3;
      // 
      // formalNameLabel
      // 
      this.formalNameLabel.AutoSize = true;
      this.formalNameLabel.Location = new System.Drawing.Point(3, 29);
      this.formalNameLabel.Name = "formalNameLabel";
      this.formalNameLabel.Size = new System.Drawing.Size(72, 13);
      this.formalNameLabel.TabIndex = 2;
      this.formalNameLabel.Text = "&Formal Name:";
      // 
      // localLabel
      // 
      this.localLabel.AutoSize = true;
      this.localLabel.Location = new System.Drawing.Point(3, 104);
      this.localLabel.Name = "localLabel";
      this.localLabel.Size = new System.Drawing.Size(36, 13);
      this.localLabel.TabIndex = 8;
      this.localLabel.Text = "&Local:";
      // 
      // localCheckBox
      // 
      this.localCheckBox.AutoSize = true;
      this.localCheckBox.Location = new System.Drawing.Point(81, 104);
      this.localCheckBox.Name = "localCheckBox";
      this.localCheckBox.Size = new System.Drawing.Size(15, 14);
      this.localCheckBox.TabIndex = 9;
      this.localCheckBox.UseVisualStyleBackColor = true;
      this.localCheckBox.CheckedChanged += new System.EventHandler(this.localCheckBox_CheckedChanged);
      // 
      // VariableInfoEditor
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.localCheckBox);
      this.Controls.Add(this.dataTypeTextBox);
      this.Controls.Add(this.kindTextBox);
      this.Controls.Add(this.descriptionTextBox);
      this.Controls.Add(this.formalNameTextBox);
      this.Controls.Add(this.actualNameTextBox);
      this.Controls.Add(this.dataTypeLabel);
      this.Controls.Add(this.kindLabel);
      this.Controls.Add(this.formalNameLabel);
      this.Controls.Add(this.localLabel);
      this.Controls.Add(this.descriptionLabel);
      this.Controls.Add(this.actualNameLabel);
      this.Name = "VariableInfoEditor";
      this.Size = new System.Drawing.Size(269, 209);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label actualNameLabel;
    private System.Windows.Forms.TextBox actualNameTextBox;
    private System.Windows.Forms.Label dataTypeLabel;
    private System.Windows.Forms.TextBox dataTypeTextBox;
    private System.Windows.Forms.Label descriptionLabel;
    private System.Windows.Forms.TextBox descriptionTextBox;
    private System.Windows.Forms.Label kindLabel;
    private System.Windows.Forms.TextBox kindTextBox;
    private System.Windows.Forms.TextBox formalNameTextBox;
    private System.Windows.Forms.Label formalNameLabel;
    private System.Windows.Forms.Label localLabel;
    private System.Windows.Forms.CheckBox localCheckBox;
  }
}
