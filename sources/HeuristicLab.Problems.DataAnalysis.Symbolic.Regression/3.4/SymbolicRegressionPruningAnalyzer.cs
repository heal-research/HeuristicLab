using System;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  [Item("SymbolicRegressionPruningAnalyzer", "An analyzer that prunes introns from the population.")]
  [StorableClass]
  public sealed class SymbolicRegressionPruningAnalyzer : SymbolicDataAnalysisSingleObjectivePruningAnalyzer {
    private SymbolicRegressionPruningAnalyzer(SymbolicRegressionPruningAnalyzer original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicRegressionPruningAnalyzer(this, cloner);
    }

    [StorableConstructor]
    private SymbolicRegressionPruningAnalyzer(bool deserializing) : base(deserializing) { }

    public SymbolicRegressionPruningAnalyzer() {
      impactValuesCalculator = new SymbolicRegressionSolutionImpactValuesCalculator();
    }

    protected override ISymbolicDataAnalysisModel CreateModel(ISymbolicExpressionTree tree,
      ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, double lowerEstimationLimit = Double.MinValue,
      double upperEstimationLimit = Double.MaxValue) {
      return new SymbolicRegressionModel(tree, interpreter, lowerEstimationLimit, upperEstimationLimit);
    }
  }
}
