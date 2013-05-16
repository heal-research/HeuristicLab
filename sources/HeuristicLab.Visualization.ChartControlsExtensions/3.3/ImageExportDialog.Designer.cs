#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

namespace HeuristicLab.Visualization.ChartControlsExtensions {
  partial class ImageExportDialog {
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
      this.titleTextBox = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.okButton = new System.Windows.Forms.Button();
      this.label2 = new System.Windows.Forms.Label();
      this.secondaryXTextBox = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.secondaryYTextBox = new System.Windows.Forms.TextBox();
      this.label4 = new System.Windows.Forms.Label();
      this.resolutionComboBox = new System.Windows.Forms.ComboBox();
      this.widthNumericUD = new System.Windows.Forms.NumericUpDown();
      this.label5 = new System.Windows.Forms.Label();
      this.label6 = new System.Windows.Forms.Label();
      this.axisFontSizeComboBox = new System.Windows.Forms.ComboBox();
      this.scalesFontSizeComboBox = new System.Windows.Forms.ComboBox();
      this.cancelButton = new System.Windows.Forms.Button();
      this.label9 = new System.Windows.Forms.Label();
      this.label10 = new System.Windows.Forms.Label();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.showSecondaryYAxisCheckBox = new System.Windows.Forms.CheckBox();
      this.showSecondaryXAxisCheckBox = new System.Windows.Forms.CheckBox();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.showPrimaryYAxisCheckBox = new System.Windows.Forms.CheckBox();
      this.showPrimaryXAxisCheckBox = new System.Windows.Forms.CheckBox();
      this.primaryXTextBox = new System.Windows.Forms.TextBox();
      this.primaryYTextBox = new System.Windows.Forms.TextBox();
      this.label14 = new System.Windows.Forms.Label();
      this.label15 = new System.Windows.Forms.Label();
      this.groupBox3 = new System.Windows.Forms.GroupBox();
      this.label18 = new System.Windows.Forms.Label();
      this.label12 = new System.Windows.Forms.Label();
      this.label11 = new System.Windows.Forms.Label();
      this.label7 = new System.Windows.Forms.Label();
      this.legendFontSizeComboBox = new System.Windows.Forms.ComboBox();
      this.label17 = new System.Windows.Forms.Label();
      this.titleFontSizeComboBox = new System.Windows.Forms.ComboBox();
      this.label16 = new System.Windows.Forms.Label();
      this.heightNumericUD = new System.Windows.Forms.NumericUpDown();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.chartAreaComboBox = new System.Windows.Forms.ComboBox();
      this.togglePreviewCheckBox = new System.Windows.Forms.CheckBox();
      this.lengthUnitComboBox = new System.Windows.Forms.ComboBox();
      this.resolutionUnitComboBox = new System.Windows.Forms.ComboBox();
      this.label8 = new System.Windows.Forms.Label();
      this.previewPictureBox = new System.Windows.Forms.PictureBox();
      this.label19 = new System.Windows.Forms.Label();
      this.label13 = new System.Windows.Forms.Label();
      this.rawImageSizeLabel = new System.Windows.Forms.Label();
      this.previewZoomLabel = new System.Windows.Forms.Label();
      this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.widthNumericUD)).BeginInit();
      this.groupBox1.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.groupBox3.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.heightNumericUD)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.previewPictureBox)).BeginInit();
      this.SuspendLayout();
      // 
      // titleTextBox
      // 
      this.titleTextBox.Location = new System.Drawing.Point(84, 11);
      this.titleTextBox.Name = "titleTextBox";
      this.titleTextBox.Size = new System.Drawing.Size(159, 20);
      this.titleTextBox.TabIndex = 1;
      this.toolTip.SetToolTip(this.titleTextBox, "The title of the chart, will be removed if set to empty.");
      this.titleTextBox.TextChanged += new System.EventHandler(this.titleTextBox_TextChanged);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(6, 22);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(30, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Title:";
      // 
      // okButton
      // 
      this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.okButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.okButton.Location = new System.Drawing.Point(141, 487);
      this.okButton.Name = "okButton";
      this.okButton.Size = new System.Drawing.Size(75, 23);
      this.okButton.TabIndex = 2;
      this.okButton.Text = "Save";
      this.okButton.UseVisualStyleBackColor = true;
      this.okButton.Click += new System.EventHandler(this.okButton_Click);
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(18, 14);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(54, 13);
      this.label2.TabIndex = 0;
      this.label2.Text = "Title Text:";
      // 
      // secondaryXTextBox
      // 
      this.secondaryXTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.secondaryXTextBox.Location = new System.Drawing.Point(72, 27);
      this.secondaryXTextBox.Name = "secondaryXTextBox";
      this.secondaryXTextBox.Size = new System.Drawing.Size(159, 20);
      this.secondaryXTextBox.TabIndex = 1;
      this.toolTip.SetToolTip(this.secondaryXTextBox, "Label of the secondary x-axis");
      this.secondaryXTextBox.TextChanged += new System.EventHandler(this.secondaryXTextBox_TextChanged);
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(6, 30);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(46, 13);
      this.label3.TabIndex = 0;
      this.label3.Text = "X Label:";
      // 
      // secondaryYTextBox
      // 
      this.secondaryYTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.secondaryYTextBox.Location = new System.Drawing.Point(72, 53);
      this.secondaryYTextBox.Name = "secondaryYTextBox";
      this.secondaryYTextBox.Size = new System.Drawing.Size(159, 20);
      this.secondaryYTextBox.TabIndex = 4;
      this.toolTip.SetToolTip(this.secondaryYTextBox, "Label of the secondary y-axis");
      this.secondaryYTextBox.TextChanged += new System.EventHandler(this.secondaryYTextBox_TextChanged);
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(6, 56);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(46, 13);
      this.label4.TabIndex = 3;
      this.label4.Text = "Y Label:";
      // 
      // resolutionComboBox
      // 
      this.resolutionComboBox.FormattingEnabled = true;
      this.resolutionComboBox.Items.AddRange(new object[] {
            "72",
            "96",
            "150",
            "300",
            "600",
            "800",
            "1200"});
      this.resolutionComboBox.Location = new System.Drawing.Point(84, 383);
      this.resolutionComboBox.Name = "resolutionComboBox";
      this.resolutionComboBox.Size = new System.Drawing.Size(83, 21);
      this.resolutionComboBox.TabIndex = 8;
      this.toolTip.SetToolTip(this.resolutionComboBox, "Specify the resolution of the output image, use a minimum of 300dpi if the image " +
        "should appear in a publication.");
      this.resolutionComboBox.TextChanged += new System.EventHandler(this.resolutionComboBox_TextChanged);
      this.resolutionComboBox.Validating += new System.ComponentModel.CancelEventHandler(this.resolutionComboBox_Validating);
      // 
      // widthNumericUD
      // 
      this.widthNumericUD.DecimalPlaces = 2;
      this.widthNumericUD.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
      this.widthNumericUD.Location = new System.Drawing.Point(84, 410);
      this.widthNumericUD.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
      this.widthNumericUD.Name = "widthNumericUD";
      this.widthNumericUD.Size = new System.Drawing.Size(83, 20);
      this.widthNumericUD.TabIndex = 11;
      this.widthNumericUD.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
      this.widthNumericUD.ValueChanged += new System.EventHandler(this.widthNumericUD_ValueChanged);
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(18, 386);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(60, 13);
      this.label5.TabIndex = 7;
      this.label5.Text = "Resolution:";
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(18, 412);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(38, 13);
      this.label6.TabIndex = 10;
      this.label6.Text = "Width:";
      // 
      // axisFontSizeComboBox
      // 
      this.axisFontSizeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.axisFontSizeComboBox.FormattingEnabled = true;
      this.axisFontSizeComboBox.Items.AddRange(new object[] {
            "6",
            "8",
            "10",
            "11",
            "12",
            "14",
            "16",
            "18",
            "24",
            "36",
            "72"});
      this.axisFontSizeComboBox.Location = new System.Drawing.Point(72, 46);
      this.axisFontSizeComboBox.Name = "axisFontSizeComboBox";
      this.axisFontSizeComboBox.Size = new System.Drawing.Size(159, 21);
      this.axisFontSizeComboBox.TabIndex = 4;
      this.toolTip.SetToolTip(this.axisFontSizeComboBox, "The font size of the axis labels");
      this.axisFontSizeComboBox.TextChanged += new System.EventHandler(this.axisFontSizeComboBox_TextChanged);
      this.axisFontSizeComboBox.Validating += new System.ComponentModel.CancelEventHandler(this.numericComboBox_Validating);
      // 
      // scalesFontSizeComboBox
      // 
      this.scalesFontSizeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.scalesFontSizeComboBox.FormattingEnabled = true;
      this.scalesFontSizeComboBox.Items.AddRange(new object[] {
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "14",
            "16",
            "18",
            "24",
            "36",
            "72"});
      this.scalesFontSizeComboBox.Location = new System.Drawing.Point(72, 73);
      this.scalesFontSizeComboBox.Name = "scalesFontSizeComboBox";
      this.scalesFontSizeComboBox.Size = new System.Drawing.Size(159, 21);
      this.scalesFontSizeComboBox.TabIndex = 7;
      this.toolTip.SetToolTip(this.scalesFontSizeComboBox, "The font size of the scales");
      this.scalesFontSizeComboBox.TextChanged += new System.EventHandler(this.scalesFontSizeComboBox_TextChanged);
      this.scalesFontSizeComboBox.Validating += new System.ComponentModel.CancelEventHandler(this.numericComboBox_Validating);
      // 
      // cancelButton
      // 
      this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.cancelButton.Location = new System.Drawing.Point(222, 487);
      this.cancelButton.Name = "cancelButton";
      this.cancelButton.Size = new System.Drawing.Size(75, 23);
      this.cancelButton.TabIndex = 0;
      this.cancelButton.Text = "Cancel";
      this.cancelButton.UseVisualStyleBackColor = true;
      this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
      // 
      // label9
      // 
      this.label9.AutoSize = true;
      this.label9.Location = new System.Drawing.Point(6, 49);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(29, 13);
      this.label9.TabIndex = 3;
      this.label9.Text = "Axis:";
      // 
      // label10
      // 
      this.label10.AutoSize = true;
      this.label10.Location = new System.Drawing.Point(6, 78);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(42, 13);
      this.label10.TabIndex = 6;
      this.label10.Text = "Scales:";
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.showSecondaryYAxisCheckBox);
      this.groupBox1.Controls.Add(this.showSecondaryXAxisCheckBox);
      this.groupBox1.Controls.Add(this.secondaryXTextBox);
      this.groupBox1.Controls.Add(this.secondaryYTextBox);
      this.groupBox1.Controls.Add(this.label4);
      this.groupBox1.Controls.Add(this.label3);
      this.groupBox1.Location = new System.Drawing.Point(12, 154);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(258, 82);
      this.groupBox1.TabIndex = 5;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Secondary Axis";
      // 
      // showSecondaryYAxisCheckBox
      // 
      this.showSecondaryYAxisCheckBox.AutoSize = true;
      this.showSecondaryYAxisCheckBox.Location = new System.Drawing.Point(237, 56);
      this.showSecondaryYAxisCheckBox.Name = "showSecondaryYAxisCheckBox";
      this.showSecondaryYAxisCheckBox.Size = new System.Drawing.Size(15, 14);
      this.showSecondaryYAxisCheckBox.TabIndex = 5;
      this.toolTip.SetToolTip(this.showSecondaryYAxisCheckBox, "Display");
      this.showSecondaryYAxisCheckBox.UseVisualStyleBackColor = true;
      this.showSecondaryYAxisCheckBox.CheckedChanged += new System.EventHandler(this.showSecondaryYAxisCheckBox_CheckedChanged);
      // 
      // showSecondaryXAxisCheckBox
      // 
      this.showSecondaryXAxisCheckBox.AutoSize = true;
      this.showSecondaryXAxisCheckBox.Location = new System.Drawing.Point(237, 30);
      this.showSecondaryXAxisCheckBox.Name = "showSecondaryXAxisCheckBox";
      this.showSecondaryXAxisCheckBox.Size = new System.Drawing.Size(15, 14);
      this.showSecondaryXAxisCheckBox.TabIndex = 2;
      this.toolTip.SetToolTip(this.showSecondaryXAxisCheckBox, "Display");
      this.showSecondaryXAxisCheckBox.UseVisualStyleBackColor = true;
      this.showSecondaryXAxisCheckBox.CheckedChanged += new System.EventHandler(this.showSecondaryXAxisCheckBox_CheckedChanged);
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.showPrimaryYAxisCheckBox);
      this.groupBox2.Controls.Add(this.showPrimaryXAxisCheckBox);
      this.groupBox2.Controls.Add(this.primaryXTextBox);
      this.groupBox2.Controls.Add(this.primaryYTextBox);
      this.groupBox2.Controls.Add(this.label14);
      this.groupBox2.Controls.Add(this.label15);
      this.groupBox2.Location = new System.Drawing.Point(12, 64);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(258, 84);
      this.groupBox2.TabIndex = 4;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Primary Axis";
      // 
      // showPrimaryYAxisCheckBox
      // 
      this.showPrimaryYAxisCheckBox.AutoSize = true;
      this.showPrimaryYAxisCheckBox.Location = new System.Drawing.Point(237, 56);
      this.showPrimaryYAxisCheckBox.Name = "showPrimaryYAxisCheckBox";
      this.showPrimaryYAxisCheckBox.Size = new System.Drawing.Size(15, 14);
      this.showPrimaryYAxisCheckBox.TabIndex = 5;
      this.toolTip.SetToolTip(this.showPrimaryYAxisCheckBox, "Display");
      this.showPrimaryYAxisCheckBox.UseVisualStyleBackColor = true;
      this.showPrimaryYAxisCheckBox.CheckedChanged += new System.EventHandler(this.showPrimaryYAxisCheckBox_CheckedChanged);
      // 
      // showPrimaryXAxisCheckBox
      // 
      this.showPrimaryXAxisCheckBox.AutoSize = true;
      this.showPrimaryXAxisCheckBox.Location = new System.Drawing.Point(237, 30);
      this.showPrimaryXAxisCheckBox.Name = "showPrimaryXAxisCheckBox";
      this.showPrimaryXAxisCheckBox.Size = new System.Drawing.Size(15, 14);
      this.showPrimaryXAxisCheckBox.TabIndex = 2;
      this.toolTip.SetToolTip(this.showPrimaryXAxisCheckBox, "Display");
      this.showPrimaryXAxisCheckBox.UseVisualStyleBackColor = true;
      this.showPrimaryXAxisCheckBox.CheckedChanged += new System.EventHandler(this.showPrimaryXAxisCheckBox_CheckedChanged);
      // 
      // primaryXTextBox
      // 
      this.primaryXTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.primaryXTextBox.Location = new System.Drawing.Point(72, 27);
      this.primaryXTextBox.Name = "primaryXTextBox";
      this.primaryXTextBox.Size = new System.Drawing.Size(159, 20);
      this.primaryXTextBox.TabIndex = 1;
      this.toolTip.SetToolTip(this.primaryXTextBox, "Label of the primary x-axis");
      this.primaryXTextBox.TextChanged += new System.EventHandler(this.primaryXTextBox_TextChanged);
      // 
      // primaryYTextBox
      // 
      this.primaryYTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.primaryYTextBox.Location = new System.Drawing.Point(72, 53);
      this.primaryYTextBox.Name = "primaryYTextBox";
      this.primaryYTextBox.Size = new System.Drawing.Size(159, 20);
      this.primaryYTextBox.TabIndex = 4;
      this.toolTip.SetToolTip(this.primaryYTextBox, "Label of the primary y-axis");
      this.primaryYTextBox.TextChanged += new System.EventHandler(this.primaryYTextBox_TextChanged);
      // 
      // label14
      // 
      this.label14.AutoSize = true;
      this.label14.Location = new System.Drawing.Point(6, 56);
      this.label14.Name = "label14";
      this.label14.Size = new System.Drawing.Size(46, 13);
      this.label14.TabIndex = 3;
      this.label14.Text = "Y Label:";
      // 
      // label15
      // 
      this.label15.AutoSize = true;
      this.label15.Location = new System.Drawing.Point(6, 30);
      this.label15.Name = "label15";
      this.label15.Size = new System.Drawing.Size(46, 13);
      this.label15.TabIndex = 0;
      this.label15.Text = "X Label:";
      // 
      // groupBox3
      // 
      this.groupBox3.Controls.Add(this.label18);
      this.groupBox3.Controls.Add(this.label12);
      this.groupBox3.Controls.Add(this.label11);
      this.groupBox3.Controls.Add(this.label7);
      this.groupBox3.Controls.Add(this.label1);
      this.groupBox3.Controls.Add(this.label9);
      this.groupBox3.Controls.Add(this.legendFontSizeComboBox);
      this.groupBox3.Controls.Add(this.label17);
      this.groupBox3.Controls.Add(this.scalesFontSizeComboBox);
      this.groupBox3.Controls.Add(this.label10);
      this.groupBox3.Controls.Add(this.axisFontSizeComboBox);
      this.groupBox3.Controls.Add(this.titleFontSizeComboBox);
      this.groupBox3.Location = new System.Drawing.Point(12, 242);
      this.groupBox3.Name = "groupBox3";
      this.groupBox3.Size = new System.Drawing.Size(258, 131);
      this.groupBox3.TabIndex = 6;
      this.groupBox3.TabStop = false;
      this.groupBox3.Text = "Font Size";
      // 
      // label18
      // 
      this.label18.AutoSize = true;
      this.label18.Location = new System.Drawing.Point(234, 105);
      this.label18.Name = "label18";
      this.label18.Size = new System.Drawing.Size(16, 13);
      this.label18.TabIndex = 11;
      this.label18.Text = "pt";
      // 
      // label12
      // 
      this.label12.AutoSize = true;
      this.label12.Location = new System.Drawing.Point(234, 78);
      this.label12.Name = "label12";
      this.label12.Size = new System.Drawing.Size(16, 13);
      this.label12.TabIndex = 8;
      this.label12.Text = "pt";
      // 
      // label11
      // 
      this.label11.AutoSize = true;
      this.label11.Location = new System.Drawing.Point(234, 49);
      this.label11.Name = "label11";
      this.label11.Size = new System.Drawing.Size(16, 13);
      this.label11.TabIndex = 5;
      this.label11.Text = "pt";
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(234, 22);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(16, 13);
      this.label7.TabIndex = 2;
      this.label7.Text = "pt";
      // 
      // legendFontSizeComboBox
      // 
      this.legendFontSizeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.legendFontSizeComboBox.FormattingEnabled = true;
      this.legendFontSizeComboBox.Items.AddRange(new object[] {
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "14",
            "16",
            "18",
            "24",
            "36",
            "72"});
      this.legendFontSizeComboBox.Location = new System.Drawing.Point(72, 100);
      this.legendFontSizeComboBox.Name = "legendFontSizeComboBox";
      this.legendFontSizeComboBox.Size = new System.Drawing.Size(159, 21);
      this.legendFontSizeComboBox.TabIndex = 10;
      this.toolTip.SetToolTip(this.legendFontSizeComboBox, "The font size of the legend text");
      this.legendFontSizeComboBox.TextChanged += new System.EventHandler(this.legendFontSizeComboBox_TextChanged);
      this.legendFontSizeComboBox.Validating += new System.ComponentModel.CancelEventHandler(this.numericComboBox_Validating);
      // 
      // label17
      // 
      this.label17.AutoSize = true;
      this.label17.Location = new System.Drawing.Point(6, 105);
      this.label17.Name = "label17";
      this.label17.Size = new System.Drawing.Size(46, 13);
      this.label17.TabIndex = 9;
      this.label17.Text = "Legend:";
      // 
      // titleFontSizeComboBox
      // 
      this.titleFontSizeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.titleFontSizeComboBox.FormattingEnabled = true;
      this.titleFontSizeComboBox.Items.AddRange(new object[] {
            "6",
            "8",
            "10",
            "11",
            "12",
            "14",
            "16",
            "18",
            "24",
            "36",
            "72"});
      this.titleFontSizeComboBox.Location = new System.Drawing.Point(72, 19);
      this.titleFontSizeComboBox.Name = "titleFontSizeComboBox";
      this.titleFontSizeComboBox.Size = new System.Drawing.Size(159, 21);
      this.titleFontSizeComboBox.TabIndex = 1;
      this.toolTip.SetToolTip(this.titleFontSizeComboBox, "The font size of the title text");
      this.titleFontSizeComboBox.TextChanged += new System.EventHandler(this.titleFontSizeComboBox_TextChanged);
      this.titleFontSizeComboBox.Validating += new System.ComponentModel.CancelEventHandler(this.numericComboBox_Validating);
      // 
      // label16
      // 
      this.label16.AutoSize = true;
      this.label16.Location = new System.Drawing.Point(18, 438);
      this.label16.Name = "label16";
      this.label16.Size = new System.Drawing.Size(41, 13);
      this.label16.TabIndex = 12;
      this.label16.Text = "Height:";
      // 
      // heightNumericUD
      // 
      this.heightNumericUD.DecimalPlaces = 2;
      this.heightNumericUD.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
      this.heightNumericUD.Location = new System.Drawing.Point(84, 436);
      this.heightNumericUD.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
      this.heightNumericUD.Name = "heightNumericUD";
      this.heightNumericUD.Size = new System.Drawing.Size(83, 20);
      this.heightNumericUD.TabIndex = 13;
      this.heightNumericUD.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
      this.heightNumericUD.ValueChanged += new System.EventHandler(this.heightNumericUD_ValueChanged);
      // 
      // splitContainer
      // 
      this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
      this.splitContainer.IsSplitterFixed = true;
      this.splitContainer.Location = new System.Drawing.Point(0, 1);
      this.splitContainer.Name = "splitContainer";
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.chartAreaComboBox);
      this.splitContainer.Panel1.Controls.Add(this.togglePreviewCheckBox);
      this.splitContainer.Panel1.Controls.Add(this.titleTextBox);
      this.splitContainer.Panel1.Controls.Add(this.groupBox3);
      this.splitContainer.Panel1.Controls.Add(this.label6);
      this.splitContainer.Panel1.Controls.Add(this.groupBox2);
      this.splitContainer.Panel1.Controls.Add(this.groupBox1);
      this.splitContainer.Panel1.Controls.Add(this.label16);
      this.splitContainer.Panel1.Controls.Add(this.heightNumericUD);
      this.splitContainer.Panel1.Controls.Add(this.widthNumericUD);
      this.splitContainer.Panel1.Controls.Add(this.label5);
      this.splitContainer.Panel1.Controls.Add(this.lengthUnitComboBox);
      this.splitContainer.Panel1.Controls.Add(this.resolutionUnitComboBox);
      this.splitContainer.Panel1.Controls.Add(this.resolutionComboBox);
      this.splitContainer.Panel1.Controls.Add(this.label8);
      this.splitContainer.Panel1.Controls.Add(this.label2);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.previewPictureBox);
      this.splitContainer.Panel2.Controls.Add(this.label19);
      this.splitContainer.Panel2.Controls.Add(this.label13);
      this.splitContainer.Panel2.Controls.Add(this.rawImageSizeLabel);
      this.splitContainer.Panel2.Controls.Add(this.previewZoomLabel);
      this.splitContainer.Size = new System.Drawing.Size(793, 478);
      this.splitContainer.SplitterDistance = 300;
      this.splitContainer.SplitterWidth = 1;
      this.splitContainer.TabIndex = 1;
      // 
      // chartAreaComboBox
      // 
      this.chartAreaComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.chartAreaComboBox.FormattingEnabled = true;
      this.chartAreaComboBox.Location = new System.Drawing.Point(84, 37);
      this.chartAreaComboBox.Name = "chartAreaComboBox";
      this.chartAreaComboBox.Size = new System.Drawing.Size(159, 21);
      this.chartAreaComboBox.TabIndex = 3;
      this.toolTip.SetToolTip(this.chartAreaComboBox, "Select the chart area in case there are multiple.");
      this.chartAreaComboBox.SelectedIndexChanged += new System.EventHandler(this.chartAreaComboBox_SelectedIndexChanged);
      // 
      // togglePreviewCheckBox
      // 
      this.togglePreviewCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
      this.togglePreviewCheckBox.Appearance = System.Windows.Forms.Appearance.Button;
      this.togglePreviewCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.togglePreviewCheckBox.Location = new System.Drawing.Point(277, 3);
      this.togglePreviewCheckBox.Name = "togglePreviewCheckBox";
      this.togglePreviewCheckBox.Size = new System.Drawing.Size(20, 472);
      this.togglePreviewCheckBox.TabIndex = 16;
      this.togglePreviewCheckBox.Text = ">";
      this.toolTip.SetToolTip(this.togglePreviewCheckBox, "Show or hide the preview pane");
      this.togglePreviewCheckBox.UseVisualStyleBackColor = true;
      this.togglePreviewCheckBox.CheckedChanged += new System.EventHandler(this.togglePreviewCheckBox_CheckedChanged);
      // 
      // lengthUnitComboBox
      // 
      this.lengthUnitComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.lengthUnitComboBox.FormattingEnabled = true;
      this.lengthUnitComboBox.Location = new System.Drawing.Point(173, 422);
      this.lengthUnitComboBox.Name = "lengthUnitComboBox";
      this.lengthUnitComboBox.Size = new System.Drawing.Size(70, 21);
      this.lengthUnitComboBox.TabIndex = 14;
      this.lengthUnitComboBox.SelectedIndexChanged += new System.EventHandler(this.lengthUnitComboBox_SelectedIndexChanged);
      // 
      // resolutionUnitComboBox
      // 
      this.resolutionUnitComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.resolutionUnitComboBox.FormattingEnabled = true;
      this.resolutionUnitComboBox.Location = new System.Drawing.Point(173, 383);
      this.resolutionUnitComboBox.Name = "resolutionUnitComboBox";
      this.resolutionUnitComboBox.Size = new System.Drawing.Size(70, 21);
      this.resolutionUnitComboBox.TabIndex = 9;
      this.resolutionUnitComboBox.SelectedIndexChanged += new System.EventHandler(this.resolutionUnitComboBox_SelectedIndexChanged);
      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.Location = new System.Drawing.Point(18, 40);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(32, 13);
      this.label8.TabIndex = 2;
      this.label8.Text = "Area:";
      // 
      // previewPictureBox
      // 
      this.previewPictureBox.Location = new System.Drawing.Point(0, 3);
      this.previewPictureBox.Name = "previewPictureBox";
      this.previewPictureBox.Size = new System.Drawing.Size(492, 453);
      this.previewPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
      this.previewPictureBox.TabIndex = 0;
      this.previewPictureBox.TabStop = false;
      // 
      // label19
      // 
      this.label19.AutoSize = true;
      this.label19.Location = new System.Drawing.Point(85, 459);
      this.label19.Name = "label19";
      this.label19.Size = new System.Drawing.Size(84, 13);
      this.label19.TabIndex = 10;
      this.label19.Text = "Raw image size:";
      // 
      // label13
      // 
      this.label13.AutoSize = true;
      this.label13.Location = new System.Drawing.Point(3, 459);
      this.label13.Name = "label13";
      this.label13.Size = new System.Drawing.Size(37, 13);
      this.label13.TabIndex = 10;
      this.label13.Text = "Zoom:";
      // 
      // rawImageSizeLabel
      // 
      this.rawImageSizeLabel.AutoSize = true;
      this.rawImageSizeLabel.Location = new System.Drawing.Point(175, 459);
      this.rawImageSizeLabel.Name = "rawImageSizeLabel";
      this.rawImageSizeLabel.Size = new System.Drawing.Size(37, 13);
      this.rawImageSizeLabel.TabIndex = 10;
      this.rawImageSizeLabel.Text = "0.00M";
      // 
      // previewZoomLabel
      // 
      this.previewZoomLabel.AutoSize = true;
      this.previewZoomLabel.Location = new System.Drawing.Point(46, 459);
      this.previewZoomLabel.Name = "previewZoomLabel";
      this.previewZoomLabel.Size = new System.Drawing.Size(33, 13);
      this.previewZoomLabel.TabIndex = 10;
      this.previewZoomLabel.Text = "100%";
      // 
      // saveFileDialog
      // 
      this.saveFileDialog.Filter = "Bitmap (*.bmp)|*.bmp|JPEG (*.jpg)|*.jpg|EMF (*.emf)|*.emf|PNG (*.png)|*.png|GIF (" +
    "*.gif)|*.gif|TIFF (*.tif)|*.tif\"";
      this.saveFileDialog.FilterIndex = 2;
      // 
      // ImageExportDialog
      // 
      this.AcceptButton = this.okButton;
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.CancelButton = this.cancelButton;
      this.ClientSize = new System.Drawing.Size(794, 522);
      this.ControlBox = false;
      this.Controls.Add(this.splitContainer);
      this.Controls.Add(this.cancelButton);
      this.Controls.Add(this.okButton);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Name = "ImageExportDialog";
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
      this.Text = "Export Image";
      this.TopMost = true;
      ((System.ComponentModel.ISupportInitialize)(this.widthNumericUD)).EndInit();
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      this.groupBox3.ResumeLayout(false);
      this.groupBox3.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.heightNumericUD)).EndInit();
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel1.PerformLayout();
      this.splitContainer.Panel2.ResumeLayout(false);
      this.splitContainer.Panel2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.previewPictureBox)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TextBox titleTextBox;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button okButton;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox secondaryXTextBox;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox secondaryYTextBox;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.ComboBox resolutionComboBox;
    private System.Windows.Forms.NumericUpDown widthNumericUD;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.ComboBox axisFontSizeComboBox;
    private System.Windows.Forms.ComboBox scalesFontSizeComboBox;
    private System.Windows.Forms.Button cancelButton;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.Label label10;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.TextBox primaryXTextBox;
    private System.Windows.Forms.TextBox primaryYTextBox;
    private System.Windows.Forms.Label label14;
    private System.Windows.Forms.Label label15;
    private System.Windows.Forms.GroupBox groupBox3;
    private System.Windows.Forms.Label label16;
    private System.Windows.Forms.NumericUpDown heightNumericUD;
    private System.Windows.Forms.SplitContainer splitContainer;
    private System.Windows.Forms.CheckBox togglePreviewCheckBox;
    private System.Windows.Forms.PictureBox previewPictureBox;
    private System.Windows.Forms.Label label12;
    private System.Windows.Forms.Label label11;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.ComboBox titleFontSizeComboBox;
    private System.Windows.Forms.ComboBox lengthUnitComboBox;
    private System.Windows.Forms.ComboBox resolutionUnitComboBox;
    private System.Windows.Forms.ComboBox chartAreaComboBox;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.SaveFileDialog saveFileDialog;
    private System.Windows.Forms.Label label13;
    private System.Windows.Forms.Label previewZoomLabel;
    private System.Windows.Forms.Label label18;
    private System.Windows.Forms.ComboBox legendFontSizeComboBox;
    private System.Windows.Forms.Label label17;
    private System.Windows.Forms.Label label19;
    private System.Windows.Forms.Label rawImageSizeLabel;
    private System.Windows.Forms.CheckBox showSecondaryYAxisCheckBox;
    private System.Windows.Forms.CheckBox showSecondaryXAxisCheckBox;
    private System.Windows.Forms.CheckBox showPrimaryYAxisCheckBox;
    private System.Windows.Forms.CheckBox showPrimaryXAxisCheckBox;
    private System.Windows.Forms.ToolTip toolTip;
  }
}