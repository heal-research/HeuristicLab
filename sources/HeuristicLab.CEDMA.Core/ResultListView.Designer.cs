namespace HeuristicLab.CEDMA.Core {
  partial class ResultListView {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if(disposing && (components != null)) {
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
      this.dataChart = new HeuristicLab.Charting.Data.DataChartControl();
      this.xAxisLabel = new System.Windows.Forms.Label();
      this.xAxisComboBox = new System.Windows.Forms.ComboBox();
      this.yAxisLabel = new System.Windows.Forms.Label();
      this.yAxisComboBox = new System.Windows.Forms.ComboBox();
      this.yTrackBar = new System.Windows.Forms.TrackBar();
      this.xTrackBar = new System.Windows.Forms.TrackBar();
      this.xJitterlabel = new System.Windows.Forms.Label();
      this.yJitterLabel = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.yTrackBar)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.xTrackBar)).BeginInit();
      this.SuspendLayout();
      // 
      // dataChart
      // 
      this.dataChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.dataChart.BackColor = System.Drawing.SystemColors.Control;
      this.dataChart.Chart = null;
      this.dataChart.Location = new System.Drawing.Point(3, 30);
      this.dataChart.Name = "dataChart";
      this.dataChart.ScaleOnResize = true;
      this.dataChart.Size = new System.Drawing.Size(447, 390);
      this.dataChart.TabIndex = 9;
      // 
      // xAxisLabel
      // 
      this.xAxisLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.xAxisLabel.AutoSize = true;
      this.xAxisLabel.Location = new System.Drawing.Point(197, 429);
      this.xAxisLabel.Name = "xAxisLabel";
      this.xAxisLabel.Size = new System.Drawing.Size(15, 13);
      this.xAxisLabel.TabIndex = 8;
      this.xAxisLabel.Text = "x:";
      // 
      // xAxisComboBox
      // 
      this.xAxisComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.xAxisComboBox.FormattingEnabled = true;
      this.xAxisComboBox.Location = new System.Drawing.Point(218, 426);
      this.xAxisComboBox.Name = "xAxisComboBox";
      this.xAxisComboBox.Size = new System.Drawing.Size(121, 21);
      this.xAxisComboBox.TabIndex = 7;
      this.xAxisComboBox.SelectedIndexChanged += new System.EventHandler(this.xAxisComboBox_SelectedIndexChanged);
      // 
      // yAxisLabel
      // 
      this.yAxisLabel.AutoSize = true;
      this.yAxisLabel.Location = new System.Drawing.Point(3, 6);
      this.yAxisLabel.Name = "yAxisLabel";
      this.yAxisLabel.Size = new System.Drawing.Size(15, 13);
      this.yAxisLabel.TabIndex = 6;
      this.yAxisLabel.Text = "y:";
      // 
      // yAxisComboBox
      // 
      this.yAxisComboBox.FormattingEnabled = true;
      this.yAxisComboBox.Location = new System.Drawing.Point(24, 3);
      this.yAxisComboBox.Name = "yAxisComboBox";
      this.yAxisComboBox.Size = new System.Drawing.Size(121, 21);
      this.yAxisComboBox.TabIndex = 5;
      this.yAxisComboBox.SelectedIndexChanged += new System.EventHandler(this.yAxisComboBox_SelectedIndexChanged);
      // 
      // yTrackBar
      // 
      this.yTrackBar.Location = new System.Drawing.Point(186, 3);
      this.yTrackBar.Maximum = 100;
      this.yTrackBar.Name = "yTrackBar";
      this.yTrackBar.Size = new System.Drawing.Size(60, 45);
      this.yTrackBar.TabIndex = 10;
      this.yTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
      this.yTrackBar.ValueChanged += new System.EventHandler(this.yTrackBar_ValueChanged);
      // 
      // xTrackBar
      // 
      this.xTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.xTrackBar.Location = new System.Drawing.Point(387, 426);
      this.xTrackBar.Maximum = 100;
      this.xTrackBar.Name = "xTrackBar";
      this.xTrackBar.Size = new System.Drawing.Size(60, 45);
      this.xTrackBar.TabIndex = 11;
      this.xTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
      this.xTrackBar.ValueChanged += new System.EventHandler(this.xTrackBar_ValueChanged);
      // 
      // xJitterlabel
      // 
      this.xJitterlabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.xJitterlabel.AutoSize = true;
      this.xJitterlabel.Location = new System.Drawing.Point(352, 429);
      this.xJitterlabel.Name = "xJitterlabel";
      this.xJitterlabel.Size = new System.Drawing.Size(29, 13);
      this.xJitterlabel.TabIndex = 12;
      this.xJitterlabel.Text = "jitter:";
      // 
      // yJitterLabel
      // 
      this.yJitterLabel.AutoSize = true;
      this.yJitterLabel.Location = new System.Drawing.Point(151, 6);
      this.yJitterLabel.Name = "yJitterLabel";
      this.yJitterLabel.Size = new System.Drawing.Size(29, 13);
      this.yJitterLabel.TabIndex = 13;
      this.yJitterLabel.Text = "jitter:";
      // 
      // ResultListView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.yJitterLabel);
      this.Controls.Add(this.xJitterlabel);
      this.Controls.Add(this.xTrackBar);
      this.Controls.Add(this.dataChart);
      this.Controls.Add(this.xAxisLabel);
      this.Controls.Add(this.xAxisComboBox);
      this.Controls.Add(this.yAxisLabel);
      this.Controls.Add(this.yAxisComboBox);
      this.Controls.Add(this.yTrackBar);
      this.Name = "ResultListView";
      this.Size = new System.Drawing.Size(450, 450);
      ((System.ComponentModel.ISupportInitialize)(this.yTrackBar)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.xTrackBar)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private HeuristicLab.Charting.Data.DataChartControl dataChart;
    private System.Windows.Forms.Label xAxisLabel;
    private System.Windows.Forms.ComboBox xAxisComboBox;
    private System.Windows.Forms.Label yAxisLabel;
    private System.Windows.Forms.ComboBox yAxisComboBox;
    private System.Windows.Forms.TrackBar yTrackBar;
    private System.Windows.Forms.TrackBar xTrackBar;
    private System.Windows.Forms.Label xJitterlabel;
    private System.Windows.Forms.Label yJitterLabel;
  }
}
