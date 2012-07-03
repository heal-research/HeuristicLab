namespace HeuristicLab.Clients.Hive.Views {
  partial class HiveItemView {
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
      this.components = new System.ComponentModel.Container();
      this.storeButton = new System.Windows.Forms.Button();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.SuspendLayout();
      // 
      // storeButton
      // 
      this.storeButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.PublishToWeb;
      this.storeButton.Location = new System.Drawing.Point(0, 0);
      this.storeButton.Name = "storeButton";
      this.storeButton.Size = new System.Drawing.Size(24, 24);
      this.storeButton.TabIndex = 0;
      this.toolTip.SetToolTip(this.storeButton, "Store Data");
      this.storeButton.UseVisualStyleBackColor = true;
      this.storeButton.Click += new System.EventHandler(this.storeButton_Click);
      // 
      // HiveItemView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.storeButton);
      this.Name = "HiveItemView";
      this.Size = new System.Drawing.Size(354, 308);
      this.ResumeLayout(false);

    }

    #endregion

    protected System.Windows.Forms.Button storeButton;
    protected System.Windows.Forms.ToolTip toolTip;
  }
}
