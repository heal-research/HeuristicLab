namespace HeuristicLab.Optimization.Views {
  partial class RunCollectionModificationEvaluatorView {
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
      this.evaluateButton = new System.Windows.Forms.Button();
      this.tabControl = new System.Windows.Forms.TabControl();
      this.runsTabPage = new System.Windows.Forms.TabPage();
      this.runCollectionViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.modifiersTabPage = new System.Windows.Forms.TabPage();
      this.modifiersViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.tabControl.SuspendLayout();
      this.runsTabPage.SuspendLayout();
      this.modifiersTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Size = new System.Drawing.Size(419, 20);
      // 
      // infoLabel
      // 
      this.infoLabel.Location = new System.Drawing.Point(483, 3);
      // 
      // evaluateButton
      // 
      this.evaluateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.evaluateButton.Location = new System.Drawing.Point(6, 266);
      this.evaluateButton.Name = "evaluateButton";
      this.evaluateButton.Size = new System.Drawing.Size(75, 23);
      this.evaluateButton.TabIndex = 4;
      this.evaluateButton.Text = "&Evaluate";
      this.evaluateButton.UseVisualStyleBackColor = true;
      this.evaluateButton.Click += new System.EventHandler(this.evaluateButton_Click);
      // 
      // tabControl
      // 
      this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl.Controls.Add(this.runsTabPage);
      this.tabControl.Controls.Add(this.modifiersTabPage);
      this.tabControl.Location = new System.Drawing.Point(3, 26);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(496, 318);
      this.tabControl.TabIndex = 5;
      // 
      // runsTabPage
      // 
      this.runsTabPage.Controls.Add(this.runCollectionViewHost);
      this.runsTabPage.Location = new System.Drawing.Point(4, 22);
      this.runsTabPage.Name = "runsTabPage";
      this.runsTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.runsTabPage.Size = new System.Drawing.Size(488, 292);
      this.runsTabPage.TabIndex = 0;
      this.runsTabPage.Text = "Runs";
      this.runsTabPage.UseVisualStyleBackColor = true;
      // 
      // runCollectionViewHost
      // 
      this.runCollectionViewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.runCollectionViewHost.Caption = "View";
      this.runCollectionViewHost.Content = null;
      this.runCollectionViewHost.Enabled = false;
      this.runCollectionViewHost.Location = new System.Drawing.Point(6, 6);
      this.runCollectionViewHost.Name = "runCollectionViewHost";
      this.runCollectionViewHost.ReadOnly = false;
      this.runCollectionViewHost.Size = new System.Drawing.Size(476, 280);
      this.runCollectionViewHost.TabIndex = 0;
      this.runCollectionViewHost.ViewsLabelVisible = true;
      this.runCollectionViewHost.ViewType = null;
      // 
      // modifiersTabPage
      // 
      this.modifiersTabPage.Controls.Add(this.modifiersViewHost);
      this.modifiersTabPage.Controls.Add(this.evaluateButton);
      this.modifiersTabPage.Location = new System.Drawing.Point(4, 22);
      this.modifiersTabPage.Name = "modifiersTabPage";
      this.modifiersTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.modifiersTabPage.Size = new System.Drawing.Size(488, 292);
      this.modifiersTabPage.TabIndex = 1;
      this.modifiersTabPage.Text = "Modifiers";
      this.modifiersTabPage.UseVisualStyleBackColor = true;
      // 
      // modifiersViewHost
      // 
      this.modifiersViewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.modifiersViewHost.Caption = "View";
      this.modifiersViewHost.Content = null;
      this.modifiersViewHost.Enabled = false;
      this.modifiersViewHost.Location = new System.Drawing.Point(6, 6);
      this.modifiersViewHost.Name = "modifiersViewHost";
      this.modifiersViewHost.ReadOnly = false;
      this.modifiersViewHost.Size = new System.Drawing.Size(476, 254);
      this.modifiersViewHost.TabIndex = 0;
      this.modifiersViewHost.ViewsLabelVisible = true;
      this.modifiersViewHost.ViewType = null;
      // 
      // RunCollectionModificationEvaluatorView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.Controls.Add(this.tabControl);
      this.Name = "RunCollectionModificationEvaluatorView";
      this.Size = new System.Drawing.Size(502, 347);
      this.Controls.SetChildIndex(this.tabControl, 0);
      this.Controls.SetChildIndex(this.infoLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.tabControl.ResumeLayout(false);
      this.runsTabPage.ResumeLayout(false);
      this.modifiersTabPage.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button evaluateButton;
    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TabPage runsTabPage;
    private MainForm.WindowsForms.ViewHost runCollectionViewHost;
    private System.Windows.Forms.TabPage modifiersTabPage;
    private MainForm.WindowsForms.ViewHost modifiersViewHost;
  }
}
