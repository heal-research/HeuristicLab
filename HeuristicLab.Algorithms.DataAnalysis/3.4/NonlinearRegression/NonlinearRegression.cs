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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Attic;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;
using HeuristicLab.Random;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Nonlinear regression data analysis algorithm.
  /// </summary>
  [Item("Nonlinear Regression (NLR)", "Nonlinear regression (curve fitting) data analysis algorithm (wrapper for ALGLIB).")]
  [Creatable(CreatableAttribute.Categories.DataAnalysisRegression, Priority = 120)]
  [StorableType("06E970EA-D366-4F46-BDC5-7156B5787BEF")]
  public sealed class NonlinearRegression : FixedDataAnalysisAlgorithm<IRegressionProblem> {
    private const string RegressionSolutionResultName = "Regression solution";
    private const string ModelStructureParameterName = "Model structure";
    private const string IterationsParameterName = "Iterations";
    private const string RestartsParameterName = "Restarts";
    private const string SetSeedRandomlyParameterName = "SetSeedRandomly";
    private const string SeedParameterName = "Seed";
    private const string InitParamsRandomlyParameterName = "InitializeParametersRandomly";
    private const string ApplyLinearScalingParameterName = "Apply linear scaling";

    public IFixedValueParameter<StringValue> ModelStructureParameter {
      get { return (IFixedValueParameter<StringValue>)Parameters[ModelStructureParameterName]; }
    }
    public IFixedValueParameter<IntValue> IterationsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[IterationsParameterName]; }
    }

    public IFixedValueParameter<BoolValue> SetSeedRandomlyParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters[SetSeedRandomlyParameterName]; }
    }

    public IFixedValueParameter<IntValue> SeedParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[SeedParameterName]; }
    }

    public IFixedValueParameter<IntValue> RestartsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[RestartsParameterName]; }
    }

    public IFixedValueParameter<BoolValue> InitParametersRandomlyParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters[InitParamsRandomlyParameterName]; }
    }

    public IFixedValueParameter<BoolValue> ApplyLinearScalingParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters[ApplyLinearScalingParameterName]; }
    }

    public string ModelStructure {
      get { return ModelStructureParameter.Value.Value; }
      set { ModelStructureParameter.Value.Value = value; }
    }

    public int Iterations {
      get { return IterationsParameter.Value.Value; }
      set { IterationsParameter.Value.Value = value; }
    }

    public int Restarts {
      get { return RestartsParameter.Value.Value; }
      set { RestartsParameter.Value.Value = value; }
    }

    public int Seed {
      get { return SeedParameter.Value.Value; }
      set { SeedParameter.Value.Value = value; }
    }

    public bool SetSeedRandomly {
      get { return SetSeedRandomlyParameter.Value.Value; }
      set { SetSeedRandomlyParameter.Value.Value = value; }
    }

    public bool InitializeParametersRandomly {
      get { return InitParametersRandomlyParameter.Value.Value; }
      set { InitParametersRandomlyParameter.Value.Value = value; }
    }

    public bool ApplyLinearScaling {
      get { return ApplyLinearScalingParameter.Value.Value; }
      set { ApplyLinearScalingParameter.Value.Value = value; }
    }

    [StorableConstructor]
    private NonlinearRegression(StorableConstructorFlag _) : base(_) { }
    private NonlinearRegression(NonlinearRegression original, Cloner cloner)
      : base(original, cloner) {
    }
    public NonlinearRegression()
      : base() {
      Problem = new RegressionProblem();
      Parameters.Add(new FixedValueParameter<StringValue>(ModelStructureParameterName, "The function for which the parameters must be fit (only numeric constants are tuned).", new StringValue("1.0 * x*x + 0.0")));
      Parameters.Add(new FixedValueParameter<IntValue>(IterationsParameterName, "The maximum number of iterations for constants optimization.", new IntValue(200)));
      Parameters.Add(new FixedValueParameter<IntValue>(RestartsParameterName, "The number of independent random restarts (>0)", new IntValue(10)));
      Parameters.Add(new FixedValueParameter<IntValue>(SeedParameterName, "The PRNG seed value.", new IntValue()));
      Parameters.Add(new FixedValueParameter<BoolValue>(SetSeedRandomlyParameterName, "Switch to determine if the random number seed should be initialized randomly.", new BoolValue(true)));
      Parameters.Add(new FixedValueParameter<BoolValue>(InitParamsRandomlyParameterName, "Switch to determine if the real-valued model parameters should be initialized randomly in each restart.", new BoolValue(false)));
      Parameters.Add(new FixedValueParameter<BoolValue>(ApplyLinearScalingParameterName, "Switch to determine if linear scaling terms should be added to the model", new BoolValue(true)));

      SetParameterHiddenState();

      InitParametersRandomlyParameter.Value.ValueChanged += (sender, args) => {
        SetParameterHiddenState();
      };
    }

    private void SetParameterHiddenState() {
      var hide = !InitializeParametersRandomly;
      RestartsParameter.Hidden = hide;
      SeedParameter.Hidden = hide;
      SetSeedRandomlyParameter.Hidden = hide;
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code, remove with 3.4
      if (!Parameters.ContainsKey(RestartsParameterName))
        Parameters.Add(new FixedValueParameter<IntValue>(RestartsParameterName, "The number of independent random restarts", new IntValue(1)));
      if (!Parameters.ContainsKey(SeedParameterName))
        Parameters.Add(new FixedValueParameter<IntValue>(SeedParameterName, "The PRNG seed value.", new IntValue()));
      if (!Parameters.ContainsKey(SetSeedRandomlyParameterName))
        Parameters.Add(new FixedValueParameter<BoolValue>(SetSeedRandomlyParameterName, "Switch to determine if the random number seed should be initialized randomly.", new BoolValue(true)));
      if (!Parameters.ContainsKey(InitParamsRandomlyParameterName))
        Parameters.Add(new FixedValueParameter<BoolValue>(InitParamsRandomlyParameterName, "Switch to determine if the numeric parameters of the model should be initialized randomly.", new BoolValue(false)));
      if (!Parameters.ContainsKey(ApplyLinearScalingParameterName))
        Parameters.Add(new FixedValueParameter<BoolValue>(ApplyLinearScalingParameterName, "Switch to determine if linear scaling terms should be added to the model", new BoolValue(true)));


      SetParameterHiddenState();
      InitParametersRandomlyParameter.Value.ValueChanged += (sender, args) => {
        SetParameterHiddenState();
      };
      #endregion
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new NonlinearRegression(this, cloner);
    }

    #region nonlinear regression
    protected override void Run(CancellationToken cancellationToken) {
      IRegressionSolution bestSolution = null;
      if (InitializeParametersRandomly) {
        var qualityTable = new DataTable("RMSE table");
        qualityTable.VisualProperties.YAxisLogScale = true;
        var trainRMSERow = new DataRow("RMSE (train)");
        trainRMSERow.VisualProperties.ChartType = DataRowVisualProperties.DataRowChartType.Points;
        var testRMSERow = new DataRow("RMSE test");
        testRMSERow.VisualProperties.ChartType = DataRowVisualProperties.DataRowChartType.Points;

        qualityTable.Rows.Add(trainRMSERow);
        qualityTable.Rows.Add(testRMSERow);
        Results.Add(new Result(qualityTable.Name, qualityTable.Name + " for all restarts", qualityTable));
        if (SetSeedRandomly) Seed = RandomSeedGenerator.GetSeed();
        var rand = new MersenneTwister((uint)Seed);
        bestSolution = CreateRegressionSolution(Problem.ProblemData, ModelStructure, Iterations, ApplyLinearScaling, rand);
        trainRMSERow.Values.Add(bestSolution.TrainingRootMeanSquaredError);
        testRMSERow.Values.Add(bestSolution.TestRootMeanSquaredError);
        for (int r = 0; r < Restarts; r++) {
          var solution = CreateRegressionSolution(Problem.ProblemData, ModelStructure, Iterations, ApplyLinearScaling, rand);
          trainRMSERow.Values.Add(solution.TrainingRootMeanSquaredError);
          testRMSERow.Values.Add(solution.TestRootMeanSquaredError);
          if (solution.TrainingRootMeanSquaredError < bestSolution.TrainingRootMeanSquaredError) {
            bestSolution = solution;
          }
        }
      } else {
        bestSolution = CreateRegressionSolution(Problem.ProblemData, ModelStructure, Iterations, ApplyLinearScaling);
      }

      Results.Add(new Result(RegressionSolutionResultName, "The nonlinear regression solution.", bestSolution));
      Results.Add(new Result("Root mean square error (train)", "The root of the mean of squared errors of the regression solution on the training set.", new DoubleValue(bestSolution.TrainingRootMeanSquaredError)));
      Results.Add(new Result("Root mean square error (test)", "The root of the mean of squared errors of the regression solution on the test set.", new DoubleValue(bestSolution.TestRootMeanSquaredError)));

    }

    /// <summary>
    /// Fits a model to the data by optimizing the numeric constants.
    /// Model is specified as infix expression containing variable names and numbers. 
    /// The starting point for the numeric constants is initialized randomly if a random number generator is specified (~N(0,1)). Otherwise the user specified constants are
    /// used as a starting point. 
    /// </summary>-
    /// <param name="problemData">Training and test data</param>
    /// <param name="modelStructure">The function as infix expression</param>
    /// <param name="maxIterations">Number of constant optimization iterations (using Levenberg-Marquardt algorithm)</param>
    /// <param name="random">Optional random number generator for random initialization of numeric constants.</param>
    /// <returns></returns>
    public static ISymbolicRegressionSolution CreateRegressionSolution(IRegressionProblemData problemData, string modelStructure, int maxIterations, bool applyLinearScaling, IRandom rand = null) {
      var parser = new InfixExpressionParser();
      var tree = parser.Parse(modelStructure);
      // parser handles double and string variables equally by creating a VariableTreeNode
      // post-process to replace VariableTreeNodes by FactorVariableTreeNodes for all string variables
      var factorSymbol = new FactorVariable();
      factorSymbol.VariableNames =
        problemData.AllowedInputVariables.Where(name => problemData.Dataset.VariableHasType<string>(name));
      factorSymbol.AllVariableNames = factorSymbol.VariableNames;
      factorSymbol.VariableValues =
        factorSymbol.VariableNames.Select(name =>
        new KeyValuePair<string, Dictionary<string, int>>(name,
        problemData.Dataset.GetReadOnlyStringValues(name).Distinct()
        .Select((n, i) => Tuple.Create(n, i))
        .ToDictionary(tup => tup.Item1, tup => tup.Item2)));

      foreach (var parent in tree.IterateNodesPrefix().ToArray()) {
        for (int i = 0; i < parent.SubtreeCount; i++) {
          var varChild = parent.GetSubtree(i) as VariableTreeNode;
          var factorVarChild = parent.GetSubtree(i) as FactorVariableTreeNode;
          if (varChild != null && factorSymbol.VariableNames.Contains(varChild.VariableName)) {
            parent.RemoveSubtree(i);
            var factorTreeNode = (FactorVariableTreeNode)factorSymbol.CreateTreeNode();
            factorTreeNode.VariableName = varChild.VariableName;
            factorTreeNode.Weights =
              factorTreeNode.Symbol.GetVariableValues(factorTreeNode.VariableName).Select(_ => 1.0).ToArray();
            // weight = 1.0 for each value
            parent.InsertSubtree(i, factorTreeNode);
          } else if (factorVarChild != null && factorSymbol.VariableNames.Contains(factorVarChild.VariableName)) {
            if (factorSymbol.GetVariableValues(factorVarChild.VariableName).Count() != factorVarChild.Weights.Length)
              throw new ArgumentException(
                string.Format("Factor variable {0} needs exactly {1} weights",
                factorVarChild.VariableName,
                factorSymbol.GetVariableValues(factorVarChild.VariableName).Count()));
            parent.RemoveSubtree(i);
            var factorTreeNode = (FactorVariableTreeNode)factorSymbol.CreateTreeNode();
            factorTreeNode.VariableName = factorVarChild.VariableName;
            factorTreeNode.Weights = factorVarChild.Weights;
            parent.InsertSubtree(i, factorTreeNode);
          }
        }
      }

      if (!SymbolicRegressionConstantOptimizationEvaluator.CanOptimizeConstants(tree)) throw new ArgumentException("The optimizer does not support the specified model structure.");

      // initialize constants randomly
      if (rand != null) {
        foreach (var node in tree.IterateNodesPrefix().OfType<ConstantTreeNode>()) {
          double f = Math.Exp(NormalDistributedRandom.NextDouble(rand, 0, 1));
          double s = rand.NextDouble() < 0.5 ? -1 : 1;
          node.Value = s * node.Value * f;
        }
      }
      var interpreter = new SymbolicDataAnalysisExpressionTreeLinearInterpreter();

      SymbolicRegressionConstantOptimizationEvaluator.OptimizeConstants(interpreter, tree, problemData, problemData.TrainingIndices,
        applyLinearScaling: applyLinearScaling, maxIterations: maxIterations,
        updateVariableWeights: false, updateConstantsInTree: true);

      var model = new SymbolicRegressionModel(problemData.TargetVariable, tree, (ISymbolicDataAnalysisExpressionTreeInterpreter)interpreter.Clone());
      if (applyLinearScaling)
        model.Scale(problemData);

      SymbolicRegressionSolution solution = new SymbolicRegressionSolution(model, (IRegressionProblemData)problemData.Clone());
      solution.Model.Name = "Regression Model";
      solution.Name = "Regression Solution";
      return solution;
    }
    #endregion
  }
}
