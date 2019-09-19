namespace HeuristicLab.Problems.PTSP.Views {
  partial class MatrixPTSPDataView {
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
      this.probabilitiesTabPage = new System.Windows.Forms.TabPage();
      this.probabilitiesView = new HeuristicLab.Data.Views.StringConvertibleArrayView();
      this.tabControl.SuspendLayout();
      this.coordinatesTabPage.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.coordinatesSplitContainer)).BeginInit();
      this.coordinatesSplitContainer.Panel1.SuspendLayout();
      this.coordinatesSplitContainer.Panel2.SuspendLayout();
      this.coordinatesSplitContainer.SuspendLayout();
      this.distancesTabPage.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.coordinatesPictureBox)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.probabilitiesTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // distanceMatrixView
      // 
      this.distanceMatrixView.ShowStatisticalInformation = false;
      // 
      // tabControl
      // 
      this.tabControl.Controls.Add(this.probabilitiesTabPage);
      this.tabControl.Controls.SetChildIndex(this.probabilitiesTabPage, 0);
      this.tabControl.Controls.SetChildIndex(this.coordinatesTabPage, 0);
      this.tabControl.Controls.SetChildIndex(this.distancesTabPage, 0);
      // 
      // coordinatesTabPage
      // 
      this.coordinatesTabPage.Size = new System.Drawing.Size(763, 544);
      // 
      // coordinatesSplitContainer
      // 
      this.coordinatesSplitContainer.Size = new System.Drawing.Size(757, 538);
      // 
      // coordinatesMatrixView
      // 
      this.coordinatesMatrixView.ShowStatisticalInformation = false;
      this.coordinatesMatrixView.Size = new System.Drawing.Size(252, 538);
      // 
      // coordinatesPictureBox
      // 
      this.coordinatesPictureBox.Size = new System.Drawing.Size(501, 538);
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      // 
      // probabilitiesTabPage
      // 
      this.probabilitiesTabPage.Controls.Add(this.probabilitiesView);
      this.probabilitiesTabPage.Location = new System.Drawing.Point(4, 22);
      this.probabilitiesTabPage.Name = "probabilitiesTabPage";
      this.probabilitiesTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.probabilitiesTabPage.Size = new System.Drawing.Size(763, 544);
      this.probabilitiesTabPage.TabIndex = 2;
      this.probabilitiesTabPage.Text = "Probabilities";
      this.probabilitiesTabPage.UseVisualStyleBackColor = true;
      // 
      // probabilitiesView
      // 
      this.probabilitiesView.Caption = "StringConvertibleArray View";
      this.probabilitiesView.Content = null;
      this.probabilitiesView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.probabilitiesView.Location = new System.Drawing.Point(3, 3);
      this.probabilitiesView.Name = "probabilitiesView";
      this.probabilitiesView.ReadOnly = false;
      this.probabilitiesView.Size = new System.Drawing.Size(757, 538);
      this.probabilitiesView.TabIndex = 0;
      // 
      // MatrixPTSPDataView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.Name = "MatrixPTSPDataView";
      this.tabControl.ResumeLayout(false);
      this.coordinatesTabPage.ResumeLayout(false);
      this.coordinatesSplitContainer.Panel1.ResumeLayout(false);
      this.coordinatesSplitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.coordinatesSplitContainer)).EndInit();
      this.coordinatesSplitContainer.ResumeLayout(false);
      this.distancesTabPage.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.coordinatesPictureBox)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.probabilitiesTabPage.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TabPage probabilitiesTabPage;
    private Data.Views.StringConvertibleArrayView probabilitiesView;
  }
}
