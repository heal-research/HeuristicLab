#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [NonDiscoverableType]
  public class FeatureCorrelationCalculator : Object {

    private BackgroundWorker bw;
    private BackgroundWorkerInfo bwInfo;

    private IDataAnalysisProblemData problemData;
    public IDataAnalysisProblemData ProblemData {
      set {
        if (bw != null) {
          bw.CancelAsync();
        }
        problemData = value;
      }
    }

    public FeatureCorrelationCalculator()
      : base() { }

    public FeatureCorrelationCalculator(IDataAnalysisProblemData problemData)
      : base() {
      this.problemData = problemData;
    }

    public void CalculateElements(IDependencyCalculator calc, string partition) {
      CalculateElements(problemData.Dataset, calc, partition);
    }

    // returns true if any calculation takes place
    public bool CalculateTimeframeElements(IDependencyCalculator calc, string partition, string variable, int frames, double[,] correlation = null) {
      if (correlation == null || correlation.GetLength(1) <= frames) {
        CalculateElements(problemData.Dataset, calc, partition, variable, frames, correlation);
        return true;
      } else {
        return false;
      }
    }

    public void TryCancelCalculation() {
      if (bw != null && bw.IsBusy) {
        bwInfo = null;
        bw.CancelAsync();
      }
    }

    private void CalculateElements(Dataset dataset, IDependencyCalculator calc, string partition, string variable = null, int frames = 0, double[,] alreadyCalculated = null) {
      var indices = GetRelevantIndices(problemData, partition);
      bwInfo = new BackgroundWorkerInfo {
        Dataset = dataset, Calculator = calc, Partition = partition, Indices = indices,
        Variable = variable, Frames = frames, AlreadyCalculated = alreadyCalculated
      };
      if (bw == null) {
        bw = new BackgroundWorker();
        bw.WorkerReportsProgress = true;
        bw.WorkerSupportsCancellation = true;
        bw.DoWork += new DoWorkEventHandler(BwDoWork);
        bw.ProgressChanged += new ProgressChangedEventHandler(BwProgressChanged);
        bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BwRunWorkerCompleted);
      }
      if (bw.IsBusy) {
        bw.CancelAsync();
      } else {
        bw.RunWorkerAsync(bwInfo);
      }
    }

    private IEnumerable<int> GetRelevantIndices(IDataAnalysisProblemData problemData, string partition) {
      IEnumerable<int> var;
      if (partition.Equals(AbstractFeatureCorrelationView.TRAININGSAMPLES))
        var = problemData.TrainingIndices;
      else if (partition.Equals(AbstractFeatureCorrelationView.TESTSAMPLES))
        var = problemData.TestIndices;
      else var = Enumerable.Range(0, problemData.Dataset.Rows);
      return var;
    }

    #region backgroundworker
    private void BwDoWork(object sender, DoWorkEventArgs e) {
      BackgroundWorkerInfo bwInfo = (BackgroundWorkerInfo)e.Argument;
      if (bwInfo.Variable == null) {
        BwCalculateCorrelation(sender, e);
      } else {
        BwCalculateTimeframeCorrelation(sender, e);
      }
    }

    private void BwCalculateCorrelation(object sender, DoWorkEventArgs e) {
      BackgroundWorker worker = sender as BackgroundWorker;

      BackgroundWorkerInfo bwInfo = (BackgroundWorkerInfo)e.Argument;
      Dataset dataset = bwInfo.Dataset;
      IEnumerable<int> indices = bwInfo.Indices;
      IDependencyCalculator calc = bwInfo.Calculator;

      IList<string> doubleVariableNames = dataset.DoubleVariables.ToList();
      OnlineCalculatorError error = OnlineCalculatorError.None;
      int length = doubleVariableNames.Count;
      double[,] elements = new double[length, length];
      double calculations = (Math.Pow(length, 2) + length) / 2;

      worker.ReportProgress(0);

      for (int i = 0; i < length; i++) {
        for (int j = 0; j < i + 1; j++) {
          if (worker.CancellationPending) {
            worker.ReportProgress(100);
            e.Cancel = true;
            return;
          }
          IEnumerable<double> var1 = problemData.Dataset.GetDoubleValues(doubleVariableNames[i], indices);
          IEnumerable<double> var2 = problemData.Dataset.GetDoubleValues(doubleVariableNames[j], indices);

          elements[i, j] = calc.Calculate(var1, var2, out error);

          if (!error.Equals(OnlineCalculatorError.None)) {
            elements[i, j] = double.NaN;
          }
          elements[j, i] = elements[i, j];
          worker.ReportProgress((int)Math.Round((((Math.Pow(i, 2) + i) / 2 + j + 1.0) / calculations) * 100));
        }
      }
      e.Result = elements;
      worker.ReportProgress(100);
    }

    private void BwCalculateTimeframeCorrelation(object sender, DoWorkEventArgs e) {
      BackgroundWorker worker = sender as BackgroundWorker;

      BackgroundWorkerInfo bwInfo = (BackgroundWorkerInfo)e.Argument;
      Dataset dataset = bwInfo.Dataset;
      IEnumerable<int> indices = bwInfo.Indices;
      IDependencyCalculator calc = bwInfo.Calculator;
      string variable = bwInfo.Variable;
      int frames = bwInfo.Frames;
      double[,] alreadyCalculated = bwInfo.AlreadyCalculated;

      IList<string> doubleVariableNames = dataset.DoubleVariables.ToList();
      OnlineCalculatorError error = OnlineCalculatorError.None;
      int length = doubleVariableNames.Count;
      double[,] elements = new double[length, frames + 1];
      double calculations = (frames + 1) * length;

      worker.ReportProgress(0);

      int start = 0;
      if (alreadyCalculated != null) {
        for (int i = 0; i < alreadyCalculated.GetLength(0); i++) {
          Array.Copy(alreadyCalculated, i * alreadyCalculated.GetLength(1), elements, i * elements.GetLength(1), alreadyCalculated.GetLength(1));
        }
        start = alreadyCalculated.GetLength(1);
      }

      for (int i = 0; i < length; i++) {
        for (int j = start; j <= frames; j++) {
          if (worker.CancellationPending) {
            worker.ReportProgress(100);
            e.Cancel = true;
            return;
          }

          IEnumerable<double> var1 = problemData.Dataset.GetDoubleValues(variable, indices);
          IEnumerable<double> var2 = problemData.Dataset.GetDoubleValues(doubleVariableNames[i], indices);

          var valuesInFrame = var1.Take(j);
          var help = var1.Skip(j).ToList();
          help.AddRange(valuesInFrame);
          var1 = help;

          elements[i, j] = calc.Calculate(var1, var2, out error);

          if (!error.Equals(OnlineCalculatorError.None)) {
            elements[i, j] = double.NaN;
          }
          worker.ReportProgress((int)((100.0 / calculations) * (i * (frames + 1) + j + 1)));
        }
      }
      e.Result = elements;
      worker.ReportProgress(100);
    }

    private void BwRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
      BackgroundWorker worker = sender as BackgroundWorker;
      if (!e.Cancelled && !worker.CancellationPending) {
        if (e.Error != null) {
          ErrorHandling.ShowErrorDialog(e.Error);
        } else {
          OnCorrelationCalculationFinished((double[,])e.Result, bwInfo.Calculator, bwInfo.Partition, bwInfo.Variable);
        }
      } else if (bwInfo != null) {
        bw.RunWorkerAsync(bwInfo);
      }
    }
    #endregion

    #region events
    public class CorrelationCalculationFinishedArgs : EventArgs {
      public double[,] Correlation { get; private set; }
      public IDependencyCalculator Calculcator { get; private set; }
      public string Partition { get; private set; }
      public string Variable { get; private set; }

      public CorrelationCalculationFinishedArgs(double[,] correlation, IDependencyCalculator calculator, string partition, string variable = null) {
        this.Correlation = correlation;
        this.Calculcator = calculator;
        this.Partition = partition;
        this.Variable = variable;
      }
    }

    public delegate void CorrelationCalculationFinishedHandler(object sender, CorrelationCalculationFinishedArgs e);
    public event CorrelationCalculationFinishedHandler CorrelationCalculationFinished;
    protected virtual void OnCorrelationCalculationFinished(double[,] correlation, IDependencyCalculator calculator, string partition, string variable = null) {
      var handler = CorrelationCalculationFinished;
      if (handler != null)
        handler(this, new CorrelationCalculationFinishedArgs(correlation, calculator, partition, variable));
    }

    public delegate void ProgressCalculationHandler(object sender, ProgressChangedEventArgs e);
    public event ProgressCalculationHandler ProgressCalculation;
    protected void BwProgressChanged(object sender, ProgressChangedEventArgs e) {
      BackgroundWorker worker = sender as BackgroundWorker;
      if (ProgressCalculation != null) {
        ProgressCalculation(sender, e);
      }
    }
    #endregion

    private class BackgroundWorkerInfo {
      public Dataset Dataset { get; set; }
      public IDependencyCalculator Calculator { get; set; }
      public string Partition { get; set; }
      public IEnumerable<int> Indices { get; set; }
      public string Variable { get; set; }
      public int Frames { get; set; }
      public double[,] AlreadyCalculated { get; set; }
    }
  }
}
