using System;
using System.Linq;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableClass]
  [Item("SymbolicDataAnalysisSingleObjectivePruningAnalyzer", "An analyzer that prunes introns from trees in single objective symbolic data analysis problems.")]
  public abstract class SymbolicDataAnalysisSingleObjectivePruningAnalyzer : SymbolicDataAnalysisSingleObjectiveAnalyzer {
    private const string ProblemDataParameterName = "ProblemData";
    private const string InterpreterParameterName = "SymbolicExpressionTreeInterpreter";

    private const string UpdateIntervalParameterName = "UpdateInverval";
    private const string UpdateCounterParameterName = "UpdateCounter";

    private const string PopulationSliceParameterName = "PopulationSlice";
    private const string PruningProbabilityParameterName = "PruningProbability";

    private const string NumberOfPrunedSubtreesParameterName = "PrunedSubtrees";
    private const string NumberOfPrunedTreesParameterName = "PrunedTrees";

    private const string RandomParameterName = "Random";
    private const string EstimationLimitsParameterName = "EstimationLimits";

    private const string PruneOnlyZeroImpactNodesParameterName = "PruneOnlyZeroImpactNodes";
    private const string NodeImpactThresholdParameterName = "ImpactThreshold";

    private const string FitnessCalculationPartitionParameterName = "FitnessCalculationPartition";

    private bool reentry;
    [Storable]
    protected ISymbolicDataAnalysisSolutionImpactValuesCalculator impactValuesCalculator;

    #region parameter properties
    public IFixedValueParameter<BoolValue> PruneOnlyZeroImpactNodesParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters[PruneOnlyZeroImpactNodesParameterName]; }
    }
    public IFixedValueParameter<DoubleValue> NodeImpactThresholdParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters[NodeImpactThresholdParameterName]; }
    }
    public ILookupParameter<DoubleLimit> EstimationLimitsParameter {
      get { return (ILookupParameter<DoubleLimit>)Parameters[EstimationLimitsParameterName]; }
    }
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters[RandomParameterName]; }
    }
    private ILookupParameter<IDataAnalysisProblemData> ProblemDataParameter {
      get { return (ILookupParameter<IDataAnalysisProblemData>)Parameters[ProblemDataParameterName]; }
    }
    public ILookupParameter<IntRange> FitnessCalculationPartitionParameter {
      get { return (ILookupParameter<IntRange>)Parameters[FitnessCalculationPartitionParameterName]; }
    }
    private ILookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter> InterpreterParameter {
      get { return (ILookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>)Parameters[InterpreterParameterName]; }
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
    public IFixedValueParameter<DoubleValue> NumberOfPrunedSubtreesParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters[NumberOfPrunedSubtreesParameterName]; }
    }
    public IFixedValueParameter<DoubleValue> NumberOfPrunedTreesParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters[NumberOfPrunedTreesParameterName]; }
    }
    #endregion
    #region properties
    protected IDataAnalysisProblemData ProblemData { get { return ProblemDataParameter.ActualValue; } }
    protected IntRange FitnessCalculationPartition { get { return FitnessCalculationPartitionParameter.ActualValue; } }
    protected ISymbolicDataAnalysisExpressionTreeInterpreter Interpreter { get { return InterpreterParameter.ActualValue; } }
    protected IntValue UpdateInterval { get { return UpdateIntervalParameter.Value; } }
    protected IntValue UpdateCounter { get { return UpdateCounterParameter.Value; } }
    protected DoubleRange PopulationSlice { get { return PopulationSliceParameter.Value; } }
    protected DoubleValue PruningProbability { get { return PruningProbabilityParameter.Value; } }
    protected DoubleValue PrunedSubtrees { get { return NumberOfPrunedSubtreesParameter.Value; } }
    protected DoubleValue PrunedTrees { get { return NumberOfPrunedTreesParameter.Value; } }
    protected DoubleLimit EstimationLimits { get { return EstimationLimitsParameter.ActualValue; } }
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
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (!Parameters.ContainsKey(FitnessCalculationPartitionParameterName))
        Parameters.Add(new LookupParameter<IntRange>(FitnessCalculationPartitionParameterName, ""));
    }
    protected SymbolicDataAnalysisSingleObjectivePruningAnalyzer(SymbolicDataAnalysisSingleObjectivePruningAnalyzer original, Cloner cloner)
      : base(original, cloner) {
      impactValuesCalculator = original.impactValuesCalculator;
    }
    protected SymbolicDataAnalysisSingleObjectivePruningAnalyzer() {
      Parameters.Add(new ValueParameter<DoubleRange>(PopulationSliceParameterName, new DoubleRange(0.75, 1)));
      Parameters.Add(new ValueParameter<DoubleValue>(PruningProbabilityParameterName, new DoubleValue(0.5)));
      // analyzer parameters
      Parameters.Add(new ValueParameter<IntValue>(UpdateIntervalParameterName, "The interval in which the tree length analysis should be applied.", new IntValue(1)));
      Parameters.Add(new ValueParameter<IntValue>(UpdateCounterParameterName, "The value which counts how many times the operator was called", new IntValue(0)));
      Parameters.Add(new LookupParameter<IRandom>(RandomParameterName));
      Parameters.Add(new LookupParameter<IDataAnalysisProblemData>(ProblemDataParameterName));
      Parameters.Add(new LookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>(InterpreterParameterName));

      Parameters.Add(new FixedValueParameter<DoubleValue>(NumberOfPrunedSubtreesParameterName, new DoubleValue(0)));
      Parameters.Add(new FixedValueParameter<DoubleValue>(NumberOfPrunedTreesParameterName, new DoubleValue(0)));
      Parameters.Add(new LookupParameter<DoubleLimit>(EstimationLimitsParameterName));
      Parameters.Add(new FixedValueParameter<DoubleValue>(NodeImpactThresholdParameterName, new DoubleValue(0.0)));
      Parameters.Add(new FixedValueParameter<BoolValue>(PruneOnlyZeroImpactNodesParameterName, new BoolValue(false)));
      Parameters.Add(new LookupParameter<IntRange>(FitnessCalculationPartitionParameterName, ""));
    }

    public override IOperation Apply() {
      if (reentry) {
        UpdateCounter.Value++;

        if (UpdateCounter.Value != UpdateInterval.Value) return base.Apply();
        UpdateCounter.Value = 0;

        var trees = SymbolicExpressionTreeParameter.ActualValue.ToList();
        var qualities = QualityParameter.ActualValue.ToList();

        var population = trees.Zip(qualities, (tree, quality) => new { Tree = tree, Quality = quality }).ToList();
        Func<double, double, int> compare = (a, b) => Maximization.Value ? a.CompareTo(b) : b.CompareTo(a);
        population.Sort((a, b) => compare(a.Quality.Value, b.Quality.Value));

        var start = (int)Math.Round(PopulationSlice.Start * trees.Count);
        var end = (int)Math.Round(PopulationSlice.End * trees.Count);

        if (end == population.Count) end--;

        if (start >= end || end >= population.Count) throw new Exception("Invalid PopulationSlice bounds.");

        PrunedSubtrees.Value = 0;
        PrunedTrees.Value = 0;

        reentry = false;

        var operations = new OperationCollection { Parallel = true };
        foreach (var p in population.Skip(start).Take(end)) {
          if (Random.NextDouble() > PruningProbability.Value) continue;
          var op = new SymbolicDataAnalysisExpressionPruningOperator {
            Model = CreateModel(p.Tree, Interpreter, EstimationLimits.Lower, EstimationLimits.Upper),
            ImpactsCalculator = impactValuesCalculator,
            ProblemData = ProblemData,
            Random = Random,
            PruneOnlyZeroImpactNodes = PruneOnlyZeroImpactNodes.Value,
            NodeImpactThreshold = NodeImpactThreshold.Value,
            FitnessCalculationPartition = FitnessCalculationPartition
          };
          operations.Add(ExecutionContext.CreateChildOperation(op, ExecutionContext.Scope));
        }
        return new OperationCollection { operations, ExecutionContext.CreateOperation(this) };
      }

      DataTable table;

      if (ResultCollection.ContainsKey("Population Pruning")) {
        table = (DataTable)ResultCollection["Population Pruning"].Value;
      } else {
        table = new DataTable("Population Pruning");
        table.Rows.Add(new DataRow("Pruned Trees") { VisualProperties = { StartIndexZero = true } });
        table.Rows.Add(new DataRow("Pruned Subtrees") { VisualProperties = { StartIndexZero = true } });
        ResultCollection.Add(new Result("Population Pruning", table));
      }

      table.Rows["Pruned Trees"].Values.Add(PrunedTrees.Value);
      table.Rows["Pruned Subtrees"].Values.Add(PrunedSubtrees.Value);

      reentry = true;

      return base.Apply();
    }

    protected abstract ISymbolicDataAnalysisModel CreateModel(ISymbolicExpressionTree tree, ISymbolicDataAnalysisExpressionTreeInterpreter interpreter,
      double lowerEstimationLimit = double.MinValue, double upperEstimationLimit = double.MaxValue);
  }
}
