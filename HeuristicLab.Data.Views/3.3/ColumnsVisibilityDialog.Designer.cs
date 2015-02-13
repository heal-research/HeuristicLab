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

namespace HeuristicLab.Data.Views {
  partial class ColumnsVisibilityDialog {
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
      this.checkedListBox = new System.Windows.Forms.CheckedListBox();
      this.btnShowAll = new System.Windows.Forms.Button();
      this.btnHideAll = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // checkedListBox
      // 
      this.checkedListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.checkedListBox.CheckOnClick = true;
      this.checkedListBox.FormattingEnabled = true;
      this.checkedListBox.Location = new System.Drawing.Point(12, 12);
      this.checkedListBox.Name = "checkedListBox";
      this.checkedListBox.Size = new System.Drawing.Size(309, 289);
      this.checkedListBox.TabIndex = 0;
      this.checkedListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBox_ItemCheck);
      // 
      // btnShowAll
      // 
      this.btnShowAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btnShowAll.Location = new System.Drawing.Point(12, 315);
      this.btnShowAll.Name = "btnShowAll";
      this.btnShowAll.Size = new System.Drawing.Size(75, 23);
      this.btnShowAll.TabIndex = 1;
      this.btnShowAll.Text = "Show all";
      this.btnShowAll.UseVisualStyleBackColor = true;
      this.btnShowAll.Click += new System.EventHandler(this.btnShowAll_Click);
      // 
      // btnHideAll
      // 
      this.btnHideAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btnHideAll.Location = new System.Drawing.Point(101, 315);
      this.btnHideAll.Name = "btnHideAll";
      this.btnHideAll.Size = new System.Drawing.Size(75, 23);
      this.btnHideAll.TabIndex = 2;
      this.btnHideAll.Text = "Hide all";
      this.btnHideAll.UseVisualStyleBackColor = true;
      this.btnHideAll.Click += new System.EventHandler(this.btnHideAll_Click);
      // 
      // ColumnsVisibilityDialog
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.ClientSize = new System.Drawing.Size(332, 350);
      this.Controls.Add(this.btnHideAll);
      this.Controls.Add(this.btnShowAll);
      this.Controls.Add(this.checkedListBox);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ColumnsVisibilityDialog";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Show/Hide Columns";
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.CheckedListBox checkedListBox;
    private System.Windows.Forms.Button btnShowAll;
    private System.Windows.Forms.Button btnHideAll;
  }
}