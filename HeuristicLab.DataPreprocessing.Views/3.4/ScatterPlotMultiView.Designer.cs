namespace HeuristicLab.DataPreprocessing.Views {
  partial class ScatterPlotMultiView {
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
      this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
      this.SuspendLayout();
      // 
      // tableLayoutPanel
      // 
      this.tableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tableLayoutPanel.AutoScroll = true;
      this.tableLayoutPanel.ColumnCount = 1;
      this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel.Location = new System.Drawing.Point(3, 3);
      this.tableLayoutPanel.Name = "tableLayoutPanel";
      this.tableLayoutPanel.RowCount = 1;
      this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 514F));
      this.tableLayoutPanel.Size = new System.Drawing.Size(860, 514);
      this.tableLayoutPanel.TabIndex = 1;
      // 
      // MultiScatterPlotView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel);
      this.Name = "MultiScatterPlotView";
      this.Size = new System.Drawing.Size(863, 517);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;

  }
}
