namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  partial class ConcreteItemsRestrictor {
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
      this.tableOptions = new System.Windows.Forms.TableLayoutPanel();
      this.SuspendLayout();
      // 
      // tableOptions
      // 
      this.tableOptions.AutoScroll = true;
      this.tableOptions.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.tableOptions.ColumnCount = 2;
      this.tableOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableOptions.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableOptions.Location = new System.Drawing.Point(0, 0);
      this.tableOptions.Name = "tableOptions";
      this.tableOptions.RowCount = 1;
      this.tableOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableOptions.Size = new System.Drawing.Size(500, 200);
      this.tableOptions.TabIndex = 13;
      // 
      // ConcreteItemsRestrictor
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableOptions);
      this.Margin = new System.Windows.Forms.Padding(0);
      this.Name = "ConcreteItemsRestrictor";
      this.Size = new System.Drawing.Size(500, 200);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableOptions;
  }
}
