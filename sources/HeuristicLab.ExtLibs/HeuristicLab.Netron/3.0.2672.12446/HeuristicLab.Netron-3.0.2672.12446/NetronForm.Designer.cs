namespace HeuristicLab.Netron {
  partial class NetronForm {
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

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NetronForm));
      this.netronVisualization = new HeuristicLab.Netron.NetronVisualization();
      ((System.ComponentModel.ISupportInitialize)(this.netronVisualization)).BeginInit();
      this.SuspendLayout();
      // 
      // netronVisualization
      // 
      this.netronVisualization.AllowDrop = true;
      this.netronVisualization.AutoScroll = true;
      this.netronVisualization.BackColor = System.Drawing.Color.DarkGray;
      this.netronVisualization.BackgroundType = global::Netron.Diagramming.Core.CanvasBackgroundTypes.FlatColor;
      this.netronVisualization.Dock = System.Windows.Forms.DockStyle.Fill;
      this.netronVisualization.EnableAddConnection = true;
      this.netronVisualization.FileName = "";
      this.netronVisualization.Location = new System.Drawing.Point(0, 0);
      this.netronVisualization.Magnification = new System.Drawing.SizeF(71F, 71F);
      this.netronVisualization.Name = "netronVisualization";
      this.netronVisualization.Origin = new System.Drawing.Point(0, 0);
      this.netronVisualization.ShowConnectors = true;
      this.netronVisualization.ShowRulers = false;
      this.netronVisualization.Size = new System.Drawing.Size(995, 567);
      this.netronVisualization.TabIndex = 1;
      this.netronVisualization.Text = "netronVisualization";
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(995, 567);
      this.Controls.Add(this.netronVisualization);
      this.Name = "Form1";
      this.Text = "Form1";
      ((System.ComponentModel.ISupportInitialize)(this.netronVisualization)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private HeuristicLab.Netron.NetronVisualization netronVisualization;
  }
}

