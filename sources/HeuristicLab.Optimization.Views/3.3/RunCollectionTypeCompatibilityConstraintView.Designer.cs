namespace HeuristicLab.Optimization.Views {
  partial class RunCollectionTypeCompatibilityConstraintView {
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
      this.cmbType = new System.Windows.Forms.ComboBox();
      this.SuspendLayout();
      // 
      // cmbType
      // 
      this.cmbType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cmbType.FormattingEnabled = true;
      this.cmbType.Location = new System.Drawing.Point(127, 55);
      this.cmbType.Name = "cmbType";
      this.cmbType.Size = new System.Drawing.Size(246, 21);
      this.cmbType.TabIndex = 9;
      this.cmbType.SelectedIndexChanged += new System.EventHandler(this.cmbType_SelectedIndexChanged);
      // 
      // RunCollectionTypeCompatibilityConstraintView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.cmbType);
      this.Name = "RunCollectionTypeCompatibilityConstraintView";
      this.Controls.SetChildIndex(this.cmbType, 0);
      this.Controls.SetChildIndex(this.cmbConstraintOperation, 0);
      this.Controls.SetChildIndex(this.cmbConstraintColumn, 0);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ComboBox cmbType;

  }
}
