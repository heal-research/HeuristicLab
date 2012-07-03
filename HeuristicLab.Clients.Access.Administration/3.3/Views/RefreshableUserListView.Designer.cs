namespace HeuristicLab.Clients.Access.Administration {
  partial class RefreshableUserListView {
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
      this.userListView = new HeuristicLab.Clients.Access.Administration.UserListView();
      this.storeButton = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // userListView
      // 
      this.userListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.userListView.Caption = "UserList View";
      this.userListView.Content = null;
      this.userListView.Location = new System.Drawing.Point(3, 33);
      this.userListView.Name = "userListView";
      this.userListView.ReadOnly = false;
      this.userListView.Size = new System.Drawing.Size(559, 375);
      this.userListView.TabIndex = 2;
      // 
      // storeButton
      // 
      this.storeButton.Enabled = false;
      this.storeButton.Location = new System.Drawing.Point(33, 3);
      this.storeButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.PublishToWeb;
      this.storeButton.Name = "storeButton";
      this.storeButton.Size = new System.Drawing.Size(24, 24);
      this.storeButton.TabIndex = 3;
      this.storeButton.UseVisualStyleBackColor = true;
      this.storeButton.Click += new System.EventHandler(this.storeButton_Click);
      // 
      // RefreshableUserListView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.userListView);
      this.Controls.Add(this.storeButton);
      this.Name = "RefreshableUserListView";
      this.Size = new System.Drawing.Size(565, 411);
      this.Controls.SetChildIndex(this.storeButton, 0);
      this.Controls.SetChildIndex(this.userListView, 0);
      this.Controls.SetChildIndex(this.refreshButton, 0);
      this.ResumeLayout(false);

    }

    #endregion

    private UserListView userListView;
    private System.Windows.Forms.Button storeButton;
  }
}
