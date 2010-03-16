namespace HeuristicLab.DeploymentService.AdminClient {
  partial class ProductEditor {
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
      this.components = new System.ComponentModel.Container();
      this.refreshButton = new System.Windows.Forms.Button();
      this.saveButton = new System.Windows.Forms.Button();
      this.newProductButton = new System.Windows.Forms.Button();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.productsListView = new System.Windows.Forms.ListView();
      this.productNameHeader = new System.Windows.Forms.ColumnHeader();
      this.productVersionHeader = new System.Windows.Forms.ColumnHeader();
      this.productImageList = new System.Windows.Forms.ImageList(this.components);
      this.pluginImageList = new System.Windows.Forms.ImageList(this.components);
      this.pluginsLabel = new System.Windows.Forms.Label();
      this.versionTextBox = new System.Windows.Forms.TextBox();
      this.versionLabel = new System.Windows.Forms.Label();
      this.nameTextBox = new System.Windows.Forms.TextBox();
      this.nameLabel = new System.Windows.Forms.Label();
      this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
      this.pluginListView = new HeuristicLab.DeploymentService.AdminClient.PluginListView();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // refreshButton
      // 
      this.refreshButton.Location = new System.Drawing.Point(3, 3);
      this.refreshButton.Name = "refreshButton";
      this.refreshButton.Size = new System.Drawing.Size(75, 23);
      this.refreshButton.TabIndex = 1;
      this.refreshButton.Text = "Refresh";
      this.refreshButton.UseVisualStyleBackColor = true;
      this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
      // 
      // saveButton
      // 
      this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.saveButton.Location = new System.Drawing.Point(3, 365);
      this.saveButton.Name = "saveButton";
      this.saveButton.Size = new System.Drawing.Size(96, 23);
      this.saveButton.TabIndex = 2;
      this.saveButton.Text = "Upload Products";
      this.saveButton.UseVisualStyleBackColor = true;
      this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
      // 
      // newProductButton
      // 
      this.newProductButton.Location = new System.Drawing.Point(84, 3);
      this.newProductButton.Name = "newProductButton";
      this.newProductButton.Size = new System.Drawing.Size(91, 23);
      this.newProductButton.TabIndex = 3;
      this.newProductButton.Text = "New product";
      this.newProductButton.UseVisualStyleBackColor = true;
      this.newProductButton.Click += new System.EventHandler(this.newProductButton_Click);
      // 
      // splitContainer
      // 
      this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.splitContainer.Location = new System.Drawing.Point(3, 32);
      this.splitContainer.Name = "splitContainer";
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.productsListView);
      this.splitContainer.Panel1.Controls.Add(this.saveButton);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.pluginListView);
      this.splitContainer.Panel2.Controls.Add(this.pluginsLabel);
      this.splitContainer.Panel2.Controls.Add(this.versionTextBox);
      this.splitContainer.Panel2.Controls.Add(this.versionLabel);
      this.splitContainer.Panel2.Controls.Add(this.nameTextBox);
      this.splitContainer.Panel2.Controls.Add(this.nameLabel);
      this.splitContainer.Size = new System.Drawing.Size(659, 391);
      this.splitContainer.SplitterDistance = 319;
      this.splitContainer.TabIndex = 4;
      // 
      // productsListView
      // 
      this.productsListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.productsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.productNameHeader,
            this.productVersionHeader});
      this.productsListView.FullRowSelect = true;
      this.productsListView.Location = new System.Drawing.Point(3, 3);
      this.productsListView.MultiSelect = false;
      this.productsListView.Name = "productsListView";
      this.productsListView.Size = new System.Drawing.Size(313, 356);
      this.productsListView.SmallImageList = this.productImageList;
      this.productsListView.TabIndex = 4;
      this.productsListView.UseCompatibleStateImageBehavior = false;
      this.productsListView.View = System.Windows.Forms.View.Details;
      this.productsListView.SelectedIndexChanged += new System.EventHandler(this.productsListBox_SelectedIndexChanged);
      // 
      // productNameHeader
      // 
      this.productNameHeader.Text = "Name";
      this.productNameHeader.Width = 150;
      // 
      // productVersionHeader
      // 
      this.productVersionHeader.Text = "Version";
      this.productVersionHeader.Width = 100;
      // 
      // productImageList
      // 
      this.productImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
      this.productImageList.ImageSize = new System.Drawing.Size(16, 16);
      this.productImageList.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // pluginImageList
      // 
      this.pluginImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
      this.pluginImageList.ImageSize = new System.Drawing.Size(16, 16);
      this.pluginImageList.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // pluginsLabel
      // 
      this.pluginsLabel.AutoSize = true;
      this.pluginsLabel.Location = new System.Drawing.Point(11, 69);
      this.pluginsLabel.Name = "pluginsLabel";
      this.pluginsLabel.Size = new System.Drawing.Size(44, 13);
      this.pluginsLabel.TabIndex = 6;
      this.pluginsLabel.Text = "Plugins:";
      // 
      // versionTextBox
      // 
      this.versionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.versionTextBox.Location = new System.Drawing.Point(68, 29);
      this.versionTextBox.Name = "versionTextBox";
      this.versionTextBox.Size = new System.Drawing.Size(233, 20);
      this.versionTextBox.TabIndex = 5;
      this.versionTextBox.TextChanged += new System.EventHandler(this.versionTextBox_TextChanged);
      // 
      // versionLabel
      // 
      this.versionLabel.AutoSize = true;
      this.versionLabel.Location = new System.Drawing.Point(10, 32);
      this.versionLabel.Name = "versionLabel";
      this.versionLabel.Size = new System.Drawing.Size(45, 13);
      this.versionLabel.TabIndex = 4;
      this.versionLabel.Text = "Version:";
      // 
      // nameTextBox
      // 
      this.nameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.nameTextBox.Location = new System.Drawing.Point(68, 3);
      this.nameTextBox.Name = "nameTextBox";
      this.nameTextBox.Size = new System.Drawing.Size(233, 20);
      this.nameTextBox.TabIndex = 3;
      this.nameTextBox.TextChanged += new System.EventHandler(this.nameTextBox_TextChanged);
      // 
      // nameLabel
      // 
      this.nameLabel.AutoSize = true;
      this.nameLabel.Location = new System.Drawing.Point(17, 6);
      this.nameLabel.Name = "nameLabel";
      this.nameLabel.Size = new System.Drawing.Size(38, 13);
      this.nameLabel.TabIndex = 2;
      this.nameLabel.Text = "Name:";
      // 
      // errorProvider
      // 
      this.errorProvider.ContainerControl = this;
      // 
      // pluginListView
      // 
      this.pluginListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.pluginListView.Location = new System.Drawing.Point(3, 85);
      this.pluginListView.Name = "pluginListView";
      this.pluginListView.Plugins = null;
      this.pluginListView.Size = new System.Drawing.Size(330, 303);
      this.pluginListView.TabIndex = 7;
      this.pluginListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.pluginListView_ItemChecked);
      // 
      // ProductEditor
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.splitContainer);
      this.Controls.Add(this.newProductButton);
      this.Controls.Add(this.refreshButton);
      this.Name = "ProductEditor";
      this.Size = new System.Drawing.Size(665, 426);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      this.splitContainer.Panel2.PerformLayout();
      this.splitContainer.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button refreshButton;
    private System.Windows.Forms.Button saveButton;
    private System.Windows.Forms.Button newProductButton;
    private System.Windows.Forms.SplitContainer splitContainer;
    private System.Windows.Forms.Label pluginsLabel;
    private System.Windows.Forms.TextBox versionTextBox;
    private System.Windows.Forms.Label versionLabel;
    private System.Windows.Forms.TextBox nameTextBox;
    private System.Windows.Forms.Label nameLabel;
    private System.Windows.Forms.ErrorProvider errorProvider;
    private System.Windows.Forms.ListView productsListView;
    private System.Windows.Forms.ColumnHeader productNameHeader;
    private System.Windows.Forms.ColumnHeader productVersionHeader;
    private System.Windows.Forms.ImageList productImageList;
    private System.Windows.Forms.ImageList pluginImageList;
    private PluginListView pluginListView;

  }
}
