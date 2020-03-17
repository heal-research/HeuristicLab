namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  partial class LookupJsonItemControl {
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
      this.textBoxActualName = new System.Windows.Forms.TextBox();
      this.labelActualName = new System.Windows.Forms.Label();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.tableLayoutPanel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // textBoxActualName
      // 
      this.textBoxActualName.Dock = System.Windows.Forms.DockStyle.Fill;
      this.textBoxActualName.Location = new System.Drawing.Point(100, 0);
      this.textBoxActualName.Margin = new System.Windows.Forms.Padding(0);
      this.textBoxActualName.Name = "textBoxActualName";
      this.textBoxActualName.Size = new System.Drawing.Size(316, 20);
      this.textBoxActualName.TabIndex = 12;
      // 
      // labelActualName
      // 
      this.labelActualName.AutoSize = true;
      this.labelActualName.Dock = System.Windows.Forms.DockStyle.Fill;
      this.labelActualName.Location = new System.Drawing.Point(0, 0);
      this.labelActualName.Margin = new System.Windows.Forms.Padding(0);
      this.labelActualName.Name = "labelActualName";
      this.labelActualName.Size = new System.Drawing.Size(100, 22);
      this.labelActualName.TabIndex = 11;
      this.labelActualName.Text = "ActualName";
      this.labelActualName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 2;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Controls.Add(this.labelActualName, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.textBoxActualName, 1, 0);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 1;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(416, 22);
      this.tableLayoutPanel1.TabIndex = 13;
      // 
      // LookupJsonItemControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "LookupJsonItemControl";
      this.Size = new System.Drawing.Size(416, 23);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion
    protected System.Windows.Forms.TextBox textBoxActualName;
    protected System.Windows.Forms.Label labelActualName;
    protected System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
  }
}
