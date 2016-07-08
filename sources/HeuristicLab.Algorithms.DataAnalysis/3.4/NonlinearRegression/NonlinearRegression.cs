#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Nonlinear regression data analysis algorithm.
  /// </summary>
  [Item("Nonlinear Regression (NLR)", "Nonlinear regression (curve fitting) data analysis algorithm (wrapper for ALGLIB).")]
  [Creatable(CreatableAttribute.Categories.DataAnalysisRegression, Priority = 120)]
  [StorableClass]
  public sealed class NonlinearRegression : FixedDataAnalysisAlgorithm<IRegressionProblem> {
    private const string LinearRegressionModelResultName = "Regression solution";
    private const string ModelStructureParameterName = "Model structure";
    private const string IterationsParameterName = "Iterations";

    public IFixedValueParameter<StringValue> ModelStructureParameter {
      get { return (IFixedValueParameter<StringValue>)Parameters[ModelStructureParameterName]; }
    }
    public IFixedValueParameter<IntValue> IterationsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[IterationsParameterName]; }
    }

    public string ModelStructure {
      get { return ModelStructureParameter.Value.Value; }
      set { ModelStructureParameter.Value.Value = value; }
    }

    public int Iterations {
      get { return IterationsParameter.Value.Value; }
      set { IterationsParameter.Value.Value = value; }
    }


    [StorableConstructor]
    private NonlinearRegression(bool deserializing) : base(deserializing) { }
    private NonlinearRegression(NonlinearRegression original, Cloner cloner)
      : base(original, cloner) {
    }
    public NonlinearRegression()
      : base() {
      Problem = new RegressionProblem();
      Parameters.Add(new FixedValueParameter<StringValue>(ModelStructureParameterName, "The function for which the parameters must be fit (only numeric constants are tuned).", new StringValue("1.0 * x*x + 0.0")));
      Parameters.Add(new FixedValueParameter<IntValue>(IterationsParameterName, "The maximum number of iterations for constants optimization.", new IntValue(200)));
    }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new NonlinearRegression(this, cloner);
    }

    #region nonlinear regression
    protected override void Run() {
      var solution = CreateRegressionSolution(Problem.ProblemData, ModelStructure, Iterations);
      Results.Add(new Result(LinearRegressionModelResultName, "The nonlinear regression solution.", solution));
      Results.Add(new Result("Root mean square error (train)", "The root of the mean of squared errors of the regression solution on the training set.", new DoubleValue(solution.TrainingRootMeanSquaredError)));
      Results.Add(new Result("Root mean square error (test)", "The root of the mean of squared errors of the regression solution on the test set.", new DoubleValue(solution.TestRootMeanSquaredError)));
    }

    public static ISymbolicRegressionSolution CreateRegressionSolution(IRegressionProblemData problemData, string modelStructure, int maxIterations) {
      var parser = new InfixExpressionParser();
      var tree = parser.Parse(modelStructure);
      var simplifier = new SymbolicDataAnalysisExpressionTreeSimplifier();
      
      if (!SymbolicRegressionConstantOptimizationEvaluator.CanOptimizeConstants(tree)) throw new ArgumentException("The optimizer does not support the specified model structure.");

      var interpreter = new SymbolicDataAnalysisExpressionTreeLinearInterpreter();
      SymbolicRegressionConstantOptimizationEvaluator.OptimizeConstants(interpreter, tree, problemData, problemData.TrainingIndices, 
        applyLinearScaling: false, maxIterations: maxIterations,
        updateVariableWeights: false, updateConstantsInTree: true);


      var scaledModel = new SymbolicRegressionModel(problemData.TargetVariable, tree, (ISymbolicDataAnalysisExpressionTreeInterpreter)interpreter.Clone());
      scaledModel.Scale(problemData);
      SymbolicRegressionSolution solution = new SymbolicRegressionSolution(scaledModel, (IRegressionProblemData)problemData.Clone());
      solution.Model.Name = "Regression Model";
      solution.Name = "Regression Solution";
      return solution;
    }
    #endregion
  }
}
