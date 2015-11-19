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

using HeuristicLab.Data;
namespace HeuristicLab.DataPreprocessing.Views {
  partial class PreprocessingChartView {
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
      this.dataTableView = new DataPreprocessing.Views.PreprocessingDataTableView();
      this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
      this.checkedItemList = new HeuristicLab.DataPreprocessing.Views.PreprocessingCheckedItemListView();
      this.tableLayoutPanel.SuspendLayout();
      this.SuspendLayout();
      // 
      // dataTableView
      // 
      this.dataTableView.Caption = "DataTable View";
      this.dataTableView.Content = null;
      this.dataTableView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dataTableView.Location = new System.Drawing.Point(3, 3);
      this.dataTableView.Name = "dataTableView";
      this.dataTableView.ReadOnly = false;
      this.dataTableView.Size = new System.Drawing.Size(486, 390);
      this.dataTableView.TabIndex = 0;
      // 
      // tableLayoutPanel
      // 
      this.tableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tableLayoutPanel.AutoScroll = true;
      this.tableLayoutPanel.ColumnCount = 1;
      this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel.Controls.Add(this.dataTableView, 0, 0);
      this.tableLayoutPanel.Location = new System.Drawing.Point(162, 4);
      this.tableLayoutPanel.Name = "tableLayoutPanel";
      this.tableLayoutPanel.RowCount = 1;
      this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel.Size = new System.Drawing.Size(492, 396);
      this.tableLayoutPanel.TabIndex = 6;
      this.tableLayoutPanel.Layout += new System.Windows.Forms.LayoutEventHandler(this.tableLayoutPanel_Layout);
      // 
      // checkedItemList
      // 
      this.checkedItemList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
      this.checkedItemList.Caption = "View";
      this.checkedItemList.Content = null;
      this.checkedItemList.Location = new System.Drawing.Point(4, 4);
      this.checkedItemList.Name = "checkedItemList";
      this.checkedItemList.ReadOnly = false;
      this.checkedItemList.Size = new System.Drawing.Size(152, 252);
      this.checkedItemList.TabIndex = 4;
      // 
      // PreprocessingChartView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel);
      this.Controls.Add(this.checkedItemList);
      this.Name = "PreprocessingChartView";
      this.Size = new System.Drawing.Size(654, 403);
      this.tableLayoutPanel.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private PreprocessingCheckedItemListView checkedItemList;
    private DataPreprocessing.Views.PreprocessingDataTableView dataTableView;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
  }
}
