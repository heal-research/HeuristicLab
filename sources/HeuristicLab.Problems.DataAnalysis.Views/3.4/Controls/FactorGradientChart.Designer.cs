namespace HeuristicLab.Problems.DataAnalysis.Views {
  partial class FactorGradientChart {
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
      System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
      System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
      System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
      System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
      System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
      this.calculationPendingLabel = new System.Windows.Forms.Label();
      this.calculationPendingTimer = new System.Windows.Forms.Timer(this.components);
      this.chart = new HeuristicLab.Visualization.ChartControlsExtensions.EnhancedChart();
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
      chartArea1.AxisX.IsMarginVisible = false;
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
      series1.BorderColor = System.Drawing.Color.Red;
      series1.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
      series1.ChartArea = "ChartArea";
      series1.IsVisibleInLegend = false;
      series1.Legend = "Default";
      series1.Name = "Series1";
      series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.String;
      series1.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
      series2.ChartArea = "ChartArea";
      series2.IsVisibleInLegend = false;
      series2.Legend = "Default";
      series2.Name = "Series2";
      series2.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.String;
      series3.ChartArea = "ChartArea";
      series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.BoxPlot;
      series3.Color = System.Drawing.Color.Black;
      series3.Legend = "Default";
      series3.Name = "Series3";
      series3.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.String;
      series3.YValuesPerPoint = 6;
      series4.ChartArea = "ChartArea";
      series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.BoxPlot;
      series4.Legend = "Default";
      series4.Name = "Series4";
      series4.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.String;
      series4.YValuesPerPoint = 6;
      this.chart.Series.Add(series1);
      this.chart.Series.Add(series2);
      this.chart.Series.Add(series3);
      this.chart.Series.Add(series4);
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
      this.chart.DragDrop += new System.Windows.Forms.DragEventHandler(this.chart_DragDrop);
      this.chart.DragEnter += new System.Windows.Forms.DragEventHandler(this.chart_DragEnter);
      this.chart.MouseClick += new System.Windows.Forms.MouseEventHandler(this.chart_MouseClick);
      this.chart.Resize += new System.EventHandler(this.chart_Resize);
      // 
      // FactorGradientChart
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.calculationPendingLabel);
      this.Controls.Add(this.chart);
      this.Name = "FactorGradientChart";
      this.Size = new System.Drawing.Size(453, 308);
      ((System.ComponentModel.ISupportInitialize)(this.chart)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private HeuristicLab.Visualization.ChartControlsExtensions.EnhancedChart chart;
    private System.Windows.Forms.Label calculationPendingLabel;
    private System.Windows.Forms.Timer calculationPendingTimer;
    private System.Windows.Forms.ToolTip toolTip;
  }
}
