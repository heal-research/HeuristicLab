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
using System.IO;
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Algorithms.GeneticAlgorithm;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;
using HeuristicLab.Problems.Instances.DataAnalysis;
using HeuristicLab.Selection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class ShapeConstrainedRegressionSampleTest {
    private const string SampleFileName = "GA_Shape_Constrained_Regression";
    private static readonly ProtoBufSerializer serializer = new ProtoBufSerializer();

    [TestMethod]
    [TestCategory("Samples.Create")]
    [TestProperty("Time", "medium")]
    public void CreateShapeConstrainedRegressionSampleTest() {
      var ga = CreateShapeConstrainedRegressionSample();
      string path = Path.Combine(SamplesUtils.SamplesDirectory, SampleFileName + SamplesUtils.SampleFileExtension);
      serializer.Serialize(ga, path);
    }
    [TestMethod]
    [TestCategory("Samples.Execute")]
    [TestProperty("Time", "long")]
    public void RunShapeConstrainedRegressionSampleTest() {
      var ga = CreateShapeConstrainedRegressionSample();
      ga.SetSeedRandomly.Value = false;
      SamplesUtils.RunAlgorithm(ga);

      Assert.AreEqual(0.0780908301423225, SamplesUtils.GetDoubleResult(ga, "BestQuality"), 1E-8);
      Assert.AreEqual(0.381314019205066, SamplesUtils.GetDoubleResult(ga, "CurrentAverageQuality"), 1E-8);
      Assert.AreEqual(1.0000000000000002, SamplesUtils.GetDoubleResult(ga, "CurrentWorstQuality"), 1E-8);
      Assert.AreEqual(150200, SamplesUtils.GetIntResult(ga, "EvaluatedSolutions"));
    }

    public static GeneticAlgorithm CreateShapeConstrainedRegressionSample() {
      var alg = new GeneticAlgorithm();
      var provider = new FeynmanSmallInstanceProvider(0);
      var instance = provider.GetDataDescriptors().Where(x => x.Name.Contains("Radiated gravitational wave power: -32/5*G**4/c**5*(m1*m2)**2*(m1+m2)/r**5 | no noise")).Single();
      var problem = new ShapeConstrainedRegressionSingleObjectiveProblem();
      problem.Load(provider.LoadData(instance));

      #region Algorithm Configuration
      alg.Name = "Genetic Programming - Shape constrained Regression";
      alg.Description = "A standard genetic programming algorithm to solve a shape constrained regression problem (Radiated gravitational wave power - Feynman instance)";
      alg.Problem = problem;

      SamplesUtils.ConfigureGeneticAlgorithmParameters<TournamentSelector, SubtreeCrossover, MultiSymbolicExpressionTreeManipulator>
        (alg, popSize: 500, elites: 1, maxGens: 300, mutationRate: 0.15, tournGroupSize: 3);

      alg.Seed.Value = 0;
      #endregion

      alg.Engine = new ParallelEngine.ParallelEngine();
      return alg;
    }
  }
}
