namespace HeuristicLab.Clients.Hive.Views {
  partial class StateLogGanttChartView {
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
      this.ganttChart = new HeuristicLab.Clients.Hive.Views.GanttChart();
      this.SuspendLayout();
      // 
      // ganttChart
      // 
      this.ganttChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.ganttChart.Location = new System.Drawing.Point(0, 0);
      this.ganttChart.Name = "ganttChart";
      this.ganttChart.Size = new System.Drawing.Size(533, 302);
      this.ganttChart.TabIndex = 0;
      // 
      // StateLogGanttChartView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.ganttChart);
      this.Name = "StateLogGanttChartView";
      this.Size = new System.Drawing.Size(533, 302);
      this.ResumeLayout(false);

    }

    #endregion

    private GanttChart ganttChart;

  }
}
