namespace HeuristicLab.Clients.Access.Administration {
  partial class RoleView {
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
      this.label2 = new System.Windows.Forms.Label();
      this.roleNameTextBox = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(3, 6);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(63, 13);
      this.label2.TabIndex = 1;
      this.label2.Text = "Role Name:";
      // 
      // roleNameTextBox
      // 
      this.roleNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.roleNameTextBox.Location = new System.Drawing.Point(75, 3);
      this.roleNameTextBox.Name = "roleNameTextBox";
      this.roleNameTextBox.Size = new System.Drawing.Size(289, 20);
      this.roleNameTextBox.TabIndex = 3;
      this.roleNameTextBox.TextChanged += new System.EventHandler(this.roleNameTextBox_TextChanged);
      // 
      // RoleView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.roleNameTextBox);
      this.Controls.Add(this.label2);
      this.Name = "RoleView";
      this.Size = new System.Drawing.Size(364, 27);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox roleNameTextBox;
  }
}
