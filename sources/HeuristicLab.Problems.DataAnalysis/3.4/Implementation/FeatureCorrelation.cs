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
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.DataAnalysis {
  [StorableClass]
  [Item("FeatureCorrelation", "Represents the correlation of features in a data set.")]
  public class FeatureCorrelation : HeatMap {
    private const string PearsonsR = "Pearsons R";
    private const string PearsonsRSquared = "Pearsons R Squared";
    private const string HoeffdingsDependence = "Hoeffdings Dependence";
    private const string SpearmansRank = "Spearmans Rank";
    public IEnumerable<string> CorrelationCalculators {
      get { return new List<string>() { PearsonsR, PearsonsRSquared, HoeffdingsDependence, SpearmansRank }; }
    }

    private const string AllSamples = "All Samples";
    private const string TrainingSamples = "Training Samples";
    private const string TestSamples = "Test Samples";
    public IEnumerable<string> Partitions {
      get { return new List<string>() { AllSamples, TrainingSamples, TestSamples }; }
    }

    private IDataAnalysisProblemData problemData;
    [Storable]
    public IDataAnalysisProblemData ProblemData {
      get { return problemData; }
      set {
        if (problemData != value) {
          problemData = value;
          columnNames = value.Dataset.DoubleVariables.ToList();
          rowNames = value.Dataset.DoubleVariables.ToList();
          OnProblemDataChanged();
        }
      }
    }

    private BackgroundWorker bw;
    private BackgroundWorkerInfo bwInfo;

    public FeatureCorrelation() {
      this.Title = "Feature Correlation";
      this.columnNames = problemData.Dataset.DoubleVariables.ToList();
      this.rowNames = problemData.Dataset.DoubleVariables.ToList();
      sortableView = true;
    }
    public FeatureCorrelation(IDataAnalysisProblemData problemData)
      : base() {
      this.problemData = problemData;
      this.Title = "Feature Correlation";
      this.columnNames = problemData.Dataset.DoubleVariables.ToList();
      this.rowNames = problemData.Dataset.DoubleVariables.ToList();
      sortableView = true;
    }
    protected FeatureCorrelation(FeatureCorrelation original, Cloner cloner)
      : base(original, cloner) {
      this.problemData = cloner.Clone(original.problemData);
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new FeatureCorrelation(this, cloner);
    }

    [StorableConstructor]
    protected FeatureCorrelation(bool deserializing) : base(deserializing) { }

    public void Recalculate(string calc, string partition) {
      CalculateElements(problemData.Dataset, calc, partition);
    }

    public void CalculateTimeframeElements(string calc, string partition, string variable, int frames) {
      CalculateElements(problemData.Dataset, calc, partition, variable, frames);
    }

    private void CalculateElements(Dataset dataset) {
      CalculateElements(dataset, CorrelationCalculators.First(), Partitions.First());
    }

    private void CalculateElements(Dataset dataset, string calc, string partition, string variable = null, int frames = 0) {
      bwInfo = new BackgroundWorkerInfo { Dataset = dataset, Calculator = calc, Partition = partition, Variable = variable, Frames = frames };
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
      if (calc.Equals(PearsonsR) || calc.Equals(SpearmansRank)) {
        Maximum = 1.0;
        Minimum = -1.0;
      } else if (calc.Equals(HoeffdingsDependence)) {
        Maximum = 1.0;
        Minimum = -0.5;
      } else {
        Maximum = 1.0;
        Minimum = 0.0;
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
      string partition = bwInfo.Partition;
      string calc = bwInfo.Calculator;

      IList<string> doubleVariableNames = dataset.DoubleVariables.ToList();
      OnlineCalculatorError error;
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

          elements[j, i] = elements[i, j];
          if (!error.Equals(OnlineCalculatorError.None)) {
            worker.ReportProgress(100);
            throw new ArgumentException("Calculator returned " + error + Environment.NewLine + "Maybe try another calculator.");
          }
          worker.ReportProgress((int)Math.Round((((Math.Pow(i, 2) + i) / 2 + j + 1.0) / calculations) * 100));
        }
      }
      e.Result = elements;
    }

    private void BwCalculateTimeframeCorrelation(object sender, DoWorkEventArgs e) {
      BackgroundWorker worker = sender as BackgroundWorker;

      BackgroundWorkerInfo bwInfo = (BackgroundWorkerInfo)e.Argument;
      Dataset dataset = bwInfo.Dataset;
      string partition = bwInfo.Partition;
      string calc = bwInfo.Calculator;
      string variable = bwInfo.Variable;
      int frames = bwInfo.Frames;

      IList<string> doubleVariableNames = dataset.DoubleVariables.ToList();
      OnlineCalculatorError error;
      int length = doubleVariableNames.Count;
      double[,] elements = new double[length, frames + 1];
      double calculations = (frames + 1) * length;

      worker.ReportProgress(0);

      for (int i = 0; i < length; i++) {
        for (int j = 0; j <= frames; j++) {
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
            worker.ReportProgress(100);
            throw new ArgumentException("Calculator returned " + error + Environment.NewLine + "Maybe try another calculator.");
          }
          worker.ReportProgress((int)((100.0 / calculations) * (i * (frames + 1) + j + 1)));
        }
      }
      e.Result = elements;
    }

    private IEnumerable<double> GetRelevantValues(IDataAnalysisProblemData problemData, string partition, string variable) {
      IEnumerable<double> var = problemData.Dataset.GetDoubleValues(variable);
      if (partition.Equals(TrainingSamples)) {
        var = var.Skip(problemData.TrainingPartition.Start).Take(problemData.TrainingPartition.End - problemData.TrainingPartition.Start);
      } else if (partition.Equals(TestSamples)) {
        var = var.Skip(problemData.TestPartition.Start).Take(problemData.TestPartition.End - problemData.TestPartition.Start);
      }
      return var;
    }

    private double CalculateElementWithCalculator(string calc, IEnumerable<double> var1, IEnumerable<double> var2, out OnlineCalculatorError error) {
      if (calc.Equals(HoeffdingsDependence)) {
        return HoeffdingsDependenceCalculator.Calculate(var1, var2, out error);
      } else if (calc.Equals(SpearmansRank)) {
        return SpearmansRankCorrelationCoefficientCalculator.Calculate(var1, var2, out error);
      } else if (calc.Equals(PearsonsRSquared)) {
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
          matrix = (double[,])e.Result;
          OnCorrelationCalculationFinished();
        }
      } else {
        bw.RunWorkerAsync(bwInfo);
      }
    }
    #endregion

    #region events
    public event EventHandler CorrelationCalculationFinished;
    protected virtual void OnCorrelationCalculationFinished() {
      EventHandler handler = CorrelationCalculationFinished;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }

    public delegate void ProgressCalculationHandler(object sender, ProgressChangedEventArgs e);
    public event ProgressCalculationHandler ProgressCalculation;
    protected void BwProgressChanged(object sender, ProgressChangedEventArgs e) {
      BackgroundWorker worker = sender as BackgroundWorker;
      if (!worker.CancellationPending && ProgressCalculation != null) {
        ProgressCalculation(sender, e);
      }
    }

    public event EventHandler ProblemDataChanged;
    protected virtual void OnProblemDataChanged() {
      EventHandler handler = ProblemDataChanged;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }
    #endregion

    protected class BackgroundWorkerInfo {
      public Dataset Dataset { get; set; }
      public string Calculator { get; set; }
      public string Partition { get; set; }
      public string Variable { get; set; }
      public int Frames { get; set; }
    }
  }
}
