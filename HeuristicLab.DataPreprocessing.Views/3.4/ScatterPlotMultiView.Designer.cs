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
      this.columnHeaderScrollPanel = new System.Windows.Forms.Panel();
      this.columnHeaderTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
      this.rowHeaderScrollPanel = new System.Windows.Forms.Panel();
      this.rowHeaderTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
      this.bodyScrollPanel = new System.Windows.Forms.Panel();
      this.bodyTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
      this.sizeGroupBox = new System.Windows.Forms.GroupBox();
      this.heightLabel = new System.Windows.Forms.Label();
      this.widthLabel = new System.Windows.Forms.Label();
      this.heightTrackBar = new System.Windows.Forms.TrackBar();
      this.widthTrackBar = new System.Windows.Forms.TrackBar();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.frameTableLayoutPanel.SuspendLayout();
      this.columnHeaderScrollPanel.SuspendLayout();
      this.rowHeaderScrollPanel.SuspendLayout();
      this.bodyScrollPanel.SuspendLayout();
      this.sizeGroupBox.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.heightTrackBar)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.widthTrackBar)).BeginInit();
      this.SuspendLayout();
      // 
      // checkedItemList
      // 
      this.checkedItemList.Size = new System.Drawing.Size(113, 369);
      // 
      // splitContainer
      // 
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.sizeGroupBox);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.frameTableLayoutPanel);
      this.splitContainer.Size = new System.Drawing.Size(863, 520);
      this.splitContainer.SplitterDistance = 120;
      // 
      // frameTableLayoutPanel
      // 
      this.frameTableLayoutPanel.ColumnCount = 2;
      this.frameTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
      this.frameTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.frameTableLayoutPanel.Controls.Add(this.columnHeaderScrollPanel, 1, 0);
      this.frameTableLayoutPanel.Controls.Add(this.rowHeaderScrollPanel, 0, 1);
      this.frameTableLayoutPanel.Controls.Add(this.bodyScrollPanel, 1, 1);
      this.frameTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.frameTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
      this.frameTableLayoutPanel.Name = "frameTableLayoutPanel";
      this.frameTableLayoutPanel.RowCount = 2;
      this.frameTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
      this.frameTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.frameTableLayoutPanel.Size = new System.Drawing.Size(739, 520);
      this.frameTableLayoutPanel.TabIndex = 0;
      // 
      // columnHeaderScrollPanel
      // 
      this.columnHeaderScrollPanel.Controls.Add(this.columnHeaderTableLayoutPanel);
      this.columnHeaderScrollPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.columnHeaderScrollPanel.Location = new System.Drawing.Point(40, 0);
      this.columnHeaderScrollPanel.Margin = new System.Windows.Forms.Padding(0);
      this.columnHeaderScrollPanel.Name = "columnHeaderScrollPanel";
      this.columnHeaderScrollPanel.Size = new System.Drawing.Size(699, 40);
      this.columnHeaderScrollPanel.TabIndex = 3;
      // 
      // columnHeaderTableLayoutPanel
      // 
      this.columnHeaderTableLayoutPanel.AutoSize = true;
      this.columnHeaderTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.columnHeaderTableLayoutPanel.ColumnCount = 2;
      this.columnHeaderTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.columnHeaderTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.columnHeaderTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Left;
      this.columnHeaderTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
      this.columnHeaderTableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
      this.columnHeaderTableLayoutPanel.Name = "columnHeaderTableLayoutPanel";
      this.columnHeaderTableLayoutPanel.RowCount = 1;
      this.columnHeaderTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.columnHeaderTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.columnHeaderTableLayoutPanel.Size = new System.Drawing.Size(0, 40);
      this.columnHeaderTableLayoutPanel.TabIndex = 1;
      // 
      // rowHeaderScrollPanel
      // 
      this.rowHeaderScrollPanel.Controls.Add(this.rowHeaderTableLayoutPanel);
      this.rowHeaderScrollPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.rowHeaderScrollPanel.Location = new System.Drawing.Point(0, 40);
      this.rowHeaderScrollPanel.Margin = new System.Windows.Forms.Padding(0);
      this.rowHeaderScrollPanel.Name = "rowHeaderScrollPanel";
      this.rowHeaderScrollPanel.Size = new System.Drawing.Size(40, 480);
      this.rowHeaderScrollPanel.TabIndex = 4;
      // 
      // rowHeaderTableLayoutPanel
      // 
      this.rowHeaderTableLayoutPanel.AutoSize = true;
      this.rowHeaderTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.rowHeaderTableLayoutPanel.ColumnCount = 1;
      this.rowHeaderTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.rowHeaderTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.rowHeaderTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Top;
      this.rowHeaderTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
      this.rowHeaderTableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
      this.rowHeaderTableLayoutPanel.Name = "rowHeaderTableLayoutPanel";
      this.rowHeaderTableLayoutPanel.RowCount = 2;
      this.rowHeaderTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.rowHeaderTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.rowHeaderTableLayoutPanel.Size = new System.Drawing.Size(40, 0);
      this.rowHeaderTableLayoutPanel.TabIndex = 2;
      // 
      // bodyScrollPanel
      // 
      this.bodyScrollPanel.AutoScroll = true;
      this.bodyScrollPanel.Controls.Add(this.bodyTableLayoutPanel);
      this.bodyScrollPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.bodyScrollPanel.Location = new System.Drawing.Point(40, 40);
      this.bodyScrollPanel.Margin = new System.Windows.Forms.Padding(0);
      this.bodyScrollPanel.Name = "bodyScrollPanel";
      this.bodyScrollPanel.Size = new System.Drawing.Size(699, 480);
      this.bodyScrollPanel.TabIndex = 5;
      this.bodyScrollPanel.Scroll += new System.Windows.Forms.ScrollEventHandler(this.bodyScrollPanel_Scroll);
      // 
      // bodyTableLayoutPanel
      // 
      this.bodyTableLayoutPanel.AutoSize = true;
      this.bodyTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.bodyTableLayoutPanel.ColumnCount = 2;
      this.bodyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.bodyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.bodyTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
      this.bodyTableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
      this.bodyTableLayoutPanel.Name = "bodyTableLayoutPanel";
      this.bodyTableLayoutPanel.RowCount = 2;
      this.bodyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.bodyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.bodyTableLayoutPanel.Size = new System.Drawing.Size(0, 0);
      this.bodyTableLayoutPanel.TabIndex = 0;
      // 
      // sizeGroupBox
      // 
      this.sizeGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.sizeGroupBox.Controls.Add(this.heightLabel);
      this.sizeGroupBox.Controls.Add(this.widthLabel);
      this.sizeGroupBox.Controls.Add(this.heightTrackBar);
      this.sizeGroupBox.Controls.Add(this.widthTrackBar);
      this.sizeGroupBox.Location = new System.Drawing.Point(4, 379);
      this.sizeGroupBox.Name = "sizeGroupBox";
      this.sizeGroupBox.Size = new System.Drawing.Size(113, 124);
      this.sizeGroupBox.TabIndex = 5;
      this.sizeGroupBox.TabStop = false;
      this.sizeGroupBox.Text = "Chart Size";
      // 
      // heightLabel
      // 
      this.heightLabel.AutoSize = true;
      this.heightLabel.Location = new System.Drawing.Point(6, 64);
      this.heightLabel.Name = "heightLabel";
      this.heightLabel.Size = new System.Drawing.Size(41, 13);
      this.heightLabel.TabIndex = 2;
      this.heightLabel.Text = "Height:";
      // 
      // widthLabel
      // 
      this.widthLabel.AutoSize = true;
      this.widthLabel.Location = new System.Drawing.Point(6, 16);
      this.widthLabel.Name = "widthLabel";
      this.widthLabel.Size = new System.Drawing.Size(38, 13);
      this.widthLabel.TabIndex = 1;
      this.widthLabel.Text = "Width:";
      // 
      // heightTrackBar
      // 
      this.heightTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.heightTrackBar.LargeChange = 50;
      this.heightTrackBar.Location = new System.Drawing.Point(6, 83);
      this.heightTrackBar.Maximum = 100;
      this.heightTrackBar.Name = "heightTrackBar";
      this.heightTrackBar.Size = new System.Drawing.Size(101, 45);
      this.heightTrackBar.SmallChange = 10;
      this.heightTrackBar.TabIndex = 0;
      this.heightTrackBar.TickFrequency = 10;
      this.heightTrackBar.Value = 20;
      this.heightTrackBar.ValueChanged += new System.EventHandler(this.heightTrackBar_ValueChanged);
      // 
      // widthTrackBar
      // 
      this.widthTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.widthTrackBar.LargeChange = 50;
      this.widthTrackBar.Location = new System.Drawing.Point(6, 32);
      this.widthTrackBar.Maximum = 100;
      this.widthTrackBar.Name = "widthTrackBar";
      this.widthTrackBar.Size = new System.Drawing.Size(101, 45);
      this.widthTrackBar.SmallChange = 10;
      this.widthTrackBar.TabIndex = 0;
      this.widthTrackBar.TickFrequency = 10;
      this.widthTrackBar.Value = 20;
      this.widthTrackBar.ValueChanged += new System.EventHandler(this.widthTrackBar_ValueChanged);
      // 
      // ScatterPlotMultiView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Name = "ScatterPlotMultiView";
      this.Size = new System.Drawing.Size(863, 520);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.frameTableLayoutPanel.ResumeLayout(false);
      this.columnHeaderScrollPanel.ResumeLayout(false);
      this.columnHeaderScrollPanel.PerformLayout();
      this.rowHeaderScrollPanel.ResumeLayout(false);
      this.rowHeaderScrollPanel.PerformLayout();
      this.bodyScrollPanel.ResumeLayout(false);
      this.bodyScrollPanel.PerformLayout();
      this.sizeGroupBox.ResumeLayout(false);
      this.sizeGroupBox.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.heightTrackBar)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.widthTrackBar)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel frameTableLayoutPanel;
    private System.Windows.Forms.TableLayoutPanel columnHeaderTableLayoutPanel;
    private System.Windows.Forms.TableLayoutPanel rowHeaderTableLayoutPanel;
    private System.Windows.Forms.TableLayoutPanel bodyTableLayoutPanel;
    private System.Windows.Forms.Panel columnHeaderScrollPanel;
    private System.Windows.Forms.Panel rowHeaderScrollPanel;
    private System.Windows.Forms.Panel bodyScrollPanel;
    private System.Windows.Forms.GroupBox sizeGroupBox;
    private System.Windows.Forms.TrackBar widthTrackBar;
    private System.Windows.Forms.TrackBar heightTrackBar;
    private System.Windows.Forms.Label heightLabel;
    private System.Windows.Forms.Label widthLabel;
  }
}
