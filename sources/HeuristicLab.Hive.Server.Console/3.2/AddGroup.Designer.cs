namespace HeuristicLab.Hive.Server.ServerConsole {
  partial class AddGroup {
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
      this.tbName = new System.Windows.Forms.TextBox();
      this.lblName = new System.Windows.Forms.Label();
      this.btnClose = new System.Windows.Forms.Button();
      this.btnAdd = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // tbName
      // 
      this.tbName.Location = new System.Drawing.Point(45, 6);
      this.tbName.Name = "tbName";
      this.tbName.Size = new System.Drawing.Size(276, 20);
      this.tbName.TabIndex = 7;
      this.tbName.TextChanged += new System.EventHandler(this.tbName_TextChanged);
      // 
      // lblName
      // 
      this.lblName.AutoSize = true;
      this.lblName.Location = new System.Drawing.Point(4, 9);
      this.lblName.Name = "lblName";
      this.lblName.Size = new System.Drawing.Size(35, 13);
      this.lblName.TabIndex = 10;
      this.lblName.Text = "Name";
      // 
      // btnClose
      // 
      this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnClose.Location = new System.Drawing.Point(246, 36);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(75, 23);
      this.btnClose.TabIndex = 9;
      this.btnClose.Text = "Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // btnAdd
      // 
      this.btnAdd.Enabled = false;
      this.btnAdd.Location = new System.Drawing.Point(7, 36);
      this.btnAdd.Name = "btnAdd";
      this.btnAdd.Size = new System.Drawing.Size(75, 23);
      this.btnAdd.TabIndex = 8;
      this.btnAdd.Text = "Add";
      this.btnAdd.UseVisualStyleBackColor = true;
      this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
      // 
      // AddGroup
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(337, 70);
      this.Controls.Add(this.tbName);
      this.Controls.Add(this.lblName);
      this.Controls.Add(this.btnClose);
      this.Controls.Add(this.btnAdd);
      this.Name = "AddGroup";
      this.Text = "AddGroup";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox tbName;
    private System.Windows.Forms.Label lblName;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.Button btnAdd;
  }
}