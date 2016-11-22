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
      this.dataTableView = new HeuristicLab.DataPreprocessing.Views.PreprocessingDataTableView();
      this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.tableLayoutPanel.SuspendLayout();
      this.SuspendLayout();
      // 
      // splitContainer
      // 
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.tableLayoutPanel);
      // 
      // dataTableView
      // 
      this.dataTableView.AutoScroll = true;
      this.dataTableView.Caption = "DataTable View";
      this.dataTableView.Classification = null;
      this.dataTableView.Content = null;
      this.dataTableView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dataTableView.IsDetailedChartViewEnabled = false;
      this.dataTableView.Location = new System.Drawing.Point(3, 3);
      this.dataTableView.Name = "dataTableView";
      this.dataTableView.ReadOnly = false;
      this.dataTableView.ShowLegend = true;
      this.dataTableView.Size = new System.Drawing.Size(553, 397);
      this.dataTableView.TabIndex = 0;
      this.dataTableView.XAxisFormat = "";
      this.dataTableView.YAxisFormat = "";
      // 
      // tableLayoutPanel
      // 
      this.tableLayoutPanel.ColumnCount = 1;
      this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel.Controls.Add(this.dataTableView, 0, 0);
      this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel.Name = "tableLayoutPanel";
      this.tableLayoutPanel.RowCount = 1;
      this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel.Size = new System.Drawing.Size(559, 403);
      this.tableLayoutPanel.TabIndex = 0;
      this.tableLayoutPanel.Layout += new System.Windows.Forms.LayoutEventHandler(this.tableLayoutPanel_Layout);
      // 
      // PreprocessingChartView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Name = "PreprocessingChartView";
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.tableLayoutPanel.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private DataPreprocessing.Views.PreprocessingDataTableView dataTableView;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
  }
}
