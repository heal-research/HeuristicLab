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
      this.panel = new System.Windows.Forms.Panel();
      this.jsonItemBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.groupBoxDetails.SuspendLayout();
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
      this.exportButton.Location = new System.Drawing.Point(801, 687);
      this.exportButton.Name = "exportButton";
      this.exportButton.Size = new System.Drawing.Size(121, 34);
      this.exportButton.TabIndex = 1;
      this.exportButton.Text = "Export";
      this.exportButton.UseVisualStyleBackColor = true;
      this.exportButton.Click += new System.EventHandler(this.exportButton_Click);
      // 
      // treeView
      // 
      this.treeView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
      this.treeView.Location = new System.Drawing.Point(12, 12);
      this.treeView.Name = "treeView";
      this.treeView.Size = new System.Drawing.Size(342, 669);
      this.treeView.TabIndex = 3;
      this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
      // 
      // groupBoxDetails
      // 
      this.groupBoxDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.groupBoxDetails.Controls.Add(this.panel);
      this.groupBoxDetails.Location = new System.Drawing.Point(360, 12);
      this.groupBoxDetails.Name = "groupBoxDetails";
      this.groupBoxDetails.Size = new System.Drawing.Size(562, 669);
      this.groupBoxDetails.TabIndex = 4;
      this.groupBoxDetails.TabStop = false;
      this.groupBoxDetails.Text = "Details";
      // 
      // panel
      // 
      this.panel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.panel.AutoScroll = true;
      this.panel.Location = new System.Drawing.Point(7, 20);
      this.panel.Name = "panel";
      this.panel.Size = new System.Drawing.Size(549, 643);
      this.panel.TabIndex = 0;
      // 
      // jsonItemBindingSource
      // 
      this.jsonItemBindingSource.DataSource = typeof(HeuristicLab.JsonInterface.JsonItem);
      // 
      // ExportJsonDialog
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(934, 733);
      this.Controls.Add(this.groupBoxDetails);
      this.Controls.Add(this.treeView);
      this.Controls.Add(this.exportButton);
      this.Name = "ExportJsonDialog";
      this.RightToLeft = System.Windows.Forms.RightToLeft.No;
      this.ShowIcon = false;
      this.Text = "Export Json";
      this.groupBoxDetails.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.jsonItemBindingSource)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.BindingSource jsonItemBindingSource;
    private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
    private System.Windows.Forms.Button exportButton;
    private System.Windows.Forms.TreeView treeView;
    private System.Windows.Forms.GroupBox groupBoxDetails;
    private System.Windows.Forms.Panel panel;
  }
}