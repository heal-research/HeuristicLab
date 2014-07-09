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
using System.Linq;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableClass]
  [Item("SymbolicDataAnalysisSingleObjectivePruningAnalyzer", "An analyzer that prunes introns from trees in single objective symbolic data analysis problems.")]
  public abstract class SymbolicDataAnalysisSingleObjectivePruningAnalyzer : SymbolicDataAnalysisSingleObjectiveAnalyzer {
    #region parameter names
    private const string ProblemDataParameterName = "ProblemData";
    private const string UpdateIntervalParameterName = "UpdateInverval";
    private const string UpdateCounterParameterName = "UpdateCounter";
    private const string PopulationSliceParameterName = "PopulationSlice";
    private const string PruningProbabilityParameterName = "PruningProbability";
    private const string TotalNumberOfPrunedSubtreesParameterName = "Number of pruned subtrees";
    private const string TotalNumberOfPrunedTreesParameterName = "Number of pruned trees";
    private const string RandomParameterName = "Random";
    private const string PruneOnlyZeroImpactNodesParameterName = "PruneOnlyZeroImpactNodes";
    private const string NodeImpactThresholdParameterName = "ImpactThreshold";
    private const string PruningOperatorParameterName = "PruningOperator";
    private const string ResultsParameterName = "Results";
    private const string PopulationSizeParameterName = "PopulationSize";
    #endregion

    #region private members
    private DataReducer prunedSubtreesReducer;
    private DataReducer prunedTreesReducer;
    private DataTableValuesCollector valuesCollector;
    private ResultsCollector resultsCollector;
    #endregion

    #region parameter properties
    public IValueParameter<SymbolicDataAnalysisExpressionPruningOperator> PruningOperatorParameter {
      get { return (IValueParameter<SymbolicDataAnalysisExpressionPruningOperator>)Parameters[PruningOperatorParameterName]; }
    }
    public IFixedValueParameter<BoolValue> PruneOnlyZeroImpactNodesParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters[PruneOnlyZeroImpactNodesParameterName]; }
    }
    public IFixedValueParameter<DoubleValue> NodeImpactThresholdParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters[NodeImpactThresholdParameterName]; }
    }
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters[RandomParameterName]; }
    }
    public IFixedValueParameter<IntValue> UpdateIntervalParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[UpdateIntervalParameterName]; }
    }
    public IFixedValueParameter<IntValue> UpdateCounterParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[UpdateCounterParameterName]; }
    }
    public IFixedValueParameter<DoubleRange> PopulationSliceParameter {
      get { return (IFixedValueParameter<DoubleRange>)Parameters[PopulationSliceParameterName]; }
    }
    public IFixedValueParameter<DoubleValue> PruningProbabilityParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters[PruningProbabilityParameterName]; }
    }
    public ILookupParameter<IntValue> PopulationSizeParameter {
      get { return (ILookupParameter<IntValue>)Parameters[PopulationSizeParameterName]; }
    }
    #endregion

    #region properties
    protected SymbolicDataAnalysisExpressionPruningOperator PruningOperator { get { return PruningOperatorParameter.Value; } }
    protected int UpdateInterval { get { return UpdateIntervalParameter.Value.Value; } }

    protected int UpdateCounter {
      get { return UpdateCounterParameter.Value.Value; }
      set { UpdateCounterParameter.Value.Value = value; }
    }

    protected double PopulationSliceStart {
      get { return PopulationSliceParameter.Value.Start; }
      set { PopulationSliceParameter.Value.Start = value; }
    }

    protected double PopulationSliceEnd {
      get { return PopulationSliceParameter.Value.End; }
      set { PopulationSliceParameter.Value.End = value; }
    }

    protected double PruningProbability {
      get { return PruningProbabilityParameter.Value.Value; }
      set { PruningProbabilityParameter.Value.Value = value; }
    }

    protected bool PruneOnlyZeroImpactNodes {
      get { return PruneOnlyZeroImpactNodesParameter.Value.Value; }
      set { PruneOnlyZeroImpactNodesParameter.Value.Value = value; }
    }
    protected double NodeImpactThreshold {
      get { return NodeImpactThresholdParameter.Value.Value; }
      set { NodeImpactThresholdParameter.Value.Value = value; }
    }
    #endregion

    #region IStatefulItem members
    public override void InitializeState() {
      base.InitializeState();
      UpdateCounter = 0;
    }
    public override void ClearState() {
      base.ClearState();
      UpdateCounter = 0;
    }
    #endregion

    [StorableConstructor]
    protected SymbolicDataAnalysisSingleObjectivePruningAnalyzer(bool deserializing) : base(deserializing) { }

    protected SymbolicDataAnalysisSingleObjectivePruningAnalyzer(SymbolicDataAnalysisSingleObjectivePruningAnalyzer original, Cloner cloner)
      : base(original, cloner) {
      if (original.prunedSubtreesReducer != null)
        this.prunedSubtreesReducer = (DataReducer)original.prunedSubtreesReducer.Clone();
      if (original.prunedTreesReducer != null)
        this.prunedTreesReducer = (DataReducer)original.prunedTreesReducer.Clone();
      if (original.valuesCollector != null)
        this.valuesCollector = (DataTableValuesCollector)original.valuesCollector.Clone();
      if (original.resultsCollector != null)
        this.resultsCollector = (ResultsCollector)original.resultsCollector.Clone();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (!Parameters.ContainsKey(PopulationSizeParameterName)) {
        Parameters.Add(new LookupParameter<IntValue>(PopulationSizeParameterName, "The population of individuals."));
      }
      if (Parameters.ContainsKey(UpdateCounterParameterName)) {
        var fixedValueParameter = Parameters[UpdateCounterParameterName] as FixedValueParameter<IntValue>;
        if (fixedValueParameter == null) {
          var valueParameter = (ValueParameter<IntValue>)Parameters[UpdateCounterParameterName];
          Parameters.Remove(UpdateCounterParameterName);
          Parameters.Add(new FixedValueParameter<IntValue>(UpdateCounterParameterName, valueParameter.Value));
        }
      }
      if (Parameters.ContainsKey(UpdateIntervalParameterName)) {
        var fixedValueParameter = Parameters[UpdateIntervalParameterName] as FixedValueParameter<IntValue>;
        if (fixedValueParameter == null) {
          var valueParameter = (ValueParameter<IntValue>)Parameters[UpdateIntervalParameterName];
          Parameters.Remove(UpdateIntervalParameterName);
          Parameters.Add(new FixedValueParameter<IntValue>(UpdateIntervalParameterName, valueParameter.Value));
        }
      }
      if (Parameters.ContainsKey(PopulationSliceParameterName)) {
        var fixedValueParameter = Parameters[PopulationSliceParameterName] as FixedValueParameter<DoubleRange>;
        if (fixedValueParameter == null) {
          var valueParameter = (ValueParameter<DoubleRange>)Parameters[PopulationSliceParameterName];
          Parameters.Remove(PopulationSliceParameterName);
          Parameters.Add(new FixedValueParameter<DoubleRange>(PopulationSliceParameterName, valueParameter.Value));
        }
      }
      if (Parameters.ContainsKey(PruningProbabilityParameterName)) {
        var fixedValueParameter = Parameters[PruningProbabilityParameterName] as FixedValueParameter<DoubleValue>;
        if (fixedValueParameter == null) {
          var valueParameter = (ValueParameter<DoubleValue>)Parameters[PruningProbabilityParameterName];
          Parameters.Remove(PruningProbabilityParameterName);
          Parameters.Add(new FixedValueParameter<DoubleValue>(PruningProbabilityParameterName, valueParameter.Value));
        }
      }
    }

    protected SymbolicDataAnalysisSingleObjectivePruningAnalyzer() {
      #region add parameters
      Parameters.Add(new FixedValueParameter<DoubleRange>(PopulationSliceParameterName, "The slice of the population where pruning should be applied.", new DoubleRange(0.75, 1)));
      Parameters.Add(new FixedValueParameter<DoubleValue>(PruningProbabilityParameterName, "The probability for pruning an individual.", new DoubleValue(0.5)));
      Parameters.Add(new FixedValueParameter<IntValue>(UpdateIntervalParameterName, "The interval in which the tree length analysis should be applied.", new IntValue(1)));
      Parameters.Add(new FixedValueParameter<IntValue>(UpdateCounterParameterName, "The value which counts how many times the operator was called", new IntValue(0)));
      Parameters.Add(new LookupParameter<IRandom>(RandomParameterName, "The random number generator."));
      Parameters.Add(new LookupParameter<IDataAnalysisProblemData>(ProblemDataParameterName, "The problem data."));
      Parameters.Add(new FixedValueParameter<DoubleValue>(NodeImpactThresholdParameterName, "The impact threshold  below which an individual should be pruned.", new DoubleValue(0.0)));
      Parameters.Add(new FixedValueParameter<BoolValue>(PruneOnlyZeroImpactNodesParameterName, "Switch to determine of only zero impact individuals should be pruned.", new BoolValue(false)));
      Parameters.Add(new LookupParameter<IntValue>(PopulationSizeParameterName, "The population of individuals."));
      #endregion
    }

    // 
    /// <summary>
    /// Computes the closed interval bounding the portion of the population that is to be pruned. 
    /// </summary>
    /// <returns>Returns an int range [start, end]</returns>
    private IntRange GetSliceBounds() {
      if (PopulationSliceStart < 0 || PopulationSliceEnd < 0) throw new ArgumentOutOfRangeException("The slice bounds cannot be negative.");
      if (PopulationSliceStart > 1 || PopulationSliceEnd > 1) throw new ArgumentOutOfRangeException("The slice bounds should be expressed as unit percentages.");
      var count = PopulationSizeParameter.ActualValue.Value;
      var start = (int)Math.Round(PopulationSliceStart * count);
      var end = (int)Math.Round(PopulationSliceEnd * count);
      if (end > count) end = count;

      if (PopulationSlice.Start > 1 || PopulationSlice.End > 1) throw new ArgumentOutOfRangeException("The slice bounds should be expressed as unit percentages.");
      if (start >= end) throw new ArgumentOutOfRangeException("Invalid PopulationSlice bounds.");
      return new IntRange(start, end);
    }

    private IOperation CreatePruningOperation() {
      var operations = new OperationCollection { Parallel = true };
      var range = GetSliceBounds();
      var qualities = Quality.Select(x => x.Value).ToArray();
      var indices = Enumerable.Range(0, qualities.Length).ToArray();
      Array.Sort(qualities, indices);
      if (!Maximization.Value) Array.Reverse(indices);

      var subscopes = ExecutionContext.Scope.SubScopes;
      var random = RandomParameter.ActualValue;

      var empty = new EmptyOperator();

      for (int i = 0; i < subscopes.Count; ++i) {
        IOperator @operator;
        if (range.Start <= i && i < range.End && random.NextDouble() <= PruningProbability)
          @operator = PruningOperator;
        else @operator = empty;
        var index = indices[i];
        var subscope = subscopes[index];
        operations.Add(ExecutionContext.CreateChildOperation(@operator, subscope));
      }
      return operations;
    }

    public override IOperation Apply() {
      UpdateCounter++;
      if (UpdateCounter != UpdateInterval) return base.Apply();
      UpdateCounter = 0;

      if (prunedSubtreesReducer == null || prunedTreesReducer == null || valuesCollector == null || resultsCollector == null) { InitializeOperators(); }

      var prune = CreatePruningOperation();
      var reducePrunedSubtrees = ExecutionContext.CreateChildOperation(prunedSubtreesReducer);
      var reducePrunedTrees = ExecutionContext.CreateChildOperation(prunedTreesReducer);
      var collectValues = ExecutionContext.CreateChildOperation(valuesCollector);
      var collectResults = ExecutionContext.CreateChildOperation(resultsCollector);

      return new OperationCollection { prune, reducePrunedSubtrees, reducePrunedTrees, collectValues, collectResults, base.Apply() };
    }

    private void InitializeOperators() {
      prunedSubtreesReducer = new DataReducer();
      prunedSubtreesReducer.ParameterToReduce.ActualName = PruningOperator.PrunedSubtreesParameter.ActualName;
      prunedSubtreesReducer.ReductionOperation.Value = new ReductionOperation(ReductionOperations.Sum); // sum all the pruned subtrees parameter values
      prunedSubtreesReducer.TargetOperation.Value = new ReductionOperation(ReductionOperations.Assign); // asign the sum to the target parameter
      prunedSubtreesReducer.TargetParameter.ActualName = TotalNumberOfPrunedSubtreesParameterName;

      prunedTreesReducer = new DataReducer();
      prunedTreesReducer.ParameterToReduce.ActualName = PruningOperator.PrunedTreesParameter.ActualName;
      prunedTreesReducer.ReductionOperation.Value = new ReductionOperation(ReductionOperations.Sum);
      prunedTreesReducer.TargetOperation.Value = new ReductionOperation(ReductionOperations.Assign);
      prunedTreesReducer.TargetParameter.ActualName = TotalNumberOfPrunedTreesParameterName;

      valuesCollector = new DataTableValuesCollector();
      valuesCollector.CollectedValues.Add(new LookupParameter<IntValue>(TotalNumberOfPrunedSubtreesParameterName));
      valuesCollector.CollectedValues.Add(new LookupParameter<IntValue>(TotalNumberOfPrunedTreesParameterName));
      valuesCollector.DataTableParameter.ActualName = "Population pruning";

      resultsCollector = new ResultsCollector();
      resultsCollector.CollectedValues.Add(new LookupParameter<DataTable>("Population pruning"));
      resultsCollector.ResultsParameter.ActualName = ResultsParameterName;
    }
  }
}
