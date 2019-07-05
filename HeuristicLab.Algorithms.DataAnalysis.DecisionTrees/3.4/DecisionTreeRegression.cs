#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Random;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableType("FC8D8E5A-D16D-41BB-91CF-B2B35D17ADD7")]
  [Creatable(CreatableAttribute.Categories.DataAnalysisRegression, Priority = 95)]
  [Item("Decision tree regression", "A regression tree / rule set learner")]
  public sealed class DecisionTreeRegression : FixedDataAnalysisAlgorithm<IRegressionProblem> {
    public override bool SupportsPause {
      get { return true; }
    }

    public const string RegressionTreeParameterVariableName = "RegressionTreeParameters";
    public const string ModelVariableName = "Model";
    public const string PruningSetVariableName = "PruningSet";
    public const string TrainingSetVariableName = "TrainingSet";

    #region Parameter names
    private const string GenerateRulesParameterName = "GenerateRules";
    private const string HoldoutSizeParameterName = "HoldoutSize";
    private const string SplitterParameterName = "Splitter";
    private const string MinimalNodeSizeParameterName = "MinimalNodeSize";
    private const string LeafModelParameterName = "LeafModel";
    private const string PruningTypeParameterName = "PruningType";
    private const string SeedParameterName = "Seed";
    private const string SetSeedRandomlyParameterName = "SetSeedRandomly";
    private const string UseHoldoutParameterName = "UseHoldout";
    #endregion

    #region Parameter properties
    public IFixedValueParameter<BoolValue> GenerateRulesParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters[GenerateRulesParameterName]; }
    }
    public IFixedValueParameter<PercentValue> HoldoutSizeParameter {
      get { return (IFixedValueParameter<PercentValue>)Parameters[HoldoutSizeParameterName]; }
    }
    public IConstrainedValueParameter<ISplitter> SplitterParameter {
      get { return (IConstrainedValueParameter<ISplitter>)Parameters[SplitterParameterName]; }
    }
    public IFixedValueParameter<IntValue> MinimalNodeSizeParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[MinimalNodeSizeParameterName]; }
    }
    public IConstrainedValueParameter<ILeafModel> LeafModelParameter {
      get { return (IConstrainedValueParameter<ILeafModel>)Parameters[LeafModelParameterName]; }
    }
    public IConstrainedValueParameter<IPruning> PruningTypeParameter {
      get { return (IConstrainedValueParameter<IPruning>)Parameters[PruningTypeParameterName]; }
    }
    public IFixedValueParameter<IntValue> SeedParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[SeedParameterName]; }
    }
    public IFixedValueParameter<BoolValue> SetSeedRandomlyParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters[SetSeedRandomlyParameterName]; }
    }
    public IFixedValueParameter<BoolValue> UseHoldoutParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters[UseHoldoutParameterName]; }
    }
    #endregion

    #region Properties 
    public bool GenerateRules {
      get { return GenerateRulesParameter.Value.Value; }
      set { GenerateRulesParameter.Value.Value = value; }
    }
    public double HoldoutSize {
      get { return HoldoutSizeParameter.Value.Value; }
      set { HoldoutSizeParameter.Value.Value = value; }
    }
    public ISplitter Splitter {
      get { return SplitterParameter.Value; }
      // no setter because this is a constrained parameter
    }
    public int MinimalNodeSize {
      get { return MinimalNodeSizeParameter.Value.Value; }
      set { MinimalNodeSizeParameter.Value.Value = value; }
    }
    public ILeafModel LeafModel {
      get { return LeafModelParameter.Value; }
    }
    public IPruning Pruning {
      get { return PruningTypeParameter.Value; }
    }
    public int Seed {
      get { return SeedParameter.Value.Value; }
      set { SeedParameter.Value.Value = value; }
    }
    public bool SetSeedRandomly {
      get { return SetSeedRandomlyParameter.Value.Value; }
      set { SetSeedRandomlyParameter.Value.Value = value; }
    }
    public bool UseHoldout {
      get { return UseHoldoutParameter.Value.Value; }
      set { UseHoldoutParameter.Value.Value = value; }
    }
    #endregion

    #region State
    [Storable]
    private IScope stateScope;
    #endregion

    #region Constructors and Cloning
    [StorableConstructor]
    private DecisionTreeRegression(StorableConstructorFlag _) : base(_) { }
    private DecisionTreeRegression(DecisionTreeRegression original, Cloner cloner) : base(original, cloner) {
      stateScope = cloner.Clone(stateScope);
    }
    public DecisionTreeRegression() {
      var modelSet = new ItemSet<ILeafModel>(ApplicationManager.Manager.GetInstances<ILeafModel>());
      var pruningSet = new ItemSet<IPruning>(ApplicationManager.Manager.GetInstances<IPruning>());
      var splitterSet = new ItemSet<ISplitter>(ApplicationManager.Manager.GetInstances<ISplitter>());
      Parameters.Add(new FixedValueParameter<BoolValue>(GenerateRulesParameterName, "Whether a set of rules or a decision tree shall be created (default=false)", new BoolValue(false)));
      Parameters.Add(new FixedValueParameter<PercentValue>(HoldoutSizeParameterName, "How much of the training set shall be reserved for pruning (default=20%).", new PercentValue(0.2)));
      Parameters.Add(new ConstrainedValueParameter<ISplitter>(SplitterParameterName, "The type of split function used to create node splits (default='Splitter').", splitterSet, splitterSet.OfType<Splitter>().First()));
      Parameters.Add(new FixedValueParameter<IntValue>(MinimalNodeSizeParameterName, "The minimal number of samples in a leaf node (default=1).", new IntValue(1)));
      Parameters.Add(new ConstrainedValueParameter<ILeafModel>(LeafModelParameterName, "The type of model used for the nodes (default='LinearLeaf').", modelSet, modelSet.OfType<LinearLeaf>().First()));
      Parameters.Add(new ConstrainedValueParameter<IPruning>(PruningTypeParameterName, "The type of pruning used (default='ComplexityPruning').", pruningSet, pruningSet.OfType<ComplexityPruning>().First()));
      Parameters.Add(new FixedValueParameter<IntValue>(SeedParameterName, "The random seed used to initialize the new pseudo random number generator.", new IntValue(0)));
      Parameters.Add(new FixedValueParameter<BoolValue>(SetSeedRandomlyParameterName, "True if the random seed should be set to a random value, otherwise false.", new BoolValue(true)));
      Parameters.Add(new FixedValueParameter<BoolValue>(UseHoldoutParameterName, "True if a holdout set should be generated, false if splitting and pruning shall be performed on the same data (default=false).", new BoolValue(false)));
      Problem = new RegressionProblem();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new DecisionTreeRegression(this, cloner);
    }
    #endregion

    protected override void Initialize(CancellationToken cancellationToken) {
      base.Initialize(cancellationToken);
      var random = new MersenneTwister();
      if (SetSeedRandomly) Seed = RandomSeedGenerator.GetSeed();
      random.Reset(Seed);
      stateScope = InitializeScope(random, Problem.ProblemData, Pruning, MinimalNodeSize, LeafModel, Splitter, GenerateRules, UseHoldout, HoldoutSize);
      stateScope.Variables.Add(new Variable("Algorithm", this));
      Results.AddOrUpdateResult("StateScope", stateScope);
    }

    protected override void Run(CancellationToken cancellationToken) {
      var model = Build(stateScope, Results, cancellationToken);
      AnalyzeSolution(model.CreateRegressionSolution(Problem.ProblemData), Results, Problem.ProblemData);
    }

    #region Static Interface
    public static IRegressionSolution CreateRegressionSolution(IRegressionProblemData problemData, IRandom random, ILeafModel leafModel = null, ISplitter splitter = null, IPruning pruning = null,
      bool useHoldout = false, double holdoutSize = 0.2, int minimumLeafSize = 1, bool generateRules = false, ResultCollection results = null, CancellationToken? cancellationToken = null) {
      if (leafModel == null) leafModel = new LinearLeaf();
      if (splitter == null) splitter = new Splitter();
      if (cancellationToken == null) cancellationToken = CancellationToken.None;
      if (pruning == null) pruning = new ComplexityPruning();

      var stateScope = InitializeScope(random, problemData, pruning, minimumLeafSize, leafModel, splitter, generateRules, useHoldout, holdoutSize);
      var model = Build(stateScope, results, cancellationToken.Value);
      return model.CreateRegressionSolution(problemData);
    }

    public static void UpdateModel(IDecisionTreeModel model, IRegressionProblemData problemData, IRandom random, ILeafModel leafModel, CancellationToken? cancellationToken = null) {
      if (cancellationToken == null) cancellationToken = CancellationToken.None;
      var regressionTreeParameters = new RegressionTreeParameters(leafModel, problemData, random);
      var scope = new Scope();
      scope.Variables.Add(new Variable(RegressionTreeParameterVariableName, regressionTreeParameters));
      leafModel.Initialize(scope);
      model.Update(problemData.TrainingIndices.ToList(), scope, cancellationToken.Value);
    }
    #endregion

    #region Helpers
    private static IScope InitializeScope(IRandom random, IRegressionProblemData problemData, IPruning pruning, int minLeafSize, ILeafModel leafModel, ISplitter splitter, bool generateRules, bool useHoldout, double holdoutSize) {
      var stateScope = new Scope("RegressionTreeStateScope");

      //reduce RegressionProblemData to AllowedInput & Target column wise and to TrainingSet row wise
      var doubleVars = new HashSet<string>(problemData.Dataset.DoubleVariables);
      var vars = problemData.AllowedInputVariables.Concat(new[] {problemData.TargetVariable}).ToArray();
      if (vars.Any(v => !doubleVars.Contains(v))) throw new NotSupportedException("Decision tree regression supports only double valued input or output features.");
      var doubles = vars.Select(v => problemData.Dataset.GetDoubleValues(v, problemData.TrainingIndices).ToArray()).ToArray();
      if (doubles.Any(v => v.Any(x => double.IsNaN(x) || double.IsInfinity(x))))
        throw new NotSupportedException("Decision tree regression does not support NaN or infinity values in the input dataset.");
      var trainingData = new Dataset(vars, doubles);
      var pd = new RegressionProblemData(trainingData, problemData.AllowedInputVariables, problemData.TargetVariable);
      pd.TrainingPartition.End = pd.TestPartition.Start = pd.TestPartition.End = pd.Dataset.Rows;
      pd.TrainingPartition.Start = 0;

      //store regression tree parameters
      var regressionTreeParams = new RegressionTreeParameters(pruning, minLeafSize, leafModel, pd, random, splitter);
      stateScope.Variables.Add(new Variable(RegressionTreeParameterVariableName, regressionTreeParams));

      //initialize tree operators
      pruning.Initialize(stateScope);
      splitter.Initialize(stateScope);
      leafModel.Initialize(stateScope);

      //store unbuilt model
      IItem model;
      if (generateRules) {
        model = RegressionRuleSetModel.CreateRuleModel(problemData.TargetVariable, regressionTreeParams);
        RegressionRuleSetModel.Initialize(stateScope);
      }
      else {
        model = RegressionNodeTreeModel.CreateTreeModel(problemData.TargetVariable, regressionTreeParams);
      }
      stateScope.Variables.Add(new Variable(ModelVariableName, model));

      //store training & pruning indices
      IReadOnlyList<int> trainingSet, pruningSet;
      GeneratePruningSet(pd.TrainingIndices.ToArray(), random, useHoldout, holdoutSize, out trainingSet, out pruningSet);
      stateScope.Variables.Add(new Variable(TrainingSetVariableName, new IntArray(trainingSet.ToArray())));
      stateScope.Variables.Add(new Variable(PruningSetVariableName, new IntArray(pruningSet.ToArray())));

      return stateScope;
    }

    private static IRegressionModel Build(IScope stateScope, ResultCollection results, CancellationToken cancellationToken) {
      var regressionTreeParams = (RegressionTreeParameters)stateScope.Variables[RegressionTreeParameterVariableName].Value;
      var model = (IDecisionTreeModel)stateScope.Variables[ModelVariableName].Value;
      var trainingRows = (IntArray)stateScope.Variables[TrainingSetVariableName].Value;
      var pruningRows = (IntArray)stateScope.Variables[PruningSetVariableName].Value;
      if (1 > trainingRows.Length)
        return new PreconstructedLinearModel(new Dictionary<string, double>(), 0, regressionTreeParams.TargetVariable);
      if (regressionTreeParams.MinLeafSize > trainingRows.Length) {
        var targets = regressionTreeParams.Data.GetDoubleValues(regressionTreeParams.TargetVariable).ToArray();
        return new PreconstructedLinearModel(new Dictionary<string, double>(), targets.Average(), regressionTreeParams.TargetVariable);
      }
      model.Build(trainingRows.ToArray(), pruningRows.ToArray(), stateScope, results, cancellationToken);
      return model;
    }

    private static void GeneratePruningSet(IReadOnlyList<int> allrows, IRandom random, bool useHoldout, double holdoutSize, out IReadOnlyList<int> training, out IReadOnlyList<int> pruning) {
      if (!useHoldout) {
        training = allrows;
        pruning = allrows;
        return;
      }
      var perm = new Permutation(PermutationTypes.Absolute, allrows.Count, random);
      var cut = (int)(holdoutSize * allrows.Count);
      pruning = perm.Take(cut).Select(i => allrows[i]).ToArray();
      training = perm.Take(cut).Select(i => allrows[i]).ToArray();
    }

    private void AnalyzeSolution(IRegressionSolution solution, ResultCollection results, IRegressionProblemData problemData) {
      results.Add(new Result("RegressionSolution", (IItem)solution.Clone()));

      Dictionary<string, int> frequencies = null;

      var tree = solution.Model as RegressionNodeTreeModel;
      if (tree != null) {
        results.Add(RegressionTreeAnalyzer.CreateLeafDepthHistogram(tree));
        frequencies = RegressionTreeAnalyzer.GetTreeVariableFrequences(tree);
        RegressionTreeAnalyzer.AnalyzeNodes(tree, results, problemData);
      }

      var ruleSet = solution.Model as RegressionRuleSetModel;
      if (ruleSet != null) {
        results.Add(RegressionTreeAnalyzer.CreateRulesResult(ruleSet, problemData, "Rules", true));
        frequencies = RegressionTreeAnalyzer.GetRuleVariableFrequences(ruleSet);
        results.Add(RegressionTreeAnalyzer.CreateCoverageDiagram(ruleSet, problemData));
      }

      //Variable frequencies
      if (frequencies != null) {
        var sum = frequencies.Values.Sum();
        sum = sum == 0 ? 1 : sum;
        var impactArray = new DoubleArray(frequencies.Select(i => (double)i.Value / sum).ToArray()) {
          ElementNames = frequencies.Select(i => i.Key)
        };
        results.Add(new Result("Variable Frequences", "relative frequencies of variables in rules and tree nodes", impactArray));
      }

      var pruning = Pruning as ComplexityPruning;
      if (pruning != null && tree != null)
        RegressionTreeAnalyzer.PruningChart(tree, pruning, results);
    }
    #endregion
  }
}