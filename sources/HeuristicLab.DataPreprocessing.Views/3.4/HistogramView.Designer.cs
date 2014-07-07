namespace HeuristicLab.DataPreprocessing.Views {
  partial class HistogramView {
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
      this.optionsBox = new System.Windows.Forms.GroupBox();
      this.label1 = new System.Windows.Forms.Label();
      this.classifierComboBox = new System.Windows.Forms.ComboBox();
      this.optionsBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // optionsBox
      // 
      this.optionsBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.optionsBox.Controls.Add(this.label1);
      this.optionsBox.Controls.Add(this.classifierComboBox);
      this.optionsBox.Location = new System.Drawing.Point(4, 263);
      this.optionsBox.Name = "optionsBox";
      this.optionsBox.Size = new System.Drawing.Size(152, 134);
      this.optionsBox.TabIndex = 7;
      this.optionsBox.TabStop = false;
      this.optionsBox.Text = "Options";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(6, 26);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(91, 13);
      this.label1.TabIndex = 2;
      this.label1.Text = "Classifier variable:";
      // 
      // classifierComboBox
      // 
      this.classifierComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.classifierComboBox.FormattingEnabled = true;
      this.classifierComboBox.Location = new System.Drawing.Point(9, 52);
      this.classifierComboBox.Name = "classifierComboBox";
      this.classifierComboBox.Size = new System.Drawing.Size(121, 21);
      this.classifierComboBox.TabIndex = 1;
      this.classifierComboBox.SelectedIndexChanged += new System.EventHandler(this.classifierComboBox_SelectedIndexChanged);
      // 
      // HistogramView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.optionsBox);
      this.Name = "HistogramView";
      this.Controls.SetChildIndex(this.optionsBox, 0);
      this.optionsBox.ResumeLayout(false);
      this.optionsBox.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox optionsBox;
    private System.Windows.Forms.ComboBox classifierComboBox;
    private System.Windows.Forms.Label label1;

  }
}
