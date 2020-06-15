namespace HeuristicLab.JsonInterface.App {
  partial class JsonInterfaceForm {
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

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.templateTextBox = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.templateOpenButton = new System.Windows.Forms.Button();
      this.configOpenButton = new System.Windows.Forms.Button();
      this.configTextBox = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.outputOpenButton = new System.Windows.Forms.Button();
      this.outputTextBox = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.runButton = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // templateTextBox
      // 
      this.templateTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.templateTextBox.Location = new System.Drawing.Point(69, 12);
      this.templateTextBox.Name = "templateTextBox";
      this.templateTextBox.ReadOnly = true;
      this.templateTextBox.Size = new System.Drawing.Size(303, 20);
      this.templateTextBox.TabIndex = 14;
      this.templateTextBox.Text = "Template";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 15);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(51, 13);
      this.label1.TabIndex = 13;
      this.label1.Text = "Template";
      // 
      // templateOpenButton
      // 
      this.templateOpenButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.templateOpenButton.Location = new System.Drawing.Point(378, 10);
      this.templateOpenButton.Name = "templateOpenButton";
      this.templateOpenButton.Size = new System.Drawing.Size(44, 23);
      this.templateOpenButton.TabIndex = 15;
      this.templateOpenButton.UseVisualStyleBackColor = true;
      this.templateOpenButton.Click += new System.EventHandler(this.OpenTemplate);
      // 
      // configOpenButton
      // 
      this.configOpenButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.configOpenButton.Location = new System.Drawing.Point(378, 36);
      this.configOpenButton.Name = "configOpenButton";
      this.configOpenButton.Size = new System.Drawing.Size(44, 23);
      this.configOpenButton.TabIndex = 18;
      this.configOpenButton.UseVisualStyleBackColor = true;
      this.configOpenButton.Click += new System.EventHandler(this.OpenConfig);
      // 
      // configTextBox
      // 
      this.configTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.configTextBox.Location = new System.Drawing.Point(69, 38);
      this.configTextBox.Name = "configTextBox";
      this.configTextBox.ReadOnly = true;
      this.configTextBox.Size = new System.Drawing.Size(303, 20);
      this.configTextBox.TabIndex = 17;
      this.configTextBox.Text = "Config";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(12, 41);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(37, 13);
      this.label2.TabIndex = 16;
      this.label2.Text = "Config";
      // 
      // outputOpenButton
      // 
      this.outputOpenButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.outputOpenButton.Location = new System.Drawing.Point(378, 62);
      this.outputOpenButton.Name = "outputOpenButton";
      this.outputOpenButton.Size = new System.Drawing.Size(44, 23);
      this.outputOpenButton.TabIndex = 21;
      this.outputOpenButton.UseVisualStyleBackColor = true;
      this.outputOpenButton.Click += new System.EventHandler(this.OpenOutput);
      // 
      // outputTextBox
      // 
      this.outputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.outputTextBox.Location = new System.Drawing.Point(69, 64);
      this.outputTextBox.Name = "outputTextBox";
      this.outputTextBox.ReadOnly = true;
      this.outputTextBox.Size = new System.Drawing.Size(303, 20);
      this.outputTextBox.TabIndex = 20;
      this.outputTextBox.Text = "Output";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(12, 67);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(39, 13);
      this.label3.TabIndex = 19;
      this.label3.Text = "Output";
      // 
      // runButton
      // 
      this.runButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.runButton.Location = new System.Drawing.Point(12, 90);
      this.runButton.Name = "runButton";
      this.runButton.Size = new System.Drawing.Size(410, 23);
      this.runButton.TabIndex = 22;
      this.runButton.Text = "Run";
      this.runButton.UseVisualStyleBackColor = true;
      this.runButton.Click += new System.EventHandler(this.Run);
      // 
      // JsonInterfaceForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(434, 121);
      this.Controls.Add(this.runButton);
      this.Controls.Add(this.outputOpenButton);
      this.Controls.Add(this.outputTextBox);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.configOpenButton);
      this.Controls.Add(this.configTextBox);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.templateOpenButton);
      this.Controls.Add(this.templateTextBox);
      this.Controls.Add(this.label1);
      this.MinimumSize = new System.Drawing.Size(450, 160);
      this.Name = "JsonInterfaceForm";
      this.Text = "Json Interface";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox templateTextBox;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button templateOpenButton;
    private System.Windows.Forms.Button configOpenButton;
    private System.Windows.Forms.TextBox configTextBox;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Button outputOpenButton;
    private System.Windows.Forms.TextBox outputTextBox;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Button runButton;
  }
}