#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.DebugEngine {
  partial class ExecutionStackView {
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
      this.treeView = new System.Windows.Forms.TreeView();
      this.imageList = new System.Windows.Forms.ImageList(this.components);
      this.groupBox = new System.Windows.Forms.GroupBox();
      this.groupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // treeView
      // 
      this.treeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.treeView.ImageIndex = 0;
      this.treeView.ImageList = this.imageList;
      this.treeView.Location = new System.Drawing.Point(6, 19);
      this.treeView.Name = "treeView";
      this.treeView.SelectedImageIndex = 0;
      this.treeView.ShowNodeToolTips = true;
      this.treeView.Size = new System.Drawing.Size(297, 255);
      this.treeView.TabIndex = 0;
      this.treeView.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView_NodeMouseDoubleClick);
      // 
      // imageList
      // 
      this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
      this.imageList.ImageSize = new System.Drawing.Size(16, 16);
      this.imageList.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // groupBox
      // 
      this.groupBox.Controls.Add(this.treeView);
      this.groupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox.Location = new System.Drawing.Point(0, 0);
      this.groupBox.Name = "groupBox";
      this.groupBox.Size = new System.Drawing.Size(309, 280);
      this.groupBox.TabIndex = 1;
      this.groupBox.TabStop = false;
      this.groupBox.Text = "Execution Stack";
      // 
      // ExecutionStackView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.groupBox);
      this.Name = "ExecutionStackView";
      this.Size = new System.Drawing.Size(309, 280);
      this.groupBox.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TreeView treeView;
    private System.Windows.Forms.GroupBox groupBox;
    private System.Windows.Forms.ImageList imageList;

  }
}
