namespace HeuristicLab.CEDMA.Charting {
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
      this.invertCheckbox = new System.Windows.Forms.CheckBox();
      this.chartPanel = new System.Windows.Forms.Panel();
      ((System.ComponentModel.ISupportInitialize)(this.xTrackBar)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.yTrackBar)).BeginInit();
      this.SuspendLayout();
      // 
      // yJitterLabel
      // 
      this.yJitterLabel.AutoSize = true;
      this.yJitterLabel.Location = new System.Drawing.Point(3, 30);
      this.yJitterLabel.Name = "yJitterLabel";
      this.yJitterLabel.Size = new System.Drawing.Size(29, 13);
      this.yJitterLabel.TabIndex = 13;
      this.yJitterLabel.Text = "jitter:";
      // 
      // xJitterlabel
      // 
      this.xJitterlabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.xJitterlabel.AutoSize = true;
      this.xJitterlabel.Location = new System.Drawing.Point(291, 418);
      this.xJitterlabel.Name = "xJitterlabel";
      this.xJitterlabel.Size = new System.Drawing.Size(29, 13);
      this.xJitterlabel.TabIndex = 12;
      this.xJitterlabel.Text = "jitter:";
      // 
      // xTrackBar
      // 
      this.xTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.xTrackBar.Location = new System.Drawing.Point(326, 418);
      this.xTrackBar.Maximum = 100;
      this.xTrackBar.Name = "xTrackBar";
      this.xTrackBar.Size = new System.Drawing.Size(121, 45);
      this.xTrackBar.TabIndex = 11;
      this.xTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
      this.xTrackBar.ValueChanged += new System.EventHandler(this.jitterTrackBar_ValueChanged);
      // 
      // xAxisLabel
      // 
      this.xAxisLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.xAxisLabel.AutoSize = true;
      this.xAxisLabel.Location = new System.Drawing.Point(305, 394);
      this.xAxisLabel.Name = "xAxisLabel";
      this.xAxisLabel.Size = new System.Drawing.Size(15, 13);
      this.xAxisLabel.TabIndex = 8;
      this.xAxisLabel.Text = "x:";
      // 
      // xAxisComboBox
      // 
      this.xAxisComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.xAxisComboBox.FormattingEnabled = true;
      this.xAxisComboBox.Location = new System.Drawing.Point(326, 391);
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
      this.yTrackBar.Location = new System.Drawing.Point(38, 30);
      this.yTrackBar.Maximum = 100;
      this.yTrackBar.Name = "yTrackBar";
      this.yTrackBar.Size = new System.Drawing.Size(107, 45);
      this.yTrackBar.TabIndex = 10;
      this.yTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
      this.yTrackBar.ValueChanged += new System.EventHandler(this.jitterTrackBar_ValueChanged);
      // 
      // sizeComboBox
      // 
      this.sizeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.sizeComboBox.FormattingEnabled = true;
      this.sizeComboBox.Location = new System.Drawing.Point(329, 3);
      this.sizeComboBox.Name = "sizeComboBox";
      this.sizeComboBox.Size = new System.Drawing.Size(121, 21);
      this.sizeComboBox.TabIndex = 14;
      this.sizeComboBox.SelectedIndexChanged += new System.EventHandler(this.sizeComboBox_SelectedIndexChanged);
      // 
      // sizeLabel
      // 
      this.sizeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.sizeLabel.AutoSize = true;
      this.sizeLabel.Location = new System.Drawing.Point(259, 6);
      this.sizeLabel.Name = "sizeLabel";
      this.sizeLabel.Size = new System.Drawing.Size(64, 13);
      this.sizeLabel.TabIndex = 15;
      this.sizeLabel.Text = "Bubble size:";
      // 
      // invertCheckbox
      // 
      this.invertCheckbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.invertCheckbox.AutoSize = true;
      this.invertCheckbox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.invertCheckbox.Location = new System.Drawing.Point(326, 30);
      this.invertCheckbox.Name = "invertCheckbox";
      this.invertCheckbox.Size = new System.Drawing.Size(64, 17);
      this.invertCheckbox.TabIndex = 16;
      this.invertCheckbox.Text = "Inverse:";
      this.invertCheckbox.UseVisualStyleBackColor = true;
      this.invertCheckbox.CheckedChanged += new System.EventHandler(this.sizeComboBox_SelectedIndexChanged);
      // 
      // chartPanel
      // 
      this.chartPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.chartPanel.Location = new System.Drawing.Point(0, 81);
      this.chartPanel.Name = "chartPanel";
      this.chartPanel.Size = new System.Drawing.Size(447, 304);
      this.chartPanel.TabIndex = 17;
      // 
      // ResultListView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.chartPanel);
      this.Controls.Add(this.invertCheckbox);
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
      this.Name = "ResultListView";
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
    private System.Windows.Forms.CheckBox invertCheckbox;
    private System.Windows.Forms.Panel chartPanel;
  }
}
