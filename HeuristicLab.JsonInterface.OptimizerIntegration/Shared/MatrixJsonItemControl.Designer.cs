
namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  partial class MatrixJsonItemControl {
    /// <summary> 
    /// Erforderliche Designervariable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Verwendete Ressourcen bereinigen.
    /// </summary>
    /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Vom Komponenten-Designer generierter Code

    /// <summary> 
    /// Erforderliche Methode für die Designerunterstützung. 
    /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
    /// </summary>
    private void InitializeComponent() {
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.checkBoxColumnsResizable = new System.Windows.Forms.CheckBox();
      this.checkBoxRowsResizable = new System.Windows.Forms.CheckBox();
      this.tableLayoutPanel1.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.tableLayoutPanel2.SuspendLayout();
      this.SuspendLayout();
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 1;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Controls.Add(this.groupBox2, 0, 0);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 2;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 63F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.Size = new System.Drawing.Size(526, 63);
      this.tableLayoutPanel1.TabIndex = 24;
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.tableLayoutPanel2);
      this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox2.Location = new System.Drawing.Point(0, 0);
      this.groupBox2.Margin = new System.Windows.Forms.Padding(0);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(526, 63);
      this.groupBox2.TabIndex = 19;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Matrix Properties";
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.ColumnCount = 1;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel2.Controls.Add(this.checkBoxRowsResizable, 0, 0);
      this.tableLayoutPanel2.Controls.Add(this.checkBoxColumnsResizable, 0, 1);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 16);
      this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 2;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(520, 44);
      this.tableLayoutPanel2.TabIndex = 22;
      // 
      // checkBoxColumnsResizable
      // 
      this.checkBoxColumnsResizable.AutoSize = true;
      this.checkBoxColumnsResizable.Dock = System.Windows.Forms.DockStyle.Fill;
      this.checkBoxColumnsResizable.Location = new System.Drawing.Point(0, 22);
      this.checkBoxColumnsResizable.Margin = new System.Windows.Forms.Padding(0);
      this.checkBoxColumnsResizable.Name = "checkBoxColumnsResizable";
      this.checkBoxColumnsResizable.Size = new System.Drawing.Size(520, 22);
      this.checkBoxColumnsResizable.TabIndex = 7;
      this.checkBoxColumnsResizable.Text = "Columns Resizable";
      this.checkBoxColumnsResizable.UseVisualStyleBackColor = true;
      // 
      // checkBoxRowsResizable
      // 
      this.checkBoxRowsResizable.AutoSize = true;
      this.checkBoxRowsResizable.Dock = System.Windows.Forms.DockStyle.Fill;
      this.checkBoxRowsResizable.Location = new System.Drawing.Point(0, 0);
      this.checkBoxRowsResizable.Margin = new System.Windows.Forms.Padding(0);
      this.checkBoxRowsResizable.Name = "checkBoxRowsResizable";
      this.checkBoxRowsResizable.Size = new System.Drawing.Size(520, 22);
      this.checkBoxRowsResizable.TabIndex = 4;
      this.checkBoxRowsResizable.Text = "Rows Resizable";
      this.checkBoxRowsResizable.UseVisualStyleBackColor = true;
      // 
      // MatrixJsonItemControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "MatrixJsonItemControl";
      this.Size = new System.Drawing.Size(526, 63);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.groupBox2.ResumeLayout(false);
      this.tableLayoutPanel2.ResumeLayout(false);
      this.tableLayoutPanel2.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.CheckBox checkBoxRowsResizable;
    private System.Windows.Forms.CheckBox checkBoxColumnsResizable;
  }
}
