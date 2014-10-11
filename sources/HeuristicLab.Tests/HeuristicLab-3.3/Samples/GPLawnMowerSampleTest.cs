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
using HeuristicLab.Algorithms.GeneticAlgorithm;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Selection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  /// <summary>
  /// Summary description for GPLawnMowerSampleTest
  /// </summary>
  [TestClass]
  public class GPLawnMowerSampleTest {
    private const string samplesDirectory = SamplesUtils.Directory;
    [ClassInitialize]
    public static void MyClassInitialize(TestContext testContext) {
      if (!Directory.Exists(samplesDirectory))
        Directory.CreateDirectory(samplesDirectory);
    }

    [TestMethod]
    [TestCategory("Samples.Execute")]
    [TestProperty("Time", "long")]
    public void RunGpLawnMowerSampleTest() {
      var ga = CreateGpLawnMowerSample();
      ga.SetSeedRandomly.Value = false;
      SamplesUtils.RunAlgorithm(ga);
    }

    public GeneticAlgorithm CreateGpLawnMowerSample() {
      GeneticAlgorithm ga = new GeneticAlgorithm();

      #region Problem Configuration

      var problem = new HeuristicLab.Problems.LawnMower.Problem();

      #endregion

      #region Algorithm Configuration

      ga.Name = "Genetic Programming - Lawn Mower";
      ga.Description = "A standard genetic programming algorithm to solve the lawn mower problem";
      ga.Problem = problem;
      SamplesUtils
        .ConfigureGeneticAlgorithmParameters
        <TournamentSelector, SubtreeCrossover, MultiSymbolicExpressionTreeArchitectureManipulator>(
          ga, 1000, 1, 50, 0.25, 5);
      var mutator = (MultiSymbolicExpressionTreeArchitectureManipulator)ga.Mutator;
      mutator.Operators.SetItemCheckedState(mutator.Operators
        .OfType<OnePointShaker>()
        .Single(), false);

      #endregion

      return ga;
    }
  }
}
