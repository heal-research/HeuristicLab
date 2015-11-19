namespace HeuristicLab.ParallelEngine.Views {
  partial class ParallelEngineView {
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
      this.degreeOfParallelizationNumericUpDown = new System.Windows.Forms.NumericUpDown();
      this.parallelismLabel = new System.Windows.Forms.Label();
      this.infoLabel = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.degreeOfParallelizationNumericUpDown)).BeginInit();
      this.SuspendLayout();
      // 
      // executionTimeTextBox
      // 
      this.executionTimeTextBox.Location = new System.Drawing.Point(132, 0);
      this.executionTimeTextBox.Size = new System.Drawing.Size(391, 20);
      // 
      // logView
      // 
      this.logView.Location = new System.Drawing.Point(0, 53);
      this.logView.Size = new System.Drawing.Size(523, 353);
      // 
      // degreeOfParallelizationNumericUpDown
      // 
      this.degreeOfParallelizationNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.degreeOfParallelizationNumericUpDown.Enabled = false;
      this.degreeOfParallelizationNumericUpDown.Location = new System.Drawing.Point(132, 27);
      this.degreeOfParallelizationNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
      this.degreeOfParallelizationNumericUpDown.Name = "degreeOfParallelizationNumericUpDown";
      this.degreeOfParallelizationNumericUpDown.Size = new System.Drawing.Size(363, 20);
      this.degreeOfParallelizationNumericUpDown.TabIndex = 3;
      this.degreeOfParallelizationNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
      this.degreeOfParallelizationNumericUpDown.ValueChanged += new System.EventHandler(this.degreeOfParallelizationNumericUpDown_ValueChanged);
      // 
      // parallelismLabel
      // 
      this.parallelismLabel.AutoSize = true;
      this.parallelismLabel.Location = new System.Drawing.Point(3, 29);
      this.parallelismLabel.Name = "parallelismLabel";
      this.parallelismLabel.Size = new System.Drawing.Size(109, 13);
      this.parallelismLabel.TabIndex = 4;
      this.parallelismLabel.Text = "Degree of Parallelism:";
      // 
      // infoLabel
      // 
      this.infoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.infoLabel.Image = HeuristicLab.Common.Resources.VSImageLibrary.Information;
      this.infoLabel.Location = new System.Drawing.Point(501, 29);
      this.infoLabel.Name = "infoLabel";
      this.infoLabel.Size = new System.Drawing.Size(16, 16);
      this.infoLabel.TabIndex = 5;
      this.infoLabel.DoubleClick += new System.EventHandler(this.infoLabel_DoubleClick);
      // 
      // ParallelEngineView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.infoLabel);
      this.Controls.Add(this.parallelismLabel);
      this.Controls.Add(this.degreeOfParallelizationNumericUpDown);
      this.Name = "ParallelEngineView";
      this.Size = new System.Drawing.Size(523, 406);
      this.Controls.SetChildIndex(this.logView, 0);
      this.Controls.SetChildIndex(this.degreeOfParallelizationNumericUpDown, 0);
      this.Controls.SetChildIndex(this.parallelismLabel, 0);
      this.Controls.SetChildIndex(this.infoLabel, 0);
      this.Controls.SetChildIndex(this.executionTimeLabel, 0);
      this.Controls.SetChildIndex(this.executionTimeTextBox, 0);
      ((System.ComponentModel.ISupportInitialize)(this.degreeOfParallelizationNumericUpDown)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.NumericUpDown degreeOfParallelizationNumericUpDown;
    private System.Windows.Forms.Label parallelismLabel;
    protected System.Windows.Forms.Label infoLabel;
  }
}
