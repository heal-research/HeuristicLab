using System;
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
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

    public SymbolicRegressionSingleObjectiveEvaluator TreeEvaluator => TreeEvaluatorParameter.Value;

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
        new SymbolicDataAnalysisExpressionTreeBatchInterpreter()) { Hidden = true });

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

    public StructuredSymbolicRegressionSingleObjectiveProblem(StructuredSymbolicRegressionSingleObjectiveProblem original, Cloner cloner) : base(original, cloner) {
      RegisterEventHandlers();
    }

    public override IDeepCloneable Clone(Cloner cloner) =>
      new StructuredSymbolicRegressionSingleObjectiveProblem(this, cloner);

    [StorableConstructor]
    protected StructuredSymbolicRegressionSingleObjectiveProblem(StorableConstructorFlag _) : base(_) { }


    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    #endregion

    private void RegisterEventHandlers() {
      if (StructureTemplate != null) {
        StructureTemplate.Changed += OnTemplateChanged;
      }

      ProblemDataParameter.ValueChanged += ProblemDataParameterValueChanged;
    }

    private void ProblemDataParameterValueChanged(object sender, EventArgs e) {
      StructureTemplate.Reset();
      // InfoBox for Reset?
    }

    private void OnTemplateChanged(object sender, EventArgs args) {
      SetupEncoding();
    }

    private void SetupEncoding() {
      foreach (var e in Encoding.Encodings.ToArray())
        Encoding.Remove(e);

      foreach (var subFunction in StructureTemplate.SubFunctions) {
        subFunction.SetupVariables(ProblemData.AllowedInputVariables);
        // to prevent the same encoding twice
        if (Encoding.Encodings.Any(x => x.Name == subFunction.Name)) continue; // duplicate subfunction

        var encoding = new SymbolicExpressionTreeEncoding(
          subFunction.Name,
          subFunction.Grammar,
          subFunction.MaximumSymbolicExpressionTreeLength,
          subFunction.MaximumSymbolicExpressionTreeDepth);
        Encoding.Add(encoding);
      }

      //set multi manipulator as default manipulator for all encoding parts
      var manipulator = (IParameterizedItem)Encoding.Operators.OfType<MultiEncodingManipulator>().FirstOrDefault();
      if (manipulator != null) {
        foreach (var param in manipulator.Parameters.OfType<ConstrainedValueParameter<IManipulator>>()) {
          var m = param.ValidValues.OfType<MultiSymbolicExpressionTreeManipulator>().FirstOrDefault();
          param.Value = m == null ? param.ValidValues.First() : m;
        }
      }
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
      var templateTree = StructureTemplate.Tree;
      if (templateTree == null)
        throw new ArgumentException("No structure template defined!");

      var tree = BuildTree(templateTree, individual);

      // NMSEConstraintsEvaluator sets linear scaling terms itself
      if (StructureTemplate.ApplyLinearScaling && !(TreeEvaluator is NMSESingleObjectiveConstraintsEvaluator)) {
        AdjustLinearScalingParams(ProblemData, tree, Interpreter);
      }

      individual[SymbolicExpressionTreeName] = tree;

      return TreeEvaluator.Evaluate(
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

    private static ISymbolicExpressionTree BuildTree(ISymbolicExpressionTree template, Individual individual) {
      var resolvedTree = (ISymbolicExpressionTree)template.Clone();

      // build main tree
      foreach (var subFunctionTreeNode in resolvedTree.IterateNodesPrefix().OfType<SubFunctionTreeNode>()) {
        var subFunctionTree = individual.SymbolicExpressionTree(subFunctionTreeNode.Name);

        // extract function tree
        var subTree = subFunctionTree.Root.GetSubtree(0)  // StartSymbol
                                          .GetSubtree(0); // First Symbol
        subTree = (ISymbolicExpressionTreeNode)subTree.Clone();
        subFunctionTreeNode.AddSubtree(subTree);
      }
      return resolvedTree;
    }


    public void Load(IRegressionProblemData data) {
      ProblemData = data;
    }
  }
}
