using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Problems.Instances;
using HeuristicLab.Parameters;
using HeuristicLab.Data;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression.SingleObjective {
  [StorableType("7464E84B-65CC-440A-91F0-9FA920D730F9")]
  [Item(Name = "StructuredSymbolicRegressionSingleObjectiveProblem", Description = "A problem with a structural definition and unfixed subfunctions.")]
  public class StructuredSymbolicRegressionSingleObjectiveProblem : SingleObjectiveBasicProblem<MultiEncoding>, IRegressionProblem, IProblemInstanceConsumer<RegressionProblemData> {

    #region Constants
    private const string ProblemDataParameterName = "ProblemData";
    private const string StructureDefinitionParameterName = "Structure Definition";
    private const string GrammarParameterName = "Grammar";

    #endregion

    #region Parameter
    public IValueParameter<IRegressionProblemData> ProblemDataParameter => (IValueParameter<IRegressionProblemData>)Parameters[ProblemDataParameterName];
    public IFixedValueParameter<StringValue> StructureDefinitionParameter => (IFixedValueParameter<StringValue>)Parameters[StructureDefinitionParameterName];
    public IValueParameter<ISymbolicDataAnalysisGrammar> GrammarParameter => (IValueParameter<ISymbolicDataAnalysisGrammar>)Parameters[GrammarParameterName];
    #endregion

    #region Properties
    public IRegressionProblemData ProblemData { 
      get => ProblemDataParameter.Value; 
      set {
        ProblemDataParameter.Value = value;
        ProblemDataChanged?.Invoke(this, EventArgs.Empty);
      }
    }

    public string StructureDefinition {
      get => StructureDefinitionParameter.Value.Value;
      set => StructureDefinitionParameter.Value.Value = value;
    }

    public ISymbolicDataAnalysisGrammar Grammar {
      get => GrammarParameter.Value;
      set => GrammarParameter.Value = value;
    }

    IParameter IDataAnalysisProblem.ProblemDataParameter => ProblemDataParameter;
    IDataAnalysisProblemData IDataAnalysisProblem.ProblemData => ProblemData;

    public override bool Maximization => false;
    #endregion

    #region EventHandlers
    public event EventHandler ProblemDataChanged;
    #endregion

    #region Constructors & Cloning
    public StructuredSymbolicRegressionSingleObjectiveProblem() {
      Parameters.Add(new ValueParameter<RegressionProblemData>(ProblemDataParameterName, new RegressionProblemData()));
      Parameters.Add(new FixedValueParameter<StringValue>(StructureDefinitionParameterName, new StringValue("")));
      Parameters.Add(new ValueParameter<ISymbolicDataAnalysisGrammar>(GrammarParameterName, new LinearScalingGrammar()));
    }

    public StructuredSymbolicRegressionSingleObjectiveProblem(StructuredSymbolicRegressionSingleObjectiveProblem original, Cloner cloner) {
    }

    [StorableConstructor]
    protected StructuredSymbolicRegressionSingleObjectiveProblem(StorableConstructorFlag _) : base(_) {
    }

    public override IDeepCloneable Clone(Cloner cloner) =>
      new StructuredSymbolicRegressionSingleObjectiveProblem(this, cloner);
    #endregion

    public override double Evaluate(Individual individual, IRandom random) {
      return 0.0;
    }

    public void Load(RegressionProblemData data) {
      ProblemData = data;
    }
  }
}
