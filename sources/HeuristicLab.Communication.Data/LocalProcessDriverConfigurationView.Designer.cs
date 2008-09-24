namespace HeuristicLab.Communication.Data {
  partial class LocalProcessDriverConfigurationView {
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
      this.executablePathStringDataView = new HeuristicLab.Data.StringDataView();
      this.argumentsStringDataView = new HeuristicLab.Data.StringDataView();
      this.executableLabel = new System.Windows.Forms.Label();
      this.argumentsLabel = new System.Windows.Forms.Label();
      this.browseExecutableButtom = new System.Windows.Forms.Button();
      this.executablePathOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
      this.SuspendLayout();
      // 
      // executablePathStringDataView
      // 
      this.executablePathStringDataView.Caption = "View";
      this.executablePathStringDataView.Location = new System.Drawing.Point(69, 3);
      this.executablePathStringDataView.Name = "executablePathStringDataView";
      this.executablePathStringDataView.Size = new System.Drawing.Size(200, 25);
      this.executablePathStringDataView.StringData = null;
      this.executablePathStringDataView.TabIndex = 0;
      // 
      // argumentsStringDataView
      // 
      this.argumentsStringDataView.Caption = "View";
      this.argumentsStringDataView.Location = new System.Drawing.Point(69, 31);
      this.argumentsStringDataView.Name = "argumentsStringDataView";
      this.argumentsStringDataView.Size = new System.Drawing.Size(200, 25);
      this.argumentsStringDataView.StringData = null;
      this.argumentsStringDataView.TabIndex = 1;
      // 
      // executableLabel
      // 
      this.executableLabel.AutoSize = true;
      this.executableLabel.Location = new System.Drawing.Point(3, 6);
      this.executableLabel.Name = "executableLabel";
      this.executableLabel.Size = new System.Drawing.Size(60, 13);
      this.executableLabel.TabIndex = 2;
      this.executableLabel.Text = "Executable";
      // 
      // argumentsLabel
      // 
      this.argumentsLabel.AutoSize = true;
      this.argumentsLabel.Location = new System.Drawing.Point(3, 34);
      this.argumentsLabel.Name = "argumentsLabel";
      this.argumentsLabel.Size = new System.Drawing.Size(57, 13);
      this.argumentsLabel.TabIndex = 3;
      this.argumentsLabel.Text = "Arguments";
      // 
      // browseExecutableButtom
      // 
      this.browseExecutableButtom.Location = new System.Drawing.Point(275, 1);
      this.browseExecutableButtom.Name = "browseExecutableButtom";
      this.browseExecutableButtom.Size = new System.Drawing.Size(75, 23);
      this.browseExecutableButtom.TabIndex = 4;
      this.browseExecutableButtom.Text = "Browse...";
      this.browseExecutableButtom.UseVisualStyleBackColor = true;
      this.browseExecutableButtom.Click += new System.EventHandler(this.browseExecutableButtom_Click);
      // 
      // executablePathOpenFileDialog
      // 
      this.executablePathOpenFileDialog.FileName = "*.exe";
      this.executablePathOpenFileDialog.Filter = "Executables|*.exe|All files|*.*";
      // 
      // LocalProcessDriverConfigurationView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.browseExecutableButtom);
      this.Controls.Add(this.argumentsLabel);
      this.Controls.Add(this.executableLabel);
      this.Controls.Add(this.argumentsStringDataView);
      this.Controls.Add(this.executablePathStringDataView);
      this.Name = "LocalProcessDriverConfigurationView";
      this.Size = new System.Drawing.Size(352, 54);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private HeuristicLab.Data.StringDataView executablePathStringDataView;
    private HeuristicLab.Data.StringDataView argumentsStringDataView;
    private System.Windows.Forms.Label executableLabel;
    private System.Windows.Forms.Label argumentsLabel;
    private System.Windows.Forms.Button browseExecutableButtom;
    private System.Windows.Forms.OpenFileDialog executablePathOpenFileDialog;
  }
}
