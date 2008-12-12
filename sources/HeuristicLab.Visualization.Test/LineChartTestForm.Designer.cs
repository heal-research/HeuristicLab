namespace HeuristicLab.Visualization.Test {
  partial class LineChartTestForm {
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
      this.lineChartGroupBox = new System.Windows.Forms.GroupBox();
      this.btnResetView = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // lineChartGroupBox
      // 
      this.lineChartGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.lineChartGroupBox.Location = new System.Drawing.Point(12, 12);
      this.lineChartGroupBox.Name = "lineChartGroupBox";
      this.lineChartGroupBox.Size = new System.Drawing.Size(607, 318);
      this.lineChartGroupBox.TabIndex = 0;
      this.lineChartGroupBox.TabStop = false;
      this.lineChartGroupBox.Text = "Line Chart";
      // 
      // btnResetView
      // 
      this.btnResetView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnResetView.Location = new System.Drawing.Point(544, 346);
      this.btnResetView.Name = "btnResetView";
      this.btnResetView.Size = new System.Drawing.Size(75, 23);
      this.btnResetView.TabIndex = 1;
      this.btnResetView.Text = "Reset View";
      this.btnResetView.UseVisualStyleBackColor = true;
      this.btnResetView.Click += new System.EventHandler(this.btnResetView_Click);
      // 
      // LineChartTestForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(631, 395);
      this.Controls.Add(this.btnResetView);
      this.Controls.Add(this.lineChartGroupBox);
      this.Name = "LineChartTestForm";
      this.Text = "LineChartTestForm";
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox lineChartGroupBox;
    private System.Windows.Forms.Button btnResetView;

  }
}