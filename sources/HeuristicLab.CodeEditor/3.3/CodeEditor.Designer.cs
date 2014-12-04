namespace HeuristicLab.CodeEditor {
  public partial class CodeEditor : System.Windows.Forms.UserControl {
    /// <summary>
    /// Designer variable used to keep track of non-visual components.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Disposes resources used by the form.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      runParser = false;
      if (disposing) {
        if (components != null) {
          components.Dispose();
        }
      }
      base.Dispose(disposing);
    }

    /// <summary>
    /// This method is required for Windows Forms designer support.
    /// Do not change the method contents inside the source code editor. The Forms designer might
    /// not be able to load this method if it was changed manually.
    /// </summary>
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      System.Windows.Forms.ToolStripStatusLabel toolStripFiller;
      this.textEditor = new ICSharpCode.TextEditor.TextEditorControl();
      this.statusStrip1 = new System.Windows.Forms.StatusStrip();
      this.parserThreadLabel = new System.Windows.Forms.ToolStripStatusLabel();
      this.errorLabel = new System.Windows.Forms.ToolStripStatusLabel();
      this.sharpDevelopLabel = new System.Windows.Forms.ToolStripStatusLabel();
      this.imageList1 = new System.Windows.Forms.ImageList(this.components);
      toolStripFiller = new System.Windows.Forms.ToolStripStatusLabel();
      this.statusStrip1.SuspendLayout();
      this.SuspendLayout();
      // 
      // toolStripFiller
      // 
      toolStripFiller.Name = "toolStripFiller";
      toolStripFiller.Size = new System.Drawing.Size(395, 17);
      toolStripFiller.Spring = true;
      // 
      // textEditor
      // 
      this.textEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.textEditor.ConvertTabsToSpaces = true;
      this.textEditor.IndentStyle = ICSharpCode.TextEditor.Document.IndentStyle.Auto;
      this.textEditor.IsIconBarVisible = true;
      this.textEditor.IsReadOnly = false;
      this.textEditor.LineViewerStyle = ICSharpCode.TextEditor.Document.LineViewerStyle.FullRow;
      this.textEditor.Location = new System.Drawing.Point(0, 0);
      this.textEditor.Name = "textEditor";
      this.textEditor.ShowEOLMarkers = true;
      this.textEditor.ShowSpaces = true;
      this.textEditor.ShowTabs = true;
      this.textEditor.ShowVRuler = false;
      this.textEditor.Size = new System.Drawing.Size(556, 245);
      this.textEditor.TabIndent = 2;
      this.textEditor.TabIndex = 0;
      // 
      // statusStrip1
      // 
      this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.parserThreadLabel,
            toolStripFiller,
            this.errorLabel,
            this.sharpDevelopLabel});
      this.statusStrip1.Location = new System.Drawing.Point(0, 248);
      this.statusStrip1.Name = "statusStrip1";
      this.statusStrip1.Size = new System.Drawing.Size(556, 22);
      this.statusStrip1.TabIndex = 1;
      this.statusStrip1.Text = "statusStrip1";
      // 
      // parserThreadLabel
      // 
      this.parserThreadLabel.Name = "parserThreadLabel";
      this.parserThreadLabel.Size = new System.Drawing.Size(39, 17);
      this.parserThreadLabel.Text = "Ready";
      // 
      // errorLabel
      // 
      this.errorLabel.ForeColor = System.Drawing.Color.Red;
      this.errorLabel.Name = "errorLabel";
      this.errorLabel.Size = new System.Drawing.Size(0, 17);
      // 
      // sharpDevelopLabel
      // 
      this.sharpDevelopLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.sharpDevelopLabel.IsLink = true;
      this.sharpDevelopLabel.Name = "sharpDevelopLabel";
      this.sharpDevelopLabel.Size = new System.Drawing.Size(107, 17);
      this.sharpDevelopLabel.Tag = "http://www.icsharpcode.net/OpenSource/SD/";
      this.sharpDevelopLabel.Text = "powered by #develop";
      this.sharpDevelopLabel.ToolTipText = "Syntax highlighting and code completion facilities provided through #develop libr" +
    "aries";
      this.sharpDevelopLabel.Click += new System.EventHandler(this.toolStripStatusLabel1_Click);
      // 
      // imageList1
      // 
      this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
      this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
      this.imageList1.TransparentColor = System.Drawing.Color.White;
      // 
      // CodeEditor
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.textEditor);
      this.Controls.Add(this.statusStrip1);
      this.Name = "CodeEditor";
      this.Size = new System.Drawing.Size(556, 270);
      this.Resize += new System.EventHandler(this.CodeEditor_Resize);
      this.statusStrip1.ResumeLayout(false);
      this.statusStrip1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }
    internal System.Windows.Forms.ImageList imageList1;
    private System.Windows.Forms.ToolStripStatusLabel parserThreadLabel;
    private System.Windows.Forms.StatusStrip statusStrip1;
    private ICSharpCode.TextEditor.TextEditorControl textEditor;
    private System.Windows.Forms.ToolStripStatusLabel sharpDevelopLabel;
    private System.Windows.Forms.ToolStripStatusLabel errorLabel;
  }
}
