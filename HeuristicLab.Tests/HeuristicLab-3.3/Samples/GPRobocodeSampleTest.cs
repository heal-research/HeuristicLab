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
using System.Linq;
using HeuristicLab.Algorithms.GeneticAlgorithm;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Problems.GeneticProgramming.Robocode;
using HeuristicLab.Selection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class GPRobocodeSampleTest {
    private const string SampleFileName = "SGP_Robocode";

    [TestMethod]
    [TestCategory("Samples.Create")]
    [TestProperty("Time", "medium")]
    public void CreateGpRobocodeSampleTest() {
      var ga = CreateGpRobocodeSample();
      string path = Path.Combine(SamplesUtils.SamplesDirectory, SampleFileName + SamplesUtils.SampleFileExtension);
      XmlGenerator.Serialize(ga, path);
    }

    // there is no unit test for running the sample because a JDK installation would be necessary (and this is not the case on our build server)

    public GeneticAlgorithm CreateGpRobocodeSample() {
      GeneticAlgorithm ga = new GeneticAlgorithm();

      #region Problem Configuration
      Problem antProblem = new Problem();
      #endregion
      #region Algorithm Configuration
      ga.Name = "Genetic Programming - Robocode Java Source";
      ga.Description = "A standard genetic programming algorithm to evolve the java source code for a robocode bot (see http://robocode.sourceforge.net/). An installation of Java SE Developmen Kit (JDK) >= 1.6 is necessary to run this sample.";
      ga.Problem = antProblem;
      SamplesUtils.ConfigureGeneticAlgorithmParameters<TournamentSelector, SubtreeCrossover, MultiSymbolicExpressionTreeArchitectureManipulator>(
        ga, 50, 1, 50, 0.15, 2);

      ga.Engine = new SequentialEngine.SequentialEngine();
      #endregion

      return ga;
    }
  }
}
