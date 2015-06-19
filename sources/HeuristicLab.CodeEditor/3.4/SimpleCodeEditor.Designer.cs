namespace HeuristicLab.CodeEditor {
  partial class SimpleCodeEditor {
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
      this.TextEditor = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // TextEditor
      // 
      this.TextEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.TextEditor.Location = new System.Drawing.Point(3, 3);
      this.TextEditor.Multiline = true;
      this.TextEditor.Name = "TextEditor";
      this.TextEditor.Size = new System.Drawing.Size(158, 57);
      this.TextEditor.TabIndex = 0;
      this.TextEditor.TextChanged += new System.EventHandler(this.TextEditor_TextChanged);
      // 
      // SimpleCodeEditor
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.TextEditor);
      this.Name = "SimpleCodeEditor";
      this.Size = new System.Drawing.Size(164, 63);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox TextEditor;
  }
}
