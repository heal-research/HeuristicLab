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
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.groupBox1.SuspendLayout();
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
      this.tableOptions.Location = new System.Drawing.Point(3, 16);
      this.tableOptions.Margin = new System.Windows.Forms.Padding(0);
      this.tableOptions.Name = "tableOptions";
      this.tableOptions.RowCount = 1;
      this.tableOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableOptions.Size = new System.Drawing.Size(491, 181);
      this.tableOptions.TabIndex = 13;
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.tableOptions);
      this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox1.Location = new System.Drawing.Point(0, 0);
      this.groupBox1.Margin = new System.Windows.Forms.Padding(0);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 3, 6, 3);
      this.groupBox1.Size = new System.Drawing.Size(500, 200);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Allowed Items";
      // 
      // ConcreteItemsRestrictor
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.groupBox1);
      this.Margin = new System.Windows.Forms.Padding(0);
      this.Name = "ConcreteItemsRestrictor";
      this.Size = new System.Drawing.Size(500, 200);
      this.groupBox1.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableOptions;
    private System.Windows.Forms.GroupBox groupBox1;
  }
}
