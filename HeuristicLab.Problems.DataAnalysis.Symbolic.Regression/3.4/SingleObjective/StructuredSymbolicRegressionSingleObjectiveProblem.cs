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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  [StorableType("7464E84B-65CC-440A-91F0-9FA920D730F9")]
  [Item(Name = "Structured Symbolic Regression Single Objective Problem (single-objective)", Description = "A problem with a structural definition and unfixed subfunctions.")]
  [Creatable(CreatableAttribute.Categories.GeneticProgrammingProblems, Priority = 150)]
  public class StructuredSymbolicRegressionSingleObjectiveProblem : SingleObjectiveBasicProblem<MultiEncoding>, IRegressionProblem, IProblemInstanceConsumer<RegressionProblemData> {

    #region Constants
    private const string ProblemDataParameterName = "ProblemData";
    private const string StructureDefinitionParameterName = "Structure Definition";
    private const string GrammarParameterName = "Grammar";
    private const string StructureTemplateParameterName = "Structure Template";
    #endregion

    #region Parameter
    public IValueParameter<IRegressionProblemData> ProblemDataParameter => (IValueParameter<IRegressionProblemData>)Parameters[ProblemDataParameterName];
    public IFixedValueParameter<StringValue> StructureDefinitionParameter => (IFixedValueParameter<StringValue>)Parameters[StructureDefinitionParameterName];
    public IFixedValueParameter<StructureTemplate> StructureTemplateParameter => (IFixedValueParameter<StructureTemplate>)Parameters[StructureTemplateParameterName];
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

    public StructureTemplate StructureTemplate {
      get => StructureTemplateParameter.Value;
    }

    public ISymbolicDataAnalysisGrammar Grammar {
      get => GrammarParameter.Value;
      set => GrammarParameter.Value = value;
    }

    IParameter IDataAnalysisProblem.ProblemDataParameter => ProblemDataParameter;
    IDataAnalysisProblemData IDataAnalysisProblem.ProblemData => ProblemData;

    public override bool Maximization => true;
    #endregion

    #region EventHandlers
    public event EventHandler ProblemDataChanged;
    #endregion

    #region Constructors & Cloning
    public StructuredSymbolicRegressionSingleObjectiveProblem() {
      var problemData = new ShapeConstrainedRegressionProblemData();
      var grammar = new LinearScalingGrammar();
      var varSym = (Variable)grammar.GetSymbol("Variable");
      varSym.AllVariableNames = problemData.InputVariables.Select(x => x.Value);
      varSym.VariableNames = problemData.InputVariables.Select(x => x.Value);
      varSym.Enabled = true;

      var structureTemplate = new StructureTemplate();
      structureTemplate.Changed += OnTemplateChanged;

      Parameters.Add(new ValueParameter<IRegressionProblemData>(ProblemDataParameterName, problemData));
      Parameters.Add(new FixedValueParameter<StructureTemplate>(StructureTemplateParameterName, structureTemplate));
      Parameters.Add(new ValueParameter<ISymbolicDataAnalysisGrammar>(GrammarParameterName, grammar));

      //structureTemplate.Template = "f(x)*f(y)+5";
    }

    public StructuredSymbolicRegressionSingleObjectiveProblem(StructuredSymbolicRegressionSingleObjectiveProblem original, Cloner cloner) { }

    [StorableConstructor]
    protected StructuredSymbolicRegressionSingleObjectiveProblem(StorableConstructorFlag _) : base(_) { }
    #endregion

    #region Cloning
    public override IDeepCloneable Clone(Cloner cloner) =>
      new StructuredSymbolicRegressionSingleObjectiveProblem(this, cloner);
    #endregion

    private void OnTemplateChanged(object sender, EventArgs args) {
      foreach (var e in Encoding.Encodings.ToArray())
        Encoding.Remove(e);

      foreach (var sf in StructureTemplate.SubFunctions.Values) {
        Encoding.Add(new SymbolicExpressionTreeEncoding(sf.Name, sf.Grammar, sf.MaximumSymbolicExpressionTreeLength, sf.MaximumSymbolicExpressionTreeDepth));
      }
    }

    public override void Analyze(Individual[] individuals, double[] qualities, ResultCollection results, IRandom random) {
      base.Analyze(individuals, qualities, results, random);

      int bestIdx = 0;
      double bestQuality = Maximization ? double.MinValue : double.MaxValue;
      for(int idx = 0; idx < qualities.Length; ++idx) {
        if((Maximization && qualities[idx] > bestQuality) || 
          (!Maximization && qualities[idx] < bestQuality)) {
          bestQuality = qualities[idx];
          bestIdx = idx;
        }
      }

      if (results.TryGetValue("Best Tree", out IResult result))
        result.Value = BuildTree(individuals[bestIdx]);
      else
        results.Add(new Result("Best Tree", BuildTree(individuals[bestIdx])));

      /*
      if (results.TryGetValue("Tree", out IResult result)) {
        var list = result.Value as ItemList<ISymbolicExpressionTree>;
        list.Clear();
        list.AddRange(individuals.Select(x => (BuildTree(x))));
      } else 
        results.Add(new Result("Tree", new ItemList<ISymbolicExpressionTree>(individuals.Select(x => (BuildTree(x))))));
      */
    }

    public override double Evaluate(Individual individual, IRandom random) {
      var tree = BuildTree(individual);
      var interpreter = new SymbolicDataAnalysisExpressionTreeInterpreter();
      var estimationInterval = ProblemData.VariableRanges.GetInterval(ProblemData.TargetVariable);
      var quality = SymbolicRegressionSingleObjectivePearsonRSquaredEvaluator.Calculate(
        interpreter, tree, 
        estimationInterval.LowerBound, estimationInterval.UpperBound, 
        ProblemData, ProblemData.TrainingIndices, false);
     
      return quality;
    }

    private ISymbolicExpressionTree BuildTree(Individual individual) {
      var templateTree = (ISymbolicExpressionTree)StructureTemplate.Tree.Clone();

      // build main tree
      foreach (var n in templateTree.IterateNodesPrefix()) {
        if (n.Symbol is SubFunctionSymbol) {
          var subFunctionTreeNode = n as SubFunctionTreeNode;
          var subFunctionTree = individual.SymbolicExpressionTree(subFunctionTreeNode.SubFunction.Name);
          var parent = n.Parent;

          // remove SubFunctionTreeNode
          parent.RemoveSubtree(parent.IndexOfSubtree(subFunctionTreeNode));

          // add new tree
          var subTree = subFunctionTree.Root.GetSubtree(0)  // Start
                                            .GetSubtree(0); // Offset
          parent.AddSubtree(subTree);
        }
      }
      return templateTree;
    }

    public void Load(RegressionProblemData data) {
      ProblemData = data;
    }
  }
}
