namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  partial class ExportJsonDialog {
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
      this.components = new System.ComponentModel.Container();
      this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.exportButton = new System.Windows.Forms.Button();
      this.treeView = new System.Windows.Forms.TreeView();
      this.groupBoxDetails = new System.Windows.Forms.GroupBox();
      this.panelParameterDetails = new System.Windows.Forms.Panel();
      this.resultItems = new System.Windows.Forms.CheckedListBox();
      this.groupBox3 = new System.Windows.Forms.GroupBox();
      this.treeViewResults = new System.Windows.Forms.TreeView();
      this.splitContainer2 = new System.Windows.Forms.SplitContainer();
      this.tabControl1 = new System.Windows.Forms.TabControl();
      this.tabPage1 = new System.Windows.Forms.TabPage();
      this.tabPage2 = new System.Windows.Forms.TabPage();
      this.label1 = new System.Windows.Forms.Label();
      this.textBoxTemplateName = new System.Windows.Forms.TextBox();
      this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      this.groupBox = new System.Windows.Forms.GroupBox();
      this.panelResultDetails = new System.Windows.Forms.Panel();
      this.jsonItemBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.groupBoxDetails.SuspendLayout();
      this.groupBox3.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
      this.splitContainer2.Panel1.SuspendLayout();
      this.splitContainer2.Panel2.SuspendLayout();
      this.splitContainer2.SuspendLayout();
      this.tabControl1.SuspendLayout();
      this.tabPage1.SuspendLayout();
      this.tabPage2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.groupBox.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.jsonItemBindingSource)).BeginInit();
      this.SuspendLayout();
      // 
      // dataGridViewTextBoxColumn1
      // 
      this.dataGridViewTextBoxColumn1.DataPropertyName = "Value";
      this.dataGridViewTextBoxColumn1.HeaderText = "Value";
      this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
      // 
      // exportButton
      // 
      this.exportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.exportButton.Location = new System.Drawing.Point(630, 575);
      this.exportButton.Name = "exportButton";
      this.exportButton.Size = new System.Drawing.Size(191, 29);
      this.exportButton.TabIndex = 1;
      this.exportButton.Text = "Export";
      this.exportButton.UseVisualStyleBackColor = true;
      this.exportButton.Click += new System.EventHandler(this.exportButton_Click);
      // 
      // treeView
      // 
      this.treeView.CheckBoxes = true;
      this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.treeView.Location = new System.Drawing.Point(0, 6);
      this.treeView.Name = "treeView";
      this.treeView.Size = new System.Drawing.Size(370, 493);
      this.treeView.TabIndex = 3;
      this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
      // 
      // groupBoxDetails
      // 
      this.groupBoxDetails.Controls.Add(this.panelParameterDetails);
      this.groupBoxDetails.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBoxDetails.Location = new System.Drawing.Point(0, 0);
      this.groupBoxDetails.Name = "groupBoxDetails";
      this.groupBoxDetails.Size = new System.Drawing.Size(422, 499);
      this.groupBoxDetails.TabIndex = 4;
      this.groupBoxDetails.TabStop = false;
      this.groupBoxDetails.Text = "Details";
      // 
      // panelParameterDetails
      // 
      this.panelParameterDetails.AutoScroll = true;
      this.panelParameterDetails.Dock = System.Windows.Forms.DockStyle.Fill;
      this.panelParameterDetails.Location = new System.Drawing.Point(3, 16);
      this.panelParameterDetails.Name = "panelParameterDetails";
      this.panelParameterDetails.Size = new System.Drawing.Size(416, 480);
      this.panelParameterDetails.TabIndex = 0;
      // 
      // resultItems
      // 
      this.resultItems.Dock = System.Windows.Forms.DockStyle.Fill;
      this.resultItems.FormattingEnabled = true;
      this.resultItems.Location = new System.Drawing.Point(0, 6);
      this.resultItems.Name = "resultItems";
      this.resultItems.Size = new System.Drawing.Size(373, 393);
      this.resultItems.TabIndex = 5;
      this.resultItems.SelectedValueChanged += new System.EventHandler(this.resultItems_SelectedValueChanged);
      // 
      // groupBox3
      // 
      this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.groupBox3.Controls.Add(this.splitContainer1);
      this.groupBox3.Controls.Add(this.treeViewResults);
      this.groupBox3.Location = new System.Drawing.Point(6, 6);
      this.groupBox3.Name = "groupBox3";
      this.groupBox3.Size = new System.Drawing.Size(790, 493);
      this.groupBox3.TabIndex = 7;
      this.groupBox3.TabStop = false;
      this.groupBox3.Text = "Result Elements";
      // 
      // treeViewResults
      // 
      this.treeViewResults.CheckBoxes = true;
      this.treeViewResults.Location = new System.Drawing.Point(6, 19);
      this.treeViewResults.Name = "treeViewResults";
      this.treeViewResults.Size = new System.Drawing.Size(777, 63);
      this.treeViewResults.TabIndex = 6;
      // 
      // splitContainer2
      // 
      this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer2.Location = new System.Drawing.Point(3, 3);
      this.splitContainer2.Name = "splitContainer2";
      // 
      // splitContainer2.Panel1
      // 
      this.splitContainer2.Panel1.Controls.Add(this.treeView);
      this.splitContainer2.Panel1.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
      // 
      // splitContainer2.Panel2
      // 
      this.splitContainer2.Panel2.Controls.Add(this.groupBoxDetails);
      this.splitContainer2.Size = new System.Drawing.Size(796, 499);
      this.splitContainer2.SplitterDistance = 370;
      this.splitContainer2.TabIndex = 9;
      // 
      // tabControl1
      // 
      this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl1.Controls.Add(this.tabPage1);
      this.tabControl1.Controls.Add(this.tabPage2);
      this.tabControl1.Location = new System.Drawing.Point(12, 38);
      this.tabControl1.Name = "tabControl1";
      this.tabControl1.SelectedIndex = 0;
      this.tabControl1.Size = new System.Drawing.Size(810, 531);
      this.tabControl1.TabIndex = 10;
      // 
      // tabPage1
      // 
      this.tabPage1.Controls.Add(this.splitContainer2);
      this.tabPage1.Location = new System.Drawing.Point(4, 22);
      this.tabPage1.Name = "tabPage1";
      this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage1.Size = new System.Drawing.Size(802, 505);
      this.tabPage1.TabIndex = 0;
      this.tabPage1.Text = "Parameters";
      this.tabPage1.UseVisualStyleBackColor = true;
      // 
      // tabPage2
      // 
      this.tabPage2.Controls.Add(this.groupBox3);
      this.tabPage2.Location = new System.Drawing.Point(4, 22);
      this.tabPage2.Name = "tabPage2";
      this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage2.Size = new System.Drawing.Size(802, 505);
      this.tabPage2.TabIndex = 1;
      this.tabPage2.Text = "Results";
      this.tabPage2.UseVisualStyleBackColor = true;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 15);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(82, 13);
      this.label1.TabIndex = 11;
      this.label1.Text = "Template Name";
      // 
      // textBoxTemplateName
      // 
      this.textBoxTemplateName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.errorProvider.SetIconAlignment(this.textBoxTemplateName, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.textBoxTemplateName.Location = new System.Drawing.Point(100, 12);
      this.textBoxTemplateName.Name = "textBoxTemplateName";
      this.textBoxTemplateName.Size = new System.Drawing.Size(721, 20);
      this.textBoxTemplateName.TabIndex = 12;
      this.textBoxTemplateName.Text = "Template";
      this.textBoxTemplateName.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxTemplateName_Validating);
      // 
      // errorProvider
      // 
      this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
      this.errorProvider.ContainerControl = this;
      // 
      // splitContainer1
      // 
      this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.splitContainer1.Location = new System.Drawing.Point(6, 88);
      this.splitContainer1.Name = "splitContainer1";
      // 
      // splitContainer1.Panel1
      // 
      this.splitContainer1.Panel1.Controls.Add(this.resultItems);
      this.splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.groupBox);
      this.splitContainer1.Size = new System.Drawing.Size(778, 399);
      this.splitContainer1.SplitterDistance = 373;
      this.splitContainer1.TabIndex = 7;
      // 
      // groupBox
      // 
      this.groupBox.Controls.Add(this.panelResultDetails);
      this.groupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox.Location = new System.Drawing.Point(0, 0);
      this.groupBox.Name = "groupBox";
      this.groupBox.Size = new System.Drawing.Size(401, 399);
      this.groupBox.TabIndex = 5;
      this.groupBox.TabStop = false;
      this.groupBox.Text = "Details";
      // 
      // panelResultDetails
      // 
      this.panelResultDetails.AutoScroll = true;
      this.panelResultDetails.Dock = System.Windows.Forms.DockStyle.Fill;
      this.panelResultDetails.Location = new System.Drawing.Point(3, 16);
      this.panelResultDetails.Name = "panelResultDetails";
      this.panelResultDetails.Size = new System.Drawing.Size(395, 380);
      this.panelResultDetails.TabIndex = 0;
      // 
      // jsonItemBindingSource
      // 
      this.jsonItemBindingSource.DataSource = typeof(HeuristicLab.JsonInterface.IJsonItem);
      // 
      // ExportJsonDialog
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(834, 616);
      this.Controls.Add(this.textBoxTemplateName);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.exportButton);
      this.Controls.Add(this.tabControl1);
      this.Name = "ExportJsonDialog";
      this.RightToLeft = System.Windows.Forms.RightToLeft.No;
      this.ShowIcon = false;
      this.Text = "Export Json";
      this.groupBoxDetails.ResumeLayout(false);
      this.groupBox3.ResumeLayout(false);
      this.splitContainer2.Panel1.ResumeLayout(false);
      this.splitContainer2.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
      this.splitContainer2.ResumeLayout(false);
      this.tabControl1.ResumeLayout(false);
      this.tabPage1.ResumeLayout(false);
      this.tabPage2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
      this.splitContainer1.ResumeLayout(false);
      this.groupBox.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.jsonItemBindingSource)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.BindingSource jsonItemBindingSource;
    private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
    private System.Windows.Forms.Button exportButton;
    private System.Windows.Forms.TreeView treeView;
    private System.Windows.Forms.GroupBox groupBoxDetails;
    private System.Windows.Forms.Panel panelParameterDetails;
    private System.Windows.Forms.CheckedListBox resultItems;
    private System.Windows.Forms.GroupBox groupBox3;
    private System.Windows.Forms.SplitContainer splitContainer2;
    private System.Windows.Forms.TabControl tabControl1;
    private System.Windows.Forms.TabPage tabPage1;
    private System.Windows.Forms.TabPage tabPage2;
    private System.Windows.Forms.TreeView treeViewResults;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox textBoxTemplateName;
    private System.Windows.Forms.ErrorProvider errorProvider;
    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.GroupBox groupBox;
    private System.Windows.Forms.Panel panelResultDetails;
  }
}