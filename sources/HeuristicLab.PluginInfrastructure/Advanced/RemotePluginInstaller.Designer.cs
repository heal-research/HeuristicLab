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
      System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Available Products", System.Windows.Forms.HorizontalAlignment.Left);
      System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Available Plugins (Most Recent Version)", System.Windows.Forms.HorizontalAlignment.Left);
      System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("Available Plugins (All Versions)", System.Windows.Forms.HorizontalAlignment.Left);
      this.remotePluginsListView = new System.Windows.Forms.ListView();
      this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
      this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
      this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
      this.imageListForRemoteItems = new System.Windows.Forms.ImageList(this.components);
      this.SuspendLayout();
      // 
      // remotePluginsListView
      // 
      this.remotePluginsListView.CheckBoxes = true;
      this.remotePluginsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
      this.remotePluginsListView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.remotePluginsListView.FullRowSelect = true;
      listViewGroup1.Header = "Available Products";
      listViewGroup1.Name = "productsGroup";
      listViewGroup2.Header = "Available Plugins (Most Recent Version)";
      listViewGroup2.Name = "newPluginsGroup";
      listViewGroup3.Header = "Available Plugins (All Versions)";
      listViewGroup3.Name = "allPluginsGroup";
      this.remotePluginsListView.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2,
            listViewGroup3});
      this.remotePluginsListView.Location = new System.Drawing.Point(0, 0);
      this.remotePluginsListView.Name = "remotePluginsListView";
      this.remotePluginsListView.Size = new System.Drawing.Size(533, 558);
      this.remotePluginsListView.StateImageList = this.imageListForRemoteItems;
      this.remotePluginsListView.TabIndex = 10;
      this.remotePluginsListView.UseCompatibleStateImageBehavior = false;
      this.remotePluginsListView.View = System.Windows.Forms.View.Details;
      this.remotePluginsListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.remotePluginsListView_ItemChecked);
      // 
      // columnHeader1
      // 
      this.columnHeader1.Text = "Name";
      this.columnHeader1.Width = 220;
      // 
      // columnHeader2
      // 
      this.columnHeader2.Text = "Version";
      this.columnHeader2.Width = 120;
      // 
      // columnHeader3
      // 
      this.columnHeader3.Text = "Description";
      this.columnHeader3.Width = 240;
      // 
      // imageListForRemoteItems
      // 
      this.imageListForRemoteItems.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
      this.imageListForRemoteItems.ImageSize = new System.Drawing.Size(13, 13);
      this.imageListForRemoteItems.TransparentColor = System.Drawing.Color.Transparent;
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

    private System.Windows.Forms.ListView remotePluginsListView;
    private System.Windows.Forms.ColumnHeader columnHeader1;
    private System.Windows.Forms.ColumnHeader columnHeader2;
    private System.Windows.Forms.ColumnHeader columnHeader3;
    private System.Windows.Forms.ImageList imageListForRemoteItems;
  }
}
