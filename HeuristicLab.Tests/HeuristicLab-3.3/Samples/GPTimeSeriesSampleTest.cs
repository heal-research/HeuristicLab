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
using HEAL.Attic;
using HeuristicLab.Algorithms.OffspringSelectionGeneticAlgorithm;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic.TimeSeriesPrognosis;
using HeuristicLab.Selection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class GPTimeSeriesSampleTest {
    private const string SampleFileName = "OSGP_TimeSeries";

    private static readonly ProtoBufSerializer serializer = new ProtoBufSerializer();

    [TestMethod]
    [TestCategory("Samples.Create")]
    [TestProperty("Time", "medium")]
    public void CreateGpTimeSeriesSampleTest() {
      var ga = CreateGpTimeSeriesSample();
      string path = Path.Combine(SamplesUtils.SamplesDirectory, SampleFileName + SamplesUtils.SampleFileExtension);
      serializer.Serialize(ga, path);
    }
    [TestMethod]
    [TestCategory("Samples.Execute")]
    [TestProperty("Time", "long")]
    public void RunGpTimeSeriesSampleTest() {
      var osga = CreateGpTimeSeriesSample();
      osga.SetSeedRandomly.Value = false;
      SamplesUtils.RunAlgorithm(osga);

      Assert.AreEqual(0.015441526903606416, SamplesUtils.GetDoubleResult(osga, "BestQuality"), 1E-8);
      Assert.AreEqual(0.017420834241279298, SamplesUtils.GetDoubleResult(osga, "CurrentAverageQuality"), 1E-8);
      Assert.AreEqual(0.065195703753298972, SamplesUtils.GetDoubleResult(osga, "CurrentWorstQuality"), 1E-8);
      Assert.AreEqual(92000, SamplesUtils.GetIntResult(osga, "EvaluatedSolutions"));
    }

    public static OffspringSelectionGeneticAlgorithm CreateGpTimeSeriesSample() {
      var problem = new SymbolicTimeSeriesPrognosisSingleObjectiveProblem();
      problem.Name = "Symbolic time series prognosis problem (Mackey Glass t=17)";
      problem.ProblemData.Name = "Mackey Glass t=17";
      problem.MaximumSymbolicExpressionTreeLength.Value = 125;
      problem.MaximumSymbolicExpressionTreeDepth.Value = 12;
      problem.EvaluatorParameter.Value.HorizonParameter.Value.Value = 10;
      problem.ApplyLinearScaling.Value = true;

      foreach (var symbol in problem.SymbolicExpressionTreeGrammar.Symbols) {
        if (symbol is Exponential || symbol is Logarithm) {
          symbol.Enabled = false;
        } else if (symbol is AutoregressiveTargetVariable) {
          symbol.Enabled = true;
          var autoRegressiveSymbol = symbol as AutoregressiveTargetVariable;
          autoRegressiveSymbol.MinLag = -30;
          autoRegressiveSymbol.MaxLag = -1;
        }
        if (symbol is VariableBase) {
          var varSy = symbol as VariableBase;
          varSy.VariableChangeProbability = 1.0; // backwards compatibility
        }
      }

      var osga = new OffspringSelectionGeneticAlgorithm();
      osga.Name = "Genetic Programming - Time Series Prediction (Mackey-Glass-17)";
      osga.Description = "A genetic programming algorithm for creating a time-series model for the Mackey-Glass-17 time series.";
      osga.Problem = problem;
      SamplesUtils.ConfigureOsGeneticAlgorithmParameters<GenderSpecificSelector, SubtreeCrossover, MultiSymbolicExpressionTreeManipulator>
        (osga, popSize: 100, elites: 1, maxGens: 25, mutationRate: 0.15);
      osga.MaximumSelectionPressure.Value = 100;
      return osga;

    }
  }
}
