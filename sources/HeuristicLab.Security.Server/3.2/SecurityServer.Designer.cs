namespace HeuristicLab.Security.Server {
  partial class SecurityServer {
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
      this.label1 = new System.Windows.Forms.Label();
      this.labelPermission = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.labelSecurity = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(26, 31);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(257, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Security Server Permission Manager running";
      // 
      // labelPermission
      // 
      this.labelPermission.AutoSize = true;
      this.labelPermission.Location = new System.Drawing.Point(29, 60);
      this.labelPermission.Name = "labelPermission";
      this.labelPermission.Size = new System.Drawing.Size(35, 13);
      this.labelPermission.TabIndex = 1;
      this.labelPermission.Text = "label2";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.Location = new System.Drawing.Point(29, 94);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(243, 13);
      this.label3.TabIndex = 2;
      this.label3.Text = "Security Server Security Manager running";
      // 
      // labelSecurity
      // 
      this.labelSecurity.AutoSize = true;
      this.labelSecurity.Location = new System.Drawing.Point(32, 128);
      this.labelSecurity.Name = "labelSecurity";
      this.labelSecurity.Size = new System.Drawing.Size(35, 13);
      this.labelSecurity.TabIndex = 3;
      this.labelSecurity.Text = "label4";
      // 
      // SecurityServer
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(345, 167);
      this.Controls.Add(this.labelSecurity);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.labelPermission);
      this.Controls.Add(this.label1);
      this.Name = "SecurityServer";
      this.Text = "SecurityServer";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label labelPermission;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label labelSecurity;
  }
}