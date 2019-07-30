#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Visualization.ChartControlsExtensions {
  partial class EnhancedChart {
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
      this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.exportChartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.copyImageToClipboardBitmapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
      this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.contextMenuStrip.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
      this.SuspendLayout();
      // 
      // contextMenuStrip
      // 
      this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportChartToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.copyImageToClipboardBitmapToolStripMenuItem});
      this.contextMenuStrip.Name = "contextMenuStrip";
      this.contextMenuStrip.Size = new System.Drawing.Size(257, 70);
      // 
      // exportChartToolStripMenuItem
      // 
      this.exportChartToolStripMenuItem.Name = "exportChartToolStripMenuItem";
      this.exportChartToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
      this.exportChartToolStripMenuItem.Text = "Quick Export Chart";
      this.exportChartToolStripMenuItem.Click += new System.EventHandler(this.exportChartToolStripMenuItem_Click);
      // 
      // copyImageToClipboardBitmapToolStripMenuItem
      // 
      this.copyImageToClipboardBitmapToolStripMenuItem.Name = "copyImageToClipboardBitmapToolStripMenuItem";
      this.copyImageToClipboardBitmapToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
      this.copyImageToClipboardBitmapToolStripMenuItem.Text = "Copy Image to Clipboard (Bitmap)";
      this.copyImageToClipboardBitmapToolStripMenuItem.Click += new System.EventHandler(this.copyImageToClipboardBitmapToolStripMenuItem_Click);
      // 
      // saveFileDialog
      // 
      this.saveFileDialog.Filter = "Bitmap (*.bmp)|*.bmp|JPEG (*.jpg)|*.jpg|EMF (*.emf)|*.emf|PNG (*.png)|*.png|GIF (" +
          "*.gif)|*.gif|TIFF (*.tif)|*.tif\"";
      this.saveFileDialog.FilterIndex = 2;
      // 
      // exportToolStripMenuItem
      // 
      this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
      this.exportToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
      this.exportToolStripMenuItem.Text = "Export Chart...";
      this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
      // 
      // EnhancedChart
      // 
      this.ContextMenuStrip = this.contextMenuStrip;
      this.contextMenuStrip.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
    private System.Windows.Forms.ToolStripMenuItem exportChartToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem copyImageToClipboardBitmapToolStripMenuItem;
    private System.Windows.Forms.SaveFileDialog saveFileDialog;
    private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
  }
}
