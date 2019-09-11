namespace HeuristicLab.Problems.TravelingSalesman.Views {
  partial class MatrixTSPDataView {
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
      this.tabControl = new System.Windows.Forms.TabControl();
      this.distancesTabPage = new System.Windows.Forms.TabPage();
      this.setMatrixButton = new System.Windows.Forms.Button();
      this.coordinatesTabPage = new System.Windows.Forms.TabPage();
      this.distanceMatrixView = new HeuristicLab.Data.Views.StringConvertibleMatrixView();
      this.coordinatesSplitContainer = new System.Windows.Forms.SplitContainer();
      this.setCoordinatesButton = new System.Windows.Forms.Button();
      this.coordinatesMatrixView = new HeuristicLab.Data.Views.StringConvertibleMatrixView();
      this.coordinatesPictureBox = new System.Windows.Forms.PictureBox();
      this.tabControl.SuspendLayout();
      this.distancesTabPage.SuspendLayout();
      this.coordinatesTabPage.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.coordinatesSplitContainer)).BeginInit();
      this.coordinatesSplitContainer.Panel1.SuspendLayout();
      this.coordinatesSplitContainer.Panel2.SuspendLayout();
      this.coordinatesSplitContainer.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.coordinatesPictureBox)).BeginInit();
      this.SuspendLayout();
      // 
      // tabControl
      // 
      this.tabControl.Controls.Add(this.distancesTabPage);
      this.tabControl.Controls.Add(this.coordinatesTabPage);
      this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabControl.Location = new System.Drawing.Point(0, 0);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(771, 596);
      this.tabControl.TabIndex = 1;
      // 
      // distancesTabPage
      // 
      this.distancesTabPage.Controls.Add(this.setMatrixButton);
      this.distancesTabPage.Controls.Add(this.distanceMatrixView);
      this.distancesTabPage.Location = new System.Drawing.Point(4, 22);
      this.distancesTabPage.Name = "distancesTabPage";
      this.distancesTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.distancesTabPage.Size = new System.Drawing.Size(763, 570);
      this.distancesTabPage.TabIndex = 0;
      this.distancesTabPage.Text = "Distances";
      this.distancesTabPage.UseVisualStyleBackColor = true;
      // 
      // setMatrixButton
      // 
      this.setMatrixButton.Location = new System.Drawing.Point(6, 6);
      this.setMatrixButton.Name = "setMatrixButton";
      this.setMatrixButton.Size = new System.Drawing.Size(26, 23);
      this.setMatrixButton.TabIndex = 1;
      this.setMatrixButton.Text = "Set";
      this.setMatrixButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
      this.setMatrixButton.UseVisualStyleBackColor = true;
      this.setMatrixButton.Click += new System.EventHandler(this.SetMatrixButton_Click);
      // 
      // coordinatesTabPage
      // 
      this.coordinatesTabPage.Controls.Add(this.coordinatesSplitContainer);
      this.coordinatesTabPage.Location = new System.Drawing.Point(4, 22);
      this.coordinatesTabPage.Name = "coordinatesTabPage";
      this.coordinatesTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.coordinatesTabPage.Size = new System.Drawing.Size(763, 570);
      this.coordinatesTabPage.TabIndex = 1;
      this.coordinatesTabPage.Text = "Display Coordinates";
      this.coordinatesTabPage.UseVisualStyleBackColor = true;
      // 
      // distanceMatrixView
      // 
      this.distanceMatrixView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.distanceMatrixView.Caption = "StringConvertibleMatrix View";
      this.distanceMatrixView.Content = null;
      this.distanceMatrixView.Location = new System.Drawing.Point(6, 35);
      this.distanceMatrixView.Name = "distanceMatrixView";
      this.distanceMatrixView.ReadOnly = false;
      this.distanceMatrixView.ShowRowsAndColumnsTextBox = true;
      this.distanceMatrixView.ShowStatisticalInformation = true;
      this.distanceMatrixView.Size = new System.Drawing.Size(751, 529);
      this.distanceMatrixView.TabIndex = 0;
      // 
      // coordinatesSplitContainer
      // 
      this.coordinatesSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.coordinatesSplitContainer.Location = new System.Drawing.Point(3, 3);
      this.coordinatesSplitContainer.Name = "coordinatesSplitContainer";
      // 
      // coordinatesSplitContainer.Panel1
      // 
      this.coordinatesSplitContainer.Panel1.Controls.Add(this.setCoordinatesButton);
      this.coordinatesSplitContainer.Panel1.Controls.Add(this.coordinatesMatrixView);
      // 
      // coordinatesSplitContainer.Panel2
      // 
      this.coordinatesSplitContainer.Panel2.Controls.Add(this.coordinatesPictureBox);
      this.coordinatesSplitContainer.Size = new System.Drawing.Size(757, 564);
      this.coordinatesSplitContainer.SplitterDistance = 252;
      this.coordinatesSplitContainer.TabIndex = 0;
      // 
      // setCoordinatesButton
      // 
      this.setCoordinatesButton.Location = new System.Drawing.Point(3, 3);
      this.setCoordinatesButton.Name = "setCoordinatesButton";
      this.setCoordinatesButton.Size = new System.Drawing.Size(26, 23);
      this.setCoordinatesButton.TabIndex = 2;
      this.setCoordinatesButton.Text = "Set";
      this.setCoordinatesButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
      this.setCoordinatesButton.UseVisualStyleBackColor = true;
      this.setCoordinatesButton.Click += new System.EventHandler(this.SetCoordinatesButton_Click);
      // 
      // coordinatesMatrixView
      // 
      this.coordinatesMatrixView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.coordinatesMatrixView.Caption = "StringConvertibleMatrix View";
      this.coordinatesMatrixView.Content = null;
      this.coordinatesMatrixView.Location = new System.Drawing.Point(0, 32);
      this.coordinatesMatrixView.Name = "coordinatesMatrixView";
      this.coordinatesMatrixView.ReadOnly = false;
      this.coordinatesMatrixView.ShowRowsAndColumnsTextBox = true;
      this.coordinatesMatrixView.ShowStatisticalInformation = true;
      this.coordinatesMatrixView.Size = new System.Drawing.Size(249, 532);
      this.coordinatesMatrixView.TabIndex = 1;
      // 
      // coordinatesPictureBox
      // 
      this.coordinatesPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.coordinatesPictureBox.Location = new System.Drawing.Point(0, 0);
      this.coordinatesPictureBox.Name = "coordinatesPictureBox";
      this.coordinatesPictureBox.Size = new System.Drawing.Size(501, 564);
      this.coordinatesPictureBox.TabIndex = 0;
      this.coordinatesPictureBox.TabStop = false;
      // 
      // MatrixTSPDataView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tabControl);
      this.Name = "MatrixTSPDataView";
      this.Size = new System.Drawing.Size(771, 596);
      this.tabControl.ResumeLayout(false);
      this.distancesTabPage.ResumeLayout(false);
      this.coordinatesTabPage.ResumeLayout(false);
      this.coordinatesSplitContainer.Panel1.ResumeLayout(false);
      this.coordinatesSplitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.coordinatesSplitContainer)).EndInit();
      this.coordinatesSplitContainer.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.coordinatesPictureBox)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private Data.Views.StringConvertibleMatrixView distanceMatrixView;
    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TabPage coordinatesTabPage;
    private System.Windows.Forms.SplitContainer coordinatesSplitContainer;
    private Data.Views.StringConvertibleMatrixView coordinatesMatrixView;
    private System.Windows.Forms.TabPage distancesTabPage;
    private System.Windows.Forms.Button setMatrixButton;
    private System.Windows.Forms.PictureBox coordinatesPictureBox;
    private System.Windows.Forms.Button setCoordinatesButton;
  }
}
