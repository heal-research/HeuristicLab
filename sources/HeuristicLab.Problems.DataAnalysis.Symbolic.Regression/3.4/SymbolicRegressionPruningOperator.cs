using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  [StorableClass]
  [Item("SymbolicRegressionPruningOperator", "An operator which prunes symbolic regression trees.")]
  public class SymbolicRegressionPruningOperator : SymbolicDataAnalysisExpressionPruningOperator {
    private const string ImpactValuesCalculatorParameterName = "ImpactValuesCalculator";
    private const string ImpactValuesCalculatorParameterDescription = "The impact values calculator to be used for figuring out the node impacts.";

    private const string EvaluatorParameterName = "Evaluator";

    public ILookupParameter<ISymbolicRegressionSingleObjectiveEvaluator> EvaluatorParameter {
      get { return (ILookupParameter<ISymbolicRegressionSingleObjectiveEvaluator>)Parameters[EvaluatorParameterName]; }
    }

    protected SymbolicRegressionPruningOperator(SymbolicRegressionPruningOperator original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicRegressionPruningOperator(this, cloner);
    }

    [StorableConstructor]
    protected SymbolicRegressionPruningOperator(bool deserializing) : base(deserializing) { }

    public SymbolicRegressionPruningOperator() {
      var impactValuesCalculator = new SymbolicRegressionSolutionImpactValuesCalculator();
      Parameters.Add(new ValueParameter<ISymbolicDataAnalysisSolutionImpactValuesCalculator>(ImpactValuesCalculatorParameterName, ImpactValuesCalculatorParameterDescription, impactValuesCalculator));
      Parameters.Add(new LookupParameter<ISymbolicRegressionSingleObjectiveEvaluator>(EvaluatorParameterName));
    }

    protected override ISymbolicDataAnalysisModel CreateModel() {
      return new SymbolicRegressionModel(SymbolicExpressionTree, Interpreter, EstimationLimits.Lower, EstimationLimits.Upper);
    }

    protected override double Evaluate(IDataAnalysisModel model) {
      var regressionModel = (IRegressionModel)model;
      var regressionProblemData = (IRegressionProblemData)ProblemData;
      var trainingIndices = ProblemData.TrainingIndices.ToList();
      var estimatedValues = regressionModel.GetEstimatedValues(ProblemData.Dataset, trainingIndices); // also bounds the values
      var targetValues = ProblemData.Dataset.GetDoubleValues(regressionProblemData.TargetVariable, trainingIndices);
      OnlineCalculatorError errorState;
      var quality = OnlinePearsonsRSquaredCalculator.Calculate(targetValues, estimatedValues, out errorState);
      if (errorState != OnlineCalculatorError.None) return double.NaN;
      return quality;
    }
  }
}
