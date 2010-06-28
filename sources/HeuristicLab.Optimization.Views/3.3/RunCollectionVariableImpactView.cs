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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Core;
using HeuristicLab.Common;
using HeuristicLab.Data;

namespace HeuristicLab.Optimization.Views {
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
        IEnumerable<IRun> runsWithVariables = Content.Where(r => r.Results.ContainsKey(variableImpactResultName));
        IEnumerable<DoubleMatrix> variableImpacts = (from run in runsWithVariables
                                                     select run.Results[variableImpactResultName]).Cast<DoubleMatrix>();
        List<string> variableNames = (from varImpact in variableImpacts
                                      from variableName in varImpact.RowNames
                                      select variableName).Distinct().ToList();
        List<string> columnNames = runsWithVariables.Select(r => r.Name).ToList();
        columnNames.AddRange( new List<string> { "Mean", "Median", "StdDev" });
        int runs = runsWithVariables.Count();

        matrix = new DoubleMatrix(variableNames.Count, runs + 3);
        matrix.SortableView = true;
        matrix.RowNames = variableNames;
        matrix.ColumnNames = columnNames;

        foreach (IRun run in runsWithVariables) {
          DoubleMatrix runVariableImpacts = (DoubleMatrix)run.Results[variableImpactResultName];
          for (int i = 0; i < runVariableImpacts.Rows; i++) {
            int rowIndex = variableNames.FindIndex(s => s == runVariableImpacts.RowNames.ElementAt(i));
            int columnIndex = columnNames.FindIndex(s => s == run.Name);
            matrix[rowIndex, columnIndex] = runVariableImpacts[i, 0];
          }
        }

        for (int variableIndex = 0; variableIndex < matrix.Rows; variableIndex++) {
          List<double> impacts = new List<double>();
          for (int runIndex = 0; runIndex < runs; runIndex++)
            impacts.Add(matrix[variableIndex, runIndex]);
          matrix[variableIndex, runs] = impacts.Average();
          matrix[variableIndex, runs + 1] = impacts.Median();
          matrix[variableIndex, runs + 2] = impacts.StandardDeviation();
        }
      }
      return matrix;
    }
  }
}
