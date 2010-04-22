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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using System.Collections.Generic;
using System.Linq;

namespace HeuristicLab.Problems.DataAnalysis.Regression.Symbolic {
  /// <summary>
  /// Represents a solution for a symbolic regression problem which can be visualized in the GUI.
  /// </summary>
  [Item("SymbolicRegressionSolution", "Represents a solution for a symbolic regression problem which can be visualized in the GUI.")]
  [StorableClass]
  public sealed class SymbolicRegressionSolution : DataAnalysisSolution {
    [Storable]
    private SymbolicRegressionModel model;
    public SymbolicRegressionModel Model {
      get { return model; }
      set {
        if (model != value) {
          if (value == null) throw new ArgumentNullException();
          model = value;
          OnModelChanged(EventArgs.Empty);
        }
      }
    }

    public SymbolicRegressionSolution() : base() { }
    public SymbolicRegressionSolution(DataAnalysisProblemData problemData, SymbolicRegressionModel model)
      : base(problemData) {
      this.model = model;
    }

    public event EventHandler ModelChanged;
    private void OnModelChanged(EventArgs e) {
      RecalculateEstimatedValues();
      var listeners = ModelChanged;
      if (listeners != null)
        listeners(this, e);
    }

    protected override void OnProblemDataChanged(EventArgs e) {
      RecalculateEstimatedValues();
    }

    private void RecalculateEstimatedValues() {
      estimatedValues = model.GetEstimatedValues(ProblemData.Dataset, 0, ProblemData.Dataset.Rows).ToList();
      OnEstimatedValuesChanged(EventArgs.Empty);
    }

    private List<double> estimatedValues;
    public override IEnumerable<double> EstimatedValues {
      get {
        if (estimatedValues == null) RecalculateEstimatedValues();
        return estimatedValues.AsEnumerable();
      }
    }

    public override IEnumerable<double> EstimatedTrainingValues {
      get {
        if (estimatedValues == null) RecalculateEstimatedValues();
        int start = ProblemData.TrainingSamplesStart.Value;
        int n = ProblemData.TrainingSamplesEnd.Value - start;
        return estimatedValues.Skip(start).Take(n).ToList();
      }
    }

    public override IEnumerable<double> EstimatedTestValues {
      get {
        if (estimatedValues == null) RecalculateEstimatedValues();
        int start = ProblemData.TestSamplesStart.Value;
        int n = ProblemData.TestSamplesEnd.Value - start;
        return estimatedValues.Skip(start).Take(n).ToList();
      }
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      SymbolicRegressionSolution clone = (SymbolicRegressionSolution)base.Clone(cloner);
      clone.model = (SymbolicRegressionModel)cloner.Clone(model);
      return clone;
    }
  }
}
