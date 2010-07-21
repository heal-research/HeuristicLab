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
        IEnumerable<DoubleMatrix> variableImpacts = (from run in runsWithVariables
                                                     select run.Results[variableImpactResultName]).Cast<DoubleMatrix>();
        List<string> variableNames = (from varImpact in variableImpacts
                                      from variableName in varImpact.RowNames
                                      select variableName).Distinct().ToList();
        List<string> statictics = new List<string> { "Mean", "Median", "StdDev", "pValue Mean<0", "pValue Median<0" };
        List<string> columnNames = runsWithVariables.Select(r => r.Name).ToList();
        columnNames.AddRange(statictics);
        int runs = runsWithVariables.Count();

        matrix = new DoubleMatrix(variableNames.Count, runs + statictics.Count);
        matrix.SortableView = true;
        matrix.RowNames = variableNames;
        matrix.ColumnNames = columnNames;

        for (int i = 0; i < runsWithVariables.Count; i++) {
          IRun run = runsWithVariables[i];
          DoubleMatrix runVariableImpacts = (DoubleMatrix)run.Results[variableImpactResultName];
          for (int j = 0; j < runVariableImpacts.Rows; j++) {
            int rowIndex = variableNames.FindIndex(s => s == runVariableImpacts.RowNames.ElementAt(j));
            matrix[rowIndex, i] = runVariableImpacts[j, 0];
          }
        }

        for (int variableIndex = 0; variableIndex < matrix.Rows; variableIndex++) {
          List<double> impacts = new List<double>();
          for (int runIndex = 0; runIndex < runs; runIndex++)
            impacts.Add(matrix[variableIndex, runIndex]);
          matrix[variableIndex, runs] = impacts.Average();
          matrix[variableIndex, runs + 1] = impacts.Median();
          matrix[variableIndex, runs + 2] = impacts.StandardDeviation();
          double leftTail = 0; double rightTail = 0; double bothTails = 0;
          double[] impactsArray = impacts.ToArray();
          studentttests.studentttest1(ref impactsArray, impactsArray.Length, 0, ref bothTails, ref leftTail, ref rightTail);
          matrix[variableIndex, runs + 3] = rightTail;
          wsr.wilcoxonsignedranktest(impactsArray, impactsArray.Length, 0, ref bothTails, ref leftTail, ref rightTail);
          matrix[variableIndex, runs + 4] = rightTail;
        }
      }
      return matrix;
    }
  }
}
