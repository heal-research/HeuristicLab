using System;
using System.Windows.Forms;

namespace HeuristicLab.Visualization.Options {
  public partial class OptionsDialog : Form {
    public OptionsDialog(IChartDataRowsModel model) {
      InitializeComponent();

      options.Model = model;
    }

    private void OptionsDialogOkButton_Click(object sender, EventArgs e) {
      Close();
    }

    private void OptionsDialogResetButton_Click(object sender, EventArgs e) {
      options.ResetSettings();
    }
  }
}