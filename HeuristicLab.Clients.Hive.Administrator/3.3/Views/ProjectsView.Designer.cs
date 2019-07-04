namespace HeuristicLab.Clients.Hive.Administrator.Views {
  partial class ProjectsView {
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
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.refreshButton = new System.Windows.Forms.Button();
      this.saveProjectButton = new System.Windows.Forms.Button();
      this.removeButton = new System.Windows.Forms.Button();
      this.addButton = new System.Windows.Forms.Button();
      this.projectsTreeView = new System.Windows.Forms.TreeView();
      this.imageList = new System.Windows.Forms.ImageList(this.components);
      this.tabControl = new System.Windows.Forms.TabControl();
      this.detailsTabPage = new System.Windows.Forms.TabPage();
      this.projectView = new HeuristicLab.Clients.Hive.Administrator.Views.ProjectView();
      this.permissionsTabPage = new System.Windows.Forms.TabPage();
      this.projectPermissionsView = new HeuristicLab.Clients.Hive.Administrator.Views.ProjectPermissionsView();
      this.resourcesTabPage = new System.Windows.Forms.TabPage();
      this.projectResourcesView = new HeuristicLab.Clients.Hive.Administrator.Views.ProjectResourcesView();
      this.jobsTabPage = new System.Windows.Forms.TabPage();
      this.projectJobsView = new HeuristicLab.Clients.Hive.Administrator.Views.ProjectJobsView();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.tabControl.SuspendLayout();
      this.detailsTabPage.SuspendLayout();
      this.permissionsTabPage.SuspendLayout();
      this.resourcesTabPage.SuspendLayout();
      this.jobsTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // splitContainer
      // 
      this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.splitContainer.Location = new System.Drawing.Point(3, 3);
      this.splitContainer.Name = "splitContainer";
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.refreshButton);
      this.splitContainer.Panel1.Controls.Add(this.saveProjectButton);
      this.splitContainer.Panel1.Controls.Add(this.removeButton);
      this.splitContainer.Panel1.Controls.Add(this.addButton);
      this.splitContainer.Panel1.Controls.Add(this.projectsTreeView);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.tabControl);
      this.splitContainer.Size = new System.Drawing.Size(847, 547);
      this.splitContainer.SplitterDistance = 249;
      this.splitContainer.TabIndex = 0;
      // 
      // refreshButton
      // 
      this.refreshButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Refresh;
      this.refreshButton.Location = new System.Drawing.Point(3, 3);
      this.refreshButton.Name = "refreshButton";
      this.refreshButton.Size = new System.Drawing.Size(24, 24);
      this.refreshButton.TabIndex = 14;
      this.toolTip.SetToolTip(this.refreshButton, "Fetch list from server");
      this.refreshButton.UseVisualStyleBackColor = true;
      this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
      // 
      // saveProjectButton
      // 
      this.saveProjectButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Save;
      this.saveProjectButton.Location = new System.Drawing.Point(93, 3);
      this.saveProjectButton.Name = "saveProjectButton";
      this.saveProjectButton.Size = new System.Drawing.Size(24, 24);
      this.saveProjectButton.TabIndex = 12;
      this.toolTip.SetToolTip(this.saveProjectButton, "Store project on the server");
      this.saveProjectButton.UseVisualStyleBackColor = true;
      this.saveProjectButton.Click += new System.EventHandler(this.saveProjectButton_Click);
      // 
      // removeButton
      // 
      this.removeButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Remove;
      this.removeButton.Location = new System.Drawing.Point(63, 3);
      this.removeButton.Name = "removeButton";
      this.removeButton.Size = new System.Drawing.Size(24, 24);
      this.removeButton.TabIndex = 11;
      this.toolTip.SetToolTip(this.removeButton, "Remove a project");
      this.removeButton.UseVisualStyleBackColor = true;
      this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
      // 
      // addButton
      // 
      this.addButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Add;
      this.addButton.Location = new System.Drawing.Point(33, 3);
      this.addButton.Name = "addButton";
      this.addButton.Size = new System.Drawing.Size(24, 24);
      this.addButton.TabIndex = 10;
      this.toolTip.SetToolTip(this.addButton, "Add a new project");
      this.addButton.UseVisualStyleBackColor = true;
      this.addButton.Click += new System.EventHandler(this.addButton_Click);
      // 
      // projectsTreeView
      // 
      this.projectsTreeView.AllowDrop = true;
      this.projectsTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.projectsTreeView.ImageIndex = 0;
      this.projectsTreeView.ImageList = this.imageList;
      this.projectsTreeView.Location = new System.Drawing.Point(3, 33);
      this.projectsTreeView.Name = "projectsTreeView";
      this.projectsTreeView.SelectedImageIndex = 0;
      this.projectsTreeView.Size = new System.Drawing.Size(243, 511);
      this.projectsTreeView.TabIndex = 9;
      this.projectsTreeView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.projectsTreeView_ItemDrag);
      this.projectsTreeView.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.projectsTreeView_BeforeSelect);
      this.projectsTreeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.projectsTreeView_MouseDown);
      this.projectsTreeView.DragDrop += new System.Windows.Forms.DragEventHandler(this.projectsTreeView_DragDrop);
      this.projectsTreeView.DragEnter += new System.Windows.Forms.DragEventHandler(this.projectsTreeView_DragEnterOver);
      this.projectsTreeView.DragOver += new System.Windows.Forms.DragEventHandler(this.projectsTreeView_DragEnterOver);
      this.projectsTreeView.QueryContinueDrag += new System.Windows.Forms.QueryContinueDragEventHandler(this.projectsTreeView_QueryContinueDrag);
      // 
      // imageList
      // 
      this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
      this.imageList.ImageSize = new System.Drawing.Size(16, 16);
      this.imageList.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // tabControl
      // 
      this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl.Controls.Add(this.detailsTabPage);
      this.tabControl.Controls.Add(this.permissionsTabPage);
      this.tabControl.Controls.Add(this.resourcesTabPage);
      this.tabControl.Controls.Add(this.jobsTabPage);
      this.tabControl.Location = new System.Drawing.Point(3, 3);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(585, 541);
      this.tabControl.TabIndex = 0;
      // 
      // detailsTabPage
      // 
      this.detailsTabPage.Controls.Add(this.projectView);
      this.detailsTabPage.Location = new System.Drawing.Point(4, 22);
      this.detailsTabPage.Name = "detailsTabPage";
      this.detailsTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.detailsTabPage.Size = new System.Drawing.Size(577, 515);
      this.detailsTabPage.TabIndex = 0;
      this.detailsTabPage.Text = "Details";
      this.detailsTabPage.UseVisualStyleBackColor = true;
      // 
      // projectView
      // 
      this.projectView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.projectView.Caption = "ProjectView";
      this.projectView.Content = null;
      this.projectView.Location = new System.Drawing.Point(6, 6);
      this.projectView.Name = "projectView";
      this.projectView.ReadOnly = false;
      this.projectView.Size = new System.Drawing.Size(565, 503);
      this.projectView.TabIndex = 0;
      // 
      // permissionsTabPage
      // 
      this.permissionsTabPage.Controls.Add(this.projectPermissionsView);
      this.permissionsTabPage.Location = new System.Drawing.Point(4, 22);
      this.permissionsTabPage.Name = "permissionsTabPage";
      this.permissionsTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.permissionsTabPage.Size = new System.Drawing.Size(577, 515);
      this.permissionsTabPage.TabIndex = 1;
      this.permissionsTabPage.Text = "Compute Permissions";
      this.permissionsTabPage.UseVisualStyleBackColor = true;
      // 
      // projectPermissionsView
      // 
      this.projectPermissionsView.Caption = "ProjectView";
      this.projectPermissionsView.Content = null;
      this.projectPermissionsView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.projectPermissionsView.Location = new System.Drawing.Point(3, 3);
      this.projectPermissionsView.Name = "projectPermissionsView";
      this.projectPermissionsView.ReadOnly = false;
      this.projectPermissionsView.Size = new System.Drawing.Size(571, 509);
      this.projectPermissionsView.TabIndex = 0;
      // 
      // resourcesTabPage
      // 
      this.resourcesTabPage.Controls.Add(this.projectResourcesView);
      this.resourcesTabPage.Location = new System.Drawing.Point(4, 22);
      this.resourcesTabPage.Name = "resourcesTabPage";
      this.resourcesTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.resourcesTabPage.Size = new System.Drawing.Size(577, 515);
      this.resourcesTabPage.TabIndex = 2;
      this.resourcesTabPage.Text = "Resources";
      this.resourcesTabPage.UseVisualStyleBackColor = true;
      // 
      // projectResourcesView
      // 
      this.projectResourcesView.Caption = "ProjectView";
      this.projectResourcesView.Content = null;
      this.projectResourcesView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.projectResourcesView.Location = new System.Drawing.Point(3, 3);
      this.projectResourcesView.Name = "projectResourcesView";
      this.projectResourcesView.ReadOnly = false;
      this.projectResourcesView.Size = new System.Drawing.Size(571, 509);
      this.projectResourcesView.TabIndex = 0;
      //
      // jobsTabPage
      //
      this.jobsTabPage.Controls.Add(this.projectJobsView);
      this.jobsTabPage.Location = new System.Drawing.Point(4, 22);
      this.jobsTabPage.Name = "jobsTabPage";
      this.jobsTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.jobsTabPage.Size = new System.Drawing.Size(577, 515);
      this.jobsTabPage.TabIndex = 3;
      this.jobsTabPage.Text = "Jobs";
      this.jobsTabPage.UseVisualStyleBackColor = true;
      //
      // projectJobsView
      //
      this.projectJobsView.Caption = "ProjectView";
      this.projectJobsView.Content = null;
      this.projectJobsView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.projectJobsView.Location = new System.Drawing.Point(3, 3);
      this.projectJobsView.Name = "projectJobsView";
      this.projectJobsView.ReadOnly = false;
      this.projectJobsView.Size = new System.Drawing.Size(571, 509);
      this.projectJobsView.TabIndex = 0;
      // 
      // ProjectsView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.splitContainer);
      this.Name = "ProjectsView";
      this.Size = new System.Drawing.Size(853, 553);
      this.Load += new System.EventHandler(this.ProjectsView_Load);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.tabControl.ResumeLayout(false);
      this.detailsTabPage.ResumeLayout(false);
      this.permissionsTabPage.ResumeLayout(false);
      this.resourcesTabPage.ResumeLayout(false);
      this.jobsTabPage.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.SplitContainer splitContainer;
    private System.Windows.Forms.Button refreshButton;
    private System.Windows.Forms.Button saveProjectButton;
    private System.Windows.Forms.Button removeButton;
    private System.Windows.Forms.Button addButton;
    private System.Windows.Forms.TreeView projectsTreeView;
    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TabPage detailsTabPage;
    private System.Windows.Forms.TabPage permissionsTabPage;
    private ProjectView projectView;
    private System.Windows.Forms.ImageList imageList;
    private System.Windows.Forms.TabPage resourcesTabPage;
    private System.Windows.Forms.TabPage jobsTabPage;
    private ProjectResourcesView projectResourcesView;
    private ProjectPermissionsView projectPermissionsView;
    private ProjectJobsView projectJobsView;
    private System.Windows.Forms.ToolTip toolTip;
  }
}
