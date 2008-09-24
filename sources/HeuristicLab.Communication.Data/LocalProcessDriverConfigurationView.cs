using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Communication.Data {
  public partial class LocalProcessDriverConfigurationView : ViewBase {
    public LocalProcessDriverConfiguration LocalProcessDriverConfiguration {
      get { return (LocalProcessDriverConfiguration)base.Item; }
      set { base.Item = value; }
    }

    public LocalProcessDriverConfigurationView() {
      InitializeComponent();
    }

    public LocalProcessDriverConfigurationView(LocalProcessDriverConfiguration localProcessDriverConfiguration)
      : this() {
      LocalProcessDriverConfiguration = localProcessDriverConfiguration;
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      if (LocalProcessDriverConfiguration == null) {
        executablePathStringDataView.Enabled = false;
        executablePathStringDataView.StringData = null;
        argumentsStringDataView.Enabled = false;
        argumentsStringDataView.StringData = null;
      } else {
        executablePathStringDataView.StringData = LocalProcessDriverConfiguration.ExecutablePath;
        executablePathStringDataView.Enabled = true;
        argumentsStringDataView.StringData = LocalProcessDriverConfiguration.Arguments;
        argumentsStringDataView.Enabled = true;
      }
    }

    private void browseExecutableButtom_Click(object sender, EventArgs e) {
      if (LocalProcessDriverConfiguration != null && executablePathOpenFileDialog.ShowDialog() == DialogResult.OK) {
        LocalProcessDriverConfiguration.ExecutablePath = new StringData(executablePathOpenFileDialog.FileName);
      }
    }
  }
}
