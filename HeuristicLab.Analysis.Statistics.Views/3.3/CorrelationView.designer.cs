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

namespace HeuristicLab.Analysis.Statistics.Views {
  partial class CorrelationView {
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

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.stringConvertibleMatrixView = new HeuristicLab.Data.Views.EnhancedStringConvertibleMatrixView();
      this.label1 = new System.Windows.Forms.Label();
      this.methodComboBox = new System.Windows.Forms.ComboBox();
      this.SuspendLayout();
      // 
      // stringConvertibleMatrixView
      // 
      this.stringConvertibleMatrixView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.stringConvertibleMatrixView.Caption = "StringConvertibleMatrix View";
      this.stringConvertibleMatrixView.Content = null;
      this.stringConvertibleMatrixView.Location = new System.Drawing.Point(3, 30);
      this.stringConvertibleMatrixView.Maximum = 0D;
      this.stringConvertibleMatrixView.Minimum = 0D;
      this.stringConvertibleMatrixView.Name = "stringConvertibleMatrixView";
      this.stringConvertibleMatrixView.ReadOnly = true;
      this.stringConvertibleMatrixView.ShowRowsAndColumnsTextBox = true;
      this.stringConvertibleMatrixView.ShowStatisticalInformation = true;
      this.stringConvertibleMatrixView.Size = new System.Drawing.Size(492, 352);
      this.stringConvertibleMatrixView.TabIndex = 0;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(5, 6);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(46, 13);
      this.label1.TabIndex = 6;
      this.label1.Text = "Method:";
      // 
      // methodComboBox
      // 
      this.methodComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.methodComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.methodComboBox.FormattingEnabled = true;
      this.methodComboBox.Location = new System.Drawing.Point(71, 3);
      this.methodComboBox.Name = "methodComboBox";
      this.methodComboBox.Size = new System.Drawing.Size(424, 21);
      this.methodComboBox.TabIndex = 5;
      this.methodComboBox.SelectedIndexChanged += new System.EventHandler(this.methodComboBox_SelectedIndexChanged);
      // 
      // CorrelationView
      // 
      this.Controls.Add(this.label1);
      this.Controls.Add(this.methodComboBox);
      this.Controls.Add(this.stringConvertibleMatrixView);
      this.Name = "CorrelationView";
      this.Size = new System.Drawing.Size(498, 385);
      this.ResumeLayout(false);
      this.PerformLayout();

    }
    #endregion

    private HeuristicLab.Data.Views.EnhancedStringConvertibleMatrixView stringConvertibleMatrixView;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.ComboBox methodComboBox;
  }
}
