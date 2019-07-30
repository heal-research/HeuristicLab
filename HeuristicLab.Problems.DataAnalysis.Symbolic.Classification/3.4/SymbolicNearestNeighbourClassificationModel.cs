#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Classification {
  /// <summary>
  /// Represents a nearest neighbour model for regression and classification
  /// </summary>
  [StorableType("B9F8A753-B102-4356-8821-76E31634A0C6")]
  [Item("SymbolicNearestNeighbourClassificationModel", "Represents a nearest neighbour model for symbolic classification.")]
  public sealed class SymbolicNearestNeighbourClassificationModel : SymbolicClassificationModel {

    [Storable]
    private int k;
    [Storable]
    private List<double> trainedClasses;
    [Storable]
    private List<double> trainedEstimatedValues;

    [Storable]
    private ClassFrequencyComparer frequencyComparer;

    [StorableConstructor]
    private SymbolicNearestNeighbourClassificationModel(StorableConstructorFlag _) : base(_) { }
    private SymbolicNearestNeighbourClassificationModel(SymbolicNearestNeighbourClassificationModel original, Cloner cloner)
      : base(original, cloner) {
      k = original.k;
      frequencyComparer = new ClassFrequencyComparer(original.frequencyComparer);
      trainedEstimatedValues = new List<double>(original.trainedEstimatedValues);
      trainedClasses = new List<double>(original.trainedClasses);
    }
    public SymbolicNearestNeighbourClassificationModel(string targetVariable, int k, ISymbolicExpressionTree tree, ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, double lowerEstimationLimit = double.MinValue, double upperEstimationLimit = double.MaxValue)
      : base(targetVariable, tree, interpreter, lowerEstimationLimit, upperEstimationLimit) {
      this.k = k;
      frequencyComparer = new ClassFrequencyComparer();

    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicNearestNeighbourClassificationModel(this, cloner);
    }

    public override IEnumerable<double> GetEstimatedClassValues(IDataset dataset, IEnumerable<int> rows) {
      var estimatedValues = Interpreter.GetSymbolicExpressionTreeValues(SymbolicExpressionTree, dataset, rows)
                                       .LimitToRange(LowerEstimationLimit, UpperEstimationLimit);
      foreach (var ev in estimatedValues) {
        // find the range [lower, upper[ of trainedTargetValues that contains the k closest neighbours
        // the range can span more than k elements when there are equal estimated values

        // find the index of the training-point to which distance is shortest
        int lower = trainedEstimatedValues.BinarySearch(ev);
        int upper;
        // if the element was not found exactly, BinarySearch returns the complement of the index of the next larger item
        if (lower < 0) {
          lower = ~lower;
          // lower is not necessarily the closer one
          // determine which element is closer to ev (lower - 1) or (lower)
          if (lower == trainedEstimatedValues.Count ||
            (lower > 0 && Math.Abs(ev - trainedEstimatedValues[lower - 1]) < Math.Abs(ev - trainedEstimatedValues[lower]))) {
            lower = lower - 1;
          }
        }
        upper = lower + 1;
        // at this point we have a range [lower, upper[ that includes only the closest element to ev

        // expand the range to left or right looking for the nearest neighbors
        while (upper - lower < Math.Min(k, trainedEstimatedValues.Count)) {
          bool lowerIsCloser = upper >= trainedEstimatedValues.Count ||
                               (lower > 0 && ev - trainedEstimatedValues[lower] <= trainedEstimatedValues[upper] - ev);
          bool upperIsCloser = lower <= 0 ||
                               (upper < trainedEstimatedValues.Count &&
                                ev - trainedEstimatedValues[lower] >= trainedEstimatedValues[upper] - ev);
          if (!lowerIsCloser && !upperIsCloser) break;
          if (lowerIsCloser) {
            lower--;
            // eat up all equal values
            while (lower > 0 && trainedEstimatedValues[lower - 1].IsAlmost(trainedEstimatedValues[lower]))
              lower--;
          }
          if (upperIsCloser) {
            upper++;
            while (upper < trainedEstimatedValues.Count &&
                   trainedEstimatedValues[upper - 1].IsAlmost(trainedEstimatedValues[upper]))
              upper++;
          }
        }
        // majority voting with preference for bigger class in case of tie
        yield return Enumerable.Range(lower, upper - lower)
          .Select(i => trainedClasses[i])
          .GroupBy(c => c)
          .Select(g => new { Class = g.Key, Votes = g.Count() })
          .MaxItems(p => p.Votes)
          .OrderByDescending(m => m.Class, frequencyComparer)
          .First().Class;
      }
    }

    public override void RecalculateModelParameters(IClassificationProblemData problemData, IEnumerable<int> rows) {
      var estimatedValues = Interpreter.GetSymbolicExpressionTreeValues(SymbolicExpressionTree, problemData.Dataset, rows)
                                       .LimitToRange(LowerEstimationLimit, UpperEstimationLimit);
      var targetValues = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, rows);
      var trainedClasses = targetValues.ToArray();
      var trainedEstimatedValues = estimatedValues.ToArray();

      Array.Sort(trainedEstimatedValues, trainedClasses);
      this.trainedClasses = new List<double>(trainedClasses);
      this.trainedEstimatedValues = new List<double>(trainedEstimatedValues);

      var freq = trainedClasses
        .GroupBy(c => c)
        .ToDictionary(g => g.Key, g => g.Count());
      this.frequencyComparer = new ClassFrequencyComparer(freq);
    }

    public override ISymbolicClassificationSolution CreateClassificationSolution(IClassificationProblemData problemData) {
      return new SymbolicClassificationSolution((ISymbolicClassificationModel)Clone(), problemData);
    }
  }

  [StorableType("523AFB5D-3758-4547-BD6E-1181A01A02B4")]
  internal sealed class ClassFrequencyComparer : IComparer<double> {
    [Storable]
    private readonly Dictionary<double, int> classFrequencies;

    [StorableConstructor]
    private ClassFrequencyComparer(StorableConstructorFlag _) { }
    public ClassFrequencyComparer() {
      classFrequencies = new Dictionary<double, int>();
    }
    public ClassFrequencyComparer(Dictionary<double, int> frequencies) {
      classFrequencies = frequencies;
    }
    public ClassFrequencyComparer(ClassFrequencyComparer original) {
      classFrequencies = new Dictionary<double, int>(original.classFrequencies);
    }

    public int Compare(double x, double y) {
      bool cx = classFrequencies.ContainsKey(x), cy = classFrequencies.ContainsKey(y);
      if (cx && cy)
        return classFrequencies[x].CompareTo(classFrequencies[y]);
      if (cx) return 1;
      return -1;
    }
  }
}
