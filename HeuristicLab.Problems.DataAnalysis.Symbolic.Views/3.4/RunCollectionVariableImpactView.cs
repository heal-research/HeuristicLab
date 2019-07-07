#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
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
      Content.UpdateOfRunsInProgressChanged += Content_UpdateOfRunsInProgressChanged;
      Content.ItemsAdded += Content_ItemsAdded;
      Content.ItemsRemoved += Content_ItemsRemoved;
      Content.CollectionReset += Content_CollectionReset;
      RegisterRunEvents(Content);
    }
    protected override void DeregisterContentEvents() {
      base.RegisterContentEvents();
      Content.UpdateOfRunsInProgressChanged -= Content_UpdateOfRunsInProgressChanged;
      Content.ItemsAdded -= Content_ItemsAdded;
      Content.ItemsRemoved -= Content_ItemsRemoved;
      Content.CollectionReset -= Content_CollectionReset;
      DeregisterRunEvents(Content);
    }
    private void RegisterRunEvents(IEnumerable<IRun> runs) {
      foreach (IRun run in runs)
        run.PropertyChanged += Run_PropertyChanged;
    }
    private void DeregisterRunEvents(IEnumerable<IRun> runs) {
      foreach (IRun run in runs)
        run.PropertyChanged -= Run_PropertyChanged;
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
    private void Run_PropertyChanged(object sender, PropertyChangedEventArgs e) {
      if (!Content.UpdateOfRunsInProgress && e.PropertyName == "Visible")
        UpdateData();
    }
    #endregion

    protected override void OnContentChanged() {
      base.OnContentChanged();
      this.UpdateData();
    }

    private void comboBox_SelectedValueChanged(object sender, EventArgs e) {
      if (comboBox.SelectedItem != null) {
        var visibleRuns = from run in Content where run.Visible select run;
        if (comboBox.SelectedIndex == 0) {
          var selectedFolds = from r in visibleRuns
                              let foldCollection = (RunCollection)r.Results[crossValidationFoldsResultName]
                              from run in foldCollection
                              let name = (r.Name + " " + run.Name)
                              select new { run, name };
          matrixView.Content = CalculateVariableImpactMatrix(selectedFolds.Select(x => x.run).ToArray(), selectedFolds.Select(x => x.name).ToArray());
        } else {
          var selectedFolds = from r in visibleRuns
                              let foldCollection = (RunCollection)r.Results[crossValidationFoldsResultName]
                              let run = foldCollection.ElementAt(comboBox.SelectedIndex - 1)
                              let name = (r.Name + " " + run.Name)
                              select new { run, name };
          matrixView.Content = CalculateVariableImpactMatrix(selectedFolds.Select(x => x.run).ToArray(), selectedFolds.Select(x => x.name).ToArray());
        }
      }
    }


    private void UpdateData() {
      if (InvokeRequired) {
        Invoke((Action)UpdateData);
      } else {
        if (Content != null) {
          comboBox.Items.Clear();
          comboBox.Enabled = false;
          comboBox.Visible = false;
          foldsLabel.Visible = false;
          variableImpactsGroupBox.Dock = DockStyle.Fill;
          var visibleRuns = Content.Where(r => r.Visible).ToArray();
          if (visibleRuns.Length == 0) {
            DisplayMessage("Run collection is empty.");
          } else if (visibleRuns.All(r => r.Parameters.ContainsKey(numberOfFoldsParameterName))) {
            // check if all runs are comparable (CV or normal runs)
            CheckAndUpdateCvRuns();
          } else if (visibleRuns.All(r => !r.Parameters.ContainsKey(numberOfFoldsParameterName))) {
            CheckAndUpdateNormalRuns();
          } else {
            // there is a mix of CV and normal runs => show an error message
            DisplayMessage("The run collection contains a mixture of normal runs and cross-validation runs. Variable impact calculation does not work in this case.");
          }
        }
      }
    }

    private void CheckAndUpdateCvRuns() {
      var visibleRuns = from run in Content where run.Visible select run;
      var representativeRun = visibleRuns.First();
      // make sure all runs have the same number of folds
      int nFolds = ((IntValue)representativeRun.Parameters[numberOfFoldsParameterName]).Value;
      if (visibleRuns.All(r => ((IntValue)r.Parameters[numberOfFoldsParameterName]).Value == nFolds)) {
        var allFoldResults = visibleRuns.SelectMany(run => (RunCollection)run.Results[crossValidationFoldsResultName]);

        // make sure each fold contains variable impacts 
        if (!allFoldResults.All(r => r.Results.ContainsKey(variableImpactResultName))) {
          DisplayMessage("At least one of the runs does not contain a variable impact result.");
        } else {
          // make sure each of the runs has the same input variables
          var allVariableNames = from run in allFoldResults
                                 let varImpacts = (DoubleMatrix)run.Results[variableImpactResultName]
                                 select varImpacts.RowNames;
          var groupedVariableNames = allVariableNames
            .SelectMany(x => x)
            .GroupBy(x => x);

          if (groupedVariableNames.Any(g => g.Count() != allFoldResults.Count())) {
            DisplayMessage("At least one of the runs has a different input variable set than the rest.");
          } else {
            // populate combobox
            comboBox.Items.Add("Overall");
            for (int foldIndex = 0; foldIndex < nFolds; foldIndex++) {
              comboBox.Items.Add("Fold " + foldIndex);
            }
            comboBox.SelectedIndex = 0;
            comboBox.Enabled = true;
            comboBox.Visible = true;
            foldsLabel.Visible = true;
            variableImpactsGroupBox.Controls.Clear();
            variableImpactsGroupBox.Dock = DockStyle.None;
            variableImpactsGroupBox.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right |
                                             AnchorStyles.Bottom;
            variableImpactsGroupBox.Height = this.Height - comboBox.Height - 12;
            variableImpactsGroupBox.Width = this.Width;
            matrixView.Dock = DockStyle.Fill;
            variableImpactsGroupBox.Controls.Add(matrixView);
          }
        }
      } else {
        DisplayMessage("At least on of the cross-validation runs has a different number of folds than the rest.");
      }
    }

    private void CheckAndUpdateNormalRuns() {
      // make sure all runs contain variable impact results
      var visibleRuns = from run in Content where run.Visible select run;

      if (!visibleRuns.All(r => r.Results.ContainsKey(variableImpactResultName))) {
        DisplayMessage("At least one of the runs does not contain a variable impact result.");
      } else {
        // make sure each of the runs has the same input variables
        var allVariableNames = from run in visibleRuns
                               let varImpacts = (DoubleMatrix)run.Results[variableImpactResultName]
                               select varImpacts.RowNames;
        var groupedVariableNames = allVariableNames
          .SelectMany(x => x)
          .GroupBy(x => x);

        if (groupedVariableNames.Any(g => g.Count() != visibleRuns.Count())) {
          DisplayMessage("At least one of the runs has a different input variable set than the rest.");
        } else {
          if (!variableImpactsGroupBox.Controls.Contains(matrixView)) {
            variableImpactsGroupBox.Controls.Clear();
            matrixView.Dock = DockStyle.Fill;
            variableImpactsGroupBox.Controls.Add(matrixView);
          }
          matrixView.Content = CalculateVariableImpactMatrix(visibleRuns.ToArray(), visibleRuns.Select(r => r.Name).ToArray());
        }
      }
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
      sortedMatrix.RowNames = sortedIndexes.Select(i => variableNamesList[i]);

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

    private void DisplayMessage(string message) {
      variableImpactsGroupBox.Controls.Remove(matrixView);
      var label = new Label { TextAlign = ContentAlignment.MiddleCenter, Text = message, Dock = DockStyle.Fill };
      variableImpactsGroupBox.Controls.Add(label);
    }
  }
}
