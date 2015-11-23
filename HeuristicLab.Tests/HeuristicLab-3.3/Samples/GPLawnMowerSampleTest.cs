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

using System.Linq;
using HeuristicLab.Algorithms.GeneticAlgorithm;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Selection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class GPLawnMowerSampleTest {
    [TestMethod]
    [TestCategory("Samples.Execute")]
    [TestProperty("Time", "long")]
    public void RunGpLawnMowerSampleTest() {
      var ga = CreateGpLawnMowerSample();
      ga.SetSeedRandomly.Value = false;
      SamplesUtils.RunAlgorithm(ga);
      Assert.AreEqual(55, SamplesUtils.GetDoubleResult(ga, "BestQuality"));
      Assert.AreEqual(49.266, SamplesUtils.GetDoubleResult(ga, "CurrentAverageQuality"));
      Assert.AreEqual(1, SamplesUtils.GetDoubleResult(ga, "CurrentWorstQuality"));
      Assert.AreEqual(50950, SamplesUtils.GetIntResult(ga, "EvaluatedSolutions"));
    }

    public GeneticAlgorithm CreateGpLawnMowerSample() {
      GeneticAlgorithm ga = new GeneticAlgorithm();

      #region Problem Configuration

      var problem = new Problems.GeneticProgramming.LawnMower.Problem();

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
