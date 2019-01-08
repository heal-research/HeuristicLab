namespace HeuristicLab.Clients.Hive.Administrator.Views {
  partial class ProjectPermissionsView {
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
      this.inheritButton = new System.Windows.Forms.Button();
      this.saveButton = new System.Windows.Forms.Button();
      this.imageList = new System.Windows.Forms.ImageList(this.components);
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.permissionsGroupBox = new System.Windows.Forms.GroupBox();
      this.treeView = new Hive.Views.TreeView.NoDoubleClickTreeView();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.detailsGroupBox = new System.Windows.Forms.GroupBox();
      this.detailsViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.permissionsGroupBox.SuspendLayout();
      this.detailsGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // refreshButton
      // 
      this.refreshButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Refresh;
      this.refreshButton.Location = new System.Drawing.Point(3, 3);
      this.refreshButton.Name = "refreshButton";
      this.refreshButton.Size = new System.Drawing.Size(24, 24);
      this.refreshButton.TabIndex = 1;
      this.toolTip.SetToolTip(this.refreshButton, "Refresh data");
      this.refreshButton.UseVisualStyleBackColor = true;
      this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
      // 
      // inheritButton
      // 
      this.inheritButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.OrgChart;
      this.inheritButton.Location = new System.Drawing.Point(33, 3);
      this.inheritButton.Name = "inheritButton";
      this.inheritButton.Size = new System.Drawing.Size(24, 24);
      this.inheritButton.TabIndex = 2;
      this.toolTip.SetToolTip(this.inheritButton, "Save and hand down permission settings to all descendant projects");
      this.inheritButton.UseVisualStyleBackColor = true;
      this.inheritButton.Click += new System.EventHandler(this.inheritButton_Click);
      // 
      // saveButton
      // 
      this.saveButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Save;
      this.saveButton.Location = new System.Drawing.Point(63, 3);
      this.saveButton.Name = "saveButton";
      this.saveButton.Size = new System.Drawing.Size(24, 24);
      this.saveButton.TabIndex = 3;
      this.toolTip.SetToolTip(this.saveButton, "Save permission settings");
      this.saveButton.UseVisualStyleBackColor = true;
      this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
      // 
      // imageList
      // 
      this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
      this.imageList.ImageSize = new System.Drawing.Size(16, 16);
      this.imageList.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // splitContainer
      // 
      this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.splitContainer.Location = new System.Drawing.Point(3, 33);
      this.splitContainer.Name = "splitContainer";
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.permissionsGroupBox);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.detailsGroupBox);
      this.splitContainer.Size = new System.Drawing.Size(824, 458);
      this.splitContainer.SplitterDistance = 274;
      this.splitContainer.TabIndex = 6;
      // 
      // permissionsGroupBox
      // 
      this.permissionsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.permissionsGroupBox.Controls.Add(this.treeView);
      this.permissionsGroupBox.Location = new System.Drawing.Point(3, 3);
      this.permissionsGroupBox.Name = "permissionsGroupBox";
      this.permissionsGroupBox.Size = new System.Drawing.Size(268, 452);
      this.permissionsGroupBox.TabIndex = 4;
      this.permissionsGroupBox.TabStop = false;
      this.permissionsGroupBox.Text = "Compute Permissions (Assigned + Included)";
      // 
      // treeView
      // 
      this.treeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.treeView.CheckBoxes = true;
      this.treeView.ImageIndex = 0;
      this.treeView.ImageList = this.imageList;
      this.treeView.Location = new System.Drawing.Point(6, 19);
      this.treeView.Name = "treeView";
      this.treeView.SelectedImageIndex = 0;
      this.treeView.Size = new System.Drawing.Size(256, 427);
      this.treeView.TabIndex = 5;
      this.treeView.BeforeCheck += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView_BeforeCheck);
      this.treeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterCheck);
      this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
      // 
      // detailsGroupBox
      // 
      this.detailsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.detailsGroupBox.Controls.Add(this.detailsViewHost);
      this.detailsGroupBox.Location = new System.Drawing.Point(3, 3);
      this.detailsGroupBox.Name = "detailsGroupBox";
      this.detailsGroupBox.Size = new System.Drawing.Size(540, 452);
      this.detailsGroupBox.TabIndex = 6;
      this.detailsGroupBox.TabStop = false;
      this.detailsGroupBox.Text = "Details";
      // 
      // detailsViewHost
      // 
      this.detailsViewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.detailsViewHost.Caption = "View";
      this.detailsViewHost.Content = null;
      this.detailsViewHost.Enabled = false;
      this.detailsViewHost.Location = new System.Drawing.Point(6, 19);
      this.detailsViewHost.Name = "detailsViewHost";
      this.detailsViewHost.ReadOnly = true;
      this.detailsViewHost.Size = new System.Drawing.Size(528, 427);
      this.detailsViewHost.TabIndex = 7;
      this.detailsViewHost.ViewsLabelVisible = true;
      this.detailsViewHost.ViewType = null;
      // 
      // ProjectPermissionsView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.splitContainer);
      this.Controls.Add(this.saveButton);
      this.Controls.Add(this.inheritButton);
      this.Controls.Add(this.refreshButton);
      this.Name = "ProjectPermissionsView";
      this.Size = new System.Drawing.Size(830, 494);
      this.Load += new System.EventHandler(this.ProjectPermissionsView_Load);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.permissionsGroupBox.ResumeLayout(false);
      this.detailsGroupBox.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion
    private System.Windows.Forms.Button refreshButton;
    private System.Windows.Forms.Button inheritButton;
    private System.Windows.Forms.Button saveButton;
    private System.Windows.Forms.ImageList imageList;
    private System.Windows.Forms.SplitContainer splitContainer;
    private System.Windows.Forms.GroupBox permissionsGroupBox;
    private HeuristicLab.Clients.Hive.Views.TreeView.NoDoubleClickTreeView treeView;
    private System.Windows.Forms.ToolTip toolTip;
    private System.Windows.Forms.GroupBox detailsGroupBox;
    private MainForm.WindowsForms.ViewHost detailsViewHost;
  }
}
