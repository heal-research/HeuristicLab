namespace HeuristicLab.Scripting.Views {
  partial class ExecutableScriptView {
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
      this.startStopButton = new System.Windows.Forms.Button();
      this.executionTimeLabel = new System.Windows.Forms.Label();
      this.executionTimeTextBox = new System.Windows.Forms.TextBox();
      this.infoTabControl.SuspendLayout();
      this.outputTabPage.SuspendLayout();
      this.errorListTabPage.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // splitContainer1
      // 
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.executionTimeLabel);
      this.splitContainer1.Panel2.Controls.Add(this.executionTimeTextBox);
      this.splitContainer1.Panel2.Controls.SetChildIndex(this.executionTimeTextBox, 0);
      this.splitContainer1.Panel2.Controls.SetChildIndex(this.executionTimeLabel, 0);
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      // 
      // startStopButton
      // 
      this.startStopButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Play;
      this.startStopButton.Location = new System.Drawing.Point(36, 26);
      this.startStopButton.Name = "startStopButton";
      this.startStopButton.Size = new System.Drawing.Size(24, 24);
      this.startStopButton.TabIndex = 10;
      this.startStopButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
      this.toolTip.SetToolTip(this.startStopButton, "Run (F5)");
      this.startStopButton.UseVisualStyleBackColor = true;
      this.startStopButton.Click += new System.EventHandler(this.startStopButton_Click);
      // 
      // executionTimeLabel
      // 
      this.executionTimeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.executionTimeLabel.AutoSize = true;
      this.executionTimeLabel.Location = new System.Drawing.Point(604, 3);
      this.executionTimeLabel.Name = "executionTimeLabel";
      this.executionTimeLabel.Size = new System.Drawing.Size(83, 13);
      this.executionTimeLabel.TabIndex = 18;
      this.executionTimeLabel.Text = "&Execution Time:";
      // 
      // executionTimeTextBox
      // 
      this.executionTimeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.executionTimeTextBox.Location = new System.Drawing.Point(693, 0);
      this.executionTimeTextBox.Name = "executionTimeTextBox";
      this.executionTimeTextBox.ReadOnly = true;
      this.executionTimeTextBox.Size = new System.Drawing.Size(137, 20);
      this.executionTimeTextBox.TabIndex = 19;
      // 
      // ExecutableScriptView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.startStopButton);
      this.Name = "ExecutableScriptView";
      this.Controls.SetChildIndex(this.infoTextLabel, 0);
      this.Controls.SetChildIndex(this.compileButton, 0);
      this.Controls.SetChildIndex(this.splitContainer1, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      this.Controls.SetChildIndex(this.infoLabel, 0);
      this.Controls.SetChildIndex(this.startStopButton, 0);
      this.infoTabControl.ResumeLayout(false);
      this.outputTabPage.ResumeLayout(false);
      this.outputTabPage.PerformLayout();
      this.errorListTabPage.ResumeLayout(false);
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      this.splitContainer1.Panel2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
      this.splitContainer1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    protected System.Windows.Forms.Button startStopButton;
    protected System.Windows.Forms.Label executionTimeLabel;
    protected System.Windows.Forms.TextBox executionTimeTextBox;
  }
}
