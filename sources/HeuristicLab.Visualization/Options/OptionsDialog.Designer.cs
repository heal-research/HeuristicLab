namespace HeuristicLab.Visualization.Options {
  partial class OptionsDialog {
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
      this.OptionsDialogCancelButton = new System.Windows.Forms.Button();
      this.OptionsDialogOkButton = new System.Windows.Forms.Button();
      this.fdFont = new System.Windows.Forms.FontDialog();
      this.tpTitle = new System.Windows.Forms.TabPage();
      this.btnChangeTitleFont = new System.Windows.Forms.Button();
      this.tabPage2 = new System.Windows.Forms.TabPage();
      this.btnChangeLegendFont = new System.Windows.Forms.Button();
      this.legendposition = new System.Windows.Forms.Label();
      this.cbLegendPosition = new System.Windows.Forms.ComboBox();
      this.tabPage1 = new System.Windows.Forms.TabPage();
      this.OptionsDialogSelectColorBt = new System.Windows.Forms.Button();
      this.ColorPreviewTB = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.LinestyleCB = new System.Windows.Forms.ComboBox();
      this.label1 = new System.Windows.Forms.Label();
      this.LineSelectCB = new System.Windows.Forms.ComboBox();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.label4 = new System.Windows.Forms.Label();
      this.LineThicknessCB = new System.Windows.Forms.ComboBox();
      this.label3 = new System.Windows.Forms.Label();
      this.Optionstabs = new System.Windows.Forms.TabControl();
      this.tpXAxis = new System.Windows.Forms.TabPage();
      this.btnChangeXAxisFont = new System.Windows.Forms.Button();
      this.tpYAxes = new System.Windows.Forms.TabPage();
      this.gbxShowYAxis = new System.Windows.Forms.GroupBox();
      this.gbxYAxisClipChangeable = new System.Windows.Forms.GroupBox();
      this.flpShowYAxis = new System.Windows.Forms.FlowLayoutPanel();
      this.flpYAxisClipChangeable = new System.Windows.Forms.FlowLayoutPanel();
      this.tpTitle.SuspendLayout();
      this.tabPage2.SuspendLayout();
      this.tabPage1.SuspendLayout();
      this.groupBox1.SuspendLayout();
      this.Optionstabs.SuspendLayout();
      this.tpXAxis.SuspendLayout();
      this.tpYAxes.SuspendLayout();
      this.gbxShowYAxis.SuspendLayout();
      this.gbxYAxisClipChangeable.SuspendLayout();
      this.SuspendLayout();
      // 
      // OptionsDialogCancelButton
      // 
      this.OptionsDialogCancelButton.Location = new System.Drawing.Point(213, 232);
      this.OptionsDialogCancelButton.Name = "OptionsDialogCancelButton";
      this.OptionsDialogCancelButton.Size = new System.Drawing.Size(75, 23);
      this.OptionsDialogCancelButton.TabIndex = 1;
      this.OptionsDialogCancelButton.Text = "Reset";
      this.OptionsDialogCancelButton.UseVisualStyleBackColor = true;
      this.OptionsDialogCancelButton.Click += new System.EventHandler(this.OptionsDialogResetButton_Click);
      // 
      // OptionsDialogOkButton
      // 
      this.OptionsDialogOkButton.Location = new System.Drawing.Point(132, 232);
      this.OptionsDialogOkButton.Name = "OptionsDialogOkButton";
      this.OptionsDialogOkButton.Size = new System.Drawing.Size(75, 23);
      this.OptionsDialogOkButton.TabIndex = 2;
      this.OptionsDialogOkButton.Text = "OK";
      this.OptionsDialogOkButton.UseVisualStyleBackColor = true;
      this.OptionsDialogOkButton.Click += new System.EventHandler(this.OptionsDialogOkButton_Click);
      // 
      // fdFont
      // 
      this.fdFont.ShowColor = true;
      // 
      // tpTitle
      // 
      this.tpTitle.Controls.Add(this.btnChangeTitleFont);
      this.tpTitle.Location = new System.Drawing.Point(4, 22);
      this.tpTitle.Name = "tpTitle";
      this.tpTitle.Size = new System.Drawing.Size(284, 199);
      this.tpTitle.TabIndex = 2;
      this.tpTitle.Text = "Title";
      this.tpTitle.UseVisualStyleBackColor = true;
      // 
      // btnChangeTitleFont
      // 
      this.btnChangeTitleFont.Location = new System.Drawing.Point(3, 3);
      this.btnChangeTitleFont.Name = "btnChangeTitleFont";
      this.btnChangeTitleFont.Size = new System.Drawing.Size(94, 23);
      this.btnChangeTitleFont.TabIndex = 1;
      this.btnChangeTitleFont.Text = "Change Font";
      this.btnChangeTitleFont.UseVisualStyleBackColor = true;
      this.btnChangeTitleFont.Click += new System.EventHandler(this.btnChangeTitleFont_Click);
      // 
      // tabPage2
      // 
      this.tabPage2.Controls.Add(this.btnChangeLegendFont);
      this.tabPage2.Controls.Add(this.legendposition);
      this.tabPage2.Controls.Add(this.cbLegendPosition);
      this.tabPage2.Location = new System.Drawing.Point(4, 22);
      this.tabPage2.Name = "tabPage2";
      this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage2.Size = new System.Drawing.Size(284, 199);
      this.tabPage2.TabIndex = 1;
      this.tabPage2.Text = "Legend";
      this.tabPage2.UseVisualStyleBackColor = true;
      // 
      // btnChangeLegendFont
      // 
      this.btnChangeLegendFont.Location = new System.Drawing.Point(3, 6);
      this.btnChangeLegendFont.Name = "btnChangeLegendFont";
      this.btnChangeLegendFont.Size = new System.Drawing.Size(94, 23);
      this.btnChangeLegendFont.TabIndex = 2;
      this.btnChangeLegendFont.Text = "Change Font";
      this.btnChangeLegendFont.UseVisualStyleBackColor = true;
      this.btnChangeLegendFont.Click += new System.EventHandler(this.btnChangeLegendFont_Click);
      // 
      // legendposition
      // 
      this.legendposition.AutoSize = true;
      this.legendposition.Location = new System.Drawing.Point(8, 38);
      this.legendposition.Name = "legendposition";
      this.legendposition.Size = new System.Drawing.Size(82, 13);
      this.legendposition.TabIndex = 1;
      this.legendposition.Text = "Legendposition:";
      // 
      // cbLegendPosition
      // 
      this.cbLegendPosition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cbLegendPosition.FormattingEnabled = true;
      this.cbLegendPosition.Location = new System.Drawing.Point(96, 35);
      this.cbLegendPosition.Name = "cbLegendPosition";
      this.cbLegendPosition.Size = new System.Drawing.Size(121, 21);
      this.cbLegendPosition.TabIndex = 0;
      this.cbLegendPosition.SelectedIndexChanged += new System.EventHandler(this.cbLegendPosition_SelectedIndexChanged);
      // 
      // tabPage1
      // 
      this.tabPage1.Controls.Add(this.OptionsDialogSelectColorBt);
      this.tabPage1.Controls.Add(this.ColorPreviewTB);
      this.tabPage1.Controls.Add(this.label2);
      this.tabPage1.Controls.Add(this.LinestyleCB);
      this.tabPage1.Controls.Add(this.label1);
      this.tabPage1.Controls.Add(this.LineSelectCB);
      this.tabPage1.Controls.Add(this.groupBox1);
      this.tabPage1.Location = new System.Drawing.Point(4, 22);
      this.tabPage1.Name = "tabPage1";
      this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage1.Size = new System.Drawing.Size(284, 199);
      this.tabPage1.TabIndex = 0;
      this.tabPage1.Text = "Linestyle";
      this.tabPage1.UseVisualStyleBackColor = true;
      // 
      // OptionsDialogSelectColorBt
      // 
      this.OptionsDialogSelectColorBt.Location = new System.Drawing.Point(217, 126);
      this.OptionsDialogSelectColorBt.Name = "OptionsDialogSelectColorBt";
      this.OptionsDialogSelectColorBt.Size = new System.Drawing.Size(50, 23);
      this.OptionsDialogSelectColorBt.TabIndex = 7;
      this.OptionsDialogSelectColorBt.Text = "Select";
      this.OptionsDialogSelectColorBt.UseVisualStyleBackColor = true;
      this.OptionsDialogSelectColorBt.Click += new System.EventHandler(this.OptionsDialogSelectColorBtn_Click);
      // 
      // ColorPreviewTB
      // 
      this.ColorPreviewTB.Location = new System.Drawing.Point(146, 127);
      this.ColorPreviewTB.Name = "ColorPreviewTB";
      this.ColorPreviewTB.ReadOnly = true;
      this.ColorPreviewTB.Size = new System.Drawing.Size(64, 20);
      this.ColorPreviewTB.TabIndex = 6;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(36, 80);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(30, 13);
      this.label2.TabIndex = 3;
      this.label2.Text = "Style";
      // 
      // LinestyleCB
      // 
      this.LinestyleCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.LinestyleCB.FormattingEnabled = true;
      this.LinestyleCB.Location = new System.Drawing.Point(146, 73);
      this.LinestyleCB.Name = "LinestyleCB";
      this.LinestyleCB.Size = new System.Drawing.Size(121, 21);
      this.LinestyleCB.TabIndex = 2;
      this.LinestyleCB.SelectedIndexChanged += new System.EventHandler(this.LinestyleCB_SelectedIndexChanged);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(7, 37);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(72, 13);
      this.label1.TabIndex = 1;
      this.label1.Text = "Selected Line";
      // 
      // LineSelectCB
      // 
      this.LineSelectCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.LineSelectCB.FormattingEnabled = true;
      this.LineSelectCB.Location = new System.Drawing.Point(146, 30);
      this.LineSelectCB.Name = "LineSelectCB";
      this.LineSelectCB.Size = new System.Drawing.Size(121, 21);
      this.LineSelectCB.TabIndex = 0;
      this.LineSelectCB.SelectedIndexChanged += new System.EventHandler(this.LineSelectCB_SelectedIndexChanged);
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.label4);
      this.groupBox1.Controls.Add(this.LineThicknessCB);
      this.groupBox1.Controls.Add(this.label3);
      this.groupBox1.Location = new System.Drawing.Point(4, 54);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(274, 129);
      this.groupBox1.TabIndex = 9;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Details";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(33, 77);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(31, 13);
      this.label4.TabIndex = 8;
      this.label4.Text = "Color";
      // 
      // LineThicknessCB
      // 
      this.LineThicknessCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.LineThicknessCB.FormattingEnabled = true;
      this.LineThicknessCB.Location = new System.Drawing.Point(142, 46);
      this.LineThicknessCB.Name = "LineThicknessCB";
      this.LineThicknessCB.Size = new System.Drawing.Size(121, 21);
      this.LineThicknessCB.TabIndex = 4;
      this.LineThicknessCB.SelectedIndexChanged += new System.EventHandler(this.LineThicknessCB_SelectedIndexChanged);
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(32, 52);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(56, 13);
      this.label3.TabIndex = 5;
      this.label3.Text = "Thickness";
      // 
      // Optionstabs
      // 
      this.Optionstabs.Controls.Add(this.tabPage1);
      this.Optionstabs.Controls.Add(this.tabPage2);
      this.Optionstabs.Controls.Add(this.tpTitle);
      this.Optionstabs.Controls.Add(this.tpXAxis);
      this.Optionstabs.Controls.Add(this.tpYAxes);
      this.Optionstabs.Location = new System.Drawing.Point(0, 1);
      this.Optionstabs.Name = "Optionstabs";
      this.Optionstabs.SelectedIndex = 0;
      this.Optionstabs.Size = new System.Drawing.Size(292, 225);
      this.Optionstabs.TabIndex = 0;
      // 
      // tpXAxis
      // 
      this.tpXAxis.Controls.Add(this.btnChangeXAxisFont);
      this.tpXAxis.Location = new System.Drawing.Point(4, 22);
      this.tpXAxis.Name = "tpXAxis";
      this.tpXAxis.Size = new System.Drawing.Size(284, 199);
      this.tpXAxis.TabIndex = 3;
      this.tpXAxis.Text = "X-Axis";
      this.tpXAxis.UseVisualStyleBackColor = true;
      // 
      // btnChangeXAxisFont
      // 
      this.btnChangeXAxisFont.Location = new System.Drawing.Point(3, 3);
      this.btnChangeXAxisFont.Name = "btnChangeXAxisFont";
      this.btnChangeXAxisFont.Size = new System.Drawing.Size(94, 23);
      this.btnChangeXAxisFont.TabIndex = 2;
      this.btnChangeXAxisFont.Text = "Change Font";
      this.btnChangeXAxisFont.UseVisualStyleBackColor = true;
      this.btnChangeXAxisFont.Click += new System.EventHandler(this.btnChangeXAxisFont_Click);
      // 
      // tpYAxes
      // 
      this.tpYAxes.Controls.Add(this.gbxYAxisClipChangeable);
      this.tpYAxes.Controls.Add(this.gbxShowYAxis);
      this.tpYAxes.Location = new System.Drawing.Point(4, 22);
      this.tpYAxes.Name = "tpYAxes";
      this.tpYAxes.Padding = new System.Windows.Forms.Padding(3);
      this.tpYAxes.Size = new System.Drawing.Size(284, 199);
      this.tpYAxes.TabIndex = 4;
      this.tpYAxes.Text = "Y-Axes";
      this.tpYAxes.UseVisualStyleBackColor = true;
      // 
      // gbxShowYAxis
      // 
      this.gbxShowYAxis.AutoSize = true;
      this.gbxShowYAxis.Controls.Add(this.flpShowYAxis);
      this.gbxShowYAxis.Location = new System.Drawing.Point(0, 0);
      this.gbxShowYAxis.Name = "gbxShowYAxis";
      this.gbxShowYAxis.Size = new System.Drawing.Size(281, 76);
      this.gbxShowYAxis.TabIndex = 0;
      this.gbxShowYAxis.TabStop = false;
      this.gbxShowYAxis.Text = "Show Y-Axis";
      // 
      // gbxYAxisClipChangeable
      // 
      this.gbxYAxisClipChangeable.AutoSize = true;
      this.gbxYAxisClipChangeable.Controls.Add(this.flpYAxisClipChangeable);
      this.gbxYAxisClipChangeable.Location = new System.Drawing.Point(0, 82);
      this.gbxYAxisClipChangeable.Name = "gbxYAxisClipChangeable";
      this.gbxYAxisClipChangeable.Size = new System.Drawing.Size(281, 76);
      this.gbxYAxisClipChangeable.TabIndex = 1;
      this.gbxYAxisClipChangeable.TabStop = false;
      this.gbxYAxisClipChangeable.Text = "Y-Axis-Clip changeable";
      // 
      // flpShowYAxis
      // 
      this.flpShowYAxis.AutoSize = true;
      this.flpShowYAxis.Dock = System.Windows.Forms.DockStyle.Fill;
      this.flpShowYAxis.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      this.flpShowYAxis.Location = new System.Drawing.Point(3, 16);
      this.flpShowYAxis.Name = "flpShowYAxis";
      this.flpShowYAxis.Size = new System.Drawing.Size(275, 57);
      this.flpShowYAxis.TabIndex = 0;
      // 
      // flpYAxisClipChangeable
      // 
      this.flpYAxisClipChangeable.AutoSize = true;
      this.flpYAxisClipChangeable.Dock = System.Windows.Forms.DockStyle.Fill;
      this.flpYAxisClipChangeable.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      this.flpYAxisClipChangeable.Location = new System.Drawing.Point(3, 16);
      this.flpYAxisClipChangeable.Name = "flpYAxisClipChangeable";
      this.flpYAxisClipChangeable.Size = new System.Drawing.Size(275, 57);
      this.flpYAxisClipChangeable.TabIndex = 0;
      // 
      // OptionsDialog
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(292, 266);
      this.Controls.Add(this.OptionsDialogOkButton);
      this.Controls.Add(this.OptionsDialogCancelButton);
      this.Controls.Add(this.Optionstabs);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.Name = "OptionsDialog";
      this.Text = "OptionsDialog";
      this.Load += new System.EventHandler(this.OptionsDialog_Load);
      this.tpTitle.ResumeLayout(false);
      this.tabPage2.ResumeLayout(false);
      this.tabPage2.PerformLayout();
      this.tabPage1.ResumeLayout(false);
      this.tabPage1.PerformLayout();
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.Optionstabs.ResumeLayout(false);
      this.tpXAxis.ResumeLayout(false);
      this.tpYAxes.ResumeLayout(false);
      this.tpYAxes.PerformLayout();
      this.gbxShowYAxis.ResumeLayout(false);
      this.gbxShowYAxis.PerformLayout();
      this.gbxYAxisClipChangeable.ResumeLayout(false);
      this.gbxYAxisClipChangeable.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button OptionsDialogCancelButton;
    private System.Windows.Forms.Button OptionsDialogOkButton;
    private System.Windows.Forms.FontDialog fdFont;
    private System.Windows.Forms.TabPage tpTitle;
    private System.Windows.Forms.Button btnChangeTitleFont;
    private System.Windows.Forms.TabPage tabPage2;
    private System.Windows.Forms.TabPage tabPage1;
    private System.Windows.Forms.Button OptionsDialogSelectColorBt;
    private System.Windows.Forms.TextBox ColorPreviewTB;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.ComboBox LinestyleCB;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.ComboBox LineSelectCB;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.ComboBox cbLegendPosition;
    private System.Windows.Forms.Label legendposition;
    private System.Windows.Forms.ComboBox LineThicknessCB;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TabControl Optionstabs;
    private System.Windows.Forms.Button btnChangeLegendFont;
    private System.Windows.Forms.TabPage tpXAxis;
    private System.Windows.Forms.Button btnChangeXAxisFont;
    private System.Windows.Forms.TabPage tpYAxes;
    private System.Windows.Forms.GroupBox gbxShowYAxis;
    private System.Windows.Forms.GroupBox gbxYAxisClipChangeable;
    private System.Windows.Forms.FlowLayoutPanel flpYAxisClipChangeable;
    private System.Windows.Forms.FlowLayoutPanel flpShowYAxis;
  }
}
