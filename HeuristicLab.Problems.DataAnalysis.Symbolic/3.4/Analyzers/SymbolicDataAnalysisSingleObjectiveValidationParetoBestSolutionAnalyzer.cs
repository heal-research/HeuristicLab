#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Fossil;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  /// <summary>
  /// An operator that collects the Pareto-best symbolic data analysis solutions for single objective symbolic data analysis problems.
  /// </summary>
  [Item("SymbolicDataAnalysisSingleObjectiveValidationParetoBestSolutionAnalyzer", "An operator that analyzes the Pareto-best symbolic data analysis solution for single objective symbolic data analysis problems.")]
  [StorableType("892CE424-FAB0-4E78-8BC2-40BFD1F4A78A")]
  public abstract class SymbolicDataAnalysisSingleObjectiveValidationParetoBestSolutionAnalyzer<S, T, U> : SymbolicDataAnalysisSingleObjectiveValidationAnalyzer<T, U>, ISymbolicDataAnalysisBoundedOperator
    where S : class, ISymbolicDataAnalysisSolution
    where T : class, ISymbolicDataAnalysisSingleObjectiveEvaluator<U>
    where U : class, IDataAnalysisProblemData {
    private const string ValidationBestSolutionsParameterName = "Best validation solutions";
    private const string ValidationBestSolutionQualitiesParameterName = "Best validation solution qualities";
    private const string ComplexityParameterName = "Complexity";
    private const string EstimationLimitsParameterName = "EstimationLimits";

    public override bool EnabledByDefault {
      get { return false; }
    }

    #region parameter properties
    public ILookupParameter<ItemList<S>> ValidationBestSolutionsParameter {
      get { return (ILookupParameter<ItemList<S>>)Parameters[ValidationBestSolutionsParameterName]; }
    }
    public ILookupParameter<ItemList<DoubleArray>> ValidationBestSolutionQualitiesParameter {
      get { return (ILookupParameter<ItemList<DoubleArray>>)Parameters[ValidationBestSolutionQualitiesParameterName]; }
    }
    public IScopeTreeLookupParameter<DoubleValue> ComplexityParameter {
      get { return (IScopeTreeLookupParameter<DoubleValue>)Parameters[ComplexityParameterName]; }
    }
    public IValueLookupParameter<DoubleLimit> EstimationLimitsParameter {
      get { return (IValueLookupParameter<DoubleLimit>)Parameters[EstimationLimitsParameterName]; }
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
    protected SymbolicDataAnalysisSingleObjectiveValidationParetoBestSolutionAnalyzer(StorableConstructorFlag _) : base(_) { }
    protected SymbolicDataAnalysisSingleObjectiveValidationParetoBestSolutionAnalyzer(SymbolicDataAnalysisSingleObjectiveValidationParetoBestSolutionAnalyzer<S, T, U> original, Cloner cloner) : base(original, cloner) { }
    public SymbolicDataAnalysisSingleObjectiveValidationParetoBestSolutionAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<ItemList<S>>(ValidationBestSolutionsParameterName, "The validation best (Pareto-optimal) symbolic data analysis solutions."));
      Parameters.Add(new LookupParameter<ItemList<DoubleArray>>(ValidationBestSolutionQualitiesParameterName, "The qualities of the validation best (Pareto-optimal) solutions."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>(ComplexityParameterName, "The complexity of each tree."));
      Parameters.Add(new ValueLookupParameter<DoubleLimit>(EstimationLimitsParameterName, "The lower and upper limit for the estimated values produced by the symbolic classification model."));
    }

    public override IOperation Apply() {
      IEnumerable<int> rows = GenerateRowsToEvaluate();
      if (!rows.Any()) return base.Apply();

      #region find best tree
      var evaluator = EvaluatorParameter.ActualValue;
      var problemData = ProblemDataParameter.ActualValue;
      ISymbolicExpressionTree[] tree = SymbolicExpressionTree.ToArray();

      // sort is ascending and we take the first n% => order so that best solutions are smallest
      // sort order is determined by maximization parameter
      double[] trainingQuality;
      if (Maximization.Value) {
        // largest values must be sorted first
        trainingQuality = Quality.Select(x => -x.Value).ToArray();
      } else {
        // smallest values must be sorted first
        trainingQuality = Quality.Select(x => x.Value).ToArray();
      }

      int[] treeIndexes = Enumerable.Range(0, tree.Length).ToArray();

      // sort trees by training qualities
      Array.Sort(trainingQuality, treeIndexes);

      // number of best training solutions to validate (at least 1)
      int topN = (int)Math.Max(tree.Length * PercentageOfBestSolutionsParameter.ActualValue.Value, 1);

      IExecutionContext childContext = (IExecutionContext)ExecutionContext.CreateChildOperation(evaluator);
      // evaluate best n training trees on validiation set
      var qualities = treeIndexes
        .Select(i => tree[i])
        .Take(topN)
        .Select(t => evaluator.Evaluate(childContext, t, problemData, rows))
        .ToArray();
      #endregion

      var results = ResultCollection;
      // create empty parameter and result values
      if (ValidationBestSolutions == null) {
        ValidationBestSolutions = new ItemList<S>();
        ValidationBestSolutionQualities = new ItemList<DoubleArray>();
        results.Add(new Result(ValidationBestSolutionQualitiesParameter.Name, ValidationBestSolutionQualitiesParameter.Description, ValidationBestSolutionQualities));
        results.Add(new Result(ValidationBestSolutionsParameter.Name, ValidationBestSolutionsParameter.Description, ValidationBestSolutions));
      }

      IList<Tuple<double, double>> validationBestQualities = ValidationBestSolutionQualities
        .Select(x => Tuple.Create(x[0], x[1]))
        .ToList();

      #region find best trees
      IList<int> nonDominatedIndexes = new List<int>();

      List<double> complexities;
      if (ComplexityParameter.ActualValue != null && ComplexityParameter.ActualValue.Length == trainingQuality.Length) {
        complexities = ComplexityParameter.ActualValue.Select(x => x.Value).ToList();
      } else {
        complexities = tree.Select(t => (double)t.Length).ToList();
      }
      List<Tuple<double, double>> fitness = new List<Tuple<double, double>>();
      for (int i = 0; i < qualities.Length; i++)
        fitness.Add(Tuple.Create(qualities[i], complexities[treeIndexes[i]]));

      var maximization = Tuple.Create(Maximization.Value, false); // complexity must be minimized
      List<Tuple<double, double>> newNonDominatedQualities = new List<Tuple<double, double>>();
      for (int i = 0; i < fitness.Count; i++) {
        if (IsNonDominated(fitness[i], validationBestQualities, maximization) &&
          IsNonDominated(fitness[i], newNonDominatedQualities, maximization) &&
          IsNonDominated(fitness[i], fitness.Skip(i + 1), maximization)) {
          if (!newNonDominatedQualities.Contains(fitness[i])) {
            newNonDominatedQualities.Add(fitness[i]);
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
          S solution = CreateSolution(tree[treeIndexes[index]]);
          nonDominatedSolutions.Add(solution);
          nonDominatedQualities.Add(new DoubleArray(new double[] { fitness[index].Item1, fitness[index].Item2 }));
        }
        // add old non-dominated solutions only if they are not dominated by one of the new solutions
        for (int i = 0; i < validationBestQualities.Count; i++) {
          if (IsNonDominated(validationBestQualities[i], newNonDominatedQualities, maximization)) {
            if (!newNonDominatedQualities.Contains(validationBestQualities[i])) {
              nonDominatedSolutions.Add(ValidationBestSolutions[i]);
              nonDominatedQualities.Add(ValidationBestSolutionQualities[i]);
            }
          }
        }

        // make sure solutions and qualities are ordered in the results
        var orderedIndexes =
          nonDominatedSolutions.Select((s, i) => i).OrderBy(i => nonDominatedQualities[i][0]).ToArray();

        var orderedNonDominatedSolutions = new ItemList<S>();
        var orderedNonDominatedQualities = new ItemList<DoubleArray>();
        foreach (var i in orderedIndexes) {
          orderedNonDominatedQualities.Add(nonDominatedQualities[i]);
          orderedNonDominatedSolutions.Add(nonDominatedSolutions[i]);
        }

        ValidationBestSolutions = orderedNonDominatedSolutions;
        ValidationBestSolutionQualities = orderedNonDominatedQualities;

        results[ValidationBestSolutionsParameter.Name].Value = orderedNonDominatedSolutions;
        results[ValidationBestSolutionQualitiesParameter.Name].Value = orderedNonDominatedQualities;
      }
      #endregion
      return base.Apply();
    }

    protected abstract S CreateSolution(ISymbolicExpressionTree bestTree);

    private bool IsNonDominated(Tuple<double, double> point, IEnumerable<Tuple<double, double>> points, Tuple<bool, bool> maximization) {
      return !points.Any(p => IsBetterOrEqual(p.Item1, point.Item1, maximization.Item1) &&
                             IsBetterOrEqual(p.Item2, point.Item2, maximization.Item2));
    }
    private bool IsBetterOrEqual(double lhs, double rhs, bool maximization) {
      if (maximization) return lhs >= rhs;
      else return lhs <= rhs;
    }
  }
}
