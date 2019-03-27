namespace HeuristicLab.Problems.DataAnalysis.Views {
  partial class RegressionSolutionPartialDependencePlotView {
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
      this.variableValuesModeComboBox = new System.Windows.Forms.ComboBox();
      this.configSplitContainer = new System.Windows.Forms.SplitContainer();
      this.variableGroupBox = new System.Windows.Forms.GroupBox();
      this.variableListView = new System.Windows.Forms.ListView();
      this.rowSelectGroupBox = new System.Windows.Forms.GroupBox();
      this.rowLabel = new System.Windows.Forms.Label();
      this.rowNrNumericUpDown = new System.Windows.Forms.NumericUpDown();
      this.densityGroupBox = new System.Windows.Forms.GroupBox();
      this.columnsNumericUpDown = new System.Windows.Forms.NumericUpDown();
      this.columnsLabel = new System.Windows.Forms.Label();
      this.densityComboBox = new System.Windows.Forms.ComboBox();
      this.densityLabel = new System.Windows.Forms.Label();
      this.yAxisConfigGroupBox = new System.Windows.Forms.GroupBox();
      this.limitView = new HeuristicLab.Problems.DataAnalysis.Views.DoubleLimitView();
      this.automaticYAxisCheckBox = new System.Windows.Forms.CheckBox();
      this.scrollPanel = new System.Windows.Forms.Panel();
      this.partialDependencePlotTableLayout = new System.Windows.Forms.TableLayoutPanel();
      this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.configSplitContainer)).BeginInit();
      this.configSplitContainer.Panel1.SuspendLayout();
      this.configSplitContainer.Panel2.SuspendLayout();
      this.configSplitContainer.SuspendLayout();
      this.variableGroupBox.SuspendLayout();
      this.rowSelectGroupBox.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.rowNrNumericUpDown)).BeginInit();
      this.densityGroupBox.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.columnsNumericUpDown)).BeginInit();
      this.yAxisConfigGroupBox.SuspendLayout();
      this.scrollPanel.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // variableValuesModeComboBox
      // 
      this.variableValuesModeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.variableValuesModeComboBox.Items.AddRange(new object[] {
            "Row",
            "Mean",
            "Median",
            "Most Common"});
      this.variableValuesModeComboBox.Location = new System.Drawing.Point(10, 19);
      this.variableValuesModeComboBox.Name = "variableValuesModeComboBox";
      this.variableValuesModeComboBox.Size = new System.Drawing.Size(150, 21);
      this.variableValuesModeComboBox.TabIndex = 3;
      this.variableValuesModeComboBox.SelectedValueChanged += new System.EventHandler(this.variableValuesComboBox_SelectedValueChanged);
      this.variableValuesModeComboBox.MouseLeave += new System.EventHandler(this.variableValuesModeComboBox_MouseLeave);
      this.variableValuesModeComboBox.MouseHover += new System.EventHandler(this.variableValuesModeComboBox_MouseHover);
      // 
      // configSplitContainer
      // 
      this.configSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.configSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
      this.configSplitContainer.Location = new System.Drawing.Point(0, 0);
      this.configSplitContainer.Name = "configSplitContainer";
      // 
      // configSplitContainer.Panel1
      // 
      this.configSplitContainer.Panel1.Controls.Add(this.variableGroupBox);
      this.configSplitContainer.Panel1.Controls.Add(this.rowSelectGroupBox);
      this.configSplitContainer.Panel1.Controls.Add(this.densityGroupBox);
      this.configSplitContainer.Panel1.Controls.Add(this.yAxisConfigGroupBox);
      // 
      // configSplitContainer.Panel2
      // 
      this.configSplitContainer.Panel2.Controls.Add(this.scrollPanel);
      this.configSplitContainer.Size = new System.Drawing.Size(715, 648);
      this.configSplitContainer.SplitterDistance = 169;
      this.configSplitContainer.TabIndex = 0;
      this.configSplitContainer.TabStop = false;
      // 
      // variableGroupBox
      // 
      this.variableGroupBox.Controls.Add(this.variableListView);
      this.variableGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.variableGroupBox.Location = new System.Drawing.Point(0, 225);
      this.variableGroupBox.Name = "variableGroupBox";
      this.variableGroupBox.Size = new System.Drawing.Size(169, 423);
      this.variableGroupBox.TabIndex = 1;
      this.variableGroupBox.TabStop = false;
      this.variableGroupBox.Text = "Variables";
      // 
      // variableListView
      // 
      this.variableListView.CheckBoxes = true;
      this.variableListView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.variableListView.Location = new System.Drawing.Point(3, 16);
      this.variableListView.Name = "variableListView";
      this.variableListView.Size = new System.Drawing.Size(163, 404);
      this.variableListView.TabIndex = 0;
      this.variableListView.UseCompatibleStateImageBehavior = false;
      this.variableListView.View = System.Windows.Forms.View.List;
      this.variableListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.variableListView_ItemChecked);
      // 
      // rowSelectGroupBox
      // 
      this.rowSelectGroupBox.Controls.Add(this.variableValuesModeComboBox);
      this.rowSelectGroupBox.Controls.Add(this.rowLabel);
      this.rowSelectGroupBox.Controls.Add(this.rowNrNumericUpDown);
      this.rowSelectGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
      this.rowSelectGroupBox.Location = new System.Drawing.Point(0, 151);
      this.rowSelectGroupBox.Name = "rowSelectGroupBox";
      this.rowSelectGroupBox.Size = new System.Drawing.Size(169, 74);
      this.rowSelectGroupBox.TabIndex = 1;
      this.rowSelectGroupBox.TabStop = false;
      this.rowSelectGroupBox.Text = "Variable Values";
      // 
      // rowLabel
      // 
      this.rowLabel.AutoSize = true;
      this.rowLabel.Location = new System.Drawing.Point(7, 48);
      this.rowLabel.Name = "rowLabel";
      this.rowLabel.Size = new System.Drawing.Size(46, 13);
      this.rowLabel.TabIndex = 2;
      this.rowLabel.Text = "Row Nr.";
      // 
      // rowNrNumericUpDown
      // 
      this.rowNrNumericUpDown.Location = new System.Drawing.Point(66, 46);
      this.rowNrNumericUpDown.Name = "rowNrNumericUpDown";
      this.rowNrNumericUpDown.Size = new System.Drawing.Size(94, 20);
      this.rowNrNumericUpDown.TabIndex = 1;
      this.rowNrNumericUpDown.ValueChanged += new System.EventHandler(this.rowNrNumericUpDown_ValueChanged);
      // 
      // densityGroupBox
      // 
      this.densityGroupBox.Controls.Add(this.columnsNumericUpDown);
      this.densityGroupBox.Controls.Add(this.columnsLabel);
      this.densityGroupBox.Controls.Add(this.densityComboBox);
      this.densityGroupBox.Controls.Add(this.densityLabel);
      this.densityGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
      this.densityGroupBox.Location = new System.Drawing.Point(0, 77);
      this.densityGroupBox.Name = "densityGroupBox";
      this.densityGroupBox.Size = new System.Drawing.Size(169, 74);
      this.densityGroupBox.TabIndex = 3;
      this.densityGroupBox.TabStop = false;
      this.densityGroupBox.Text = "Settings";
      // 
      // columnsNumericUpDown
      // 
      this.columnsNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.columnsNumericUpDown.Location = new System.Drawing.Point(66, 46);
      this.columnsNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.columnsNumericUpDown.Name = "columnsNumericUpDown";
      this.columnsNumericUpDown.Size = new System.Drawing.Size(94, 20);
      this.columnsNumericUpDown.TabIndex = 1;
      this.columnsNumericUpDown.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
      this.columnsNumericUpDown.ValueChanged += new System.EventHandler(this.columnsNumericUpDown_ValueChanged);
      // 
      // columnsLabel
      // 
      this.columnsLabel.AutoSize = true;
      this.columnsLabel.Location = new System.Drawing.Point(7, 49);
      this.columnsLabel.Name = "columnsLabel";
      this.columnsLabel.Size = new System.Drawing.Size(50, 13);
      this.columnsLabel.TabIndex = 0;
      this.columnsLabel.Text = "Columns:";
      // 
      // densityComboBox
      // 
      this.densityComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.densityComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.densityComboBox.FormattingEnabled = true;
      this.densityComboBox.Items.AddRange(new object[] {
            "None",
            "Training",
            "Test",
            "All"});
      this.densityComboBox.Location = new System.Drawing.Point(66, 19);
      this.densityComboBox.Name = "densityComboBox";
      this.densityComboBox.Size = new System.Drawing.Size(94, 21);
      this.densityComboBox.TabIndex = 0;
      this.densityComboBox.SelectedIndexChanged += new System.EventHandler(this.densityComboBox_SelectedIndexChanged);
      // 
      // densityLabel
      // 
      this.densityLabel.AutoSize = true;
      this.densityLabel.Location = new System.Drawing.Point(7, 22);
      this.densityLabel.Name = "densityLabel";
      this.densityLabel.Size = new System.Drawing.Size(45, 13);
      this.densityLabel.TabIndex = 0;
      this.densityLabel.Text = "Density:";
      // 
      // yAxisConfigGroupBox
      // 
      this.yAxisConfigGroupBox.Controls.Add(this.limitView);
      this.yAxisConfigGroupBox.Controls.Add(this.automaticYAxisCheckBox);
      this.yAxisConfigGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
      this.yAxisConfigGroupBox.Location = new System.Drawing.Point(0, 0);
      this.yAxisConfigGroupBox.Name = "yAxisConfigGroupBox";
      this.yAxisConfigGroupBox.Size = new System.Drawing.Size(169, 77);
      this.yAxisConfigGroupBox.TabIndex = 2;
      this.yAxisConfigGroupBox.TabStop = false;
      this.yAxisConfigGroupBox.Text = "Y-Axis";
      // 
      // limitView
      // 
      this.limitView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.limitView.Caption = "DoubleLimit View";
      this.limitView.Content = null;
      this.limitView.Location = new System.Drawing.Point(6, 20);
      this.limitView.Name = "limitView";
      this.limitView.ReadOnly = false;
      this.limitView.Size = new System.Drawing.Size(157, 47);
      this.limitView.TabIndex = 1;
      // 
      // automaticYAxisCheckBox
      // 
      this.automaticYAxisCheckBox.AutoSize = true;
      this.automaticYAxisCheckBox.Location = new System.Drawing.Point(49, -1);
      this.automaticYAxisCheckBox.Name = "automaticYAxisCheckBox";
      this.automaticYAxisCheckBox.Size = new System.Drawing.Size(73, 17);
      this.automaticYAxisCheckBox.TabIndex = 0;
      this.automaticYAxisCheckBox.Text = "Automatic";
      this.automaticYAxisCheckBox.UseVisualStyleBackColor = true;
      this.automaticYAxisCheckBox.CheckedChanged += new System.EventHandler(this.automaticYAxisCheckBox_CheckedChanged);
      // 
      // scrollPanel
      // 
      this.scrollPanel.Controls.Add(this.partialDependencePlotTableLayout);
      this.scrollPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.scrollPanel.Location = new System.Drawing.Point(0, 0);
      this.scrollPanel.Name = "scrollPanel";
      this.scrollPanel.Size = new System.Drawing.Size(542, 648);
      this.scrollPanel.TabIndex = 0;
      // 
      // partialDependencePlotTableLayout
      // 
      this.partialDependencePlotTableLayout.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.partialDependencePlotTableLayout.AutoSize = true;
      this.partialDependencePlotTableLayout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.partialDependencePlotTableLayout.ColumnCount = 1;
      this.partialDependencePlotTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.partialDependencePlotTableLayout.Location = new System.Drawing.Point(0, 0);
      this.partialDependencePlotTableLayout.Name = "partialDependencePlotTableLayout";
      this.partialDependencePlotTableLayout.RowCount = 1;
      this.partialDependencePlotTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.partialDependencePlotTableLayout.Size = new System.Drawing.Size(542, 0);
      this.partialDependencePlotTableLayout.TabIndex = 2;
      // 
      // errorProvider
      // 
      this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
      this.errorProvider.ContainerControl = this;
      // 
      // RegressionSolutionPartialDependencePlotView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.configSplitContainer);
      this.Name = "RegressionSolutionPartialDependencePlotView";
      this.Size = new System.Drawing.Size(715, 648);
      this.configSplitContainer.Panel1.ResumeLayout(false);
      this.configSplitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.configSplitContainer)).EndInit();
      this.configSplitContainer.ResumeLayout(false);
      this.variableGroupBox.ResumeLayout(false);
      this.rowSelectGroupBox.ResumeLayout(false);
      this.rowSelectGroupBox.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.rowNrNumericUpDown)).EndInit();
      this.densityGroupBox.ResumeLayout(false);
      this.densityGroupBox.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.columnsNumericUpDown)).EndInit();
      this.yAxisConfigGroupBox.ResumeLayout(false);
      this.yAxisConfigGroupBox.PerformLayout();
      this.scrollPanel.ResumeLayout(false);
      this.scrollPanel.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ListView variableListView;
    private System.Windows.Forms.TableLayoutPanel partialDependencePlotTableLayout;
    private System.Windows.Forms.GroupBox yAxisConfigGroupBox;
    private System.Windows.Forms.CheckBox automaticYAxisCheckBox;
    private DoubleLimitView limitView;
    private System.Windows.Forms.GroupBox densityGroupBox;
    private System.Windows.Forms.ComboBox densityComboBox;
    private System.Windows.Forms.SplitContainer configSplitContainer;
    private System.Windows.Forms.GroupBox variableGroupBox;
    private System.Windows.Forms.Panel scrollPanel;
    private System.Windows.Forms.Label columnsLabel;
    private System.Windows.Forms.ErrorProvider errorProvider;
    private System.Windows.Forms.Label densityLabel;
    private System.Windows.Forms.NumericUpDown columnsNumericUpDown;
    private System.Windows.Forms.GroupBox rowSelectGroupBox;
    private System.Windows.Forms.ComboBox variableValuesModeComboBox;
    private System.Windows.Forms.Label rowLabel;
    private System.Windows.Forms.NumericUpDown rowNrNumericUpDown;
    private System.Windows.Forms.ToolTip toolTip;
  }
}
