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
        Assert.AreEqual(2.8334909529803554E-08, SamplesUtils.GetDoubleResult(pso, "BestQuality"));
        Assert.AreEqual(128.08680460446624, SamplesUtils.GetDoubleResult(pso, "CurrentAverageQuality"));
        Assert.AreEqual(713.67728101375587, SamplesUtils.GetDoubleResult(pso, "CurrentWorstQuality"));
        Assert.AreEqual(200, SamplesUtils.GetIntResult(pso, "Iterations"));
      } else {
        Assert.AreEqual(2.8334909529803554E-08, SamplesUtils.GetDoubleResult(pso, "BestQuality"));
        Assert.AreEqual(128.08680460446624, SamplesUtils.GetDoubleResult(pso, "CurrentAverageQuality"));
        Assert.AreEqual(713.67728101375587, SamplesUtils.GetDoubleResult(pso, "CurrentWorstQuality"));
        Assert.AreEqual(200, SamplesUtils.GetIntResult(pso, "Iterations"));
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
      pso.Inertia.Value = 1.1;
      pso.MaxIterations.Value = 200;
      pso.NeighborBestAttraction.Value = 1;
      pso.PersonalBestAttraction.Value = 1;
      pso.SwarmSize.Value = 40;

      var inertiaUpdater = pso.InertiaUpdaterParameter.ValidValues
        .OfType<ExponentialDiscreteDoubleValueModifier>()
        .Single();
      inertiaUpdater.EndValueParameter.Value = new DoubleValue(0.721);
      pso.InertiaUpdater = inertiaUpdater;
      
      pso.TopologyInitializer = null;
      pso.TopologyUpdater = null;
      pso.Seed.Value = 0;
      pso.SetSeedRandomly.Value = true;
      #endregion
      pso.Engine = new ParallelEngine.ParallelEngine();
      return pso;
    }
  }
}
