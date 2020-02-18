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
      this.labelEnable = new System.Windows.Forms.Label();
      this.checkBoxActive = new System.Windows.Forms.CheckBox();
      this.textBoxActualName = new System.Windows.Forms.TextBox();
      this.labelActualName = new System.Windows.Forms.Label();
      this.textBoxName = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.labelDescription = new System.Windows.Forms.Label();
      this.textBoxDescription = new System.Windows.Forms.TextBox();
      this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // labelEnable
      // 
      this.labelEnable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.labelEnable.AutoSize = true;
      this.labelEnable.Location = new System.Drawing.Point(6, 3);
      this.labelEnable.Name = "labelEnable";
      this.labelEnable.Size = new System.Drawing.Size(40, 13);
      this.labelEnable.TabIndex = 3;
      this.labelEnable.Text = "Enable";
      // 
      // checkBoxActive
      // 
      this.checkBoxActive.AutoSize = true;
      this.checkBoxActive.Location = new System.Drawing.Point(92, 3);
      this.checkBoxActive.Name = "checkBoxActive";
      this.checkBoxActive.Size = new System.Drawing.Size(15, 14);
      this.checkBoxActive.TabIndex = 2;
      this.checkBoxActive.UseVisualStyleBackColor = true;
      // 
      // textBoxActualName
      // 
      this.textBoxActualName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.textBoxActualName.Location = new System.Drawing.Point(92, 75);
      this.textBoxActualName.Name = "textBoxActualName";
      this.textBoxActualName.Size = new System.Drawing.Size(402, 20);
      this.textBoxActualName.TabIndex = 12;
      // 
      // labelActualName
      // 
      this.labelActualName.AutoSize = true;
      this.labelActualName.Location = new System.Drawing.Point(6, 78);
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
      this.errorProvider.SetIconAlignment(this.textBoxName, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.textBoxName.Location = new System.Drawing.Point(92, 23);
      this.textBoxName.Name = "textBoxName";
      this.textBoxName.Size = new System.Drawing.Size(402, 20);
      this.textBoxName.TabIndex = 10;
      this.textBoxName.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxName_Validating);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(6, 26);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(35, 13);
      this.label1.TabIndex = 9;
      this.label1.Text = "Name";
      this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // labelDescription
      // 
      this.labelDescription.AutoSize = true;
      this.labelDescription.Location = new System.Drawing.Point(6, 52);
      this.labelDescription.Name = "labelDescription";
      this.labelDescription.Size = new System.Drawing.Size(60, 13);
      this.labelDescription.TabIndex = 13;
      this.labelDescription.Text = "Description";
      // 
      // textBoxDescription
      // 
      this.textBoxDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.textBoxDescription.Location = new System.Drawing.Point(92, 49);
      this.textBoxDescription.Name = "textBoxDescription";
      this.textBoxDescription.Size = new System.Drawing.Size(402, 20);
      this.textBoxDescription.TabIndex = 14;
      // 
      // errorProvider
      // 
      this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
      this.errorProvider.ContainerControl = this;
      // 
      // JsonItemBaseControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.textBoxDescription);
      this.Controls.Add(this.labelDescription);
      this.Controls.Add(this.labelEnable);
      this.Controls.Add(this.checkBoxActive);
      this.Controls.Add(this.textBoxActualName);
      this.Controls.Add(this.labelActualName);
      this.Controls.Add(this.textBoxName);
      this.Controls.Add(this.label1);
      this.errorProvider.SetIconAlignment(this, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.Name = "JsonItemBaseControl";
      this.Padding = new System.Windows.Forms.Padding(3);
      this.Size = new System.Drawing.Size(500, 154);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label labelEnable;
    private System.Windows.Forms.CheckBox checkBoxActive;
    private System.Windows.Forms.TextBox textBoxActualName;
    private System.Windows.Forms.Label labelActualName;
    private System.Windows.Forms.TextBox textBoxName;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label labelDescription;
    private System.Windows.Forms.TextBox textBoxDescription;
    protected System.Windows.Forms.ErrorProvider errorProvider;
  }
}
