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
    #endregion
    #region private members
    private DataReducer prunedSubtreesReducer;
    private DataReducer prunedTreesReducer;
    private DataTableValuesCollector valuesCollector;
    private ResultsCollector resultsCollector;
    private EmptyOperator emptyOp;
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
    private ILookupParameter<IDataAnalysisProblemData> ProblemDataParameter {
      get { return (ILookupParameter<IDataAnalysisProblemData>)Parameters[ProblemDataParameterName]; }
    }
    public IValueParameter<IntValue> UpdateIntervalParameter {
      get { return (IValueParameter<IntValue>)Parameters[UpdateIntervalParameterName]; }
    }
    public IValueParameter<IntValue> UpdateCounterParameter {
      get { return (IValueParameter<IntValue>)Parameters[UpdateCounterParameterName]; }
    }
    public IValueParameter<DoubleRange> PopulationSliceParameter {
      get { return (IValueParameter<DoubleRange>)Parameters[PopulationSliceParameterName]; }
    }
    public IValueParameter<DoubleValue> PruningProbabilityParameter {
      get { return (IValueParameter<DoubleValue>)Parameters[PruningProbabilityParameterName]; }
    }
    #endregion
    #region properties
    protected SymbolicDataAnalysisExpressionPruningOperator PruningOperator { get { return PruningOperatorParameter.Value; } }
    protected IDataAnalysisProblemData ProblemData { get { return ProblemDataParameter.ActualValue; } }
    protected IntValue UpdateInterval { get { return UpdateIntervalParameter.Value; } }
    protected IntValue UpdateCounter { get { return UpdateCounterParameter.Value; } }
    protected DoubleRange PopulationSlice { get { return PopulationSliceParameter.Value; } }
    protected DoubleValue PruningProbability { get { return PruningProbabilityParameter.Value; } }
    protected IRandom Random { get { return RandomParameter.ActualValue; } }
    protected DoubleValue NodeImpactThreshold { get { return NodeImpactThresholdParameter.Value; } }
    protected BoolValue PruneOnlyZeroImpactNodes { get { return PruneOnlyZeroImpactNodesParameter.Value; } }
    #endregion
    #region IStatefulItem members
    public override void InitializeState() {
      base.InitializeState();
      UpdateCounter.Value = 0;
    }
    public override void ClearState() {
      base.ClearState();
      UpdateCounter.Value = 0;
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
    protected SymbolicDataAnalysisSingleObjectivePruningAnalyzer() {
      #region add parameters
      Parameters.Add(new ValueParameter<DoubleRange>(PopulationSliceParameterName, new DoubleRange(0.75, 1)));
      Parameters.Add(new ValueParameter<DoubleValue>(PruningProbabilityParameterName, new DoubleValue(0.5)));
      Parameters.Add(new ValueParameter<IntValue>(UpdateIntervalParameterName, "The interval in which the tree length analysis should be applied.", new IntValue(1)));
      Parameters.Add(new ValueParameter<IntValue>(UpdateCounterParameterName, "The value which counts how many times the operator was called", new IntValue(0)));
      Parameters.Add(new LookupParameter<IRandom>(RandomParameterName));
      Parameters.Add(new LookupParameter<IDataAnalysisProblemData>(ProblemDataParameterName));
      Parameters.Add(new FixedValueParameter<DoubleValue>(NodeImpactThresholdParameterName, new DoubleValue(0.0)));
      Parameters.Add(new FixedValueParameter<BoolValue>(PruneOnlyZeroImpactNodesParameterName, new BoolValue(false)));
      #endregion
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

      emptyOp = new EmptyOperator();
    }

    // 
    /// <summary>
    /// Computes the closed interval bounding the portion of the population that is to be pruned. 
    /// </summary>
    /// <returns>Returns an int range [start, end]</returns>
    private IntRange GetSliceBounds() {
      var count = ExecutionContext.Scope.SubScopes.Count;
      var start = (int)Math.Round(PopulationSlice.Start * count);
      var end = (int)Math.Round(PopulationSlice.End * count);
      if (end > count) end = count;

      if (PopulationSlice.Start > 1 || PopulationSlice.End > 1) throw new ArgumentOutOfRangeException("The slice bounds should be expressed as unit percentages.");
      if (start >= end) throw new ArgumentOutOfRangeException("Invalid PopulationSlice bounds.");
      return new IntRange(start, end);
    }

    private IOperation CreatePruningOperation() {
      var oc = new OperationCollection { Parallel = true };
      var range = GetSliceBounds();
      var qualities = Quality.Select(x => x.Value).ToArray();
      var indices = Enumerable.Range(0, qualities.Length).ToArray();
      Array.Sort(qualities, indices);
      if (!Maximization.Value) Array.Reverse(indices);

      var subscopes = ExecutionContext.Scope.SubScopes;

      for (int i = 0; i < subscopes.Count; ++i) {
        IOperator op;
        if (range.Start <= i && i < range.End && Random.NextDouble() <= PruningProbability.Value)
          op = PruningOperator;
        else op = emptyOp;
        var index = indices[i];
        var subscope = subscopes[index];
        oc.Add(ExecutionContext.CreateChildOperation(op, subscope));
      }
      return oc;
    }

    public override IOperation Apply() {
      UpdateCounter.Value++;
      if (UpdateCounter.Value != UpdateInterval.Value) return base.Apply();
      UpdateCounter.Value = 0;

      if (prunedSubtreesReducer == null || prunedTreesReducer == null || valuesCollector == null || resultsCollector == null) { InitializeOperators(); }

      var prune = CreatePruningOperation();
      var reducePrunedSubtrees = ExecutionContext.CreateChildOperation(prunedSubtreesReducer);
      var reducePrunedTrees = ExecutionContext.CreateChildOperation(prunedTreesReducer);
      var collectValues = ExecutionContext.CreateChildOperation(valuesCollector);
      var collectResults = ExecutionContext.CreateChildOperation(resultsCollector);

      return new OperationCollection { prune, reducePrunedSubtrees, reducePrunedTrees, collectValues, collectResults, base.Apply() };
    }
  }
}
