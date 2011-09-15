#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.Data;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Optimization;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Views {
  [Content(typeof(RunCollection), false)]
  [View("Variable Impacts")]
  public sealed partial class RunCollectionVariableImpactView : AsynchronousContentView {
    private const string variableImpactResultName = "Variable impacts";
    private const string crossValidationFoldsResultName = "CrossValidation Folds";
    private const string numberOfFoldsParameterName = "Folds";
    public RunCollectionVariableImpactView() {
      InitializeComponent();
    }

    public new RunCollection Content {
      get { return (RunCollection)base.Content; }
      set { base.Content = value; }
    }

    #region events
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.UpdateOfRunsInProgressChanged += new EventHandler(Content_UpdateOfRunsInProgressChanged);
      Content.ItemsAdded += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IRun>(Content_ItemsAdded);
      Content.ItemsRemoved += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IRun>(Content_ItemsRemoved);
      Content.CollectionReset += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IRun>(Content_CollectionReset);
      RegisterRunEvents(Content);
    }
    protected override void DeregisterContentEvents() {
      base.RegisterContentEvents();
      Content.UpdateOfRunsInProgressChanged -= new EventHandler(Content_UpdateOfRunsInProgressChanged);
      Content.ItemsAdded -= new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IRun>(Content_ItemsAdded);
      Content.ItemsRemoved -= new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IRun>(Content_ItemsRemoved);
      Content.CollectionReset -= new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IRun>(Content_CollectionReset);
      DeregisterRunEvents(Content);
    }
    private void RegisterRunEvents(IEnumerable<IRun> runs) {
      foreach (IRun run in runs)
        run.Changed += new EventHandler(Run_Changed);
    }
    private void DeregisterRunEvents(IEnumerable<IRun> runs) {
      foreach (IRun run in runs)
        run.Changed -= new EventHandler(Run_Changed);
    }
    private void Content_ItemsAdded(object sender, HeuristicLab.Collections.CollectionItemsChangedEventArgs<IRun> e) {
      RegisterRunEvents(e.Items);
      UpdateData();
    }
    private void Content_ItemsRemoved(object sender, HeuristicLab.Collections.CollectionItemsChangedEventArgs<IRun> e) {
      DeregisterRunEvents(e.Items);
      UpdateData();
    }
    private void Content_CollectionReset(object sender, HeuristicLab.Collections.CollectionItemsChangedEventArgs<IRun> e) {
      DeregisterRunEvents(e.OldItems);
      RegisterRunEvents(e.Items);
      UpdateData();
    }
    private void Content_UpdateOfRunsInProgressChanged(object sender, EventArgs e) {
      if (!Content.UpdateOfRunsInProgress) UpdateData();
    }
    private void Run_Changed(object sender, EventArgs e) {
      if (!Content.UpdateOfRunsInProgress) UpdateData();
    }
    #endregion

    protected override void OnContentChanged() {
      base.OnContentChanged();
      this.UpdateData();
    }

    private void comboBox_SelectedValueChanged(object sender, EventArgs e) {
      if (comboBox.SelectedItem != null) {
        var cvRuns = from r in Content
                     where r.Visible
                     where r.Parameters.ContainsKey(numberOfFoldsParameterName)
                     select r;
        if (comboBox.SelectedIndex == 0) {
          var selectedFolds = cvRuns
            .SelectMany(r => (RunCollection)r.Results[crossValidationFoldsResultName])
            .Where(r => r.Results.ContainsKey(variableImpactResultName));
          matrixView.Content = CalculateVariableImpactMatrix(selectedFolds.ToArray());
        } else {
          var selectedFolds = from r in cvRuns
                              let foldCollection = (RunCollection)r.Results[crossValidationFoldsResultName]
                              let run = foldCollection.ElementAt(comboBox.SelectedIndex - 1)
                              where run.Results.ContainsKey(variableImpactResultName)
                              select new { run, r.Name };
          matrixView.Content = CalculateVariableImpactMatrix(selectedFolds.Select(x => x.run).ToArray(), selectedFolds.Select(x => x.Name).ToArray());
        }
      }
    }


    private void UpdateData() {
      if (Content != null) {
        comboBox.Items.Clear();
        comboBox.Enabled = false;
        var visibleRuns = Content.Where(r => r.Visible).ToArray();
        var representativeCvRun =
          visibleRuns.Where(r => r.Parameters.ContainsKey(numberOfFoldsParameterName)).FirstOrDefault();
        if (representativeCvRun != null) {
          // make sure all runs have the same number of folds
          int nFolds = ((IntValue)representativeCvRun.Parameters[numberOfFoldsParameterName]).Value;
          var cvRuns = visibleRuns.Where(r => r.Parameters.ContainsKey(numberOfFoldsParameterName));
          if (cvRuns.All(r => ((IntValue)r.Parameters[numberOfFoldsParameterName]).Value == nFolds)) {
            // populate combobox
            comboBox.Items.Add("Overall");
            for (int foldIndex = 0; foldIndex < nFolds; foldIndex++) {
              comboBox.Items.Add("Fold " + foldIndex);
            }
            comboBox.SelectedIndex = 0;
            comboBox.Enabled = true;
          } else {
            matrixView.Content = null;
          }
        } else {
          var runsWithVariables = visibleRuns.Where(r => r.Results.ContainsKey(variableImpactResultName)).ToArray();
          matrixView.Content = CalculateVariableImpactMatrix(runsWithVariables);
        }
      }
    }

    private IStringConvertibleMatrix CalculateVariableImpactMatrix(IRun[] runs) {
      return CalculateVariableImpactMatrix(runs, runs.Select(r => r.Name).ToArray());
    }

    private DoubleMatrix CalculateVariableImpactMatrix(IRun[] runs, string[] runNames) {
      DoubleMatrix matrix = null;
      IEnumerable<DoubleMatrix> allVariableImpacts = (from run in runs
                                                      select run.Results[variableImpactResultName]).Cast<DoubleMatrix>();
      IEnumerable<string> variableNames = (from variableImpact in allVariableImpacts
                                           from variableName in variableImpact.RowNames
                                           select variableName)
                                          .Distinct();
      // filter variableNames: only include names that have at least one non-zero value in a run
      List<string> variableNamesList = (from variableName in variableNames
                                        where GetVariableImpacts(variableName, allVariableImpacts).Any(x => !x.IsAlmost(0.0))
                                        select variableName)
                                       .ToList();

      List<string> statictics = new List<string> { "Median Rank", "Mean", "StdDev", "pValue" };
      List<string> columnNames = new List<string>(runNames);
      columnNames.AddRange(statictics);
      int numberOfRuns = runs.Length;

      matrix = new DoubleMatrix(variableNamesList.Count, numberOfRuns + statictics.Count);
      matrix.SortableView = true;
      matrix.RowNames = variableNamesList;
      matrix.ColumnNames = columnNames;

      // calculate statistics
      List<List<double>> variableImpactsOverRuns = (from variableName in variableNamesList
                                                    select GetVariableImpacts(variableName, allVariableImpacts).ToList())
                                             .ToList();
      List<List<double>> variableRanks = (from variableName in variableNamesList
                                          select GetVariableImpactRanks(variableName, allVariableImpacts).ToList())
                                      .ToList();
      if (variableImpactsOverRuns.Count() > 0) {
        // the variable with the worst median impact value is chosen as the reference variable
        // this is problematic if all variables are relevant, however works often in practice
        List<double> referenceImpacts = (from impacts in variableImpactsOverRuns
                                         let avg = impacts.Median()
                                         orderby avg
                                         select impacts)
                                         .First();
        // for all variables
        for (int row = 0; row < variableImpactsOverRuns.Count; row++) {
          // median rank
          matrix[row, numberOfRuns] = variableRanks[row].Median();
          // also show mean and std.dev. of relative variable impacts to indicate the relative difference in impacts of variables
          matrix[row, numberOfRuns + 1] = Math.Round(variableImpactsOverRuns[row].Average(), 3);
          matrix[row, numberOfRuns + 2] = Math.Round(variableImpactsOverRuns[row].StandardDeviation(), 3);

          double leftTail = 0; double rightTail = 0; double bothTails = 0;
          // calc differences of impacts for current variable and reference variable
          double[] z = new double[referenceImpacts.Count];
          for (int i = 0; i < z.Length; i++) {
            z[i] = variableImpactsOverRuns[row][i] - referenceImpacts[i];
          }
          // wilcoxon signed rank test is used because the impact values of two variables in a single run are not independent
          alglib.wsr.wilcoxonsignedranktest(z, z.Length, 0, ref bothTails, ref leftTail, ref rightTail);
          matrix[row, numberOfRuns + 3] = Math.Round(bothTails, 4);
        }
      }

      // fill matrix with impacts from runs
      for (int i = 0; i < runs.Length; i++) {
        IRun run = runs[i];
        DoubleMatrix runVariableImpacts = (DoubleMatrix)run.Results[variableImpactResultName];
        for (int j = 0; j < runVariableImpacts.Rows; j++) {
          int rowIndex = variableNamesList.FindIndex(s => s == runVariableImpacts.RowNames.ElementAt(j));
          if (rowIndex > -1) {
            matrix[rowIndex, i] = Math.Round(runVariableImpacts[j, 0], 3);
          }
        }
      }
      // sort by median
      var sortedMatrix = (DoubleMatrix)matrix.Clone();
      var sortedIndexes = from i in Enumerable.Range(0, sortedMatrix.Rows)
                          orderby matrix[i, numberOfRuns]
                          select i;

      int targetIndex = 0;
      foreach (var sourceIndex in sortedIndexes) {
        for (int c = 0; c < matrix.Columns; c++)
          sortedMatrix[targetIndex, c] = matrix[sourceIndex, c];
        targetIndex++;
      }
      return sortedMatrix;
    }

    private IEnumerable<double> GetVariableImpactRanks(string variableName, IEnumerable<DoubleMatrix> allVariableImpacts) {
      foreach (DoubleMatrix runVariableImpacts in allVariableImpacts) {
        // certainly not yet very efficient because ranks are computed multiple times for the same run
        string[] variableNames = runVariableImpacts.RowNames.ToArray();
        double[] values = (from row in Enumerable.Range(0, runVariableImpacts.Rows)
                           select runVariableImpacts[row, 0] * -1)
                          .ToArray();
        Array.Sort(values, variableNames);
        // calculate ranks
        double[] ranks = new double[values.Length];
        // check for tied ranks
        int i = 0;
        while (i < values.Length) {
          ranks[i] = i + 1;
          int j = i + 1;
          while (j < values.Length && values[i].IsAlmost(values[j])) {
            ranks[j] = ranks[i];
            j++;
          }
          i = j;
        }
        int rankIndex = 0;
        foreach (string rowVariableName in variableNames) {
          if (rowVariableName == variableName)
            yield return ranks[rankIndex];
          rankIndex++;
        }
      }
    }

    private IEnumerable<double> GetVariableImpacts(string variableName, IEnumerable<DoubleMatrix> allVariableImpacts) {
      foreach (DoubleMatrix runVariableImpacts in allVariableImpacts) {
        int row = 0;
        foreach (string rowName in runVariableImpacts.RowNames) {
          if (rowName == variableName)
            yield return runVariableImpacts[row, 0];
          row++;
        }
      }
    }

  }
}
