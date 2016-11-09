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
  partial class ScatterPlotMultiView {
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
      this.frameTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
      this.columnHeaderTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
      this.rowHeaderTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
      this.bodyTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.frameTableLayoutPanel.SuspendLayout();
      this.SuspendLayout();
      // 
      // splitContainer
      // 
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.frameTableLayoutPanel);
      // 
      // frameTableLayoutPanel
      // 
      this.frameTableLayoutPanel.ColumnCount = 2;
      this.frameTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
      this.frameTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.frameTableLayoutPanel.Controls.Add(this.columnHeaderTableLayoutPanel, 1, 0);
      this.frameTableLayoutPanel.Controls.Add(this.rowHeaderTableLayoutPanel, 0, 1);
      this.frameTableLayoutPanel.Controls.Add(this.bodyTableLayoutPanel, 1, 1);
      this.frameTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.frameTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
      this.frameTableLayoutPanel.Name = "frameTableLayoutPanel";
      this.frameTableLayoutPanel.RowCount = 2;
      this.frameTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
      this.frameTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.frameTableLayoutPanel.Size = new System.Drawing.Size(739, 517);
      this.frameTableLayoutPanel.TabIndex = 0;
      // 
      // columnHeaderTableLayoutPanel
      // 
      this.columnHeaderTableLayoutPanel.ColumnCount = 2;
      this.columnHeaderTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.columnHeaderTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.columnHeaderTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.columnHeaderTableLayoutPanel.Location = new System.Drawing.Point(43, 3);
      this.columnHeaderTableLayoutPanel.Name = "columnHeaderTableLayoutPanel";
      this.columnHeaderTableLayoutPanel.RowCount = 1;
      this.columnHeaderTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.columnHeaderTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.columnHeaderTableLayoutPanel.Size = new System.Drawing.Size(693, 34);
      this.columnHeaderTableLayoutPanel.TabIndex = 1;
      // 
      // rowHeaderTableLayoutPanel
      // 
      this.rowHeaderTableLayoutPanel.ColumnCount = 1;
      this.rowHeaderTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.rowHeaderTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.rowHeaderTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.rowHeaderTableLayoutPanel.Location = new System.Drawing.Point(3, 43);
      this.rowHeaderTableLayoutPanel.Name = "rowHeaderTableLayoutPanel";
      this.rowHeaderTableLayoutPanel.RowCount = 2;
      this.rowHeaderTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.rowHeaderTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.rowHeaderTableLayoutPanel.Size = new System.Drawing.Size(34, 471);
      this.rowHeaderTableLayoutPanel.TabIndex = 2;
      // 
      // bodyTableLayoutPanel
      // 
      this.bodyTableLayoutPanel.ColumnCount = 2;
      this.bodyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.bodyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.bodyTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.bodyTableLayoutPanel.Location = new System.Drawing.Point(43, 43);
      this.bodyTableLayoutPanel.Name = "bodyTableLayoutPanel";
      this.bodyTableLayoutPanel.RowCount = 2;
      this.bodyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.bodyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.bodyTableLayoutPanel.Size = new System.Drawing.Size(693, 471);
      this.bodyTableLayoutPanel.TabIndex = 0;
      this.bodyTableLayoutPanel.Scroll += new System.Windows.Forms.ScrollEventHandler(this.bodyTableLayoutPanel_Scroll);
      // 
      // ScatterPlotMultiView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Name = "ScatterPlotMultiView";
      this.Size = new System.Drawing.Size(863, 517);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.frameTableLayoutPanel.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel frameTableLayoutPanel;
    private System.Windows.Forms.TableLayoutPanel columnHeaderTableLayoutPanel;
    private System.Windows.Forms.TableLayoutPanel rowHeaderTableLayoutPanel;
    private System.Windows.Forms.TableLayoutPanel bodyTableLayoutPanel;
  }
}
