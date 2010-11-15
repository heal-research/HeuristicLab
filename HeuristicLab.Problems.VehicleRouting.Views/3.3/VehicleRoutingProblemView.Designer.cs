namespace HeuristicLab.Problems.VehicleRouting.Views {
  partial class VehicleRoutingProblemView {
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
      this.importButton = new System.Windows.Forms.Button();
      this.tabControl1 = new System.Windows.Forms.TabControl();
      this.tabPage1 = new System.Windows.Forms.TabPage();
      this.parameterCollectionView = new HeuristicLab.Core.Views.ParameterCollectionView();
      this.tabPage2 = new System.Windows.Forms.TabPage();
      this.importButton2 = new System.Windows.Forms.Button();
      this.importButton3 = new System.Windows.Forms.Button();
      this.importBestButton = new System.Windows.Forms.Button();
      this.vrpSolutionView = new HeuristicLab.Problems.VehicleRouting.Views.VRPSolutionView();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.tabControl1.SuspendLayout();
      this.tabPage1.SuspendLayout();
      this.tabPage2.SuspendLayout();
      this.SuspendLayout();
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      // 
      // importButton
      // 
      this.importButton.Location = new System.Drawing.Point(0, 55);
      this.importButton.Name = "importButton";
      this.importButton.Size = new System.Drawing.Size(139, 23);
      this.importButton.TabIndex = 5;
      this.importButton.Text = "Import from Solomon";
      this.importButton.UseVisualStyleBackColor = true;
      this.importButton.Click += new System.EventHandler(this.importButton_Click);
      // 
      // tabControl1
      // 
      this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl1.Controls.Add(this.tabPage1);
      this.tabControl1.Controls.Add(this.tabPage2);
      this.tabControl1.Location = new System.Drawing.Point(0, 111);
      this.tabControl1.Name = "tabControl1";
      this.tabControl1.SelectedIndex = 0;
      this.tabControl1.Size = new System.Drawing.Size(490, 255);
      this.tabControl1.TabIndex = 6;
      // 
      // tabPage1
      // 
      this.tabPage1.Controls.Add(this.parameterCollectionView);
      this.tabPage1.Location = new System.Drawing.Point(4, 22);
      this.tabPage1.Name = "tabPage1";
      this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage1.Size = new System.Drawing.Size(482, 229);
      this.tabPage1.TabIndex = 0;
      this.tabPage1.Text = "Parameters";
      this.tabPage1.UseVisualStyleBackColor = true;
      // 
      // parameterCollectionView
      // 
      this.parameterCollectionView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.parameterCollectionView.Caption = "ParameterCollection View";
      this.parameterCollectionView.Content = null;
      this.parameterCollectionView.Location = new System.Drawing.Point(3, 3);
      this.parameterCollectionView.Name = "parameterCollectionView";
      this.parameterCollectionView.ReadOnly = false;
      this.parameterCollectionView.Size = new System.Drawing.Size(476, 223);
      this.parameterCollectionView.TabIndex = 1;
      // 
      // tabPage2
      // 
      this.tabPage2.Controls.Add(this.vrpSolutionView);
      this.tabPage2.Location = new System.Drawing.Point(4, 22);
      this.tabPage2.Name = "tabPage2";
      this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage2.Size = new System.Drawing.Size(482, 229);
      this.tabPage2.TabIndex = 1;
      this.tabPage2.Text = "Visualization";
      this.tabPage2.UseVisualStyleBackColor = true;
      // 
      // importButton2
      // 
      this.importButton2.Location = new System.Drawing.Point(145, 55);
      this.importButton2.Name = "importButton2";
      this.importButton2.Size = new System.Drawing.Size(126, 23);
      this.importButton2.TabIndex = 7;
      this.importButton2.Text = "Import from TSPLib";
      this.importButton2.UseVisualStyleBackColor = true;
      this.importButton2.Click += new System.EventHandler(this.importButton2_Click);
      // 
      // importButton3
      // 
      this.importButton3.Location = new System.Drawing.Point(277, 55);
      this.importButton3.Name = "importButton3";
      this.importButton3.Size = new System.Drawing.Size(131, 23);
      this.importButton3.TabIndex = 8;
      this.importButton3.Text = "Import from ORLib";
      this.importButton3.UseVisualStyleBackColor = true;
      this.importButton3.Click += new System.EventHandler(this.importButton3_Click);
      // 
      // importBestButton
      // 
      this.importBestButton.Location = new System.Drawing.Point(0, 82);
      this.importBestButton.Name = "importBestButton";
      this.importBestButton.Size = new System.Drawing.Size(139, 23);
      this.importBestButton.TabIndex = 9;
      this.importBestButton.Text = "Import solution";
      this.importBestButton.UseVisualStyleBackColor = true;
      this.importBestButton.Click += new System.EventHandler(this.importBestButton_Click);
      // 
      // vrpSolutionView
      // 
      this.vrpSolutionView.Caption = "VRPSolution View";
      this.vrpSolutionView.Content = null;
      this.vrpSolutionView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.vrpSolutionView.Location = new System.Drawing.Point(3, 3);
      this.vrpSolutionView.Name = "vrpSolutionView";
      this.vrpSolutionView.ReadOnly = false;
      this.vrpSolutionView.Size = new System.Drawing.Size(476, 223);
      this.vrpSolutionView.TabIndex = 0;
      // 
      // VehicleRoutingProblemView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.Controls.Add(this.importBestButton);
      this.Controls.Add(this.importButton3);
      this.Controls.Add(this.importButton2);
      this.Controls.Add(this.importButton);
      this.Controls.Add(this.tabControl1);
      this.Name = "VehicleRoutingProblemView";
      this.Size = new System.Drawing.Size(490, 369);
      this.Controls.SetChildIndex(this.tabControl1, 0);
      this.Controls.SetChildIndex(this.importButton, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.descriptionLabel, 0);
      this.Controls.SetChildIndex(this.descriptionTextBox, 0);
      this.Controls.SetChildIndex(this.importButton2, 0);
      this.Controls.SetChildIndex(this.importButton3, 0);
      this.Controls.SetChildIndex(this.importBestButton, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.tabControl1.ResumeLayout(false);
      this.tabPage1.ResumeLayout(false);
      this.tabPage2.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button importButton;
    private System.Windows.Forms.TabControl tabControl1;
    private System.Windows.Forms.TabPage tabPage1;
    private System.Windows.Forms.TabPage tabPage2;
    private Core.Views.ParameterCollectionView parameterCollectionView;
    private VRPSolutionView vrpSolutionView;
    private System.Windows.Forms.Button importButton2;
    private System.Windows.Forms.Button importButton3;
    private System.Windows.Forms.Button importBestButton;
  }
}
