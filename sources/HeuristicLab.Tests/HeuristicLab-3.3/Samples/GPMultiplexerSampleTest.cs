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

using System.IO;
using HeuristicLab.Algorithms.OffspringSelectionGeneticAlgorithm;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;
using HeuristicLab.Problems.Instances.DataAnalysis;
using HeuristicLab.Selection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class GPMultiplexerSampleTest {
    private const string SampleFileName = "GP_Multiplexer";

    [TestMethod]
    [TestCategory("Samples.Create")]
    [TestProperty("Time", "medium")]
    public void CreateGpMultiplexerSampleTest() {
      var ga = CreateGpMultiplexerSample();
      string path = Path.Combine(SamplesUtils.SamplesDirectory, SampleFileName + SamplesUtils.SampleFileExtension);
      XmlGenerator.Serialize(ga, path);
    }
    [TestMethod]
    [TestCategory("Samples.Execute")]
    [TestProperty("Time", "long")]
    public void RunGpMultiplexerSampleTest() {
      var osga = CreateGpMultiplexerSample();
      osga.SetSeedRandomly.Value = false;
      SamplesUtils.RunAlgorithm(osga);

      Assert.AreEqual(0.125, SamplesUtils.GetDoubleResult(osga, "BestQuality"), 1E-8);
      Assert.AreEqual(0.237275390625, SamplesUtils.GetDoubleResult(osga, "CurrentAverageQuality"), 1E-8);
      Assert.AreEqual(1.181640625, SamplesUtils.GetDoubleResult(osga, "CurrentWorstQuality"), 1E-8);
      Assert.AreEqual(105500, SamplesUtils.GetIntResult(osga, "EvaluatedSolutions"));
    }

    public static OffspringSelectionGeneticAlgorithm CreateGpMultiplexerSample() {
      var instanceProvider = new RegressionCSVInstanceProvider();
      var regressionImportType = new RegressionImportType();
      regressionImportType.TargetVariable = "output";
      regressionImportType.TrainingPercentage = 100;
      var dataAnalysisCSVFormat = new DataAnalysisCSVFormat();
      dataAnalysisCSVFormat.Separator = ',';
      dataAnalysisCSVFormat.VariableNamesAvailable = true;

      var problemData = instanceProvider.ImportData(@"Test Resources\Multiplexer11.csv", regressionImportType, dataAnalysisCSVFormat);
      problemData.Name = "11-Multiplexer";

      var problem = new SymbolicRegressionSingleObjectiveProblem();
      problem.Name = "11-Multiplexer Problem";
      problem.ProblemData = problemData;
      problem.MaximumSymbolicExpressionTreeLength.Value = 50;
      problem.MaximumSymbolicExpressionTreeDepth.Value = 50;
      problem.EvaluatorParameter.Value = new SymbolicRegressionSingleObjectiveMeanSquaredErrorEvaluator();
      problem.ApplyLinearScaling.Value = false;


      var grammar = new FullFunctionalExpressionGrammar();
      problem.SymbolicExpressionTreeGrammar = grammar;
      foreach (var symbol in grammar.Symbols) {
        if (symbol is ProgramRootSymbol) symbol.Enabled = true;
        else if (symbol is StartSymbol) symbol.Enabled = true;
        else if (symbol is IfThenElse) symbol.Enabled = true;
        else if (symbol is And) symbol.Enabled = true;
        else if (symbol is Or) symbol.Enabled = true;
        else if (symbol is Xor) symbol.Enabled = true;
        else if (symbol.GetType() == typeof(Variable)) {
          //necessary as there are multiple classes derived from Variable (e.g., VariableCondition)
          symbol.Enabled = true;
          var variableSymbol = (Variable)symbol;
          variableSymbol.MultiplicativeWeightManipulatorSigma = 0.0;
          variableSymbol.WeightManipulatorSigma = 0.0;
          variableSymbol.WeightSigma = 0.0;
        } else symbol.Enabled = false;
      }

      var osga = new OffspringSelectionGeneticAlgorithm();
      osga.Name = "Genetic Programming - Multiplexer 11 Problem";
      osga.Description = "A genetic programming algorithm that solves the 11-bit multiplexer problem.";
      osga.Problem = problem;
      SamplesUtils.ConfigureOsGeneticAlgorithmParameters<GenderSpecificSelector, SubtreeCrossover, MultiSymbolicExpressionTreeManipulator>
        (osga, popSize: 100, elites: 1, maxGens: 50, mutationRate: 0.25);
      osga.MaximumSelectionPressure.Value = 200;
      return osga;

    }
  }
}
