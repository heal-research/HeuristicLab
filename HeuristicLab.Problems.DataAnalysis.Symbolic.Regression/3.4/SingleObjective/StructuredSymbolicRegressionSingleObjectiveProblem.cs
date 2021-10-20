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
    private const string StructureTemplateParameterName = "Structure Template";
    #endregion

    #region Parameter
    public IValueParameter<IRegressionProblemData> ProblemDataParameter => (IValueParameter<IRegressionProblemData>)Parameters[ProblemDataParameterName];
    public IFixedValueParameter<StringValue> StructureDefinitionParameter => (IFixedValueParameter<StringValue>)Parameters[StructureDefinitionParameterName];
    public IFixedValueParameter<StructureTemplate> StructureTemplateParameter => (IFixedValueParameter<StructureTemplate>)Parameters[StructureTemplateParameterName];
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

    public ISymbolicDataAnalysisExpressionTreeInterpreter Interpreter { get; } = new SymbolicDataAnalysisExpressionTreeInterpreter();

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

      var structureTemplate = new StructureTemplate();
      structureTemplate.Changed += OnTemplateChanged;

      Parameters.Add(new ValueParameter<IRegressionProblemData>(ProblemDataParameterName, problemData));
      Parameters.Add(new FixedValueParameter<StructureTemplate>(StructureTemplateParameterName, structureTemplate));

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
      SetupStructureTemplate();
    }

    private void SetupStructureTemplate() {
      foreach (var e in Encoding.Encodings.ToArray())
        Encoding.Remove(e);

      foreach (var f in StructureTemplate.SubFunctions.Values) {
        SetupVariables(f);
        if(!Encoding.Encodings.Any(x => x.Name == f.Name)) // to prevent the same encoding twice
          Encoding.Add(new SymbolicExpressionTreeEncoding(f.Name, f.Grammar, f.MaximumSymbolicExpressionTreeLength, f.MaximumSymbolicExpressionTreeDepth));
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

      if (results.TryGetValue("Best Tree", out IResult result)) {
        var tree = BuildTree(individuals[bestIdx]);
        AdjustLinearScalingParams(tree, Interpreter);
        result.Value = tree;
      }
      else {
        var tree = BuildTree(individuals[bestIdx]);
        AdjustLinearScalingParams(tree, Interpreter);
        results.Add(new Result("Best Tree", tree));
      }
        
    }

    public override double Evaluate(Individual individual, IRandom random) {
      var tree = BuildTree(individual);

      AdjustLinearScalingParams(tree, Interpreter);
      var estimationInterval = ProblemData.VariableRanges.GetInterval(ProblemData.TargetVariable);
      var quality = SymbolicRegressionSingleObjectivePearsonRSquaredEvaluator.Calculate(
        Interpreter, tree, 
        estimationInterval.LowerBound, estimationInterval.UpperBound, 
        ProblemData, ProblemData.TrainingIndices, false);
     
      return quality;
    }

    private void AdjustLinearScalingParams(ISymbolicExpressionTree tree, ISymbolicDataAnalysisExpressionTreeInterpreter interpreter) {
      var offsetNode = tree.Root.GetSubtree(0).GetSubtree(0);
      var scalingNode = offsetNode.Subtrees.Where(x => !(x is ConstantTreeNode)).First();

      var offsetConstantNode = (ConstantTreeNode)offsetNode.Subtrees.Where(x => x is ConstantTreeNode).First();
      var scalingConstantNode = (ConstantTreeNode)scalingNode.Subtrees.Where(x => x is ConstantTreeNode).First();

      var estimatedValues = interpreter.GetSymbolicExpressionTreeValues(tree, ProblemData.Dataset, ProblemData.TrainingIndices);
      var targetValues = ProblemData.Dataset.GetDoubleValues(ProblemData.TargetVariable, ProblemData.TrainingIndices);

      OnlineLinearScalingParameterCalculator.Calculate(estimatedValues, targetValues, out double a, out double b, out OnlineCalculatorError error);
      if(error == OnlineCalculatorError.None) {
        offsetConstantNode.Value = a;
        scalingConstantNode.Value = b;
      }
    }

    private ISymbolicExpressionTree BuildTree(Individual individual) {
      var templateTree = (ISymbolicExpressionTree)StructureTemplate.Tree.Clone();

      // build main tree
      foreach (var n in templateTree.IterateNodesPrefix()) {
        if (n.Symbol is SubFunctionSymbol) {
          var subFunctionTreeNode = n as SubFunctionTreeNode;
          var subFunctionTree = individual.SymbolicExpressionTree(subFunctionTreeNode.Name);
          //var parent = n.Parent;

          // remove SubFunctionTreeNode
          //parent.RemoveSubtree(parent.IndexOfSubtree(subFunctionTreeNode));

          // add new tree
          var subTree = subFunctionTree.Root.GetSubtree(0)  // Start
                                            .GetSubtree(0); // Offset
          //parent.AddSubtree(subTree);
          subFunctionTreeNode.AddSubtree(subTree);
        }
      }
      return templateTree;
    }

    private void SetupVariables(SubFunction subFunction) {
      var varSym = (Variable)subFunction.Grammar.GetSymbol("Variable");
      if (varSym == null) {
        varSym = new Variable();
        subFunction.Grammar.AddSymbol(varSym);
      }

      var allVariables = ProblemData.InputVariables.Select(x => x.Value);
      var allInputs = allVariables.Where(x => x != ProblemData.TargetVariable);

      // set all variables
      varSym.AllVariableNames = allVariables;

      // set all allowed variables
      if (subFunction.Arguments.Contains("_")) {
        varSym.VariableNames = allInputs;
      } else {
        var vars = new List<string>();
        var exceptions = new List<Exception>();
        foreach (var arg in subFunction.Arguments) {
          if (allInputs.Contains(arg))
            vars.Add(arg);
          else
            exceptions.Add(new ArgumentException($"The argument '{arg}' for sub-function '{subFunction.Name}' is not a valid variable."));
        }
        if (exceptions.Any())
          throw new AggregateException(exceptions);
        varSym.VariableNames = vars;
      }

      varSym.Enabled = true;
    }

    public void Load(RegressionProblemData data) {
      ProblemData = data;
      SetupStructureTemplate();
    }
  }
}
