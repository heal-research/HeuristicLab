namespace HeuristicLab.CEDMA.Charting {
  partial class BubbleChartView {
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
      this.bubbleChartControl = new HeuristicLab.CEDMA.Charting.BubbleChartControl();
      ((System.ComponentModel.ISupportInitialize)(this.xTrackBar)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.yTrackBar)).BeginInit();
      this.SuspendLayout();
      // 
      // yJitterLabel
      // 
      this.yJitterLabel.AutoSize = true;
      this.yJitterLabel.BackColor = System.Drawing.SystemColors.Control;
      this.yJitterLabel.Location = new System.Drawing.Point(151, 6);
      this.yJitterLabel.Name = "yJitterLabel";
      this.yJitterLabel.Size = new System.Drawing.Size(29, 13);
      this.yJitterLabel.TabIndex = 13;
      this.yJitterLabel.Text = "jitter:";
      // 
      // xJitterlabel
      // 
      this.xJitterlabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.xJitterlabel.AutoSize = true;
      this.xJitterlabel.BackColor = System.Drawing.SystemColors.Control;
      this.xJitterlabel.Location = new System.Drawing.Point(346, 439);
      this.xJitterlabel.Name = "xJitterlabel";
      this.xJitterlabel.Size = new System.Drawing.Size(29, 13);
      this.xJitterlabel.TabIndex = 12;
      this.xJitterlabel.Text = "jitter:";
      // 
      // xTrackBar
      // 
      this.xTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.xTrackBar.BackColor = System.Drawing.SystemColors.Control;
      this.xTrackBar.Location = new System.Drawing.Point(381, 435);
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
      this.xAxisLabel.BackColor = System.Drawing.SystemColors.Control;
      this.xAxisLabel.Location = new System.Drawing.Point(198, 438);
      this.xAxisLabel.Name = "xAxisLabel";
      this.xAxisLabel.Size = new System.Drawing.Size(15, 13);
      this.xAxisLabel.TabIndex = 8;
      this.xAxisLabel.Text = "x:";
      // 
      // xAxisComboBox
      // 
      this.xAxisComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.xAxisComboBox.BackColor = System.Drawing.SystemColors.ControlLightLight;
      this.xAxisComboBox.FormattingEnabled = true;
      this.xAxisComboBox.Location = new System.Drawing.Point(219, 435);
      this.xAxisComboBox.Name = "xAxisComboBox";
      this.xAxisComboBox.Size = new System.Drawing.Size(121, 21);
      this.xAxisComboBox.TabIndex = 7;
      this.xAxisComboBox.SelectedIndexChanged += new System.EventHandler(this.axisComboBox_SelectedIndexChanged);
      // 
      // yAxisLabel
      // 
      this.yAxisLabel.AutoSize = true;
      this.yAxisLabel.BackColor = System.Drawing.SystemColors.Control;
      this.yAxisLabel.Location = new System.Drawing.Point(3, 6);
      this.yAxisLabel.Name = "yAxisLabel";
      this.yAxisLabel.Size = new System.Drawing.Size(15, 13);
      this.yAxisLabel.TabIndex = 6;
      this.yAxisLabel.Text = "y:";
      // 
      // yAxisComboBox
      // 
      this.yAxisComboBox.BackColor = System.Drawing.SystemColors.ControlLightLight;
      this.yAxisComboBox.FormattingEnabled = true;
      this.yAxisComboBox.Location = new System.Drawing.Point(24, 3);
      this.yAxisComboBox.Name = "yAxisComboBox";
      this.yAxisComboBox.Size = new System.Drawing.Size(121, 21);
      this.yAxisComboBox.TabIndex = 5;
      this.yAxisComboBox.SelectedIndexChanged += new System.EventHandler(this.axisComboBox_SelectedIndexChanged);
      // 
      // yTrackBar
      // 
      this.yTrackBar.BackColor = System.Drawing.SystemColors.Control;
      this.yTrackBar.Location = new System.Drawing.Point(186, 3);
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
      this.sizeComboBox.FormattingEnabled = true;
      this.sizeComboBox.Location = new System.Drawing.Point(324, 3);
      this.sizeComboBox.Name = "sizeComboBox";
      this.sizeComboBox.Size = new System.Drawing.Size(121, 21);
      this.sizeComboBox.TabIndex = 14;
      this.sizeComboBox.SelectedIndexChanged += new System.EventHandler(this.sizeComboBox_SelectedIndexChanged);
      // 
      // sizeLabel
      // 
      this.sizeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.sizeLabel.AutoSize = true;
      this.sizeLabel.BackColor = System.Drawing.SystemColors.Control;
      this.sizeLabel.Location = new System.Drawing.Point(254, 6);
      this.sizeLabel.Name = "sizeLabel";
      this.sizeLabel.Size = new System.Drawing.Size(64, 13);
      this.sizeLabel.TabIndex = 15;
      this.sizeLabel.Text = "Bubble size:";
      // 
      // bubbleChartControl
      // 
      this.bubbleChartControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.bubbleChartControl.BackColor = System.Drawing.SystemColors.Control;
      this.bubbleChartControl.Chart = null;
      this.bubbleChartControl.Location = new System.Drawing.Point(0, 30);
      this.bubbleChartControl.Name = "bubbleChartControl";
      this.bubbleChartControl.ScaleOnResize = true;
      this.bubbleChartControl.Size = new System.Drawing.Size(450, 399);
      this.bubbleChartControl.TabIndex = 17;
      // 
      // BubbleChartView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.sizeLabel);
      this.Controls.Add(this.sizeComboBox);
      this.Controls.Add(this.yJitterLabel);
      this.Controls.Add(this.xJitterlabel);
      this.Controls.Add(this.xTrackBar);
      this.Controls.Add(this.xAxisLabel);
      this.Controls.Add(this.xAxisComboBox);
      this.Controls.Add(this.yAxisLabel);
      this.Controls.Add(this.yAxisComboBox);
      this.Controls.Add(this.bubbleChartControl);
      this.Controls.Add(this.yTrackBar);
      this.Name = "BubbleChartView";
      this.Size = new System.Drawing.Size(450, 459);
      ((System.ComponentModel.ISupportInitialize)(this.xTrackBar)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.yTrackBar)).EndInit();
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
    private BubbleChartControl bubbleChartControl;
  }
}
