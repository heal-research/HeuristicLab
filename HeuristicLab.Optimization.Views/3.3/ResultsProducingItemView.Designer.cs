namespace HeuristicLab.Optimization.Views {
  partial class ResultsProducingItemView {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ResultsProducingItemView));
      System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Parameters", System.Windows.Forms.HorizontalAlignment.Left);
      System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Results", System.Windows.Forms.HorizontalAlignment.Left);
      this.parameterResultsGroupBox = new System.Windows.Forms.GroupBox();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.showDetailsCheckBox = new System.Windows.Forms.CheckBox();
      this.sortDescendingButton = new System.Windows.Forms.Button();
      this.sortAscendingButton = new System.Windows.Forms.Button();
      this.removeButton = new System.Windows.Forms.Button();
      this.addButton = new System.Windows.Forms.Button();
      this.listView = new System.Windows.Forms.ListView();
      this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.imageList = new System.Windows.Forms.ImageList(this.components);
      this.detailsGroupBox = new System.Windows.Forms.GroupBox();
      this.viewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.parameterResultsGroupBox.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.detailsGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // parameterResultsGroupBox
      // 
      this.parameterResultsGroupBox.Controls.Add(this.splitContainer);
      this.parameterResultsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.parameterResultsGroupBox.Location = new System.Drawing.Point(0, 0);
      this.parameterResultsGroupBox.Margin = new System.Windows.Forms.Padding(0);
      this.parameterResultsGroupBox.Name = "parameterResultsGroupBox";
      this.parameterResultsGroupBox.Size = new System.Drawing.Size(1066, 649);
      this.parameterResultsGroupBox.TabIndex = 1;
      this.parameterResultsGroupBox.TabStop = false;
      this.parameterResultsGroupBox.Text = "Parameter && Results";
      // 
      // splitContainer
      // 
      this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer.Location = new System.Drawing.Point(3, 16);
      this.splitContainer.Name = "splitContainer";
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.showDetailsCheckBox);
      this.splitContainer.Panel1.Controls.Add(this.sortDescendingButton);
      this.splitContainer.Panel1.Controls.Add(this.sortAscendingButton);
      this.splitContainer.Panel1.Controls.Add(this.removeButton);
      this.splitContainer.Panel1.Controls.Add(this.addButton);
      this.splitContainer.Panel1.Controls.Add(this.listView);
      this.splitContainer.Panel1MinSize = 100;
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.detailsGroupBox);
      this.splitContainer.Size = new System.Drawing.Size(1060, 630);
      this.splitContainer.SplitterDistance = 251;
      this.splitContainer.TabIndex = 2;
      // 
      // showDetailsCheckBox
      // 
      this.showDetailsCheckBox.Appearance = System.Windows.Forms.Appearance.Button;
      this.showDetailsCheckBox.Checked = true;
      this.showDetailsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
      this.showDetailsCheckBox.Image = HeuristicLab.Common.Resources.VSImageLibrary.Properties;
      this.showDetailsCheckBox.Location = new System.Drawing.Point(123, 3);
      this.showDetailsCheckBox.Name = "showDetailsCheckBox";
      this.showDetailsCheckBox.Size = new System.Drawing.Size(24, 24);
      this.showDetailsCheckBox.TabIndex = 9;
      this.showDetailsCheckBox.UseVisualStyleBackColor = true;
      // 
      // sortDescendingButton
      // 
      this.sortDescendingButton.Enabled = false;
      this.sortDescendingButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Sort;
      this.sortDescendingButton.Location = new System.Drawing.Point(33, 3);
      this.sortDescendingButton.Name = "sortDescendingButton";
      this.sortDescendingButton.Size = new System.Drawing.Size(24, 24);
      this.sortDescendingButton.TabIndex = 6;
      this.sortDescendingButton.UseVisualStyleBackColor = true;
      // 
      // sortAscendingButton
      // 
      this.sortAscendingButton.Enabled = false;
      this.sortAscendingButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.SortUp;
      this.sortAscendingButton.Location = new System.Drawing.Point(63, 3);
      this.sortAscendingButton.Name = "sortAscendingButton";
      this.sortAscendingButton.Size = new System.Drawing.Size(24, 24);
      this.sortAscendingButton.TabIndex = 7;
      this.sortAscendingButton.UseVisualStyleBackColor = true;
      // 
      // removeButton
      // 
      this.removeButton.Enabled = false;
      this.removeButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Remove;
      this.removeButton.Location = new System.Drawing.Point(93, 3);
      this.removeButton.Name = "removeButton";
      this.removeButton.Size = new System.Drawing.Size(24, 24);
      this.removeButton.TabIndex = 8;
      this.removeButton.UseVisualStyleBackColor = true;
      // 
      // addButton
      // 
      this.addButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Add;
      this.addButton.Location = new System.Drawing.Point(3, 3);
      this.addButton.Name = "addButton";
      this.addButton.Size = new System.Drawing.Size(24, 24);
      this.addButton.TabIndex = 5;
      this.addButton.UseVisualStyleBackColor = true;
      // 
      // listView
      // 
      this.listView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
      listViewGroup1.Header = "Parameters";
      listViewGroup1.Name = "parametersGroup";
      listViewGroup2.Header = "Results";
      listViewGroup2.Name = "resultsGroup";
      this.listView.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2});
      this.listView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
      this.listView.HideSelection = false;
      this.listView.Location = new System.Drawing.Point(3, 33);
      this.listView.Name = "listView";
      this.listView.Size = new System.Drawing.Size(245, 591);
      this.listView.SmallImageList = this.imageList;
      this.listView.TabIndex = 0;
      this.listView.UseCompatibleStateImageBehavior = false;
      this.listView.View = System.Windows.Forms.View.Details;
      this.listView.SelectedIndexChanged += new System.EventHandler(this.listView_SelectedIndexChanged);
      this.listView.DoubleClick += new System.EventHandler(this.listView_DoubleClick);
      this.listView.Layout += new System.Windows.Forms.LayoutEventHandler(this.listView_Layout);
      this.listView.Resize += new System.EventHandler(this.listView_Resize);
      // 
      // columnHeader1
      // 
      this.columnHeader1.Width = 240;
      // 
      // imageList
      // 
      this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
      this.imageList.ImageSize = new System.Drawing.Size(16, 16);
      this.imageList.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // detailsGroupBox
      // 
      this.detailsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.detailsGroupBox.Controls.Add(this.viewHost);
      this.detailsGroupBox.Location = new System.Drawing.Point(3, 27);
      this.detailsGroupBox.Name = "detailsGroupBox";
      this.detailsGroupBox.Size = new System.Drawing.Size(786, 600);
      this.detailsGroupBox.TabIndex = 1;
      this.detailsGroupBox.TabStop = false;
      this.detailsGroupBox.Text = "Details";
      // 
      // viewHost
      // 
      this.viewHost.Caption = "View";
      this.viewHost.Content = null;
      this.viewHost.Dock = System.Windows.Forms.DockStyle.Fill;
      this.viewHost.Enabled = false;
      this.viewHost.Location = new System.Drawing.Point(3, 16);
      this.viewHost.Name = "viewHost";
      this.viewHost.ReadOnly = false;
      this.viewHost.Size = new System.Drawing.Size(780, 581);
      this.viewHost.TabIndex = 0;
      this.viewHost.ViewsLabelVisible = true;
      this.viewHost.ViewType = null;
      // 
      // ResultsProducingItemView
      // 
      this.Controls.Add(this.parameterResultsGroupBox);
      this.Name = "ResultsProducingItemView";
      this.Size = new System.Drawing.Size(1066, 649);
      this.parameterResultsGroupBox.ResumeLayout(false);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.detailsGroupBox.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion
    protected System.Windows.Forms.ImageList imageList;
    protected System.Windows.Forms.GroupBox parameterResultsGroupBox;
    protected System.Windows.Forms.SplitContainer splitContainer;
    protected System.Windows.Forms.ListView listView;
    protected System.Windows.Forms.GroupBox detailsGroupBox;
    protected MainForm.WindowsForms.ViewHost viewHost;
    protected System.Windows.Forms.ColumnHeader columnHeader1;
    protected System.Windows.Forms.CheckBox showDetailsCheckBox;
    protected System.Windows.Forms.Button sortDescendingButton;
    protected System.Windows.Forms.Button sortAscendingButton;
    protected System.Windows.Forms.Button removeButton;
    protected System.Windows.Forms.Button addButton;
  }
}
