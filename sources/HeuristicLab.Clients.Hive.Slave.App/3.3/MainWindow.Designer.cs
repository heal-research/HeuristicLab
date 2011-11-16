namespace HeuristicLab.Clients.Hive.Slave.App {
  partial class MainWindow {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
      this.slaveMainView = new HeuristicLab.Clients.Hive.SlaveCore.Views.SlaveMainView();
      this.SuspendLayout();
      // 
      // slaveMainView
      // 
      this.slaveMainView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.slaveMainView.Caption = "HeuristicLab Slave View";
      this.slaveMainView.Content = null;
      this.slaveMainView.Location = new System.Drawing.Point(12, 12);
      this.slaveMainView.Name = "slaveMainView";
      this.slaveMainView.ReadOnly = false;
      this.slaveMainView.Size = new System.Drawing.Size(590, 488);
      this.slaveMainView.TabIndex = 0;
      // 
      // MainWindow
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(614, 512);
      this.Controls.Add(this.slaveMainView);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "MainWindow";
      this.Text = "Hive Slave";
      this.SizeChanged += new System.EventHandler(this.MainWindow_SizeChanged);
      this.ResumeLayout(false);

    }

    #endregion

    private SlaveCore.Views.SlaveMainView slaveMainView;
  }
}