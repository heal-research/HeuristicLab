namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  partial class ValueLookupJsonItemControl {
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
      this.content = new System.Windows.Forms.Panel();
      this.SuspendLayout();
      // 
      // content
      // 
      this.content.Dock = System.Windows.Forms.DockStyle.Fill;
      this.content.Location = new System.Drawing.Point(0, 22);
      this.content.Margin = new System.Windows.Forms.Padding(0);
      this.content.Name = "content";
      this.content.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
      this.content.Size = new System.Drawing.Size(500, 523);
      this.content.TabIndex = 14;
      // 
      // ValueLookupJsonItemControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.content);
      this.Name = "ValueLookupJsonItemControl";
      this.Size = new System.Drawing.Size(500, 545);
      this.Controls.SetChildIndex(this.content, 0);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel content;
  }
}
