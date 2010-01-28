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

namespace HeuristicLab.Core.Views {
  partial class ViewHost {
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
      this.viewPanel = new System.Windows.Forms.Panel();
      this.messageLabel = new System.Windows.Forms.Label();
      this.viewsLabel = new System.Windows.Forms.Label();
      this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.SuspendLayout();
      // 
      // viewPanel
      // 
      this.viewPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.viewPanel.Location = new System.Drawing.Point(0, 0);
      this.viewPanel.Name = "viewPanel";
      this.viewPanel.Size = new System.Drawing.Size(205, 184);
      this.viewPanel.TabIndex = 1;
      // 
      // messageLabel
      // 
      this.messageLabel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.messageLabel.Location = new System.Drawing.Point(0, 0);
      this.messageLabel.Name = "messageLabel";
      this.messageLabel.Size = new System.Drawing.Size(227, 184);
      this.messageLabel.TabIndex = 2;
      this.messageLabel.Text = "No view available.";
      this.messageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // viewsLabel
      // 
      this.viewsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.viewsLabel.ContextMenuStrip = this.contextMenuStrip;
      this.viewsLabel.Image = HeuristicLab.Common.Resources.VS2008ImageLibrary.Windows;
      this.viewsLabel.Location = new System.Drawing.Point(211, 0);
      this.viewsLabel.Name = "viewsLabel";
      this.viewsLabel.Size = new System.Drawing.Size(16, 16);
      this.viewsLabel.TabIndex = 0;
      this.toolTip.SetToolTip(this.viewsLabel, "Double-click to open a new window of the current view.\r\nRight-click to change cur" +
              "rent view.");
      this.viewsLabel.DoubleClick += new System.EventHandler(this.viewsLabel_DoubleClick);
      // 
      // contextMenuStrip
      // 
      this.contextMenuStrip.Name = "contextMenuStrip";
      this.contextMenuStrip.ShowCheckMargin = true;
      this.contextMenuStrip.ShowImageMargin = false;
      this.contextMenuStrip.Size = new System.Drawing.Size(61, 4);
      this.contextMenuStrip.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStrip_ItemClicked);
      // 
      // ViewHost
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.viewPanel);
      this.Controls.Add(this.viewsLabel);
      this.Controls.Add(this.messageLabel);
      this.Name = "ViewHost";
      this.Size = new System.Drawing.Size(227, 184);
      this.ResumeLayout(false);

    }

    #endregion

    protected System.Windows.Forms.Panel viewPanel;
    protected System.Windows.Forms.Label viewsLabel;
    protected System.Windows.Forms.Label messageLabel;
    protected System.Windows.Forms.ContextMenuStrip contextMenuStrip;
    protected System.Windows.Forms.ToolTip toolTip;

  }
}
