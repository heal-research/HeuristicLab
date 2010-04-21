namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views {
  partial class SymbolicExpressionTreeChart {
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
      this.components = new System.ComponentModel.Container();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.SuspendLayout();
      // 
      // SymbolicExpressionTreeChart
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Name = "SymbolicExpressionTreeChart";
      this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SymbolicExpressionTreeChart_MouseMove);
      this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.SymbolicExpressionTreeChart_MouseDoubleClick);
      this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.SymbolicExpressionTreeChart_MouseClick);
      this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SymbolicExpressionTreeChart_MouseDown);
      this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.SymbolicExpressionTreeChart_MouseUp);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ToolTip toolTip;
  }
}
