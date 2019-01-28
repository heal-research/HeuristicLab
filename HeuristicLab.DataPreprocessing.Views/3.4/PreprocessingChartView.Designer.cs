#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
      this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
      this.sizeGroupBox = new System.Windows.Forms.GroupBox();
      this.columnsNumericUpDown = new System.Windows.Forms.NumericUpDown();
      this.columnsLabel = new System.Windows.Forms.Label();
      this.scrollPanel = new System.Windows.Forms.Panel();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.sizeGroupBox.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.columnsNumericUpDown)).BeginInit();
      this.scrollPanel.SuspendLayout();
      this.SuspendLayout();
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
      this.splitContainer.Panel2.Controls.Add(this.scrollPanel);
      this.splitContainer.Panel2.Resize += new System.EventHandler(this.splitContainer_Panel2_Resize);
      // 
      // tableLayoutPanel
      // 
      this.tableLayoutPanel.AutoSize = true;
      this.tableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.tableLayoutPanel.ColumnCount = 1;
      this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel.Name = "tableLayoutPanel";
      this.tableLayoutPanel.RowCount = 1;
      this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel.Size = new System.Drawing.Size(0, 0);
      this.tableLayoutPanel.TabIndex = 0;
      this.tableLayoutPanel.Layout += new System.Windows.Forms.LayoutEventHandler(this.tableLayoutPanel_Layout);
      // 
      // sizeGroupBox
      // 
      this.sizeGroupBox.Controls.Add(this.columnsNumericUpDown);
      this.sizeGroupBox.Controls.Add(this.columnsLabel);
      this.sizeGroupBox.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.sizeGroupBox.Location = new System.Drawing.Point(0, 359);
      this.sizeGroupBox.Name = "sizeGroupBox";
      this.sizeGroupBox.Size = new System.Drawing.Size(180, 44);
      this.sizeGroupBox.TabIndex = 8;
      this.sizeGroupBox.TabStop = false;
      this.sizeGroupBox.Text = "Chart Size";
      // 
      // columnsNumericUpDown
      // 
      this.columnsNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.columnsNumericUpDown.Location = new System.Drawing.Point(88, 14);
      this.columnsNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.columnsNumericUpDown.Name = "columnsNumericUpDown";
      this.columnsNumericUpDown.Size = new System.Drawing.Size(86, 20);
      this.columnsNumericUpDown.TabIndex = 3;
      this.columnsNumericUpDown.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
      this.columnsNumericUpDown.ValueChanged += new System.EventHandler(this.columnsNumericUpDown_ValueChanged);
      // 
      // columnsLabel
      // 
      this.columnsLabel.AutoSize = true;
      this.columnsLabel.Location = new System.Drawing.Point(6, 16);
      this.columnsLabel.Name = "columnsLabel";
      this.columnsLabel.Size = new System.Drawing.Size(76, 13);
      this.columnsLabel.TabIndex = 1;
      this.columnsLabel.Text = "Max. Columns:";
      // 
      // scrollPanel
      // 
      this.scrollPanel.AutoScroll = true;
      this.scrollPanel.Controls.Add(this.tableLayoutPanel);
      this.scrollPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.scrollPanel.Location = new System.Drawing.Point(0, 0);
      this.scrollPanel.Name = "scrollPanel";
      this.scrollPanel.Size = new System.Drawing.Size(470, 403);
      this.scrollPanel.TabIndex = 1;
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
      this.sizeGroupBox.ResumeLayout(false);
      this.sizeGroupBox.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.columnsNumericUpDown)).EndInit();
      this.scrollPanel.ResumeLayout(false);
      this.scrollPanel.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    protected System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
    private System.Windows.Forms.NumericUpDown columnsNumericUpDown;
    private System.Windows.Forms.Label columnsLabel;
    private System.Windows.Forms.Panel scrollPanel;
    protected System.Windows.Forms.GroupBox sizeGroupBox;
  }
}
