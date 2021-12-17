using System;
using System.Collections.Generic;
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.Instances;
using HeuristicLab.Problems.Instances.DataAnalysis;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  [StorableType("7464E84B-65CC-440A-91F0-9FA920D730F9")]
  [Item(Name = "Structured Symbolic Regression Single Objective Problem (single-objective)", Description = "A problem with a structural definition and unfixed subfunctions.")]
  [Creatable(CreatableAttribute.Categories.GeneticProgrammingProblems, Priority = 150)]
  public class StructuredSymbolicRegressionSingleObjectiveProblem : SingleObjectiveBasicProblem<MultiEncoding>, IRegressionProblem, IProblemInstanceConsumer<IRegressionProblemData> {

    #region Constants
    private const string TreeEvaluatorParameterName = "TreeEvaluator";
    private const string ProblemDataParameterName = "ProblemData";
    private const string StructureTemplateParameterName = "Structure Template";
    private const string InterpreterParameterName = "Interpreter";
    private const string EstimationLimitsParameterName = "EstimationLimits";
    private const string BestTrainingSolutionParameterName = "Best Training Solution";

    private const string SymbolicExpressionTreeName = "SymbolicExpressionTree";
    private const string VariableName = "Variable";

    private const string StructureTemplateDescriptionText =
      "Enter your expression as string in infix format into the empty input field.\n" +
      "By checking the \"Apply Linear Scaling\" checkbox you can add the relevant scaling terms to your expression.\n" +
      "After entering the expression click parse to build the tree.\n" +
      "To edit the defined sub-functions, click on the corresponding-colored node in the tree view.\n" +
      "Check the info box besides the input field for more information.";
    #endregion

    #region Parameters
    public IConstrainedValueParameter<SymbolicRegressionSingleObjectiveEvaluator> TreeEvaluatorParameter => (IConstrainedValueParameter<SymbolicRegressionSingleObjectiveEvaluator>)Parameters[TreeEvaluatorParameterName];
    public IValueParameter<IRegressionProblemData> ProblemDataParameter => (IValueParameter<IRegressionProblemData>)Parameters[ProblemDataParameterName];
    public IFixedValueParameter<StructureTemplate> StructureTemplateParameter => (IFixedValueParameter<StructureTemplate>)Parameters[StructureTemplateParameterName];
    public IValueParameter<ISymbolicDataAnalysisExpressionTreeInterpreter> InterpreterParameter => (IValueParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>)Parameters[InterpreterParameterName];
    public IFixedValueParameter<DoubleLimit> EstimationLimitsParameter => (IFixedValueParameter<DoubleLimit>)Parameters[EstimationLimitsParameterName];
    public IResultParameter<ISymbolicRegressionSolution> BestTrainingSolutionParameter => (IResultParameter<ISymbolicRegressionSolution>)Parameters[BestTrainingSolutionParameterName];
    #endregion

    #region Properties

    public IRegressionProblemData ProblemData {
      get => ProblemDataParameter.Value;
      set {
        ProblemDataParameter.Value = value;
        ProblemDataChanged?.Invoke(this, EventArgs.Empty);
      }
    }

    public StructureTemplate StructureTemplate => StructureTemplateParameter.Value;

    public ISymbolicDataAnalysisExpressionTreeInterpreter Interpreter => InterpreterParameter.Value;

    IParameter IDataAnalysisProblem.ProblemDataParameter => ProblemDataParameter;
    IDataAnalysisProblemData IDataAnalysisProblem.ProblemData => ProblemData;

    public DoubleLimit EstimationLimits => EstimationLimitsParameter.Value;

    public override bool Maximization => false;
    #endregion

    #region EventHandlers
    public event EventHandler ProblemDataChanged;
    #endregion

    #region Constructors & Cloning
    public StructuredSymbolicRegressionSingleObjectiveProblem() {
      var provider = new PhysicsInstanceProvider();
      var descriptor = new SheetBendingProcess();
      var problemData = provider.LoadData(descriptor);
      var shapeConstraintProblemData = new ShapeConstrainedRegressionProblemData(problemData);

      var structureTemplate = new StructureTemplate();

      var evaluators = new ItemSet<SymbolicRegressionSingleObjectiveEvaluator>(
        ApplicationManager.Manager.GetInstances<SymbolicRegressionSingleObjectiveEvaluator>()
        .Where(x => x.Maximization == Maximization));

      Parameters.Add(new ConstrainedValueParameter<SymbolicRegressionSingleObjectiveEvaluator>(
        TreeEvaluatorParameterName,
        evaluators,
        evaluators.First()));

      Parameters.Add(new ValueParameter<IRegressionProblemData>(
        ProblemDataParameterName,
        shapeConstraintProblemData));

      Parameters.Add(new FixedValueParameter<StructureTemplate>(
        StructureTemplateParameterName,
        StructureTemplateDescriptionText,
        structureTemplate));

      Parameters.Add(new ValueParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>(
        InterpreterParameterName,
        new SymbolicDataAnalysisExpressionTreeInterpreter()) { Hidden = true });

      Parameters.Add(new FixedValueParameter<DoubleLimit>(
        EstimationLimitsParameterName,
        new DoubleLimit(double.NegativeInfinity, double.PositiveInfinity)) { Hidden = true });

      Parameters.Add(new ResultParameter<ISymbolicRegressionSolution>(BestTrainingSolutionParameterName, "") { Hidden = true });

      this.EvaluatorParameter.Hidden = true;

      Operators.Add(new SymbolicDataAnalysisVariableFrequencyAnalyzer());
      Operators.Add(new MinAverageMaxSymbolicExpressionTreeLengthAnalyzer());
      Operators.Add(new SymbolicExpressionSymbolFrequencyAnalyzer());

      RegisterEventHandlers();
      StructureTemplate.Template =
        "(" +
          "(210000 / (210000 + h)) * ((sigma_y * t * t) / (wR * Rt * t)) + " +
          "PlasticHardening(_) - Elasticity(_)" +
        ")" +
        " * C(_)";
    }

    public StructuredSymbolicRegressionSingleObjectiveProblem(StructuredSymbolicRegressionSingleObjectiveProblem original,
      Cloner cloner) : base(original, cloner) {
      ProblemDataParameter.ValueChanged += ProblemDataParameterValueChanged;
      RegisterEventHandlers();
    }

    [StorableConstructor]
    protected StructuredSymbolicRegressionSingleObjectiveProblem(StorableConstructorFlag _) : base(_) { }


    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    #endregion

    #region Cloning
    public override IDeepCloneable Clone(Cloner cloner) =>
      new StructuredSymbolicRegressionSingleObjectiveProblem(this, cloner);
    #endregion

    private void ProblemDataParameterValueChanged(object sender, EventArgs e) {
      StructureTemplate.Reset();
      // InfoBox for Reset?
    }

    private void RegisterEventHandlers() {
      if (StructureTemplate != null) {
        StructureTemplate.Changed += OnTemplateChanged;
      }

      ProblemDataParameter.ValueChanged += ProblemDataParameterValueChanged;
    }

    private void OnTemplateChanged(object sender, EventArgs args) {
      SetupStructureTemplate();
    }

    private void SetupStructureTemplate() {
      foreach (var e in Encoding.Encodings.ToArray())
        Encoding.Remove(e);

      foreach (var f in StructureTemplate.SubFunctions) {
        SetupVariables(f);
        // to prevent the same encoding twice
        if (!Encoding.Encodings.Any(x => x.Name == f.Name)) 
          Encoding.Add(CreateEncoding(f));
      }

      ParameterizeEncoding();
    }

    public override void Analyze(Individual[] individuals, double[] qualities, ResultCollection results, IRandom random) {
      base.Analyze(individuals, qualities, results, random);

      var best = GetBestIndividual(individuals, qualities).Item1;

      if (!results.ContainsKey(BestTrainingSolutionParameter.ActualName)) {
        results.Add(new Result(BestTrainingSolutionParameter.ActualName, typeof(SymbolicRegressionSolution)));
      }

      var tree = (ISymbolicExpressionTree)best[SymbolicExpressionTreeName];

      var model = new SymbolicRegressionModel(ProblemData.TargetVariable, tree, Interpreter);
      var solution = model.CreateRegressionSolution(ProblemData);

      results[BestTrainingSolutionParameter.ActualName].Value = solution;
    }


    public override double Evaluate(Individual individual, IRandom random) {
      var tree = BuildTree(individual);

      if (StructureTemplate.ApplyLinearScaling)
        AdjustLinearScalingParams(ProblemData, tree, Interpreter);

      individual[SymbolicExpressionTreeName] = tree;

      // dpiringe: needed when Maximization = true
      if (TreeEvaluatorParameter.Value is SymbolicRegressionParameterOptimizationEvaluator constantOptEvaluator) {
        constantOptEvaluator.RandomParameter.Value = random;
        constantOptEvaluator.RelativeNumberOfEvaluatedSamplesParameter.Value =
          (PercentValue)constantOptEvaluator.ParameterOptimizationRowsPercentage.Clone();
      }

      return TreeEvaluatorParameter.Value.Evaluate(
        tree, ProblemData,
        ProblemData.TrainingIndices,
        Interpreter,
        StructureTemplate.ApplyLinearScaling,
        EstimationLimits.Lower,
        EstimationLimits.Upper);
    }

    private static void AdjustLinearScalingParams(IRegressionProblemData problemData, ISymbolicExpressionTree tree, ISymbolicDataAnalysisExpressionTreeInterpreter interpreter) {
      var offsetNode = tree.Root.GetSubtree(0).GetSubtree(0);
      var scalingNode = offsetNode.Subtrees.Where(x => !(x is NumberTreeNode)).First();

      var offsetNumberNode = (NumberTreeNode)offsetNode.Subtrees.Where(x => x is NumberTreeNode).First();
      var scalingNumberNode = (NumberTreeNode)scalingNode.Subtrees.Where(x => x is NumberTreeNode).First();

      var estimatedValues = interpreter.GetSymbolicExpressionTreeValues(tree, problemData.Dataset, problemData.TrainingIndices);
      var targetValues = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, problemData.TrainingIndices);

      OnlineLinearScalingParameterCalculator.Calculate(estimatedValues, targetValues, out double a, out double b, out OnlineCalculatorError error);
      if (error == OnlineCalculatorError.None) {
        offsetNumberNode.Value = a;
        scalingNumberNode.Value = b;
      }
    }

    private ISymbolicExpressionTree BuildTree(Individual individual) {
      if (StructureTemplate.Tree == null)
        throw new ArgumentException("No structure template defined!");

      var templateTree = (ISymbolicExpressionTree)StructureTemplate.Tree.Clone();

      // build main tree
      foreach (var subFunctionTreeNode in templateTree.IterateNodesPrefix().OfType<SubFunctionTreeNode>()) {
        var subFunctionTree = individual.SymbolicExpressionTree(subFunctionTreeNode.Name);

        // add new tree
        var subTree = subFunctionTree.Root.GetSubtree(0)  // Start
                                          .GetSubtree(0); // Offset
        subTree = (ISymbolicExpressionTreeNode)subTree.Clone();
        subFunctionTreeNode.AddSubtree(subTree);

      }
      return templateTree;
    }

    private SymbolicExpressionTreeEncoding CreateEncoding(SubFunction subFunction) {
      var encoding = new SymbolicExpressionTreeEncoding(
            subFunction.Name,
            subFunction.Grammar,
            subFunction.MaximumSymbolicExpressionTreeLength,
            subFunction.MaximumSymbolicExpressionTreeDepth);
      return encoding;
    }

    private void ParameterizeEncoding() {
      var manipulator = (IParameterizedItem)Encoding.Operators.OfType<MultiEncodingManipulator>().FirstOrDefault();
      if (manipulator != null) {
        foreach (var param in manipulator.Parameters.OfType<ConstrainedValueParameter<IManipulator>>()) {
          var m = param.ValidValues.OfType<MultiSymbolicExpressionTreeManipulator>().FirstOrDefault();
          param.Value = m == null ? param.ValidValues.First() : m;
        }
      }
    }
    private void SetupVariables(SubFunction subFunction) {
      var varSym = (Variable)subFunction.Grammar.GetSymbol(VariableName);
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

    public void Load(IRegressionProblemData data) {
      ProblemData = data;
    }
  }
}
