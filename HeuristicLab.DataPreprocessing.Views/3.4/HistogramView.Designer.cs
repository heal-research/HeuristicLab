#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.DataPreprocessing.Views {
  partial class HistogramView {
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
      this.optionsBox = new System.Windows.Forms.GroupBox();
      this.displayDetailsCheckBox = new System.Windows.Forms.CheckBox();
      this.label1 = new System.Windows.Forms.Label();
      this.classifierComboBox = new System.Windows.Forms.ComboBox();
      this.optionsBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // optionsBox
      // 
      this.optionsBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.optionsBox.Controls.Add(this.displayDetailsCheckBox);
      this.optionsBox.Controls.Add(this.label1);
      this.optionsBox.Controls.Add(this.classifierComboBox);
      this.optionsBox.Location = new System.Drawing.Point(5, 324);
      this.optionsBox.Margin = new System.Windows.Forms.Padding(4);
      this.optionsBox.Name = "optionsBox";
      this.optionsBox.Padding = new System.Windows.Forms.Padding(4);
      this.optionsBox.Size = new System.Drawing.Size(203, 165);
      this.optionsBox.TabIndex = 7;
      this.optionsBox.TabStop = false;
      this.optionsBox.Text = "Options";
      // 
      // displayDetailsCheckBox
      // 
      this.displayDetailsCheckBox.AutoSize = true;
      this.displayDetailsCheckBox.Location = new System.Drawing.Point(7, 71);
      this.displayDetailsCheckBox.Name = "displayDetailsCheckBox";
      this.displayDetailsCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
      this.displayDetailsCheckBox.Size = new System.Drawing.Size(153, 21);
      this.displayDetailsCheckBox.TabIndex = 3;
      this.displayDetailsCheckBox.Text = "Display value count";
      this.displayDetailsCheckBox.UseVisualStyleBackColor = true;
      this.displayDetailsCheckBox.CheckedChanged += new System.EventHandler(this.displayDetailsCheckBox_CheckedChanged);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(4, 19);
      this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(108, 17);
      this.label1.TabIndex = 2;
      this.label1.Text = "Target variable:";
      // 
      // classifierComboBox
      // 
      this.classifierComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.classifierComboBox.FormattingEnabled = true;
      this.classifierComboBox.Location = new System.Drawing.Point(7, 40);
      this.classifierComboBox.Margin = new System.Windows.Forms.Padding(4);
      this.classifierComboBox.Name = "classifierComboBox";
      this.classifierComboBox.Size = new System.Drawing.Size(160, 24);
      this.classifierComboBox.TabIndex = 1;
      this.classifierComboBox.SelectedIndexChanged += new System.EventHandler(this.classifierComboBox_SelectedIndexChanged);
      // 
      // HistogramView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.optionsBox);
      this.Margin = new System.Windows.Forms.Padding(5);
      this.Name = "HistogramView";
      this.Controls.SetChildIndex(this.optionsBox, 0);
      this.optionsBox.ResumeLayout(false);
      this.optionsBox.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox optionsBox;
    private System.Windows.Forms.ComboBox classifierComboBox;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.CheckBox displayDetailsCheckBox;

  }
}
