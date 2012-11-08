using HeuristicLab.Encodings.ScheduleEncoding.Views;
namespace HeuristicLab.Problems.Scheduling.Views {
  partial class JobShopSchedulingProblemView {
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
      this.problemTabControl = new System.Windows.Forms.TabControl();
      this.problemTabPage = new System.Windows.Forms.TabPage();
      this.visualizationTabPage = new System.Windows.Forms.TabPage();
      this.ganttChart = new HeuristicLab.Encodings.ScheduleEncoding.Views.GanttChart();
      ((System.ComponentModel.ISupportInitialize)(this.problemInstanceSplitContainer)).BeginInit();
      this.problemInstanceSplitContainer.Panel1.SuspendLayout();
      this.problemInstanceSplitContainer.Panel2.SuspendLayout();
      this.problemInstanceSplitContainer.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.problemTabControl.SuspendLayout();
      this.problemTabPage.SuspendLayout();
      this.visualizationTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // problemInstanceSplitContainer
      // 
      this.problemInstanceSplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
      this.problemInstanceSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      // 
      // problemInstanceSplitContainer.Panel2
      // 
      this.problemInstanceSplitContainer.Panel2.Controls.Add(this.problemTabControl);
      // 
      // ProblemInstanceConsumerViewHost
      // 
      this.ProblemInstanceConsumerViewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
      this.ProblemInstanceConsumerViewHost.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ProblemInstanceConsumerViewHost.Location = new System.Drawing.Point(0, 0);
      this.ProblemInstanceConsumerViewHost.Size = new System.Drawing.Size(511, 26);
      // 
      // parameterCollectionView
      // 
      this.parameterCollectionView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
      this.parameterCollectionView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.parameterCollectionView.Location = new System.Drawing.Point(3, 3);
      this.parameterCollectionView.Size = new System.Drawing.Size(491, 271);
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Size = new System.Drawing.Size(439, 20);
      // 
      // infoLabel
      // 
      this.infoLabel.Location = new System.Drawing.Point(492, 3);
      // 
      // problemTabControl
      // 
      this.problemTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.problemTabControl.Controls.Add(this.problemTabPage);
      this.problemTabControl.Controls.Add(this.visualizationTabPage);
      this.problemTabControl.Location = new System.Drawing.Point(3, 27);
      this.problemTabControl.Name = "problemTabControl";
      this.problemTabControl.SelectedIndex = 0;
      this.problemTabControl.Size = new System.Drawing.Size(505, 303);
      this.problemTabControl.TabIndex = 5;
      // 
      // problemTabPage
      // 
      this.problemTabPage.Controls.Add(this.parameterCollectionView);
      this.problemTabPage.Location = new System.Drawing.Point(4, 22);
      this.problemTabPage.Name = "problemTabPage";
      this.problemTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.problemTabPage.Size = new System.Drawing.Size(497, 277);
      this.problemTabPage.TabIndex = 0;
      this.problemTabPage.Text = "Problem";
      this.problemTabPage.UseVisualStyleBackColor = true;
      // 
      // visualizationTabPage
      // 
      this.visualizationTabPage.Controls.Add(this.ganttChart);
      this.visualizationTabPage.Location = new System.Drawing.Point(4, 22);
      this.visualizationTabPage.Name = "visualizationTabPage";
      this.visualizationTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.visualizationTabPage.Size = new System.Drawing.Size(497, 277);
      this.visualizationTabPage.TabIndex = 1;
      this.visualizationTabPage.Text = "Visualization";
      this.visualizationTabPage.UseVisualStyleBackColor = true;
      // 
      // ganttChart
      // 
      this.ganttChart.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ganttChart.Location = new System.Drawing.Point(3, 3);
      this.ganttChart.Name = "ganttChart";
      this.ganttChart.Size = new System.Drawing.Size(491, 271);
      this.ganttChart.TabIndex = 0;
      // 
      // JobShopSchedulingProblemView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Name = "JobShopSchedulingProblemView";
      this.problemInstanceSplitContainer.Panel1.ResumeLayout(false);
      this.problemInstanceSplitContainer.Panel2.ResumeLayout(false);
      this.problemInstanceSplitContainer.Panel2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.problemInstanceSplitContainer)).EndInit();
      this.problemInstanceSplitContainer.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.problemTabControl.ResumeLayout(false);
      this.problemTabPage.ResumeLayout(false);
      this.visualizationTabPage.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TabControl problemTabControl;
    private System.Windows.Forms.TabPage problemTabPage;
    private System.Windows.Forms.TabPage visualizationTabPage;
    private GanttChart ganttChart;
  }
}
