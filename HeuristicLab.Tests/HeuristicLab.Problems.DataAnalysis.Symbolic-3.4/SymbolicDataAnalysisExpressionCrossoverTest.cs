#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Diagnostics;
using System.Linq;
using System.Threading;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;
using HeuristicLab.Random;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ExecutionContext = HeuristicLab.Core.ExecutionContext;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Tests {
  [TestClass()]
  public class SymbolicDataAnalysisExpressionCrossoverTest {
    private const int PopulationSize = 10000;
    private const int MaxTreeDepth = 10;
    private const int MaxTreeLength = 100;
    private const int Rows = 1000;
    private const int Columns = 50;

    /// <summary>
    ///Gets or sets the test context which provides
    ///information about and functionality for the current test run.
    ///</summary>
    public TestContext TestContext { get; set; }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "long")]
    public void SymbolicDataAnalysisExpressionSemanticSimilarityCrossoverPerformanceTest() {
      var problem = new SymbolicRegressionSingleObjectiveProblem();
      var crossover = problem.OperatorsParameter.Value.OfType<SymbolicDataAnalysisExpressionSemanticSimilarityCrossover<IRegressionProblemData>>().First();
      SymbolicDataAnalysisCrossoverPerformanceTest(crossover);
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "long")]
    public void SymbolicDataAnalysisExpressionProbabilisticFunctionalCrossoverPerformanceTest() {
      var problem = new SymbolicRegressionSingleObjectiveProblem();
      var crossover = problem.OperatorsParameter.Value.OfType<SymbolicDataAnalysisExpressionProbabilisticFunctionalCrossover<IRegressionProblemData>>().First();
      SymbolicDataAnalysisCrossoverPerformanceTest(crossover);
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "long")]
    public void SymbolicDataAnalysisExpressionDeterministicBestCrossoverPerformanceTest() {
      var problem = new SymbolicRegressionSingleObjectiveProblem();
      var crossover = problem.OperatorsParameter.Value.OfType<SymbolicDataAnalysisExpressionDeterministicBestCrossover<IRegressionProblemData>>().First();
      SymbolicDataAnalysisCrossoverPerformanceTest(crossover);
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "long")]
    public void SymbolicDataAnalysisExpressionContextAwareCrossoverPerformanceTest() {
      var problem = new SymbolicRegressionSingleObjectiveProblem();
      var crossover = problem.OperatorsParameter.Value.OfType<SymbolicDataAnalysisExpressionContextAwareCrossover<IRegressionProblemData>>().First();
      SymbolicDataAnalysisCrossoverPerformanceTest(crossover);
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "long")]
    public void SymbolicDataAnalysisExpressionDepthConstrainedCrossoverPerformanceTest() {
      var problem = new SymbolicRegressionSingleObjectiveProblem();
      var crossover = problem.OperatorsParameter.Value.OfType<SymbolicDataAnalysisExpressionDepthConstrainedCrossover<IRegressionProblemData>>().First();

      crossover.DepthRangeParameter.Value = crossover.DepthRangeParameter.ValidValues.First(s => s.Value == "HighLevel");
      SymbolicDataAnalysisCrossoverPerformanceTest(crossover);
      crossover.DepthRangeParameter.Value = crossover.DepthRangeParameter.ValidValues.First(s => s.Value == "Standard");
      SymbolicDataAnalysisCrossoverPerformanceTest(crossover);
      crossover.DepthRangeParameter.Value = crossover.DepthRangeParameter.ValidValues.First(s => s.Value == "LowLevel");
      SymbolicDataAnalysisCrossoverPerformanceTest(crossover);
    }


    private static void SymbolicDataAnalysisCrossoverPerformanceTest(ISymbolicDataAnalysisExpressionCrossover<IRegressionProblemData> crossover) {
      var twister = new MersenneTwister(31415);
      var dataset = Util.CreateRandomDataset(twister, Rows, Columns);
      var grammar = new FullFunctionalExpressionGrammar();
      var stopwatch = new Stopwatch();

      grammar.MaximumFunctionArguments = 0;
      grammar.MaximumFunctionDefinitions = 0;
      grammar.MinimumFunctionArguments = 0;
      grammar.MinimumFunctionDefinitions = 0;

      var trees = Util.CreateRandomTrees(twister, dataset, grammar, PopulationSize, 1, MaxTreeLength, 0, 0);
      foreach (ISymbolicExpressionTree tree in trees) {
        Util.InitTree(tree, twister, new List<string>(dataset.VariableNames));
      }
      var problemData = new RegressionProblemData(dataset, dataset.VariableNames, dataset.VariableNames.Last());
      var problem = new SymbolicRegressionSingleObjectiveProblem();
      problem.ProblemData = problemData;

      var globalScope = new Scope("Global Scope");
      globalScope.Variables.Add(new Core.Variable("Random", twister));
      var context = new ExecutionContext(null, problem, globalScope);
      context = new ExecutionContext(context, crossover, globalScope);

      stopwatch.Start();
      for (int i = 0; i != PopulationSize; ++i) {
        var parent0 = (ISymbolicExpressionTree)trees.SampleRandom(twister).Clone();
        var scopeParent0 = new Scope();
        scopeParent0.Variables.Add(new Core.Variable(crossover.ParentsParameter.ActualName, parent0));
        context.Scope.SubScopes.Add(scopeParent0);

        var parent1 = (ISymbolicExpressionTree)trees.SampleRandom(twister).Clone();
        var scopeParent1 = new Scope();
        scopeParent1.Variables.Add(new Core.Variable(crossover.ParentsParameter.ActualName, parent1));
        context.Scope.SubScopes.Add(scopeParent1);

        crossover.Execute(context, new CancellationToken());

        context.Scope.SubScopes.Remove(scopeParent0); // clean the scope in preparation for the next iteration
        context.Scope.SubScopes.Remove(scopeParent1); // clean the scope in preparation for the next iteration
      }
      stopwatch.Stop();
      double msPerCrossover = 2 * stopwatch.ElapsedMilliseconds / (double)PopulationSize;
      Console.WriteLine(crossover.Name + ": " + Environment.NewLine +
                        msPerCrossover + " ms per crossover (~" + Math.Round(1000.0 / (msPerCrossover)) + " crossover operations / s)");

      foreach (var tree in trees)
        HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Tests.Util.IsValid(tree);
    }
  }
}
