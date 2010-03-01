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
      this.graphVisualizationInfoView = new HeuristicLab.Operators.Views.GraphVisualization.GraphVisualizationInfoView();
      this.shapeContextMenu = new System.Windows.Forms.ContextMenuStrip();
      this.openViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.initialToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.breakPointToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.SuspendLayout();
      this.shapeContextMenu.SuspendLayout();
      // 
      // graphVisualizationInfoView
      // 
      this.graphVisualizationInfoView.AllowDrop = false;
      this.graphVisualizationInfoView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.graphVisualizationInfoView.Caption = null;
      this.graphVisualizationInfoView.Content = null;
      this.graphVisualizationInfoView.Location = new System.Drawing.Point(3, 3);
      this.graphVisualizationInfoView.Name = "graphVisualizationInfoView";
      this.graphVisualizationInfoView.Size = new System.Drawing.Size(659, 336);
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
      this.shapeContextMenu.Opening += new System.ComponentModel.CancelEventHandler(shapeContextMenu_Opening);
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
      // BreakPointToolStripMenuItem
      // 
      this.breakPointToolStripMenuItem.Name = "breakPointToolStripMenuItem";
      this.breakPointToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
      this.breakPointToolStripMenuItem.Text = "Break Point";
      this.breakPointToolStripMenuItem.Click += new System.EventHandler(this.breakPointToolStripMenuItem_Click);
      // 
      // OperatorGraphView
      // 
      this.AllowDrop = true;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.graphVisualizationInfoView);
      this.Name = "OperatorGraphView";
      this.Size = new System.Drawing.Size(665, 444);
      this.DragDrop += new System.Windows.Forms.DragEventHandler(this.OperatorGraphView_DragDrop);
      this.DragEnter += new System.Windows.Forms.DragEventHandler(this.OperatorGraphView_DragEnter);
      this.shapeContextMenu.ResumeLayout(false);
      this.ResumeLayout(false);
    }
    #endregion

    private GraphVisualizationInfoView graphVisualizationInfoView;
    private System.Windows.Forms.ContextMenuStrip shapeContextMenu;
    private System.Windows.Forms.ToolStripMenuItem openViewToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem initialToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem breakPointToolStripMenuItem;
  }
}
