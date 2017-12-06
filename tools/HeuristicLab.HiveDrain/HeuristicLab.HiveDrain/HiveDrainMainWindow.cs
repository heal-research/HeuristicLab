using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using HeuristicLab.Clients.Hive;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.HiveDrain {
  public partial class HiveDrainMainWindow : Form {
    public HiveDrainMainWindow() {
      InitializeComponent();
      ContentManager.Initialize(new PersistenceContentManager());
      ListHiveJobs();
    }

    private System.Threading.Tasks.Task task;

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

      var directory = outputDirectoryTextBox.Text;

      if (string.IsNullOrEmpty(directory))
        directory = Environment.CurrentDirectory;

      JobDownloader jobDownloader = new JobDownloader(directory, pattern, Log, oneFileCheckBox.Checked);
      task = new System.Threading.Tasks.Task(jobDownloader.Start);
      task.ContinueWith(x => { Log.LogMessage("All tasks written, quitting."); EnableButton(); }, TaskContinuationOptions.OnlyOnRanToCompletion);
      task.ContinueWith(x => { Log.LogMessage("Unexpected Exception while draining the Hive: " + x.Exception.ToString()); EnableButton(); }, TaskContinuationOptions.OnlyOnFaulted);
      task.Start();
    }

    private void HiveDrainMainWindow_FormClosing(object sender, FormClosingEventArgs e) {
      //TODO: implement task cancelation
    }

    private void ListHiveJobs() {
      if (logView.Content == null)
        logView.Content = Log;
      Log.Clear();
      var jobs = HiveServiceLocator.Instance.CallHiveService<IEnumerable<Job>>(s => s.GetJobs());
      foreach (var job in jobs) {
        Log.LogMessage(string.Format("{0}\t{1}", job.DateCreated, job.Name));
      }
    }

    private void browseOutputPathButton_Click(object sender, EventArgs e) {
      var path = string.Empty;

      var dialog = new FolderBrowserDialog {
        RootFolder = Environment.SpecialFolder.MyComputer
      };

      if (dialog.ShowDialog() == DialogResult.OK) {
        path = dialog.SelectedPath;
      }

      if (!string.IsNullOrEmpty(path))
        outputDirectoryTextBox.Text = path;
    }

    private void listJobsButton_Click(object sender, EventArgs e) {
      ListHiveJobs();
    }
  }
}
