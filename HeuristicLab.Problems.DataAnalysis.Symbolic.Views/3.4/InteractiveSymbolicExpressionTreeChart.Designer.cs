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

using System.Windows.Forms;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Views {
  partial class InteractiveSymbolicExpressionTreeChart {
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
      this.insertNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.changeNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.removeNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.removeSubtreeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
      this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
      this.SuspendLayout();
      // 
      // insertNodeToolStripMenuItem
      // 
      this.insertNodeToolStripMenuItem.Name = "insertNodeToolStripMenuItem";
      this.insertNodeToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
      this.insertNodeToolStripMenuItem.Text = "Insert Node";
      this.insertNodeToolStripMenuItem.Click += new System.EventHandler(this.insertNodeToolStripMenuItem_Click);
      // 
      // changeNodeToolStripMenuItem
      // 
      this.changeNodeToolStripMenuItem.Name = "changeNodeToolStripMenuItem";
      this.changeNodeToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
      this.changeNodeToolStripMenuItem.Text = "Change Node";
      this.changeNodeToolStripMenuItem.Click += new System.EventHandler(this.changeNodeToolStripMenuItem_Click);
      // 
      // cutToolStripMenuItem
      // 
      this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
      this.cutToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
      this.cutToolStripMenuItem.Text = "Cut Subtree";
      this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
      // 
      // removeToolStripMenuItem
      // 
      this.removeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeNodeToolStripMenuItem,
            this.removeSubtreeToolStripMenuItem});
      this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
      this.removeToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
      this.removeToolStripMenuItem.Text = "Remove";
      this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeSubtreeToolStripMenuItem_Click);
      // 
      // removeNodeToolStripMenuItem
      // 
      this.removeNodeToolStripMenuItem.Name = "removeNodeToolStripMenuItem";
      this.removeNodeToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
      this.removeNodeToolStripMenuItem.Text = "Node";
      this.removeNodeToolStripMenuItem.Click += new System.EventHandler(this.removeNodeToolStripMenuItem_Click);
      // 
      // removeSubtreeToolStripMenuItem
      // 
      this.removeSubtreeToolStripMenuItem.Name = "removeSubtreeToolStripMenuItem";
      this.removeSubtreeToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
      this.removeSubtreeToolStripMenuItem.Text = "Subtree";
      this.removeSubtreeToolStripMenuItem.Click += new System.EventHandler(this.removeSubtreeToolStripMenuItem_Click);
      // 
      // pasteToolStripMenuItem
      // 
      this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
      this.pasteToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
      this.pasteToolStripMenuItem.Text = "Paste Subtree";
      this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Clicked);
      // 
      // toolStripSeparator
      // 
      this.toolStripSeparator.Name = "toolStripSeparator";
      this.toolStripSeparator.Size = new System.Drawing.Size(149, 6);
      // 
      // copyToolStripMenuItem
      // 
      this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
      this.copyToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
      this.copyToolStripMenuItem.Text = "Copy Subtree";
      this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
      // 
      // toolStripSeparator1
      // 
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      this.toolStripSeparator1.Size = new System.Drawing.Size(162, 6);
      // 
      // toolStripSeparator2
      // 
      this.toolStripSeparator2.Name = "toolStripSeparator2";
      this.toolStripSeparator2.Size = new System.Drawing.Size(162, 6);
      //
      // contextMenuStrip
      //
      this.contextMenuStrip.Opened += this.contextMenuStrip_Opened;
      this.contextMenuStrip.Items.AddRange(new ToolStripItem[] { toolStripSeparator1,
                                                                 insertNodeToolStripMenuItem, 
                                                                 removeToolStripMenuItem, 
                                                                 changeNodeToolStripMenuItem, 
                                                                 toolStripSeparator2,
                                                                 copyToolStripMenuItem, 
                                                                 cutToolStripMenuItem, 
                                                                 pasteToolStripMenuItem });
      // 
      // InteractiveSymbolicExpressionTreeChart
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.DoubleBuffered = true;
      this.Name = "InteractiveSymbolicExpressionTreeChart";
      this.ResumeLayout(false);
    }

    private ToolStripMenuItem insertNodeToolStripMenuItem;
    private ToolStripMenuItem changeNodeToolStripMenuItem;
    private ToolStripMenuItem cutToolStripMenuItem;
    private ToolStripMenuItem pasteToolStripMenuItem;
    private ToolStripMenuItem removeToolStripMenuItem;
    private ToolStripMenuItem removeNodeToolStripMenuItem;
    private ToolStripMenuItem removeSubtreeToolStripMenuItem;

    #endregion

    private ToolStripSeparator toolStripSeparator;
    private ToolStripMenuItem copyToolStripMenuItem;
    private ToolStripSeparator toolStripSeparator1;
    private ToolStripSeparator toolStripSeparator2;
  }
}
