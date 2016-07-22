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
using HeuristicLab.Algorithms.EvolutionStrategy;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Problems.TestFunctions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class EsGriewankSampleTest {
    private const string SampleFileName = "ES_Griewank";

    [TestMethod]
    [TestCategory("Samples.Create")]
    [TestProperty("Time", "medium")]
    public void CreateEsGriewankSampleTest() {
      var es = CreateEsGriewankSample();
      string path = Path.Combine(SamplesUtils.SamplesDirectory, SampleFileName + SamplesUtils.SampleFileExtension);
      XmlGenerator.Serialize(es, path);
    }

    [TestMethod]
    [TestCategory("Samples.Execute")]
    [TestProperty("Time", "long")]
    public void RunEsGriewankSampleTest() {
      var es = CreateEsGriewankSample();
      es.SetSeedRandomly.Value = false;
      SamplesUtils.RunAlgorithm(es);
      Assert.AreEqual(0, SamplesUtils.GetDoubleResult(es, "BestQuality"));
      Assert.AreEqual(0, SamplesUtils.GetDoubleResult(es, "CurrentAverageQuality"));
      Assert.AreEqual(0, SamplesUtils.GetDoubleResult(es, "CurrentWorstQuality"));
      Assert.AreEqual(100020, SamplesUtils.GetIntResult(es, "EvaluatedSolutions"));
    }

    private EvolutionStrategy CreateEsGriewankSample() {
      EvolutionStrategy es = new EvolutionStrategy();

      #region Problem Configuration

      SingleObjectiveTestFunctionProblem problem = new SingleObjectiveTestFunctionProblem();

      problem.ProblemSize.Value = 10;
      problem.EvaluatorParameter.Value = new GriewankEvaluator();
      problem.SolutionCreatorParameter.Value = new UniformRandomRealVectorCreator();
      problem.Maximization.Value = false;
      problem.Bounds = new DoubleMatrix(new double[,] { { -600, 600 } });
      problem.BestKnownQuality.Value = 0;
      problem.BestKnownSolutionParameter.Value = new RealVector(10);
      problem.Name = "Single Objective Test Function";
      problem.Description = "Test function with real valued inputs and a single objective.";

      #endregion

      #region Algorithm Configuration

      es.Name = "Evolution Strategy - Griewank";
      es.Description = "An evolution strategy which solves the 10-dimensional Griewank test function";
      es.Problem = problem;
      SamplesUtils.ConfigureEvolutionStrategyParameters<AverageCrossover, NormalAllPositionsManipulator,
        StdDevStrategyVectorCreator, StdDevStrategyVectorCrossover, StdDevStrategyVectorManipulator>(
          es, 20, 500, 2, 200, false);

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
