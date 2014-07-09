#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views {
  public partial class SymbolicExpressionTreeChart {
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
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.saveImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.exportPgfLaTeXToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.layoutEngineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.reingoldTilfordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.boxesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
      this.contextMenuStrip.SuspendLayout();
      this.SuspendLayout();
      // 
      // contextMenuStrip
      // 
      this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveImageToolStripMenuItem,
            this.exportPgfLaTeXToolStripMenuItem,
            this.layoutEngineToolStripMenuItem});
      this.contextMenuStrip.Name = "contextMenuStrip";
      this.contextMenuStrip.Size = new System.Drawing.Size(166, 70);
      // 
      // saveImageToolStripMenuItem
      // 
      this.saveImageToolStripMenuItem.Name = "saveImageToolStripMenuItem";
      this.saveImageToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
      this.saveImageToolStripMenuItem.Text = "Save Image";
      this.saveImageToolStripMenuItem.Click += new System.EventHandler(this.saveImageToolStripMenuItem_Click);
      // 
      // exportPgfLaTeXToolStripMenuItem
      // 
      this.exportPgfLaTeXToolStripMenuItem.Name = "exportPgfLaTeXToolStripMenuItem";
      this.exportPgfLaTeXToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
      this.exportPgfLaTeXToolStripMenuItem.Text = "Export Pgf/LaTeX";
      this.exportPgfLaTeXToolStripMenuItem.Click += new System.EventHandler(this.exportLatexToolStripMenuItem_Click);
      // 
      // layoutEngineToolStripMenuItem
      // 
      this.layoutEngineToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reingoldTilfordToolStripMenuItem,
            this.boxesToolStripMenuItem});
      this.layoutEngineToolStripMenuItem.Name = "layoutEngineToolStripMenuItem";
      this.layoutEngineToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
      this.layoutEngineToolStripMenuItem.Text = "Layout Engine:";
      // 
      // reingoldTilfordToolStripMenuItem
      // 
      this.reingoldTilfordToolStripMenuItem.Name = "reingoldTilfordToolStripMenuItem";
      this.reingoldTilfordToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
      this.reingoldTilfordToolStripMenuItem.Text = "Reingold-Tilford";
      this.reingoldTilfordToolStripMenuItem.Click += new System.EventHandler(this.reingoldTilfordToolStripMenuItem_Click);
      // 
      // boxesToolStripMenuItem
      // 
      this.boxesToolStripMenuItem.Name = "boxesToolStripMenuItem";
      this.boxesToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
      this.boxesToolStripMenuItem.Text = "Boxes";
      this.boxesToolStripMenuItem.Click += new System.EventHandler(this.boxesToolStripMenuItem_Click);
      // 
      // saveFileDialog
      // 
      this.saveFileDialog.Filter = "Bitmap (*.bmp)|*.bmp|EMF (*.emf)|*.emf";
      // 
      // SymbolicExpressionTreeChart
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.ContextMenuStrip = this.contextMenuStrip;
      this.DoubleBuffered = true;
      this.Name = "SymbolicExpressionTreeChart";
      this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.SymbolicExpressionTreeChart_MouseClick);
      this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.SymbolicExpressionTreeChart_MouseDoubleClick);
      this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SymbolicExpressionTreeChart_MouseDown);
      this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SymbolicExpressionTreeChart_MouseMove);
      this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.SymbolicExpressionTreeChart_MouseUp);
      this.contextMenuStrip.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    protected System.Windows.Forms.ToolTip toolTip;
    protected System.Windows.Forms.ContextMenuStrip contextMenuStrip;
    protected System.Windows.Forms.ToolStripMenuItem saveImageToolStripMenuItem;
    protected System.Windows.Forms.SaveFileDialog saveFileDialog;
    private System.Windows.Forms.ToolStripMenuItem exportPgfLaTeXToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem layoutEngineToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem reingoldTilfordToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem boxesToolStripMenuItem;
  }
}
