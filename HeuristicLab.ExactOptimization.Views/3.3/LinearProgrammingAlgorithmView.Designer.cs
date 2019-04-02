using HeuristicLab.Common.Resources;

namespace HeuristicLab.ExactOptimization.Views {
  partial class LinearProgrammingAlgorithmView {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LinearProgrammingAlgorithmView));
      this.exportModelButton = new System.Windows.Forms.Button();
      this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
      this.showInRunCheckBox = new System.Windows.Forms.CheckBox();
      this.tabControl.SuspendLayout();
      this.parametersTabPage.SuspendLayout();
      this.problemTabPage.SuspendLayout();
      this.resultsTabPage.SuspendLayout();
      this.runsTabPage.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // parametersTabPage
      // 
      this.parametersTabPage.Size = new System.Drawing.Size(671, 404);
      // 
      // problemTabPage
      // 
      this.problemTabPage.Controls.Add(this.showInRunCheckBox);
      this.problemTabPage.Controls.Add(this.exportModelButton);
      this.problemTabPage.Controls.SetChildIndex(this.newProblemButton, 0);
      this.problemTabPage.Controls.SetChildIndex(this.openProblemButton, 0);
      this.problemTabPage.Controls.SetChildIndex(this.problemViewHost, 0);
      this.problemTabPage.Controls.SetChildIndex(this.exportModelButton, 0);
      this.problemTabPage.Controls.SetChildIndex(this.showInRunCheckBox, 0);
      // 
      // parameterCollectionView
      // 
      this.parameterCollectionView.Size = new System.Drawing.Size(659, 392);
      // 
      // problemViewHost
      // 
      this.problemViewHost.TabIndex = 4;
      // 
      // newProblemButton
      // 
      this.toolTip.SetToolTip(this.newProblemButton, "New Programmable Model");
      // 
      // openProblemButton
      // 
      this.toolTip.SetToolTip(this.openProblemButton, "Import Model From File");
      // 
      // resultsTabPage
      // 
      this.resultsTabPage.Size = new System.Drawing.Size(671, 404);
      // 
      // resultsView
      // 
      this.resultsView.Size = new System.Drawing.Size(659, 392);
      // 
      // runsTabPage
      // 
      this.runsTabPage.Size = new System.Drawing.Size(671, 404);
      // 
      // runsView
      // 
      this.runsView.Size = new System.Drawing.Size(659, 392);
      // 
      // storeAlgorithmInEachRunCheckBox
      // 
      this.toolTip.SetToolTip(this.storeAlgorithmInEachRunCheckBox, "Check to store a copy of the algorithm in each run.");
      // 
      // startButton
      // 
      this.toolTip.SetToolTip(this.startButton, "Start/Resume Algorithm");
      // 
      // pauseButton
      // 
      this.toolTip.SetToolTip(this.pauseButton, "Pause Algorithm");
      // 
      // stopButton
      // 
      this.toolTip.SetToolTip(this.stopButton, "Stop Algorithm");
      // 
      // resetButton
      // 
      this.toolTip.SetToolTip(this.resetButton, "Reset Algorithm");
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      // 
      // exportModelButton
      // 
      this.exportModelButton.Image = VSImageLibrary.SaveAs;
      this.exportModelButton.Location = new System.Drawing.Point(66, 6);
      this.exportModelButton.Name = "exportModelButton";
      this.exportModelButton.Size = new System.Drawing.Size(24, 24);
      this.exportModelButton.TabIndex = 2;
      this.toolTip.SetToolTip(this.exportModelButton, "Export Model To File");
      this.exportModelButton.UseVisualStyleBackColor = true;
      this.exportModelButton.Click += new System.EventHandler(this.exportModelButton_Click);
      // 
      // showInRunCheckBox
      // 
      this.showInRunCheckBox.AutoSize = true;
      this.showInRunCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.showInRunCheckBox.Location = new System.Drawing.Point(96, 11);
      this.showInRunCheckBox.Name = "showInRunCheckBox";
      this.showInRunCheckBox.Size = new System.Drawing.Size(90, 17);
      this.showInRunCheckBox.TabIndex = 3;
      this.showInRunCheckBox.Text = "&Show in Run:";
      this.toolTip.SetToolTip(this.showInRunCheckBox, "Check to show the model in each run.");
      this.showInRunCheckBox.UseVisualStyleBackColor = true;
      this.showInRunCheckBox.CheckedChanged += new System.EventHandler(this.showInRunCheckBox_CheckedChanged);
      // 
      // LinearProgrammingAlgorithmView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Name = "LinearProgrammingAlgorithmView";
      this.tabControl.ResumeLayout(false);
      this.parametersTabPage.ResumeLayout(false);
      this.problemTabPage.ResumeLayout(false);
      this.problemTabPage.PerformLayout();
      this.resultsTabPage.ResumeLayout(false);
      this.runsTabPage.ResumeLayout(false);
      this.runsTabPage.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    protected System.Windows.Forms.Button exportModelButton;
    private System.Windows.Forms.SaveFileDialog saveFileDialog;
    protected System.Windows.Forms.CheckBox showInRunCheckBox;
  }
}
