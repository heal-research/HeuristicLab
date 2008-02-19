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

namespace HeuristicLab.Charting {
  partial class ChartControl {
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
      this.pictureBox = new System.Windows.Forms.PictureBox();
      this.pictureBoxContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.moveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.zoomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.selectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
      this.oneLayerUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.oneLayerDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.intoForegroundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.intoBackgroundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
      this.propertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
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
      this.pictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseDown);
      this.pictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseMove);
      this.pictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseUp);
      this.pictureBox.SizeChanged += new System.EventHandler(this.pictureBox_SizeChanged);
      // 
      // pictureBoxContextMenuStrip
      // 
      this.pictureBoxContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.moveToolStripMenuItem,
            this.zoomToolStripMenuItem,
            this.selectToolStripMenuItem,
            this.toolStripMenuItem1,
            this.oneLayerUpToolStripMenuItem,
            this.oneLayerDownToolStripMenuItem,
            this.intoForegroundToolStripMenuItem,
            this.intoBackgroundToolStripMenuItem,
            this.toolStripMenuItem2,
            this.propertiesToolStripMenuItem});
      this.pictureBoxContextMenuStrip.Name = "pictureBoxContextMenuStrip";
      this.pictureBoxContextMenuStrip.Size = new System.Drawing.Size(165, 192);
      // 
      // moveToolStripMenuItem
      // 
      this.moveToolStripMenuItem.Checked = true;
      this.moveToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
      this.moveToolStripMenuItem.Name = "moveToolStripMenuItem";
      this.moveToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
      this.moveToolStripMenuItem.Text = "&Move";
      this.moveToolStripMenuItem.Click += new System.EventHandler(this.moveToolStripMenuItem_Click);
      // 
      // zoomToolStripMenuItem
      // 
      this.zoomToolStripMenuItem.Name = "zoomToolStripMenuItem";
      this.zoomToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
      this.zoomToolStripMenuItem.Text = "&Zoom";
      this.zoomToolStripMenuItem.Click += new System.EventHandler(this.zoomToolStripMenuItem_Click);
      // 
      // selectToolStripMenuItem
      // 
      this.selectToolStripMenuItem.Name = "selectToolStripMenuItem";
      this.selectToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
      this.selectToolStripMenuItem.Text = "&Select";
      this.selectToolStripMenuItem.Click += new System.EventHandler(this.selectToolStripMenuItem_Click);
      // 
      // toolStripMenuItem1
      // 
      this.toolStripMenuItem1.Name = "toolStripMenuItem1";
      this.toolStripMenuItem1.Size = new System.Drawing.Size(161, 6);
      // 
      // oneLayerUpToolStripMenuItem
      // 
      this.oneLayerUpToolStripMenuItem.Enabled = false;
      this.oneLayerUpToolStripMenuItem.Name = "oneLayerUpToolStripMenuItem";
      this.oneLayerUpToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
      this.oneLayerUpToolStripMenuItem.Text = "One Layer &Up";
      this.oneLayerUpToolStripMenuItem.Click += new System.EventHandler(this.oneLayerUpToolStripMenuItem_Click);
      // 
      // oneLayerDownToolStripMenuItem
      // 
      this.oneLayerDownToolStripMenuItem.Enabled = false;
      this.oneLayerDownToolStripMenuItem.Name = "oneLayerDownToolStripMenuItem";
      this.oneLayerDownToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
      this.oneLayerDownToolStripMenuItem.Text = "OneLayer &Down";
      this.oneLayerDownToolStripMenuItem.Click += new System.EventHandler(this.oneLayerDownToolStripMenuItem_Click);
      // 
      // intoForegroundToolStripMenuItem
      // 
      this.intoForegroundToolStripMenuItem.Enabled = false;
      this.intoForegroundToolStripMenuItem.Name = "intoForegroundToolStripMenuItem";
      this.intoForegroundToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
      this.intoForegroundToolStripMenuItem.Text = "Into &Foreground";
      this.intoForegroundToolStripMenuItem.Click += new System.EventHandler(this.intoForegroundToolStripMenuItem_Click);
      // 
      // intoBackgroundToolStripMenuItem
      // 
      this.intoBackgroundToolStripMenuItem.Enabled = false;
      this.intoBackgroundToolStripMenuItem.Name = "intoBackgroundToolStripMenuItem";
      this.intoBackgroundToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
      this.intoBackgroundToolStripMenuItem.Text = "Into &Background";
      this.intoBackgroundToolStripMenuItem.Click += new System.EventHandler(this.intoBackgroundToolStripMenuItem_Click);
      // 
      // toolStripMenuItem2
      // 
      this.toolStripMenuItem2.Name = "toolStripMenuItem2";
      this.toolStripMenuItem2.Size = new System.Drawing.Size(161, 6);
      // 
      // propertiesToolStripMenuItem
      // 
      this.propertiesToolStripMenuItem.Enabled = false;
      this.propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
      this.propertiesToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
      this.propertiesToolStripMenuItem.Text = "&Properties";
      this.propertiesToolStripMenuItem.Click += new System.EventHandler(this.propertiesToolStripMenuItem_Click);
      // 
      // ChartControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.Control;
      this.Controls.Add(this.pictureBox);
      this.Name = "ChartControl";
      this.Size = new System.Drawing.Size(266, 236);
      this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ChartControl_KeyDown);
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
      this.pictureBoxContextMenuStrip.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    protected System.Windows.Forms.PictureBox pictureBox;
    protected System.Windows.Forms.ToolTip toolTip;
    protected System.Windows.Forms.ContextMenuStrip pictureBoxContextMenuStrip;
    protected System.Windows.Forms.ToolStripMenuItem moveToolStripMenuItem;
    protected System.Windows.Forms.ToolStripMenuItem zoomToolStripMenuItem;
    protected System.Windows.Forms.ToolStripMenuItem selectToolStripMenuItem;
    protected System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
    protected System.Windows.Forms.ToolStripMenuItem propertiesToolStripMenuItem;
    protected System.Windows.Forms.ToolStripMenuItem oneLayerUpToolStripMenuItem;
    protected System.Windows.Forms.ToolStripMenuItem oneLayerDownToolStripMenuItem;
    protected System.Windows.Forms.ToolStripMenuItem intoForegroundToolStripMenuItem;
    protected System.Windows.Forms.ToolStripMenuItem intoBackgroundToolStripMenuItem;
    protected System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;

  }
}
