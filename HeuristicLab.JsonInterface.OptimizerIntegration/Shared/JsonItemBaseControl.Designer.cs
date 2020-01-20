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
      this.labelPath = new System.Windows.Forms.Label();
      this.checkBoxActive = new System.Windows.Forms.CheckBox();
      this.panel1 = new System.Windows.Forms.Panel();
      this.label3 = new System.Windows.Forms.Label();
      this.textBoxActualName = new System.Windows.Forms.TextBox();
      this.labelActualName = new System.Windows.Forms.Label();
      this.textBoxName = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // labelPath
      // 
      this.labelPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.labelPath.AutoSize = true;
      this.labelPath.Location = new System.Drawing.Point(24, 4);
      this.labelPath.Name = "labelPath";
      this.labelPath.Size = new System.Drawing.Size(29, 13);
      this.labelPath.TabIndex = 3;
      this.labelPath.Text = "Path";
      // 
      // checkBoxActive
      // 
      this.checkBoxActive.AutoSize = true;
      this.checkBoxActive.Location = new System.Drawing.Point(3, 3);
      this.checkBoxActive.Name = "checkBoxActive";
      this.checkBoxActive.Size = new System.Drawing.Size(15, 14);
      this.checkBoxActive.TabIndex = 2;
      this.checkBoxActive.UseVisualStyleBackColor = true;
      this.checkBoxActive.CheckedChanged += new System.EventHandler(this.checkBoxActive_CheckedChanged);
      // 
      // panel1
      // 
      this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.panel1.BackColor = System.Drawing.Color.Gray;
      this.panel1.Controls.Add(this.checkBoxActive);
      this.panel1.Controls.Add(this.labelPath);
      this.panel1.Location = new System.Drawing.Point(0, 0);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(501, 20);
      this.panel1.TabIndex = 4;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(6, 84);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(34, 13);
      this.label3.TabIndex = 13;
      this.label3.Text = "Value";
      this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // textBoxActualName
      // 
      this.textBoxActualName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.textBoxActualName.Location = new System.Drawing.Point(92, 55);
      this.textBoxActualName.Name = "textBoxActualName";
      this.textBoxActualName.Size = new System.Drawing.Size(402, 20);
      this.textBoxActualName.TabIndex = 12;
      this.textBoxActualName.TextChanged += new System.EventHandler(this.textBoxActualName_TextChanged);
      // 
      // labelActualName
      // 
      this.labelActualName.AutoSize = true;
      this.labelActualName.Location = new System.Drawing.Point(6, 58);
      this.labelActualName.Name = "labelActualName";
      this.labelActualName.Size = new System.Drawing.Size(65, 13);
      this.labelActualName.TabIndex = 11;
      this.labelActualName.Text = "ActualName";
      this.labelActualName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // textBoxName
      // 
      this.textBoxName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.textBoxName.Location = new System.Drawing.Point(92, 29);
      this.textBoxName.Name = "textBoxName";
      this.textBoxName.Size = new System.Drawing.Size(402, 20);
      this.textBoxName.TabIndex = 10;
      this.textBoxName.TextChanged += new System.EventHandler(this.textBoxName_TextChanged);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(6, 32);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(35, 13);
      this.label1.TabIndex = 9;
      this.label1.Text = "Name";
      this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // JsonItemBaseControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.Controls.Add(this.label3);
      this.Controls.Add(this.textBoxActualName);
      this.Controls.Add(this.labelActualName);
      this.Controls.Add(this.textBoxName);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.panel1);
      this.Name = "JsonItemBaseControl";
      this.Padding = new System.Windows.Forms.Padding(3);
      this.Size = new System.Drawing.Size(500, 152);
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label labelPath;
    private System.Windows.Forms.CheckBox checkBoxActive;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox textBoxActualName;
    private System.Windows.Forms.Label labelActualName;
    private System.Windows.Forms.TextBox textBoxName;
    private System.Windows.Forms.Label label1;
  }
}
