namespace HeuristicLab.Operators.Views.GraphVisualization {
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
      this.graphVisualizationInfoView = new HeuristicLab.Operators.Views.GraphVisualization.GraphVisualizationInfoView();
      this.shapeContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.openViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.initialToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.breakPointToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.detailsGroupBox = new System.Windows.Forms.GroupBox();
      this.detailsViewHost = new HeuristicLab.Core.Views.ViewHost();
      this.selectButton = new System.Windows.Forms.Button();
      this.panButton = new System.Windows.Forms.Button();
      this.connectButton = new System.Windows.Forms.Button();
      this.relayoutButton = new System.Windows.Forms.Button();
      this.zoomAreaButton = new System.Windows.Forms.Button();
      this.zoomInButton = new System.Windows.Forms.Button();
      this.zoomOutButton = new System.Windows.Forms.Button();
      this.buttonToolTip = new System.Windows.Forms.ToolTip();
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
      this.graphVisualizationInfoView.Caption = null;
      this.graphVisualizationInfoView.Content = null;
      this.graphVisualizationInfoView.Location = new System.Drawing.Point(3, 30);
      this.graphVisualizationInfoView.Name = "graphVisualizationInfoView";
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
      this.breakPointToolStripMenuItem.Text = "Break Point";
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
      this.splitContainer.Panel1.Controls.Add(this.zoomOutButton);
      this.splitContainer.Panel1.Controls.Add(this.zoomInButton);
      this.splitContainer.Panel1.Controls.Add(this.zoomAreaButton);
      this.splitContainer.Panel1.Controls.Add(this.relayoutButton);
      this.splitContainer.Panel1.Controls.Add(this.connectButton);
      this.splitContainer.Panel1.Controls.Add(this.panButton);
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
      // detailsGroupBox
      // 
      this.detailsGroupBox.Controls.Add(this.detailsViewHost);
      this.detailsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.detailsGroupBox.Location = new System.Drawing.Point(0, 0);
      this.detailsGroupBox.Name = "detailsGroupBox";
      this.detailsGroupBox.Size = new System.Drawing.Size(665, 161);
      this.detailsGroupBox.TabIndex = 0;
      this.detailsGroupBox.TabStop = false;
      this.detailsGroupBox.Text = "Details";
      // 
      // detailsViewHost
      // 
      this.detailsViewHost.Content = null;
      this.detailsViewHost.Dock = System.Windows.Forms.DockStyle.Fill;
      this.detailsViewHost.Location = new System.Drawing.Point(3, 16);
      this.detailsViewHost.Name = "detailsViewHost";
      this.detailsViewHost.Size = new System.Drawing.Size(659, 142);
      this.detailsViewHost.TabIndex = 0;
      this.detailsViewHost.ViewType = null;
      // 
      // selectButton
      // 
      this.selectButton.Image = HeuristicLab.Common.Resources.VS2008ImageLibrary.Pointer;
      this.selectButton.Location = new System.Drawing.Point(3, 3);
      this.selectButton.Name = "selectButton";
      this.selectButton.Size = new System.Drawing.Size(24, 24);
      this.selectButton.TabIndex = 1;
      this.selectButton.UseVisualStyleBackColor = true;
      this.selectButton.Click += new System.EventHandler(selectButton_Click);
      this.buttonToolTip.SetToolTip(this.selectButton, "Select Tool");
      // 
      // panButton
      // 
      this.panButton.Image = HeuristicLab.Common.Resources.VS2008ImageLibrary.Breakpoint;
      this.panButton.Location = new System.Drawing.Point(33, 3);
      this.panButton.Name = "panButton";
      this.panButton.Size = new System.Drawing.Size(24, 24);
      this.panButton.TabIndex = 2;
      this.panButton.UseVisualStyleBackColor = true;
      this.panButton.Click += new System.EventHandler(panButton_Click);
      this.buttonToolTip.SetToolTip(this.panButton, "Pan Tool");
      // 
      // connectButton
      // 
      this.connectButton.Image = HeuristicLab.Common.Resources.VS2008ImageLibrary.Interface;
      this.connectButton.Location = new System.Drawing.Point(63, 3);
      this.connectButton.Name = "connectButton";
      this.connectButton.Size = new System.Drawing.Size(24, 24);
      this.connectButton.TabIndex = 3;
      this.connectButton.UseVisualStyleBackColor = true;
      this.connectButton.Click += new System.EventHandler(connectButton_Click);
      this.buttonToolTip.SetToolTip(this.connectButton, "Connection Tool");
      // 
      // relayoutButton
      // 
      this.relayoutButton.Image = HeuristicLab.Common.Resources.VS2008ImageLibrary.RefreshDocument;
      this.relayoutButton.Location = new System.Drawing.Point(123, 3);
      this.relayoutButton.Name = "relayoutButton";
      this.relayoutButton.Size = new System.Drawing.Size(24, 24);
      this.relayoutButton.TabIndex = 4;
      this.relayoutButton.UseVisualStyleBackColor = true;
      this.relayoutButton.Click += new System.EventHandler(relayoutButton_Click);
      this.buttonToolTip.SetToolTip(this.relayoutButton, "Relayout Graph");
      // 
      // zoomAreaButton
      // 
      this.zoomAreaButton.Image = HeuristicLab.Common.Resources.VS2008ImageLibrary.Zoom;
      this.zoomAreaButton.Location = new System.Drawing.Point(153, 3);
      this.zoomAreaButton.Name = "zoomAreaButton";
      this.zoomAreaButton.Size = new System.Drawing.Size(24, 24);
      this.zoomAreaButton.TabIndex = 5;
      this.zoomAreaButton.UseVisualStyleBackColor = true;
      this.zoomAreaButton.Click += new System.EventHandler(zoomAreaButton_Click);
      this.buttonToolTip.SetToolTip(this.zoomAreaButton, "Zoom Area Tool");
      // 
      // zoomInButton
      // 
      this.zoomInButton.Image = HeuristicLab.Common.Resources.VS2008ImageLibrary.ZoomIn;
      this.zoomInButton.Location = new System.Drawing.Point(183, 3);
      this.zoomInButton.Name = "zoomInButton";
      this.zoomInButton.Size = new System.Drawing.Size(24, 24);
      this.zoomInButton.TabIndex = 6;
      this.zoomInButton.UseVisualStyleBackColor = true;
      this.zoomInButton.Click += new System.EventHandler(zoomInButton_Click);
      this.buttonToolTip.SetToolTip(this.zoomInButton, "Zoom In");
      // 
      // zoomOutButton
      // 
      this.zoomOutButton.Image = HeuristicLab.Common.Resources.VS2008ImageLibrary.ZoomOut;
      this.zoomOutButton.Location = new System.Drawing.Point(213, 3);
      this.zoomOutButton.Name = "zoomOutButton";
      this.zoomOutButton.Size = new System.Drawing.Size(24, 24);
      this.zoomOutButton.TabIndex = 7;
      this.zoomOutButton.UseVisualStyleBackColor = true;
      this.zoomOutButton.Click += new System.EventHandler(zoomOutButton_Click);
      this.buttonToolTip.SetToolTip(this.zoomOutButton, "Zoom Out");
      // 
      // OperatorGraphView
      // 
      this.AllowDrop = true;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.splitContainer);
      this.Name = "OperatorGraphView";
      this.Size = new System.Drawing.Size(665, 444);
      this.DragDrop += new System.Windows.Forms.DragEventHandler(this.OperatorGraphView_DragDrop);
      this.DragEnter += new System.Windows.Forms.DragEventHandler(this.OperatorGraphView_DragEnter);
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
    private HeuristicLab.Core.Views.ViewHost detailsViewHost;
    private System.Windows.Forms.Button selectButton;
    private System.Windows.Forms.Button zoomOutButton;
    private System.Windows.Forms.Button zoomInButton;
    private System.Windows.Forms.Button zoomAreaButton;
    private System.Windows.Forms.Button relayoutButton;
    private System.Windows.Forms.Button connectButton;
    private System.Windows.Forms.Button panButton;
    private System.Windows.Forms.ToolTip buttonToolTip;
  }
}
