namespace HeuristicLab.Hive.Server.Console {
  partial class AddJobForm {
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
      this.lblParentJob = new System.Windows.Forms.Label();
      this.cbParJob = new System.Windows.Forms.ComboBox();
      this.btnAdd = new System.Windows.Forms.Button();
      this.btnClose = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // lblParentJob
      // 
      this.lblParentJob.AutoSize = true;
      this.lblParentJob.Location = new System.Drawing.Point(12, 9);
      this.lblParentJob.Name = "lblParentJob";
      this.lblParentJob.Size = new System.Drawing.Size(58, 13);
      this.lblParentJob.TabIndex = 1;
      this.lblParentJob.Text = "Parent Job";
      // 
      // cbParJob
      // 
      this.cbParJob.FormattingEnabled = true;
      this.cbParJob.Location = new System.Drawing.Point(120, 6);
      this.cbParJob.Name = "cbParJob";
      this.cbParJob.Size = new System.Drawing.Size(212, 21);
      this.cbParJob.TabIndex = 3;
      // 
      // btnAdd
      // 
      this.btnAdd.Location = new System.Drawing.Point(15, 38);
      this.btnAdd.Name = "btnAdd";
      this.btnAdd.Size = new System.Drawing.Size(75, 23);
      this.btnAdd.TabIndex = 4;
      this.btnAdd.Text = "Add";
      this.btnAdd.UseVisualStyleBackColor = true;
      this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
      // 
      // btnClose
      // 
      this.btnClose.Location = new System.Drawing.Point(257, 38);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(75, 23);
      this.btnClose.TabIndex = 5;
      this.btnClose.Text = "Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // AddJobForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(344, 68);
      this.Controls.Add(this.btnClose);
      this.Controls.Add(this.btnAdd);
      this.Controls.Add(this.cbParJob);
      this.Controls.Add(this.lblParentJob);
      this.Name = "AddJobForm";
      this.Text = "Add Job";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label lblParentJob;
    private System.Windows.Forms.ComboBox cbParJob;
    private System.Windows.Forms.Button btnAdd;
    private System.Windows.Forms.Button btnClose;
  }
}