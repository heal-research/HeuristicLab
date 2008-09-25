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

namespace HeuristicLab.SimOpt {
  partial class DoubleParameterBoundConstraintView {
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
      this.ubTextBox = new System.Windows.Forms.TextBox();
      this.lbTextBox = new System.Windows.Forms.TextBox();
      this.ubIncludedCheckBox = new System.Windows.Forms.CheckBox();
      this.lbIncludedCheckBox = new System.Windows.Forms.CheckBox();
      this.ubEnabledCheckBox = new System.Windows.Forms.CheckBox();
      this.lbEnabledCheckBox = new System.Windows.Forms.CheckBox();
      this.pmTextBox = new System.Windows.Forms.TextBox();
      this.pmLabel = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // ubTextBox
      // 
      this.ubTextBox.Location = new System.Drawing.Point(47, 107);
      this.ubTextBox.Name = "ubTextBox";
      this.ubTextBox.Size = new System.Drawing.Size(83, 20);
      this.ubTextBox.TabIndex = 31;
      this.ubTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      this.ubTextBox.Validated += new System.EventHandler(this.ubTextBox_Validated);
      this.ubTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.ubTextBox_Validating);
      // 
      // lbTextBox
      // 
      this.lbTextBox.Location = new System.Drawing.Point(47, 58);
      this.lbTextBox.Name = "lbTextBox";
      this.lbTextBox.Size = new System.Drawing.Size(83, 20);
      this.lbTextBox.TabIndex = 30;
      this.lbTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      this.lbTextBox.Validated += new System.EventHandler(this.lbTextBox_Validated);
      this.lbTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.lbTextBox_Validating);
      // 
      // ubIncludedCheckBox
      // 
      this.ubIncludedCheckBox.AutoSize = true;
      this.ubIncludedCheckBox.Location = new System.Drawing.Point(136, 109);
      this.ubIncludedCheckBox.Name = "ubIncludedCheckBox";
      this.ubIncludedCheckBox.Size = new System.Drawing.Size(73, 17);
      this.ubIncludedCheckBox.TabIndex = 29;
      this.ubIncludedCheckBox.Text = "Included?";
      this.ubIncludedCheckBox.UseVisualStyleBackColor = true;
      this.ubIncludedCheckBox.CheckedChanged += new System.EventHandler(this.ubIncludedCheckBox_CheckedChanged);
      // 
      // lbIncludedCheckBox
      // 
      this.lbIncludedCheckBox.AutoSize = true;
      this.lbIncludedCheckBox.Location = new System.Drawing.Point(136, 61);
      this.lbIncludedCheckBox.Name = "lbIncludedCheckBox";
      this.lbIncludedCheckBox.Size = new System.Drawing.Size(73, 17);
      this.lbIncludedCheckBox.TabIndex = 28;
      this.lbIncludedCheckBox.Text = "Included?";
      this.lbIncludedCheckBox.UseVisualStyleBackColor = true;
      this.lbIncludedCheckBox.CheckedChanged += new System.EventHandler(this.lbIncludedCheckBox_CheckedChanged);
      // 
      // ubEnabledCheckBox
      // 
      this.ubEnabledCheckBox.AutoSize = true;
      this.ubEnabledCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.ubEnabledCheckBox.Location = new System.Drawing.Point(3, 84);
      this.ubEnabledCheckBox.Name = "ubEnabledCheckBox";
      this.ubEnabledCheckBox.Size = new System.Drawing.Size(91, 17);
      this.ubEnabledCheckBox.TabIndex = 27;
      this.ubEnabledCheckBox.Text = "Upper bound:";
      this.ubEnabledCheckBox.UseVisualStyleBackColor = true;
      this.ubEnabledCheckBox.CheckedChanged += new System.EventHandler(this.ubEnabledCheckBox_CheckedChanged);
      // 
      // lbEnabledCheckBox
      // 
      this.lbEnabledCheckBox.AutoSize = true;
      this.lbEnabledCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.lbEnabledCheckBox.Location = new System.Drawing.Point(3, 35);
      this.lbEnabledCheckBox.Name = "lbEnabledCheckBox";
      this.lbEnabledCheckBox.Size = new System.Drawing.Size(91, 17);
      this.lbEnabledCheckBox.TabIndex = 26;
      this.lbEnabledCheckBox.Text = "Lower bound:";
      this.lbEnabledCheckBox.UseVisualStyleBackColor = true;
      this.lbEnabledCheckBox.CheckedChanged += new System.EventHandler(this.lbEnabledCheckBox_CheckedChanged);
      // 
      // pmTextBox
      // 
      this.pmTextBox.Location = new System.Drawing.Point(47, 7);
      this.pmTextBox.Name = "pmTextBox";
      this.pmTextBox.Size = new System.Drawing.Size(156, 20);
      this.pmTextBox.TabIndex = 32;
      this.pmTextBox.TextChanged += new System.EventHandler(this.pmTextBox_TextChanged);
      // 
      // pmLabel
      // 
      this.pmLabel.AutoSize = true;
      this.pmLabel.Location = new System.Drawing.Point(3, 10);
      this.pmLabel.Name = "pmLabel";
      this.pmLabel.Size = new System.Drawing.Size(38, 13);
      this.pmLabel.TabIndex = 33;
      this.pmLabel.Text = "Name:";
      // 
      // DoubleParameterBoundConstraintView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.pmLabel);
      this.Controls.Add(this.pmTextBox);
      this.Controls.Add(this.ubTextBox);
      this.Controls.Add(this.lbTextBox);
      this.Controls.Add(this.ubIncludedCheckBox);
      this.Controls.Add(this.lbIncludedCheckBox);
      this.Controls.Add(this.ubEnabledCheckBox);
      this.Controls.Add(this.lbEnabledCheckBox);
      this.Name = "DoubleParameterBoundConstraintView";
      this.Size = new System.Drawing.Size(206, 131);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox ubTextBox;
    private System.Windows.Forms.TextBox lbTextBox;
    private System.Windows.Forms.CheckBox ubIncludedCheckBox;
    private System.Windows.Forms.CheckBox lbIncludedCheckBox;
    private System.Windows.Forms.CheckBox ubEnabledCheckBox;
    private System.Windows.Forms.CheckBox lbEnabledCheckBox;
    private System.Windows.Forms.TextBox pmTextBox;
    private System.Windows.Forms.Label pmLabel;
  }
}
