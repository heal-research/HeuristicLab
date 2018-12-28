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
  /// An operator that analyzes the training best symbolic data analysis solution for single objective symbolic data analysis problems.
  /// </summary>
  [Item("SymbolicDataAnalysisSingleObjectiveTrainingBestSolutionAnalyzer", "An operator that analyzes the training best symbolic data analysis solution for single objective symbolic data analysis problems.")]
  [StorableType("DD82C026-CF68-40D7-A898-77EA6A872DE9")]
  public abstract class SymbolicDataAnalysisSingleObjectiveTrainingBestSolutionAnalyzer<T> : SymbolicDataAnalysisSingleObjectiveAnalyzer, IIterationBasedOperator where T : class, ISymbolicDataAnalysisSolution {
    private const string TrainingBestSolutionParameterName = "Best training solution";
    private const string TrainingBestSolutionQualityParameterName = "Best training solution quality";
    private const string TrainingBestSolutionGenerationParameterName = "Best training solution generation";
    private const string TrainingBestSolutionsHistoryParameterName = "Best training solutions history";
    private const string UpdateAlwaysParameterName = "Always update best solution";
    private const string IterationsParameterName = "Iterations";
    private const string MaximumIterationsParameterName = "Maximum Iterations";
    private const string StoreHistoryParameterName = "Store History";

    #region parameter properties
    public ILookupParameter<T> TrainingBestSolutionParameter {
      get { return (ILookupParameter<T>)Parameters[TrainingBestSolutionParameterName]; }
    }
    public ILookupParameter<DoubleValue> TrainingBestSolutionQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[TrainingBestSolutionQualityParameterName]; }
    }
    public ILookupParameter<IntValue> TrainingBestSolutionGenerationParameter {
      get { return (ILookupParameter<IntValue>)Parameters[TrainingBestSolutionGenerationParameterName]; }
    }
    public ILookupParameter<ItemList<T>> TrainingBestSolutionsHistoryParameter {
      get { return (ILookupParameter<ItemList<T>>)Parameters[TrainingBestSolutionsHistoryParameterName]; }
    }
    public IFixedValueParameter<BoolValue> UpdateAlwaysParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters[UpdateAlwaysParameterName]; }
    }
    public ILookupParameter<IntValue> IterationsParameter {
      get { return (ILookupParameter<IntValue>)Parameters[IterationsParameterName]; }
    }
    public IValueLookupParameter<IntValue> MaximumIterationsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[MaximumIterationsParameterName]; }
    }

    public IFixedValueParameter<BoolValue> StoreHistoryParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters[StoreHistoryParameterName]; }
    }
    #endregion
    #region properties
    public T TrainingBestSolution {
      get { return TrainingBestSolutionParameter.ActualValue; }
      set { TrainingBestSolutionParameter.ActualValue = value; }
    }
    public DoubleValue TrainingBestSolutionQuality {
      get { return TrainingBestSolutionQualityParameter.ActualValue; }
      set { TrainingBestSolutionQualityParameter.ActualValue = value; }
    }
    public bool UpdateAlways {
      get { return UpdateAlwaysParameter.Value.Value; }
      set { UpdateAlwaysParameter.Value.Value = value; }
    }

    public bool StoreHistory {
      get { return StoreHistoryParameter.Value.Value; }
      set { StoreHistoryParameter.Value.Value = value; }
    }
    #endregion


    [StorableConstructor]
    protected SymbolicDataAnalysisSingleObjectiveTrainingBestSolutionAnalyzer(StorableConstructorFlag _) : base(_) { }
    protected SymbolicDataAnalysisSingleObjectiveTrainingBestSolutionAnalyzer(SymbolicDataAnalysisSingleObjectiveTrainingBestSolutionAnalyzer<T> original, Cloner cloner) : base(original, cloner) { }
    public SymbolicDataAnalysisSingleObjectiveTrainingBestSolutionAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<T>(TrainingBestSolutionParameterName, "The best training symbolic data analyis solution."));
      Parameters.Add(new LookupParameter<DoubleValue>(TrainingBestSolutionQualityParameterName, "The quality of the training best symbolic data analysis solution."));
      Parameters.Add(new LookupParameter<IntValue>(TrainingBestSolutionGenerationParameterName, "The generation in which the best training solution was found."));
      Parameters.Add(new FixedValueParameter<BoolValue>(UpdateAlwaysParameterName, "Determines if the best training solution should always be updated regardless of its quality.", new BoolValue(false)));
      Parameters.Add(new LookupParameter<IntValue>(IterationsParameterName, "The number of performed iterations."));
      Parameters.Add(new ValueLookupParameter<IntValue>(MaximumIterationsParameterName, "The maximum number of performed iterations.") { Hidden = true });
      Parameters.Add(new FixedValueParameter<BoolValue>(StoreHistoryParameterName, "Flag that determines whether all encountered best solutions should be stored as results.", new BoolValue(false)));
      Parameters.Add(new LookupParameter<ItemList<T>>(TrainingBestSolutionsHistoryParameterName, "The history of the best training symbolic data analysis solutions."));
      UpdateAlwaysParameter.Hidden = true;
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (!Parameters.ContainsKey(UpdateAlwaysParameterName)) {
        Parameters.Add(new FixedValueParameter<BoolValue>(UpdateAlwaysParameterName, "Determines if the best training solution should always be updated regardless of its quality.", new BoolValue(false)));
        UpdateAlwaysParameter.Hidden = true;
      }
      if (!Parameters.ContainsKey(TrainingBestSolutionGenerationParameterName))
        Parameters.Add(new LookupParameter<IntValue>(TrainingBestSolutionGenerationParameterName, "The generation in which the best training solution was found."));
      if (!Parameters.ContainsKey(IterationsParameterName))
        Parameters.Add(new LookupParameter<IntValue>(IterationsParameterName, "The number of performed iterations."));
      if (!Parameters.ContainsKey(MaximumIterationsParameterName))
        Parameters.Add(new ValueLookupParameter<IntValue>(MaximumIterationsParameterName, "The maximum number of performed iterations.") { Hidden = true });
      if (!Parameters.ContainsKey(StoreHistoryParameterName))
        Parameters.Add(new FixedValueParameter<BoolValue>(StoreHistoryParameterName, "Flag that determines whether all encountered best solutions should be stored as results.", new BoolValue(false)));
      if (!Parameters.ContainsKey(TrainingBestSolutionsHistoryParameterName))
        Parameters.Add(new LookupParameter<ItemList<T>>(TrainingBestSolutionsHistoryParameterName, "The history of the best training symbolic data analysis solutions."));
    }

    public override IOperation Apply() {
      var results = ResultCollection;
      #region create results
      if (!results.ContainsKey(TrainingBestSolutionParameter.Name))
        results.Add(new Result(TrainingBestSolutionParameter.Name, TrainingBestSolutionParameter.Description, typeof(T)));
      if (!results.ContainsKey(TrainingBestSolutionQualityParameter.Name))
        results.Add(new Result(TrainingBestSolutionQualityParameter.Name, TrainingBestSolutionQualityParameter.Description, typeof(DoubleValue)));
      if (!results.ContainsKey(TrainingBestSolutionGenerationParameter.Name) && IterationsParameter.ActualValue != null)
        results.Add(new Result(TrainingBestSolutionGenerationParameter.Name, TrainingBestSolutionGenerationParameter.Description, typeof(IntValue)));
      if (StoreHistory && !results.ContainsKey(TrainingBestSolutionsHistoryParameter.Name)) {

        results.Add(new Result(TrainingBestSolutionsHistoryParameter.Name, TrainingBestSolutionsHistoryParameter.Description, typeof(ItemList<T>)));
        TrainingBestSolutionsHistoryParameter.ActualValue = new ItemList<T>();
        results[TrainingBestSolutionsHistoryParameter.Name].Value = TrainingBestSolutionsHistoryParameter.ActualValue;
      }
      #endregion

      #region find best tree
      double bestQuality = Maximization.Value ? double.NegativeInfinity : double.PositiveInfinity;
      ISymbolicExpressionTree bestTree = null;
      ISymbolicExpressionTree[] tree = SymbolicExpressionTree.ToArray();
      double[] quality = Quality.Select(x => x.Value).ToArray();
      for (int i = 0; i < tree.Length; i++) {
        if (IsBetter(quality[i], bestQuality, Maximization.Value)) {
          bestQuality = quality[i];
          bestTree = tree[i];
        }
      }
      #endregion

      if (bestTree != null && (UpdateAlways || TrainingBestSolutionQuality == null ||
        IsBetter(bestQuality, TrainingBestSolutionQuality.Value, Maximization.Value))) {
        TrainingBestSolution = CreateSolution(bestTree, bestQuality);
        TrainingBestSolutionQuality = new DoubleValue(bestQuality);
        if (IterationsParameter.ActualValue != null)
          TrainingBestSolutionGenerationParameter.ActualValue = new IntValue(IterationsParameter.ActualValue.Value);

        results[TrainingBestSolutionParameter.Name].Value = TrainingBestSolution;
        results[TrainingBestSolutionQualityParameter.Name].Value = TrainingBestSolutionQuality;
        if (TrainingBestSolutionGenerationParameter.ActualValue != null)
          results[TrainingBestSolutionGenerationParameter.Name].Value = TrainingBestSolutionGenerationParameter.ActualValue;

        if (StoreHistory) {
          TrainingBestSolutionsHistoryParameter.ActualValue.Add(TrainingBestSolution);
        }
      }
      return base.Apply();
    }

    protected abstract T CreateSolution(ISymbolicExpressionTree bestTree, double bestQuality);

    private bool IsBetter(double lhs, double rhs, bool maximization) {
      if (maximization) return lhs > rhs;
      else return lhs < rhs;
    }
  }
}
