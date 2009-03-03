namespace HeuristicLab.Assignment.QAP {
  partial class QAPInjectorView {
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
      this.loadButton = new System.Windows.Forms.Button();
      this.facilitiesTextBox = new System.Windows.Forms.TextBox();
      this.qualTextBox = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
      this.knownQualCheckBox = new System.Windows.Forms.CheckBox();
      this.tabControl = new System.Windows.Forms.TabControl();
      this.dataTabPage = new System.Windows.Forms.TabPage();
      this.varInfoTabPage = new System.Windows.Forms.TabPage();
      this.descriptionTabPage = new System.Windows.Forms.TabPage();
      this.operatorBaseVariableInfosView = new HeuristicLab.Core.OperatorBaseVariableInfosView();
      this.operatorBaseDescriptionView = new HeuristicLab.Core.OperatorBaseDescriptionView();
      this.tabControl.SuspendLayout();
      this.dataTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // loadButton
      // 
      this.loadButton.Location = new System.Drawing.Point(11, 96);
      this.loadButton.Name = "loadButton";
      this.loadButton.Size = new System.Drawing.Size(231, 24);
      this.loadButton.TabIndex = 0;
      this.loadButton.Text = "Load QAP file";
      this.loadButton.UseVisualStyleBackColor = true;
      this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
      // 
      // facilitiesTextBox
      // 
      this.facilitiesTextBox.Location = new System.Drawing.Point(167, 11);
      this.facilitiesTextBox.Name = "facilitiesTextBox";
      this.facilitiesTextBox.Size = new System.Drawing.Size(78, 20);
      this.facilitiesTextBox.TabIndex = 2;
      this.facilitiesTextBox.TextChanged += new System.EventHandler(this.facilitiesTextBox_TextChanged);
      // 
      // qualTextBox
      // 
      this.qualTextBox.Location = new System.Drawing.Point(167, 64);
      this.qualTextBox.Name = "qualTextBox";
      this.qualTextBox.Size = new System.Drawing.Size(78, 20);
      this.qualTextBox.TabIndex = 3;
      this.qualTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.qualTextBox_Validating);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(15, 14);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(50, 13);
      this.label1.TabIndex = 4;
      this.label1.Text = "Facilities:";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(15, 66);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(96, 13);
      this.label2.TabIndex = 5;
      this.label2.Text = "Best Know Quality:";
      // 
      // openFileDialog
      // 
      this.openFileDialog.FileName = "openFileDialog1";
      // 
      // knownQualCheckBox
      // 
      this.knownQualCheckBox.AutoSize = true;
      this.knownQualCheckBox.Location = new System.Drawing.Point(14, 39);
      this.knownQualCheckBox.Name = "knownQualCheckBox";
      this.knownQualCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
      this.knownQualCheckBox.Size = new System.Drawing.Size(167, 17);
      this.knownQualCheckBox.TabIndex = 6;
      this.knownQualCheckBox.Text = "Best Known Quality Available:";
      this.knownQualCheckBox.UseVisualStyleBackColor = true;
      this.knownQualCheckBox.CheckedChanged += new System.EventHandler(this.knownQualCheckBox_CheckedChanged);
      // 
      // tabControl
      // 
      this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl.Controls.Add(this.dataTabPage);
      this.tabControl.Controls.Add(this.varInfoTabPage);
      this.tabControl.Controls.Add(this.descriptionTabPage);
      this.tabControl.Location = new System.Drawing.Point(3, 3);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(256, 152);
      this.tabControl.TabIndex = 7;
      // 
      // dataTabPage
      // 
      this.dataTabPage.Controls.Add(this.loadButton);
      this.dataTabPage.Controls.Add(this.knownQualCheckBox);
      this.dataTabPage.Controls.Add(this.label1);
      this.dataTabPage.Controls.Add(this.label2);
      this.dataTabPage.Controls.Add(this.facilitiesTextBox);
      this.dataTabPage.Controls.Add(this.qualTextBox);
      this.dataTabPage.Location = new System.Drawing.Point(4, 22);
      this.dataTabPage.Name = "dataTabPage";
      this.dataTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.dataTabPage.Size = new System.Drawing.Size(248, 126);
      this.dataTabPage.TabIndex = 0;
      this.dataTabPage.Text = "QAP Data";
      this.dataTabPage.UseVisualStyleBackColor = true;
      // 
      // operatorBaseVariableInfosView
      // 
      this.operatorBaseVariableInfosView.Caption = "Operator";
      this.operatorBaseVariableInfosView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.operatorBaseVariableInfosView.Location = new System.Drawing.Point(3, 3);
      this.operatorBaseVariableInfosView.Name = "operatorBaseVariableInfosView";
      this.operatorBaseVariableInfosView.Operator = null;
      this.operatorBaseVariableInfosView.Size = new System.Drawing.Size(262, 152);
      this.operatorBaseVariableInfosView.TabIndex = 0;
      // 
      // varInfoTabPage
      // 
      this.varInfoTabPage.Controls.Add(this.operatorBaseVariableInfosView);
      this.varInfoTabPage.Location = new System.Drawing.Point(4, 22);
      this.varInfoTabPage.Name = "varInfoTabPage";
      this.varInfoTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.varInfoTabPage.Size = new System.Drawing.Size(453, 166);
      this.varInfoTabPage.TabIndex = 1;
      this.varInfoTabPage.Text = "Variable Infos";
      this.varInfoTabPage.UseVisualStyleBackColor = true;
      // 
      // operatorBaseDescriptionView
      // 
      this.operatorBaseDescriptionView.Caption = "Operator";
      this.operatorBaseDescriptionView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.operatorBaseDescriptionView.Location = new System.Drawing.Point(3, 3);
      this.operatorBaseDescriptionView.Name = "operatorBaseDescriptionView";
      this.operatorBaseDescriptionView.Operator = null;
      this.operatorBaseDescriptionView.Size = new System.Drawing.Size(262, 152);
      this.operatorBaseDescriptionView.TabIndex = 0;
      // 
      // descriptionTabPage
      // 
      this.descriptionTabPage.Controls.Add(this.operatorBaseDescriptionView);
      this.descriptionTabPage.Location = new System.Drawing.Point(4, 22);
      this.descriptionTabPage.Name = "descriptionTabPage";
      this.descriptionTabPage.Size = new System.Drawing.Size(453, 166);
      this.descriptionTabPage.TabIndex = 2;
      this.descriptionTabPage.Text = "Description";
      this.descriptionTabPage.UseVisualStyleBackColor = true;
      // 
      // QAPInjectorView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tabControl);
      this.Name = "QAPInjectorView";
      this.Size = new System.Drawing.Size(262, 156);
      this.tabControl.ResumeLayout(false);
      this.dataTabPage.ResumeLayout(false);
      this.dataTabPage.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button loadButton;
    private System.Windows.Forms.TextBox facilitiesTextBox;
    private System.Windows.Forms.TextBox qualTextBox;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.OpenFileDialog openFileDialog;
    private System.Windows.Forms.CheckBox knownQualCheckBox;
    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TabPage dataTabPage;
    private System.Windows.Forms.TabPage varInfoTabPage;
    private System.Windows.Forms.TabPage descriptionTabPage;
    private HeuristicLab.Core.OperatorBaseVariableInfosView operatorBaseVariableInfosView;
    private HeuristicLab.Core.OperatorBaseDescriptionView operatorBaseDescriptionView;
  }
}
