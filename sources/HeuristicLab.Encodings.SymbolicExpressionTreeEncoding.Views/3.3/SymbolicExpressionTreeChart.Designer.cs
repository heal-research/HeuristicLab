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
      this.pictureBox = new System.Windows.Forms.PictureBox();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
      this.SuspendLayout();
      // 
      // pictureBox
      // 
      this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.pictureBox.Location = new System.Drawing.Point(0, 0);
      this.pictureBox.Name = "pictureBox";
      this.pictureBox.Size = new System.Drawing.Size(150, 150);
      this.pictureBox.TabIndex = 0;
      this.pictureBox.TabStop = false;
      this.pictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SymbolicExpressionTreeChart_MouseMove);
      this.pictureBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.SymbolicExpressionTreeChart_MouseDoubleClick);
      this.pictureBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.SymbolicExpressionTreeChart_MouseClick);
      this.pictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SymbolicExpressionTreeChart_MouseDown);
      this.pictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.SymbolicExpressionTreeChart_MouseUp);
      // 
      // SymbolicExpressionTreeChart
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.pictureBox);
      this.Name = "SymbolicExpressionTreeChart";
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.PictureBox pictureBox;
    private System.Windows.Forms.ToolTip toolTip;
  }
}
