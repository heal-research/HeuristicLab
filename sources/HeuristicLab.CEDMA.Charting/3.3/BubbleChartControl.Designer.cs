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

namespace HeuristicLab.CEDMA.Charting {
  partial class BubbleChartControl {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if(disposing && (components != null)) {
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
      this.pictureBox = new System.Windows.Forms.PictureBox();
      this.pictureBoxContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.moveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.zoomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.selectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.invertSelectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.filterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.clearSelectionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.showHiddenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
      this.pictureBoxContextMenuStrip.SuspendLayout();
      this.SuspendLayout();
      // 
      // pictureBox
      // 
      this.pictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.pictureBox.BackColor = System.Drawing.Color.White;
      this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.pictureBox.ContextMenuStrip = this.pictureBoxContextMenuStrip;
      this.pictureBox.Location = new System.Drawing.Point(0, 0);
      this.pictureBox.Name = "pictureBox";
      this.pictureBox.Size = new System.Drawing.Size(266, 236);
      this.pictureBox.TabIndex = 0;
      this.pictureBox.TabStop = false;
      this.pictureBox.VisibleChanged += new System.EventHandler(this.pictureBox_VisibleChanged);
      this.pictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseMove);
      this.pictureBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseDoubleClick);
      this.pictureBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseClick);
      this.pictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseDown);
      this.pictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseUp);
      this.pictureBox.SizeChanged += new System.EventHandler(this.pictureBox_SizeChanged);
      // 
      // pictureBoxContextMenuStrip
      // 
      this.pictureBoxContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.moveToolStripMenuItem,
            this.zoomToolStripMenuItem,
            this.selectToolStripMenuItem,
            this.toolStripSeparator1,
            this.clearSelectionMenuItem,
            this.invertSelectionToolStripMenuItem,
            this.filterToolStripMenuItem,
            this.showHiddenToolStripMenuItem});
      this.pictureBoxContextMenuStrip.Name = "pictureBoxContextMenuStrip";
      this.pictureBoxContextMenuStrip.Size = new System.Drawing.Size(155, 186);
      // 
      // moveToolStripMenuItem
      // 
      this.moveToolStripMenuItem.Name = "moveToolStripMenuItem";
      this.moveToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
      this.moveToolStripMenuItem.Text = "Move";
      this.moveToolStripMenuItem.Click += new System.EventHandler(this.moveToolStripMenuItem_Click);
      // 
      // zoomToolStripMenuItem
      // 
      this.zoomToolStripMenuItem.Name = "zoomToolStripMenuItem";
      this.zoomToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
      this.zoomToolStripMenuItem.Text = "&Zoom";
      this.zoomToolStripMenuItem.Click += new System.EventHandler(this.zoomToolStripMenuItem_Click);
      // 
      // selectToolStripMenuItem
      // 
      this.selectToolStripMenuItem.Name = "selectToolStripMenuItem";
      this.selectToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
      this.selectToolStripMenuItem.Text = "&Select";
      this.selectToolStripMenuItem.Click += new System.EventHandler(this.selectToolStripMenuItem_Click);
      // 
      // invertSelectionToolStripMenuItem
      // 
      this.invertSelectionToolStripMenuItem.Name = "invertSelectionToolStripMenuItem";
      this.invertSelectionToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
      this.invertSelectionToolStripMenuItem.Text = "Invert selection";
      this.invertSelectionToolStripMenuItem.Click += new System.EventHandler(this.invertSelectionToolStripMenuItem_Click);
      // 
      // filterToolStripMenuItem
      // 
      this.filterToolStripMenuItem.Name = "filterToolStripMenuItem";
      this.filterToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
      this.filterToolStripMenuItem.Text = "Hide selected";
      this.filterToolStripMenuItem.Click += new System.EventHandler(this.hideSelectedToolStripMenuItem_Click);
      // 
      // toolStripSeparator1
      // 
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      this.toolStripSeparator1.Size = new System.Drawing.Size(151, 6);
      // 
      // clearSelectionMenuItem
      // 
      this.clearSelectionMenuItem.Name = "clearSelectionMenuItem";
      this.clearSelectionMenuItem.Size = new System.Drawing.Size(154, 22);
      this.clearSelectionMenuItem.Text = "Clear selection";
      this.clearSelectionMenuItem.Click += new System.EventHandler(this.clearSelectionMenuItem_Click);
      // 
      // showHiddenToolStripMenuItem
      // 
      this.showHiddenToolStripMenuItem.Enabled = false;
      this.showHiddenToolStripMenuItem.Name = "showHiddenToolStripMenuItem";
      this.showHiddenToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
      this.showHiddenToolStripMenuItem.Text = "Show hidden";
      this.showHiddenToolStripMenuItem.Click += new System.EventHandler(this.showHiddenToolStripMenuItem_Click);
      // 
      // BubbleChartControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.Control;
      this.Controls.Add(this.pictureBox);
      this.Name = "BubbleChartControl";
      this.Size = new System.Drawing.Size(266, 236);
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
      this.pictureBoxContextMenuStrip.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    protected System.Windows.Forms.PictureBox pictureBox;
    protected System.Windows.Forms.ToolTip toolTip;
    protected System.Windows.Forms.ContextMenuStrip pictureBoxContextMenuStrip;
    protected System.Windows.Forms.ToolStripMenuItem zoomToolStripMenuItem;
    protected System.Windows.Forms.ToolStripMenuItem selectToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem moveToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem invertSelectionToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem filterToolStripMenuItem;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    private System.Windows.Forms.ToolStripMenuItem clearSelectionMenuItem;
    private System.Windows.Forms.ToolStripMenuItem showHiddenToolStripMenuItem;

  }
}
