using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Classification {
  [StorableClass]
  [Item("SymbolicClassificationPruningOperator", "An operator which prunes symbolic classificaton trees.")]
  public class SymbolicClassificationPruningOperator : SymbolicDataAnalysisExpressionPruningOperator {
    private const string ImpactValuesCalculatorParameterName = "ImpactValuesCalculator";
    private const string ModelCreatorParameterName = "ModelCreator";
    private const string ApplyLinearScalingParmameterName = "ApplyLinearScaling";

    #region parameter properties
    public ILookupParameter<ISymbolicClassificationModelCreator> ModelCreatorParameter {
      get { return (ILookupParameter<ISymbolicClassificationModelCreator>)Parameters[ModelCreatorParameterName]; }
    }

    public ILookupParameter<BoolValue> ApplyLinearScalingParameter {
      get { return (ILookupParameter<BoolValue>)Parameters[ApplyLinearScalingParmameterName]; }
    }
    #endregion
    #region properties
    private ISymbolicClassificationModelCreator ModelCreator { get { return ModelCreatorParameter.ActualValue; } }
    private BoolValue ApplyLinearScaling { get { return ApplyLinearScalingParameter.ActualValue; } }
    #endregion

    protected SymbolicClassificationPruningOperator(SymbolicClassificationPruningOperator original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicClassificationPruningOperator(this, cloner);
    }

    [StorableConstructor]
    protected SymbolicClassificationPruningOperator(bool deserializing) : base(deserializing) { }

    public SymbolicClassificationPruningOperator() {
      Parameters.Add(new ValueParameter<ISymbolicDataAnalysisSolutionImpactValuesCalculator>(ImpactValuesCalculatorParameterName, new SymbolicClassificationSolutionImpactValuesCalculator()));
      Parameters.Add(new LookupParameter<ISymbolicClassificationModelCreator>(ModelCreatorParameterName));
    }

    protected override ISymbolicDataAnalysisModel CreateModel() {
      var model = ModelCreator.CreateSymbolicClassificationModel(SymbolicExpressionTree, Interpreter, EstimationLimits.Lower, EstimationLimits.Upper);
      var rows = Enumerable.Range(FitnessCalculationPartition.Start, FitnessCalculationPartition.Size);
      var problemData = (IClassificationProblemData)ProblemData;
      model.RecalculateModelParameters(problemData, rows);
      return model;
    }

    protected override double Evaluate(IDataAnalysisModel model) {
      var classificationModel = (IClassificationModel)model;
      var classificationProblemData = (IClassificationProblemData)ProblemData;
      var trainingIndices = ProblemData.TrainingIndices.ToList();
      var estimatedValues = classificationModel.GetEstimatedClassValues(ProblemData.Dataset, trainingIndices);
      var targetValues = ProblemData.Dataset.GetDoubleValues(classificationProblemData.TargetVariable, trainingIndices);
      OnlineCalculatorError errorState;
      var quality = OnlinePearsonsRSquaredCalculator.Calculate(targetValues, estimatedValues, out errorState);
      if (errorState != OnlineCalculatorError.None) return double.NaN;
      return quality;
    }
  }
}
