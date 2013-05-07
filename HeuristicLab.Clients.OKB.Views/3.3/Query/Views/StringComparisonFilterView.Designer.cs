#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Clients.OKB.Query {
  partial class StringComparisonFilterView {
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
      this.comparisonComboBox = new System.Windows.Forms.ComboBox();
      this.valueTextBox = new System.Windows.Forms.TextBox();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
      this.splitContainer2.Panel1.SuspendLayout();
      this.splitContainer2.SuspendLayout();
      this.SuspendLayout();
      // 
      // splitContainer1
      // 
      this.splitContainer1.TabIndex = 0;
      // 
      // splitContainer2
      // 
      // 
      // splitContainer2.Panel1
      // 
      this.splitContainer2.Panel1.Controls.Add(this.comparisonComboBox);
      // 
      // splitContainer2.Panel2
      // 
      this.splitContainer2.Panel2.Controls.Add(this.valueTextBox);
      // 
      // comparisonComboBox
      // 
      this.comparisonComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.comparisonComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.comparisonComboBox.FormattingEnabled = true;
      this.comparisonComboBox.Items.AddRange(new object[] {
            "=",
            "<>",
            "contains",
            "not contains",
            "like",
            "not like"});
      this.comparisonComboBox.Location = new System.Drawing.Point(3, 2);
      this.comparisonComboBox.Name = "comparisonComboBox";
      this.comparisonComboBox.Size = new System.Drawing.Size(100, 21);
      this.comparisonComboBox.TabIndex = 0;
      this.comparisonComboBox.SelectedIndexChanged += new System.EventHandler(this.comparisonComboBox_SelectedIndexChanged);
      // 
      // valueTextBox
      // 
      this.valueTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.valueTextBox.Location = new System.Drawing.Point(3, 2);
      this.valueTextBox.Name = "valueTextBox";
      this.valueTextBox.Size = new System.Drawing.Size(280, 20);
      this.valueTextBox.TabIndex = 0;
      this.valueTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.valueTextBox_KeyDown);
      this.valueTextBox.Validated += new System.EventHandler(this.valueTextBox_Validated);
      // 
      // SetComparisonFilterView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Name = "SetComparisonFilterView";
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
      this.splitContainer1.ResumeLayout(false);
      this.splitContainer2.Panel1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
      this.splitContainer2.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ComboBox comparisonComboBox;
    private System.Windows.Forms.TextBox valueTextBox;

  }
}
