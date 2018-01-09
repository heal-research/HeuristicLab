#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Data.Views {
  partial class EnumValueView<T> {
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
      this.valueLabel = new System.Windows.Forms.Label();
      this.valueComboBox = new System.Windows.Forms.ComboBox();
      this.flagsListView = new System.Windows.Forms.ListView();
      this.columnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.SuspendLayout();
      // 
      // valueLabel
      // 
      this.valueLabel.AutoSize = true;
      this.valueLabel.Location = new System.Drawing.Point(3, 3);
      this.valueLabel.Name = "valueLabel";
      this.valueLabel.Size = new System.Drawing.Size(37, 13);
      this.valueLabel.TabIndex = 0;
      this.valueLabel.Text = "&Value:";
      // 
      // valueComboBox
      // 
      this.valueComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.valueComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.valueComboBox.Enabled = false;
      this.valueComboBox.FormattingEnabled = true;
      this.valueComboBox.Location = new System.Drawing.Point(65, 0);
      this.valueComboBox.Name = "valueComboBox";
      this.valueComboBox.Size = new System.Drawing.Size(212, 21);
      this.valueComboBox.TabIndex = 1;
      this.valueComboBox.SelectedIndexChanged += new System.EventHandler(this.valueComboBox_SelectedIndexChanged);
      // 
      // flagsListView
      // 
      this.flagsListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.flagsListView.CheckBoxes = true;
      this.flagsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader});
      this.flagsListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
      this.flagsListView.Location = new System.Drawing.Point(65, 0);
      this.flagsListView.Name = "flagsListView";
      this.flagsListView.Size = new System.Drawing.Size(212, 21);
      this.flagsListView.TabIndex = 2;
      this.flagsListView.UseCompatibleStateImageBehavior = false;
      this.flagsListView.View = System.Windows.Forms.View.Details;
      this.flagsListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.flagsListView_ItemChecked);
      // 
      // columnHeader
      // 
      this.columnHeader.Width = 100;
      // 
      // EnumValueView
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.flagsListView);
      this.Controls.Add(this.valueComboBox);
      this.Controls.Add(this.valueLabel);
      this.Name = "EnumValueView";
      this.Size = new System.Drawing.Size(277, 29);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label valueLabel;
    private System.Windows.Forms.ComboBox valueComboBox;
    private System.Windows.Forms.ListView flagsListView;
    private System.Windows.Forms.ColumnHeader columnHeader;
  }
}
