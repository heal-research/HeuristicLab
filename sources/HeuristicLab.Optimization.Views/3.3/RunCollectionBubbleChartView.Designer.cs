namespace HeuristicLab.Optimization.Views {
  partial class RunCollectionBubbleChartView {
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
      System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
      System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
      this.yJitterLabel = new System.Windows.Forms.Label();
      this.xJitterlabel = new System.Windows.Forms.Label();
      this.xTrackBar = new System.Windows.Forms.TrackBar();
      this.xAxisLabel = new System.Windows.Forms.Label();
      this.xAxisComboBox = new System.Windows.Forms.ComboBox();
      this.yAxisLabel = new System.Windows.Forms.Label();
      this.yAxisComboBox = new System.Windows.Forms.ComboBox();
      this.yTrackBar = new System.Windows.Forms.TrackBar();
      this.sizeComboBox = new System.Windows.Forms.ComboBox();
      this.sizeLabel = new System.Windows.Forms.Label();
      this.chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
      this.zoomButton = new System.Windows.Forms.RadioButton();
      this.selectButton = new System.Windows.Forms.RadioButton();
      this.radioButtonGroup = new System.Windows.Forms.GroupBox();
      this.colorButton = new System.Windows.Forms.Button();
      this.colorDialog = new System.Windows.Forms.ColorDialog();
      ((System.ComponentModel.ISupportInitialize)(this.xTrackBar)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.yTrackBar)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.chart)).BeginInit();
      this.radioButtonGroup.SuspendLayout();
      this.SuspendLayout();
      // 
      // yJitterLabel
      // 
      this.yJitterLabel.AutoSize = true;
      this.yJitterLabel.Location = new System.Drawing.Point(257, 6);
      this.yJitterLabel.Name = "yJitterLabel";
      this.yJitterLabel.Size = new System.Drawing.Size(29, 13);
      this.yJitterLabel.TabIndex = 13;
      this.yJitterLabel.Text = "jitter:";
      // 
      // xJitterlabel
      // 
      this.xJitterlabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.xJitterlabel.AutoSize = true;
      this.xJitterlabel.Location = new System.Drawing.Point(461, 309);
      this.xJitterlabel.Name = "xJitterlabel";
      this.xJitterlabel.Size = new System.Drawing.Size(29, 13);
      this.xJitterlabel.TabIndex = 12;
      this.xJitterlabel.Text = "jitter:";
      // 
      // xTrackBar
      // 
      this.xTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.xTrackBar.Location = new System.Drawing.Point(496, 306);
      this.xTrackBar.Maximum = 100;
      this.xTrackBar.Name = "xTrackBar";
      this.xTrackBar.Size = new System.Drawing.Size(64, 45);
      this.xTrackBar.TabIndex = 11;
      this.xTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
      this.xTrackBar.ValueChanged += new System.EventHandler(this.jitterTrackBar_ValueChanged);
      // 
      // xAxisLabel
      // 
      this.xAxisLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.xAxisLabel.AutoSize = true;
      this.xAxisLabel.Location = new System.Drawing.Point(213, 309);
      this.xAxisLabel.Name = "xAxisLabel";
      this.xAxisLabel.Size = new System.Drawing.Size(15, 13);
      this.xAxisLabel.TabIndex = 8;
      this.xAxisLabel.Text = "x:";
      // 
      // xAxisComboBox
      // 
      this.xAxisComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.xAxisComboBox.BackColor = System.Drawing.SystemColors.ControlLightLight;
      this.xAxisComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.xAxisComboBox.FormattingEnabled = true;
      this.xAxisComboBox.Location = new System.Drawing.Point(234, 307);
      this.xAxisComboBox.Name = "xAxisComboBox";
      this.xAxisComboBox.Size = new System.Drawing.Size(221, 21);
      this.xAxisComboBox.TabIndex = 7;
      this.xAxisComboBox.SelectedIndexChanged += new System.EventHandler(this.AxisComboBox_SelectedIndexChanged);
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
      this.yAxisComboBox.BackColor = System.Drawing.SystemColors.ControlLightLight;
      this.yAxisComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.yAxisComboBox.FormattingEnabled = true;
      this.yAxisComboBox.Location = new System.Drawing.Point(24, 3);
      this.yAxisComboBox.Name = "yAxisComboBox";
      this.yAxisComboBox.Size = new System.Drawing.Size(221, 21);
      this.yAxisComboBox.TabIndex = 5;
      this.yAxisComboBox.SelectedIndexChanged += new System.EventHandler(this.AxisComboBox_SelectedIndexChanged);
      // 
      // yTrackBar
      // 
      this.yTrackBar.Location = new System.Drawing.Point(292, 3);
      this.yTrackBar.Maximum = 100;
      this.yTrackBar.Name = "yTrackBar";
      this.yTrackBar.Size = new System.Drawing.Size(59, 45);
      this.yTrackBar.TabIndex = 10;
      this.yTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
      this.yTrackBar.ValueChanged += new System.EventHandler(this.jitterTrackBar_ValueChanged);
      // 
      // sizeComboBox
      // 
      this.sizeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.sizeComboBox.BackColor = System.Drawing.SystemColors.ControlLightLight;
      this.sizeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.sizeComboBox.FormattingEnabled = true;
      this.sizeComboBox.Location = new System.Drawing.Point(441, 3);
      this.sizeComboBox.Name = "sizeComboBox";
      this.sizeComboBox.Size = new System.Drawing.Size(121, 21);
      this.sizeComboBox.TabIndex = 14;
      this.sizeComboBox.SelectedIndexChanged += new System.EventHandler(this.AxisComboBox_SelectedIndexChanged);
      // 
      // sizeLabel
      // 
      this.sizeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.sizeLabel.AutoSize = true;
      this.sizeLabel.Location = new System.Drawing.Point(371, 6);
      this.sizeLabel.Name = "sizeLabel";
      this.sizeLabel.Size = new System.Drawing.Size(64, 13);
      this.sizeLabel.TabIndex = 15;
      this.sizeLabel.Text = "Bubble size:";
      // 
      // chart
      // 
      this.chart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      chartArea2.Name = "ChartArea1";
      this.chart.ChartAreas.Add(chartArea2);
      this.chart.Location = new System.Drawing.Point(6, 30);
      this.chart.Name = "chart";
      series2.ChartArea = "ChartArea1";
      series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Bubble;
      series2.CustomProperties = "BubbleMaxSize=0";
      series2.IsVisibleInLegend = false;
      series2.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
      series2.Name = "Bubbles";
      series2.YValuesPerPoint = 2;
      this.chart.Series.Add(series2);
      this.chart.Size = new System.Drawing.Size(556, 263);
      this.chart.TabIndex = 16;
      this.chart.Text = "chart1";
      this.chart.MouseUp += new System.Windows.Forms.MouseEventHandler(this.chart_MouseUp);
      this.chart.MouseMove += new System.Windows.Forms.MouseEventHandler(this.chart_MouseMove);
      this.chart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.chart_MouseDown);
      this.chart.LostFocus += new System.EventHandler(this.chart_LostFocus);
      // 
      // zoomButton
      // 
      this.zoomButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.zoomButton.AutoSize = true;
      this.zoomButton.Checked = true;
      this.zoomButton.Location = new System.Drawing.Point(6, 10);
      this.zoomButton.Name = "zoomButton";
      this.zoomButton.Size = new System.Drawing.Size(52, 17);
      this.zoomButton.TabIndex = 17;
      this.zoomButton.TabStop = true;
      this.zoomButton.Text = "Zoom";
      this.zoomButton.UseVisualStyleBackColor = true;
      this.zoomButton.CheckedChanged += new System.EventHandler(this.zoomButton_CheckedChanged);
      // 
      // selectButton
      // 
      this.selectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.selectButton.AutoSize = true;
      this.selectButton.Location = new System.Drawing.Point(64, 9);
      this.selectButton.Name = "selectButton";
      this.selectButton.Size = new System.Drawing.Size(55, 17);
      this.selectButton.TabIndex = 18;
      this.selectButton.Text = "Select";
      this.selectButton.UseVisualStyleBackColor = true;
      // 
      // radioButtonGroup
      // 
      this.radioButtonGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.radioButtonGroup.Controls.Add(this.zoomButton);
      this.radioButtonGroup.Controls.Add(this.selectButton);
      this.radioButtonGroup.Location = new System.Drawing.Point(6, 299);
      this.radioButtonGroup.Name = "radioButtonGroup";
      this.radioButtonGroup.Size = new System.Drawing.Size(135, 32);
      this.radioButtonGroup.TabIndex = 19;
      this.radioButtonGroup.TabStop = false;
      // 
      // colorButton
      // 
      this.colorButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.colorButton.Enabled = false;
      this.colorButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.colorButton.Location = new System.Drawing.Point(144, 306);
      this.colorButton.Name = "colorButton";
      this.colorButton.Size = new System.Drawing.Size(64, 23);
      this.colorButton.TabIndex = 20;
      this.colorButton.Text = "Color";
      this.colorButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.colorButton.UseVisualStyleBackColor = true;
      this.colorButton.Click += new System.EventHandler(this.colorButton_Click);
      // 
      // colorDialog
      // 
      this.colorDialog.FullOpen = true;
      // 
      // RunCollectionBubbleChartView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.colorButton);
      this.Controls.Add(this.radioButtonGroup);
      this.Controls.Add(this.chart);
      this.Controls.Add(this.sizeLabel);
      this.Controls.Add(this.sizeComboBox);
      this.Controls.Add(this.yJitterLabel);
      this.Controls.Add(this.xJitterlabel);
      this.Controls.Add(this.xTrackBar);
      this.Controls.Add(this.xAxisLabel);
      this.Controls.Add(this.xAxisComboBox);
      this.Controls.Add(this.yAxisLabel);
      this.Controls.Add(this.yAxisComboBox);
      this.Controls.Add(this.yTrackBar);
      this.Name = "RunCollectionBubbleChartView";
      this.Size = new System.Drawing.Size(567, 334);
      ((System.ComponentModel.ISupportInitialize)(this.xTrackBar)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.yTrackBar)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.chart)).EndInit();
      this.radioButtonGroup.ResumeLayout(false);
      this.radioButtonGroup.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label xAxisLabel;
    private System.Windows.Forms.ComboBox xAxisComboBox;
    private System.Windows.Forms.Label yAxisLabel;
    private System.Windows.Forms.ComboBox yAxisComboBox;
    private System.Windows.Forms.TrackBar yTrackBar;
    private System.Windows.Forms.TrackBar xTrackBar;
    private System.Windows.Forms.Label xJitterlabel;
    private System.Windows.Forms.Label yJitterLabel;
    private System.Windows.Forms.ComboBox sizeComboBox;
    private System.Windows.Forms.Label sizeLabel;
    private System.Windows.Forms.DataVisualization.Charting.Chart chart;
    private System.Windows.Forms.RadioButton zoomButton;
    private System.Windows.Forms.RadioButton selectButton;
    private System.Windows.Forms.GroupBox radioButtonGroup;
    private System.Windows.Forms.Button colorButton;
    private System.Windows.Forms.ColorDialog colorDialog;
  }
}
