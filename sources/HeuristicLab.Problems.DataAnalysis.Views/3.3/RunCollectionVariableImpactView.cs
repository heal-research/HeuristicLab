#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using alglib;
using HeuristicLab.Common;
using HeuristicLab.Data;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Optimization;
using System;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [Content(typeof(RunCollection), false)]
  [View("RunCollection Variable Impact View")]
  public partial class RunCollectionVariableImpactView : AsynchronousContentView {
    private const string variableImpactResultName = "Integrated variable frequencies";
    public RunCollectionVariableImpactView() {
      InitializeComponent();
    }

    public new RunCollection Content {
      get { return (RunCollection)base.Content; }
      set { base.Content = value; }
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      this.Content.ItemsAdded += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IRun>(Content_ItemsAdded);
      this.Content.ItemsRemoved += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IRun>(Content_ItemsRemoved);
      this.Content.CollectionReset += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IRun>(Content_CollectionReset);
    }
    protected override void DeregisterContentEvents() {
      base.RegisterContentEvents();
      this.Content.ItemsAdded -= new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IRun>(Content_ItemsAdded);
      this.Content.ItemsRemoved -= new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IRun>(Content_ItemsRemoved);
      this.Content.CollectionReset -= new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IRun>(Content_CollectionReset);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      this.UpdateData();
    }
    private void Content_ItemsAdded(object sender, HeuristicLab.Collections.CollectionItemsChangedEventArgs<IRun> e) {
      this.UpdateData();
    }
    private void Content_ItemsRemoved(object sender, HeuristicLab.Collections.CollectionItemsChangedEventArgs<IRun> e) {
      this.UpdateData();
    }
    private void Content_CollectionReset(object sender, HeuristicLab.Collections.CollectionItemsChangedEventArgs<IRun> e) {
      this.UpdateData();
    }

    private void UpdateData() {
      matrixView.Content = CalculateVariableImpactMatrix();
    }

    private DoubleMatrix CalculateVariableImpactMatrix() {
      DoubleMatrix matrix = null;
      if (Content != null) {
        List<IRun> runsWithVariables = Content.Where(r => r.Results.ContainsKey(variableImpactResultName)).ToList();
        IEnumerable<DoubleMatrix> allVariableImpacts = (from run in runsWithVariables
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
        List<string> columnNames = runsWithVariables.Select(r => r.Name).ToList();
        columnNames.AddRange(statictics);
        int runs = runsWithVariables.Count();

        matrix = new DoubleMatrix(variableNamesList.Count, runs + statictics.Count);
        matrix.SortableView = true;
        matrix.RowNames = variableNamesList;
        matrix.ColumnNames = columnNames;

        for (int i = 0; i < runsWithVariables.Count; i++) {
          IRun run = runsWithVariables[i];
          DoubleMatrix runVariableImpacts = (DoubleMatrix)run.Results[variableImpactResultName];
          for (int j = 0; j < runVariableImpacts.Rows; j++) {
            int rowIndex = variableNamesList.FindIndex(s => s == runVariableImpacts.RowNames.ElementAt(j));
            if (rowIndex > -1) {
              matrix[rowIndex, i] = runVariableImpacts[j, 0];
            }
          }
        }

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
            matrix[row, runs] = variableRanks[row].Median();
            // also show mean and std.dev. of relative variable impacts to indicate the relative difference in impacts of variables
            matrix[row, runs + 1] = variableImpactsOverRuns[row].Average();
            matrix[row, runs + 2] = variableImpactsOverRuns[row].StandardDeviation();

            double leftTail = 0; double rightTail = 0; double bothTails = 0;
            // calc differences of impacts for current variable and reference variable
            double[] z = new double[referenceImpacts.Count];
            for (int i = 0; i < z.Length; i++) {
              z[i] = variableImpactsOverRuns[row][i] - referenceImpacts[i];
            }
            // wilcoxon signed rank test is used because the impact values of two variables in a single run are not independent
            alglib.wsr.wilcoxonsignedranktest(z, z.Length, 0, ref bothTails, ref leftTail, ref rightTail);
            matrix[row, runs + 3] = bothTails;
          }
        }
      }
      return matrix;
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
