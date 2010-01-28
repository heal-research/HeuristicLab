namespace HeuristicLab.MainForm.Test {
  partial class EditorView {
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
      this.ChangeStateButton = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
      this.label1.Location = new System.Drawing.Point(52, 73);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(57, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "EditorForm";
      // 
      // ChangeStateButton
      // 
      this.ChangeStateButton.Location = new System.Drawing.Point(34, 100);
      this.ChangeStateButton.Name = "ChangeStateButton";
      this.ChangeStateButton.Size = new System.Drawing.Size(75, 23);
      this.ChangeStateButton.TabIndex = 1;
      this.ChangeStateButton.Text = "ChangeState";
      this.ChangeStateButton.UseVisualStyleBackColor = true;
      this.ChangeStateButton.Click += new System.EventHandler(this.ChangeStateButton_Click);
      // 
      // EditorView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.ChangeStateButton);
      this.Controls.Add(this.label1);
      this.ForeColor = System.Drawing.SystemColors.ControlText;
      this.Name = "EditorView";
      this.VisibleChanged += new System.EventHandler(this.EditorView_VisibleChanged);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button ChangeStateButton;
  }
}
