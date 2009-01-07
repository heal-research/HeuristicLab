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
      this.pbClientControl = new System.Windows.Forms.PictureBox();
      this.lvClientControl = new System.Windows.Forms.ListView();
      this.ilClientControl = new System.Windows.Forms.ImageList(this.components);
      this.tpJobControl = new System.Windows.Forms.TabPage();
      this.scJobControl = new System.Windows.Forms.SplitContainer();
      this.tvJobControl = new System.Windows.Forms.TreeView();
      this.lvJobControl = new System.Windows.Forms.ListView();
      this.ilJobControl = new System.Windows.Forms.ImageList(this.components);
      this.tpUserControl = new System.Windows.Forms.TabPage();
      this.scUserControl = new System.Windows.Forms.SplitContainer();
      this.tvUserControl = new System.Windows.Forms.TreeView();
      this.lvUserControl = new System.Windows.Forms.ListView();
      this.ilUserControl = new System.Windows.Forms.ImageList(this.components);
      this.treeView2 = new System.Windows.Forms.TreeView();
      this.listView2 = new System.Windows.Forms.ListView();
      this.timerSyncronize = new System.Windows.Forms.Timer(this.components);
      this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
      this.btnClose = new System.Windows.Forms.Button();
      this.lblClientName = new System.Windows.Forms.Label();
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
      ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
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
      this.jobToolStripMenuItem.Click += new System.EventHandler(this.JobToolStripMenuItem1_Click);
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
      this.userToolStripMenuItem1.Click += new System.EventHandler(this.UserToolStripMenuItem1_Click);
      // 
      // groupToolStripMenuItem2
      // 
      this.groupToolStripMenuItem2.Name = "groupToolStripMenuItem2";
      this.groupToolStripMenuItem2.Size = new System.Drawing.Size(114, 22);
      this.groupToolStripMenuItem2.Text = "Group";
      this.groupToolStripMenuItem2.Click += new System.EventHandler(this.GroupToolStripMenuItem2_Click);
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
      //this.scClientControl.Panel2.Controls.Add(this.plClientDetails);
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
      // 
      // plClientDetails
      // 
      this.plClientDetails.BackColor = System.Drawing.SystemColors.Window;
      this.plClientDetails.Controls.Add(this.lblClientName);
      this.plClientDetails.Controls.Add(this.btnClose);
      this.plClientDetails.Controls.Add(this.pbClientControl);
      this.plClientDetails.Dock = System.Windows.Forms.DockStyle.Fill;
      this.plClientDetails.Location = new System.Drawing.Point(0, 0);
      this.plClientDetails.Name = "plClientDetails";
      this.plClientDetails.Size = new System.Drawing.Size(494, 346);
      this.plClientDetails.TabIndex = 1;
      // 
      // pbClientControl
      // 
      this.pbClientControl.Location = new System.Drawing.Point(3, 4);
      this.pbClientControl.Name = "pbClientControl";
      this.pbClientControl.Size = new System.Drawing.Size(60, 50);
      this.pbClientControl.TabIndex = 0;
      this.pbClientControl.TabStop = false;
      // 
      // lvClientControl
      // 
      this.lvClientControl.AllowDrop = true;
      this.lvClientControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.lvClientControl.ImeMode = System.Windows.Forms.ImeMode.Off;
      this.lvClientControl.LargeImageList = this.ilClientControl;
      this.lvClientControl.Location = new System.Drawing.Point(0, 0);
      this.lvClientControl.Name = "lvClientControl";
      this.lvClientControl.Size = new System.Drawing.Size(494, 346);
      this.lvClientControl.TabIndex = 0;
      this.lvClientControl.UseCompatibleStateImageBehavior = false;
      this.lvClientControl.Click += new System.EventHandler(this.OnClientClicked);
      // 
      // ilClientControl
      // 
      this.ilClientControl.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilClientControl.ImageStream")));
      this.ilClientControl.TransparentColor = System.Drawing.Color.Transparent;
      this.ilClientControl.Images.SetKeyName(0, "monitor-green.png");
      this.ilClientControl.Images.SetKeyName(1, "monitor-orange.png");
      this.ilClientControl.Images.SetKeyName(2, "monitor-red.png");
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
      // 
      // lvJobControl
      // 
      this.lvJobControl.AllowDrop = true;
      this.lvJobControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.lvJobControl.LargeImageList = this.ilJobControl;
      this.lvJobControl.Location = new System.Drawing.Point(0, 0);
      this.lvJobControl.Name = "lvJobControl";
      this.lvJobControl.Size = new System.Drawing.Size(494, 346);
      this.lvJobControl.TabIndex = 0;
      this.lvJobControl.UseCompatibleStateImageBehavior = false;
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
      // 
      // ilUserControl
      // 
      this.ilUserControl.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilUserControl.ImageStream")));
      this.ilUserControl.TransparentColor = System.Drawing.Color.Transparent;
      this.ilUserControl.Images.SetKeyName(0, "Users.png");
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
      // 
      // btnClose
      // 
      this.btnClose.Location = new System.Drawing.Point(414, 4);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(75, 23);
      this.btnClose.TabIndex = 1;
      this.btnClose.Text = "Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // lblClientName
      // 
      this.lblClientName.AutoSize = true;
      this.lblClientName.Location = new System.Drawing.Point(79, 19);
      this.lblClientName.Name = "lblClientName";
      this.lblClientName.Size = new System.Drawing.Size(35, 13);
      this.lblClientName.TabIndex = 2;
      this.lblClientName.Text = "label1";
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
      ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
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
    private System.Windows.Forms.BindingSource bindingSource1;
    private System.Windows.Forms.Panel plClientDetails;
    private System.Windows.Forms.PictureBox pbClientControl;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.Label lblClientName;
  }
}