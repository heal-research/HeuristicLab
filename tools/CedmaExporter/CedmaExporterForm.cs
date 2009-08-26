using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using HeuristicLab.Core;
using HeuristicLab.DataAnalysis;
using HeuristicLab.Modeling.Database.SQLServerCompact;

namespace CedmaExporter {
  public partial class CedmaExporterForm : Form {
    private BackgroundWorker worker;
    public CedmaExporterForm() {
      InitializeComponent();
    }

    private void exportButton_Click(object sender, EventArgs e) {
      var dialog = new OpenFileDialog();
      DialogResult result;
      result = dialog.ShowDialog();
      if (result == DialogResult.OK) {
        string fileName = dialog.FileName;
        exportButton.Enabled = false;
        cancelButton.Enabled = true;
        worker = new BackgroundWorker();
        worker.WorkerReportsProgress = true;
        worker.WorkerSupportsCancellation = true;
        worker.DoWork += CreateWorkerDelegate(worker, fileName);
        worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
        worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
        worker.RunWorkerAsync();
      }
    }

    private DoWorkEventHandler CreateWorkerDelegate(BackgroundWorker worker, string fileName) {
      return (sender, args) => {
        string inputFileName = fileName;
        string outputFileName = fileName.Replace(".sdf", "") + ".txt";
        string sqlCompactConnectionString = "sqlite:rdf:Data Source=\"" + inputFileName + "\"";
        using (StreamWriter writer = File.CreateText(outputFileName)) {
          DatabaseService database = new DatabaseService("Data Source=" + fileName);
          database.Connect();
          var models = database.GetAllModels();
          var dataset = database.GetDataset();
          CedmaExporter.WriteColumnHeaders(writer);
          List<string> inputVariables = CedmaExporter.WriteVariableImpactHeaders(database, writer);
          writer.WriteLine();
          int i = 0;
          var exporter = new ModelExporter(dataset, Path.GetDirectoryName(outputFileName), true);
          var allModels = database.GetAllModels();
          foreach (HeuristicLab.Modeling.Database.IModel m in allModels) {
            CedmaExporter.WriteModel(m, ++i, database, writer, inputVariables, exporter);
            worker.ReportProgress((i * 100) / allModels.Count());
            if (worker.CancellationPending) return;
          }
        }
      };
    }

    void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
      exportButton.Enabled = true;
      cancelButton.Enabled = false;
      progressBar.Value = 0;
    }

    void worker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
      progressBar.Value = e.ProgressPercentage;
    }

    private void cancelButton_Click(object sender, EventArgs e) {
      worker.CancelAsync();
    }
  }
}
