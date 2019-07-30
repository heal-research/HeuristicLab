#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  partial class SearchAndReplaceDialog {
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
      this.tabSearchReplace = new System.Windows.Forms.TabControl();
      this.tabSearch = new System.Windows.Forms.TabPage();
      this.tabReplace = new System.Windows.Forms.TabPage();
      this.cmbComparisonOperator = new System.Windows.Forms.ComboBox();
      this.txtSearchString = new System.Windows.Forms.TextBox();
      this.lblSearch = new System.Windows.Forms.Label();
      this.btnFindAll = new System.Windows.Forms.Button();
      this.btnFindNext = new System.Windows.Forms.Button();
      this.lblValue = new System.Windows.Forms.Label();
      this.btnReplaceAll = new System.Windows.Forms.Button();
      this.btnReplace = new System.Windows.Forms.Button();
      this.cmbReplaceWith = new System.Windows.Forms.ComboBox();
      this.txtValue = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.tabSearchReplace.SuspendLayout();
      this.tabReplace.SuspendLayout();
      this.SuspendLayout();
      // 
      // tabSearchReplace
      // 
      this.tabSearchReplace.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tabSearchReplace.Controls.Add(this.tabSearch);
      this.tabSearchReplace.Controls.Add(this.tabReplace);
      this.tabSearchReplace.Location = new System.Drawing.Point(12, 12);
      this.tabSearchReplace.Name = "tabSearchReplace";
      this.tabSearchReplace.SelectedIndex = 0;
      this.tabSearchReplace.Size = new System.Drawing.Size(564, 132);
      this.tabSearchReplace.TabIndex = 0;
      this.tabSearchReplace.SelectedIndexChanged += new System.EventHandler(this.tabSearchReplace_SelectedIndexChanged);
      // 
      // tabSearch
      // 
      this.tabSearch.BackColor = System.Drawing.SystemColors.Window;
      this.tabSearch.Location = new System.Drawing.Point(4, 22);
      this.tabSearch.Name = "tabSearch";
      this.tabSearch.Padding = new System.Windows.Forms.Padding(3);
      this.tabSearch.Size = new System.Drawing.Size(556, 106);
      this.tabSearch.TabIndex = 0;
      this.tabSearch.Text = "Search";
      // 
      // tabReplace
      // 
      this.tabReplace.BackColor = System.Drawing.SystemColors.Window;
      this.tabReplace.Controls.Add(this.cmbComparisonOperator);
      this.tabReplace.Controls.Add(this.txtSearchString);
      this.tabReplace.Controls.Add(this.lblSearch);
      this.tabReplace.Controls.Add(this.btnFindAll);
      this.tabReplace.Controls.Add(this.btnFindNext);
      this.tabReplace.Controls.Add(this.lblValue);
      this.tabReplace.Controls.Add(this.btnReplaceAll);
      this.tabReplace.Controls.Add(this.btnReplace);
      this.tabReplace.Controls.Add(this.cmbReplaceWith);
      this.tabReplace.Controls.Add(this.txtValue);
      this.tabReplace.Controls.Add(this.label1);
      this.tabReplace.Location = new System.Drawing.Point(4, 22);
      this.tabReplace.Name = "tabReplace";
      this.tabReplace.Padding = new System.Windows.Forms.Padding(3);
      this.tabReplace.Size = new System.Drawing.Size(556, 106);
      this.tabReplace.TabIndex = 1;
      this.tabReplace.Text = "Replace";
      // 
      // cmbComparisonOperator
      // 
      this.cmbComparisonOperator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cmbComparisonOperator.FormattingEnabled = true;
      this.cmbComparisonOperator.Location = new System.Drawing.Point(103, 19);
      this.cmbComparisonOperator.Name = "cmbComparisonOperator";
      this.cmbComparisonOperator.Size = new System.Drawing.Size(45, 21);
      this.cmbComparisonOperator.TabIndex = 24;
      // 
      // txtSearchString
      // 
      this.txtSearchString.Location = new System.Drawing.Point(154, 20);
      this.txtSearchString.Name = "txtSearchString";
      this.txtSearchString.Size = new System.Drawing.Size(203, 20);
      this.txtSearchString.TabIndex = 23;
      // 
      // lblSearch
      // 
      this.lblSearch.AutoSize = true;
      this.lblSearch.Location = new System.Drawing.Point(41, 26);
      this.lblSearch.Name = "lblSearch";
      this.lblSearch.Size = new System.Drawing.Size(56, 13);
      this.lblSearch.TabIndex = 22;
      this.lblSearch.Text = "Search for";
      // 
      // btnFindAll
      // 
      this.btnFindAll.Location = new System.Drawing.Point(458, 20);
      this.btnFindAll.Name = "btnFindAll";
      this.btnFindAll.Size = new System.Drawing.Size(80, 23);
      this.btnFindAll.TabIndex = 21;
      this.btnFindAll.Text = "Find All";
      this.btnFindAll.UseVisualStyleBackColor = true;
      // 
      // btnFindNext
      // 
      this.btnFindNext.Location = new System.Drawing.Point(372, 20);
      this.btnFindNext.Name = "btnFindNext";
      this.btnFindNext.Size = new System.Drawing.Size(80, 23);
      this.btnFindNext.TabIndex = 20;
      this.btnFindNext.Text = "Find Next";
      this.btnFindNext.UseVisualStyleBackColor = true;
      // 
      // lblValue
      // 
      this.lblValue.AutoSize = true;
      this.lblValue.Location = new System.Drawing.Point(64, 76);
      this.lblValue.Name = "lblValue";
      this.lblValue.Size = new System.Drawing.Size(34, 13);
      this.lblValue.TabIndex = 19;
      this.lblValue.Text = "Value";
      // 
      // btnReplaceAll
      // 
      this.btnReplaceAll.Location = new System.Drawing.Point(458, 47);
      this.btnReplaceAll.Name = "btnReplaceAll";
      this.btnReplaceAll.Size = new System.Drawing.Size(80, 23);
      this.btnReplaceAll.TabIndex = 18;
      this.btnReplaceAll.Text = "Replace All";
      this.btnReplaceAll.UseVisualStyleBackColor = true;
      // 
      // btnReplace
      // 
      this.btnReplace.Location = new System.Drawing.Point(372, 47);
      this.btnReplace.Name = "btnReplace";
      this.btnReplace.Size = new System.Drawing.Size(80, 23);
      this.btnReplace.TabIndex = 17;
      this.btnReplace.Text = "Replace";
      this.btnReplace.UseVisualStyleBackColor = true;
      // 
      // cmbReplaceWith
      // 
      this.cmbReplaceWith.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cmbReplaceWith.FormattingEnabled = true;
      this.cmbReplaceWith.Location = new System.Drawing.Point(103, 46);
      this.cmbReplaceWith.Name = "cmbReplaceWith";
      this.cmbReplaceWith.Size = new System.Drawing.Size(254, 21);
      this.cmbReplaceWith.TabIndex = 16;
      this.cmbReplaceWith.SelectedIndexChanged += new System.EventHandler(this.cmbReplaceWith_SelectedIndexChanged);
      // 
      // txtValue
      // 
      this.txtValue.Location = new System.Drawing.Point(103, 73);
      this.txtValue.Name = "txtValue";
      this.txtValue.Size = new System.Drawing.Size(254, 20);
      this.txtValue.TabIndex = 15;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(28, 49);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(69, 13);
      this.label1.TabIndex = 14;
      this.label1.Text = "Replace with";
      // 
      // SearchAndReplaceDialog
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(588, 156);
      this.Controls.Add(this.tabSearchReplace);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "SearchAndReplaceDialog";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.Text = "Search and Replace";
      this.tabSearchReplace.ResumeLayout(false);
      this.tabReplace.ResumeLayout(false);
      this.tabReplace.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TabControl tabSearchReplace;
    private System.Windows.Forms.TabPage tabSearch;
    private System.Windows.Forms.TabPage tabReplace;
    private System.Windows.Forms.TextBox txtSearchString;
    private System.Windows.Forms.Label lblSearch;
    private System.Windows.Forms.Button btnFindAll;
    private System.Windows.Forms.Button btnFindNext;
    private System.Windows.Forms.Label lblValue;
    private System.Windows.Forms.Button btnReplaceAll;
    private System.Windows.Forms.Button btnReplace;
    private System.Windows.Forms.ComboBox cmbReplaceWith;
    private System.Windows.Forms.TextBox txtValue;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.ComboBox cmbComparisonOperator;
  }
}