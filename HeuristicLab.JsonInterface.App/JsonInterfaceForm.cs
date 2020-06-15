using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.JsonInterface.App {
  public partial class JsonInterfaceForm : Form {
    private OpenFileDialog openDialog = null;
    private SaveFileDialog saveDialog = null;
    private string templatePath = null;
    private string configPath = null;
    private string outputPath = null;
    private int loadingCounter = 0;

    public JsonInterfaceForm() {
      openDialog = new OpenFileDialog();
      openDialog.Multiselect = false;
      openDialog.DefaultExt = ".json";
      openDialog.AddExtension = true;
      openDialog.Filter = "Json-File (*.json)|*.json";

      saveDialog = new SaveFileDialog();
      saveDialog.DefaultExt = ".json";
      saveDialog.AddExtension = true;
      saveDialog.Filter = "Json-File (*.json)|*.json";

      InitializeComponent();

      this.templateOpenButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Open;
      this.configOpenButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Open;
      this.outputOpenButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Save;
    }

    private void OpenTemplate(object sender, EventArgs e) {
      if(openDialog.ShowDialog() == DialogResult.OK) {
        templatePath = openDialog.FileName;
        templateTextBox.Text = templatePath;
      }
    }

    private void OpenConfig(object sender, EventArgs e) {
      if (openDialog.ShowDialog() == DialogResult.OK) {
        configPath = openDialog.FileName;
        configTextBox.Text = configPath;
      }
    }

    private void OpenOutput(object sender, EventArgs e) {
      if (saveDialog.ShowDialog() == DialogResult.OK) {
        outputPath = saveDialog.FileName;
        outputTextBox.Text = outputPath;
      }
    }

    /// <summary>
    /// Run button onClick logic
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void Run(object sender, EventArgs e) {
      runButton.Enabled = false;
      CancellationTokenSource cts = new CancellationTokenSource();
      var token = cts.Token;

      // update text on run button to show a working program (loading/working)
      Task uiUpdater = Task.Run(async () => {
        while(!token.IsCancellationRequested) {

          StringBuilder sb = new StringBuilder("Run");
          sb.Append('.', loadingCounter);
          UpdateButton(sb.ToString());
          loadingCounter = ++loadingCounter % 3;
          await Task.Delay(500);
        }
      }, token);

      // performs the json interfaces run
      Task work = Task.Run(() => {
        if(templatePath != null && configPath != null && outputPath != null) {
          try {
            Runner.Run(templatePath, configPath, outputPath);
          } catch (Exception ex) {
            ErrorHandling.ShowErrorDialog(this, ex);
          }
        }
        cts.Cancel();
      });

      await Task.WhenAll(work, uiUpdater);

      runButton.Enabled = true;
      runButton.Text = "Run";
    }

    /// <summary>
    /// Update displayed text for run button.
    /// </summary>
    /// <param name="text"></param>
    private void UpdateButton(string text) {
      if (InvokeRequired) {
        Invoke((Action<string>) UpdateButton, text);
      } else {
        runButton.Text = text;
      }
    }
  }
}
