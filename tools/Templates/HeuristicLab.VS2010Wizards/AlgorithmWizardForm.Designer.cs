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
      this.algorithmDescriptionTextBox = new System.Windows.Forms.TextBox();
      this.isMultiObjectiveCheckBox = new System.Windows.Forms.CheckBox();
      this.algorithmNameTextBox = new System.Windows.Forms.TextBox();
      this.cancelButton = new System.Windows.Forms.Button();
      this.finishButton = new System.Windows.Forms.Button();
      this.panel1 = new System.Windows.Forms.Panel();
      this.label4 = new System.Windows.Forms.Label();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.page2Panel = new System.Windows.Forms.Panel();
      this.label3 = new System.Windows.Forms.Label();
      this.parametersControl = new HeuristicLab.VS2010Wizards.ParametersControl();
      this.panel2 = new System.Windows.Forms.Panel();
      this.nextButton = new System.Windows.Forms.Button();
      this.previousButton = new System.Windows.Forms.Button();
      this.page1Panel = new System.Windows.Forms.Panel();
      this.panel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.page2Panel.SuspendLayout();
      this.page1Panel.SuspendLayout();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(100, 48);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(38, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Name:";
      // 
      // label2
      // 
      this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(100, 96);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(63, 13);
      this.label2.TabIndex = 2;
      this.label2.Text = "Description:";
      // 
      // algorithmDescriptionTextBox
      // 
      this.algorithmDescriptionTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.algorithmDescriptionTextBox.Location = new System.Drawing.Point(102, 112);
      this.algorithmDescriptionTextBox.Multiline = true;
      this.algorithmDescriptionTextBox.Name = "algorithmDescriptionTextBox";
      this.algorithmDescriptionTextBox.Size = new System.Drawing.Size(469, 63);
      this.algorithmDescriptionTextBox.TabIndex = 3;
      // 
      // isMultiObjectiveCheckBox
      // 
      this.isMultiObjectiveCheckBox.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.isMultiObjectiveCheckBox.AutoSize = true;
      this.isMultiObjectiveCheckBox.Location = new System.Drawing.Point(102, 188);
      this.isMultiObjectiveCheckBox.Name = "isMultiObjectiveCheckBox";
      this.isMultiObjectiveCheckBox.Size = new System.Drawing.Size(96, 17);
      this.isMultiObjectiveCheckBox.TabIndex = 5;
      this.isMultiObjectiveCheckBox.Text = "Multi-Objective";
      this.isMultiObjectiveCheckBox.UseVisualStyleBackColor = true;
      // 
      // algorithmNameTextBox
      // 
      this.algorithmNameTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.algorithmNameTextBox.Location = new System.Drawing.Point(103, 64);
      this.algorithmNameTextBox.Name = "algorithmNameTextBox";
      this.algorithmNameTextBox.Size = new System.Drawing.Size(468, 20);
      this.algorithmNameTextBox.TabIndex = 1;
      this.algorithmNameTextBox.Text = "MyAlgorithm";
      // 
      // cancelButton
      // 
      this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.cancelButton.Location = new System.Drawing.Point(589, 391);
      this.cancelButton.Name = "cancelButton";
      this.cancelButton.Size = new System.Drawing.Size(75, 25);
      this.cancelButton.TabIndex = 7;
      this.cancelButton.Text = "Cancel";
      this.cancelButton.UseVisualStyleBackColor = true;
      this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
      // 
      // finishButton
      // 
      this.finishButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.finishButton.Location = new System.Drawing.Point(504, 391);
      this.finishButton.Name = "finishButton";
      this.finishButton.Size = new System.Drawing.Size(75, 25);
      this.finishButton.TabIndex = 6;
      this.finishButton.Text = "Finish";
      this.finishButton.UseVisualStyleBackColor = true;
      this.finishButton.Click += new System.EventHandler(this.finishButton_Click);
      // 
      // panel1
      // 
      this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.panel1.BackColor = System.Drawing.Color.White;
      this.panel1.Controls.Add(this.label4);
      this.panel1.Controls.Add(this.pictureBox1);
      this.panel1.Location = new System.Drawing.Point(0, 0);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(676, 81);
      this.panel1.TabIndex = 8;
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Font = new System.Drawing.Font("Calibri", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label4.Location = new System.Drawing.Point(96, 21);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(311, 39);
      this.label4.TabIndex = 1;
      this.label4.Text = "New Algorithm Wizard";
      // 
      // pictureBox1
      // 
      this.pictureBox1.Image = global::HeuristicLab.VS2010Wizards.Properties.Resources.HL3_Logo;
      this.pictureBox1.Location = new System.Drawing.Point(3, 3);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(75, 75);
      this.pictureBox1.TabIndex = 0;
      this.pictureBox1.TabStop = false;
      // 
      // page2Panel
      // 
      this.page2Panel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.page2Panel.Controls.Add(this.label3);
      this.page2Panel.Controls.Add(this.parametersControl);
      this.page2Panel.Location = new System.Drawing.Point(0, 86);
      this.page2Panel.Name = "page2Panel";
      this.page2Panel.Size = new System.Drawing.Size(676, 289);
      this.page2Panel.TabIndex = 10;
      this.page2Panel.Visible = false;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(13, 10);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(63, 13);
      this.label3.TabIndex = 1;
      this.label3.Text = "Parameters:";
      // 
      // parametersControl
      // 
      this.parametersControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.parametersControl.Location = new System.Drawing.Point(0, 33);
      this.parametersControl.Name = "parametersControl";
      this.parametersControl.Size = new System.Drawing.Size(676, 256);
      this.parametersControl.TabIndex = 0;
      // 
      // panel2
      // 
      this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.panel2.BackColor = System.Drawing.Color.White;
      this.panel2.ForeColor = System.Drawing.SystemColors.ControlText;
      this.panel2.Location = new System.Drawing.Point(0, 379);
      this.panel2.Name = "panel2";
      this.panel2.Size = new System.Drawing.Size(676, 2);
      this.panel2.TabIndex = 8;
      // 
      // nextButton
      // 
      this.nextButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.nextButton.Location = new System.Drawing.Point(406, 391);
      this.nextButton.Name = "nextButton";
      this.nextButton.Size = new System.Drawing.Size(75, 25);
      this.nextButton.TabIndex = 6;
      this.nextButton.Text = "Next >";
      this.nextButton.UseVisualStyleBackColor = true;
      this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
      // 
      // previousButton
      // 
      this.previousButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.previousButton.Enabled = false;
      this.previousButton.Location = new System.Drawing.Point(321, 391);
      this.previousButton.Name = "previousButton";
      this.previousButton.Size = new System.Drawing.Size(75, 25);
      this.previousButton.TabIndex = 6;
      this.previousButton.Text = "< Previous";
      this.previousButton.UseVisualStyleBackColor = true;
      this.previousButton.Click += new System.EventHandler(this.previousButton_Click);
      // 
      // page1Panel
      // 
      this.page1Panel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.page1Panel.Controls.Add(this.label1);
      this.page1Panel.Controls.Add(this.label2);
      this.page1Panel.Controls.Add(this.algorithmDescriptionTextBox);
      this.page1Panel.Controls.Add(this.algorithmNameTextBox);
      this.page1Panel.Controls.Add(this.isMultiObjectiveCheckBox);
      this.page1Panel.Location = new System.Drawing.Point(0, 86);
      this.page1Panel.Name = "page1Panel";
      this.page1Panel.Size = new System.Drawing.Size(676, 298);
      this.page1Panel.TabIndex = 9;
      // 
      // AlgorithmWizardForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(676, 428);
      this.Controls.Add(this.panel1);
      this.Controls.Add(this.cancelButton);
      this.Controls.Add(this.previousButton);
      this.Controls.Add(this.nextButton);
      this.Controls.Add(this.finishButton);
      this.Controls.Add(this.panel2);
      this.Controls.Add(this.page2Panel);
      this.Controls.Add(this.page1Panel);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "AlgorithmWizardForm";
      this.Text = "New Algorithm Wizard";
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.page2Panel.ResumeLayout(false);
      this.page2Panel.PerformLayout();
      this.page1Panel.ResumeLayout(false);
      this.page1Panel.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox algorithmDescriptionTextBox;
    private System.Windows.Forms.CheckBox isMultiObjectiveCheckBox;
    private System.Windows.Forms.TextBox algorithmNameTextBox;
    private System.Windows.Forms.Button cancelButton;
    private System.Windows.Forms.Button finishButton;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.Panel panel2;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Button nextButton;
    private System.Windows.Forms.Button previousButton;
    private System.Windows.Forms.Panel page1Panel;
    private System.Windows.Forms.Panel page2Panel;
    private ParametersControl parametersControl;
    private System.Windows.Forms.Label label3;
  }
}