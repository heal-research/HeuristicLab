#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  partial class StringConvertibleMatrixView {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing) {
        if (components != null) components.Dispose();
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
      this.rowsLabel = new System.Windows.Forms.Label();
      this.rowsTextBox = new System.Windows.Forms.TextBox();
      this.dataGridView = new System.Windows.Forms.DataGridView();
      this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
      this.columnsTextBox = new System.Windows.Forms.TextBox();
      this.columnsLabel = new System.Windows.Forms.Label();
      this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.ShowHideColumns = new System.Windows.Forms.ToolStripMenuItem();
      this.statusStrip = new System.Windows.Forms.StatusStrip();
      this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.contextMenu.SuspendLayout();
      this.statusStrip.SuspendLayout();
      this.SuspendLayout();
      // 
      // rowsLabel
      // 
      this.rowsLabel.AutoSize = true;
      this.rowsLabel.Location = new System.Drawing.Point(3, 3);
      this.rowsLabel.Name = "rowsLabel";
      this.rowsLabel.Size = new System.Drawing.Size(37, 13);
      this.rowsLabel.TabIndex = 0;
      this.rowsLabel.Text = "&Rows:";
      // 
      // rowsTextBox
      // 
      this.rowsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.rowsTextBox.Location = new System.Drawing.Point(69, 0);
      this.rowsTextBox.Name = "rowsTextBox";
      this.rowsTextBox.Size = new System.Drawing.Size(355, 20);
      this.rowsTextBox.TabIndex = 1;
      this.rowsTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rowsTextBox_KeyDown);
      this.rowsTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.rowsTextBox_Validating);
      this.rowsTextBox.Validated += new System.EventHandler(this.rowsTextBox_Validated);
      // 
      // dataGridView
      // 
      this.dataGridView.AllowUserToAddRows = false;
      this.dataGridView.AllowUserToDeleteRows = false;
      this.dataGridView.AllowUserToOrderColumns = true;
      this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.dataGridView.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
      this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dataGridView.Location = new System.Drawing.Point(0, 52);
      this.dataGridView.Name = "dataGridView";
      this.dataGridView.RowHeadersWidth = 160;
      this.dataGridView.Size = new System.Drawing.Size(424, 327);
      this.dataGridView.TabIndex = 4;
      this.dataGridView.VirtualMode = true;
      this.dataGridView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellEndEdit);
      this.dataGridView.CellParsing += new System.Windows.Forms.DataGridViewCellParsingEventHandler(this.dataGridView_CellParsing);
      this.dataGridView.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dataGridView_CellValidating);
      this.dataGridView.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dataGridView_CellValueNeeded);
      this.dataGridView.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView_ColumnHeaderMouseClick);
      this.dataGridView.Scroll += new System.Windows.Forms.ScrollEventHandler(this.dataGridView_Scroll);
      this.dataGridView.SelectionChanged += new System.EventHandler(this.dataGridView_SelectionChanged);
      this.dataGridView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridView_KeyDown);
      this.dataGridView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dataGridView_MouseClick);
      this.dataGridView.Resize += new System.EventHandler(this.dataGridView_Resize);
      // 
      // errorProvider
      // 
      this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
      this.errorProvider.ContainerControl = this;
      // 
      // columnsTextBox
      // 
      this.columnsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.columnsTextBox.Location = new System.Drawing.Point(69, 26);
      this.columnsTextBox.Name = "columnsTextBox";
      this.columnsTextBox.Size = new System.Drawing.Size(355, 20);
      this.columnsTextBox.TabIndex = 3;
      this.columnsTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.columnsTextBox_KeyDown);
      this.columnsTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.columnsTextBox_Validating);
      this.columnsTextBox.Validated += new System.EventHandler(this.columnsTextBox_Validated);
      // 
      // columnsLabel
      // 
      this.columnsLabel.AutoSize = true;
      this.columnsLabel.Location = new System.Drawing.Point(3, 29);
      this.columnsLabel.Name = "columnsLabel";
      this.columnsLabel.Size = new System.Drawing.Size(50, 13);
      this.columnsLabel.TabIndex = 2;
      this.columnsLabel.Text = "&Columns:";
      // 
      // contextMenu
      // 
      this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ShowHideColumns});
      this.contextMenu.Name = "contextMenu";
      this.contextMenu.Size = new System.Drawing.Size(191, 26);
      // 
      // ShowHideColumns
      // 
      this.ShowHideColumns.Name = "ShowHideColumns";
      this.ShowHideColumns.Size = new System.Drawing.Size(190, 22);
      this.ShowHideColumns.Text = "Show / Hide Columns";
      this.ShowHideColumns.Click += new System.EventHandler(this.ShowHideColumns_Click);
      // 
      // statusStrip1
      // 
      this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
      this.statusStrip.Location = new System.Drawing.Point(0, 382);
      this.statusStrip.Name = "statusStrip1";
      this.statusStrip.Size = new System.Drawing.Size(424, 22);
      this.statusStrip.TabIndex = 5;
      this.statusStrip.Text = "statusStrip1";
      // 
      // toolStripStatusLabel
      // 
      this.toolStripStatusLabel.Name = "toolStripStatusLabel";
      this.toolStripStatusLabel.Size = new System.Drawing.Size(378, 17);
      this.toolStripStatusLabel.Spring = true;
      // 
      // StringConvertibleMatrixView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.statusStrip);
      this.Controls.Add(this.dataGridView);
      this.Controls.Add(this.columnsTextBox);
      this.Controls.Add(this.rowsTextBox);
      this.Controls.Add(this.columnsLabel);
      this.Controls.Add(this.rowsLabel);
      this.Name = "StringConvertibleMatrixView";
      this.Size = new System.Drawing.Size(424, 404);
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.contextMenu.ResumeLayout(false);
      this.statusStrip.ResumeLayout(false);
      this.statusStrip.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }
    #endregion

    protected System.Windows.Forms.Label rowsLabel;
    protected System.Windows.Forms.TextBox rowsTextBox;
    protected System.Windows.Forms.ErrorProvider errorProvider;
    protected System.Windows.Forms.TextBox columnsTextBox;
    protected System.Windows.Forms.Label columnsLabel;
    protected System.Windows.Forms.ContextMenuStrip contextMenu;
    protected System.Windows.Forms.ToolStripMenuItem ShowHideColumns;
    protected System.Windows.Forms.DataGridView dataGridView;
    private System.Windows.Forms.StatusStrip statusStrip;
    private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;

  }
}
