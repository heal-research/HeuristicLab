namespace HeuristicLab.Problems.DataAnalysis.Views {
  partial class GradientChart {
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
      System.Windows.Forms.DataVisualization.Charting.VerticalLineAnnotation verticalLineAnnotation1 = new System.Windows.Forms.DataVisualization.Charting.VerticalLineAnnotation();
      System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
      System.Windows.Forms.DataVisualization.Charting.StripLine stripLine1 = new System.Windows.Forms.DataVisualization.Charting.StripLine();
      System.Windows.Forms.DataVisualization.Charting.StripLine stripLine2 = new System.Windows.Forms.DataVisualization.Charting.StripLine();
      System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
      System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
      this.calculationPendingLabel = new System.Windows.Forms.Label();
      this.calculationPendingTimer = new System.Windows.Forms.Timer(this.components);
      this.chart = new HeuristicLab.Visualization.ChartControlsExtensions.EnhancedChart();
      this.configurationButton = new System.Windows.Forms.Button();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.chart)).BeginInit();
      this.SuspendLayout();
      // 
      // calculationPendingLabel
      // 
      this.calculationPendingLabel.BackColor = System.Drawing.Color.White;
      this.calculationPendingLabel.Image = HeuristicLab.Common.Resources.VSImageLibrary.Timer;
      this.calculationPendingLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.calculationPendingLabel.Location = new System.Drawing.Point(3, 3);
      this.calculationPendingLabel.Margin = new System.Windows.Forms.Padding(0);
      this.calculationPendingLabel.Name = "calculationPendingLabel";
      this.calculationPendingLabel.Size = new System.Drawing.Size(17, 17);
      this.calculationPendingLabel.TabIndex = 1;
      this.calculationPendingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.calculationPendingLabel.Visible = false;
      // 
      // calculationPendingTimer
      // 
      this.calculationPendingTimer.Tick += new System.EventHandler(this.calculationPendingTimer_Tick);
      // 
      // chart
      // 
      this.chart.AllowDrop = true;
      verticalLineAnnotation1.AllowMoving = true;
      verticalLineAnnotation1.AxisXName = "ChartArea\\rX";
      verticalLineAnnotation1.ClipToChartArea = "ChartArea";
      verticalLineAnnotation1.IsInfinitive = true;
      verticalLineAnnotation1.LineColor = System.Drawing.Color.Red;
      verticalLineAnnotation1.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
      verticalLineAnnotation1.Name = "VerticalLineAnnotation";
      verticalLineAnnotation1.YAxisName = "ChartArea\\rY";
      this.chart.Annotations.Add(verticalLineAnnotation1);
      stripLine1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(223)))), ((int)(((byte)(58)))), ((int)(((byte)(2)))));
      stripLine2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(223)))), ((int)(((byte)(58)))), ((int)(((byte)(2)))));
      chartArea1.AxisX.StripLines.Add(stripLine1);
      chartArea1.AxisX.StripLines.Add(stripLine2);
      chartArea1.Name = "ChartArea";
      chartArea1.Position.Auto = false;
      chartArea1.Position.Height = 90F;
      chartArea1.Position.Width = 100F;
      chartArea1.Position.Y = 10F;
      this.chart.ChartAreas.Add(chartArea1);
      this.chart.Dock = System.Windows.Forms.DockStyle.Fill;
      legend1.Alignment = System.Drawing.StringAlignment.Center;
      legend1.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
      legend1.LegendItemOrder = System.Windows.Forms.DataVisualization.Charting.LegendItemOrder.ReversedSeriesOrder;
      legend1.LegendStyle = System.Windows.Forms.DataVisualization.Charting.LegendStyle.Row;
      legend1.Name = "Default";
      this.chart.Legends.Add(legend1);
      this.chart.Location = new System.Drawing.Point(0, 0);
      this.chart.Name = "chart";
      this.chart.Size = new System.Drawing.Size(453, 308);
      this.chart.TabIndex = 0;
      title1.Alignment = System.Drawing.ContentAlignment.TopCenter;
      title1.DockedToChartArea = "ChartArea";
      title1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      title1.IsDockedInsideChartArea = false;
      title1.Name = "Title";
      title1.Text = "[Title]";
      this.chart.Titles.Add(title1);
      this.chart.SelectionRangeChanged += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.CursorEventArgs>(this.chart_SelectionRangeChanged);
      this.chart.PostPaint += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.ChartPaintEventArgs>(this.chart_PostPaint);
      this.chart.AnnotationPositionChanged += new System.EventHandler(this.chart_AnnotationPositionChanged);
      this.chart.AnnotationPositionChanging += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.AnnotationPositionChangingEventArgs>(this.chart_AnnotationPositionChanging);
      this.chart.DragDrop += new System.Windows.Forms.DragEventHandler(this.chart_DragDrop);
      this.chart.DragEnter += new System.Windows.Forms.DragEventHandler(this.chart_DragEnter);
      this.chart.MouseMove += new System.Windows.Forms.MouseEventHandler(this.chart_MouseMove);
      this.chart.Resize += new System.EventHandler(this.chart_Resize);
      // 
      // configurationButton
      // 
      this.configurationButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.configurationButton.AutoSize = true;
      this.configurationButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Properties;
      this.configurationButton.Location = new System.Drawing.Point(426, 3);
      this.configurationButton.Name = "configurationButton";
      this.configurationButton.Size = new System.Drawing.Size(24, 24);
      this.configurationButton.TabIndex = 2;
      this.configurationButton.TabStop = false;
      this.toolTip.SetToolTip(this.configurationButton, "Configuration");
      this.configurationButton.UseVisualStyleBackColor = true;
      this.configurationButton.Click += new System.EventHandler(this.config_Click);
      // 
      // GradientChart
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.configurationButton);
      this.Controls.Add(this.calculationPendingLabel);
      this.Controls.Add(this.chart);
      this.Name = "GradientChart";
      this.Size = new System.Drawing.Size(453, 308);
      ((System.ComponentModel.ISupportInitialize)(this.chart)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private HeuristicLab.Visualization.ChartControlsExtensions.EnhancedChart chart;
    private System.Windows.Forms.Label calculationPendingLabel;
    private System.Windows.Forms.Timer calculationPendingTimer;
    private System.Windows.Forms.Button configurationButton;
    private System.Windows.Forms.ToolTip toolTip;
  }
}
