namespace HeuristicLab.Clients.Access.Administration {
  partial class UserGroupView {
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
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.groupNameTextBox = new System.Windows.Forms.TextBox();
      this.idTextBox = new System.Windows.Forms.TextBox();
      this.refreshableLightweightUserView = new HeuristicLab.Clients.Access.Views.RefreshableLightweightUserView();
      this.storeButton = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(3, 6);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(70, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Group Name:";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(3, 32);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(19, 13);
      this.label2.TabIndex = 1;
      this.label2.Text = "Id:";
      // 
      // groupNameTextBox
      // 
      this.groupNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.groupNameTextBox.Location = new System.Drawing.Point(79, 3);
      this.groupNameTextBox.Name = "groupNameTextBox";
      this.groupNameTextBox.Size = new System.Drawing.Size(675, 20);
      this.groupNameTextBox.TabIndex = 2;
      this.groupNameTextBox.TextChanged += new System.EventHandler(this.groupNameTextBox_TextChanged);
      // 
      // idTextBox
      // 
      this.idTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.idTextBox.Enabled = false;
      this.idTextBox.Location = new System.Drawing.Point(79, 29);
      this.idTextBox.Name = "idTextBox";
      this.idTextBox.Size = new System.Drawing.Size(675, 20);
      this.idTextBox.TabIndex = 3;
      // 
      // refreshableLightweightUserView
      // 
      this.refreshableLightweightUserView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.refreshableLightweightUserView.Caption = "RefreshableLightweightUser View";
      this.refreshableLightweightUserView.Content = null;
      this.refreshableLightweightUserView.FetchSelectedUsers = null;
      this.refreshableLightweightUserView.Location = new System.Drawing.Point(0, 55);
      this.refreshableLightweightUserView.Name = "refreshableLightweightUserView";
      this.refreshableLightweightUserView.ReadOnly = false;
      this.refreshableLightweightUserView.Size = new System.Drawing.Size(754, 282);
      this.refreshableLightweightUserView.TabIndex = 4;
      // 
      // storeButton
      // 
      this.storeButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.PublishToWeb;
      this.storeButton.Location = new System.Drawing.Point(31, 58);
      this.storeButton.Name = "storeButton";
      this.storeButton.Size = new System.Drawing.Size(24, 24);
      this.storeButton.TabIndex = 5;
      this.storeButton.UseVisualStyleBackColor = true;
      this.storeButton.Click += new System.EventHandler(this.storeButton_Click);
      // 
      // UserGroupView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.storeButton);
      this.Controls.Add(this.refreshableLightweightUserView);
      this.Controls.Add(this.idTextBox);
      this.Controls.Add(this.groupNameTextBox);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Name = "UserGroupView";
      this.Size = new System.Drawing.Size(757, 340);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox groupNameTextBox;
    private System.Windows.Forms.TextBox idTextBox;
    private Views.RefreshableLightweightUserView refreshableLightweightUserView;
    private System.Windows.Forms.Button storeButton;
  }
}
