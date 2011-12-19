using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Classification {
  [Item("Pearson R² & Tree size Evaluator", "Calculates the Pearson R² and the tree size of a symbolic classification solution.")]
  [StorableClass]
  public class SymbolicClassificationMultiObjectivePearsonRSquaredTreeSizeEvaluator : SymbolicClassificationMultiObjectiveEvaluator {
    [StorableConstructor]
    protected SymbolicClassificationMultiObjectivePearsonRSquaredTreeSizeEvaluator(bool deserializing) : base(deserializing) { }
    protected SymbolicClassificationMultiObjectivePearsonRSquaredTreeSizeEvaluator(SymbolicClassificationMultiObjectivePearsonRSquaredTreeSizeEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicClassificationMultiObjectivePearsonRSquaredTreeSizeEvaluator(this, cloner);
    }

    public SymbolicClassificationMultiObjectivePearsonRSquaredTreeSizeEvaluator() : base() { }

    public override IEnumerable<bool> Maximization { get { return new bool[2] { true, false }; } }

    public override IOperation Apply() {
      IEnumerable<int> rows = GenerateRowsToEvaluate();
      var solution = SymbolicExpressionTreeParameter.ActualValue;
      double[] qualities = Calculate(SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, solution, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper, ProblemDataParameter.ActualValue, rows);
      QualitiesParameter.ActualValue = new DoubleArray(qualities);
      return base.Apply();
    }

    public static double[] Calculate(ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, ISymbolicExpressionTree solution, double lowerEstimationLimit, double upperEstimationLimit, IClassificationProblemData problemData, IEnumerable<int> rows) {
      IEnumerable<double> estimatedValues = interpreter.GetSymbolicExpressionTreeValues(solution, problemData.Dataset, rows);
      IEnumerable<double> originalValues = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, rows);
      OnlineCalculatorError errorState;
      double r2 = OnlinePearsonsRSquaredCalculator.Calculate(estimatedValues, originalValues, out errorState);
      if (errorState != OnlineCalculatorError.None) r2 = 0.0;
      return new double[] { r2, solution.Length };

    }

    public override double[] Evaluate(IExecutionContext context, ISymbolicExpressionTree tree, IClassificationProblemData problemData, IEnumerable<int> rows) {
      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = context;
      EstimationLimitsParameter.ExecutionContext = context;

      double[] quality = Calculate(SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, tree, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper, problemData, rows);

      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = null;
      EstimationLimitsParameter.ExecutionContext = null;

      return quality;
    }
  }
}
