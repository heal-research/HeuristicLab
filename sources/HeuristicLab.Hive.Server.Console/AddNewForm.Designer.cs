namespace HeuristicLab.Hive.Server.Console {
  partial class AddNewForm {
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
      this.lblOne = new System.Windows.Forms.Label();
      this.lblGroup = new System.Windows.Forms.Label();
      this.textBox1 = new System.Windows.Forms.TextBox();
      this.comboBox1 = new System.Windows.Forms.ComboBox();
      this.btnAdd = new System.Windows.Forms.Button();
      this.btnClose = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // lblOne
      // 
      this.lblOne.AutoSize = true;
      this.lblOne.Location = new System.Drawing.Point(12, 14);
      this.lblOne.Name = "lblOne";
      this.lblOne.Size = new System.Drawing.Size(35, 13);
      this.lblOne.TabIndex = 0;
      this.lblOne.Text = "label1";
      // 
      // lblGroup
      // 
      this.lblGroup.AutoSize = true;
      this.lblGroup.Location = new System.Drawing.Point(12, 51);
      this.lblGroup.Name = "lblGroup";
      this.lblGroup.Size = new System.Drawing.Size(35, 13);
      this.lblGroup.TabIndex = 1;
      this.lblGroup.Text = "label2";
      // 
      // textBox1
      // 
      this.textBox1.Location = new System.Drawing.Point(120, 7);
      this.textBox1.Name = "textBox1";
      this.textBox1.Size = new System.Drawing.Size(212, 20);
      this.textBox1.TabIndex = 2;
      // 
      // comboBox1
      // 
      this.comboBox1.FormattingEnabled = true;
      this.comboBox1.Location = new System.Drawing.Point(120, 42);
      this.comboBox1.Name = "comboBox1";
      this.comboBox1.Size = new System.Drawing.Size(212, 21);
      this.comboBox1.TabIndex = 3;
      // 
      // btnAdd
      // 
      this.btnAdd.Location = new System.Drawing.Point(12, 69);
      this.btnAdd.Name = "btnAdd";
      this.btnAdd.Size = new System.Drawing.Size(75, 23);
      this.btnAdd.TabIndex = 4;
      this.btnAdd.Text = "Add";
      this.btnAdd.UseVisualStyleBackColor = true;
      // 
      // btnClose
      // 
      this.btnClose.Location = new System.Drawing.Point(257, 69);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(75, 23);
      this.btnClose.TabIndex = 5;
      this.btnClose.Text = "Close";
      this.btnClose.UseVisualStyleBackColor = true;
      // 
      // AddNewForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(344, 102);
      this.Controls.Add(this.btnClose);
      this.Controls.Add(this.btnAdd);
      this.Controls.Add(this.comboBox1);
      this.Controls.Add(this.textBox1);
      this.Controls.Add(this.lblGroup);
      this.Controls.Add(this.lblOne);
      this.Name = "AddNewForm";
      this.Text = "AddNewForm";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label lblOne;
    private System.Windows.Forms.Label lblGroup;
    private System.Windows.Forms.TextBox textBox1;
    private System.Windows.Forms.ComboBox comboBox1;
    private System.Windows.Forms.Button btnAdd;
    private System.Windows.Forms.Button btnClose;
  }
}