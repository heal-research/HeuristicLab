#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Algorithms.ParticleSwarmOptimization;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Problems.TestFunctions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class PsoRastriginSampleTest {
    private const string SampleFileName = "PSO_Rastrigin";
    private static readonly ProtoBufSerializer serializer = new ProtoBufSerializer();

    [TestMethod]
    [TestCategory("Samples.Create")]
    [TestProperty("Time", "medium")]
    public void CreatePsoRastriginSampleTest() {
      var pso = CreatePsoRastriginSample();
      string path = Path.Combine(SamplesUtils.SamplesDirectory, SampleFileName + SamplesUtils.SampleFileExtension);
      serializer.Serialize(pso, path);
    }
    [TestMethod]
    [TestCategory("Samples.Execute")]
    [TestProperty("Time", "medium")]
    public void RunPsoRastriginSampleTest() {
      var pso = CreatePsoRastriginSample();
      pso.SetSeedRandomly.Value = false;
      SamplesUtils.RunAlgorithm(pso);
      if (Environment.Is64BitProcess) {
        Assert.AreEqual(0, SamplesUtils.GetDoubleResult(pso, "BestQuality"));
        Assert.AreEqual(3.9649516110677525, SamplesUtils.GetDoubleResult(pso, "CurrentAverageQuality"), 1e-08);
        Assert.AreEqual(25.566430359483757, SamplesUtils.GetDoubleResult(pso, "CurrentWorstQuality"), 1e-08);
        Assert.AreEqual(200, SamplesUtils.GetIntResult(pso, "Iterations"));
      } else {
        Assert.AreEqual(0, SamplesUtils.GetDoubleResult(pso, "BestQuality"));
        Assert.AreEqual(3.3957460831564048, SamplesUtils.GetDoubleResult(pso, "CurrentAverageQuality"), 1e-08);
        Assert.AreEqual(34.412788077766145, SamplesUtils.GetDoubleResult(pso, "CurrentWorstQuality"), 1e-08);
        Assert.AreEqual(200, SamplesUtils.GetIntResult(pso, "Iterations"));
      }
    }

    private ParticleSwarmOptimization CreatePsoRastriginSample() {
      ParticleSwarmOptimization pso = new ParticleSwarmOptimization();
      #region Problem Configuration
      var problem = new SingleObjectiveTestFunctionProblem();
      var provider = new SOTFInstanceProvider();
      problem.Load(provider.LoadData(provider.GetDataDescriptors().Single(x => x.Name == "Rastrigin Function")));
      problem.SolutionCreatorParameter.Value = new UniformRandomRealVectorCreator();
      #endregion
      #region Algorithm Configuration
      pso.Name = "Particle Swarm Optimization - Rastrigin";
      pso.Description = "A particle swarm optimization algorithm which solves the 2-dimensional Rastrigin test function.";
      pso.Problem = problem;
      pso.Inertia.Value = 0.721;
      pso.MaxIterations.Value = 200;
      pso.NeighborBestAttraction.Value = 1.193;
      pso.PersonalBestAttraction.Value = 1.193;
      pso.SwarmSize.Value = 40;
            
      pso.TopologyInitializer = pso.TopologyInitializerParameter.ValidValues.OfType<SPSORandomTopologyInitializer>().First();
      pso.TopologyUpdater = pso.TopologyUpdaterParameter.ValidValues.OfType<SPSOAdaptiveRandomTopologyUpdater>().First();
      pso.Seed.Value = 0;
      pso.SetSeedRandomly.Value = true;
      #endregion
      pso.Engine = new ParallelEngine.ParallelEngine();
      return pso;
    }
  }
}
