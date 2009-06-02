namespace HeuristicLab.Visualization.Options {
  partial class OptionsDialog {
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
      HeuristicLab.Visualization.ChartDataRowsModel chartDataRowsModel1 = new HeuristicLab.Visualization.ChartDataRowsModel();
      HeuristicLab.Visualization.Options.ViewSettings viewSettings1 = new HeuristicLab.Visualization.Options.ViewSettings();
      HeuristicLab.Visualization.LabelProvider.ContinuousLabelProvider continuousLabelProvider1 = new HeuristicLab.Visualization.LabelProvider.ContinuousLabelProvider();
      this.OptionsDialogCancelButton = new System.Windows.Forms.Button();
      this.OptionsDialogOkButton = new System.Windows.Forms.Button();
      this.fdFont = new System.Windows.Forms.FontDialog();
      this.options = new HeuristicLab.Visualization.Options.Options();
      this.SuspendLayout();
      // 
      // OptionsDialogCancelButton
      // 
      this.OptionsDialogCancelButton.Location = new System.Drawing.Point(225, 241);
      this.OptionsDialogCancelButton.Name = "OptionsDialogCancelButton";
      this.OptionsDialogCancelButton.Size = new System.Drawing.Size(75, 23);
      this.OptionsDialogCancelButton.TabIndex = 1;
      this.OptionsDialogCancelButton.Text = "Reset";
      this.OptionsDialogCancelButton.UseVisualStyleBackColor = true;
      this.OptionsDialogCancelButton.Click += new System.EventHandler(this.OptionsDialogResetButton_Click);
      // 
      // OptionsDialogOkButton
      // 
      this.OptionsDialogOkButton.Location = new System.Drawing.Point(132, 241);
      this.OptionsDialogOkButton.Name = "OptionsDialogOkButton";
      this.OptionsDialogOkButton.Size = new System.Drawing.Size(75, 23);
      this.OptionsDialogOkButton.TabIndex = 2;
      this.OptionsDialogOkButton.Text = "OK";
      this.OptionsDialogOkButton.UseVisualStyleBackColor = true;
      this.OptionsDialogOkButton.Click += new System.EventHandler(this.OptionsDialogOkButton_Click);
      // 
      // fdFont
      // 
      this.fdFont.ShowColor = true;
      // 
      // options
      // 
      this.options.Location = new System.Drawing.Point(2, 3);
      chartDataRowsModel1.XAxis.ShowLabel = true;
      chartDataRowsModel1.Title = "Title";
      viewSettings1.LegendColor = System.Drawing.Color.Blue;
      viewSettings1.LegendFont = new System.Drawing.Font("Arial", 8F);
      viewSettings1.LegendPosition = HeuristicLab.Visualization.Legend.LegendPosition.Bottom;
      viewSettings1.TitleColor = System.Drawing.Color.Blue;
      viewSettings1.TitleFont = new System.Drawing.Font("Arial", 8F);
      chartDataRowsModel1.ViewSettings = viewSettings1;
      chartDataRowsModel1.XAxis.Label = "";
      chartDataRowsModel1.XAxis.LabelProvider = continuousLabelProvider1;
      this.options.Model = chartDataRowsModel1;
      this.options.Name = "options";
      this.options.Size = new System.Drawing.Size(298, 232);
      this.options.TabIndex = 3;
      // 
      // OptionsDialog
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(302, 270);
      this.Controls.Add(this.options);
      this.Controls.Add(this.OptionsDialogOkButton);
      this.Controls.Add(this.OptionsDialogCancelButton);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.Name = "OptionsDialog";
      this.Text = "OptionsDialog";
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button OptionsDialogCancelButton;
    private System.Windows.Forms.Button OptionsDialogOkButton;
    private System.Windows.Forms.FontDialog fdFont;
    private Options options;
  }
}
