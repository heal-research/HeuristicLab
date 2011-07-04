namespace HeuristicLab.Problems.ExternalEvaluation.GP.Views {
  partial class VariableSymbolView {
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
      this.tabControl = new HeuristicLab.MainForm.WindowsForms.DragOverTabControl();
      this.variableNamesTabPage = new System.Windows.Forms.TabPage();
      this.parametersTabPage = new System.Windows.Forms.TabPage();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.weightManipulatorNuTextBox = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.weightManipulatorSigmaTextBox = new System.Windows.Forms.TextBox();
      this.label4 = new System.Windows.Forms.Label();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.weightNuTextBox = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.weightSigmaTextBox = new System.Windows.Forms.TextBox();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.tabControl.SuspendLayout();
      this.parametersTabPage.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.groupBox1.SuspendLayout();
      this.SuspendLayout();
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Size = new System.Drawing.Size(466, 20);
      // 
      // infoLabel
      // 
      this.infoLabel.Location = new System.Drawing.Point(530, 3);
      // 
      // tabControl
      // 
      this.tabControl.AllowDrop = true;
      this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl.Controls.Add(this.variableNamesTabPage);
      this.tabControl.Controls.Add(this.parametersTabPage);
      this.tabControl.Location = new System.Drawing.Point(0, 26);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(549, 393);
      this.tabControl.TabIndex = 3;
      // 
      // variableNamesTabPage
      // 
      this.variableNamesTabPage.Location = new System.Drawing.Point(4, 22);
      this.variableNamesTabPage.Name = "variableNamesTabPage";
      this.variableNamesTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.variableNamesTabPage.Size = new System.Drawing.Size(541, 367);
      this.variableNamesTabPage.TabIndex = 0;
      this.variableNamesTabPage.Text = "Variable Names";
      this.variableNamesTabPage.UseVisualStyleBackColor = true;
      // 
      // parametersTabPage
      // 
      this.parametersTabPage.Controls.Add(this.groupBox2);
      this.parametersTabPage.Controls.Add(this.groupBox1);
      this.parametersTabPage.Location = new System.Drawing.Point(4, 22);
      this.parametersTabPage.Name = "parametersTabPage";
      this.parametersTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.parametersTabPage.Size = new System.Drawing.Size(541, 367);
      this.parametersTabPage.TabIndex = 1;
      this.parametersTabPage.Text = "Parameters";
      this.parametersTabPage.UseVisualStyleBackColor = true;
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.weightManipulatorNuTextBox);
      this.groupBox2.Controls.Add(this.label3);
      this.groupBox2.Controls.Add(this.weightManipulatorSigmaTextBox);
      this.groupBox2.Controls.Add(this.label4);
      this.groupBox2.Location = new System.Drawing.Point(6, 100);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(200, 94);
      this.groupBox2.TabIndex = 1;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Weight Manipulator";
      // 
      // weightManipulatorNuTextBox
      // 
      this.weightManipulatorNuTextBox.Location = new System.Drawing.Point(61, 31);
      this.weightManipulatorNuTextBox.Name = "weightManipulatorNuTextBox";
      this.weightManipulatorNuTextBox.Size = new System.Drawing.Size(100, 20);
      this.weightManipulatorNuTextBox.TabIndex = 1;
      this.weightManipulatorNuTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.weightTextBox_Validating);
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(27, 34);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(16, 13);
      this.label3.TabIndex = 0;
      this.label3.Text = "µ:";
      // 
      // weightManipulatorSigmaTextBox
      // 
      this.weightManipulatorSigmaTextBox.Location = new System.Drawing.Point(61, 57);
      this.weightManipulatorSigmaTextBox.Name = "weightManipulatorSigmaTextBox";
      this.weightManipulatorSigmaTextBox.Size = new System.Drawing.Size(100, 20);
      this.weightManipulatorSigmaTextBox.TabIndex = 3;
      this.weightManipulatorSigmaTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.weightTextBox_Validating);
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(27, 60);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(17, 13);
      this.label4.TabIndex = 2;
      this.label4.Text = "σ:";
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.weightNuTextBox);
      this.groupBox1.Controls.Add(this.label1);
      this.groupBox1.Controls.Add(this.label2);
      this.groupBox1.Controls.Add(this.weightSigmaTextBox);
      this.groupBox1.Location = new System.Drawing.Point(6, 11);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(200, 83);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Weight";
      // 
      // weightNuTextBox
      // 
      this.weightNuTextBox.Location = new System.Drawing.Point(61, 19);
      this.weightNuTextBox.Name = "weightNuTextBox";
      this.weightNuTextBox.Size = new System.Drawing.Size(100, 20);
      this.weightNuTextBox.TabIndex = 1;
      this.weightNuTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.weightTextBox_Validating);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(27, 22);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(16, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "µ:";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(27, 48);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(17, 13);
      this.label2.TabIndex = 2;
      this.label2.Text = "σ:";
      // 
      // weightSigmaTextBox
      // 
      this.weightSigmaTextBox.Location = new System.Drawing.Point(61, 45);
      this.weightSigmaTextBox.Name = "weightSigmaTextBox";
      this.weightSigmaTextBox.Size = new System.Drawing.Size(100, 20);
      this.weightSigmaTextBox.TabIndex = 3;
      this.weightSigmaTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.weightTextBox_Validating);
      // 
      // VariableSymbolView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tabControl);
      this.Name = "VariableSymbolView";
      this.Size = new System.Drawing.Size(549, 419);
      this.Controls.SetChildIndex(this.infoLabel, 0);
      this.Controls.SetChildIndex(this.tabControl, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.tabControl.ResumeLayout(false);
      this.parametersTabPage.ResumeLayout(false);
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private HeuristicLab.MainForm.WindowsForms.DragOverTabControl tabControl;
    private System.Windows.Forms.TabPage variableNamesTabPage;
    private System.Windows.Forms.TabPage parametersTabPage;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.TextBox weightManipulatorNuTextBox;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox weightManipulatorSigmaTextBox;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.TextBox weightNuTextBox;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox weightSigmaTextBox;
  }
}
