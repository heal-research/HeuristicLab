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
using HeuristicLab.Algorithms.OffspringSelectionEvolutionStrategy;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Problems.TestFunctions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class OSESGriewankSampleTest {
    private const string SampleFileName = "OSES_Griewank";

    [TestMethod]
    [TestCategory("Samples.Create")]
    [TestProperty("Time", "medium")]
    public void CreateOSESGriewankSampleTest() {
      var es = CreateOSESGriewankSample();
      string path = Path.Combine(SamplesUtils.SamplesDirectory, SampleFileName + SamplesUtils.SampleFileExtension);
      XmlGenerator.Serialize(es, path);
    }

    [TestMethod]
    [TestCategory("Samples.Execute")]
    [TestProperty("Time", "long")]
    public void RunOSESGriewankSampleTest() {
      var es = CreateOSESGriewankSample();
      es.SetSeedRandomly.Value = false;
      SamplesUtils.RunAlgorithm(es);
      Assert.AreEqual(1.80366417E-07, SamplesUtils.GetDoubleResult(es, "BestQuality"), 1.0E-15);
      Assert.AreEqual(4.84418627E-07, SamplesUtils.GetDoubleResult(es, "CurrentAverageQuality"), 1.0E-15);
      Assert.AreEqual(9.20629802E-07, SamplesUtils.GetDoubleResult(es, "CurrentWorstQuality"), 1.0E-15);
      Assert.AreEqual(39750, SamplesUtils.GetIntResult(es, "EvaluatedSolutions"));
    }

    private OffspringSelectionEvolutionStrategy CreateOSESGriewankSample() {
      OffspringSelectionEvolutionStrategy es = new OffspringSelectionEvolutionStrategy();

      #region Problem Configuration

      SingleObjectiveTestFunctionProblem problem = new SingleObjectiveTestFunctionProblem();

      problem.ProblemSize = 10;
      problem.TestFunction = new Griewank();
      problem.Encoding.SolutionCreator = new UniformRandomRealVectorCreator();
      problem.Bounds = new DoubleMatrix(new double[,] { { -600, 600 } });
      problem.BestKnownQuality = 0;
      problem.BestKnownSolutionParameter.Value = new RealVector(10);
      problem.Name = "Single Objective Test Function";
      problem.Description = "Test function with real valued inputs and a single objective.";

      #endregion

      #region Algorithm Configuration

      es.Name = "Offspring Selection Evolution Strategy - Griewank";
      es.Description = "An evolution strategy with offspring selection which solves the 10-dimensional Griewank test function";
      es.Problem = problem;
      SamplesUtils.ConfigureOffspringSelectionEvolutionStrategyParameters<AverageCrossover, NormalAllPositionsManipulator,
        StdDevStrategyVectorCreator, StdDevStrategyVectorCrossover, StdDevStrategyVectorManipulator>(es, 50, 1.0, 0.5, 100, 2, 100, false);

      StdDevStrategyVectorCreator strategyCreator = (StdDevStrategyVectorCreator)es.StrategyParameterCreator;
      strategyCreator.BoundsParameter.Value = new DoubleMatrix(new double[,] { { 1, 20 } });

      StdDevStrategyVectorManipulator strategyManipulator =
        (StdDevStrategyVectorManipulator)es.StrategyParameterManipulator;
      strategyManipulator.BoundsParameter.Value = new DoubleMatrix(new double[,] { { 1E-12, 30 } });
      strategyManipulator.GeneralLearningRateParameter.Value = new DoubleValue(0.22360679774997896);
      strategyManipulator.LearningRateParameter.Value = new DoubleValue(0.39763536438352531);

      #endregion

      return es;
    }
  }
}
