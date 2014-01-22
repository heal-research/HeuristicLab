using System;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Classification {
  [Item("SymbolicClassificationPruningAnalyzer", "An analyzer that prunes introns from the population.")]
  [StorableClass]
  public sealed class SymbolicClassificationPruningAnalyzer : SymbolicDataAnalysisSingleObjectivePruningAnalyzer {
    private const string ModelCreatorParameterName = "ModelCreator";
    #region parameter properties
    public ILookupParameter<ISymbolicClassificationModelCreator> ModelCreatorParameter {
      get { return (ILookupParameter<ISymbolicClassificationModelCreator>)Parameters[ModelCreatorParameterName]; }
    }
    #endregion
    #region properties
    private ISymbolicClassificationModelCreator ModelCreator {
      get { return ModelCreatorParameter.ActualValue; }
      set { ModelCreatorParameter.ActualValue = value; }
    }
    #endregion

    protected SymbolicClassificationPruningAnalyzer(SymbolicClassificationPruningAnalyzer original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicClassificationPruningAnalyzer(this, cloner);
    }

    public SymbolicClassificationPruningAnalyzer() {
      // pruning parameters
      Parameters.Add(new LookupParameter<ISymbolicClassificationModelCreator>(ModelCreatorParameterName));
      impactValuesCalculator = new SymbolicClassificationSolutionImpactValuesCalculator();
    }

    protected override ISymbolicDataAnalysisModel CreateModel(ISymbolicExpressionTree tree,
      ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, double lowerEstimationLimit = Double.MinValue,
      double upperEstimationLimit = Double.MaxValue) {
      var model = ModelCreator.CreateSymbolicClassificationModel(tree, Interpreter, lowerEstimationLimit, upperEstimationLimit);
      model.RecalculateModelParameters((IClassificationProblemData)ProblemData, ProblemData.TrainingIndices);
      return model;
    }
  }
}
