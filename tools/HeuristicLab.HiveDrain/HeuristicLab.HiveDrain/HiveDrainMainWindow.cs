using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.HiveDrain {
  public partial class HiveDrainMainWindow : Form {
    public HiveDrainMainWindow() {
      InitializeComponent();
      ContentManager.Initialize(new PersistenceContentManager());
    }

    private Task task;

    public static ThreadSafeLog Log = new ThreadSafeLog();

    private void EnableButton() {
      if (InvokeRequired)
        Invoke(new Action(EnableButton));
      else
        downloadButton.Enabled = true;
    }

    private void downloadButton_Click(object sender, EventArgs e) {
      string pattern = (patterTextBox.Text.Trim().Length > 0) ? patterTextBox.Text.Trim() : null;
      Log.Clear();
      logView.Content = Log;
      downloadButton.Enabled = false;

      JobDownloader jobDownloader = new JobDownloader(Environment.CurrentDirectory, pattern, Log, oneFileCheckBox.Checked);
      task = new Task(jobDownloader.Start);
      task.ContinueWith(x => { Log.LogMessage("All tasks written, quitting."); EnableButton(); }, TaskContinuationOptions.OnlyOnRanToCompletion);
      task.ContinueWith(x => { Log.LogMessage("Unexpected Exception while draining the Hive: " + x.Exception.ToString()); EnableButton(); }, TaskContinuationOptions.OnlyOnFaulted);
      task.Start();
    }

    private void HiveDrainMainWindow_FormClosing(object sender, FormClosingEventArgs e) {
      //TODO: implement task cancelation
    }
  }
}
