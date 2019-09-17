namespace HeuristicLab.Problems.TravelingSalesman.Views {
  partial class EuclideanTSPDataView {
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
      this.roundingModeLabel = new System.Windows.Forms.Label();
      this.roundingModeComboBox = new System.Windows.Forms.ComboBox();
      ((System.ComponentModel.ISupportInitialize)(this.coordinatesSplitContainer)).BeginInit();
      this.coordinatesSplitContainer.Panel1.SuspendLayout();
      this.coordinatesSplitContainer.Panel2.SuspendLayout();
      this.coordinatesSplitContainer.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.coordinatesPictureBox)).BeginInit();
      this.SuspendLayout();
      // 
      // coordinatesSplitContainer
      // 
      // 
      // coordinatesSplitContainer.Panel1
      // 
      this.coordinatesSplitContainer.Panel1.Controls.Add(this.roundingModeComboBox);
      this.coordinatesSplitContainer.Panel1.Controls.Add(this.roundingModeLabel);
      // 
      // coordinatesMatrixView
      // 
      this.coordinatesMatrixView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.coordinatesMatrixView.Dock = System.Windows.Forms.DockStyle.None;
      this.coordinatesMatrixView.Location = new System.Drawing.Point(0, 30);
      this.coordinatesMatrixView.Size = new System.Drawing.Size(256, 598);
      // 
      // roundingModeLabel
      // 
      this.roundingModeLabel.AutoSize = true;
      this.roundingModeLabel.Location = new System.Drawing.Point(3, 6);
      this.roundingModeLabel.Name = "roundingModeLabel";
      this.roundingModeLabel.Size = new System.Drawing.Size(101, 13);
      this.roundingModeLabel.TabIndex = 2;
      this.roundingModeLabel.Text = "Distance Rounding:";
      // 
      // roundingModeComboBox
      // 
      this.roundingModeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.roundingModeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.roundingModeComboBox.FormattingEnabled = true;
      this.roundingModeComboBox.Location = new System.Drawing.Point(110, 3);
      this.roundingModeComboBox.Name = "roundingModeComboBox";
      this.roundingModeComboBox.Size = new System.Drawing.Size(146, 21);
      this.roundingModeComboBox.TabIndex = 3;
      // 
      // EuclideanTSPDataView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Name = "EuclideanTSPDataView";
      this.coordinatesSplitContainer.Panel1.ResumeLayout(false);
      this.coordinatesSplitContainer.Panel1.PerformLayout();
      this.coordinatesSplitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.coordinatesSplitContainer)).EndInit();
      this.coordinatesSplitContainer.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.coordinatesPictureBox)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    protected System.Windows.Forms.ComboBox roundingModeComboBox;
    protected System.Windows.Forms.Label roundingModeLabel;
  }
}
