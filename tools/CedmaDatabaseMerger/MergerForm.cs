using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Modeling.Database.SQLServerCompact;
using HeuristicLab.Modeling;
using HeuristicLab.GP;
using HeuristicLab.Core;
using HeuristicLab.Modeling.Database;
using System.Reflection;

namespace CedmaDatabaseMerger {
  public partial class MergerForm : Form {
    string destinationFile;
    public MergerForm() {
      InitializeComponent();
    }

    private void setOutputButton_Click(object sender, EventArgs e) {
      using (OpenFileDialog dialog = new OpenFileDialog()) {
        DialogResult result = dialog.ShowDialog();
        if (result == DialogResult.OK) {
          outputTextBox.Text = dialog.FileName;
          destinationFile = dialog.FileName;
          importButton.Enabled = true;
        } else {
          outputTextBox.Text = string.Empty;
          destinationFile = string.Empty;
          importButton.Enabled = false;
        }
      }
    }

    private void importButton_Click(object sender, EventArgs e) {
      string importFileName;
      using (OpenFileDialog dialog = new OpenFileDialog()) {
        DialogResult result = dialog.ShowDialog();
        if (result == DialogResult.OK) {
          importFileName = dialog.FileName;
        } else {
          importFileName = string.Empty;
        }
      }
      if (!string.IsNullOrEmpty(importFileName)) {
        importButton.Enabled = false;
        BackgroundWorker worker = new BackgroundWorker();
        worker.WorkerReportsProgress = true;
        worker.DoWork += CreateDoWorkDelegate(worker, importFileName);
        worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
        worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
        worker.RunWorkerAsync();
      }
    }

    void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
      importButton.Enabled = true;
      importProgressBar.Value = 0;
    }

    void worker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
      importProgressBar.Value = e.ProgressPercentage;
    }

    private DoWorkEventHandler CreateDoWorkDelegate(BackgroundWorker worker, string importFileName) {
      return (sender, args) => {
        DatabaseService destiationDatabase = new DatabaseService("Data Source=" + destinationFile);
        DatabaseService sourceDatabase = new DatabaseService("Data Source=" + importFileName);

        sourceDatabase.Connect();
        var models = sourceDatabase.GetAllModels();
        var sourceDataset = sourceDatabase.GetDataset();
        int importCount = 0;
        foreach (HeuristicLab.Modeling.Database.IModel m in models) {

          HeuristicLab.Modeling.IAnalyzerModel model = new AnalyzerModel();
          model.Predictor = (HeuristicLab.Modeling.IPredictor)PersistenceManager.RestoreFromGZip(sourceDatabase.GetModelData(m));
          model.TargetVariable = m.TargetVariable.Name;
          model.Dataset = sourceDataset;
          model.TrainingSamplesStart = m.TrainingSamplesStart;
          model.TrainingSamplesEnd = m.TrainingSamplesEnd;
          model.ValidationSamplesStart = m.ValidationSamplesStart;
          model.ValidationSamplesEnd = m.ValidationSamplesEnd;
          model.TestSamplesStart = m.TestSamplesStart;
          model.TestSamplesEnd = m.TestSamplesEnd;
          model.Predictor.Predict(sourceDataset, 10, 20);
          //get all double properties to save as modelResult
          IEnumerable<PropertyInfo> modelResultInfos = model.GetType().GetProperties().Where(
            info => info.PropertyType == typeof(double));
          var modelResults = sourceDatabase.GetModelResults(m);
          foreach (IModelResult result in modelResults) {
            PropertyInfo matchingPropInfo = modelResultInfos.First(x => x.Name == result.Result.Name);
            if (matchingPropInfo != null) matchingPropInfo.SetValue(model, result.Value, null);
          }
          var inputVariableResults = sourceDatabase.GetInputVariableResults(m);
          foreach (IInputVariableResult result in inputVariableResults) {
            model.AddInputVariable(result.Variable.Name);
            if (result.Result.Name == "VariableEvaluationImpact") {
              model.SetVariableEvaluationImpact(result.Variable.Name, result.Value);
            } else if (result.Result.Name == "VariableQualityImpact") {
              model.SetVariableQualityImpact(result.Variable.Name, result.Value);
            } else throw new FormatException();
          }

          destiationDatabase.Persist(model, m.Algorithm.Name, m.Algorithm.Description);
          worker.ReportProgress((++importCount * 100) / models.Count());
        }
        sourceDatabase.Disconnect();
      };
    }
  }
}
