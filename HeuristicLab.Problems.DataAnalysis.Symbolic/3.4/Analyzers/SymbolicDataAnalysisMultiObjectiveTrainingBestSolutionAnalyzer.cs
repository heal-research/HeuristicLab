#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  /// <summary>
  /// An operator that analyzes the training best symbolic data analysis solution for multi objective symbolic data analysis problems.
  /// </summary>
  [Item("SymbolicDataAnalysisMultiObjectiveTrainingBestSolutionAnalyzer", "An operator that analyzes the training best symbolic data analysis solution for multi objective symbolic data analysis problems.")]
  [StorableClass]
  public abstract class SymbolicDataAnalysisMultiObjectiveTrainingBestSolutionAnalyzer<T> : SymbolicDataAnalysisMultiObjectiveAnalyzer
    where T : class, ISymbolicDataAnalysisSolution {
    private const string TrainingBestSolutionsParameterName = "Best training solutions";
    private const string TrainingBestSolutionQualitiesParameterName = "Best training solution qualities";
    private const string UpdateAlwaysParameterName = "Always update best solutions";

    #region parameter properties
    public ILookupParameter<ItemList<T>> TrainingBestSolutionsParameter {
      get { return (ILookupParameter<ItemList<T>>)Parameters[TrainingBestSolutionsParameterName]; }
    }
    public ILookupParameter<ItemList<DoubleArray>> TrainingBestSolutionQualitiesParameter {
      get { return (ILookupParameter<ItemList<DoubleArray>>)Parameters[TrainingBestSolutionQualitiesParameterName]; }
    }
    public IFixedValueParameter<BoolValue> UpdateAlwaysParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters[UpdateAlwaysParameterName]; }
    }
    #endregion
    #region properties
    public ItemList<T> TrainingBestSolutions {
      get { return TrainingBestSolutionsParameter.ActualValue; }
      set { TrainingBestSolutionsParameter.ActualValue = value; }
    }
    public ItemList<DoubleArray> TrainingBestSolutionQualities {
      get { return TrainingBestSolutionQualitiesParameter.ActualValue; }
      set { TrainingBestSolutionQualitiesParameter.ActualValue = value; }
    }
    public BoolValue UpdateAlways {
      get { return UpdateAlwaysParameter.Value; }
    }
    #endregion

    [StorableConstructor]
    protected SymbolicDataAnalysisMultiObjectiveTrainingBestSolutionAnalyzer(bool deserializing) : base(deserializing) { }
    protected SymbolicDataAnalysisMultiObjectiveTrainingBestSolutionAnalyzer(SymbolicDataAnalysisMultiObjectiveTrainingBestSolutionAnalyzer<T> original, Cloner cloner) : base(original, cloner) { }
    public SymbolicDataAnalysisMultiObjectiveTrainingBestSolutionAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<ItemList<T>>(TrainingBestSolutionsParameterName, "The training best (Pareto-optimal) symbolic data analysis solutions."));
      Parameters.Add(new LookupParameter<ItemList<DoubleArray>>(TrainingBestSolutionQualitiesParameterName, "The qualities of the training best (Pareto-optimal) solutions."));
      Parameters.Add(new FixedValueParameter<BoolValue>(UpdateAlwaysParameterName, "Determines if the best training solutions should always be updated regardless of its quality.", new BoolValue(false)));
      UpdateAlwaysParameter.Hidden = true;
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (!Parameters.ContainsKey(UpdateAlwaysParameterName)) {
        Parameters.Add(new FixedValueParameter<BoolValue>(UpdateAlwaysParameterName, "Determines if the best training solutions should always be updated regardless of its quality.", new BoolValue(false)));
        UpdateAlwaysParameter.Hidden = true;
      }
    }

    public override IOperation Apply() {
      var results = ResultCollection;
      // create empty parameter and result values
      if (TrainingBestSolutions == null) {
        TrainingBestSolutions = new ItemList<T>();
        TrainingBestSolutionQualities = new ItemList<DoubleArray>();
        results.Add(new Result(TrainingBestSolutionQualitiesParameter.Name, TrainingBestSolutionQualitiesParameter.Description, TrainingBestSolutionQualities));
        results.Add(new Result(TrainingBestSolutionsParameter.Name, TrainingBestSolutionsParameter.Description, TrainingBestSolutions));
      }

      //if the pareto front of best solutions shall be updated regardless of the quality, the list initialized empty to discard old solutions
      IList<double[]> trainingBestQualities;
      if (UpdateAlways.Value) {
        trainingBestQualities = new List<double[]>();
      } else {
        trainingBestQualities = TrainingBestSolutionQualities.Select(x => x.ToArray()).ToList();
      }

      #region find best trees
      IList<int> nonDominatedIndexes = new List<int>();
      ISymbolicExpressionTree[] tree = SymbolicExpressionTree.ToArray();
      List<double[]> qualities = Qualities.Select(x => x.ToArray()).ToList();
      bool[] maximization = Maximization.ToArray();
      List<double[]> newNonDominatedQualities = new List<double[]>();
      for (int i = 0; i < tree.Length; i++) {
        if (IsNonDominated(qualities[i], trainingBestQualities, maximization) &&
          IsNonDominated(qualities[i], qualities, maximization)) {
          if (!newNonDominatedQualities.Contains(qualities[i], new DoubleArrayComparer())) {
            newNonDominatedQualities.Add(qualities[i]);
            nonDominatedIndexes.Add(i);
          }
        }
      }
      #endregion
      #region update Pareto-optimal solution archive
      if (nonDominatedIndexes.Count > 0) {
        ItemList<DoubleArray> nonDominatedQualities = new ItemList<DoubleArray>();
        ItemList<T> nonDominatedSolutions = new ItemList<T>();
        // add all new non-dominated solutions to the archive
        foreach (var index in nonDominatedIndexes) {
          T solution = CreateSolution(tree[index], qualities[index]);
          nonDominatedSolutions.Add(solution);
          nonDominatedQualities.Add(new DoubleArray(qualities[index]));
        }
        // add old non-dominated solutions only if they are not dominated by one of the new solutions
        for (int i = 0; i < trainingBestQualities.Count; i++) {
          if (IsNonDominated(trainingBestQualities[i], newNonDominatedQualities, maximization)) {
            if (!newNonDominatedQualities.Contains(trainingBestQualities[i], new DoubleArrayComparer())) {
              nonDominatedSolutions.Add(TrainingBestSolutions[i]);
              nonDominatedQualities.Add(TrainingBestSolutionQualities[i]);
            }
          }
        }

        results[TrainingBestSolutionsParameter.Name].Value = nonDominatedSolutions;
        results[TrainingBestSolutionQualitiesParameter.Name].Value = nonDominatedQualities;
      }
      #endregion
      return base.Apply();
    }

    private class DoubleArrayComparer : IEqualityComparer<double[]> {
      public bool Equals(double[] x, double[] y) {
        if (y.Length != x.Length) throw new ArgumentException();
        for (int i = 0; i < x.Length; i++) {
          if (!x[i].IsAlmost(y[i])) return false;
        }
        return true;
      }

      public int GetHashCode(double[] obj) {
        int c = obj.Length;
        for (int i = 0; i < obj.Length; i++)
          c ^= obj[i].GetHashCode();
        return c;
      }
    }

    protected abstract T CreateSolution(ISymbolicExpressionTree bestTree, double[] bestQuality);

    private bool IsNonDominated(double[] point, IList<double[]> points, bool[] maximization) {
      foreach (var refPoint in points) {
        bool refPointDominatesPoint = true;
        for (int i = 0; i < point.Length; i++) {
          refPointDominatesPoint &= IsBetterOrEqual(refPoint[i], point[i], maximization[i]);
        }
        if (refPointDominatesPoint) return false;
      }
      return true;
    }
    private bool IsBetterOrEqual(double lhs, double rhs, bool maximization) {
      if (maximization) return lhs > rhs;
      else return lhs < rhs;
    }
  }
}
