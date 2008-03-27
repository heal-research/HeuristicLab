namespace HeuristicLab.TestFunctions {
  partial class TestFunctionInjectorView {
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
      this.DimensionLabel = new System.Windows.Forms.Label();
      this.MinimumLabel = new System.Windows.Forms.Label();
      this.maximumLabel = new System.Windows.Forms.Label();
      this.maximizationCheckBox = new System.Windows.Forms.CheckBox();
      this.dimensionTextBox = new System.Windows.Forms.TextBox();
      this.minimumTextBox = new System.Windows.Forms.TextBox();
      this.maximumTextBox = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // DimensionLabel
      // 
      this.DimensionLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.DimensionLabel.AutoSize = true;
      this.DimensionLabel.Location = new System.Drawing.Point(4, 29);
      this.DimensionLabel.Name = "DimensionLabel";
      this.DimensionLabel.Size = new System.Drawing.Size(59, 13);
      this.DimensionLabel.TabIndex = 1;
      this.DimensionLabel.Text = "Dimension:";
      // 
      // MinimumLabel
      // 
      this.MinimumLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.MinimumLabel.AutoSize = true;
      this.MinimumLabel.Location = new System.Drawing.Point(4, 55);
      this.MinimumLabel.Name = "MinimumLabel";
      this.MinimumLabel.Size = new System.Drawing.Size(51, 13);
      this.MinimumLabel.TabIndex = 3;
      this.MinimumLabel.Text = "Minimum:";
      // 
      // maximumLabel
      // 
      this.maximumLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.maximumLabel.AutoSize = true;
      this.maximumLabel.Location = new System.Drawing.Point(4, 81);
      this.maximumLabel.Name = "maximumLabel";
      this.maximumLabel.Size = new System.Drawing.Size(54, 13);
      this.maximumLabel.TabIndex = 5;
      this.maximumLabel.Text = "Maximum:";
      // 
      // maximizationCheckBox
      // 
      this.maximizationCheckBox.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.maximizationCheckBox.AutoSize = true;
      this.maximizationCheckBox.Location = new System.Drawing.Point(69, 3);
      this.maximizationCheckBox.Name = "maximizationCheckBox";
      this.maximizationCheckBox.Size = new System.Drawing.Size(86, 17);
      this.maximizationCheckBox.TabIndex = 0;
      this.maximizationCheckBox.Text = "Maximization";
      this.maximizationCheckBox.UseVisualStyleBackColor = true;
      // 
      // dimensionTextBox
      // 
      this.dimensionTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.dimensionTextBox.Location = new System.Drawing.Point(69, 26);
      this.dimensionTextBox.Name = "dimensionTextBox";
      this.dimensionTextBox.Size = new System.Drawing.Size(100, 20);
      this.dimensionTextBox.TabIndex = 2;
      // 
      // minimumTextBox
      // 
      this.minimumTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.minimumTextBox.Location = new System.Drawing.Point(69, 52);
      this.minimumTextBox.Name = "minimumTextBox";
      this.minimumTextBox.Size = new System.Drawing.Size(100, 20);
      this.minimumTextBox.TabIndex = 4;
      // 
      // maximumTextBox
      // 
      this.maximumTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.maximumTextBox.Location = new System.Drawing.Point(69, 78);
      this.maximumTextBox.Name = "maximumTextBox";
      this.maximumTextBox.Size = new System.Drawing.Size(100, 20);
      this.maximumTextBox.TabIndex = 6;
      // 
      // TestFunctionInjectorView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.maximumTextBox);
      this.Controls.Add(this.minimumTextBox);
      this.Controls.Add(this.dimensionTextBox);
      this.Controls.Add(this.maximizationCheckBox);
      this.Controls.Add(this.maximumLabel);
      this.Controls.Add(this.MinimumLabel);
      this.Controls.Add(this.DimensionLabel);
      this.Name = "TestFunctionInjectorView";
      this.Size = new System.Drawing.Size(173, 101);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label DimensionLabel;
    private System.Windows.Forms.Label MinimumLabel;
    private System.Windows.Forms.Label maximumLabel;
    private System.Windows.Forms.CheckBox maximizationCheckBox;
    private System.Windows.Forms.TextBox dimensionTextBox;
    private System.Windows.Forms.TextBox minimumTextBox;
    private System.Windows.Forms.TextBox maximumTextBox;
  }
}
