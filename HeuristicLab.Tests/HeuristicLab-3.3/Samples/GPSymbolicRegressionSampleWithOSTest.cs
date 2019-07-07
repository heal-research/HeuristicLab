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
using System.IO;
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Algorithms.OffspringSelectionGeneticAlgorithm;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;
using HeuristicLab.Problems.Instances.DataAnalysis;
using HeuristicLab.Selection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class GPSymbolicRegressionSampleWithOSTest {
    private const string SampleFileName = "OSGP_SymReg";
    private const int seed = 12345;

    private static readonly ProtoBufSerializer serializer = new ProtoBufSerializer();

    [TestMethod]
    [TestCategory("Samples.Create")]
    [TestProperty("Time", "medium")]
    public void CreateGPSymbolicRegressionSampleWithOSTest() {
      var osga = CreateGpSymbolicRegressionSample();
      string path = Path.Combine(SamplesUtils.SamplesDirectory, SampleFileName + SamplesUtils.SampleFileExtension);
      serializer.Serialize(osga, path);
    }
    [TestMethod]
    [TestCategory("Samples.Execute")]
    [TestProperty("Time", "long")]
    public void RunGPSymbolicRegressionSampleWithOSTest() {
      var osga = CreateGpSymbolicRegressionSample();
      osga.SetSeedRandomly.Value = false;
      osga.Seed.Value = seed;
      osga.MaximumGenerations.Value = 10; //reduce unit test runtime

      SamplesUtils.RunAlgorithm(osga);
      var bestTrainingSolution = (IRegressionSolution)osga.Results["Best training solution"].Value;

      if (Environment.Is64BitProcess) {
        // the following are the result values as produced on builder.heuristiclab.com
        // Unfortunately, running the same test on a different machine results in different values
        // For x86 environments the results below match but on x64 there is a difference
        // We tracked down the ConstantOptimizationEvaluator as a possible cause but have not
        // been able to identify the real cause. Presumably, execution on a Xeon and a Core i7 processor 
        // leads to different results. 
        Assert.AreEqual(0.99174959007940156, SamplesUtils.GetDoubleResult(osga, "BestQuality"), 1E-8, Environment.NewLine + "Best Quality differs.");
        Assert.AreEqual(0.9836083751914968, SamplesUtils.GetDoubleResult(osga, "CurrentAverageQuality"), 1E-8, Environment.NewLine + "Current Average Quality differs.");
        Assert.AreEqual(0.98298394717065463, SamplesUtils.GetDoubleResult(osga, "CurrentWorstQuality"), 1E-8, Environment.NewLine + "Current Worst Quality differs.");
        Assert.AreEqual(10100, SamplesUtils.GetIntResult(osga, "EvaluatedSolutions"), Environment.NewLine + "Evaluated Solutions differ.");
        Assert.AreEqual(0.99174959007940156, bestTrainingSolution.TrainingRSquared, 1E-8, Environment.NewLine + "Best Training Solution Training R² differs.");
        Assert.AreEqual(0.8962902319942232, bestTrainingSolution.TestRSquared, 1E-8, Environment.NewLine + "Best Training Solution Test R² differs.");
      } else {
        Assert.AreEqual(0.9971536312165723, SamplesUtils.GetDoubleResult(osga, "BestQuality"), 1E-8, Environment.NewLine + "Best Qualitiy differs.");
        Assert.AreEqual(0.98382832370544937, SamplesUtils.GetDoubleResult(osga, "CurrentAverageQuality"), 1E-8, Environment.NewLine + "Current Average Quality differs.");
        Assert.AreEqual(0.960805603777699, SamplesUtils.GetDoubleResult(osga, "CurrentWorstQuality"), 1E-8, Environment.NewLine + "Current Worst Quality differs.");
        Assert.AreEqual(10500, SamplesUtils.GetIntResult(osga, "EvaluatedSolutions"), Environment.NewLine + "Evaluated Solutions differ.");
        Assert.AreEqual(0.9971536312165723, bestTrainingSolution.TrainingRSquared, 1E-8, Environment.NewLine + "Best Training Solution Training R² differs.");
        Assert.AreEqual(0.010190137960908724, bestTrainingSolution.TestRSquared, 1E-8, Environment.NewLine + "Best Training Solution Test R² differs.");
      }
    }

    private OffspringSelectionGeneticAlgorithm CreateGpSymbolicRegressionSample() {
      var osga = new OffspringSelectionGeneticAlgorithm();
      #region Problem Configuration
      var provider = new VariousInstanceProvider(seed);
      var instance = provider.GetDataDescriptors().First(x => x.Name.StartsWith("Spatial co-evolution"));
      var problemData = (RegressionProblemData)provider.LoadData(instance);
      var problem = new SymbolicRegressionSingleObjectiveProblem();
      problem.ProblemData = problemData;
      problem.Load(problemData);
      problem.BestKnownQuality.Value = 1.0;

      #region configure grammar

      var grammar = (TypeCoherentExpressionGrammar)problem.SymbolicExpressionTreeGrammar;
      grammar.ConfigureAsDefaultRegressionGrammar();

      //symbols square, power, squareroot, root, log, exp, sine, cosine, tangent, variable
      var square = grammar.Symbols.OfType<Square>().Single();
      var power = grammar.Symbols.OfType<Power>().Single();
      var squareroot = grammar.Symbols.OfType<SquareRoot>().Single();
      var root = grammar.Symbols.OfType<Root>().Single();
      var cube = grammar.Symbols.OfType<Cube>().Single();
      var cuberoot = grammar.Symbols.OfType<CubeRoot>().Single();
      var log = grammar.Symbols.OfType<Logarithm>().Single();
      var exp = grammar.Symbols.OfType<Exponential>().Single();
      var sine = grammar.Symbols.OfType<Sine>().Single();
      var cosine = grammar.Symbols.OfType<Cosine>().Single();
      var tangent = grammar.Symbols.OfType<Tangent>().Single();
      var variable = grammar.Symbols.OfType<Variable>().Single();
      var powerSymbols = grammar.Symbols.Single(s => s.Name == "Power Functions");
      powerSymbols.Enabled = true;

      square.Enabled = true;
      square.InitialFrequency = 1.0;
      foreach (var allowed in grammar.GetAllowedChildSymbols(square))
        grammar.RemoveAllowedChildSymbol(square, allowed);
      foreach (var allowed in grammar.GetAllowedChildSymbols(square, 0))
        grammar.RemoveAllowedChildSymbol(square, allowed, 0);
      grammar.AddAllowedChildSymbol(square, variable);

      power.Enabled = false;

      squareroot.Enabled = false;
      foreach (var allowed in grammar.GetAllowedChildSymbols(squareroot))
        grammar.RemoveAllowedChildSymbol(squareroot, allowed);
      foreach (var allowed in grammar.GetAllowedChildSymbols(squareroot, 0))
        grammar.RemoveAllowedChildSymbol(squareroot, allowed, 0);
      grammar.AddAllowedChildSymbol(squareroot, variable);

      cube.Enabled = false;
      cuberoot.Enabled = false;
      root.Enabled = false;

      log.Enabled = true;
      log.InitialFrequency = 1.0;
      foreach (var allowed in grammar.GetAllowedChildSymbols(log))
        grammar.RemoveAllowedChildSymbol(log, allowed);
      foreach (var allowed in grammar.GetAllowedChildSymbols(log, 0))
        grammar.RemoveAllowedChildSymbol(log, allowed, 0);
      grammar.AddAllowedChildSymbol(log, variable);

      exp.Enabled = true;
      exp.InitialFrequency = 1.0;
      foreach (var allowed in grammar.GetAllowedChildSymbols(exp))
        grammar.RemoveAllowedChildSymbol(exp, allowed);
      foreach (var allowed in grammar.GetAllowedChildSymbols(exp, 0))
        grammar.RemoveAllowedChildSymbol(exp, allowed, 0);
      grammar.AddAllowedChildSymbol(exp, variable);

      sine.Enabled = false;
      foreach (var allowed in grammar.GetAllowedChildSymbols(sine))
        grammar.RemoveAllowedChildSymbol(sine, allowed);
      foreach (var allowed in grammar.GetAllowedChildSymbols(sine, 0))
        grammar.RemoveAllowedChildSymbol(sine, allowed, 0);
      grammar.AddAllowedChildSymbol(sine, variable);

      cosine.Enabled = false;
      foreach (var allowed in grammar.GetAllowedChildSymbols(cosine))
        grammar.RemoveAllowedChildSymbol(cosine, allowed);
      foreach (var allowed in grammar.GetAllowedChildSymbols(cosine, 0))
        grammar.RemoveAllowedChildSymbol(cosine, allowed, 0);
      grammar.AddAllowedChildSymbol(cosine, variable);

      tangent.Enabled = false;
      foreach (var allowed in grammar.GetAllowedChildSymbols(tangent))
        grammar.RemoveAllowedChildSymbol(tangent, allowed);
      foreach (var allowed in grammar.GetAllowedChildSymbols(tangent, 0))
        grammar.RemoveAllowedChildSymbol(tangent, allowed, 0);
      grammar.AddAllowedChildSymbol(tangent, variable);
      #endregion

      problem.SymbolicExpressionTreeGrammar = grammar;

      // configure remaining problem parameters
      problem.MaximumSymbolicExpressionTreeLength.Value = 50;
      problem.MaximumSymbolicExpressionTreeDepth.Value = 12;
      problem.MaximumFunctionDefinitions.Value = 0;
      problem.MaximumFunctionArguments.Value = 0;

      var evaluator = new SymbolicRegressionConstantOptimizationEvaluator();
      evaluator.ConstantOptimizationIterations.Value = 5;
      problem.EvaluatorParameter.Value = evaluator;
      problem.RelativeNumberOfEvaluatedSamplesParameter.Hidden = true;
      problem.SolutionCreatorParameter.Hidden = true;
      #endregion

      #region Algorithm Configuration
      osga.Problem = problem;
      osga.Name = "Offspring Selection Genetic Programming - Symbolic Regression";
      osga.Description = "Genetic programming with strict offspring selection for solving a benchmark regression problem.";
      SamplesUtils.ConfigureOsGeneticAlgorithmParameters<GenderSpecificSelector, SubtreeCrossover, MultiSymbolicExpressionTreeManipulator>(osga, 100, 1, 25, 0.2, 50);
      var mutator = (MultiSymbolicExpressionTreeManipulator)osga.Mutator;
      mutator.Operators.OfType<FullTreeShaker>().Single().ShakingFactor = 0.1;
      mutator.Operators.OfType<OnePointShaker>().Single().ShakingFactor = 1.0;

      osga.Analyzer.Operators.SetItemCheckedState(
        osga.Analyzer.Operators
          .OfType<SymbolicRegressionSingleObjectiveOverfittingAnalyzer>()
          .Single(), false);
      osga.Analyzer.Operators.SetItemCheckedState(
        osga.Analyzer.Operators
          .OfType<SymbolicDataAnalysisAlleleFrequencyAnalyzer>()
          .First(), false);

      osga.ComparisonFactorModifierParameter.Hidden = true;
      osga.ComparisonFactorLowerBoundParameter.Hidden = true;
      osga.ComparisonFactorUpperBoundParameter.Hidden = true;
      osga.OffspringSelectionBeforeMutationParameter.Hidden = true;
      osga.SuccessRatioParameter.Hidden = true;
      osga.SelectedParentsParameter.Hidden = true;
      osga.ElitesParameter.Hidden = true;

      #endregion
      return osga;
    }
  }
}
