#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Problems.Instances;
using HeuristicLab.Problems.Instances.DataAnalysis;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  [StorableType("7464E84B-65CC-440A-91F0-9FA920D730F9")]
  [Item(Name = "Structured Symbolic Regression Single Objective Problem (single-objective)", Description = "A problem with a structural definition and unfixed subfunctions.")]
  [Creatable(CreatableAttribute.Categories.GeneticProgrammingProblems, Priority = 150)]
  public class StructuredSymbolicRegressionSingleObjectiveProblem : SingleObjectiveBasicProblem<MultiEncoding>, IRegressionProblem, IProblemInstanceConsumer<IRegressionProblemData> {

    #region Constants
    private const string ProblemDataParameterName = "ProblemData";
    private const string StructureTemplateParameterName = "Structure Template";
    private const string InterpreterParameterName = "Interpreter";
    private const string EstimationLimitsParameterName = "EstimationLimits";
    private const string BestTrainingSolutionParameterName = "Best Training Solution";
    private const string ApplyLinearScalingParameterName = "Apply Linear Scaling";
    private const string OptimizeParametersParameterName = "Optimize Parameters";

    private const string SymbolicExpressionTreeName = "SymbolicExpressionTree";
    private const string NumericParametersEncoding = "Numeric Parameters";

    private const string StructureTemplateDescriptionText =
      "Enter your expression as string in infix format into the empty input field.\n" +
      "By checking the \"Apply Linear Scaling\" checkbox you can add the relevant scaling terms to your expression.\n" +
      "After entering the expression click parse to build the tree.\n" +
      "To edit the defined sub-functions, click on the corresponding-colored node in the tree view.\n" +
      "Check the info box besides the input field for more information.";
    #endregion

    #region Parameters
    public IValueParameter<IRegressionProblemData> ProblemDataParameter => (IValueParameter<IRegressionProblemData>)Parameters[ProblemDataParameterName];
    public IFixedValueParameter<StructureTemplate> StructureTemplateParameter => (IFixedValueParameter<StructureTemplate>)Parameters[StructureTemplateParameterName];
    public IValueParameter<ISymbolicDataAnalysisExpressionTreeInterpreter> InterpreterParameter => (IValueParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>)Parameters[InterpreterParameterName];
    public IFixedValueParameter<DoubleLimit> EstimationLimitsParameter => (IFixedValueParameter<DoubleLimit>)Parameters[EstimationLimitsParameterName];
    public IResultParameter<ISymbolicRegressionSolution> BestTrainingSolutionParameter => (IResultParameter<ISymbolicRegressionSolution>)Parameters[BestTrainingSolutionParameterName];

    public IFixedValueParameter<BoolValue> ApplyLinearScalingParameter => (IFixedValueParameter<BoolValue>)Parameters[ApplyLinearScalingParameterName];
    public IFixedValueParameter<BoolValue> OptimizeParametersParameter => (IFixedValueParameter<BoolValue>)Parameters[OptimizeParametersParameterName];
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

    public bool ApplyLinearScaling {
      get => ApplyLinearScalingParameter.Value.Value;
      set => ApplyLinearScalingParameter.Value.Value = value;
    }

    public bool OptimizeParameters {
      get => OptimizeParametersParameter.Value.Value;
      set => OptimizeParametersParameter.Value.Value = value;
    }

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

      Parameters.Add(new ValueParameter<IRegressionProblemData>(
        ProblemDataParameterName,
        shapeConstraintProblemData));

      Parameters.Add(new FixedValueParameter<StructureTemplate>(
        StructureTemplateParameterName,
        StructureTemplateDescriptionText,
        structureTemplate));

      Parameters.Add(new FixedValueParameter<BoolValue>(
        ApplyLinearScalingParameterName, new BoolValue(true)
        ));

      Parameters.Add(new FixedValueParameter<BoolValue>(
        OptimizeParametersParameterName, new BoolValue(true)
        ));

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

      StructureTemplate.ApplyLinearScaling = ApplyLinearScaling;
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
      if (!Parameters.ContainsKey(ApplyLinearScalingParameterName)) {
        Parameters.Add(new FixedValueParameter<BoolValue>(ApplyLinearScalingParameterName, new BoolValue(StructureTemplate.ApplyLinearScaling)));
      }

      if (!Parameters.ContainsKey(OptimizeParametersParameterName)) {
        Parameters.Add(new FixedValueParameter<BoolValue>(OptimizeParametersParameterName, new BoolValue(false)));
      }

      RegisterEventHandlers();
    }

    #endregion

    private void RegisterEventHandlers() {
      if (StructureTemplate != null) {
        StructureTemplate.Changed += OnTemplateChanged;
      }

      ProblemDataParameter.ValueChanged += ProblemDataParameterValueChanged;
      ApplyLinearScalingParameter.Value.ValueChanged += (o, e) => StructureTemplate.ApplyLinearScaling = ApplyLinearScaling;
    }

    private void ProblemDataParameterValueChanged(object sender, EventArgs e) {
      StructureTemplate.Reset();
      // InfoBox for Reset?
    }

    private void OnTemplateChanged(object sender, EventArgs args) {
      ApplyLinearScaling = StructureTemplate.ApplyLinearScaling;
      SetupEncoding();
    }

    private void SetupEncoding() {
      foreach (var e in Encoding.Encodings.ToArray())
        Encoding.Remove(e);


      var templateNumberTreeNodes = StructureTemplate.Tree.IterateNodesPrefix().OfType<NumberTreeNode>();
      if (templateNumberTreeNodes.Any()) {
        var templateParameterValues = templateNumberTreeNodes.Select(n => n.Value).ToArray();
        var encoding = new RealVectorEncoding(NumericParametersEncoding, templateParameterValues.Length);

        var creator = encoding.Operators.OfType<NormalDistributedRealVectorCreator>().First();
        creator.MeanParameter.Value = new RealVector(templateParameterValues);
        creator.SigmaParameter.Value = new DoubleArray(templateParameterValues.Length);
        encoding.SolutionCreator = creator;

        Encoding.Add(encoding);
      }

      foreach (var subFunction in StructureTemplate.SubFunctions) {
        subFunction.SetupVariables(ProblemData.AllowedInputVariables);
        // prevent the same encoding twice
        if (Encoding.Encodings.Any(x => x.Name == subFunction.Name)) continue;

        var encoding = new SymbolicExpressionTreeEncoding(
          subFunction.Name,
          subFunction.Grammar,
          subFunction.MaximumSymbolicExpressionTreeLength,
          subFunction.MaximumSymbolicExpressionTreeDepth);
        Encoding.Add(encoding);
      }

      //set single point crossover for numeric parameters
      var multiCrossover = (IParameterizedItem)Encoding.Operators.OfType<MultiEncodingCrossover>().First();
      foreach (var param in multiCrossover.Parameters.OfType<ConstrainedValueParameter<ICrossover>>()) {
        var singlePointCrossover = param.ValidValues.OfType<SinglePointCrossover>().FirstOrDefault();
        param.Value = singlePointCrossover ?? param.ValidValues.First();
      }

      //adapt crossover probability for subtree crossover
      foreach (var param in multiCrossover.Parameters.OfType<ConstrainedValueParameter<ICrossover>>()) {
        var subtreeCrossover = param.ValidValues.OfType<SubtreeCrossover>().FirstOrDefault();
        if (subtreeCrossover != null) {
          subtreeCrossover.CrossoverProbability = 1.0 / Encoding.Encodings.OfType<SymbolicExpressionTreeEncoding>().Count();
          param.Value = subtreeCrossover;
        }
      }

      //set multi manipulator as default manipulator for all symbolic expression tree encoding parts
      var manipulator = (IParameterizedItem)Encoding.Operators.OfType<MultiEncodingManipulator>().First();
      foreach (var param in manipulator.Parameters.OfType<ConstrainedValueParameter<IManipulator>>()) {
        var m = param.ValidValues.OfType<MultiSymbolicExpressionTreeManipulator>().FirstOrDefault();
        param.Value = m ?? param.ValidValues.First();
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

      var tree = BuildTreeFromIndividual(templateTree, individual, updateNumericParameters: StructureTemplate.ContainsNumericParameters);
      individual[SymbolicExpressionTreeName] = tree;

      if (OptimizeParameters) {
        ParameterOptimization.OptimizeTreeParameters(ProblemData, tree, interpreter: Interpreter);
      } else if (ApplyLinearScaling) {
        LinearScaling.AdjustLinearScalingParams(ProblemData, tree, Interpreter);
      }

      UpdateIndividualFromTree(tree, individual, updateNumericParameters: StructureTemplate.ContainsNumericParameters);

      //calculate NMSE
      var estimatedValues = Interpreter.GetSymbolicExpressionTreeValues(tree, ProblemData.Dataset, ProblemData.TrainingIndices);
      var boundedEstimatedValues = estimatedValues.LimitToRange(EstimationLimits.Lower, EstimationLimits.Upper);
      var targetValues = ProblemData.TargetVariableTrainingValues;
      var nmse = OnlineNormalizedMeanSquaredErrorCalculator.Calculate(targetValues, boundedEstimatedValues, out var errorState);
      if (errorState != OnlineCalculatorError.None)
        nmse = 1.0;

      //evaluate constraints
      var constraints = Enumerable.Empty<ShapeConstraint>();
      if (ProblemData is ShapeConstrainedRegressionProblemData scProbData)
        constraints = scProbData.ShapeConstraints.EnabledConstraints;
      if (constraints.Any()) {
        var boundsEstimator = new IntervalArithBoundsEstimator();
        var constraintViolations = IntervalUtil.GetConstraintViolations(constraints, boundsEstimator, ProblemData.VariableRanges, tree);

        // infinite/NaN constraints
        if (constraintViolations.Any(x => double.IsNaN(x) || double.IsInfinity(x)))
          nmse = 1.0;

        if (constraintViolations.Any(x => x > 0.0))
          nmse = 1.0;
      }

      return nmse;
    }

    private static ISymbolicExpressionTree BuildTreeFromIndividual(ISymbolicExpressionTree template, Individual individual, bool updateNumericParameters) {
      var resolvedTree = (ISymbolicExpressionTree)template.Clone();

      //set numeric parameter values
      if (updateNumericParameters) {
        var realVector = individual.RealVector(NumericParametersEncoding);
        var numberTreeNodes = resolvedTree.IterateNodesPrefix().OfType<NumberTreeNode>().ToArray();

        if (realVector.Length != numberTreeNodes.Length)
          throw new InvalidOperationException("The number of numeric parameters in the tree does not match the provided numerical values.");

        for (int i = 0; i < numberTreeNodes.Length; i++)
          numberTreeNodes[i].Value = realVector[i];
      }

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

    private static void UpdateIndividualFromTree(ISymbolicExpressionTree tree, Individual individual, bool updateNumericParameters) {
      var clonedTree = (ISymbolicExpressionTree)tree.Clone();

      foreach (var subFunctionTreeNode in clonedTree.IterateNodesPrefix().OfType<SubFunctionTreeNode>()) {
        var grammar = ((ISymbolicExpressionTree)individual[subFunctionTreeNode.Name]).Root.Grammar;
        var functionTreeNode = subFunctionTreeNode.GetSubtree(0);
        //remove function code to make numeric parameters extraction easier
        subFunctionTreeNode.RemoveSubtree(0);


        var rootNode = (SymbolicExpressionTreeTopLevelNode)new ProgramRootSymbol().CreateTreeNode();
        rootNode.SetGrammar(grammar);

        var startNode = (SymbolicExpressionTreeTopLevelNode)new StartSymbol().CreateTreeNode();
        startNode.SetGrammar(grammar);

        rootNode.AddSubtree(startNode);
        startNode.AddSubtree(functionTreeNode);
        var functionTree = new SymbolicExpressionTree(rootNode);
        individual[subFunctionTreeNode.Name] = functionTree;
      }

      //set numeric parameter values
      if (updateNumericParameters) {
        var realVector = individual.RealVector(NumericParametersEncoding);
        var numberTreeNodes = clonedTree.IterateNodesPrefix().OfType<NumberTreeNode>().ToArray();

        if (realVector.Length != numberTreeNodes.Length)
          throw new InvalidOperationException("The number of numeric parameters in the tree does not match the provided numerical values.");

        for (int i = 0; i < numberTreeNodes.Length; i++)
          realVector[i] = numberTreeNodes[i].Value;
      }
    }

    public void Load(IRegressionProblemData data) {
      ProblemData = data;
    }
  }
}
