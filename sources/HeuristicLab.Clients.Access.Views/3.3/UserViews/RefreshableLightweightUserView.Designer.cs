namespace HeuristicLab.Clients.Access.Views {
  partial class RefreshableLightweightUserView {
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
      this.lightweightUserView = new HeuristicLab.Clients.Access.Views.LightweightUserView();
      this.SuspendLayout();
      // 
      // lightweightUserView
      // 
      this.lightweightUserView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.lightweightUserView.Caption = "LightweightUser View";
      this.lightweightUserView.Content = null;
      this.lightweightUserView.Location = new System.Drawing.Point(3, 33);
      this.lightweightUserView.Name = "lightweightUserView";
      this.lightweightUserView.ReadOnly = false;
      this.lightweightUserView.Size = new System.Drawing.Size(500, 218);
      this.lightweightUserView.TabIndex = 2;
      this.lightweightUserView.SelectedUsersChanged += new System.EventHandler(this.lightweightUserView_SelectedUsersChanged);
      // 
      // RefreshableLightweightUserView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.lightweightUserView);
      this.Name = "RefreshableLightweightUserView";
      this.Controls.SetChildIndex(this.lightweightUserView, 0);
      this.Controls.SetChildIndex(this.refreshButton, 0);
      this.ResumeLayout(false);

    }

    #endregion

    private LightweightUserView lightweightUserView;
  }
}
