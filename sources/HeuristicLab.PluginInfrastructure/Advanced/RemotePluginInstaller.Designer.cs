namespace HeuristicLab.PluginInfrastructure.Advanced {
  partial class RemotePluginInstaller {
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
      System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Products", System.Windows.Forms.HorizontalAlignment.Left);
      System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("New Plugins", System.Windows.Forms.HorizontalAlignment.Left);
      System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("All Plugins", System.Windows.Forms.HorizontalAlignment.Left);
      this.imageListForRemoteItems = new System.Windows.Forms.ImageList(this.components);
      this.remotePluginsListView = new HeuristicLab.PluginInfrastructure.Advanced.MultiSelectListView();
      this.nameHeader = new System.Windows.Forms.ColumnHeader();
      this.versionHeader = new System.Windows.Forms.ColumnHeader();
      this.descriptionHeader = new System.Windows.Forms.ColumnHeader();
      this.SuspendLayout();
      // 
      // imageListForRemoteItems
      // 
      this.imageListForRemoteItems.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
      this.imageListForRemoteItems.ImageSize = new System.Drawing.Size(13, 13);
      this.imageListForRemoteItems.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // remotePluginsListView
      // 
      this.remotePluginsListView.CheckBoxes = true;
      this.remotePluginsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.nameHeader,
            this.versionHeader,
            this.descriptionHeader});
      this.remotePluginsListView.Dock = System.Windows.Forms.DockStyle.Fill;
      listViewGroup1.Header = "Products";
      listViewGroup1.Name = "productsGroup";
      listViewGroup2.Header = "New Plugins";
      listViewGroup2.Name = "newPluginsGroup";
      listViewGroup3.Header = "All Plugins";
      listViewGroup3.Name = "allPluginsGroup";
      this.remotePluginsListView.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2,
            listViewGroup3});
      this.remotePluginsListView.Location = new System.Drawing.Point(0, 0);
      this.remotePluginsListView.Name = "remotePluginsListView";
      this.remotePluginsListView.Size = new System.Drawing.Size(533, 558);
      this.remotePluginsListView.StateImageList = this.imageListForRemoteItems;
      this.remotePluginsListView.SuppressItemCheckedEvents = false;
      this.remotePluginsListView.TabIndex = 0;
      this.remotePluginsListView.UseCompatibleStateImageBehavior = false;
      this.remotePluginsListView.View = System.Windows.Forms.View.Details;
      this.remotePluginsListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.remotePluginsListView_ItemChecked);
      // 
      // nameHeader
      // 
      this.nameHeader.Text = "Name";
      this.nameHeader.Width = 185;
      // 
      // versionHeader
      // 
      this.versionHeader.Text = "Version";
      this.versionHeader.Width = 93;
      // 
      // descriptionHeader
      // 
      this.descriptionHeader.Text = "Description";
      this.descriptionHeader.Width = 250;
      // 
      // RemotePluginInstaller
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.remotePluginsListView);
      this.Name = "RemotePluginInstaller";
      this.Size = new System.Drawing.Size(533, 558);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ImageList imageListForRemoteItems;
    private MultiSelectListView remotePluginsListView;
    private System.Windows.Forms.ColumnHeader nameHeader;
    private System.Windows.Forms.ColumnHeader versionHeader;
    private System.Windows.Forms.ColumnHeader descriptionHeader;
  }
}
