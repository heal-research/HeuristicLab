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
      this.lowerBoundTextBox = new System.Windows.Forms.TextBox();
      this.upperBoundTextBox = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // DimensionLabel
      // 
      this.DimensionLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.DimensionLabel.AutoSize = true;
      this.DimensionLabel.Location = new System.Drawing.Point(3, 29);
      this.DimensionLabel.Name = "DimensionLabel";
      this.DimensionLabel.Size = new System.Drawing.Size(59, 13);
      this.DimensionLabel.TabIndex = 1;
      this.DimensionLabel.Text = "Dimension:";
      // 
      // MinimumLabel
      // 
      this.MinimumLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.MinimumLabel.AutoSize = true;
      this.MinimumLabel.Location = new System.Drawing.Point(3, 55);
      this.MinimumLabel.Name = "MinimumLabel";
      this.MinimumLabel.Size = new System.Drawing.Size(73, 13);
      this.MinimumLabel.TabIndex = 3;
      this.MinimumLabel.Text = "Lower Bound:";
      // 
      // maximumLabel
      // 
      this.maximumLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.maximumLabel.AutoSize = true;
      this.maximumLabel.Location = new System.Drawing.Point(3, 81);
      this.maximumLabel.Name = "maximumLabel";
      this.maximumLabel.Size = new System.Drawing.Size(73, 13);
      this.maximumLabel.TabIndex = 5;
      this.maximumLabel.Text = "Upper Bound:";
      // 
      // maximizationCheckBox
      // 
      this.maximizationCheckBox.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.maximizationCheckBox.AutoSize = true;
      this.maximizationCheckBox.Location = new System.Drawing.Point(83, 3);
      this.maximizationCheckBox.Name = "maximizationCheckBox";
      this.maximizationCheckBox.Size = new System.Drawing.Size(86, 17);
      this.maximizationCheckBox.TabIndex = 0;
      this.maximizationCheckBox.Text = "Maximization";
      this.maximizationCheckBox.UseVisualStyleBackColor = true;
      // 
      // dimensionTextBox
      // 
      this.dimensionTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.dimensionTextBox.Location = new System.Drawing.Point(82, 26);
      this.dimensionTextBox.Name = "dimensionTextBox";
      this.dimensionTextBox.Size = new System.Drawing.Size(100, 20);
      this.dimensionTextBox.TabIndex = 2;
      // 
      // lowerBoundTextBox
      // 
      this.lowerBoundTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.lowerBoundTextBox.Location = new System.Drawing.Point(82, 52);
      this.lowerBoundTextBox.Name = "lowerBoundTextBox";
      this.lowerBoundTextBox.Size = new System.Drawing.Size(100, 20);
      this.lowerBoundTextBox.TabIndex = 4;
      // 
      // upperBoundTextBox
      // 
      this.upperBoundTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.upperBoundTextBox.Location = new System.Drawing.Point(82, 78);
      this.upperBoundTextBox.Name = "upperBoundTextBox";
      this.upperBoundTextBox.Size = new System.Drawing.Size(100, 20);
      this.upperBoundTextBox.TabIndex = 6;
      // 
      // TestFunctionInjectorView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.upperBoundTextBox);
      this.Controls.Add(this.lowerBoundTextBox);
      this.Controls.Add(this.dimensionTextBox);
      this.Controls.Add(this.maximizationCheckBox);
      this.Controls.Add(this.maximumLabel);
      this.Controls.Add(this.MinimumLabel);
      this.Controls.Add(this.DimensionLabel);
      this.Name = "TestFunctionInjectorView";
      this.Size = new System.Drawing.Size(186, 101);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label DimensionLabel;
    private System.Windows.Forms.Label MinimumLabel;
    private System.Windows.Forms.Label maximumLabel;
    private System.Windows.Forms.CheckBox maximizationCheckBox;
    private System.Windows.Forms.TextBox dimensionTextBox;
    private System.Windows.Forms.TextBox lowerBoundTextBox;
    private System.Windows.Forms.TextBox upperBoundTextBox;
  }
}
