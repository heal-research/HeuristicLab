namespace HeuristicLab.Hive.Server.ServerConsole {
  partial class HiveServerManagementConsole {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HiveServerManagementConsole));
      this.menuStrip1 = new System.Windows.Forms.MenuStrip();
      this.informationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.jobToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.userToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.userToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
      this.groupToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
      this.tcManagementConsole = new System.Windows.Forms.TabControl();
      this.tpClientControl = new System.Windows.Forms.TabPage();
      this.scClientControl = new System.Windows.Forms.SplitContainer();
      this.tvClientControl = new System.Windows.Forms.TreeView();
      this.plClientDetails = new System.Windows.Forms.Panel();
      this.lblState = new System.Windows.Forms.Label();
      this.lblStateClient = new System.Windows.Forms.Label();
      this.lblLogin = new System.Windows.Forms.Label();
      this.lblLoginOn = new System.Windows.Forms.Label();
      this.lblClientName = new System.Windows.Forms.Label();
      this.btnClientDetailClose = new System.Windows.Forms.Button();
      this.pbClientControl = new System.Windows.Forms.PictureBox();
      this.tpJobControl = new System.Windows.Forms.TabPage();
      this.scJobControl = new System.Windows.Forms.SplitContainer();
      this.tvJobControl = new System.Windows.Forms.TreeView();
      this.lvJobControl = new System.Windows.Forms.ListView();
      this.ilJobControl = new System.Windows.Forms.ImageList(this.components);
      this.tpUserControl = new System.Windows.Forms.TabPage();
      this.scUserControl = new System.Windows.Forms.SplitContainer();
      this.tvUserControl = new System.Windows.Forms.TreeView();
      this.plUserDetails = new System.Windows.Forms.Panel();
      this.lblUserName = new System.Windows.Forms.Label();
      this.btnUserControlClose = new System.Windows.Forms.Button();
      this.pbUserControl = new System.Windows.Forms.PictureBox();
      this.lvClientControl = new System.Windows.Forms.ListView();
      this.ilClientControl = new System.Windows.Forms.ImageList(this.components);
      this.lvUserControl = new System.Windows.Forms.ListView();
      this.ilUserControl = new System.Windows.Forms.ImageList(this.components);
      this.plJobDetails = new System.Windows.Forms.Panel();
      this.lvSnapshots = new System.Windows.Forms.ListView();
      this.chJobId = new System.Windows.Forms.ColumnHeader();
      this.chTimeCalculated = new System.Windows.Forms.ColumnHeader();
      this.chProgress = new System.Windows.Forms.ColumnHeader();
      this.lblPriorityJob = new System.Windows.Forms.Label();
      this.lblParentJob = new System.Windows.Forms.Label();
      this.lblClientCalculating = new System.Windows.Forms.Label();
      this.lblJobCalculationEnd = new System.Windows.Forms.Label();
      this.lblJobCalculationBegin = new System.Windows.Forms.Label();
      this.lblJobCreated = new System.Windows.Forms.Label();
      this.lblUserCreatedJob = new System.Windows.Forms.Label();
      this.lblProgress = new System.Windows.Forms.Label();
      this.lblStatus = new System.Windows.Forms.Label();
      this.progressJob = new System.Windows.Forms.ProgressBar();
      this.lblJobName = new System.Windows.Forms.Label();
      this.btnJobDetailClose = new System.Windows.Forms.Button();
      this.pbJobControl = new System.Windows.Forms.PictureBox();
      this.treeView2 = new System.Windows.Forms.TreeView();
      this.listView2 = new System.Windows.Forms.ListView();
      this.timerSyncronize = new System.Windows.Forms.Timer(this.components);
      this.fileSystemWatcher1 = new System.IO.FileSystemWatcher();
      this.menuStrip1.SuspendLayout();
      this.tcManagementConsole.SuspendLayout();
      this.tpClientControl.SuspendLayout();
      this.scClientControl.Panel1.SuspendLayout();
      this.scClientControl.Panel2.SuspendLayout();
      this.scClientControl.SuspendLayout();
      this.plClientDetails.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pbClientControl)).BeginInit();
      this.tpJobControl.SuspendLayout();
      this.scJobControl.Panel1.SuspendLayout();
      this.scJobControl.Panel2.SuspendLayout();
      this.scJobControl.SuspendLayout();
      this.tpUserControl.SuspendLayout();
      this.scUserControl.Panel1.SuspendLayout();
      this.scUserControl.Panel2.SuspendLayout();
      this.scUserControl.SuspendLayout();
      this.plUserDetails.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pbUserControl)).BeginInit();
      this.plJobDetails.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pbJobControl)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).BeginInit();
      this.SuspendLayout();
      // 
      // menuStrip1
      // 
      this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.informationToolStripMenuItem,
            this.addToolStripMenuItem});
      this.menuStrip1.Location = new System.Drawing.Point(0, 0);
      this.menuStrip1.Name = "menuStrip1";
      this.menuStrip1.Size = new System.Drawing.Size(651, 24);
      this.menuStrip1.TabIndex = 0;
      this.menuStrip1.Text = "menuStrip1";
      // 
      // informationToolStripMenuItem
      // 
      this.informationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeToolStripMenuItem});
      this.informationToolStripMenuItem.Name = "informationToolStripMenuItem";
      this.informationToolStripMenuItem.Size = new System.Drawing.Size(81, 20);
      this.informationToolStripMenuItem.Text = "Management";
      // 
      // closeToolStripMenuItem
      // 
      this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
      this.closeToolStripMenuItem.Size = new System.Drawing.Size(111, 22);
      this.closeToolStripMenuItem.Text = "Close";
      this.closeToolStripMenuItem.Click += new System.EventHandler(this.Close_Click);
      // 
      // addToolStripMenuItem
      // 
      this.addToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.jobToolStripMenuItem,
            this.userToolStripMenuItem});
      this.addToolStripMenuItem.Name = "addToolStripMenuItem";
      this.addToolStripMenuItem.Size = new System.Drawing.Size(38, 20);
      this.addToolStripMenuItem.Text = "Add";
      // 
      // jobToolStripMenuItem
      // 
      this.jobToolStripMenuItem.Name = "jobToolStripMenuItem";
      this.jobToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
      this.jobToolStripMenuItem.Text = "Job";
      this.jobToolStripMenuItem.Click += new System.EventHandler(this.AddJob_Click);
      // 
      // userToolStripMenuItem
      // 
      this.userToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.userToolStripMenuItem1,
            this.groupToolStripMenuItem2});
      this.userToolStripMenuItem.Name = "userToolStripMenuItem";
      this.userToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
      this.userToolStripMenuItem.Text = "User";
      // 
      // userToolStripMenuItem1
      // 
      this.userToolStripMenuItem1.Name = "userToolStripMenuItem1";
      this.userToolStripMenuItem1.Size = new System.Drawing.Size(114, 22);
      this.userToolStripMenuItem1.Text = "User";
      this.userToolStripMenuItem1.Click += new System.EventHandler(this.AddUser_Click);
      // 
      // groupToolStripMenuItem2
      // 
      this.groupToolStripMenuItem2.Name = "groupToolStripMenuItem2";
      this.groupToolStripMenuItem2.Size = new System.Drawing.Size(114, 22);
      this.groupToolStripMenuItem2.Text = "Group";
      this.groupToolStripMenuItem2.Click += new System.EventHandler(this.AddUserGroup_Click);
      // 
      // tcManagementConsole
      // 
      this.tcManagementConsole.Controls.Add(this.tpClientControl);
      this.tcManagementConsole.Controls.Add(this.tpJobControl);
      this.tcManagementConsole.Controls.Add(this.tpUserControl);
      this.tcManagementConsole.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tcManagementConsole.Location = new System.Drawing.Point(0, 24);
      this.tcManagementConsole.Name = "tcManagementConsole";
      this.tcManagementConsole.SelectedIndex = 0;
      this.tcManagementConsole.Size = new System.Drawing.Size(651, 378);
      this.tcManagementConsole.TabIndex = 1;
      // 
      // tpClientControl
      // 
      this.tpClientControl.AllowDrop = true;
      this.tpClientControl.Controls.Add(this.scClientControl);
      this.tpClientControl.Location = new System.Drawing.Point(4, 22);
      this.tpClientControl.Name = "tpClientControl";
      this.tpClientControl.Padding = new System.Windows.Forms.Padding(3);
      this.tpClientControl.Size = new System.Drawing.Size(643, 352);
      this.tpClientControl.TabIndex = 0;
      this.tpClientControl.Text = "Client Control";
      this.tpClientControl.UseVisualStyleBackColor = true;
      // 
      // scClientControl
      // 
      this.scClientControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.scClientControl.Location = new System.Drawing.Point(3, 3);
      this.scClientControl.Name = "scClientControl";
      // 
      // scClientControl.Panel1
      // 
      this.scClientControl.Panel1.Controls.Add(this.tvClientControl);
      // 
      // scClientControl.Panel2
      // 
      this.scClientControl.Panel2.Controls.Add(this.lvClientControl);
      this.scClientControl.Size = new System.Drawing.Size(637, 346);
      this.scClientControl.SplitterDistance = 139;
      this.scClientControl.TabIndex = 0;
      // 
      // tvClientControl
      // 
      this.tvClientControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tvClientControl.Location = new System.Drawing.Point(0, 0);
      this.tvClientControl.Name = "tvClientControl";
      this.tvClientControl.Size = new System.Drawing.Size(139, 346);
      this.tvClientControl.TabIndex = 0;
      this.tvClientControl.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.OnTVClientClicked);
      // 
      // plClientDetails
      // 
      this.plClientDetails.BackColor = System.Drawing.SystemColors.Window;
      this.plClientDetails.Controls.Add(this.lblState);
      this.plClientDetails.Controls.Add(this.lblStateClient);
      this.plClientDetails.Controls.Add(this.lblLogin);
      this.plClientDetails.Controls.Add(this.lblLoginOn);
      this.plClientDetails.Controls.Add(this.lblClientName);
      this.plClientDetails.Controls.Add(this.btnClientDetailClose);
      this.plClientDetails.Controls.Add(this.pbClientControl);
      this.plClientDetails.Dock = System.Windows.Forms.DockStyle.Fill;
      this.plClientDetails.Location = new System.Drawing.Point(0, 0);
      this.plClientDetails.Name = "plClientDetails";
      this.plClientDetails.Size = new System.Drawing.Size(494, 346);
      this.plClientDetails.TabIndex = 1;
      // 
      // lblState
      // 
      this.lblState.AutoSize = true;
      this.lblState.Location = new System.Drawing.Point(103, 76);
      this.lblState.Name = "lblState";
      this.lblState.Size = new System.Drawing.Size(35, 13);
      this.lblState.TabIndex = 6;
      this.lblState.Text = "label2";
      // 
      // lblStateClient
      // 
      this.lblStateClient.AutoSize = true;
      this.lblStateClient.Location = new System.Drawing.Point(29, 76);
      this.lblStateClient.Name = "lblStateClient";
      this.lblStateClient.Size = new System.Drawing.Size(32, 13);
      this.lblStateClient.TabIndex = 5;
      this.lblStateClient.Text = "State";
      // 
      // lblLogin
      // 
      this.lblLogin.AutoSize = true;
      this.lblLogin.Location = new System.Drawing.Point(100, 55);
      this.lblLogin.Name = "lblLogin";
      this.lblLogin.Size = new System.Drawing.Size(43, 13);
      this.lblLogin.TabIndex = 4;
      this.lblLogin.Text = "lblLogin";
      // 
      // lblLoginOn
      // 
      this.lblLoginOn.AutoSize = true;
      this.lblLoginOn.Location = new System.Drawing.Point(29, 55);
      this.lblLoginOn.Name = "lblLoginOn";
      this.lblLoginOn.Size = new System.Drawing.Size(65, 13);
      this.lblLoginOn.TabIndex = 3;
      this.lblLoginOn.Text = "logged in on";
      // 
      // lblClientName
      // 
      this.lblClientName.AutoSize = true;
      this.lblClientName.Location = new System.Drawing.Point(41, 14);
      this.lblClientName.Name = "lblClientName";
      this.lblClientName.Size = new System.Drawing.Size(71, 13);
      this.lblClientName.TabIndex = 2;
      this.lblClientName.Text = "lblClientName";
      // 
      // btnClientDetailClose
      // 
      this.btnClientDetailClose.Location = new System.Drawing.Point(414, 4);
      this.btnClientDetailClose.Name = "btnClientDetailClose";
      this.btnClientDetailClose.Size = new System.Drawing.Size(75, 23);
      this.btnClientDetailClose.TabIndex = 1;
      this.btnClientDetailClose.Text = "Close";
      this.btnClientDetailClose.UseVisualStyleBackColor = true;
      this.btnClientDetailClose.Click += new System.EventHandler(this.btnClientClose_Click);
      // 
      // pbClientControl
      // 
      this.pbClientControl.Location = new System.Drawing.Point(3, 4);
      this.pbClientControl.Name = "pbClientControl";
      this.pbClientControl.Size = new System.Drawing.Size(32, 32);
      this.pbClientControl.TabIndex = 0;
      this.pbClientControl.TabStop = false;
      // 
      // tpJobControl
      // 
      this.tpJobControl.Controls.Add(this.scJobControl);
      this.tpJobControl.Location = new System.Drawing.Point(4, 22);
      this.tpJobControl.Name = "tpJobControl";
      this.tpJobControl.Padding = new System.Windows.Forms.Padding(3);
      this.tpJobControl.Size = new System.Drawing.Size(643, 352);
      this.tpJobControl.TabIndex = 1;
      this.tpJobControl.Text = "Job Control";
      this.tpJobControl.UseVisualStyleBackColor = true;
      // 
      // scJobControl
      // 
      this.scJobControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.scJobControl.Location = new System.Drawing.Point(3, 3);
      this.scJobControl.Name = "scJobControl";
      // 
      // scJobControl.Panel1
      // 
      this.scJobControl.Panel1.Controls.Add(this.tvJobControl);
      // 
      // scJobControl.Panel2
      // 
      this.scJobControl.Panel2.Controls.Add(this.lvJobControl);
      this.scJobControl.Size = new System.Drawing.Size(637, 346);
      this.scJobControl.SplitterDistance = 139;
      this.scJobControl.TabIndex = 1;
      // 
      // tvJobControl
      // 
      this.tvJobControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tvJobControl.Location = new System.Drawing.Point(0, 0);
      this.tvJobControl.Name = "tvJobControl";
      this.tvJobControl.Size = new System.Drawing.Size(139, 346);
      this.tvJobControl.TabIndex = 2;
      this.tvJobControl.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.OnTVJobControlClicked);
      // 
      // lvJobControl
      // 
      this.lvJobControl.AllowDrop = true;
      this.lvJobControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.lvJobControl.LargeImageList = this.ilJobControl;
      this.lvJobControl.Location = new System.Drawing.Point(0, 0);
      this.lvJobControl.MultiSelect = false;
      this.lvJobControl.Name = "lvJobControl";
      this.lvJobControl.Size = new System.Drawing.Size(494, 346);
      this.lvJobControl.TabIndex = 0;
      this.lvJobControl.UseCompatibleStateImageBehavior = false;
      this.lvJobControl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lvJobControl_MouseMove);
      this.lvJobControl.Click += new System.EventHandler(this.OnLVJobControlClicked);
      // 
      // ilJobControl
      // 
      this.ilJobControl.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilJobControl.ImageStream")));
      this.ilJobControl.TransparentColor = System.Drawing.Color.Transparent;
      this.ilJobControl.Images.SetKeyName(0, "PlayHS.png");
      // 
      // tpUserControl
      // 
      this.tpUserControl.Controls.Add(this.scUserControl);
      this.tpUserControl.Location = new System.Drawing.Point(4, 22);
      this.tpUserControl.Name = "tpUserControl";
      this.tpUserControl.Padding = new System.Windows.Forms.Padding(3);
      this.tpUserControl.Size = new System.Drawing.Size(643, 352);
      this.tpUserControl.TabIndex = 2;
      this.tpUserControl.Text = "User Control";
      this.tpUserControl.UseVisualStyleBackColor = true;
      // 
      // scUserControl
      // 
      this.scUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.scUserControl.Location = new System.Drawing.Point(3, 3);
      this.scUserControl.Name = "scUserControl";
      // 
      // scUserControl.Panel1
      // 
      this.scUserControl.Panel1.Controls.Add(this.tvUserControl);
      // 
      // scUserControl.Panel2
      // 
      this.scUserControl.Panel2.Controls.Add(this.lvUserControl);
      this.scUserControl.Size = new System.Drawing.Size(637, 346);
      this.scUserControl.SplitterDistance = 139;
      this.scUserControl.TabIndex = 1;
      // 
      // tvUserControl
      // 
      this.tvUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tvUserControl.Location = new System.Drawing.Point(0, 0);
      this.tvUserControl.Name = "tvUserControl";
      this.tvUserControl.Size = new System.Drawing.Size(139, 346);
      this.tvUserControl.TabIndex = 1;
      this.tvUserControl.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.OnTVUserControlClicked);
      // 
      // plUserDetails
      // 
      this.plUserDetails.BackColor = System.Drawing.SystemColors.Window;
      this.plUserDetails.Controls.Add(this.lblUserName);
      this.plUserDetails.Controls.Add(this.btnUserControlClose);
      this.plUserDetails.Controls.Add(this.pbUserControl);
      this.plUserDetails.Dock = System.Windows.Forms.DockStyle.Fill;
      this.plUserDetails.Location = new System.Drawing.Point(0, 0);
      this.plUserDetails.Name = "plUserDetails";
      this.plUserDetails.Size = new System.Drawing.Size(494, 346);
      this.plUserDetails.TabIndex = 2;
      // 
      // lblUserName
      // 
      this.lblUserName.AutoSize = true;
      this.lblUserName.Location = new System.Drawing.Point(41, 13);
      this.lblUserName.Name = "lblUserName";
      this.lblUserName.Size = new System.Drawing.Size(67, 13);
      this.lblUserName.TabIndex = 5;
      this.lblUserName.Text = "lblUserName";
      // 
      // btnUserControlClose
      // 
      this.btnUserControlClose.Location = new System.Drawing.Point(414, 3);
      this.btnUserControlClose.Name = "btnUserControlClose";
      this.btnUserControlClose.Size = new System.Drawing.Size(75, 23);
      this.btnUserControlClose.TabIndex = 4;
      this.btnUserControlClose.Text = "Close";
      this.btnUserControlClose.UseVisualStyleBackColor = true;
      this.btnUserControlClose.Click += new System.EventHandler(this.btnUserControlClose_Click);
      // 
      // pbUserControl
      // 
      this.pbUserControl.Location = new System.Drawing.Point(3, 3);
      this.pbUserControl.Name = "pbUserControl";
      this.pbUserControl.Size = new System.Drawing.Size(32, 32);
      this.pbUserControl.TabIndex = 3;
      this.pbUserControl.TabStop = false;
      // 
      // lvClientControl
      // 
      this.lvClientControl.AllowDrop = true;
      this.lvClientControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.lvClientControl.ImeMode = System.Windows.Forms.ImeMode.Off;
      this.lvClientControl.LargeImageList = this.ilClientControl;
      this.lvClientControl.Location = new System.Drawing.Point(0, 0);
      this.lvClientControl.MultiSelect = false;
      this.lvClientControl.Name = "lvClientControl";
      this.lvClientControl.Size = new System.Drawing.Size(494, 346);
      this.lvClientControl.TabIndex = 0;
      this.lvClientControl.UseCompatibleStateImageBehavior = false;
      this.lvClientControl.Click += new System.EventHandler(this.OnLVClientClicked);
      // 
      // ilClientControl
      // 
      this.ilClientControl.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilClientControl.ImageStream")));
      this.ilClientControl.TransparentColor = System.Drawing.Color.Transparent;
      this.ilClientControl.Images.SetKeyName(0, "monitor-green.png");
      this.ilClientControl.Images.SetKeyName(1, "monitor-orange.png");
      this.ilClientControl.Images.SetKeyName(2, "monitor-red.png");
      // 
      // lvUserControl
      // 
      this.lvUserControl.AllowDrop = true;
      this.lvUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.lvUserControl.LargeImageList = this.ilUserControl;
      this.lvUserControl.Location = new System.Drawing.Point(0, 0);
      this.lvUserControl.Name = "lvUserControl";
      this.lvUserControl.Size = new System.Drawing.Size(494, 346);
      this.lvUserControl.TabIndex = 1;
      this.lvUserControl.UseCompatibleStateImageBehavior = false;
      this.lvUserControl.Click += new System.EventHandler(this.OnLVUserControlClicked);
      // 
      // ilUserControl
      // 
      this.ilUserControl.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilUserControl.ImageStream")));
      this.ilUserControl.TransparentColor = System.Drawing.Color.Transparent;
      this.ilUserControl.Images.SetKeyName(0, "Users.png");
      // 
      // plJobDetails
      // 
      this.plJobDetails.BackColor = System.Drawing.SystemColors.Window;
      this.plJobDetails.Controls.Add(this.lvSnapshots);
      this.plJobDetails.Controls.Add(this.lblPriorityJob);
      this.plJobDetails.Controls.Add(this.lblParentJob);
      this.plJobDetails.Controls.Add(this.lblClientCalculating);
      this.plJobDetails.Controls.Add(this.lblJobCalculationEnd);
      this.plJobDetails.Controls.Add(this.lblJobCalculationBegin);
      this.plJobDetails.Controls.Add(this.lblJobCreated);
      this.plJobDetails.Controls.Add(this.lblUserCreatedJob);
      this.plJobDetails.Controls.Add(this.lblProgress);
      this.plJobDetails.Controls.Add(this.lblStatus);
      this.plJobDetails.Controls.Add(this.progressJob);
      this.plJobDetails.Controls.Add(this.lblJobName);
      this.plJobDetails.Controls.Add(this.btnJobDetailClose);
      this.plJobDetails.Controls.Add(this.pbJobControl);
      this.plJobDetails.Dock = System.Windows.Forms.DockStyle.Fill;
      this.plJobDetails.Location = new System.Drawing.Point(0, 0);
      this.plJobDetails.Name = "plJobDetails";
      this.plJobDetails.Size = new System.Drawing.Size(494, 346);
      this.plJobDetails.TabIndex = 1;
      // 
      // lvSnapshots
      // 
      this.lvSnapshots.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chJobId,
            this.chTimeCalculated,
            this.chProgress});
      this.lvSnapshots.Enabled = false;
      this.lvSnapshots.FullRowSelect = true;
      this.lvSnapshots.GridLines = true;
      this.lvSnapshots.Location = new System.Drawing.Point(20, 271);
      this.lvSnapshots.Name = "lvSnapshots";
      this.lvSnapshots.Size = new System.Drawing.Size(449, 70);
      this.lvSnapshots.TabIndex = 16;
      this.lvSnapshots.UseCompatibleStateImageBehavior = false;
      this.lvSnapshots.View = System.Windows.Forms.View.Details;
      // 
      // chJobId
      // 
      this.chJobId.Text = "Job-ID";
      // 
      // chTimeCalculated
      // 
      this.chTimeCalculated.Text = "Calculated At";
      // 
      // chProgress
      // 
      this.chProgress.Text = "Progress";
      // 
      // lblPriorityJob
      // 
      this.lblPriorityJob.AutoSize = true;
      this.lblPriorityJob.Location = new System.Drawing.Point(17, 213);
      this.lblPriorityJob.Name = "lblPriorityJob";
      this.lblPriorityJob.Size = new System.Drawing.Size(0, 13);
      this.lblPriorityJob.TabIndex = 15;
      // 
      // lblParentJob
      // 
      this.lblParentJob.AutoSize = true;
      this.lblParentJob.Location = new System.Drawing.Point(17, 188);
      this.lblParentJob.Name = "lblParentJob";
      this.lblParentJob.Size = new System.Drawing.Size(0, 13);
      this.lblParentJob.TabIndex = 14;
      // 
      // lblClientCalculating
      // 
      this.lblClientCalculating.AutoSize = true;
      this.lblClientCalculating.Location = new System.Drawing.Point(17, 284);
      this.lblClientCalculating.Name = "lblClientCalculating";
      this.lblClientCalculating.Size = new System.Drawing.Size(0, 13);
      this.lblClientCalculating.TabIndex = 13;
      // 
      // lblJobCalculationEnd
      // 
      this.lblJobCalculationEnd.AutoSize = true;
      this.lblJobCalculationEnd.Location = new System.Drawing.Point(17, 259);
      this.lblJobCalculationEnd.Name = "lblJobCalculationEnd";
      this.lblJobCalculationEnd.Size = new System.Drawing.Size(0, 13);
      this.lblJobCalculationEnd.TabIndex = 12;
      // 
      // lblJobCalculationBegin
      // 
      this.lblJobCalculationBegin.AutoSize = true;
      this.lblJobCalculationBegin.Location = new System.Drawing.Point(17, 236);
      this.lblJobCalculationBegin.Name = "lblJobCalculationBegin";
      this.lblJobCalculationBegin.Size = new System.Drawing.Size(0, 13);
      this.lblJobCalculationBegin.TabIndex = 11;
      // 
      // lblJobCreated
      // 
      this.lblJobCreated.AutoSize = true;
      this.lblJobCreated.Location = new System.Drawing.Point(17, 164);
      this.lblJobCreated.Name = "lblJobCreated";
      this.lblJobCreated.Size = new System.Drawing.Size(0, 13);
      this.lblJobCreated.TabIndex = 10;
      // 
      // lblUserCreatedJob
      // 
      this.lblUserCreatedJob.AutoSize = true;
      this.lblUserCreatedJob.Location = new System.Drawing.Point(17, 142);
      this.lblUserCreatedJob.Name = "lblUserCreatedJob";
      this.lblUserCreatedJob.Size = new System.Drawing.Size(0, 13);
      this.lblUserCreatedJob.TabIndex = 9;
      // 
      // lblProgress
      // 
      this.lblProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.lblProgress.BackColor = System.Drawing.Color.Transparent;
      this.lblProgress.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.lblProgress.Location = new System.Drawing.Point(326, 99);
      this.lblProgress.Name = "lblProgress";
      this.lblProgress.Size = new System.Drawing.Size(143, 13);
      this.lblProgress.TabIndex = 8;
      this.lblProgress.Text = "lblProgress";
      this.lblProgress.TextAlign = System.Drawing.ContentAlignment.TopRight;
      // 
      // lblStatus
      // 
      this.lblStatus.AutoSize = true;
      this.lblStatus.BackColor = System.Drawing.Color.Transparent;
      this.lblStatus.Location = new System.Drawing.Point(14, 57);
      this.lblStatus.Name = "lblStatus";
      this.lblStatus.Size = new System.Drawing.Size(88, 13);
      this.lblStatus.TabIndex = 7;
      this.lblStatus.Text = "Statusinformation";
      // 
      // progressJob
      // 
      this.progressJob.Location = new System.Drawing.Point(17, 73);
      this.progressJob.Name = "progressJob";
      this.progressJob.Size = new System.Drawing.Size(452, 23);
      this.progressJob.TabIndex = 6;
      // 
      // lblJobName
      // 
      this.lblJobName.AutoSize = true;
      this.lblJobName.Location = new System.Drawing.Point(41, 13);
      this.lblJobName.Name = "lblJobName";
      this.lblJobName.Size = new System.Drawing.Size(62, 13);
      this.lblJobName.TabIndex = 5;
      this.lblJobName.Text = "lblJobName";
      // 
      // btnJobDetailClose
      // 
      this.btnJobDetailClose.Location = new System.Drawing.Point(414, 3);
      this.btnJobDetailClose.Name = "btnJobDetailClose";
      this.btnJobDetailClose.Size = new System.Drawing.Size(75, 23);
      this.btnJobDetailClose.TabIndex = 4;
      this.btnJobDetailClose.Text = "Close";
      this.btnJobDetailClose.UseVisualStyleBackColor = true;
      this.btnJobDetailClose.Click += new System.EventHandler(this.btnJobDetailClose_Click);
      // 
      // pbJobControl
      // 
      this.pbJobControl.Location = new System.Drawing.Point(3, 3);
      this.pbJobControl.Name = "pbJobControl";
      this.pbJobControl.Size = new System.Drawing.Size(32, 32);
      this.pbJobControl.TabIndex = 3;
      this.pbJobControl.TabStop = false;
      // 
      // treeView2
      // 
      this.treeView2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.treeView2.LineColor = System.Drawing.Color.Empty;
      this.treeView2.Location = new System.Drawing.Point(0, 0);
      this.treeView2.Name = "treeView2";
      this.treeView2.Size = new System.Drawing.Size(139, 346);
      this.treeView2.TabIndex = 0;
      // 
      // listView2
      // 
      this.listView2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.listView2.Location = new System.Drawing.Point(0, 0);
      this.listView2.Name = "listView2";
      this.listView2.Size = new System.Drawing.Size(494, 346);
      this.listView2.TabIndex = 0;
      this.listView2.UseCompatibleStateImageBehavior = false;
      // 
      // timerSyncronize
      // 
      this.timerSyncronize.Interval = 10000;
      this.timerSyncronize.Tick += new System.EventHandler(this.TickSync);
      // 
      // fileSystemWatcher1
      // 
      this.fileSystemWatcher1.EnableRaisingEvents = true;
      this.fileSystemWatcher1.SynchronizingObject = this;
      // 
      // HiveServerManagementConsole
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(651, 402);
      this.Controls.Add(this.tcManagementConsole);
      this.Controls.Add(this.menuStrip1);
      this.MainMenuStrip = this.menuStrip1;
      this.Name = "HiveServerManagementConsole";
      this.Text = "Management Console";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HiveServerConsoleInformation_FormClosing);
      this.menuStrip1.ResumeLayout(false);
      this.menuStrip1.PerformLayout();
      this.tcManagementConsole.ResumeLayout(false);
      this.tpClientControl.ResumeLayout(false);
      this.scClientControl.Panel1.ResumeLayout(false);
      this.scClientControl.Panel2.ResumeLayout(false);
      this.scClientControl.ResumeLayout(false);
      this.plClientDetails.ResumeLayout(false);
      this.plClientDetails.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pbClientControl)).EndInit();
      this.tpJobControl.ResumeLayout(false);
      this.scJobControl.Panel1.ResumeLayout(false);
      this.scJobControl.Panel2.ResumeLayout(false);
      this.scJobControl.ResumeLayout(false);
      this.tpUserControl.ResumeLayout(false);
      this.scUserControl.Panel1.ResumeLayout(false);
      this.scUserControl.Panel2.ResumeLayout(false);
      this.scUserControl.ResumeLayout(false);
      this.plUserDetails.ResumeLayout(false);
      this.plUserDetails.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pbUserControl)).EndInit();
      this.plJobDetails.ResumeLayout(false);
      this.plJobDetails.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pbJobControl)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.MenuStrip menuStrip1;
    private System.Windows.Forms.ToolStripMenuItem informationToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
    private System.Windows.Forms.TabControl tcManagementConsole;
    private System.Windows.Forms.TabPage tpClientControl;
    private System.Windows.Forms.TabPage tpJobControl;
    private System.Windows.Forms.SplitContainer scClientControl;
    private System.Windows.Forms.TabPage tpUserControl;
    private System.Windows.Forms.TreeView tvClientControl;
    private System.Windows.Forms.ListView lvClientControl;
    private System.Windows.Forms.SplitContainer scJobControl;
    private System.Windows.Forms.ListView lvJobControl;
    private System.Windows.Forms.SplitContainer scUserControl;
    private System.Windows.Forms.TreeView treeView2;
    private System.Windows.Forms.ListView listView2;
    private System.Windows.Forms.TreeView tvJobControl;
    private System.Windows.Forms.TreeView tvUserControl;
    private System.Windows.Forms.ListView lvUserControl;
    private System.Windows.Forms.ImageList ilClientControl;
    private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem jobToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem userToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem userToolStripMenuItem1;
    private System.Windows.Forms.ToolStripMenuItem groupToolStripMenuItem2;
    private System.Windows.Forms.ImageList ilUserControl;
    private System.Windows.Forms.Timer timerSyncronize;
    private System.Windows.Forms.ImageList ilJobControl;
    private System.Windows.Forms.Panel plClientDetails;
    private System.Windows.Forms.PictureBox pbClientControl;
    private System.Windows.Forms.Button btnClientDetailClose;
    private System.Windows.Forms.Label lblClientName;
    private System.Windows.Forms.Label lblLoginOn;
    private System.Windows.Forms.Label lblLogin;
    private System.Windows.Forms.Panel plJobDetails;
    private System.Windows.Forms.Label lblJobName;
    private System.Windows.Forms.Button btnJobDetailClose;
    private System.Windows.Forms.PictureBox pbJobControl;
    private System.Windows.Forms.Panel plUserDetails;
    private System.Windows.Forms.Label lblUserName;
    private System.Windows.Forms.Button btnUserControlClose;
    private System.Windows.Forms.PictureBox pbUserControl;
    private System.Windows.Forms.ProgressBar progressJob;
    private System.IO.FileSystemWatcher fileSystemWatcher1;
    private System.Windows.Forms.Label lblStatus;
    private System.Windows.Forms.Label lblProgress;
    private System.Windows.Forms.Label lblJobCalculationBegin;
    private System.Windows.Forms.Label lblJobCreated;
    private System.Windows.Forms.Label lblUserCreatedJob;
    private System.Windows.Forms.Label lblClientCalculating;
    private System.Windows.Forms.Label lblJobCalculationEnd;
    private System.Windows.Forms.Label lblPriorityJob;
    private System.Windows.Forms.Label lblParentJob;
    private System.Windows.Forms.ListView lvSnapshots;
    private System.Windows.Forms.ColumnHeader chJobId;
    private System.Windows.Forms.ColumnHeader chTimeCalculated;
    private System.Windows.Forms.ColumnHeader chProgress;
    private System.Windows.Forms.Label lblStateClient;
    private System.Windows.Forms.Label lblState;
  }
}