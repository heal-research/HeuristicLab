namespace HeuristicLab.Problems.TravelingSalesman.Views {
  partial class CoordinatesTSPDataView {
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
      this.coordinatesSplitContainer = new System.Windows.Forms.SplitContainer();
      this.coordinatesMatrixView = new HeuristicLab.Data.Views.StringConvertibleMatrixView();
      this.coordinatesPictureBox = new System.Windows.Forms.PictureBox();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.coordinatesSplitContainer)).BeginInit();
      this.coordinatesSplitContainer.Panel1.SuspendLayout();
      this.coordinatesSplitContainer.Panel2.SuspendLayout();
      this.coordinatesSplitContainer.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.coordinatesPictureBox)).BeginInit();
      this.SuspendLayout();
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Size = new System.Drawing.Size(688, 20);
      // 
      // infoLabel
      // 
      this.infoLabel.Location = new System.Drawing.Point(752, 3);
      // 
      // coordinatesSplitContainer
      // 
      this.coordinatesSplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.coordinatesSplitContainer.Location = new System.Drawing.Point(0, 26);
      this.coordinatesSplitContainer.Name = "coordinatesSplitContainer";
      // 
      // coordinatesSplitContainer.Panel1
      // 
      this.coordinatesSplitContainer.Panel1.Controls.Add(this.coordinatesMatrixView);
      // 
      // coordinatesSplitContainer.Panel2
      // 
      this.coordinatesSplitContainer.Panel2.Controls.Add(this.coordinatesPictureBox);
      this.coordinatesSplitContainer.Size = new System.Drawing.Size(771, 570);
      this.coordinatesSplitContainer.SplitterDistance = 256;
      this.coordinatesSplitContainer.TabIndex = 1;
      // 
      // coordinatesMatrixView
      // 
      this.coordinatesMatrixView.Caption = "StringConvertibleMatrix View";
      this.coordinatesMatrixView.Content = null;
      this.coordinatesMatrixView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.coordinatesMatrixView.Location = new System.Drawing.Point(0, 0);
      this.coordinatesMatrixView.Name = "coordinatesMatrixView";
      this.coordinatesMatrixView.ReadOnly = false;
      this.coordinatesMatrixView.ShowRowsAndColumnsTextBox = true;
      this.coordinatesMatrixView.ShowStatisticalInformation = true;
      this.coordinatesMatrixView.Size = new System.Drawing.Size(256, 570);
      this.coordinatesMatrixView.TabIndex = 1;
      // 
      // coordinatesPictureBox
      // 
      this.coordinatesPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.coordinatesPictureBox.Location = new System.Drawing.Point(0, 0);
      this.coordinatesPictureBox.Name = "coordinatesPictureBox";
      this.coordinatesPictureBox.Size = new System.Drawing.Size(511, 570);
      this.coordinatesPictureBox.TabIndex = 0;
      this.coordinatesPictureBox.TabStop = false;
      // 
      // CoordinatesTSPDataView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.coordinatesSplitContainer);
      this.Name = "CoordinatesTSPDataView";
      this.Size = new System.Drawing.Size(771, 596);
      this.Controls.SetChildIndex(this.coordinatesSplitContainer, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      this.Controls.SetChildIndex(this.infoLabel, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.coordinatesSplitContainer.Panel1.ResumeLayout(false);
      this.coordinatesSplitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.coordinatesSplitContainer)).EndInit();
      this.coordinatesSplitContainer.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.coordinatesPictureBox)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    protected System.Windows.Forms.SplitContainer coordinatesSplitContainer;
    protected Data.Views.StringConvertibleMatrixView coordinatesMatrixView;
    protected System.Windows.Forms.PictureBox coordinatesPictureBox;
  }
}
