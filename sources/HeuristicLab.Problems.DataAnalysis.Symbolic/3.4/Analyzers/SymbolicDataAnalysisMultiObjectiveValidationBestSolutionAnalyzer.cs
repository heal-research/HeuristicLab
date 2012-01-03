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
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  /// <summary>
  /// An operator that analyzes the validation best symbolic data analysis solution for multi objective symbolic data analysis problems.
  /// </summary>
  [Item("SymbolicDataAnalysisMultiObjectiveValidationBestSolutionAnalyzer", "An operator that analyzes the validation best symbolic data analysis solution for multi objective symbolic data analysis problems.")]
  [StorableClass]
  public abstract class SymbolicDataAnalysisMultiObjectiveValidationBestSolutionAnalyzer<S, T, U> : SymbolicDataAnalysisMultiObjectiveValidationAnalyzer<T, U>,
    ISymbolicDataAnalysisMultiObjectiveAnalyzer
    where S : class, ISymbolicDataAnalysisSolution
    where T : class, ISymbolicDataAnalysisMultiObjectiveEvaluator<U>
    where U : class, IDataAnalysisProblemData {
    private const string ValidationBestSolutionsParameterName = "Best validation solutions";
    private const string ValidationBestSolutionQualitiesParameterName = "Best validation solution qualities";

    #region parameter properties
    public ILookupParameter<ItemList<S>> ValidationBestSolutionsParameter {
      get { return (ILookupParameter<ItemList<S>>)Parameters[ValidationBestSolutionsParameterName]; }
    }
    public ILookupParameter<ItemList<DoubleArray>> ValidationBestSolutionQualitiesParameter {
      get { return (ILookupParameter<ItemList<DoubleArray>>)Parameters[ValidationBestSolutionQualitiesParameterName]; }
    }
    #endregion
    #region properties
    public ItemList<S> ValidationBestSolutions {
      get { return ValidationBestSolutionsParameter.ActualValue; }
      set { ValidationBestSolutionsParameter.ActualValue = value; }
    }
    public ItemList<DoubleArray> ValidationBestSolutionQualities {
      get { return ValidationBestSolutionQualitiesParameter.ActualValue; }
      set { ValidationBestSolutionQualitiesParameter.ActualValue = value; }
    }
    #endregion

    [StorableConstructor]
    protected SymbolicDataAnalysisMultiObjectiveValidationBestSolutionAnalyzer(bool deserializing) : base(deserializing) { }
    protected SymbolicDataAnalysisMultiObjectiveValidationBestSolutionAnalyzer(SymbolicDataAnalysisMultiObjectiveValidationBestSolutionAnalyzer<S, T, U> original, Cloner cloner) : base(original, cloner) { }
    public SymbolicDataAnalysisMultiObjectiveValidationBestSolutionAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<ItemList<S>>(ValidationBestSolutionsParameterName, "The validation best (Pareto-optimal) symbolic data analysis solutions."));
      Parameters.Add(new LookupParameter<ItemList<DoubleArray>>(ValidationBestSolutionQualitiesParameterName, "The qualities of the validation best (Pareto-optimal) solutions."));
    }

    public override IOperation Apply() {
      IEnumerable<int> rows = GenerateRowsToEvaluate();
      if (!rows.Any()) return base.Apply();

      var results = ResultCollection;
      // create empty parameter and result values
      if (ValidationBestSolutions == null) {
        ValidationBestSolutions = new ItemList<S>();
        ValidationBestSolutionQualities = new ItemList<DoubleArray>();
        results.Add(new Result(ValidationBestSolutionQualitiesParameter.Name, ValidationBestSolutionQualitiesParameter.Description, ValidationBestSolutionQualities));
        results.Add(new Result(ValidationBestSolutionsParameter.Name, ValidationBestSolutionsParameter.Description, ValidationBestSolutions));
      }

      IList<double[]> trainingBestQualities = ValidationBestSolutionQualities
        .Select(x => x.ToArray())
        .ToList();

      #region find best trees
      IList<int> nonDominatedIndexes = new List<int>();
      ISymbolicExpressionTree[] tree = SymbolicExpressionTree.ToArray();
      bool[] maximization = Maximization.ToArray();
      List<double[]> newNonDominatedQualities = new List<double[]>();
      var evaluator = EvaluatorParameter.ActualValue;
      var problemData = ProblemDataParameter.ActualValue;
      IExecutionContext childContext = (IExecutionContext)ExecutionContext.CreateChildOperation(evaluator);

      var qualities = tree
        .AsParallel()
        .Select(t => evaluator.Evaluate(childContext, t, problemData, rows))
        .ToArray();
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
        ItemList<S> nonDominatedSolutions = new ItemList<S>();
        // add all new non-dominated solutions to the archive
        foreach (var index in nonDominatedIndexes) {
          S solution = CreateSolution(tree[index], qualities[index]);
          nonDominatedSolutions.Add(solution);
          nonDominatedQualities.Add(new DoubleArray(qualities[index]));
        }
        // add old non-dominated solutions only if they are not dominated by one of the new solutions
        for (int i = 0; i < trainingBestQualities.Count; i++) {
          if (IsNonDominated(trainingBestQualities[i], newNonDominatedQualities, maximization)) {
            if (!newNonDominatedQualities.Contains(trainingBestQualities[i], new DoubleArrayComparer())) {
              nonDominatedSolutions.Add(ValidationBestSolutions[i]);
              nonDominatedQualities.Add(ValidationBestSolutionQualities[i]);
            }
          }
        }

        results[ValidationBestSolutionsParameter.Name].Value = nonDominatedSolutions;
        results[ValidationBestSolutionQualitiesParameter.Name].Value = nonDominatedQualities;
      }
      #endregion
      return base.Apply();
    }

    protected abstract S CreateSolution(ISymbolicExpressionTree bestTree, double[] bestQuality);

    private bool IsNonDominated(double[] point, IList<double[]> points, bool[] maximization) {
      foreach (var refPoint in points) {
        bool refPointDominatesPoint = true;
        for (int i = 0; i < point.Length; i++) {
          refPointDominatesPoint &= IsBetter(refPoint[i], point[i], maximization[i]);
        }
        if (refPointDominatesPoint) return false;
      }
      return true;
    }
    private bool IsBetter(double lhs, double rhs, bool maximization) {
      if (maximization) return lhs > rhs;
      else return lhs < rhs;
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

  }
}
