namespace HeuristicLab.VS2010Wizards {
  partial class ProblemWizardForm {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProblemWizardForm));
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.problemDescriptionTextBox = new System.Windows.Forms.TextBox();
      this.multiObjectiveCheckBox = new System.Windows.Forms.CheckBox();
      this.problemNameTextBox = new System.Windows.Forms.TextBox();
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
      this.solutionCreatorTypeTextBox = new System.Windows.Forms.TextBox();
      this.evaluatorTypeTextBox = new System.Windows.Forms.TextBox();
      this.label6 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.singleObjectiveCheckBox = new System.Windows.Forms.CheckBox();
      this.contentPanel = new System.Windows.Forms.Panel();
      this.panel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.page2Panel.SuspendLayout();
      this.page1Panel.SuspendLayout();
      this.contentPanel.SuspendLayout();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(100, 19);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(38, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Name:";
      // 
      // label2
      // 
      this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(100, 67);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(63, 13);
      this.label2.TabIndex = 2;
      this.label2.Text = "Description:";
      // 
      // problemDescriptionTextBox
      // 
      this.problemDescriptionTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.problemDescriptionTextBox.Location = new System.Drawing.Point(103, 83);
      this.problemDescriptionTextBox.Multiline = true;
      this.problemDescriptionTextBox.Name = "problemDescriptionTextBox";
      this.problemDescriptionTextBox.Size = new System.Drawing.Size(468, 63);
      this.problemDescriptionTextBox.TabIndex = 3;
      // 
      // multiObjectiveCheckBox
      // 
      this.multiObjectiveCheckBox.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.multiObjectiveCheckBox.AutoSize = true;
      this.multiObjectiveCheckBox.Location = new System.Drawing.Point(212, 152);
      this.multiObjectiveCheckBox.Name = "multiObjectiveCheckBox";
      this.multiObjectiveCheckBox.Size = new System.Drawing.Size(96, 17);
      this.multiObjectiveCheckBox.TabIndex = 5;
      this.multiObjectiveCheckBox.Text = "Multi-Objective";
      this.multiObjectiveCheckBox.UseVisualStyleBackColor = true;
      this.multiObjectiveCheckBox.CheckedChanged += new System.EventHandler(this.multiObjectiveCheckBox_CheckedChanged);
      // 
      // problemNameTextBox
      // 
      this.problemNameTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.problemNameTextBox.Location = new System.Drawing.Point(103, 35);
      this.problemNameTextBox.Name = "problemNameTextBox";
      this.problemNameTextBox.Size = new System.Drawing.Size(468, 20);
      this.problemNameTextBox.TabIndex = 1;
      this.problemNameTextBox.Text = "MyProblem";
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
      this.label4.Size = new System.Drawing.Size(291, 39);
      this.label4.TabIndex = 1;
      this.label4.Text = "New Problem Wizard";
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
      this.page2Panel.Controls.Add(this.label3);
      this.page2Panel.Controls.Add(this.parametersControl);
      this.page2Panel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.page2Panel.Location = new System.Drawing.Point(0, 0);
      this.page2Panel.Name = "page2Panel";
      this.page2Panel.Size = new System.Drawing.Size(676, 293);
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
      this.parametersControl.Size = new System.Drawing.Size(676, 260);
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
      this.page1Panel.Controls.Add(this.solutionCreatorTypeTextBox);
      this.page1Panel.Controls.Add(this.evaluatorTypeTextBox);
      this.page1Panel.Controls.Add(this.label6);
      this.page1Panel.Controls.Add(this.label5);
      this.page1Panel.Controls.Add(this.label1);
      this.page1Panel.Controls.Add(this.label2);
      this.page1Panel.Controls.Add(this.problemDescriptionTextBox);
      this.page1Panel.Controls.Add(this.problemNameTextBox);
      this.page1Panel.Controls.Add(this.singleObjectiveCheckBox);
      this.page1Panel.Controls.Add(this.multiObjectiveCheckBox);
      this.page1Panel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.page1Panel.Location = new System.Drawing.Point(0, 0);
      this.page1Panel.Name = "page1Panel";
      this.page1Panel.Size = new System.Drawing.Size(676, 293);
      this.page1Panel.TabIndex = 9;
      // 
      // solutionCreatorTypeTextBox
      // 
      this.solutionCreatorTypeTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.solutionCreatorTypeTextBox.Location = new System.Drawing.Point(103, 236);
      this.solutionCreatorTypeTextBox.Name = "solutionCreatorTypeTextBox";
      this.solutionCreatorTypeTextBox.Size = new System.Drawing.Size(468, 20);
      this.solutionCreatorTypeTextBox.TabIndex = 8;
      this.solutionCreatorTypeTextBox.Text = "IMyProblemSolutionCreator";
      this.solutionCreatorTypeTextBox.TextChanged += new System.EventHandler(this.solutionCreatorTypeTextBox_TextChanged);
      // 
      // evaluatorTypeTextBox
      // 
      this.evaluatorTypeTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.evaluatorTypeTextBox.Location = new System.Drawing.Point(103, 197);
      this.evaluatorTypeTextBox.Name = "evaluatorTypeTextBox";
      this.evaluatorTypeTextBox.Size = new System.Drawing.Size(468, 20);
      this.evaluatorTypeTextBox.TabIndex = 9;
      this.evaluatorTypeTextBox.Text = "IMyProblemEvaluator";
      this.evaluatorTypeTextBox.TextChanged += new System.EventHandler(this.evaluatorTypeTextBox_TextChanged);
      // 
      // label6
      // 
      this.label6.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(100, 220);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(112, 13);
      this.label6.TabIndex = 6;
      this.label6.Text = "Solution Creator Type:";
      // 
      // label5
      // 
      this.label5.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(100, 181);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(82, 13);
      this.label5.TabIndex = 7;
      this.label5.Text = "Evaluator Type:";
      // 
      // singleObjectiveCheckBox
      // 
      this.singleObjectiveCheckBox.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.singleObjectiveCheckBox.AutoSize = true;
      this.singleObjectiveCheckBox.Checked = true;
      this.singleObjectiveCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
      this.singleObjectiveCheckBox.Location = new System.Drawing.Point(103, 152);
      this.singleObjectiveCheckBox.Name = "singleObjectiveCheckBox";
      this.singleObjectiveCheckBox.Size = new System.Drawing.Size(103, 17);
      this.singleObjectiveCheckBox.TabIndex = 5;
      this.singleObjectiveCheckBox.Text = "Single-Objective";
      this.singleObjectiveCheckBox.UseVisualStyleBackColor = true;
      this.singleObjectiveCheckBox.CheckedChanged += new System.EventHandler(this.singleObjectiveCheckBox_CheckedChanged);
      // 
      // contentPanel
      // 
      this.contentPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.contentPanel.Controls.Add(this.page1Panel);
      this.contentPanel.Controls.Add(this.page2Panel);
      this.contentPanel.Location = new System.Drawing.Point(0, 85);
      this.contentPanel.Name = "contentPanel";
      this.contentPanel.Size = new System.Drawing.Size(676, 293);
      this.contentPanel.TabIndex = 11;
      // 
      // ProblemWizardForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(676, 428);
      this.Controls.Add(this.contentPanel);
      this.Controls.Add(this.panel1);
      this.Controls.Add(this.cancelButton);
      this.Controls.Add(this.previousButton);
      this.Controls.Add(this.nextButton);
      this.Controls.Add(this.finishButton);
      this.Controls.Add(this.panel2);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "ProblemWizardForm";
      this.Text = "New Problem Wizard";
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.page2Panel.ResumeLayout(false);
      this.page2Panel.PerformLayout();
      this.page1Panel.ResumeLayout(false);
      this.page1Panel.PerformLayout();
      this.contentPanel.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox problemDescriptionTextBox;
    private System.Windows.Forms.CheckBox multiObjectiveCheckBox;
    private System.Windows.Forms.TextBox problemNameTextBox;
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
    private System.Windows.Forms.CheckBox singleObjectiveCheckBox;
    private System.Windows.Forms.Panel contentPanel;
    private System.Windows.Forms.TextBox solutionCreatorTypeTextBox;
    private System.Windows.Forms.TextBox evaluatorTypeTextBox;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Label label5;
  }
}