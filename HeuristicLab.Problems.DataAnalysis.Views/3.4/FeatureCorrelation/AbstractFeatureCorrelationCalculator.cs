#region License Information

/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public abstract class AbstractFeatureCorrelationCalculator {
    private readonly BackgroundWorker backgroundWorker;

    public AbstractFeatureCorrelationCalculator() {
      backgroundWorker = new BackgroundWorker { WorkerReportsProgress = true, WorkerSupportsCancellation = true };

      backgroundWorker.DoWork += BackgroundWorker_DoWork;
      backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
      backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
    }

    private BackgroundWorkerInfo bwInfo;
    public void TryCancelCalculation() {
      if (!backgroundWorker.IsBusy) return;

      bwInfo = null;
      backgroundWorker.CancelAsync();
    }

    protected static IEnumerable<int> GetRelevantIndices(IDataAnalysisProblemData problemData, string partition) {
      IEnumerable<int> var;
      if (partition.Equals(AbstractFeatureCorrelationView.TRAININGSAMPLES))
        var = problemData.TrainingIndices;
      else if (partition.Equals(AbstractFeatureCorrelationView.TESTSAMPLES))
        var = problemData.TestIndices;
      else var = Enumerable.Range(0, problemData.Dataset.Rows);
      return var;
    }

    #region backgroundworker

    protected void StartCalculation(BackgroundWorkerInfo info) {
      bwInfo = info;
      if (backgroundWorker.IsBusy) {
        backgroundWorker.CancelAsync();
        return;
      }

      backgroundWorker.RunWorkerAsync(bwInfo);
    }

    protected abstract void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e);

    private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
      BackgroundWorker worker = (BackgroundWorker)sender;
      if (!e.Cancelled && !worker.CancellationPending) {
        if (e.Error != null) {
          ErrorHandling.ShowErrorDialog(e.Error);
        } else {
          OnCorrelationCalculationFinished((double[,])e.Result, bwInfo.Calculator, bwInfo.Partition, bwInfo.IgnoreMissingValues, bwInfo.Variable);
        }
      } else if (bwInfo != null) {
        backgroundWorker.RunWorkerAsync(bwInfo);
      }
    }

    #endregion

    #region events
    public class CorrelationCalculationFinishedArgs : EventArgs {
      public double[,] Correlation { get; private set; }
      public IDependencyCalculator Calculcator { get; private set; }
      public string Partition { get; private set; }
      public bool IgnoreMissingValues { get; private set; }
      public string Variable { get; private set; }


      public CorrelationCalculationFinishedArgs(double[,] correlation, IDependencyCalculator calculator, string partition, bool ignoreMissingValues, string variable = null) {
        this.Correlation = correlation;
        this.Calculcator = calculator;
        this.Partition = partition;
        this.IgnoreMissingValues = ignoreMissingValues;
        this.Variable = variable;
      }
    }

    public delegate void CorrelationCalculationFinishedHandler(object sender, CorrelationCalculationFinishedArgs e);
    public event CorrelationCalculationFinishedHandler CorrelationCalculationFinished;
    protected virtual void OnCorrelationCalculationFinished(double[,] correlation, IDependencyCalculator calculator, string partition, bool ignoreMissingValues, string variable = null) {
      var handler = CorrelationCalculationFinished;
      if (handler != null)
        handler(this, new CorrelationCalculationFinishedArgs(correlation, calculator, partition, ignoreMissingValues, variable));
    }

    public event ProgressChangedEventHandler ProgressChanged;
    protected virtual void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
      var handler = ProgressChanged;
      if (handler != null) {
        handler(sender, e);
      }
    }
    #endregion

    protected class BackgroundWorkerInfo {
      public IDataset Dataset { get; set; }
      public IDependencyCalculator Calculator { get; set; }
      public string Partition { get; set; }
      public IEnumerable<int> Indices { get; set; }

      public bool IgnoreMissingValues { get; set; }

      public string Variable { get; set; }
      public int Frames { get; set; }
      public double[,] AlreadyCalculated { get; set; }
    }
  }
}
