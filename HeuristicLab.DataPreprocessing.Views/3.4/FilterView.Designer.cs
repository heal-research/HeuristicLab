#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  partial class FilterView {
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
      this.groupBoxFilter = new System.Windows.Forms.GroupBox();
      this.checkedFilterView = new HeuristicLab.DataPreprocessing.Views.CheckedFilterCollectionView();
      this.groupBoxFilterInfo = new System.Windows.Forms.GroupBox();
      this.lblPercentage = new System.Windows.Forms.Label();
      this.tbPercentage = new System.Windows.Forms.TextBox();
      this.tbRemaining = new System.Windows.Forms.TextBox();
      this.lblRemaining = new System.Windows.Forms.Label();
      this.tbTotal = new System.Windows.Forms.TextBox();
      this.lblTotal = new System.Windows.Forms.Label();
      this.applyFilterButton = new System.Windows.Forms.Button();
      this.rBtnOr = new System.Windows.Forms.RadioButton();
      this.rBtnAnd = new System.Windows.Forms.RadioButton();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.label4 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.bottomPanel = new System.Windows.Forms.Panel();
      this.groupBoxFilter.SuspendLayout();
      this.groupBoxFilterInfo.SuspendLayout();
      this.groupBox1.SuspendLayout();
      this.bottomPanel.SuspendLayout();
      this.SuspendLayout();
      // 
      // groupBoxFilter
      // 
      this.groupBoxFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.groupBoxFilter.Controls.Add(this.checkedFilterView);
      this.groupBoxFilter.Location = new System.Drawing.Point(4, 4);
      this.groupBoxFilter.Name = "groupBoxFilter";
      this.groupBoxFilter.Size = new System.Drawing.Size(658, 327);
      this.groupBoxFilter.TabIndex = 0;
      this.groupBoxFilter.TabStop = false;
      this.groupBoxFilter.Text = "Filter";
      // 
      // checkedFilterView
      // 
      this.checkedFilterView.Caption = "filterView";
      this.checkedFilterView.Content = null;
      this.checkedFilterView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.checkedFilterView.Location = new System.Drawing.Point(3, 16);
      this.checkedFilterView.Name = "checkedFilterView";
      this.checkedFilterView.ReadOnly = false;
      this.checkedFilterView.ShowDetails = true;
      this.checkedFilterView.Size = new System.Drawing.Size(652, 308);
      this.checkedFilterView.TabIndex = 0;
      // 
      // groupBoxFilterInfo
      // 
      this.groupBoxFilterInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.groupBoxFilterInfo.Controls.Add(this.lblPercentage);
      this.groupBoxFilterInfo.Controls.Add(this.tbPercentage);
      this.groupBoxFilterInfo.Controls.Add(this.tbRemaining);
      this.groupBoxFilterInfo.Controls.Add(this.lblRemaining);
      this.groupBoxFilterInfo.Controls.Add(this.tbTotal);
      this.groupBoxFilterInfo.Controls.Add(this.lblTotal);
      this.groupBoxFilterInfo.Location = new System.Drawing.Point(4, 337);
      this.groupBoxFilterInfo.Name = "groupBoxFilterInfo";
      this.groupBoxFilterInfo.Size = new System.Drawing.Size(658, 102);
      this.groupBoxFilterInfo.TabIndex = 1;
      this.groupBoxFilterInfo.TabStop = false;
      this.groupBoxFilterInfo.Text = "Filter Preview";
      // 
      // lblPercentage
      // 
      this.lblPercentage.AutoSize = true;
      this.lblPercentage.Location = new System.Drawing.Point(15, 74);
      this.lblPercentage.Name = "lblPercentage";
      this.lblPercentage.Size = new System.Drawing.Size(65, 13);
      this.lblPercentage.TabIndex = 11;
      this.lblPercentage.Text = "Percentage:";
      // 
      // tbPercentage
      // 
      this.tbPercentage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tbPercentage.Enabled = false;
      this.tbPercentage.Location = new System.Drawing.Point(104, 71);
      this.tbPercentage.Name = "tbPercentage";
      this.tbPercentage.Size = new System.Drawing.Size(548, 20);
      this.tbPercentage.TabIndex = 10;
      // 
      // tbRemaining
      // 
      this.tbRemaining.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tbRemaining.Enabled = false;
      this.tbRemaining.Location = new System.Drawing.Point(104, 45);
      this.tbRemaining.Name = "tbRemaining";
      this.tbRemaining.Size = new System.Drawing.Size(548, 20);
      this.tbRemaining.TabIndex = 9;
      // 
      // lblRemaining
      // 
      this.lblRemaining.AutoSize = true;
      this.lblRemaining.Location = new System.Drawing.Point(15, 47);
      this.lblRemaining.Name = "lblRemaining";
      this.lblRemaining.Size = new System.Drawing.Size(60, 13);
      this.lblRemaining.TabIndex = 8;
      this.lblRemaining.Text = "Remaining:";
      // 
      // tbTotal
      // 
      this.tbTotal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tbTotal.Enabled = false;
      this.tbTotal.Location = new System.Drawing.Point(104, 19);
      this.tbTotal.Name = "tbTotal";
      this.tbTotal.Size = new System.Drawing.Size(548, 20);
      this.tbTotal.TabIndex = 7;
      // 
      // lblTotal
      // 
      this.lblTotal.AutoSize = true;
      this.lblTotal.Location = new System.Drawing.Point(15, 21);
      this.lblTotal.Name = "lblTotal";
      this.lblTotal.Size = new System.Drawing.Size(34, 13);
      this.lblTotal.TabIndex = 6;
      this.lblTotal.Text = "Total:";
      // 
      // applyFilterButton
      // 
      this.applyFilterButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.applyFilterButton.Cursor = System.Windows.Forms.Cursors.Default;
      this.applyFilterButton.Enabled = false;
      this.applyFilterButton.Location = new System.Drawing.Point(559, 3);
      this.applyFilterButton.Name = "applyFilterButton";
      this.applyFilterButton.Size = new System.Drawing.Size(107, 23);
      this.applyFilterButton.TabIndex = 2;
      this.applyFilterButton.Text = "Apply Filters";
      this.applyFilterButton.UseVisualStyleBackColor = true;
      this.applyFilterButton.Click += new System.EventHandler(this.applyFilterButton_Click);
      // 
      // rBtnOr
      // 
      this.rBtnOr.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.rBtnOr.AutoSize = true;
      this.rBtnOr.Cursor = System.Windows.Forms.Cursors.Default;
      this.rBtnOr.Enabled = false;
      this.rBtnOr.Location = new System.Drawing.Point(53, 6);
      this.rBtnOr.Name = "rBtnOr";
      this.rBtnOr.Size = new System.Drawing.Size(36, 17);
      this.rBtnOr.TabIndex = 3;
      this.rBtnOr.Text = "Or";
      this.rBtnOr.UseVisualStyleBackColor = true;
      // 
      // rBtnAnd
      // 
      this.rBtnAnd.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.rBtnAnd.AutoSize = true;
      this.rBtnAnd.Checked = true;
      this.rBtnAnd.Cursor = System.Windows.Forms.Cursors.Default;
      this.rBtnAnd.Enabled = false;
      this.rBtnAnd.Location = new System.Drawing.Point(3, 6);
      this.rBtnAnd.Name = "rBtnAnd";
      this.rBtnAnd.Size = new System.Drawing.Size(44, 17);
      this.rBtnAnd.TabIndex = 4;
      this.rBtnAnd.TabStop = true;
      this.rBtnAnd.Text = "And";
      this.rBtnAnd.UseVisualStyleBackColor = true;
      this.rBtnAnd.CheckedChanged += new System.EventHandler(this.rBtnAnd_CheckedChanged);
      // 
      // groupBox1
      // 
      this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.groupBox1.Controls.Add(this.label4);
      this.groupBox1.Controls.Add(this.label1);
      this.groupBox1.Controls.Add(this.label2);
      this.groupBox1.Controls.Add(this.label3);
      this.groupBox1.Location = new System.Drawing.Point(4, 445);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(658, 130);
      this.groupBox1.TabIndex = 12;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Filter Info";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(15, 98);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(455, 13);
      this.label4.TabIndex = 12;
      this.label4.Text = "The Apply Filter Button permanently activates the filters. This means unfiltered " +
    "rows are deleted.";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(15, 74);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(529, 13);
      this.label1.TabIndex = 11;
      this.label1.Text = "All preprocessing views are updated when a filter gets activated or deactivated b" +
    "ut the unfiltered rows still exist.";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(15, 47);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(402, 13);
      this.label2.TabIndex = 8;
      this.label2.Text = "In the Filter Preview all effects of the activated filters on the dataset are sum" +
    "marized.";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(15, 21);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(249, 13);
      this.label3.TabIndex = 6;
      this.label3.Text = "A filter specifies the data rows which should remain.";
      // 
      // bottomPanel
      // 
      this.bottomPanel.Controls.Add(this.applyFilterButton);
      this.bottomPanel.Controls.Add(this.rBtnAnd);
      this.bottomPanel.Controls.Add(this.rBtnOr);
      this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.bottomPanel.Location = new System.Drawing.Point(0, 580);
      this.bottomPanel.Name = "bottomPanel";
      this.bottomPanel.Size = new System.Drawing.Size(670, 30);
      this.bottomPanel.TabIndex = 13;
      // 
      // FilterView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.bottomPanel);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.groupBoxFilterInfo);
      this.Controls.Add(this.groupBoxFilter);
      this.Name = "FilterView";
      this.Size = new System.Drawing.Size(670, 610);
      this.groupBoxFilter.ResumeLayout(false);
      this.groupBoxFilterInfo.ResumeLayout(false);
      this.groupBoxFilterInfo.PerformLayout();
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.bottomPanel.ResumeLayout(false);
      this.bottomPanel.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox groupBoxFilter;
    private System.Windows.Forms.GroupBox groupBoxFilterInfo;
    private System.Windows.Forms.Button applyFilterButton;
    private System.Windows.Forms.TextBox tbTotal;
    private System.Windows.Forms.Label lblTotal;
    private System.Windows.Forms.Label lblPercentage;
    private System.Windows.Forms.TextBox tbPercentage;
    private System.Windows.Forms.TextBox tbRemaining;
    private System.Windows.Forms.Label lblRemaining;
    private System.Windows.Forms.RadioButton rBtnOr;
    private System.Windows.Forms.RadioButton rBtnAnd;
    private CheckedFilterCollectionView checkedFilterView;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Panel bottomPanel;
  }
}
