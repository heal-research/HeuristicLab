namespace HeuristicLab.Problems.DataAnalysis.Views {
  partial class EnhancedStringConvertibleMatrixView {
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
      this.ShowHideRows = new System.Windows.Forms.ToolStripMenuItem();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // rowsTextBox
      // 
      this.errorProvider.SetIconAlignment(this.rowsTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.rowsTextBox, 2);
      //
      // dataGridView
      //
      this.dataGridView.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(dataGridView_CellPainting);
      // 
      // contextMenu
      // 
      this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ShowHideRows});
      // 
      // ShowHideRows
      // 
      this.ShowHideRows.Name = "ShowHideRows";
      this.ShowHideRows.Size = new System.Drawing.Size(190, 22);
      this.ShowHideRows.Text = "Show / Hide Rows";
      this.ShowHideRows.Click += new System.EventHandler(this.ShowHideRows_Click);
      // 
      // ColoredStringConvertibleMatrixView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Name = "ColoredStringConvertibleMatrixView";
      this.ShowRowsAndColumnsTextBox = false;
      this.ShowStatisticalInformation = false;
      this.Controls.SetChildIndex(this.statisticsTextBox, 0);
      this.Controls.SetChildIndex(this.rowsLabel, 0);
      this.Controls.SetChildIndex(this.columnsLabel, 0);
      this.Controls.SetChildIndex(this.rowsTextBox, 0);
      this.Controls.SetChildIndex(this.columnsTextBox, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }
    #endregion

    protected System.Windows.Forms.ToolStripMenuItem ShowHideRows;
  }
}
