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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Classification {
  /// <summary>
  /// Represents a nearest neighbour model for regression and classification
  /// </summary>
  [StorableClass]
  [Item("SymbolicNearestNeighbourClassificationModel", "Represents a nearest neighbour model for symbolic classification.")]
  public sealed class SymbolicNearestNeighbourClassificationModel : SymbolicClassificationModel {

    [Storable]
    private int k;
    [Storable]
    private List<KeyValuePair<double, double>> trainedTargetPair;

    [StorableConstructor]
    private SymbolicNearestNeighbourClassificationModel(bool deserializing) : base(deserializing) { }
    private SymbolicNearestNeighbourClassificationModel(SymbolicNearestNeighbourClassificationModel original, Cloner cloner)
      : base(original, cloner) {
      k = original.k;
      trainedTargetPair = new List<KeyValuePair<double, double>>(original.trainedTargetPair);
    }
    public SymbolicNearestNeighbourClassificationModel(int k, ISymbolicExpressionTree tree, ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, double lowerEstimationLimit = double.MinValue, double upperEstimationLimit = double.MaxValue)
      : base(tree, interpreter, lowerEstimationLimit, upperEstimationLimit) {
      this.k = k;
      this.trainedTargetPair = new List<KeyValuePair<double, double>>();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicNearestNeighbourClassificationModel(this, cloner);
    }

    public override IEnumerable<double> GetEstimatedClassValues(Dataset dataset, IEnumerable<int> rows) {
      var estimatedValues = Interpreter.GetSymbolicExpressionTreeValues(SymbolicExpressionTree, dataset, rows);
      var neighbors = new Dictionary<double, int>();
      foreach (var ev in estimatedValues) {
        int lower = 0, upper = 1;
        double sdist = Math.Abs(ev - trainedTargetPair[0].Key);
        for (int i = 1; i < trainedTargetPair.Count; i++) {
          double d = Math.Abs(ev - trainedTargetPair[i].Key);
          if (d > sdist) break;
          lower = i;
          upper = i + 1;
          sdist = d;
        }
        neighbors.Clear();
        neighbors[trainedTargetPair[lower].Value] = 1;
        lower--;
        for (int i = 1; i < Math.Min(k, trainedTargetPair.Count); i++) {
          if (upper >= trainedTargetPair.Count || (lower > 0 && ev - trainedTargetPair[lower].Key < trainedTargetPair[upper].Key - ev)) {
            if (!neighbors.ContainsKey(trainedTargetPair[lower].Value))
              neighbors[trainedTargetPair[lower].Value] = 1;
            else neighbors[trainedTargetPair[lower].Value]++;
            lower--;
          } else {
            if (!neighbors.ContainsKey(trainedTargetPair[upper].Value))
              neighbors[trainedTargetPair[upper].Value] = 1;
            else neighbors[trainedTargetPair[upper].Value]++;
            upper++;
          }
        }
        yield return neighbors.MaxItems(x => x.Value).First().Key;
      }
    }

    public override void RecalculateModelParameters(IClassificationProblemData problemData, IEnumerable<int> rows) {
      var estimatedValues = Interpreter.GetSymbolicExpressionTreeValues(SymbolicExpressionTree, problemData.Dataset, rows);
      var targetValues = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, rows);
      var pair = estimatedValues.Zip(targetValues, (e, t) => new { Estimated = e, Target = t });

      // there could be more than one target value per estimated value
      var dict = new Dictionary<double, Dictionary<double, int>>();
      foreach (var p in pair) {
        if (!dict.ContainsKey(p.Estimated)) dict[p.Estimated] = new Dictionary<double, int>();
        if (!dict[p.Estimated].ContainsKey(p.Target)) dict[p.Estimated][p.Target] = 0;
        dict[p.Estimated][p.Target]++;
      }

      trainedTargetPair = new List<KeyValuePair<double, double>>();
      foreach (var ev in dict) {
        var target = ev.Value.MaxItems(x => x.Value).First().Key;
        trainedTargetPair.Add(new KeyValuePair<double, double>(ev.Key, target));
      }
      trainedTargetPair = trainedTargetPair.OrderBy(x => x.Key).ToList();
    }

    public override ISymbolicClassificationSolution CreateClassificationSolution(IClassificationProblemData problemData) {
      return new SymbolicClassificationSolution((ISymbolicClassificationModel)this.Clone(), problemData);
    }
  }
}
