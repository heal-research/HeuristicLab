namespace HeuristicLab.CEDMA.Server {
  partial class DispatcherView {
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
      this.targetVariableList = new System.Windows.Forms.CheckedListBox();
      this.inputVariableList = new System.Windows.Forms.CheckedListBox();
      this.targetVariablesLabel = new System.Windows.Forms.Label();
      this.inputVariablesLabel = new System.Windows.Forms.Label();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.learningTaskGroupBox = new System.Windows.Forms.GroupBox();
      this.setAlgorithmDefault = new System.Windows.Forms.Button();
      this.autoregressiveLabel = new System.Windows.Forms.Label();
      this.classificationRadioButton = new System.Windows.Forms.RadioButton();
      this.algorithmsListBox = new System.Windows.Forms.CheckedListBox();
      this.minTimeOffsetLabel = new System.Windows.Forms.Label();
      this.minTimeOffsetTextBox = new System.Windows.Forms.TextBox();
      this.maxTimeOffsetLabel = new System.Windows.Forms.Label();
      this.regressionRadioButton = new System.Windows.Forms.RadioButton();
      this.maxTimeOffsetTextBox = new System.Windows.Forms.TextBox();
      this.timeSeriesRadioButton = new System.Windows.Forms.RadioButton();
      this.autoregressiveCheckBox = new System.Windows.Forms.CheckBox();
      this.partitioningGroupBox = new System.Windows.Forms.GroupBox();
      this.validationSamplesEndTextBox = new System.Windows.Forms.TextBox();
      this.trainingLabel = new System.Windows.Forms.Label();
      this.trainingSamplesStartTextBox = new System.Windows.Forms.TextBox();
      this.trainingSamplesEndTextBox = new System.Windows.Forms.TextBox();
      this.validationLabel = new System.Windows.Forms.Label();
      this.testSamplesEndTextBox = new System.Windows.Forms.TextBox();
      this.validationSamplesStartTextBox = new System.Windows.Forms.TextBox();
      this.testSamplesStartTextBox = new System.Windows.Forms.TextBox();
      this.testLabel = new System.Windows.Forms.Label();
      this.setAllButton = new System.Windows.Forms.Button();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.learningTaskGroupBox.SuspendLayout();
      this.partitioningGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // targetVariableList
      // 
      this.targetVariableList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.targetVariableList.FormattingEnabled = true;
      this.targetVariableList.HorizontalScrollbar = true;
      this.targetVariableList.Location = new System.Drawing.Point(3, 16);
      this.targetVariableList.Name = "targetVariableList";
      this.targetVariableList.Size = new System.Drawing.Size(346, 214);
      this.targetVariableList.TabIndex = 0;
      this.targetVariableList.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.targetVariableList_ItemCheck);
      this.targetVariableList.SelectedValueChanged += new System.EventHandler(this.targetVariableList_SelectedValueChanged);
      // 
      // inputVariableList
      // 
      this.inputVariableList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.inputVariableList.FormattingEnabled = true;
      this.inputVariableList.HorizontalScrollbar = true;
      this.inputVariableList.Location = new System.Drawing.Point(2, 16);
      this.inputVariableList.Name = "inputVariableList";
      this.inputVariableList.Size = new System.Drawing.Size(393, 634);
      this.inputVariableList.TabIndex = 1;
      this.inputVariableList.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.inputVariableList_ItemCheck);
      // 
      // targetVariablesLabel
      // 
      this.targetVariablesLabel.AutoSize = true;
      this.targetVariablesLabel.Location = new System.Drawing.Point(0, 0);
      this.targetVariablesLabel.Name = "targetVariablesLabel";
      this.targetVariablesLabel.Size = new System.Drawing.Size(86, 13);
      this.targetVariablesLabel.TabIndex = 2;
      this.targetVariablesLabel.Text = "Target variables:";
      // 
      // inputVariablesLabel
      // 
      this.inputVariablesLabel.AutoSize = true;
      this.inputVariablesLabel.Location = new System.Drawing.Point(3, 0);
      this.inputVariablesLabel.Name = "inputVariablesLabel";
      this.inputVariablesLabel.Size = new System.Drawing.Size(79, 13);
      this.inputVariablesLabel.TabIndex = 3;
      this.inputVariablesLabel.Text = "Input variables:";
      // 
      // splitContainer
      // 
      this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer.Location = new System.Drawing.Point(0, 0);
      this.splitContainer.Name = "splitContainer";
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.learningTaskGroupBox);
      this.splitContainer.Panel1.Controls.Add(this.partitioningGroupBox);
      this.splitContainer.Panel1.Controls.Add(this.targetVariablesLabel);
      this.splitContainer.Panel1.Controls.Add(this.targetVariableList);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.setAllButton);
      this.splitContainer.Panel2.Controls.Add(this.inputVariablesLabel);
      this.splitContainer.Panel2.Controls.Add(this.inputVariableList);
      this.splitContainer.Size = new System.Drawing.Size(754, 688);
      this.splitContainer.SplitterDistance = 355;
      this.splitContainer.SplitterWidth = 1;
      this.splitContainer.TabIndex = 4;
      // 
      // learningTaskGroupBox
      // 
      this.learningTaskGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.learningTaskGroupBox.Controls.Add(this.setAlgorithmDefault);
      this.learningTaskGroupBox.Controls.Add(this.autoregressiveLabel);
      this.learningTaskGroupBox.Controls.Add(this.classificationRadioButton);
      this.learningTaskGroupBox.Controls.Add(this.algorithmsListBox);
      this.learningTaskGroupBox.Controls.Add(this.minTimeOffsetLabel);
      this.learningTaskGroupBox.Controls.Add(this.minTimeOffsetTextBox);
      this.learningTaskGroupBox.Controls.Add(this.maxTimeOffsetLabel);
      this.learningTaskGroupBox.Controls.Add(this.regressionRadioButton);
      this.learningTaskGroupBox.Controls.Add(this.maxTimeOffsetTextBox);
      this.learningTaskGroupBox.Controls.Add(this.timeSeriesRadioButton);
      this.learningTaskGroupBox.Controls.Add(this.autoregressiveCheckBox);
      this.learningTaskGroupBox.Enabled = false;
      this.learningTaskGroupBox.Location = new System.Drawing.Point(6, 236);
      this.learningTaskGroupBox.Name = "learningTaskGroupBox";
      this.learningTaskGroupBox.Size = new System.Drawing.Size(343, 321);
      this.learningTaskGroupBox.TabIndex = 36;
      this.learningTaskGroupBox.TabStop = false;
      this.learningTaskGroupBox.Text = "Learning task";
      // 
      // setAlgorithmDefault
      // 
      this.setAlgorithmDefault.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.setAlgorithmDefault.Location = new System.Drawing.Point(6, 292);
      this.setAlgorithmDefault.Name = "setAlgorithmDefault";
      this.setAlgorithmDefault.Size = new System.Drawing.Size(96, 23);
      this.setAlgorithmDefault.TabIndex = 36;
      this.setAlgorithmDefault.Text = "Use as default";
      this.setAlgorithmDefault.UseVisualStyleBackColor = true;
      this.setAlgorithmDefault.Click += new System.EventHandler(this.setAlgorithmDefault_Click);
      // 
      // autoregressiveLabel
      // 
      this.autoregressiveLabel.AutoSize = true;
      this.autoregressiveLabel.Enabled = false;
      this.autoregressiveLabel.Location = new System.Drawing.Point(45, 83);
      this.autoregressiveLabel.Name = "autoregressiveLabel";
      this.autoregressiveLabel.Size = new System.Drawing.Size(80, 13);
      this.autoregressiveLabel.TabIndex = 35;
      this.autoregressiveLabel.Text = "Autoregressive:";
      // 
      // classificationRadioButton
      // 
      this.classificationRadioButton.AutoSize = true;
      this.classificationRadioButton.Location = new System.Drawing.Point(6, 16);
      this.classificationRadioButton.Name = "classificationRadioButton";
      this.classificationRadioButton.Size = new System.Drawing.Size(86, 17);
      this.classificationRadioButton.TabIndex = 28;
      this.classificationRadioButton.Text = "Classification";
      this.classificationRadioButton.UseVisualStyleBackColor = true;
      this.classificationRadioButton.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
      // 
      // algorithmsListBox
      // 
      this.algorithmsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.algorithmsListBox.Enabled = false;
      this.algorithmsListBox.FormattingEnabled = true;
      this.algorithmsListBox.HorizontalScrollbar = true;
      this.algorithmsListBox.Location = new System.Drawing.Point(6, 155);
      this.algorithmsListBox.Name = "algorithmsListBox";
      this.algorithmsListBox.Size = new System.Drawing.Size(331, 124);
      this.algorithmsListBox.TabIndex = 4;
      this.algorithmsListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.algorithmsListBox_ItemCheck);
      // 
      // minTimeOffsetLabel
      // 
      this.minTimeOffsetLabel.AutoSize = true;
      this.minTimeOffsetLabel.Enabled = false;
      this.minTimeOffsetLabel.Location = new System.Drawing.Point(38, 106);
      this.minTimeOffsetLabel.Name = "minTimeOffsetLabel";
      this.minTimeOffsetLabel.Size = new System.Drawing.Size(87, 13);
      this.minTimeOffsetLabel.TabIndex = 31;
      this.minTimeOffsetLabel.Text = "Min. Time Offset:";
      // 
      // minTimeOffsetTextBox
      // 
      this.minTimeOffsetTextBox.Enabled = false;
      this.minTimeOffsetTextBox.Location = new System.Drawing.Point(131, 103);
      this.minTimeOffsetTextBox.Name = "minTimeOffsetTextBox";
      this.minTimeOffsetTextBox.Size = new System.Drawing.Size(96, 20);
      this.minTimeOffsetTextBox.TabIndex = 32;
      this.minTimeOffsetTextBox.Validated += new System.EventHandler(this.timeOffsetTextBox_Validated);
      this.minTimeOffsetTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.timeOffsetTextBox_Validating);
      // 
      // maxTimeOffsetLabel
      // 
      this.maxTimeOffsetLabel.AutoSize = true;
      this.maxTimeOffsetLabel.Enabled = false;
      this.maxTimeOffsetLabel.Location = new System.Drawing.Point(35, 132);
      this.maxTimeOffsetLabel.Name = "maxTimeOffsetLabel";
      this.maxTimeOffsetLabel.Size = new System.Drawing.Size(90, 13);
      this.maxTimeOffsetLabel.TabIndex = 33;
      this.maxTimeOffsetLabel.Text = "Max. Time Offset:";
      // 
      // regressionRadioButton
      // 
      this.regressionRadioButton.AutoSize = true;
      this.regressionRadioButton.Location = new System.Drawing.Point(6, 39);
      this.regressionRadioButton.Name = "regressionRadioButton";
      this.regressionRadioButton.Size = new System.Drawing.Size(78, 17);
      this.regressionRadioButton.TabIndex = 29;
      this.regressionRadioButton.Text = "Regression";
      this.regressionRadioButton.UseVisualStyleBackColor = true;
      this.regressionRadioButton.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
      // 
      // maxTimeOffsetTextBox
      // 
      this.maxTimeOffsetTextBox.Enabled = false;
      this.maxTimeOffsetTextBox.Location = new System.Drawing.Point(131, 129);
      this.maxTimeOffsetTextBox.Name = "maxTimeOffsetTextBox";
      this.maxTimeOffsetTextBox.Size = new System.Drawing.Size(96, 20);
      this.maxTimeOffsetTextBox.TabIndex = 34;
      this.maxTimeOffsetTextBox.Validated += new System.EventHandler(this.timeOffsetTextBox_Validated);
      this.maxTimeOffsetTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.timeOffsetTextBox_Validating);
      // 
      // timeSeriesRadioButton
      // 
      this.timeSeriesRadioButton.AutoSize = true;
      this.timeSeriesRadioButton.Location = new System.Drawing.Point(6, 62);
      this.timeSeriesRadioButton.Name = "timeSeriesRadioButton";
      this.timeSeriesRadioButton.Size = new System.Drawing.Size(119, 17);
      this.timeSeriesRadioButton.TabIndex = 30;
      this.timeSeriesRadioButton.Text = "Time series forecast";
      this.timeSeriesRadioButton.UseVisualStyleBackColor = true;
      this.timeSeriesRadioButton.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
      // 
      // autoregressiveCheckBox
      // 
      this.autoregressiveCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Top;
      this.autoregressiveCheckBox.AutoSize = true;
      this.autoregressiveCheckBox.Enabled = false;
      this.autoregressiveCheckBox.Location = new System.Drawing.Point(139, 83);
      this.autoregressiveCheckBox.Name = "autoregressiveCheckBox";
      this.autoregressiveCheckBox.Size = new System.Drawing.Size(15, 14);
      this.autoregressiveCheckBox.TabIndex = 22;
      this.autoregressiveCheckBox.UseVisualStyleBackColor = true;
      this.autoregressiveCheckBox.CheckedChanged += new System.EventHandler(this.autoregressiveCheckBox_CheckedChanged);
      // 
      // partitioningGroupBox
      // 
      this.partitioningGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.partitioningGroupBox.Controls.Add(this.validationSamplesEndTextBox);
      this.partitioningGroupBox.Controls.Add(this.trainingLabel);
      this.partitioningGroupBox.Controls.Add(this.trainingSamplesStartTextBox);
      this.partitioningGroupBox.Controls.Add(this.trainingSamplesEndTextBox);
      this.partitioningGroupBox.Controls.Add(this.validationLabel);
      this.partitioningGroupBox.Controls.Add(this.testSamplesEndTextBox);
      this.partitioningGroupBox.Controls.Add(this.validationSamplesStartTextBox);
      this.partitioningGroupBox.Controls.Add(this.testSamplesStartTextBox);
      this.partitioningGroupBox.Controls.Add(this.testLabel);
      this.partitioningGroupBox.Enabled = false;
      this.partitioningGroupBox.Location = new System.Drawing.Point(6, 563);
      this.partitioningGroupBox.Name = "partitioningGroupBox";
      this.partitioningGroupBox.Size = new System.Drawing.Size(343, 122);
      this.partitioningGroupBox.TabIndex = 26;
      this.partitioningGroupBox.TabStop = false;
      this.partitioningGroupBox.Text = "Data set partitions:";
      // 
      // validationSamplesEndTextBox
      // 
      this.validationSamplesEndTextBox.Location = new System.Drawing.Point(221, 43);
      this.validationSamplesEndTextBox.Name = "validationSamplesEndTextBox";
      this.validationSamplesEndTextBox.Size = new System.Drawing.Size(96, 20);
      this.validationSamplesEndTextBox.TabIndex = 13;
      this.validationSamplesEndTextBox.Validated += new System.EventHandler(this.samplesTextBox_Validated);
      this.validationSamplesEndTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.samplesTextBox_Validating);
      // 
      // trainingLabel
      // 
      this.trainingLabel.AutoSize = true;
      this.trainingLabel.Location = new System.Drawing.Point(13, 20);
      this.trainingLabel.Name = "trainingLabel";
      this.trainingLabel.Size = new System.Drawing.Size(89, 13);
      this.trainingLabel.TabIndex = 3;
      this.trainingLabel.Text = "Training samples:";
      // 
      // trainingSamplesStartTextBox
      // 
      this.trainingSamplesStartTextBox.Location = new System.Drawing.Point(119, 17);
      this.trainingSamplesStartTextBox.Name = "trainingSamplesStartTextBox";
      this.trainingSamplesStartTextBox.Size = new System.Drawing.Size(96, 20);
      this.trainingSamplesStartTextBox.TabIndex = 4;
      this.trainingSamplesStartTextBox.Validated += new System.EventHandler(this.samplesTextBox_Validated);
      this.trainingSamplesStartTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.samplesTextBox_Validating);
      // 
      // trainingSamplesEndTextBox
      // 
      this.trainingSamplesEndTextBox.Location = new System.Drawing.Point(221, 17);
      this.trainingSamplesEndTextBox.Name = "trainingSamplesEndTextBox";
      this.trainingSamplesEndTextBox.Size = new System.Drawing.Size(96, 20);
      this.trainingSamplesEndTextBox.TabIndex = 10;
      this.trainingSamplesEndTextBox.Validated += new System.EventHandler(this.samplesTextBox_Validated);
      this.trainingSamplesEndTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.samplesTextBox_Validating);
      // 
      // validationLabel
      // 
      this.validationLabel.AutoSize = true;
      this.validationLabel.Location = new System.Drawing.Point(5, 46);
      this.validationLabel.Name = "validationLabel";
      this.validationLabel.Size = new System.Drawing.Size(97, 13);
      this.validationLabel.TabIndex = 11;
      this.validationLabel.Text = "Validation samples:";
      // 
      // testSamplesEndTextBox
      // 
      this.testSamplesEndTextBox.Location = new System.Drawing.Point(221, 68);
      this.testSamplesEndTextBox.Name = "testSamplesEndTextBox";
      this.testSamplesEndTextBox.Size = new System.Drawing.Size(96, 20);
      this.testSamplesEndTextBox.TabIndex = 16;
      this.testSamplesEndTextBox.Validated += new System.EventHandler(this.samplesTextBox_Validated);
      this.testSamplesEndTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.samplesTextBox_Validating);
      // 
      // validationSamplesStartTextBox
      // 
      this.validationSamplesStartTextBox.Location = new System.Drawing.Point(119, 43);
      this.validationSamplesStartTextBox.Name = "validationSamplesStartTextBox";
      this.validationSamplesStartTextBox.Size = new System.Drawing.Size(96, 20);
      this.validationSamplesStartTextBox.TabIndex = 12;
      this.validationSamplesStartTextBox.Validated += new System.EventHandler(this.samplesTextBox_Validated);
      this.validationSamplesStartTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.samplesTextBox_Validating);
      // 
      // testSamplesStartTextBox
      // 
      this.testSamplesStartTextBox.Location = new System.Drawing.Point(119, 68);
      this.testSamplesStartTextBox.Name = "testSamplesStartTextBox";
      this.testSamplesStartTextBox.Size = new System.Drawing.Size(96, 20);
      this.testSamplesStartTextBox.TabIndex = 15;
      this.testSamplesStartTextBox.Validated += new System.EventHandler(this.samplesTextBox_Validated);
      this.testSamplesStartTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.samplesTextBox_Validating);
      // 
      // testLabel
      // 
      this.testLabel.AutoSize = true;
      this.testLabel.Location = new System.Drawing.Point(30, 71);
      this.testLabel.Name = "testLabel";
      this.testLabel.Size = new System.Drawing.Size(72, 13);
      this.testLabel.TabIndex = 14;
      this.testLabel.Text = "Test samples:";
      // 
      // setAllButton
      // 
      this.setAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.setAllButton.Location = new System.Drawing.Point(3, 662);
      this.setAllButton.Name = "setAllButton";
      this.setAllButton.Size = new System.Drawing.Size(91, 23);
      this.setAllButton.TabIndex = 4;
      this.setAllButton.Text = "Use as default";
      this.setAllButton.UseVisualStyleBackColor = true;
      this.setAllButton.Click += new System.EventHandler(this.setAllButton_Click);
      // 
      // DispatcherView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.splitContainer);
      this.Name = "DispatcherView";
      this.Size = new System.Drawing.Size(754, 688);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel1.PerformLayout();
      this.splitContainer.Panel2.ResumeLayout(false);
      this.splitContainer.Panel2.PerformLayout();
      this.splitContainer.ResumeLayout(false);
      this.learningTaskGroupBox.ResumeLayout(false);
      this.learningTaskGroupBox.PerformLayout();
      this.partitioningGroupBox.ResumeLayout(false);
      this.partitioningGroupBox.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.CheckedListBox targetVariableList;
    private System.Windows.Forms.CheckedListBox inputVariableList;
    private System.Windows.Forms.Label targetVariablesLabel;
    private System.Windows.Forms.Label inputVariablesLabel;
    private System.Windows.Forms.SplitContainer splitContainer;
    private System.Windows.Forms.Button setAllButton;
    private System.Windows.Forms.CheckedListBox algorithmsListBox;
    private System.Windows.Forms.GroupBox partitioningGroupBox;
    private System.Windows.Forms.TextBox validationSamplesEndTextBox;
    private System.Windows.Forms.Label trainingLabel;
    private System.Windows.Forms.TextBox trainingSamplesStartTextBox;
    private System.Windows.Forms.TextBox trainingSamplesEndTextBox;
    private System.Windows.Forms.Label validationLabel;
    private System.Windows.Forms.TextBox testSamplesEndTextBox;
    private System.Windows.Forms.TextBox validationSamplesStartTextBox;
    private System.Windows.Forms.TextBox testSamplesStartTextBox;
    private System.Windows.Forms.Label testLabel;
    private System.Windows.Forms.GroupBox learningTaskGroupBox;
    private System.Windows.Forms.Label autoregressiveLabel;
    private System.Windows.Forms.RadioButton classificationRadioButton;
    private System.Windows.Forms.Label minTimeOffsetLabel;
    private System.Windows.Forms.TextBox minTimeOffsetTextBox;
    private System.Windows.Forms.Label maxTimeOffsetLabel;
    private System.Windows.Forms.RadioButton regressionRadioButton;
    private System.Windows.Forms.TextBox maxTimeOffsetTextBox;
    private System.Windows.Forms.RadioButton timeSeriesRadioButton;
    private System.Windows.Forms.CheckBox autoregressiveCheckBox;
    private System.Windows.Forms.Button setAlgorithmDefault;
  }
}
