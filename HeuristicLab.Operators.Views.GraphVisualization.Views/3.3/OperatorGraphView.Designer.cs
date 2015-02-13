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

namespace HeuristicLab.Operators.Views.GraphVisualization.Views {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OperatorGraphView));
      this.graphVisualizationInfoView = new HeuristicLab.Operators.Views.GraphVisualization.Views.GraphVisualizationInfoView();
      this.shapeContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.openViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.initialToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.breakPointToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.screenshotButton = new System.Windows.Forms.Button();
      this.zoomOutButton = new System.Windows.Forms.Button();
      this.zoomInButton = new System.Windows.Forms.Button();
      this.zoomToFitButton = new System.Windows.Forms.Button();
      this.relayoutButton = new System.Windows.Forms.Button();
      this.connectButton = new System.Windows.Forms.Button();
      this.selectButton = new System.Windows.Forms.Button();
      this.detailsGroupBox = new System.Windows.Forms.GroupBox();
      this.detailsViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.buttonToolTip = new System.Windows.Forms.ToolTip(this.components);
      this.shapeContextMenu.SuspendLayout();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.detailsGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // graphVisualizationInfoView
      // 
      this.graphVisualizationInfoView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.graphVisualizationInfoView.Content = null;
      this.graphVisualizationInfoView.Location = new System.Drawing.Point(3, 30);
      this.graphVisualizationInfoView.Name = "graphVisualizationInfoView";
      this.graphVisualizationInfoView.ReadOnly = false;
      this.graphVisualizationInfoView.Size = new System.Drawing.Size(662, 248);
      this.graphVisualizationInfoView.TabIndex = 0;
      // 
      // shapeContextMenu
      // 
      this.shapeContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openViewToolStripMenuItem,
            this.initialToolStripMenuItem,
            this.breakPointToolStripMenuItem});
      this.shapeContextMenu.Name = "shapeContextMenu";
      this.shapeContextMenu.Size = new System.Drawing.Size(154, 70);
      this.shapeContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.shapeContextMenu_Opening);
      // 
      // openViewToolStripMenuItem
      // 
      this.openViewToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
      this.openViewToolStripMenuItem.Name = "openViewToolStripMenuItem";
      this.openViewToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
      this.openViewToolStripMenuItem.Text = "Open View";
      this.openViewToolStripMenuItem.Click += new System.EventHandler(this.openViewToolStripMenuItem_Click);
      // 
      // initialToolStripMenuItem
      // 
      this.initialToolStripMenuItem.Name = "initialToolStripMenuItem";
      this.initialToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
      this.initialToolStripMenuItem.Text = "Initial Operator";
      this.initialToolStripMenuItem.Click += new System.EventHandler(this.initialOperatorToolStripMenuItem_Click);
      // 
      // breakPointToolStripMenuItem
      // 
      this.breakPointToolStripMenuItem.Name = "breakPointToolStripMenuItem";
      this.breakPointToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
      this.breakPointToolStripMenuItem.Text = "Breakpoint";
      this.breakPointToolStripMenuItem.Click += new System.EventHandler(this.breakPointToolStripMenuItem_Click);
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
      this.splitContainer.Panel1.Controls.Add(this.screenshotButton);
      this.splitContainer.Panel1.Controls.Add(this.zoomOutButton);
      this.splitContainer.Panel1.Controls.Add(this.zoomInButton);
      this.splitContainer.Panel1.Controls.Add(this.zoomToFitButton);
      this.splitContainer.Panel1.Controls.Add(this.relayoutButton);
      this.splitContainer.Panel1.Controls.Add(this.connectButton);
      this.splitContainer.Panel1.Controls.Add(this.selectButton);
      this.splitContainer.Panel1.Controls.Add(this.graphVisualizationInfoView);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.detailsGroupBox);
      this.splitContainer.Size = new System.Drawing.Size(665, 444);
      this.splitContainer.SplitterDistance = 279;
      this.splitContainer.TabIndex = 1;
      // 
      // screenshotButton
      // 
      this.screenshotButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Image;
      this.screenshotButton.Location = new System.Drawing.Point(213, 3);
      this.screenshotButton.Name = "screenshotButton";
      this.screenshotButton.Size = new System.Drawing.Size(24, 24);
      this.screenshotButton.TabIndex = 7;
      this.buttonToolTip.SetToolTip(this.screenshotButton, "Screenshot");
      this.screenshotButton.UseVisualStyleBackColor = true;
      this.screenshotButton.Click += new System.EventHandler(this.screenshotButton_Click);
      // 
      // zoomOutButton
      // 
      this.zoomOutButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.ZoomOut;
      this.zoomOutButton.Location = new System.Drawing.Point(183, 3);
      this.zoomOutButton.Name = "zoomOutButton";
      this.zoomOutButton.Size = new System.Drawing.Size(24, 24);
      this.zoomOutButton.TabIndex = 6;
      this.buttonToolTip.SetToolTip(this.zoomOutButton, "Zoom Out");
      this.zoomOutButton.UseVisualStyleBackColor = true;
      this.zoomOutButton.Click += new System.EventHandler(this.zoomOutButton_Click);
      // 
      // zoomInButton
      // 
      this.zoomInButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.ZoomIn;
      this.zoomInButton.Location = new System.Drawing.Point(153, 3);
      this.zoomInButton.Name = "zoomInButton";
      this.zoomInButton.Size = new System.Drawing.Size(24, 24);
      this.zoomInButton.TabIndex = 5;
      this.buttonToolTip.SetToolTip(this.zoomInButton, "Zoom In");
      this.zoomInButton.UseVisualStyleBackColor = true;
      this.zoomInButton.Click += new System.EventHandler(this.zoomInButton_Click);
      // 
      // zoomAreaButton
      // 
      this.zoomToFitButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.ActualSize;
      this.zoomToFitButton.Location = new System.Drawing.Point(123, 3);
      this.zoomToFitButton.Name = "zoomAreaButton";
      this.zoomToFitButton.Size = new System.Drawing.Size(24, 24);
      this.zoomToFitButton.TabIndex = 4;
      this.buttonToolTip.SetToolTip(this.zoomToFitButton, "Zoom to Fit");
      this.zoomToFitButton.UseVisualStyleBackColor = true;
      this.zoomToFitButton.Click += new System.EventHandler(this.zoomAreaButton_Click);
      // 
      // relayoutButton
      // 
      this.relayoutButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.RefreshDocument;
      this.relayoutButton.Location = new System.Drawing.Point(93, 3);
      this.relayoutButton.Name = "relayoutButton";
      this.relayoutButton.Size = new System.Drawing.Size(24, 24);
      this.relayoutButton.TabIndex = 3;
      this.buttonToolTip.SetToolTip(this.relayoutButton, "Relayout Graph");
      this.relayoutButton.UseVisualStyleBackColor = true;
      this.relayoutButton.Click += new System.EventHandler(this.relayoutButton_Click);
      // 
      // connectButton
      // 
      this.connectButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Interface;
      this.connectButton.Location = new System.Drawing.Point(33, 3);
      this.connectButton.Name = "connectButton";
      this.connectButton.Size = new System.Drawing.Size(24, 24);
      this.connectButton.TabIndex = 2;
      this.buttonToolTip.SetToolTip(this.connectButton, "Connection Tool");
      this.connectButton.UseVisualStyleBackColor = true;
      this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
      // 
      // selectButton
      // 
      this.selectButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Pointer;
      this.selectButton.Location = new System.Drawing.Point(3, 3);
      this.selectButton.Name = "selectButton";
      this.selectButton.Size = new System.Drawing.Size(24, 24);
      this.selectButton.TabIndex = 1;
      this.buttonToolTip.SetToolTip(this.selectButton, "Select Tool");
      this.selectButton.UseVisualStyleBackColor = true;
      this.selectButton.Click += new System.EventHandler(this.selectButton_Click);
      // 
      // detailsGroupBox
      // 
      this.detailsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.detailsGroupBox.Controls.Add(this.detailsViewHost);
      this.detailsGroupBox.Location = new System.Drawing.Point(0, 0);
      this.detailsGroupBox.Name = "detailsGroupBox";
      this.detailsGroupBox.Size = new System.Drawing.Size(665, 161);
      this.detailsGroupBox.TabIndex = 0;
      this.detailsGroupBox.TabStop = false;
      this.detailsGroupBox.Text = "Details";
      // 
      // detailsViewHost
      // 
      this.detailsViewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.detailsViewHost.Content = null;
      this.detailsViewHost.Location = new System.Drawing.Point(3, 16);
      this.detailsViewHost.Name = "detailsViewHost";
      this.detailsViewHost.ReadOnly = false;
      this.detailsViewHost.Size = new System.Drawing.Size(659, 142);
      this.detailsViewHost.TabIndex = 0;
      this.detailsViewHost.ViewType = null;
      // 
      // OperatorGraphView
      // 
      this.DragOver += new System.Windows.Forms.DragEventHandler(this.OperatorGraphView_DragEnterOver);
      this.DragDrop += new System.Windows.Forms.DragEventHandler(this.OperatorGraphView_DragDrop);
      this.DragEnter += new System.Windows.Forms.DragEventHandler(this.OperatorGraphView_DragEnterOver);
      this.AllowDrop = true;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.splitContainer);
      this.Name = "OperatorGraphView";
      this.Size = new System.Drawing.Size(665, 444);
      this.shapeContextMenu.ResumeLayout(false);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      this.splitContainer.ResumeLayout(false);
      this.detailsGroupBox.ResumeLayout(false);
      this.ResumeLayout(false);

    }
    #endregion

    private GraphVisualizationInfoView graphVisualizationInfoView;
    private System.Windows.Forms.ContextMenuStrip shapeContextMenu;
    private System.Windows.Forms.ToolStripMenuItem openViewToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem initialToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem breakPointToolStripMenuItem;
    private System.Windows.Forms.SplitContainer splitContainer;
    private System.Windows.Forms.GroupBox detailsGroupBox;
    private HeuristicLab.MainForm.WindowsForms.ViewHost detailsViewHost;
    private System.Windows.Forms.Button selectButton;
    private System.Windows.Forms.Button zoomOutButton;
    private System.Windows.Forms.Button zoomInButton;
    private System.Windows.Forms.Button zoomToFitButton;
    private System.Windows.Forms.Button relayoutButton;
    private System.Windows.Forms.Button connectButton;
    private System.Windows.Forms.Button screenshotButton;
    private System.Windows.Forms.ToolTip buttonToolTip;
  }
}
