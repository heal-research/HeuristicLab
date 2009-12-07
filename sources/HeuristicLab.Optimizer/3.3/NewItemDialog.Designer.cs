namespace HeuristicLab.Optimizer {
  partial class NewItemDialog {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewItemDialog));
      this.okButton = new System.Windows.Forms.Button();
      this.cancelButton = new System.Windows.Forms.Button();
      this.itemsListView = new System.Windows.Forms.ListView();
      this.nameColumnHeader = new System.Windows.Forms.ColumnHeader();
      this.descriptioncolumnHeader = new System.Windows.Forms.ColumnHeader();
      this.itemsLabel = new System.Windows.Forms.Label();
      this.showIconsCheckBox = new System.Windows.Forms.CheckBox();
      this.showDetailsCheckBox = new System.Windows.Forms.CheckBox();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.SuspendLayout();
      // 
      // okButton
      // 
      this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.okButton.Enabled = false;
      this.okButton.Location = new System.Drawing.Point(456, 409);
      this.okButton.Name = "okButton";
      this.okButton.Size = new System.Drawing.Size(75, 23);
      this.okButton.TabIndex = 2;
      this.okButton.Text = "&OK";
      this.okButton.UseVisualStyleBackColor = true;
      this.okButton.Click += new System.EventHandler(this.okButton_Click);
      // 
      // cancelButton
      // 
      this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.cancelButton.Location = new System.Drawing.Point(537, 409);
      this.cancelButton.Name = "cancelButton";
      this.cancelButton.Size = new System.Drawing.Size(75, 23);
      this.cancelButton.TabIndex = 3;
      this.cancelButton.Text = "&Cancel";
      this.cancelButton.UseVisualStyleBackColor = true;
      // 
      // itemsListView
      // 
      this.itemsListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.itemsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.nameColumnHeader,
            this.descriptioncolumnHeader});
      this.itemsListView.HideSelection = false;
      this.itemsListView.Location = new System.Drawing.Point(12, 33);
      this.itemsListView.MultiSelect = false;
      this.itemsListView.Name = "itemsListView";
      this.itemsListView.Size = new System.Drawing.Size(600, 370);
      this.itemsListView.TabIndex = 1;
      this.itemsListView.UseCompatibleStateImageBehavior = false;
      this.itemsListView.View = System.Windows.Forms.View.SmallIcon;
      this.itemsListView.SelectedIndexChanged += new System.EventHandler(this.itemTypesListView_SelectedIndexChanged);
      this.itemsListView.DoubleClick += new System.EventHandler(this.itemTypesListView_DoubleClick);
      // 
      // nameColumnHeader
      // 
      this.nameColumnHeader.Text = "Name";
      // 
      // descriptioncolumnHeader
      // 
      this.descriptioncolumnHeader.Text = "Description";
      // 
      // itemsLabel
      // 
      this.itemsLabel.AutoSize = true;
      this.itemsLabel.Location = new System.Drawing.Point(12, 9);
      this.itemsLabel.Name = "itemsLabel";
      this.itemsLabel.Size = new System.Drawing.Size(35, 13);
      this.itemsLabel.TabIndex = 0;
      this.itemsLabel.Text = "&Items:";
      // 
      // showIconsCheckBox
      // 
      this.showIconsCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.showIconsCheckBox.Appearance = System.Windows.Forms.Appearance.Button;
      this.showIconsCheckBox.Checked = true;
      this.showIconsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
      this.showIconsCheckBox.Image = ((System.Drawing.Image)(resources.GetObject("showIconsCheckBox.Image")));
      this.showIconsCheckBox.Location = new System.Drawing.Point(558, 3);
      this.showIconsCheckBox.Name = "showIconsCheckBox";
      this.showIconsCheckBox.Size = new System.Drawing.Size(24, 24);
      this.showIconsCheckBox.TabIndex = 4;
      this.toolTip.SetToolTip(this.showIconsCheckBox, "Show Icons");
      this.showIconsCheckBox.UseVisualStyleBackColor = true;
      this.showIconsCheckBox.Click += new System.EventHandler(this.showIconsCheckBox_Click);
      // 
      // showDetailsCheckBox
      // 
      this.showDetailsCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.showDetailsCheckBox.Appearance = System.Windows.Forms.Appearance.Button;
      this.showDetailsCheckBox.Image = ((System.Drawing.Image)(resources.GetObject("showDetailsCheckBox.Image")));
      this.showDetailsCheckBox.Location = new System.Drawing.Point(588, 3);
      this.showDetailsCheckBox.Name = "showDetailsCheckBox";
      this.showDetailsCheckBox.Size = new System.Drawing.Size(24, 24);
      this.showDetailsCheckBox.TabIndex = 5;
      this.toolTip.SetToolTip(this.showDetailsCheckBox, "Show Details");
      this.showDetailsCheckBox.UseVisualStyleBackColor = true;
      this.showDetailsCheckBox.Click += new System.EventHandler(this.showDetailsCheckBox_Click);
      // 
      // NewItemDialog
      // 
      this.AcceptButton = this.okButton;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.cancelButton;
      this.ClientSize = new System.Drawing.Size(624, 444);
      this.Controls.Add(this.showDetailsCheckBox);
      this.Controls.Add(this.showIconsCheckBox);
      this.Controls.Add(this.itemsLabel);
      this.Controls.Add(this.itemsListView);
      this.Controls.Add(this.cancelButton);
      this.Controls.Add(this.okButton);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "NewItemDialog";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "New Item";
      this.TopMost = true;
      this.Load += new System.EventHandler(this.NewItemDialog_Load);
      this.Shown += new System.EventHandler(this.NewItemDialog_Shown);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button okButton;
    private System.Windows.Forms.Button cancelButton;
    private System.Windows.Forms.ListView itemsListView;
    private System.Windows.Forms.Label itemsLabel;
    private System.Windows.Forms.CheckBox showIconsCheckBox;
    private System.Windows.Forms.CheckBox showDetailsCheckBox;
    private System.Windows.Forms.ToolTip toolTip;
    private System.Windows.Forms.ColumnHeader nameColumnHeader;
    private System.Windows.Forms.ColumnHeader descriptioncolumnHeader;
  }
}