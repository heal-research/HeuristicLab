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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  /// <summary>
  /// An operator that collects the Pareto-best symbolic data analysis solutions for single objective symbolic data analysis problems.
  /// </summary>
  [Item("SymbolicDataAnalysisSingleObjectiveTrainingParetoBestSolutionAnalyzer", "An operator that analyzes the Pareto-best symbolic data analysis solution for single objective symbolic data analysis problems.")]
  [StorableClass]
  public abstract class SymbolicDataAnalysisSingleObjectiveTrainingParetoBestSolutionAnalyzer<S, T> : SymbolicDataAnalysisSingleObjectiveAnalyzer, ISymbolicDataAnalysisInterpreterOperator, ISymbolicDataAnalysisBoundedOperator
    where T : class, ISymbolicDataAnalysisSolution
    where S : class, IDataAnalysisProblemData {
    private const string ProblemDataParameterName = "ProblemData";
    private const string TrainingBestSolutionsParameterName = "Best training solutions";
    private const string TrainingBestSolutionQualitiesParameterName = "Best training solution qualities";
    private const string ComplexityParameterName = "Complexity";
    private const string SymbolicDataAnalysisTreeInterpreterParameterName = "SymbolicDataAnalysisTreeInterpreter";
    private const string EstimationLimitsParameterName = "EstimationLimits";

    public override bool EnabledByDefault {
      get { return false; }
    }

    #region parameter properties
    public ILookupParameter<ItemList<T>> TrainingBestSolutionsParameter {
      get { return (ILookupParameter<ItemList<T>>)Parameters[TrainingBestSolutionsParameterName]; }
    }
    public ILookupParameter<ItemList<DoubleArray>> TrainingBestSolutionQualitiesParameter {
      get { return (ILookupParameter<ItemList<DoubleArray>>)Parameters[TrainingBestSolutionQualitiesParameterName]; }
    }
    public IScopeTreeLookupParameter<DoubleValue> ComplexityParameter {
      get { return (IScopeTreeLookupParameter<DoubleValue>)Parameters[ComplexityParameterName]; }
    }
    public ILookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter> SymbolicDataAnalysisTreeInterpreterParameter {
      get { return (ILookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>)Parameters[SymbolicDataAnalysisTreeInterpreterParameterName]; }
    }
    public ILookupParameter<S> ProblemDataParameter {
      get { return (ILookupParameter<S>)Parameters[ProblemDataParameterName]; }
    }
    public IValueLookupParameter<DoubleLimit> EstimationLimitsParameter {
      get { return (IValueLookupParameter<DoubleLimit>)Parameters[EstimationLimitsParameterName]; }
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
    #endregion

    [StorableConstructor]
    protected SymbolicDataAnalysisSingleObjectiveTrainingParetoBestSolutionAnalyzer(bool deserializing) : base(deserializing) { }
    protected SymbolicDataAnalysisSingleObjectiveTrainingParetoBestSolutionAnalyzer(SymbolicDataAnalysisSingleObjectiveTrainingParetoBestSolutionAnalyzer<S, T> original, Cloner cloner) : base(original, cloner) { }
    public SymbolicDataAnalysisSingleObjectiveTrainingParetoBestSolutionAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<S>(ProblemDataParameterName, "The problem data for the symbolic data analysis solution."));
      Parameters.Add(new LookupParameter<ItemList<T>>(TrainingBestSolutionsParameterName, "The training best (Pareto-optimal) symbolic data analysis solutions."));
      Parameters.Add(new LookupParameter<ItemList<DoubleArray>>(TrainingBestSolutionQualitiesParameterName, "The qualities of the training best (Pareto-optimal) solutions."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>(ComplexityParameterName, "The complexity of each tree."));
      Parameters.Add(new LookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>(SymbolicDataAnalysisTreeInterpreterParameterName, "The symbolic data analysis tree interpreter for the symbolic expression tree."));
      Parameters.Add(new ValueLookupParameter<DoubleLimit>(EstimationLimitsParameterName, "The lower and upper limit for the estimated values produced by the symbolic classification model."));
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

      IList<Tuple<double, double>> trainingBestQualities = TrainingBestSolutionQualities
        .Select(x => Tuple.Create(x[0], x[1]))
        .ToList();

      #region find best trees
      IList<int> nonDominatedIndexes = new List<int>();
      ISymbolicExpressionTree[] tree = SymbolicExpressionTree.ToArray();
      List<double> qualities = Quality.Select(x => x.Value).ToList();

      List<double> complexities;
      if (ComplexityParameter.ActualValue != null && ComplexityParameter.ActualValue.Length == qualities.Count) {
        complexities = ComplexityParameter.ActualValue.Select(x => x.Value).ToList();
      } else {
        complexities = tree.Select(t => (double)t.Length).ToList();
      }
      List<Tuple<double, double>> fitness = new List<Tuple<double, double>>();
      for (int i = 0; i < qualities.Count; i++)
        fitness.Add(Tuple.Create(qualities[i], complexities[i]));
      var maximization = Tuple.Create(Maximization.Value, false);// complexity must be minimized
      List<Tuple<double, double>> newNonDominatedQualities = new List<Tuple<double, double>>();
      for (int i = 0; i < tree.Length; i++) {
        if (IsNonDominated(fitness[i], trainingBestQualities, maximization) &&
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
        ItemList<T> nonDominatedSolutions = new ItemList<T>();
        // add all new non-dominated solutions to the archive
        foreach (var index in nonDominatedIndexes) {
          T solution = CreateSolution(tree[index]);
          nonDominatedSolutions.Add(solution);
          nonDominatedQualities.Add(new DoubleArray(new double[] { fitness[index].Item1, fitness[index].Item2 }));
        }
        // add old non-dominated solutions only if they are not dominated by one of the new solutions
        for (int i = 0; i < trainingBestQualities.Count; i++) {
          if (IsNonDominated(trainingBestQualities[i], newNonDominatedQualities, maximization)) {
            if (!newNonDominatedQualities.Contains(trainingBestQualities[i])) {
              nonDominatedSolutions.Add(TrainingBestSolutions[i]);
              nonDominatedQualities.Add(TrainingBestSolutionQualities[i]);
            }
          }
        }

        // make sure solutions and qualities are ordered in the results
        var orderedIndexes =
          nonDominatedSolutions.Select((s, i) => i).OrderBy(i => nonDominatedQualities[i][0]).ToArray();

        var orderedNonDominatedSolutions = new ItemList<T>();
        var orderedNonDominatedQualities = new ItemList<DoubleArray>();
        foreach (var i in orderedIndexes) {
          orderedNonDominatedQualities.Add(nonDominatedQualities[i]);
          orderedNonDominatedSolutions.Add(nonDominatedSolutions[i]);
        }

        TrainingBestSolutions = orderedNonDominatedSolutions;
        TrainingBestSolutionQualities = orderedNonDominatedQualities;

        results[TrainingBestSolutionsParameter.Name].Value = orderedNonDominatedSolutions;
        results[TrainingBestSolutionQualitiesParameter.Name].Value = orderedNonDominatedQualities;
      }
      #endregion
      return base.Apply();
    }

    protected abstract T CreateSolution(ISymbolicExpressionTree bestTree);

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
