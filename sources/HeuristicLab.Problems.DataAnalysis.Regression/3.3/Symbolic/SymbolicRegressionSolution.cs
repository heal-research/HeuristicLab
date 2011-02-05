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
using System.Drawing;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Symbols;

namespace HeuristicLab.Problems.DataAnalysis.Regression.Symbolic {
  /// <summary>
  /// Represents a solution for a symbolic regression problem which can be visualized in the GUI.
  /// </summary>
  [Item("SymbolicRegressionSolution", "Represents a solution for a symbolic regression problem which can be visualized in the GUI.")]
  [StorableClass]
  public class SymbolicRegressionSolution : DataAnalysisSolution {
    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Function; }
    }

    public new SymbolicRegressionModel Model {
      get { return (SymbolicRegressionModel)base.Model; }
      set { base.Model = value; }
    }

    protected List<double> estimatedValues;
    public override IEnumerable<double> EstimatedValues {
      get {
        if (estimatedValues == null) RecalculateEstimatedValues();
        return estimatedValues;
      }
    }

    public override IEnumerable<double> EstimatedTrainingValues {
      get { return GetEstimatedValues(ProblemData.TrainingIndizes); }
    }

    public override IEnumerable<double> EstimatedTestValues {
      get { return GetEstimatedValues(ProblemData.TestIndizes); }
    }

    [StorableConstructor]
    protected SymbolicRegressionSolution(bool deserializing) : base(deserializing) { }
    protected SymbolicRegressionSolution(SymbolicRegressionSolution original, Cloner cloner)
      : base(original, cloner) {
    }
    public SymbolicRegressionSolution(DataAnalysisProblemData problemData, SymbolicRegressionModel model, double lowerEstimationLimit, double upperEstimationLimit)
      : base(problemData, lowerEstimationLimit, upperEstimationLimit) {
      this.Model = model;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicRegressionSolution(this, cloner);
    }

    protected override void RecalculateEstimatedValues() {
      int minLag = GetMinimumLagFromTree(Model.SymbolicExpressionTree.Root);
      IEnumerable<double> calculatedValues =
          from x in Model.GetEstimatedValues(ProblemData, 0 - minLag, ProblemData.Dataset.Rows)
          let boundedX = Math.Min(UpperEstimationLimit, Math.Max(LowerEstimationLimit, x))
          select double.IsNaN(boundedX) ? UpperEstimationLimit : boundedX;
      estimatedValues = Enumerable.Repeat(UpperEstimationLimit, Math.Abs(minLag)).Concat(calculatedValues).ToList();
      OnEstimatedValuesChanged();
    }

    public virtual IEnumerable<double> GetEstimatedValues(IEnumerable<int> rows) {
      if (estimatedValues == null) RecalculateEstimatedValues();
      foreach (int row in rows)
        yield return estimatedValues[row];
    }

    protected int GetMinimumLagFromTree(SymbolicExpressionTreeNode node) {
      if (node == null) return 0;
      int lag = 0;

      var laggedTreeNode = node as ILaggedTreeNode;
      if (laggedTreeNode != null) lag += laggedTreeNode.Lag;
      else if (node.Symbol is Derivative) lag -= 4;

      int subtreeLag = 0;
      foreach (var subtree in node.SubTrees) {
        subtreeLag = Math.Min(subtreeLag, GetMinimumLagFromTree(subtree));
      }
      return lag + subtreeLag;
    }
  }
}
