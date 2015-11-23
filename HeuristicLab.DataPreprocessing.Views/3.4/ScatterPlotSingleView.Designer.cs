namespace HeuristicLab.DataPreprocessing.Views {
  partial class ScatterPlotSingleView {
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
      this.scatterPlotView = new HeuristicLab.DataPreprocessing.Views.PreprocessingScatterPlotView();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.label2 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.comboBoxYVariable = new System.Windows.Forms.ComboBox();
      this.comboBoxXVariable = new System.Windows.Forms.ComboBox();
      this.groupBox1.SuspendLayout();
      this.SuspendLayout();
      // 
      // scatterPlotView
      // 
      this.scatterPlotView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.scatterPlotView.Caption = "ScatterPlot View";
      this.scatterPlotView.Content = null;
      this.scatterPlotView.Location = new System.Drawing.Point(169, 3);
      this.scatterPlotView.Name = "scatterPlotView";
      this.scatterPlotView.ReadOnly = false;
      this.scatterPlotView.Size = new System.Drawing.Size(689, 509);
      this.scatterPlotView.TabIndex = 0;
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.label2);
      this.groupBox1.Controls.Add(this.label1);
      this.groupBox1.Controls.Add(this.comboBoxYVariable);
      this.groupBox1.Controls.Add(this.comboBoxXVariable);
      this.groupBox1.Location = new System.Drawing.Point(3, 3);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(160, 140);
      this.groupBox1.TabIndex = 1;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Options";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(17, 80);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(55, 13);
      this.label2.TabIndex = 3;
      this.label2.Text = "Y Variable";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(17, 25);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(55, 13);
      this.label1.TabIndex = 2;
      this.label1.Text = "X Variable";
      // 
      // comboBoxYVariable
      // 
      this.comboBoxYVariable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.comboBoxYVariable.FormattingEnabled = true;
      this.comboBoxYVariable.Location = new System.Drawing.Point(20, 103);
      this.comboBoxYVariable.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
      this.comboBoxYVariable.Name = "comboBoxYVariable";
      this.comboBoxYVariable.Size = new System.Drawing.Size(121, 21);
      this.comboBoxYVariable.TabIndex = 1;
      this.comboBoxYVariable.SelectedIndexChanged += new System.EventHandler(this.comboBox_SelectedIndexChanged);
      // 
      // comboBoxXVariable
      // 
      this.comboBoxXVariable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.comboBoxXVariable.FormattingEnabled = true;
      this.comboBoxXVariable.Location = new System.Drawing.Point(20, 48);
      this.comboBoxXVariable.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
      this.comboBoxXVariable.Name = "comboBoxXVariable";
      this.comboBoxXVariable.Size = new System.Drawing.Size(121, 21);
      this.comboBoxXVariable.TabIndex = 0;
      this.comboBoxXVariable.SelectedIndexChanged += new System.EventHandler(this.comboBox_SelectedIndexChanged);
      // 
      // ScatterPlotSingleView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.scatterPlotView);
      this.Name = "ScatterPlotSingleView";
      this.Size = new System.Drawing.Size(863, 517);
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private HeuristicLab.DataPreprocessing.Views.PreprocessingScatterPlotView scatterPlotView;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.ComboBox comboBoxYVariable;
    private System.Windows.Forms.ComboBox comboBoxXVariable;
  }
}
