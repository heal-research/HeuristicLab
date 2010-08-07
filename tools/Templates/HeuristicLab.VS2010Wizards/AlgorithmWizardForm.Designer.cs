namespace HeuristicLab.VS2010Wizards {
  partial class AlgorithmWizardForm {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AlgorithmWizardForm));
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.algorithmDescriptionTextBox = new System.Windows.Forms.TextBox();
      this.isMultiObjectiveCheckBox = new System.Windows.Forms.CheckBox();
      this.algorithmNameTextBox = new System.Windows.Forms.TextBox();
      this.cancelButton = new System.Windows.Forms.Button();
      this.okButton = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 12);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(38, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Name:";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(12, 38);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(63, 13);
      this.label2.TabIndex = 2;
      this.label2.Text = "Description:";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(12, 180);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(80, 13);
      this.label3.TabIndex = 4;
      this.label3.Text = "Multi-Objective:";
      // 
      // algorithmDescriptionTextBox
      // 
      this.algorithmDescriptionTextBox.Location = new System.Drawing.Point(96, 35);
      this.algorithmDescriptionTextBox.Multiline = true;
      this.algorithmDescriptionTextBox.Name = "algorithmDescriptionTextBox";
      this.algorithmDescriptionTextBox.Size = new System.Drawing.Size(277, 139);
      this.algorithmDescriptionTextBox.TabIndex = 3;
      this.algorithmDescriptionTextBox.Text = "Description";
      this.algorithmDescriptionTextBox.TextChanged += new System.EventHandler(this.algorithmDescriptionTextBox_TextChanged);
      // 
      // isMultiObjectiveCheckBox
      // 
      this.isMultiObjectiveCheckBox.AutoSize = true;
      this.isMultiObjectiveCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.isMultiObjectiveCheckBox.Location = new System.Drawing.Point(96, 180);
      this.isMultiObjectiveCheckBox.Name = "isMultiObjectiveCheckBox";
      this.isMultiObjectiveCheckBox.Size = new System.Drawing.Size(15, 14);
      this.isMultiObjectiveCheckBox.TabIndex = 5;
      this.isMultiObjectiveCheckBox.UseVisualStyleBackColor = true;
      this.isMultiObjectiveCheckBox.CheckedChanged += new System.EventHandler(this.isMultiObjectiveCheckBox_CheckedChanged);
      // 
      // algorithmNameTextBox
      // 
      this.algorithmNameTextBox.Location = new System.Drawing.Point(97, 9);
      this.algorithmNameTextBox.Name = "algorithmNameTextBox";
      this.algorithmNameTextBox.Size = new System.Drawing.Size(276, 20);
      this.algorithmNameTextBox.TabIndex = 1;
      this.algorithmNameTextBox.Text = "MyAlgorithm";
      this.algorithmNameTextBox.TextChanged += new System.EventHandler(this.algorithmNameTextBox_TextChanged);
      // 
      // cancelButton
      // 
      this.cancelButton.Location = new System.Drawing.Point(97, 208);
      this.cancelButton.Name = "cancelButton";
      this.cancelButton.Size = new System.Drawing.Size(75, 23);
      this.cancelButton.TabIndex = 7;
      this.cancelButton.Text = "Cancel";
      this.cancelButton.UseVisualStyleBackColor = true;
      this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
      // 
      // okButton
      // 
      this.okButton.Location = new System.Drawing.Point(15, 208);
      this.okButton.Name = "okButton";
      this.okButton.Size = new System.Drawing.Size(75, 23);
      this.okButton.TabIndex = 6;
      this.okButton.Text = "Ok";
      this.okButton.UseVisualStyleBackColor = true;
      this.okButton.Click += new System.EventHandler(this.okButton_Click);
      // 
      // AlgorithmWizardForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(385, 242);
      this.Controls.Add(this.algorithmDescriptionTextBox);
      this.Controls.Add(this.isMultiObjectiveCheckBox);
      this.Controls.Add(this.algorithmNameTextBox);
      this.Controls.Add(this.cancelButton);
      this.Controls.Add(this.okButton);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "AlgorithmWizardForm";
      this.Text = "New Algorithm Wizard";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox algorithmDescriptionTextBox;
    private System.Windows.Forms.CheckBox isMultiObjectiveCheckBox;
    private System.Windows.Forms.TextBox algorithmNameTextBox;
    private System.Windows.Forms.Button cancelButton;
    private System.Windows.Forms.Button okButton;
  }
}