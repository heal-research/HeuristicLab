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

namespace HeuristicLab.Constraints {
  partial class VariableComparisonConstraintView {
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
      this.label1 = new System.Windows.Forms.Label();
      this.comparerGroupBox = new System.Windows.Forms.GroupBox();
      this.greaterRadioButton = new System.Windows.Forms.RadioButton();
      this.greaterOrEqualRadioButton = new System.Windows.Forms.RadioButton();
      this.equalRadioButton = new System.Windows.Forms.RadioButton();
      this.lessOrEqualRadioButton = new System.Windows.Forms.RadioButton();
      this.lessRadioButton = new System.Windows.Forms.RadioButton();
      this.label2 = new System.Windows.Forms.Label();
      this.rightVarNameStringDataView = new HeuristicLab.Data.StringDataView();
      this.leftVarNameStringDataView = new HeuristicLab.Data.StringDataView();
      this.comparerGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(3, 5);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(25, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Left";
      // 
      // comparerGroupBox
      // 
      this.comparerGroupBox.Controls.Add(this.greaterRadioButton);
      this.comparerGroupBox.Controls.Add(this.greaterOrEqualRadioButton);
      this.comparerGroupBox.Controls.Add(this.equalRadioButton);
      this.comparerGroupBox.Controls.Add(this.lessOrEqualRadioButton);
      this.comparerGroupBox.Controls.Add(this.lessRadioButton);
      this.comparerGroupBox.Location = new System.Drawing.Point(3, 54);
      this.comparerGroupBox.Name = "comparerGroupBox";
      this.comparerGroupBox.Size = new System.Drawing.Size(141, 137);
      this.comparerGroupBox.TabIndex = 4;
      this.comparerGroupBox.TabStop = false;
      this.comparerGroupBox.Text = "Comparer";
      // 
      // greaterRadioButton
      // 
      this.greaterRadioButton.AutoSize = true;
      this.greaterRadioButton.Location = new System.Drawing.Point(9, 111);
      this.greaterRadioButton.Name = "greaterRadioButton";
      this.greaterRadioButton.Size = new System.Drawing.Size(31, 17);
      this.greaterRadioButton.TabIndex = 4;
      this.greaterRadioButton.Text = ">";
      this.greaterRadioButton.UseVisualStyleBackColor = true;
      this.greaterRadioButton.CheckedChanged += new System.EventHandler(this.anyRadioButton_CheckedChanged);
      // 
      // greaterOrEqualRadioButton
      // 
      this.greaterOrEqualRadioButton.AutoSize = true;
      this.greaterOrEqualRadioButton.Location = new System.Drawing.Point(9, 88);
      this.greaterOrEqualRadioButton.Name = "greaterOrEqualRadioButton";
      this.greaterOrEqualRadioButton.Size = new System.Drawing.Size(37, 17);
      this.greaterOrEqualRadioButton.TabIndex = 3;
      this.greaterOrEqualRadioButton.Text = ">=";
      this.greaterOrEqualRadioButton.UseVisualStyleBackColor = true;
      this.greaterOrEqualRadioButton.CheckedChanged += new System.EventHandler(this.anyRadioButton_CheckedChanged);
      // 
      // equalRadioButton
      // 
      this.equalRadioButton.AutoSize = true;
      this.equalRadioButton.Location = new System.Drawing.Point(9, 65);
      this.equalRadioButton.Name = "equalRadioButton";
      this.equalRadioButton.Size = new System.Drawing.Size(31, 17);
      this.equalRadioButton.TabIndex = 2;
      this.equalRadioButton.Text = "=";
      this.equalRadioButton.UseVisualStyleBackColor = true;
      this.equalRadioButton.CheckedChanged += new System.EventHandler(this.anyRadioButton_CheckedChanged);
      // 
      // lessOrEqualRadioButton
      // 
      this.lessOrEqualRadioButton.AutoSize = true;
      this.lessOrEqualRadioButton.Location = new System.Drawing.Point(9, 42);
      this.lessOrEqualRadioButton.Name = "lessOrEqualRadioButton";
      this.lessOrEqualRadioButton.Size = new System.Drawing.Size(37, 17);
      this.lessOrEqualRadioButton.TabIndex = 1;
      this.lessOrEqualRadioButton.Text = "<=";
      this.lessOrEqualRadioButton.UseVisualStyleBackColor = true;
      this.lessOrEqualRadioButton.CheckedChanged += new System.EventHandler(this.anyRadioButton_CheckedChanged);
      // 
      // lessRadioButton
      // 
      this.lessRadioButton.AutoSize = true;
      this.lessRadioButton.Location = new System.Drawing.Point(9, 19);
      this.lessRadioButton.Name = "lessRadioButton";
      this.lessRadioButton.Size = new System.Drawing.Size(31, 17);
      this.lessRadioButton.TabIndex = 0;
      this.lessRadioButton.Text = "<";
      this.lessRadioButton.UseVisualStyleBackColor = true;
      this.lessRadioButton.CheckedChanged += new System.EventHandler(this.anyRadioButton_CheckedChanged);
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(3, 31);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(32, 13);
      this.label2.TabIndex = 2;
      this.label2.Text = "Right";
      // 
      // rightVarNameStringDataView
      // 
      this.rightVarNameStringDataView.Caption = "View";
      this.rightVarNameStringDataView.Location = new System.Drawing.Point(44, 28);
      this.rightVarNameStringDataView.Name = "rightVarNameStringDataView";
      this.rightVarNameStringDataView.Size = new System.Drawing.Size(100, 20);
      this.rightVarNameStringDataView.StringData = null;
      this.rightVarNameStringDataView.TabIndex = 3;
      // 
      // leftVarNameStringDataView
      // 
      this.leftVarNameStringDataView.Caption = "View";
      this.leftVarNameStringDataView.Location = new System.Drawing.Point(44, 2);
      this.leftVarNameStringDataView.Name = "leftVarNameStringDataView";
      this.leftVarNameStringDataView.Size = new System.Drawing.Size(100, 20);
      this.leftVarNameStringDataView.StringData = null;
      this.leftVarNameStringDataView.TabIndex = 1;
      // 
      // VariableComparisonConstraintView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.leftVarNameStringDataView);
      this.Controls.Add(this.rightVarNameStringDataView);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.comparerGroupBox);
      this.Controls.Add(this.label1);
      this.Name = "VariableComparisonConstraintView";
      this.Size = new System.Drawing.Size(147, 193);
      this.comparerGroupBox.ResumeLayout(false);
      this.comparerGroupBox.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.GroupBox comparerGroupBox;
    private System.Windows.Forms.RadioButton greaterRadioButton;
    private System.Windows.Forms.RadioButton greaterOrEqualRadioButton;
    private System.Windows.Forms.RadioButton equalRadioButton;
    private System.Windows.Forms.RadioButton lessOrEqualRadioButton;
    private System.Windows.Forms.RadioButton lessRadioButton;
    private System.Windows.Forms.Label label2;
    private HeuristicLab.Data.StringDataView rightVarNameStringDataView;
    private HeuristicLab.Data.StringDataView leftVarNameStringDataView;
  }
}
