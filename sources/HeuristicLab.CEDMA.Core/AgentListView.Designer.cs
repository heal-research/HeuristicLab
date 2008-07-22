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

using System;
using System.Windows.Forms;

namespace HeuristicLab.CEDMA.Core {
  partial class AgentListView {
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

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      this.agentsGroupBox = new System.Windows.Forms.GroupBox();
      this.detailsGroupBox = new System.Windows.Forms.GroupBox();
      this.addButton = new System.Windows.Forms.Button();
      this.agentTreeView = new System.Windows.Forms.TreeView();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.agentsGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // splitContainer1
      // 
      this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
      this.splitContainer1.Location = new System.Drawing.Point(0, 0);
      this.splitContainer1.Name = "splitContainer1";
      // 
      // splitContainer1.Panel1
      // 
      this.splitContainer1.Panel1.Controls.Add(this.agentsGroupBox);
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.detailsGroupBox);
      this.splitContainer1.Size = new System.Drawing.Size(276, 155);
      this.splitContainer1.SplitterDistance = 135;
      this.splitContainer1.TabIndex = 0;
      // 
      // agentsGroupBox
      // 
      this.agentsGroupBox.Controls.Add(this.agentTreeView);
      this.agentsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.agentsGroupBox.Location = new System.Drawing.Point(0, 0);
      this.agentsGroupBox.Name = "agentsGroupBox";
      this.agentsGroupBox.Size = new System.Drawing.Size(135, 155);
      this.agentsGroupBox.TabIndex = 0;
      this.agentsGroupBox.TabStop = false;
      this.agentsGroupBox.Text = "&Agents";
      // 
      // detailsGroupBox
      // 
      this.detailsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.detailsGroupBox.Location = new System.Drawing.Point(0, 0);
      this.detailsGroupBox.Name = "detailsGroupBox";
      this.detailsGroupBox.Size = new System.Drawing.Size(137, 155);
      this.detailsGroupBox.TabIndex = 0;
      this.detailsGroupBox.TabStop = false;
      this.detailsGroupBox.Text = "&Details";
      // 
      // addButton
      // 
      this.addButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.addButton.Location = new System.Drawing.Point(0, 161);
      this.addButton.Name = "addButton";
      this.addButton.Size = new System.Drawing.Size(75, 23);
      this.addButton.TabIndex = 1;
      this.addButton.Text = "&Add...";
      this.addButton.UseVisualStyleBackColor = true;
      this.addButton.Click += new System.EventHandler(this.addButton_Click);
      // 
      // agentTreeView
      // 
      this.agentTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.agentTreeView.Location = new System.Drawing.Point(3, 16);
      this.agentTreeView.Name = "agentTreeView";
      this.agentTreeView.Size = new System.Drawing.Size(129, 136);
      this.agentTreeView.TabIndex = 0;
      this.agentTreeView.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.agentTreeView_BeforeExpand);
      // 
      // AgentListView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.Controls.Add(this.addButton);
      this.Controls.Add(this.splitContainer1);
      this.Name = "AgentListView";
      this.Size = new System.Drawing.Size(276, 184);
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      this.splitContainer1.ResumeLayout(false);
      this.agentsGroupBox.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.GroupBox agentsGroupBox;
    private System.Windows.Forms.GroupBox detailsGroupBox;
    private System.Windows.Forms.Button addButton;
    private TreeView agentTreeView;
  }
}
