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

namespace HeuristicLab.Core.Views {
  partial class OperatorGraphView {
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
      this.operatorsContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.initialOperatorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.operatorsView = new HeuristicLab.Core.Views.OperatorSetView();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.graphGroupBox = new System.Windows.Forms.GroupBox();
      this.operatorTreeView = new HeuristicLab.Core.Views.OperatorTreeView();
      this.operatorsContextMenuStrip.SuspendLayout();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.graphGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // operatorsContextMenuStrip
      // 
      this.operatorsContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.initialOperatorToolStripMenuItem});
      this.operatorsContextMenuStrip.Name = "operatorsContextMenuStrip";
      this.operatorsContextMenuStrip.Size = new System.Drawing.Size(154, 26);
      this.operatorsContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.operatorsContextMenuStrip_Opening);
      // 
      // initialOperatorToolStripMenuItem
      // 
      this.initialOperatorToolStripMenuItem.CheckOnClick = true;
      this.initialOperatorToolStripMenuItem.Name = "initialOperatorToolStripMenuItem";
      this.initialOperatorToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
      this.initialOperatorToolStripMenuItem.Text = "&Initial Operator";
      this.initialOperatorToolStripMenuItem.ToolTipText = "Set as initial operator";
      this.initialOperatorToolStripMenuItem.Click += new System.EventHandler(this.initialOperatorToolStripMenuItem_Click);
      // 
      // operatorsView
      // 
      this.operatorsView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.operatorsView.Content = null;
      this.operatorsView.Location = new System.Drawing.Point(3, 3);
      this.operatorsView.Name = "operatorsView";
      this.operatorsView.Size = new System.Drawing.Size(602, 309);
      this.operatorsView.TabIndex = 0;
      this.operatorsView.Load += new System.EventHandler(this.operatorsView_Load);
      // 
      // splitContainer
      // 
      this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer.Location = new System.Drawing.Point(0, 0);
      this.splitContainer.Name = "splitContainer";
      this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.operatorsView);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.graphGroupBox);
      this.splitContainer.Size = new System.Drawing.Size(608, 533);
      this.splitContainer.SplitterDistance = 315;
      this.splitContainer.TabIndex = 3;
      // 
      // graphGroupBox
      // 
      this.graphGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.graphGroupBox.Controls.Add(this.operatorTreeView);
      this.graphGroupBox.Location = new System.Drawing.Point(3, 3);
      this.graphGroupBox.Name = "graphGroupBox";
      this.graphGroupBox.Size = new System.Drawing.Size(602, 208);
      this.graphGroupBox.TabIndex = 0;
      this.graphGroupBox.TabStop = false;
      this.graphGroupBox.Text = "Operator Graph";
      // 
      // operatorTreeView
      // 
      this.operatorTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.operatorTreeView.Content = null;
      this.operatorTreeView.Location = new System.Drawing.Point(6, 19);
      this.operatorTreeView.Name = "operatorTreeView";
      this.operatorTreeView.Size = new System.Drawing.Size(590, 183);
      this.operatorTreeView.TabIndex = 0;
      this.operatorTreeView.SelectedOperatorChanged += new System.EventHandler(this.operatorTreeView_SelectedOperatorChanged);
      // 
      // OperatorGraphView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.splitContainer);
      this.Name = "OperatorGraphView";
      this.Size = new System.Drawing.Size(608, 533);
      this.operatorsContextMenuStrip.ResumeLayout(false);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      this.splitContainer.ResumeLayout(false);
      this.graphGroupBox.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    protected System.Windows.Forms.ContextMenuStrip operatorsContextMenuStrip;
    protected System.Windows.Forms.ToolStripMenuItem initialOperatorToolStripMenuItem;
    protected OperatorSetView operatorsView;
    protected System.Windows.Forms.SplitContainer splitContainer;
    protected System.Windows.Forms.GroupBox graphGroupBox;
    private OperatorTreeView operatorTreeView;
  }
}
