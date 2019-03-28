namespace HeuristicLab.Optimization.Views.RunCollectionViews {
  partial class RunCollectionResultAggregationView {
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
      this.aggregatedResultsMatrixView = new HeuristicLab.Data.Views.StringConvertibleMatrixView();
      this.resultsGroupBox = new System.Windows.Forms.GroupBox();
      this.resultsCheckedList = new System.Windows.Forms.CheckedListBox();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.statisticsCheckedList = new System.Windows.Forms.CheckedListBox();
      this.groupingGroupBox = new System.Windows.Forms.GroupBox();
      this.secondCriterionComboBox = new System.Windows.Forms.ComboBox();
      this.label2 = new System.Windows.Forms.Label();
      this.firstCriterionComboBox = new System.Windows.Forms.ComboBox();
      this.label1 = new System.Windows.Forms.Label();
      this.panel1 = new System.Windows.Forms.Panel();
      this.orderByStatisticCheckbox = new System.Windows.Forms.CheckBox();
      this.transposeMatrixCheckBox = new System.Windows.Forms.CheckBox();
      this.resultsGroupBox.SuspendLayout();
      this.groupBox1.SuspendLayout();
      this.groupingGroupBox.SuspendLayout();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // aggregatedResultsMatrixView
      // 
      this.aggregatedResultsMatrixView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.aggregatedResultsMatrixView.Caption = "StringConvertibleMatrix View";
      this.aggregatedResultsMatrixView.Content = null;
      this.aggregatedResultsMatrixView.Location = new System.Drawing.Point(368, 3);
      this.aggregatedResultsMatrixView.Name = "aggregatedResultsMatrixView";
      this.aggregatedResultsMatrixView.ReadOnly = true;
      this.aggregatedResultsMatrixView.ShowRowsAndColumnsTextBox = false;
      this.aggregatedResultsMatrixView.ShowStatisticalInformation = true;
      this.aggregatedResultsMatrixView.Size = new System.Drawing.Size(665, 643);
      this.aggregatedResultsMatrixView.TabIndex = 1;
      // 
      // resultsGroupBox
      // 
      this.resultsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
      this.resultsGroupBox.AutoSize = true;
      this.resultsGroupBox.Controls.Add(this.resultsCheckedList);
      this.resultsGroupBox.Location = new System.Drawing.Point(4, 245);
      this.resultsGroupBox.Name = "resultsGroupBox";
      this.resultsGroupBox.Size = new System.Drawing.Size(349, 349);
      this.resultsGroupBox.TabIndex = 4;
      this.resultsGroupBox.TabStop = false;
      this.resultsGroupBox.Text = "Results";
      // 
      // resultsCheckedList
      // 
      this.resultsCheckedList.CheckOnClick = true;
      this.resultsCheckedList.Dock = System.Windows.Forms.DockStyle.Fill;
      this.resultsCheckedList.FormattingEnabled = true;
      this.resultsCheckedList.Location = new System.Drawing.Point(3, 16);
      this.resultsCheckedList.Name = "resultsCheckedList";
      this.resultsCheckedList.Size = new System.Drawing.Size(343, 330);
      this.resultsCheckedList.TabIndex = 0;
      this.resultsCheckedList.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.resultsCheckedList_ItemCheck);
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.statisticsCheckedList);
      this.groupBox1.Location = new System.Drawing.Point(4, 85);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(349, 154);
      this.groupBox1.TabIndex = 5;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Show";
      // 
      // statisticsCheckedList
      // 
      this.statisticsCheckedList.CheckOnClick = true;
      this.statisticsCheckedList.Dock = System.Windows.Forms.DockStyle.Fill;
      this.statisticsCheckedList.FormattingEnabled = true;
      this.statisticsCheckedList.Location = new System.Drawing.Point(3, 16);
      this.statisticsCheckedList.Name = "statisticsCheckedList";
      this.statisticsCheckedList.Size = new System.Drawing.Size(343, 135);
      this.statisticsCheckedList.TabIndex = 1;
      this.statisticsCheckedList.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.statisticsToCalculateCheckBox_ItemCheck);
      // 
      // groupingGroupBox
      // 
      this.groupingGroupBox.Controls.Add(this.secondCriterionComboBox);
      this.groupingGroupBox.Controls.Add(this.label2);
      this.groupingGroupBox.Controls.Add(this.firstCriterionComboBox);
      this.groupingGroupBox.Controls.Add(this.label1);
      this.groupingGroupBox.Location = new System.Drawing.Point(4, 3);
      this.groupingGroupBox.Name = "groupingGroupBox";
      this.groupingGroupBox.Size = new System.Drawing.Size(349, 76);
      this.groupingGroupBox.TabIndex = 6;
      this.groupingGroupBox.TabStop = false;
      this.groupingGroupBox.Text = "Group by";
      // 
      // secondCriterionComboBox
      // 
      this.secondCriterionComboBox.FormattingEnabled = true;
      this.secondCriterionComboBox.Location = new System.Drawing.Point(98, 44);
      this.secondCriterionComboBox.Name = "secondCriterionComboBox";
      this.secondCriterionComboBox.Size = new System.Drawing.Size(245, 21);
      this.secondCriterionComboBox.TabIndex = 3;
      this.secondCriterionComboBox.SelectedIndexChanged += new System.EventHandler(this.criterionComboBox_SelectedIndexChanged);
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(3, 47);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(84, 13);
      this.label2.TabIndex = 2;
      this.label2.Text = "Second criterion";
      // 
      // firstCriterionComboBox
      // 
      this.firstCriterionComboBox.FormattingEnabled = true;
      this.firstCriterionComboBox.Location = new System.Drawing.Point(98, 17);
      this.firstCriterionComboBox.Name = "firstCriterionComboBox";
      this.firstCriterionComboBox.Size = new System.Drawing.Size(245, 21);
      this.firstCriterionComboBox.TabIndex = 1;
      this.firstCriterionComboBox.SelectedIndexChanged += new System.EventHandler(this.criterionComboBox_SelectedIndexChanged);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(3, 20);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(66, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "First criterion";
      // 
      // panel1
      // 
      this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
      this.panel1.Controls.Add(this.orderByStatisticCheckbox);
      this.panel1.Controls.Add(this.transposeMatrixCheckBox);
      this.panel1.Controls.Add(this.groupingGroupBox);
      this.panel1.Controls.Add(this.groupBox1);
      this.panel1.Controls.Add(this.resultsGroupBox);
      this.panel1.Location = new System.Drawing.Point(3, 3);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(359, 643);
      this.panel1.TabIndex = 0;
      // 
      // orderByStatisticCheckbox
      // 
      this.orderByStatisticCheckbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.orderByStatisticCheckbox.AutoSize = true;
      this.orderByStatisticCheckbox.Location = new System.Drawing.Point(4, 600);
      this.orderByStatisticCheckbox.Name = "orderByStatisticCheckbox";
      this.orderByStatisticCheckbox.Size = new System.Drawing.Size(129, 17);
      this.orderByStatisticCheckbox.TabIndex = 7;
      this.orderByStatisticCheckbox.Text = "Group similar statistics";
      this.orderByStatisticCheckbox.UseVisualStyleBackColor = true;
      this.orderByStatisticCheckbox.CheckedChanged += new System.EventHandler(this.orderByStatisticCheckbox_CheckedChanged);
      // 
      // transposeMatrixCheckBox
      // 
      this.transposeMatrixCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.transposeMatrixCheckBox.AutoSize = true;
      this.transposeMatrixCheckBox.Location = new System.Drawing.Point(4, 623);
      this.transposeMatrixCheckBox.Name = "transposeMatrixCheckBox";
      this.transposeMatrixCheckBox.Size = new System.Drawing.Size(106, 17);
      this.transposeMatrixCheckBox.TabIndex = 7;
      this.transposeMatrixCheckBox.Text = "Transpose matrix";
      this.transposeMatrixCheckBox.UseVisualStyleBackColor = true;
      this.transposeMatrixCheckBox.CheckedChanged += new System.EventHandler(this.transposeMatrixCheckBox_CheckedChanged);
      // 
      // RunCollectionResultAggregationView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.aggregatedResultsMatrixView);
      this.Controls.Add(this.panel1);
      this.Name = "RunCollectionResultAggregationView";
      this.Size = new System.Drawing.Size(1036, 649);
      this.resultsGroupBox.ResumeLayout(false);
      this.groupBox1.ResumeLayout(false);
      this.groupingGroupBox.ResumeLayout(false);
      this.groupingGroupBox.PerformLayout();
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion
    private Data.Views.StringConvertibleMatrixView aggregatedResultsMatrixView;
    private System.Windows.Forms.GroupBox resultsGroupBox;
    private System.Windows.Forms.CheckedListBox resultsCheckedList;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.CheckedListBox statisticsCheckedList;
    private System.Windows.Forms.GroupBox groupingGroupBox;
    private System.Windows.Forms.ComboBox secondCriterionComboBox;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.ComboBox firstCriterionComboBox;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.CheckBox orderByStatisticCheckbox;
    private System.Windows.Forms.CheckBox transposeMatrixCheckBox;
  }
}
