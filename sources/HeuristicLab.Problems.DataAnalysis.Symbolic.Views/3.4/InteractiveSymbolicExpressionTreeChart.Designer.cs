#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
      this.DoubleBuffered = true;
      this.insertNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.changeValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.copyNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.copySubtreeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.cutNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.cutSubtreeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.deleteNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.deleteSubtreeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.treeStatusLabel = new System.Windows.Forms.Label();
      this.treeStatusValue = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // insertNodeToolStripMenuItem
      // 
      this.insertNodeToolStripMenuItem.Name = "insertNodeToolStripMenuItem";
      this.insertNodeToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
      this.insertNodeToolStripMenuItem.Text = "Insert Node";
      this.insertNodeToolStripMenuItem.Click += new System.EventHandler(this.insertNodeToolStripMenuItem_Click);
      // 
      // changeValueToolStripMenuItem
      // 
      this.changeValueToolStripMenuItem.Name = "changeValueToolStripMenuItem";
      this.changeValueToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
      this.changeValueToolStripMenuItem.Text = "Change Value";
      this.changeValueToolStripMenuItem.Click += new System.EventHandler(this.changeValueToolStripMenuItem_Click);
      // 
      // copyToolStripMenuItem
      // 
      this.copyToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyNodeToolStripMenuItem,
            this.copySubtreeToolStripMenuItem});
      this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
      this.copyToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
      this.copyToolStripMenuItem.Text = "Copy";
      // 
      // copyNodeToolStripMenuItem
      // 
      this.copyNodeToolStripMenuItem.Name = "copyNodeToolStripMenuItem";
      this.copyNodeToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
      this.copyNodeToolStripMenuItem.Text = "Node";
      this.copyNodeToolStripMenuItem.Click += new System.EventHandler(this.copyNodeToolStripMenuItem_Click);
      // 
      // copySubtreeToolStripMenuItem
      // 
      this.copySubtreeToolStripMenuItem.Name = "copySubtreeToolStripMenuItem";
      this.copySubtreeToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
      this.copySubtreeToolStripMenuItem.Text = "Subtree";
      this.copySubtreeToolStripMenuItem.Click += new System.EventHandler(this.copySubtreeToolStripMenuItem_Click);
      // 
      // cutToolStripMenuItem
      // 
      this.cutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cutNodeToolStripMenuItem,
            this.cutSubtreeToolStripMenuItem});
      this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
      this.cutToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
      this.cutToolStripMenuItem.Text = "Cut";
      // 
      // cutNodeToolStripMenuItem
      // 
      this.cutNodeToolStripMenuItem.Name = "cutNodeToolStripMenuItem";
      this.cutNodeToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
      this.cutNodeToolStripMenuItem.Text = "Node";
      this.cutNodeToolStripMenuItem.Click += new System.EventHandler(this.cutNodeToolStripMenuItem_Click);
      // 
      // cutSubtreeToolStripMenuItem
      // 
      this.cutSubtreeToolStripMenuItem.Name = "cutSubtreeToolStripMenuItem";
      this.cutSubtreeToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
      this.cutSubtreeToolStripMenuItem.Text = "Subtree";
      this.cutSubtreeToolStripMenuItem.Click += new System.EventHandler(this.cutSubtreeToolStripMenuItem_Click);
      // 
      // deleteToolStripMenuItem
      // 
      this.deleteToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteNodeToolStripMenuItem,
            this.deleteSubtreeToolStripMenuItem});
      this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
      this.deleteToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
      this.deleteToolStripMenuItem.Text = "Delete";
      // 
      // deleteNodeToolStripMenuItem
      // 
      this.deleteNodeToolStripMenuItem.Name = "deleteNodeToolStripMenuItem";
      this.deleteNodeToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
      this.deleteNodeToolStripMenuItem.Text = "Node";
      this.deleteNodeToolStripMenuItem.Click += new System.EventHandler(this.deleteNodeToolStripMenuItem_Click);
      // 
      // deleteSubtreeToolStripMenuItem
      // 
      this.deleteSubtreeToolStripMenuItem.Name = "deleteSubtreeToolStripMenuItem";
      this.deleteSubtreeToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
      this.deleteSubtreeToolStripMenuItem.Text = "Subtree";
      this.deleteSubtreeToolStripMenuItem.Click += new System.EventHandler(this.deleteSubtreeToolStripMenuItem_Click);
      // 
      // pasteToolStripMenuItem
      // 
      this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
      this.pasteToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
      this.pasteToolStripMenuItem.Text = "Paste";
      this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Clicked);
      //
      // contextMenuStrip
      //
      this.contextMenuStrip.Opened += this.contextMenuStrip_Opened;
      this.contextMenuStrip.Items.AddRange(new ToolStripItem[] { insertNodeToolStripMenuItem,
                                                                 changeValueToolStripMenuItem, 
                                                                 copyToolStripMenuItem, 
                                                                 cutToolStripMenuItem, 
                                                                 deleteToolStripMenuItem, 
                                                                 pasteToolStripMenuItem });
      // 
      // treeStatusLabel
      // 
      this.treeStatusLabel.AutoSize = true;
      this.treeStatusLabel.BackColor = System.Drawing.Color.Transparent;
      this.treeStatusLabel.Location = new System.Drawing.Point(3, 0);
      this.treeStatusLabel.Name = "treeStatusLabel";
      this.treeStatusLabel.Size = new System.Drawing.Size(68, 13);
      this.treeStatusLabel.TabIndex = 0;
      this.treeStatusLabel.Text = "Tree Status: ";
      // 
      // treeStatusValue
      // 
      this.treeStatusValue.AutoSize = true;
      this.treeStatusValue.BackColor = System.Drawing.Color.Transparent;
      this.treeStatusValue.ForeColor = System.Drawing.Color.Green;
      this.treeStatusValue.Location = new System.Drawing.Point(77, 0);
      this.treeStatusValue.Name = "treeStatusValue";
      this.treeStatusValue.Size = new System.Drawing.Size(30, 13);
      this.treeStatusValue.TabIndex = 1;
      this.treeStatusValue.Text = "Valid";
      // 
      // InteractiveSymbolicExpressionTreeChart
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.treeStatusLabel);
      this.Controls.Add(this.treeStatusValue);
      this.Name = "InteractiveSymbolicExpressionTreeChart";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    private ToolStripMenuItem insertNodeToolStripMenuItem;
    private ToolStripMenuItem changeValueToolStripMenuItem;
    private ToolStripMenuItem copyToolStripMenuItem;
    private ToolStripMenuItem copyNodeToolStripMenuItem;
    private ToolStripMenuItem copySubtreeToolStripMenuItem;
    private ToolStripMenuItem cutToolStripMenuItem;
    private ToolStripMenuItem cutNodeToolStripMenuItem;
    private ToolStripMenuItem cutSubtreeToolStripMenuItem;
    private ToolStripMenuItem pasteToolStripMenuItem;
    private ToolStripMenuItem deleteToolStripMenuItem;
    private ToolStripMenuItem deleteNodeToolStripMenuItem;
    private ToolStripMenuItem deleteSubtreeToolStripMenuItem;

    #endregion
    private Label treeStatusLabel;
    private Label treeStatusValue;
  }
}
