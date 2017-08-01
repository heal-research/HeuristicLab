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

using System.IO;
using System.Linq;
using HeuristicLab.Algorithms.GeneticAlgorithm;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Classification;
using HeuristicLab.Problems.Instances.DataAnalysis;
using HeuristicLab.Selection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class GPSymbolicClassificationSampleTest {
    private const string SampleFileName = "SGP_SymbClass";

    [TestMethod]
    [TestCategory("Samples.Create")]
    [TestProperty("Time", "medium")]
    public void CreateGpSymbolicClassificationSampleTest() {
      var ga = CreateGpSymbolicClassificationSample();
      string path = Path.Combine(SamplesUtils.SamplesDirectory, SampleFileName + SamplesUtils.SampleFileExtension);
      XmlGenerator.Serialize(ga, path);
    }

    [TestMethod]
    [TestCategory("Samples.Execute")]
    [TestProperty("Time", "long")]
    public void RunGpSymbolicClassificationSampleTest() {
      var ga = CreateGpSymbolicClassificationSample();
      ga.SetSeedRandomly.Value = false;
      SamplesUtils.RunAlgorithm(ga);
      Assert.AreEqual(0.141880203907627, SamplesUtils.GetDoubleResult(ga, "BestQuality"), 1E-8);
      Assert.AreEqual(4.3246992327753295, SamplesUtils.GetDoubleResult(ga, "CurrentAverageQuality"), 1E-8);
      Assert.AreEqual(100.62175156249987, SamplesUtils.GetDoubleResult(ga, "CurrentWorstQuality"), 1E-8);
      Assert.AreEqual(100900, SamplesUtils.GetIntResult(ga, "EvaluatedSolutions"));
      var bestTrainingSolution = (IClassificationSolution)ga.Results["Best training solution"].Value;
      Assert.AreEqual(0.80875, bestTrainingSolution.TrainingAccuracy, 1E-8);
      Assert.AreEqual(0.795031055900621, bestTrainingSolution.TestAccuracy, 1E-8);
      var bestValidationSolution = (IClassificationSolution)ga.Results["Best validation solution"].Value;
      Assert.AreEqual(0.81375, bestValidationSolution.TrainingAccuracy, 1E-8);
      Assert.AreEqual(0.788819875776398, bestValidationSolution.TestAccuracy, 1E-8);
    }

    private GeneticAlgorithm CreateGpSymbolicClassificationSample() {
      GeneticAlgorithm ga = new GeneticAlgorithm();
      #region Problem Configuration
      SymbolicClassificationSingleObjectiveProblem symbClassProblem = new SymbolicClassificationSingleObjectiveProblem();
      symbClassProblem.Name = "Mammography Classification Problem";
      symbClassProblem.Description = "Mammography dataset imported from the UCI machine learning repository (http://archive.ics.uci.edu/ml/datasets/Mammographic+Mass)";
      UCIInstanceProvider provider = new UCIInstanceProvider();
      var instance = provider.GetDataDescriptors().Where(x => x.Name.Equals("Mammography, M. Elter, 2007")).Single();
      var mammoData = (ClassificationProblemData)provider.LoadData(instance);
      mammoData.TargetVariableParameter.Value = mammoData.TargetVariableParameter.ValidValues
        .First(v => v.Value == "Severity");
      mammoData.InputVariables.SetItemCheckedState(
        mammoData.InputVariables.Single(x => x.Value == "BI-RADS"), false);
      mammoData.InputVariables.SetItemCheckedState(
        mammoData.InputVariables.Single(x => x.Value == "Age"), true);
      mammoData.InputVariables.SetItemCheckedState(
        mammoData.InputVariables.Single(x => x.Value == "Shape"), true);
      mammoData.InputVariables.SetItemCheckedState(
        mammoData.InputVariables.Single(x => x.Value == "Margin"), true);
      mammoData.InputVariables.SetItemCheckedState(
        mammoData.InputVariables.Single(x => x.Value == "Density"), true);
      mammoData.InputVariables.SetItemCheckedState(
        mammoData.InputVariables.Single(x => x.Value == "Severity"), false);
      mammoData.TrainingPartition.Start = 0;
      mammoData.TrainingPartition.End = 800;
      mammoData.TestPartition.Start = 800;
      mammoData.TestPartition.End = 961;
      mammoData.Name = "Data imported from mammographic_masses.csv";
      mammoData.Description = "Original dataset: http://archive.ics.uci.edu/ml/datasets/Mammographic+Mass, missing values have been replaced with median values.";
      symbClassProblem.ProblemData = mammoData;

      // configure grammar
      var grammar = new TypeCoherentExpressionGrammar();
      grammar.ConfigureAsDefaultClassificationGrammar();
      grammar.Symbols.OfType<VariableCondition>().Single().Enabled = false;
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
      symbClassProblem.SymbolicExpressionTreeGrammar = grammar;

      // configure remaining problem parameters
      symbClassProblem.BestKnownQuality.Value = 0.0;
      symbClassProblem.FitnessCalculationPartition.Start = 0;
      symbClassProblem.FitnessCalculationPartition.End = 400;
      symbClassProblem.ValidationPartition.Start = 400;
      symbClassProblem.ValidationPartition.End = 800;
      symbClassProblem.RelativeNumberOfEvaluatedSamples.Value = 1;
      symbClassProblem.MaximumSymbolicExpressionTreeLength.Value = 100;
      symbClassProblem.MaximumSymbolicExpressionTreeDepth.Value = 10;
      symbClassProblem.MaximumFunctionDefinitions.Value = 0;
      symbClassProblem.MaximumFunctionArguments.Value = 0;
      symbClassProblem.EvaluatorParameter.Value = new SymbolicClassificationSingleObjectiveMeanSquaredErrorEvaluator();
      #endregion
      #region Algorithm Configuration
      ga.Problem = symbClassProblem;
      ga.Name = "Genetic Programming - Symbolic Classification";
      ga.Description = "A standard genetic programming algorithm to solve a classification problem (Mammographic+Mass dataset)";
      SamplesUtils.ConfigureGeneticAlgorithmParameters<TournamentSelector, SubtreeCrossover, MultiSymbolicExpressionTreeManipulator>(
        ga, 1000, 1, 100, 0.15, 5
        );

      var mutator = (MultiSymbolicExpressionTreeManipulator)ga.Mutator;
      mutator.Operators.OfType<FullTreeShaker>().Single().ShakingFactor = 0.1;
      mutator.Operators.OfType<OnePointShaker>().Single().ShakingFactor = 1.0;

      ga.Analyzer.Operators.SetItemCheckedState(
        ga.Analyzer.Operators
        .OfType<SymbolicClassificationSingleObjectiveOverfittingAnalyzer>()
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
