
namespace HeuristicLab.Optimization.Views {
  partial class MultiObjectiveEvaluationResultView {
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
            this.qualitiesLabel = new System.Windows.Forms.Label();
            this.qualitiesTextBox = new System.Windows.Forms.TextBox();
            this.dataLabel = new System.Windows.Forms.Label();
            this.dataViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
            this.SuspendLayout();
            // 
            // qualitiesLabel
            // 
            this.qualitiesLabel.AutoSize = true;
            this.qualitiesLabel.Location = new System.Drawing.Point(3, 6);
            this.qualitiesLabel.Name = "qualitiesLabel";
            this.qualitiesLabel.Size = new System.Drawing.Size(50, 13);
            this.qualitiesLabel.TabIndex = 0;
            this.qualitiesLabel.Text = "Qualities:";
            // 
            // qualitiesTextBox
            // 
            this.qualitiesTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.qualitiesTextBox.Location = new System.Drawing.Point(59, 3);
            this.qualitiesTextBox.Name = "qualitiesTextBox";
            this.qualitiesTextBox.ReadOnly = true;
            this.qualitiesTextBox.Size = new System.Drawing.Size(327, 20);
            this.qualitiesTextBox.TabIndex = 1;
            // 
            // dataLabel
            // 
            this.dataLabel.AutoSize = true;
            this.dataLabel.Location = new System.Drawing.Point(3, 29);
            this.dataLabel.Name = "dataLabel";
            this.dataLabel.Size = new System.Drawing.Size(82, 13);
            this.dataLabel.TabIndex = 0;
            this.dataLabel.Text = "Additional Data:";
            // 
            // dataViewHost
            // 
            this.dataViewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataViewHost.Caption = "View";
            this.dataViewHost.Content = null;
            this.dataViewHost.Enabled = false;
            this.dataViewHost.Location = new System.Drawing.Point(0, 45);
            this.dataViewHost.Name = "dataViewHost";
            this.dataViewHost.ReadOnly = false;
            this.dataViewHost.Size = new System.Drawing.Size(386, 369);
            this.dataViewHost.TabIndex = 2;
            this.dataViewHost.ViewsLabelVisible = false;
            this.dataViewHost.ViewType = null;
            // 
            // MultiObjectiveEvaluationResultView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataViewHost);
            this.Controls.Add(this.qualitiesTextBox);
            this.Controls.Add(this.dataLabel);
            this.Controls.Add(this.qualitiesLabel);
            this.Name = "MultiObjectiveEvaluationResultView";
            this.Size = new System.Drawing.Size(386, 414);
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label qualitiesLabel;
    private System.Windows.Forms.TextBox qualitiesTextBox;
    private System.Windows.Forms.Label dataLabel;
    private MainForm.WindowsForms.ViewHost dataViewHost;
  }
}
