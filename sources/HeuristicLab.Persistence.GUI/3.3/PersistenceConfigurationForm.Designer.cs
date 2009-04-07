namespace HeuristicLab.Persistence.GUI {
  partial class PersistenceConfigurationForm {
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
      System.Windows.Forms.Button updateButton;
      this.configurationTabs = new System.Windows.Forms.TabControl();
      updateButton = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // updateButton
      // 
      updateButton.Dock = System.Windows.Forms.DockStyle.Bottom;
      updateButton.Location = new System.Drawing.Point(0, 555);
      updateButton.Name = "updateButton";
      updateButton.Size = new System.Drawing.Size(597, 25);
      updateButton.TabIndex = 1;
      updateButton.Text = "Define Configuration";
      updateButton.UseVisualStyleBackColor = true;
      updateButton.Click += new System.EventHandler(this.updateButton_Click);
      // 
      // configurationTabs
      // 
      this.configurationTabs.Dock = System.Windows.Forms.DockStyle.Fill;
      this.configurationTabs.Location = new System.Drawing.Point(0, 0);
      this.configurationTabs.Name = "configurationTabs";
      this.configurationTabs.SelectedIndex = 0;
      this.configurationTabs.Size = new System.Drawing.Size(597, 580);
      this.configurationTabs.TabIndex = 0;
      // 
      // PersistenceConfigurationForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(597, 580);
      this.Controls.Add(updateButton);
      this.Controls.Add(this.configurationTabs);
      this.Name = "PersistenceConfigurationForm";
      this.Text = "PersistenceConfigurationForm";      
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TabControl configurationTabs;
  }
}