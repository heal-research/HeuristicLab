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
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Problems.BinPacking3D;
using HeuristicLab.Selection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class GABppSampleTest {
    private const string SampleFileName = "GA_BPP";

    [TestMethod]
    [TestCategory("Samples.Create")]
    [TestProperty("Time", "medium")]
    public void CreateGaBppSampleTest() {
      var ga = CreateGaBppSample();
      string path = Path.Combine(SamplesUtils.SamplesDirectory, SampleFileName + SamplesUtils.SampleFileExtension);
      XmlGenerator.Serialize(ga, path);
    }

    [TestMethod]
    [TestCategory("Samples.Execute")]
    [TestProperty("Time", "long")]
    public void RunGaBppSampleTest() {
      var ga = CreateGaBppSample();
      ga.SetSeedRandomly.Value = false;
      SamplesUtils.RunAlgorithm(ga);
      Assert.AreEqual(0.59369488300408157, SamplesUtils.GetDoubleResult(ga, "BestQuality"), 1E-6);
      Assert.AreEqual(0.51834350243004124, SamplesUtils.GetDoubleResult(ga, "CurrentAverageQuality"),1E-6);
      Assert.AreEqual(0.36227753691428577, SamplesUtils.GetDoubleResult(ga, "CurrentWorstQuality"),1E-6);
      Assert.AreEqual(15250, SamplesUtils.GetIntResult(ga, "EvaluatedSolutions"));
    }

    private GeneticAlgorithm CreateGaBppSample() {
      GeneticAlgorithm ga = new GeneticAlgorithm();

      #region Problem Configuration
      var bpp = new PermutationProblem();
      #endregion
      #region Algorithm Configuration
      ga.Name = "Genetic Algorithm - Bin Packing Problem (3D)";
      ga.Description = "A genetic algorithm which solves the a 3d bin packing problem instance";
      ga.Problem = bpp;
      SamplesUtils.ConfigureGeneticAlgorithmParameters<TournamentSelector, PartiallyMatchedCrossover, Swap2Manipulator>(
        ga, 300, 1, 50, 0.05, 3);
      #endregion

      return ga;
    }
  }
}
