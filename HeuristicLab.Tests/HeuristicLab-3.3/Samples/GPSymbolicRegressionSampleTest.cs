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

using System.IO;
using System.Linq;
using HeuristicLab.Algorithms.GeneticAlgorithm;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;
using HeuristicLab.Problems.Instances.DataAnalysis;
using HeuristicLab.Selection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class GPSymbolicRegressionSampleTest {
    private const string SampleFileName = "SGP_SymbReg";

    [TestMethod]
    [TestCategory("Samples.Create")]
    [TestProperty("Time", "medium")]
    public void CreateGpSymbolicRegressionSampleTest() {
      var ga = CreateGpSymbolicRegressionSample();
      string path = Path.Combine(SamplesUtils.SamplesDirectory, SampleFileName + SamplesUtils.SampleFileExtension);
      XmlGenerator.Serialize(ga, path);
    }
    [TestMethod]
    [TestCategory("Samples.Execute")]
    [TestProperty("Time", "long")]
    public void RunGpSymbolicRegressionSampleTest() {
      var ga = CreateGpSymbolicRegressionSample();
      ga.SetSeedRandomly.Value = false;
      SamplesUtils.RunAlgorithm(ga);
      Assert.AreEqual(0.858344291534625, SamplesUtils.GetDoubleResult(ga, "BestQuality"), 1E-8);
      Assert.AreEqual(0.56758466520692641, SamplesUtils.GetDoubleResult(ga, "CurrentAverageQuality"), 1E-8);
      Assert.AreEqual(0, SamplesUtils.GetDoubleResult(ga, "CurrentWorstQuality"), 1E-8);
      Assert.AreEqual(50950, SamplesUtils.GetIntResult(ga, "EvaluatedSolutions"));
      var bestTrainingSolution = (IRegressionSolution)ga.Results["Best training solution"].Value;
      Assert.AreEqual(0.85504801557844745, bestTrainingSolution.TrainingRSquared, 1E-8);
      Assert.AreEqual(0.86259381948647817, bestTrainingSolution.TestRSquared, 1E-8);
      var bestValidationSolution = (IRegressionSolution)ga.Results["Best validation solution"].Value;
      Assert.AreEqual(0.84854338315539746, bestValidationSolution.TrainingRSquared, 1E-8);
      Assert.AreEqual(0.8662813452656678, bestValidationSolution.TestRSquared, 1E-8);
    }

    private GeneticAlgorithm CreateGpSymbolicRegressionSample() {
      GeneticAlgorithm ga = new GeneticAlgorithm();
      #region Problem Configuration
      SymbolicRegressionSingleObjectiveProblem symbRegProblem = new SymbolicRegressionSingleObjectiveProblem();
      symbRegProblem.Name = "Tower Symbolic Regression Problem";
      symbRegProblem.Description = "Tower Dataset (downloaded from: http://www.symbolicregression.com/?q=towerProblem)";
      RegressionRealWorldInstanceProvider provider = new RegressionRealWorldInstanceProvider();
      var instance = provider.GetDataDescriptors().Where(x => x.Name.Equals("Tower")).Single();
      var towerProblemData = (RegressionProblemData)provider.LoadData(instance);
      towerProblemData.TargetVariableParameter.Value = towerProblemData.TargetVariableParameter.ValidValues
        .First(v => v.Value == "towerResponse");
      towerProblemData.InputVariables.SetItemCheckedState(
        towerProblemData.InputVariables.Single(x => x.Value == "x1"), true);
      towerProblemData.InputVariables.SetItemCheckedState(
        towerProblemData.InputVariables.Single(x => x.Value == "x7"), false);
      towerProblemData.InputVariables.SetItemCheckedState(
        towerProblemData.InputVariables.Single(x => x.Value == "x11"), false);
      towerProblemData.InputVariables.SetItemCheckedState(
        towerProblemData.InputVariables.Single(x => x.Value == "x16"), false);
      towerProblemData.InputVariables.SetItemCheckedState(
        towerProblemData.InputVariables.Single(x => x.Value == "x21"), false);
      towerProblemData.InputVariables.SetItemCheckedState(
        towerProblemData.InputVariables.Single(x => x.Value == "x25"), false);
      towerProblemData.InputVariables.SetItemCheckedState(
        towerProblemData.InputVariables.Single(x => x.Value == "towerResponse"), false);
      towerProblemData.TrainingPartition.Start = 0;
      towerProblemData.TrainingPartition.End = 3136;
      towerProblemData.TestPartition.Start = 3136;
      towerProblemData.TestPartition.End = 4999;
      towerProblemData.Name = "Data imported from towerData.txt";
      towerProblemData.Description = "Chemical concentration at top of distillation tower, dataset downloaded from: http://vanillamodeling.com/realproblems.html, best R² achieved with nu-SVR = 0.97";
      symbRegProblem.ProblemData = towerProblemData;

      // configure grammar
      var grammar = new TypeCoherentExpressionGrammar();
      grammar.ConfigureAsDefaultRegressionGrammar();
      grammar.Symbols.OfType<VariableCondition>().Single().InitialFrequency = 0.0;
      foreach (var varSy in grammar.Symbols.OfType<VariableBase>()) varSy.VariableChangeProbability = 1.0; // for backwards compatibilty
      var varSymbol = grammar.Symbols.OfType<Variable>().Single();
      varSymbol.WeightMu = 1.0;
      varSymbol.WeightSigma = 1.0;
      varSymbol.WeightManipulatorMu = 0.0;
      varSymbol.WeightManipulatorSigma = 0.05;
      varSymbol.MultiplicativeWeightManipulatorSigma = 0.03;
      var constSymbol = grammar.Symbols.OfType<Constant>().Single();
      constSymbol.MaxValue = 20;
      constSymbol.MinValue = -20;
      constSymbol.ManipulatorMu = 0.0;
      constSymbol.ManipulatorSigma = 1;
      constSymbol.MultiplicativeManipulatorSigma = 0.03;
      symbRegProblem.SymbolicExpressionTreeGrammar = grammar;

      // configure remaining problem parameters
      symbRegProblem.BestKnownQuality.Value = 0.97;
      symbRegProblem.FitnessCalculationPartition.Start = 0;
      symbRegProblem.FitnessCalculationPartition.End = 2300;
      symbRegProblem.ValidationPartition.Start = 2300;
      symbRegProblem.ValidationPartition.End = 3136;
      symbRegProblem.RelativeNumberOfEvaluatedSamples.Value = 1;
      symbRegProblem.MaximumSymbolicExpressionTreeLength.Value = 150;
      symbRegProblem.MaximumSymbolicExpressionTreeDepth.Value = 12;
      symbRegProblem.MaximumFunctionDefinitions.Value = 0;
      symbRegProblem.MaximumFunctionArguments.Value = 0;

      symbRegProblem.EvaluatorParameter.Value = new SymbolicRegressionSingleObjectivePearsonRSquaredEvaluator();
      #endregion
      #region Algorithm Configuration
      ga.Problem = symbRegProblem;
      ga.Name = "Genetic Programming - Symbolic Regression";
      ga.Description = "A standard genetic programming algorithm to solve a symbolic regression problem (tower dataset)";
      SamplesUtils.ConfigureGeneticAlgorithmParameters<TournamentSelector, SubtreeCrossover, MultiSymbolicExpressionTreeManipulator>(
        ga, 1000, 1, 50, 0.15, 5);
      var mutator = (MultiSymbolicExpressionTreeManipulator)ga.Mutator;
      mutator.Operators.OfType<FullTreeShaker>().Single().ShakingFactor = 0.1;
      mutator.Operators.OfType<OnePointShaker>().Single().ShakingFactor = 1.0;

      ga.Analyzer.Operators.SetItemCheckedState(
        ga.Analyzer.Operators
        .OfType<SymbolicRegressionSingleObjectiveOverfittingAnalyzer>()
        .Single(), false);
      ga.Analyzer.Operators.SetItemCheckedState(
        ga.Analyzer.Operators
        .OfType<SymbolicDataAnalysisAlleleFrequencyAnalyzer>()
        .First(), false);
      #endregion
      return ga;
    }
  }
}
