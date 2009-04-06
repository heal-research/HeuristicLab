namespace HeuristicLab.StatisticalAnalysis {
  partial class MannWhitneyWilcoxonTestControl {
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
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.p1TextBox = new System.Windows.Forms.TextBox();
      this.p2TextBox = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.resultLabel = new System.Windows.Forms.Label();
      this.testExactButton = new System.Windows.Forms.Button();
      this.testApproximateButton = new System.Windows.Forms.Button();
      this.panel1 = new System.Windows.Forms.Panel();
      this.panel2 = new System.Windows.Forms.Panel();
      this.alphaTextBox = new System.Windows.Forms.TextBox();
      this.panel1.SuspendLayout();
      this.panel2.SuspendLayout();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(11, 25);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(54, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Sample 1:";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(11, 51);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(54, 13);
      this.label2.TabIndex = 1;
      this.label2.Text = "Sample 2:";
      // 
      // p1TextBox
      // 
      this.p1TextBox.Location = new System.Drawing.Point(71, 22);
      this.p1TextBox.Name = "p1TextBox";
      this.p1TextBox.Size = new System.Drawing.Size(219, 20);
      this.p1TextBox.TabIndex = 2;
      // 
      // p2TextBox
      // 
      this.p2TextBox.Location = new System.Drawing.Point(71, 48);
      this.p2TextBox.Name = "p2TextBox";
      this.p2TextBox.Size = new System.Drawing.Size(219, 20);
      this.p2TextBox.TabIndex = 3;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(28, 78);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(37, 13);
      this.label3.TabIndex = 4;
      this.label3.Text = "Alpha:";
      // 
      // resultLabel
      // 
      this.resultLabel.AutoSize = true;
      this.resultLabel.Location = new System.Drawing.Point(4, 4);
      this.resultLabel.MaximumSize = new System.Drawing.Size(265, 80);
      this.resultLabel.Name = "resultLabel";
      this.resultLabel.Size = new System.Drawing.Size(40, 13);
      this.resultLabel.TabIndex = 5;
      this.resultLabel.Text = "Result:";
      // 
      // testExactButton
      // 
      this.testExactButton.Location = new System.Drawing.Point(215, 103);
      this.testExactButton.Name = "testExactButton";
      this.testExactButton.Size = new System.Drawing.Size(75, 23);
      this.testExactButton.TabIndex = 8;
      this.testExactButton.Text = "Exact";
      this.testExactButton.UseVisualStyleBackColor = true;
      this.testExactButton.Click += new System.EventHandler(this.testExactButton_Click);
      // 
      // testApproximateButton
      // 
      this.testApproximateButton.Location = new System.Drawing.Point(215, 74);
      this.testApproximateButton.Name = "testApproximateButton";
      this.testApproximateButton.Size = new System.Drawing.Size(75, 23);
      this.testApproximateButton.TabIndex = 9;
      this.testApproximateButton.Text = "Approximate";
      this.testApproximateButton.UseVisualStyleBackColor = true;
      this.testApproximateButton.Click += new System.EventHandler(this.testApproximateButton_Click);
      // 
      // panel1
      // 
      this.panel1.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
      this.panel1.Controls.Add(this.alphaTextBox);
      this.panel1.Controls.Add(this.panel2);
      this.panel1.Controls.Add(this.testExactButton);
      this.panel1.Controls.Add(this.testApproximateButton);
      this.panel1.Controls.Add(this.label1);
      this.panel1.Controls.Add(this.label2);
      this.panel1.Controls.Add(this.p1TextBox);
      this.panel1.Controls.Add(this.p2TextBox);
      this.panel1.Controls.Add(this.label3);
      this.panel1.Location = new System.Drawing.Point(3, 3);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(309, 248);
      this.panel1.TabIndex = 10;
      // 
      // panel2
      // 
      this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.panel2.Controls.Add(this.resultLabel);
      this.panel2.Location = new System.Drawing.Point(14, 132);
      this.panel2.Name = "panel2";
      this.panel2.Size = new System.Drawing.Size(276, 90);
      this.panel2.TabIndex = 10;
      // 
      // alphaTextBox
      // 
      this.alphaTextBox.Location = new System.Drawing.Point(71, 75);
      this.alphaTextBox.Name = "alphaTextBox";
      this.alphaTextBox.Size = new System.Drawing.Size(84, 20);
      this.alphaTextBox.TabIndex = 11;
      // 
      // MannWhitneyWilcoxonTestControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.panel1);
      this.Name = "MannWhitneyWilcoxonTestControl";
      this.Size = new System.Drawing.Size(316, 255);
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.panel2.ResumeLayout(false);
      this.panel2.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox p1TextBox;
    private System.Windows.Forms.TextBox p2TextBox;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label resultLabel;
    private System.Windows.Forms.Button testExactButton;
    private System.Windows.Forms.Button testApproximateButton;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Panel panel2;
    private System.Windows.Forms.TextBox alphaTextBox;
  }
}
