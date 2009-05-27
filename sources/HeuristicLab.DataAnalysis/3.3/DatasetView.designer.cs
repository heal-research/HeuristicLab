#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.DataAnalysis {
  partial class DatasetView {
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
      this.components = new System.ComponentModel.Container();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
      this.rowsTextBox = new System.Windows.Forms.TextBox();
      this.columnsTextBox = new System.Windows.Forms.TextBox();
      this.dataGridView = new System.Windows.Forms.DataGridView();
      this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.scaleValuesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.scaleValuesmanuallyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.showScalingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.originalValuesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.rowsLabel = new System.Windows.Forms.Label();
      this.columnsLabel = new System.Windows.Forms.Label();
      this.nameLabel = new System.Windows.Forms.Label();
      this.nameTextBox = new System.Windows.Forms.TextBox();
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
      this.contextMenuStrip.SuspendLayout();
      this.SuspendLayout();
      // 
      // rowsTextBox
      // 
      this.rowsTextBox.Location = new System.Drawing.Point(50, 29);
      this.rowsTextBox.Name = "rowsTextBox";
      this.rowsTextBox.ReadOnly = true;
      this.rowsTextBox.Size = new System.Drawing.Size(100, 20);
      this.rowsTextBox.TabIndex = 1;
      // 
      // columnsTextBox
      // 
      this.columnsTextBox.Location = new System.Drawing.Point(217, 29);
      this.columnsTextBox.Name = "columnsTextBox";
      this.columnsTextBox.ReadOnly = true;
      this.columnsTextBox.Size = new System.Drawing.Size(100, 20);
      this.columnsTextBox.TabIndex = 2;
      // 
      // dataGridView
      // 
      this.dataGridView.AllowUserToAddRows = false;
      this.dataGridView.AllowUserToDeleteRows = false;
      this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dataGridView.ContextMenuStrip = this.contextMenuStrip;
      this.dataGridView.Location = new System.Drawing.Point(3, 55);
      this.dataGridView.Name = "dataGridView";
      dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
      dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
      dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
      dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
      dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
      dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
      this.dataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle1;
      this.dataGridView.Size = new System.Drawing.Size(554, 485);
      this.dataGridView.TabIndex = 3;
      this.dataGridView.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dataGridView_CellValidating);
      // 
      // contextMenuStrip
      // 
      this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.scaleValuesToolStripMenuItem,
            this.scaleValuesmanuallyToolStripMenuItem,
            this.showScalingToolStripMenuItem,
            this.originalValuesToolStripMenuItem});
      this.contextMenuStrip.Name = "contextMenuStrip";
      this.contextMenuStrip.Size = new System.Drawing.Size(187, 92);
      // 
      // scaleValuesToolStripMenuItem
      // 
      this.scaleValuesToolStripMenuItem.Name = "scaleValuesToolStripMenuItem";
      this.scaleValuesToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
      this.scaleValuesToolStripMenuItem.Text = "Scale values [0..1]";
      this.scaleValuesToolStripMenuItem.Click += new System.EventHandler(this.scaleValuesToolStripMenuItem_Click);
      // 
      // scaleValuesmanuallyToolStripMenuItem
      // 
      this.scaleValuesmanuallyToolStripMenuItem.Name = "scaleValuesmanuallyToolStripMenuItem";
      this.scaleValuesmanuallyToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
      this.scaleValuesmanuallyToolStripMenuItem.Text = "Scale values (manually)";
      this.scaleValuesmanuallyToolStripMenuItem.Click += new System.EventHandler(this.scaleValuesmanuallyToolStripMenuItem_Click);
      // 
      // showScalingToolStripMenuItem
      // 
      this.showScalingToolStripMenuItem.Name = "showScalingToolStripMenuItem";
      this.showScalingToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
      this.showScalingToolStripMenuItem.Text = "Show scaling...";
      this.showScalingToolStripMenuItem.Click += new System.EventHandler(this.showScalingToolStripMenuItem_Click);
      // 
      // originalValuesToolStripMenuItem
      // 
      this.originalValuesToolStripMenuItem.Name = "originalValuesToolStripMenuItem";
      this.originalValuesToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
      this.originalValuesToolStripMenuItem.Text = "Unscale values";
      this.originalValuesToolStripMenuItem.Click += new System.EventHandler(this.originalValuesToolStripMenuItem_Click);
      // 
      // rowsLabel
      // 
      this.rowsLabel.AutoSize = true;
      this.rowsLabel.Location = new System.Drawing.Point(3, 32);
      this.rowsLabel.Name = "rowsLabel";
      this.rowsLabel.Size = new System.Drawing.Size(37, 13);
      this.rowsLabel.TabIndex = 4;
      this.rowsLabel.Text = "Rows:";
      // 
      // columnsLabel
      // 
      this.columnsLabel.AutoSize = true;
      this.columnsLabel.Location = new System.Drawing.Point(161, 32);
      this.columnsLabel.Name = "columnsLabel";
      this.columnsLabel.Size = new System.Drawing.Size(50, 13);
      this.columnsLabel.TabIndex = 5;
      this.columnsLabel.Text = "Columns:";
      // 
      // nameLabel
      // 
      this.nameLabel.AutoSize = true;
      this.nameLabel.Location = new System.Drawing.Point(3, 6);
      this.nameLabel.Name = "nameLabel";
      this.nameLabel.Size = new System.Drawing.Size(41, 13);
      this.nameLabel.TabIndex = 6;
      this.nameLabel.Text = "Name: ";
      // 
      // nameTextBox
      // 
      this.nameTextBox.Location = new System.Drawing.Point(50, 3);
      this.nameTextBox.Name = "nameTextBox";
      this.nameTextBox.ReadOnly = true;
      this.nameTextBox.Size = new System.Drawing.Size(219, 20);
      this.nameTextBox.TabIndex = 7;
      // 
      // DatasetView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.nameTextBox);
      this.Controls.Add(this.nameLabel);
      this.Controls.Add(this.columnsLabel);
      this.Controls.Add(this.rowsLabel);
      this.Controls.Add(this.dataGridView);
      this.Controls.Add(this.columnsTextBox);
      this.Controls.Add(this.rowsTextBox);
      this.Name = "DatasetView";
      this.Size = new System.Drawing.Size(560, 543);
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
      this.contextMenuStrip.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox rowsTextBox;
    private System.Windows.Forms.TextBox columnsTextBox;
    private System.Windows.Forms.DataGridView dataGridView;
    private System.Windows.Forms.Label rowsLabel;
    private System.Windows.Forms.Label columnsLabel;
    private System.Windows.Forms.Label nameLabel;
    private System.Windows.Forms.TextBox nameTextBox;
    private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
    private System.Windows.Forms.ToolStripMenuItem scaleValuesToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem originalValuesToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem scaleValuesmanuallyToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem showScalingToolStripMenuItem;
  }
}
