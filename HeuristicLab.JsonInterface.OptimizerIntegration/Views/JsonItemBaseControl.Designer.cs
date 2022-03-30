namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  partial class JsonItemBaseControl {
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
      this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
      this.textBoxName = new System.Windows.Forms.TextBox();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
      this.textBoxDescription = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.labelDescription = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.tableLayoutPanel1.SuspendLayout();
      this.tableLayoutPanel5.SuspendLayout();
      this.SuspendLayout();
      // 
      // errorProvider
      // 
      this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
      this.errorProvider.ContainerControl = this;
      // 
      // textBoxName
      // 
      this.textBoxName.Dock = System.Windows.Forms.DockStyle.Fill;
      this.errorProvider.SetIconAlignment(this.textBoxName, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.textBoxName.Location = new System.Drawing.Point(100, 0);
      this.textBoxName.Margin = new System.Windows.Forms.Padding(0);
      this.textBoxName.Name = "textBoxName";
      this.textBoxName.Size = new System.Drawing.Size(394, 20);
      this.textBoxName.TabIndex = 10;
      this.textBoxName.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxName_Validating);
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 1;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel5, 0, 0);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
      this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 3;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 6F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(494, 594);
      this.tableLayoutPanel1.TabIndex = 16;
      // 
      // tableLayoutPanel5
      // 
      this.tableLayoutPanel5.ColumnCount = 2;
      this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
      this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel5.Controls.Add(this.textBoxDescription, 1, 1);
      this.tableLayoutPanel5.Controls.Add(this.textBoxName, 1, 0);
      this.tableLayoutPanel5.Controls.Add(this.label1, 0, 0);
      this.tableLayoutPanel5.Controls.Add(this.labelDescription, 0, 1);
      this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel5.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel5.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel5.Name = "tableLayoutPanel5";
      this.tableLayoutPanel5.RowCount = 1;
      this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel5.Size = new System.Drawing.Size(494, 44);
      this.tableLayoutPanel5.TabIndex = 17;
      // 
      // textBoxDescription
      // 
      this.textBoxDescription.Dock = System.Windows.Forms.DockStyle.Fill;
      this.textBoxDescription.Location = new System.Drawing.Point(100, 22);
      this.textBoxDescription.Margin = new System.Windows.Forms.Padding(0);
      this.textBoxDescription.Name = "textBoxDescription";
      this.textBoxDescription.Size = new System.Drawing.Size(394, 20);
      this.textBoxDescription.TabIndex = 14;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.label1.Location = new System.Drawing.Point(0, 0);
      this.label1.Margin = new System.Windows.Forms.Padding(0);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(100, 22);
      this.label1.TabIndex = 9;
      this.label1.Text = "Name";
      this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // labelDescription
      // 
      this.labelDescription.AutoSize = true;
      this.labelDescription.Dock = System.Windows.Forms.DockStyle.Fill;
      this.labelDescription.Location = new System.Drawing.Point(0, 22);
      this.labelDescription.Margin = new System.Windows.Forms.Padding(0);
      this.labelDescription.Name = "labelDescription";
      this.labelDescription.Size = new System.Drawing.Size(100, 22);
      this.labelDescription.TabIndex = 13;
      this.labelDescription.Text = "Description";
      this.labelDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // JsonItemBaseControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel1);
      this.errorProvider.SetIconAlignment(this, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.Name = "JsonItemBaseControl";
      this.Padding = new System.Windows.Forms.Padding(3);
      this.Size = new System.Drawing.Size(500, 600);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel5.ResumeLayout(false);
      this.tableLayoutPanel5.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion
    protected System.Windows.Forms.ErrorProvider errorProvider;
    protected System.Windows.Forms.TextBox textBoxName;
    protected System.Windows.Forms.Label label1;
    protected System.Windows.Forms.Label labelDescription;
    protected System.Windows.Forms.TextBox textBoxDescription;
    protected System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
  }
}
