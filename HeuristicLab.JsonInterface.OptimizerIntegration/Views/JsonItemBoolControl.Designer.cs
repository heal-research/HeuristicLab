namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  partial class JsonItemBoolControl {
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
      this.checkBoxValue = new System.Windows.Forms.CheckBox();
      this.SuspendLayout();
      // 
      // checkBoxValue
      // 
      this.checkBoxValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.checkBoxValue.AutoSize = true;
      this.checkBoxValue.Location = new System.Drawing.Point(92, 84);
      this.checkBoxValue.Name = "checkBoxValue";
      this.checkBoxValue.Size = new System.Drawing.Size(15, 14);
      this.checkBoxValue.TabIndex = 19;
      this.checkBoxValue.UseVisualStyleBackColor = true;
      this.checkBoxValue.CheckStateChanged += new System.EventHandler(this.checkBoxValue_CheckStateChanged);
      // 
      // JsonItemBoolControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.Control;
      this.Controls.Add(this.checkBoxValue);
      this.Name = "JsonItemBoolControl";
      this.Size = new System.Drawing.Size(500, 105);
      this.Controls.SetChildIndex(this.checkBoxValue, 0);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion
    private System.Windows.Forms.CheckBox checkBoxValue;
  }
}
