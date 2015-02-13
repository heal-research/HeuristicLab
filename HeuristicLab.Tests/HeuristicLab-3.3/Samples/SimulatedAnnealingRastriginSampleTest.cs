#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Algorithms.SimulatedAnnealing;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Problems.TestFunctions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class SimulatedAnnealingRastriginSampleTest {
    private const string SampleFileName = "SA_Rastrigin";

    [TestMethod]
    [TestCategory("Samples.Create")]
    [TestProperty("Time", "medium")]
    public void CreateSimulatedAnnealingRastriginSampleTest() {
      var sa = CreateSimulatedAnnealingRastriginSample();
      string path = Path.Combine(SamplesUtils.SamplesDirectory, SampleFileName + SamplesUtils.SampleFileExtension);
      XmlGenerator.Serialize(sa, path);
    }
    [TestMethod]
    [TestCategory("Samples.Execute")]
    [TestProperty("Time", "medium")]
    public void RunSimulatedAnnealingRastriginSampleTest() {
      var sa = CreateSimulatedAnnealingRastriginSample();
      sa.SetSeedRandomly.Value = false;
      SamplesUtils.RunAlgorithm(sa);
      Assert.AreEqual(0.00014039606034543795, SamplesUtils.GetDoubleResult(sa, "BestQuality"));
      Assert.AreEqual(5000, SamplesUtils.GetIntResult(sa, "EvaluatedMoves"));
    }
    private SimulatedAnnealing CreateSimulatedAnnealingRastriginSample() {
      SimulatedAnnealing sa = new SimulatedAnnealing();
      #region Problem Configuration
      var problem = new SingleObjectiveTestFunctionProblem();
      problem.BestKnownQuality.Value = 0.0;
      problem.BestKnownSolutionParameter.Value = new RealVector(new double[] { 0, 0 });
      problem.Bounds = new DoubleMatrix(new double[,] { { -5.12, 5.12 } });
      problem.EvaluatorParameter.Value = new RastriginEvaluator();
      problem.Maximization.Value = false;
      problem.ProblemSize.Value = 2;
      problem.SolutionCreatorParameter.Value = new UniformRandomRealVectorCreator();
      #endregion
      #region Algorithm Configuration
      sa.Name = "Simulated Annealing - Rastrigin";
      sa.Description = "A simulated annealing algorithm that solves the 2-dimensional Rastrigin test function";
      sa.Problem = problem;
      var annealingOperator = sa.AnnealingOperatorParameter.ValidValues
        .OfType<ExponentialDiscreteDoubleValueModifier>()
        .Single();
      annealingOperator.StartIndexParameter.Value = new IntValue(0);
      sa.AnnealingOperator = annealingOperator;

      sa.EndTemperature.Value = 1E-6;
      sa.InnerIterations.Value = 50;
      sa.MaximumIterations.Value = 100;
      var moveEvaluator = sa.MoveEvaluatorParameter.ValidValues
        .OfType<RastriginAdditiveMoveEvaluator>()
        .Single();
      moveEvaluator.A.Value = 10;
      sa.MoveEvaluator = moveEvaluator;

      var moveGenerator = sa.MoveGeneratorParameter.ValidValues
        .OfType<StochasticNormalMultiMoveGenerator>()
        .Single();
      moveGenerator.SigmaParameter.Value = new DoubleValue(1);
      sa.MoveGenerator = moveGenerator;

      sa.MoveMaker = sa.MoveMakerParameter.ValidValues
        .OfType<AdditiveMoveMaker>()
        .Single();

      sa.Seed.Value = 0;
      sa.SetSeedRandomly.Value = true;
      sa.StartTemperature.Value = 1;
      #endregion
      sa.Engine = new ParallelEngine.ParallelEngine();
      return sa;
    }
  }
}
