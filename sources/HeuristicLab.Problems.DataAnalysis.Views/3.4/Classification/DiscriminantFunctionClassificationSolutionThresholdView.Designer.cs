using System.Windows.Forms.DataVisualization.Charting;
namespace HeuristicLab.Problems.DataAnalysis.Views {
  partial class DiscriminantFunctionClassificationSolutionThresholdView {
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
      System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
      System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
      this.chart = new HeuristicLab.Visualization.ChartControlsExtensions.EnhancedChart();
      this.JitterTrackBar = new System.Windows.Forms.TrackBar();
      this.label1 = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.chart)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.JitterTrackBar)).BeginInit();
      this.SuspendLayout();
      // 
      // chart
      // 
      this.chart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      chartArea1.Name = "ChartArea1";
      this.chart.ChartAreas.Add(chartArea1);
      legend1.Alignment = System.Drawing.StringAlignment.Center;
      legend1.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
      legend1.Name = "Legend1";
      this.chart.Legends.Add(legend1);
      this.chart.Location = new System.Drawing.Point(3, 3);
      this.chart.Name = "chart";
      this.chart.Size = new System.Drawing.Size(553, 364);
      this.chart.TabIndex = 0;
      this.chart.Text = "chart";
      this.chart.CustomizeLegend += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.CustomizeLegendEventArgs>(this.chart_CustomizeLegend);
      this.chart.AnnotationPositionChanging += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.AnnotationPositionChangingEventArgs>(this.chart_AnnotationPositionChanging);
      this.chart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.chart_MouseDown);
      this.chart.MouseMove += new System.Windows.Forms.MouseEventHandler(this.chart_MouseMove);
      this.chart.PostPaint += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.ChartPaintEventArgs>(chart_PostPaint);
      // 
      // JitterTrackBar
      // 
      this.JitterTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.JitterTrackBar.BackColor = System.Drawing.SystemColors.Window;
      this.JitterTrackBar.Location = new System.Drawing.Point(483, 341);
      this.JitterTrackBar.Maximum = 100;
      this.JitterTrackBar.Name = "JitterTrackBar";
      this.JitterTrackBar.Size = new System.Drawing.Size(71, 45);
      this.JitterTrackBar.TabIndex = 1;
      this.JitterTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
      this.JitterTrackBar.ValueChanged += new System.EventHandler(this.JitterTrackBar_ValueChanged);
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.label1.AutoSize = true;
      this.label1.BackColor = System.Drawing.SystemColors.Window;
      this.label1.Location = new System.Drawing.Point(448, 345);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(32, 13);
      this.label1.TabIndex = 2;
      this.label1.Text = "Jitter:";
      // 
      // SymbolicClassificationSolutionView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.label1);
      this.Controls.Add(this.JitterTrackBar);
      this.Controls.Add(this.chart);
      this.Name = "SymbolicClassificationSolutionView";
      this.Size = new System.Drawing.Size(559, 370);
      ((System.ComponentModel.ISupportInitialize)(this.chart)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.JitterTrackBar)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }
    #endregion

    private HeuristicLab.Visualization.ChartControlsExtensions.EnhancedChart chart;
    private System.Windows.Forms.TrackBar JitterTrackBar;
    private System.Windows.Forms.Label label1;
  }
}
