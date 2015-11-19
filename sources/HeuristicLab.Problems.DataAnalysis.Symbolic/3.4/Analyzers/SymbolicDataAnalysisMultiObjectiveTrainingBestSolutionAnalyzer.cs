#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
    private const string TrainingBestSolutionParameterName = "Best training solution";

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
    private ItemList<T> TrainingBestSolutions {
      get { return TrainingBestSolutionsParameter.ActualValue; }
      set { TrainingBestSolutionsParameter.ActualValue = value; }
    }
    private ItemList<DoubleArray> TrainingBestSolutionQualities {
      get { return TrainingBestSolutionQualitiesParameter.ActualValue; }
      set { TrainingBestSolutionQualitiesParameter.ActualValue = value; }
    }
    public bool UpdateAlways {
      get { return UpdateAlwaysParameter.Value.Value; }
      set { UpdateAlwaysParameter.Value.Value = value; }
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

      if (!results.ContainsKey(TrainingBestSolutionParameterName)) {
        results.Add(new Result(TrainingBestSolutionParameterName, "", typeof(ISymbolicDataAnalysisSolution)));
      }

      //if the pareto front of best solutions shall be updated regardless of the quality, the list initialized empty to discard old solutions
      List<double[]> trainingBestQualities;
      if (UpdateAlways) {
        trainingBestQualities = new List<double[]>();
      } else {
        trainingBestQualities = TrainingBestSolutionQualities.Select(x => x.ToArray()).ToList();
      }

      ISymbolicExpressionTree[] trees = SymbolicExpressionTree.ToArray();
      List<double[]> qualities = Qualities.Select(x => x.ToArray()).ToList();
      bool[] maximization = Maximization.ToArray();

      var nonDominatedIndividuals = new[] { new { Tree = default(ISymbolicExpressionTree), Qualities = default(double[]) } }.ToList();
      nonDominatedIndividuals.Clear();

      // build list of new non-dominated solutions
      for (int i = 0; i < trees.Length; i++) {
        if (IsNonDominated(qualities[i], nonDominatedIndividuals.Select(ind => ind.Qualities), maximization) &&
            IsNonDominated(qualities[i], trainingBestQualities, maximization)) {
          for (int j = nonDominatedIndividuals.Count - 1; j >= 0; j--) {
            if (IsBetterOrEqual(qualities[i], nonDominatedIndividuals[j].Qualities, maximization)) {
              nonDominatedIndividuals.RemoveAt(j);
            }
          }
          nonDominatedIndividuals.Add(new { Tree = trees[i], Qualities = qualities[i] });
        }
      }

      var nonDominatedSolutions = nonDominatedIndividuals.Select(x => new { Solution = CreateSolution(x.Tree, x.Qualities), Qualities = x.Qualities }).ToList();
      nonDominatedSolutions.ForEach(s => s.Solution.Name = string.Join(",", s.Qualities.Select(q => q.ToString())));

      #region update Pareto-optimal solution archive
      if (nonDominatedSolutions.Count > 0) {
        //add old non-dominated solutions only if they are not dominated by one of the new solutions
        for (int i = 0; i < trainingBestQualities.Count; i++) {
          if (IsNonDominated(trainingBestQualities[i], nonDominatedSolutions.Select(x => x.Qualities), maximization)) {
            nonDominatedSolutions.Add(new { Solution = TrainingBestSolutions[i], Qualities = TrainingBestSolutionQualities[i].ToArray() });
          }
        }

        //assumes the the first objective is always the accuracy
        var sortedNonDominatedSolutions = maximization[0]
          ? nonDominatedSolutions.OrderByDescending(x => x.Qualities[0])
          : nonDominatedSolutions.OrderBy(x => x.Qualities[0]);
        var trainingBestSolution = sortedNonDominatedSolutions.Select(s => s.Solution).First();
        results[TrainingBestSolutionParameterName].Value = trainingBestSolution;
        TrainingBestSolutions = new ItemList<T>(sortedNonDominatedSolutions.Select(x => x.Solution));
        results[TrainingBestSolutionsParameter.Name].Value = TrainingBestSolutions;
        TrainingBestSolutionQualities = new ItemList<DoubleArray>(sortedNonDominatedSolutions.Select(x => new DoubleArray(x.Qualities)));
        results[TrainingBestSolutionQualitiesParameter.Name].Value = TrainingBestSolutionQualities;
      }
      #endregion
      return base.Apply();
    }

    protected abstract T CreateSolution(ISymbolicExpressionTree bestTree, double[] bestQuality);

    private bool IsNonDominated(double[] point, IEnumerable<double[]> points, bool[] maximization) {
      foreach (var refPoint in points) {
        bool refPointDominatesPoint = IsBetterOrEqual(refPoint, point, maximization);
        if (refPointDominatesPoint) return false;
      }
      return true;
    }

    private bool IsBetterOrEqual(double[] lhs, double[] rhs, bool[] maximization) {
      for (int i = 0; i < lhs.Length; i++) {
        var result = IsBetterOrEqual(lhs[i], rhs[i], maximization[i]);
        if (!result) return false;
      }
      return true;
    }

    private bool IsBetterOrEqual(double lhs, double rhs, bool maximization) {
      if (maximization) return lhs >= rhs;
      else return lhs <= rhs;
    }
  }
}
