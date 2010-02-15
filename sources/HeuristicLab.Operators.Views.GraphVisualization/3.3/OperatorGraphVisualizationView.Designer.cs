namespace HeuristicLab.Operators.Views.GraphVisualization {
  partial class OperatorGraphVisualizationView {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OperatorGraphVisualizationView));
      this.graphVisualization = new HeuristicLab.Netron.NetronVisualization();
      ((System.ComponentModel.ISupportInitialize)(this.graphVisualization)).BeginInit();
      this.SuspendLayout();
      // 
      // netronVisualization1
      // 
      this.graphVisualization.AllowDrop = true;
      this.graphVisualization.AutoScroll = true;
      this.graphVisualization.BackColor = System.Drawing.Color.DarkGray;
      this.graphVisualization.BackgroundType = global::Netron.Diagramming.Core.CanvasBackgroundTypes.FlatColor;
      this.graphVisualization.Dock = System.Windows.Forms.DockStyle.Fill;
      this.graphVisualization.Document = ((global::Netron.Diagramming.Core.Document)(resources.GetObject("netronVisualization1.Document")));
      this.graphVisualization.EnableAddConnection = true;
      this.graphVisualization.FileName = "";
      this.graphVisualization.Location = new System.Drawing.Point(0, 0);
      this.graphVisualization.Magnification = new System.Drawing.SizeF(71F, 71F);
      this.graphVisualization.Name = "graphVisualization";
      this.graphVisualization.Origin = new System.Drawing.Point(0, 0);
      this.graphVisualization.PageSettings = ((System.Drawing.Printing.PageSettings)(resources.GetObject("netronVisualization1.PageSettings")));
      this.graphVisualization.ShowConnectors = true;
      this.graphVisualization.ShowRulers = false;
      this.graphVisualization.Size = new System.Drawing.Size(602, 432);
      this.graphVisualization.TabIndex = 0;
      this.graphVisualization.Text = "graphVisualization";
      // 
      // OperatorGraphVisualizationView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.graphVisualization);
      this.Name = "OperatorGraphVisualizationView";
      this.Size = new System.Drawing.Size(602, 432);
      ((System.ComponentModel.ISupportInitialize)(this.graphVisualization)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private HeuristicLab.Netron.NetronVisualization graphVisualization;



  }
}
