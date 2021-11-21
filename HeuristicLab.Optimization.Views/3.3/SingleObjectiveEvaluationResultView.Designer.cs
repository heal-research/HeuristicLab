
namespace HeuristicLab.Optimization.Views {
  partial class SingleObjectiveEvaluationResultView {
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
            this.qualityLabel = new System.Windows.Forms.Label();
            this.qualityTextBox = new System.Windows.Forms.TextBox();
            this.dataLabel = new System.Windows.Forms.Label();
            this.dataViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
            this.SuspendLayout();
            // 
            // qualityLabel
            // 
            this.qualityLabel.AutoSize = true;
            this.qualityLabel.Location = new System.Drawing.Point(3, 6);
            this.qualityLabel.Name = "qualityLabel";
            this.qualityLabel.Size = new System.Drawing.Size(42, 13);
            this.qualityLabel.TabIndex = 0;
            this.qualityLabel.Text = "Quality:";
            // 
            // qualityTextBox
            // 
            this.qualityTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.qualityTextBox.Location = new System.Drawing.Point(51, 3);
            this.qualityTextBox.Name = "qualityTextBox";
            this.qualityTextBox.ReadOnly = true;
            this.qualityTextBox.Size = new System.Drawing.Size(335, 20);
            this.qualityTextBox.TabIndex = 1;
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
            // SingleObjectiveEvaluationResultView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataViewHost);
            this.Controls.Add(this.qualityTextBox);
            this.Controls.Add(this.dataLabel);
            this.Controls.Add(this.qualityLabel);
            this.Name = "SingleObjectiveEvaluationResultView";
            this.Size = new System.Drawing.Size(386, 414);
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label qualityLabel;
    private System.Windows.Forms.TextBox qualityTextBox;
    private System.Windows.Forms.Label dataLabel;
    private MainForm.WindowsForms.ViewHost dataViewHost;
  }
}
