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

namespace HeuristicLab.MainForm.WindowsForms {
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
      this.messageLabel = new System.Windows.Forms.Label();
      this.viewsLabel = new System.Windows.Forms.Label();
      this.viewContextMenuStrip = new HeuristicLab.MainForm.WindowsForms.ViewContextMenuStrip();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.configurationLabel = new System.Windows.Forms.Label();
      this.SuspendLayout();
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
      this.viewsLabel.Image = HeuristicLab.Common.Resources.VSImageLibrary.Windows;
      this.viewsLabel.Location = new System.Drawing.Point(211, 0);
      this.viewsLabel.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
      this.viewsLabel.Name = "viewsLabel";
      this.viewsLabel.Size = new System.Drawing.Size(16, 16);
      this.viewsLabel.TabIndex = 0;
      this.toolTip.SetToolTip(this.viewsLabel,
        "Double-click to open a new window of the current view." + System.Environment.NewLine +
        "Right-click to change current view." + System.Environment.NewLine +
        "Drag icon to copy or link content to another view.");
      this.viewsLabel.DoubleClick += new System.EventHandler(this.viewsLabel_DoubleClick);
      this.viewsLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.viewsLabel_MouseDown);
      this.viewsLabel.MouseLeave += new System.EventHandler(this.viewsLabel_MouseLeave);
      // 
      // viewContextMenuStrip
      // 
      this.viewContextMenuStrip.IgnoredViewTypes = System.Type.EmptyTypes;
      this.viewContextMenuStrip.Item = null;
      this.viewContextMenuStrip.Name = "viewContextMenuStrip";
      this.viewContextMenuStrip.Size = new System.Drawing.Size(61, 4);
      this.viewContextMenuStrip.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.viewContextMenuStrip_ItemClicked);
      // 
      // configurationLabel
      // 
      this.configurationLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.configurationLabel.Image = HeuristicLab.Common.Resources.VSImageLibrary.EditInformation;
      this.configurationLabel.Location = new System.Drawing.Point(211, 22);
      this.configurationLabel.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
      this.configurationLabel.Name = "configurationLabel";
      this.configurationLabel.Size = new System.Drawing.Size(16, 16);
      this.configurationLabel.TabIndex = 1;
      this.toolTip.SetToolTip(this.configurationLabel, "Double-click to open view configuration.");
      this.configurationLabel.Visible = false;
      this.configurationLabel.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.configurationLabel_DoubleClick);
      // 
      // ViewHost
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.viewsLabel);
      this.Controls.Add(this.configurationLabel);
      this.Controls.Add(this.messageLabel);
      this.Name = "ViewHost";
      this.Size = new System.Drawing.Size(227, 184);
      this.ResumeLayout(false);

    }
    #endregion

    private System.Windows.Forms.Label viewsLabel;
    private System.Windows.Forms.Label messageLabel;
    private System.Windows.Forms.ToolTip toolTip;
    private HeuristicLab.MainForm.WindowsForms.ViewContextMenuStrip viewContextMenuStrip;
    private System.Windows.Forms.Label configurationLabel;

  }
}
