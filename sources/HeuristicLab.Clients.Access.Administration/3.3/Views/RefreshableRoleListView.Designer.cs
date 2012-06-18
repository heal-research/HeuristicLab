namespace HeuristicLab.Clients.Access.Administration {
  partial class RefreshableRoleListView {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RefreshableRoleListView));
      this.roleListView = new HeuristicLab.Clients.Access.Administration.RoleListView();
      this.storeButton = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // roleListView
      // 
      this.roleListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.roleListView.Caption = "UserList View";
      this.roleListView.Content = null;
      this.roleListView.Location = new System.Drawing.Point(3, 33);
      this.roleListView.Name = "roleListView";
      this.roleListView.ReadOnly = false;
      this.roleListView.Size = new System.Drawing.Size(559, 375);
      this.roleListView.TabIndex = 2;
      // 
      // storeButton
      // 
      this.storeButton.Enabled = false;
      this.storeButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.PublishToWeb;
      this.storeButton.Location = new System.Drawing.Point(33, 3);
      this.storeButton.Name = "storeButton";
      this.storeButton.Size = new System.Drawing.Size(24, 24);
      this.storeButton.TabIndex = 3;
      this.storeButton.UseVisualStyleBackColor = true;
      this.storeButton.Click += new System.EventHandler(this.storeButton_Click);
      // 
      // RefreshableRoleListView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.roleListView);
      this.Controls.Add(this.storeButton);
      this.Name = "RefreshableRoleListView";
      this.Size = new System.Drawing.Size(565, 411);
      this.Controls.SetChildIndex(this.storeButton, 0);
      this.Controls.SetChildIndex(this.roleListView, 0);
      this.Controls.SetChildIndex(this.refreshButton, 0);
      this.ResumeLayout(false);

    }

    #endregion

    private RoleListView roleListView;
    private System.Windows.Forms.Button storeButton;
  }
}
