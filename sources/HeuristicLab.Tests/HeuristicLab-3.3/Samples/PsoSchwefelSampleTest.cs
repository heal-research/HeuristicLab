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
using System.IO;
using System.Linq;
using HeuristicLab.Algorithms.ParticleSwarmOptimization;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Problems.TestFunctions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class PsoSchwefelSampleTest {
    private const string SampleFileName = "PSO_Schwefel";

    [TestMethod]
    [TestCategory("Samples.Create")]
    [TestProperty("Time", "medium")]
    public void CreatePsoSchwefelSampleTest() {
      var pso = CreatePsoSchwefelSample();
      string path = Path.Combine(SamplesUtils.SamplesDirectory, SampleFileName + SamplesUtils.SampleFileExtension);
      XmlGenerator.Serialize(pso, path);
    }
    [TestMethod]
    [TestCategory("Samples.Execute")]
    [TestProperty("Time", "medium")]
    public void RunPsoSchwefelSampleTest() {
      var pso = CreatePsoSchwefelSample();
      pso.SetSeedRandomly.Value = false;
      SamplesUtils.RunAlgorithm(pso);
      if (Environment.Is64BitProcess) {
        Assert.AreEqual(118.43833503632464, SamplesUtils.GetDoubleResult(pso, "BestQuality"));
        Assert.AreEqual(118.43935663125784, SamplesUtils.GetDoubleResult(pso, "CurrentAverageQuality"));
        Assert.AreEqual(118.44721627137812, SamplesUtils.GetDoubleResult(pso, "CurrentWorstQuality"));
        Assert.AreEqual(1000, SamplesUtils.GetIntResult(pso, "Iterations"));
      } else {
        Assert.AreEqual(118.4383350363247, SamplesUtils.GetDoubleResult(pso, "BestQuality"));
        Assert.AreEqual(118.43935663125787, SamplesUtils.GetDoubleResult(pso, "CurrentAverageQuality"));
        Assert.AreEqual(118.44721627137824, SamplesUtils.GetDoubleResult(pso, "CurrentWorstQuality"));
        Assert.AreEqual(1000, SamplesUtils.GetIntResult(pso, "Iterations"));
      }
    }

    private ParticleSwarmOptimization CreatePsoSchwefelSample() {
      ParticleSwarmOptimization pso = new ParticleSwarmOptimization();
      #region Problem Configuration
      var problem = new SingleObjectiveTestFunctionProblem();
      problem.BestKnownQuality.Value = 0.0;
      problem.BestKnownSolutionParameter.Value = new RealVector(new double[] { 420.968746, 420.968746 });
      problem.Bounds = new DoubleMatrix(new double[,] { { -500, 500 } });
      problem.EvaluatorParameter.Value = new SchwefelEvaluator();
      problem.Maximization.Value = false;
      problem.ProblemSize.Value = 2;
      problem.SolutionCreatorParameter.Value = new UniformRandomRealVectorCreator();
      #endregion
      #region Algorithm Configuration
      pso.Name = "Particle Swarm Optimization - Schwefel";
      pso.Description = "A particle swarm optimization algorithm which solves the 2-dimensional Schwefel test function (based on the description in Pedersen, M.E.H. (2010). PhD thesis. University of Southampton)";
      pso.Problem = problem;
      pso.Inertia.Value = 10;
      pso.MaxIterations.Value = 1000;
      pso.NeighborBestAttraction.Value = 0.5;
      pso.PersonalBestAttraction.Value = -0.01;
      pso.SwarmSize.Value = 50;

      var inertiaUpdater = pso.InertiaUpdaterParameter.ValidValues
        .OfType<ExponentialDiscreteDoubleValueModifier>()
        .Single();
      inertiaUpdater.StartValueParameter.Value = new DoubleValue(10);
      inertiaUpdater.EndValueParameter.Value = new DoubleValue(0.8);
      pso.InertiaUpdater = inertiaUpdater;

      pso.ParticleCreator = pso.ParticleCreatorParameter.ValidValues
        .OfType<RealVectorParticleCreator>()
        .Single();
      var swarmUpdater = pso.SwarmUpdaterParameter.ValidValues
        .OfType<RealVectorSwarmUpdater>()
        .Single();
      swarmUpdater.MaxVelocityParameter.Value = new DoubleValue(20.0);
      swarmUpdater.FinalMaxVelocityParameter.Value = new DoubleValue(1.0);
      swarmUpdater.MaxVelocityScalingOperatorParameter.Value = swarmUpdater.MaxVelocityScalingOperatorParameter.ValidValues
        .OfType<ExponentialDiscreteDoubleValueModifier>()
        .Single();

      pso.TopologyInitializer = null;
      pso.TopologyUpdater = null;
      pso.SwarmUpdater = swarmUpdater;
      pso.Seed.Value = 0;
      pso.SetSeedRandomly.Value = true;
      #endregion
      pso.Engine = new ParallelEngine.ParallelEngine();
      return pso;
    }
  }
}
