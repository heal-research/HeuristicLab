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
using FCE = HeuristicLab.Problems.DataAnalysis.FeatureCorrelationEnums;

namespace HeuristicLab.Problems.DataAnalysis {
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

    public void CalculateElements(FCE.CorrelationCalculators calc, FCE.Partitions partition) {
      CalculateElements(problemData.Dataset, calc, partition);
    }

    // returns if any calculation takes place
    public bool CalculateTimeframeElements(FCE.CorrelationCalculators calc, FCE.Partitions partition, string variable, int frames, double[,] correlation = null) {
      if (correlation == null || correlation.GetLength(1) <= frames) {
        CalculateElements(problemData.Dataset, calc, partition, variable, frames, correlation);
        return true;
      } else {
        return false;
      }
    }

    private double[,] GetElementsOfCorrelation(double[,] corr, int frames) {
      double[,] elements = new double[corr.GetLength(0), frames + 1];
      for (int i = 0; i < corr.GetLength(0); i++) {
        for (int j = 0; j <= frames; j++) {
          elements[i, j] = corr[i, j];
        }
      }
      return elements;
    }

    private void CalculateElements(Dataset dataset, FCE.CorrelationCalculators calc, FCE.Partitions partition, string variable = null, int frames = 0, double[,] alreadyCalculated = null) {
      bwInfo = new BackgroundWorkerInfo { Dataset = dataset, Calculator = calc, Partition = partition, Variable = variable, Frames = frames, AlreadyCalculated = alreadyCalculated };
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
      FCE.Partitions partition = bwInfo.Partition;
      FCE.CorrelationCalculators calc = bwInfo.Calculator;

      IList<string> doubleVariableNames = dataset.DoubleVariables.ToList();
      OnlineCalculatorError error = OnlineCalculatorError.None;
      int length = doubleVariableNames.Count;
      double[,] elements = new double[length, length];
      double calculations = (Math.Pow(length, 2) + length) / 2;

      worker.ReportProgress(0);

      for (int i = 0; i < length; i++) {
        for (int j = 0; j < i + 1; j++) {
          if (worker.CancellationPending) {
            e.Cancel = true;
            return;
          }
          IEnumerable<double> var1 = GetRelevantValues(problemData, partition, doubleVariableNames[i]);
          IEnumerable<double> var2 = GetRelevantValues(problemData, partition, doubleVariableNames[j]);

          elements[i, j] = CalculateElementWithCalculator(calc, var1, var2, out error);

          if (!error.Equals(OnlineCalculatorError.None)) {
            elements[i, j] = double.NaN;
          }
          elements[j, i] = elements[i, j];
          worker.ReportProgress((int)Math.Round((((Math.Pow(i, 2) + i) / 2 + j + 1.0) / calculations) * 100));
        }
      }
      e.Result = elements;
    }

    private void BwCalculateTimeframeCorrelation(object sender, DoWorkEventArgs e) {
      BackgroundWorker worker = sender as BackgroundWorker;

      BackgroundWorkerInfo bwInfo = (BackgroundWorkerInfo)e.Argument;
      Dataset dataset = bwInfo.Dataset;
      FCE.Partitions partition = bwInfo.Partition;
      FCE.CorrelationCalculators calc = bwInfo.Calculator;
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
            e.Cancel = true;
            return;
          }

          IEnumerable<double> var1 = GetRelevantValues(problemData, partition, variable);
          IEnumerable<double> var2 = GetRelevantValues(problemData, partition, doubleVariableNames[i]);

          var valuesInFrame = var1.Take(j);
          var help = var1.Skip(j).ToList();
          help.AddRange(valuesInFrame);
          var1 = help;

          elements[i, j] = CalculateElementWithCalculator(calc, var1, var2, out error);

          if (!error.Equals(OnlineCalculatorError.None)) {
            elements[i, j] = double.NaN;
          }
          worker.ReportProgress((int)((100.0 / calculations) * (i * (frames + 1) + j + 1)));
        }
      }
      e.Result = elements;
    }

    private IEnumerable<double> GetRelevantValues(IDataAnalysisProblemData problemData, FCE.Partitions partition, string variable) {
      IEnumerable<double> var = problemData.Dataset.GetDoubleValues(variable);
      if (partition.Equals(FCE.Partitions.TrainingSamples)) {
        var = var.Skip(problemData.TrainingPartition.Start).Take(problemData.TrainingPartition.End - problemData.TrainingPartition.Start);
      } else if (partition.Equals(FCE.Partitions.TestSamples)) {
        var = var.Skip(problemData.TestPartition.Start).Take(problemData.TestPartition.End - problemData.TestPartition.Start);
      }
      return var;
    }

    private double CalculateElementWithCalculator(FCE.CorrelationCalculators calc, IEnumerable<double> var1, IEnumerable<double> var2, out OnlineCalculatorError error) {
      if (calc.Equals(FCE.CorrelationCalculators.HoeffdingsDependence)) {
        return HoeffdingsDependenceCalculator.Calculate(var1, var2, out error);
      } else if (calc.Equals(FCE.CorrelationCalculators.SpearmansRank)) {
        return SpearmansRankCorrelationCoefficientCalculator.Calculate(var1, var2, out error);
      } else if (calc.Equals(FCE.CorrelationCalculators.PearsonsRSquared)) {
        return OnlinePearsonsRSquaredCalculator.Calculate(var1, var2, out error);
      } else {
        return Math.Sqrt(OnlinePearsonsRSquaredCalculator.Calculate(var1, var2, out error));
      }
    }

    private void BwRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
      BackgroundWorker worker = sender as BackgroundWorker;
      if (!e.Cancelled && !worker.CancellationPending) {
        if (!(e.Error == null)) {
          ErrorHandling.ShowErrorDialog(e.Error);
        } else {
          OnCorrelationCalculationFinished((double[,])e.Result, bwInfo.Calculator, bwInfo.Partition, bwInfo.Variable);
        }
      } else {
        bw.RunWorkerAsync(bwInfo);
      }
    }
    #endregion

    #region events
    public class CorrelationCalculationFinishedArgs : EventArgs {
      public double[,] Correlation { get; private set; }
      public FCE.CorrelationCalculators Calculcator { get; private set; }
      public FCE.Partitions Partition { get; private set; }
      public string Variable { get; private set; }

      public CorrelationCalculationFinishedArgs(double[,] correlation, FCE.CorrelationCalculators calculator, FCE.Partitions partition, string variable = null) {
        this.Correlation = correlation;
        this.Calculcator = calculator;
        this.Partition = partition;
        this.Variable = variable;
      }
    }
    public delegate void CorrelationCalculationFinishedHandler(object sender, CorrelationCalculationFinishedArgs e);
    public event CorrelationCalculationFinishedHandler CorrelationCalculationFinished;
    protected virtual void OnCorrelationCalculationFinished(double[,] correlation, FCE.CorrelationCalculators calculator, FCE.Partitions partition, string variable = null) {
      var handler = CorrelationCalculationFinished;
      if (handler != null)
        handler(this, new CorrelationCalculationFinishedArgs(correlation, calculator, partition, variable));
    }

    public delegate void ProgressCalculationHandler(object sender, ProgressChangedEventArgs e);
    public event ProgressCalculationHandler ProgressCalculation;
    protected void BwProgressChanged(object sender, ProgressChangedEventArgs e) {
      BackgroundWorker worker = sender as BackgroundWorker;
      if (!worker.CancellationPending && ProgressCalculation != null) {
        ProgressCalculation(sender, e);
      }
    }
    #endregion

    protected class BackgroundWorkerInfo {
      public Dataset Dataset { get; set; }
      public FCE.CorrelationCalculators Calculator { get; set; }
      public FCE.Partitions Partition { get; set; }
      public string Variable { get; set; }
      public int Frames { get; set; }
      public double[,] AlreadyCalculated { get; set; }
    }
  }
}
