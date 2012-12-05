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
    private List<KeyValuePair<double, Dictionary<double, int>>> trainedTargetPair;
    [Storable]
    private ClassFrequencyComparer frequencyComparer;

    [StorableConstructor]
    private SymbolicNearestNeighbourClassificationModel(bool deserializing) : base(deserializing) { }
    private SymbolicNearestNeighbourClassificationModel(SymbolicNearestNeighbourClassificationModel original, Cloner cloner)
      : base(original, cloner) {
      k = original.k;
      trainedTargetPair = original.trainedTargetPair.Select(x => new KeyValuePair<double, Dictionary<double, int>>(x.Key, new Dictionary<double, int>(x.Value))).ToList();
      frequencyComparer = new ClassFrequencyComparer(original.frequencyComparer);
    }
    public SymbolicNearestNeighbourClassificationModel(int k, ISymbolicExpressionTree tree, ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, double lowerEstimationLimit = double.MinValue, double upperEstimationLimit = double.MaxValue)
      : base(tree, interpreter, lowerEstimationLimit, upperEstimationLimit) {
      this.k = k;
      trainedTargetPair = new List<KeyValuePair<double, Dictionary<double, int>>>();
      frequencyComparer = new ClassFrequencyComparer();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicNearestNeighbourClassificationModel(this, cloner);
    }

    public override IEnumerable<double> GetEstimatedClassValues(Dataset dataset, IEnumerable<int> rows) {
      var estimatedValues = Interpreter.GetSymbolicExpressionTreeValues(SymbolicExpressionTree, dataset, rows)
                                       .LimitToRange(LowerEstimationLimit, UpperEstimationLimit);
      var neighborClasses = new Dictionary<double, int>();
      foreach (var ev in estimatedValues) {
        // find the index of the training-point to which distance is shortest
        var upper = trainedTargetPair.BinarySearch(0, trainedTargetPair.Count, new KeyValuePair<double, Dictionary<double, int>>(ev, null), new KeyValuePairKeyComparer<Dictionary<double, int>>());
        if (upper < 0) upper = ~upper; // if the element was not found exactly, BinarySearch returns the complement of the index of the next larger item
        var lower = upper - 1;
        neighborClasses.Clear();
        // continue to the left and right of this index and look for the nearest neighbors
        for (int i = 0; i < Math.Min(k, trainedTargetPair.Count); i++) {
          bool lowerIsCloser = upper >= trainedTargetPair.Count || (lower >= 0 && ev - trainedTargetPair[lower].Key <= trainedTargetPair[upper].Key - ev);
          bool upperIsCloser = lower < 0 || (upper < trainedTargetPair.Count && ev - trainedTargetPair[lower].Key >= trainedTargetPair[upper].Key - ev);
          if (lowerIsCloser) {
            // the nearer neighbor is to the left
            var lowerClassSamples = trainedTargetPair[lower].Value.Select(x => x.Value).Sum(); // should be 1, except when multiple samples are estimated the same value
            var lowerClasses = trainedTargetPair[lower].Value;
            foreach (var lowerClass in lowerClasses) {
              if (!neighborClasses.ContainsKey(lowerClass.Key)) neighborClasses[lowerClass.Key] = lowerClass.Value;
              else neighborClasses[lowerClass.Key] += lowerClass.Value;
            }
            lower--;
            i += (lowerClassSamples - 1);
          }
          // they could, in very rare cases, be equally far apart
          if (upperIsCloser) {
            // the nearer neighbor is to the right
            var upperClassSamples = trainedTargetPair[upper].Value.Select(x => x.Value).Sum(); // should be 1, except when multiple samples are estimated the same value
            var upperClasses = trainedTargetPair[upper].Value;
            foreach (var upperClass in upperClasses) {
              if (!neighborClasses.ContainsKey(upperClass.Key)) neighborClasses[upperClass.Key] = upperClass.Value;
              else neighborClasses[upperClass.Key] += upperClass.Value;
            }
            upper++;
            i += (upperClassSamples - 1);
          }
        }
        // majority voting with preference for bigger class in case of tie
        yield return neighborClasses.MaxItems(x => x.Value).OrderByDescending(x => x.Key, frequencyComparer).First().Key;
      }
    }

    public override void RecalculateModelParameters(IClassificationProblemData problemData, IEnumerable<int> rows) {
      var estimatedValues = Interpreter.GetSymbolicExpressionTreeValues(SymbolicExpressionTree, problemData.Dataset, rows)
                                       .LimitToRange(LowerEstimationLimit, UpperEstimationLimit);
      var targetValues = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, rows);
      var pair = estimatedValues.Zip(targetValues, (e, t) => new { Estimated = e, Target = t });

      // there could be more than one target value per estimated value
      var dict = new Dictionary<double, Dictionary<double, int>>();
      var classFrequencies = new Dictionary<double, int>();
      foreach (var p in pair) {
        // get target values for each estimated value (foreach estimated value there may be different target values in different amounts) 
        if (!dict.ContainsKey(p.Estimated)) dict[p.Estimated] = new Dictionary<double, int>();
        if (!dict[p.Estimated].ContainsKey(p.Target)) dict[p.Estimated][p.Target] = 0;
        dict[p.Estimated][p.Target]++;
        // get class frequencies
        if (!classFrequencies.ContainsKey(p.Target))
          classFrequencies[p.Target] = 1;
        else classFrequencies[p.Target]++;
      }

      frequencyComparer = new ClassFrequencyComparer(classFrequencies);

      trainedTargetPair = new List<KeyValuePair<double, Dictionary<double, int>>>();
      foreach (var ev in dict) {
        trainedTargetPair.Add(new KeyValuePair<double, Dictionary<double, int>>(ev.Key, ev.Value));
      }
      trainedTargetPair = trainedTargetPair.OrderBy(x => x.Key).ToList();
    }

    public override ISymbolicClassificationSolution CreateClassificationSolution(IClassificationProblemData problemData) {
      return new SymbolicClassificationSolution((ISymbolicClassificationModel)Clone(), problemData);
    }
  }

  internal class KeyValuePairKeyComparer<T> : IComparer<KeyValuePair<double, T>> {
    public int Compare(KeyValuePair<double, T> x, KeyValuePair<double, T> y) {
      return x.Key.CompareTo(y.Key);
    }
  }

  [StorableClass]
  internal sealed class ClassFrequencyComparer : IComparer<double> {
    [Storable]
    private readonly Dictionary<double, int> classFrequencies;

    [StorableConstructor]
    private ClassFrequencyComparer(bool deserializing) { }
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
