namespace HeuristicLab.Hive.Server.Console {
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
      System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            "",
            "Test"}, -1);
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HiveServerManagementConsole));
      this.menuStrip1 = new System.Windows.Forms.MenuStrip();
      this.informationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.tcManagementConsole = new System.Windows.Forms.TabControl();
      this.tpClientControl = new System.Windows.Forms.TabPage();
      this.scClientControl = new System.Windows.Forms.SplitContainer();
      this.tvClientControl = new System.Windows.Forms.TreeView();
      this.lvClientControl = new System.Windows.Forms.ListView();
      this.imageList1 = new System.Windows.Forms.ImageList(this.components);
      this.tpJobControl = new System.Windows.Forms.TabPage();
      this.scJobControl = new System.Windows.Forms.SplitContainer();
      this.tvJobControl = new System.Windows.Forms.TreeView();
      this.listView3 = new System.Windows.Forms.ListView();
      this.tpUserControl = new System.Windows.Forms.TabPage();
      this.scUserControl = new System.Windows.Forms.SplitContainer();
      this.tvUserControl = new System.Windows.Forms.TreeView();
      this.listView4 = new System.Windows.Forms.ListView();
      this.treeView2 = new System.Windows.Forms.TreeView();
      this.listView2 = new System.Windows.Forms.ListView();
      this.menuStrip1.SuspendLayout();
      this.tcManagementConsole.SuspendLayout();
      this.tpClientControl.SuspendLayout();
      this.scClientControl.Panel1.SuspendLayout();
      this.scClientControl.Panel2.SuspendLayout();
      this.scClientControl.SuspendLayout();
      this.tpJobControl.SuspendLayout();
      this.scJobControl.Panel1.SuspendLayout();
      this.scJobControl.Panel2.SuspendLayout();
      this.scJobControl.SuspendLayout();
      this.tpUserControl.SuspendLayout();
      this.scUserControl.Panel1.SuspendLayout();
      this.scUserControl.Panel2.SuspendLayout();
      this.scUserControl.SuspendLayout();
      this.SuspendLayout();
      // 
      // menuStrip1
      // 
      this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.informationToolStripMenuItem});
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
      this.closeToolStripMenuItem.Click += new System.EventHandler(this.close_Click);
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
      // 
      // lvClientControl
      // 
      this.lvClientControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.lvClientControl.ImeMode = System.Windows.Forms.ImeMode.Off;
      this.lvClientControl.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1});
      this.lvClientControl.LargeImageList = this.imageList1;
      this.lvClientControl.Location = new System.Drawing.Point(0, 0);
      this.lvClientControl.Name = "lvClientControl";
      this.lvClientControl.Size = new System.Drawing.Size(494, 346);
      this.lvClientControl.TabIndex = 0;
      this.lvClientControl.UseCompatibleStateImageBehavior = false;
      // 
      // imageList1
      // 
      this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
      this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
      this.imageList1.Images.SetKeyName(0, "monitor-green.png");
      this.imageList1.Images.SetKeyName(1, "monitor-orange.png");
      this.imageList1.Images.SetKeyName(2, "monitor-red.png");
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
      this.scJobControl.Panel2.Controls.Add(this.listView3);
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
      // listView3
      // 
      this.listView3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.listView3.Location = new System.Drawing.Point(0, 0);
      this.listView3.Name = "listView3";
      this.listView3.Size = new System.Drawing.Size(494, 346);
      this.listView3.TabIndex = 0;
      this.listView3.UseCompatibleStateImageBehavior = false;
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
      this.scUserControl.Panel2.Controls.Add(this.listView4);
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
      // listView4
      // 
      this.listView4.Dock = System.Windows.Forms.DockStyle.Fill;
      this.listView4.Location = new System.Drawing.Point(0, 0);
      this.listView4.Name = "listView4";
      this.listView4.Size = new System.Drawing.Size(494, 346);
      this.listView4.TabIndex = 1;
      this.listView4.UseCompatibleStateImageBehavior = false;
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
      this.tpJobControl.ResumeLayout(false);
      this.scJobControl.Panel1.ResumeLayout(false);
      this.scJobControl.Panel2.ResumeLayout(false);
      this.scJobControl.ResumeLayout(false);
      this.tpUserControl.ResumeLayout(false);
      this.scUserControl.Panel1.ResumeLayout(false);
      this.scUserControl.Panel2.ResumeLayout(false);
      this.scUserControl.ResumeLayout(false);
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
    private System.Windows.Forms.ListView listView3;
    private System.Windows.Forms.SplitContainer scUserControl;
    private System.Windows.Forms.TreeView treeView2;
    private System.Windows.Forms.ListView listView2;
    private System.Windows.Forms.TreeView tvJobControl;
    private System.Windows.Forms.TreeView tvUserControl;
    private System.Windows.Forms.ListView listView4;
    private System.Windows.Forms.ImageList imageList1;
  }
}