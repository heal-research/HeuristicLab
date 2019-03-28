#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Problems.Binary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ParameterlessPopulationPyramid.Test {
  [TestClass]
  public class ParameterlessPopulationPyramidTest {

    // Utility function that sets up and executes the run, then asserts the results
    private PrivateObject DoRun(BinaryProblem problem, int maximumEvaluations, int seed, double bestQuality, int foundOn) {
      var solver = new HeuristicLab.Algorithms.ParameterlessPopulationPyramid.ParameterlessPopulationPyramid();
      solver.Problem = problem;
      solver.MaximumEvaluations = maximumEvaluations;
      solver.Seed = seed;
      solver.SetSeedRandomly = false;
      PrivateObject hidden = new PrivateObject(solver);
      try {
        var ct = new CancellationToken();
        hidden.Invoke("Initialize", ct);
        hidden.Invoke("Run", ct);
      } catch (OperationCanceledException) {
        // Ignore
      }

      Assert.AreEqual(maximumEvaluations, hidden.GetProperty("ResultsEvaluations"), "Total Evaluations");
      double foundQuality = (double)hidden.GetProperty("ResultsBestQuality");
      Assert.IsTrue(foundQuality.IsAlmost(bestQuality), string.Format("Expected <{0}> Actual <{1}>", bestQuality, foundQuality));
      Assert.AreEqual(foundOn, hidden.GetProperty("ResultsBestFoundOnEvaluation"), "Found On");

      return hidden;
    }

    [TestMethod]
    [TestProperty("Time", "medium")]
    [TestCategory("Algorithms.ParameterlessPopulationPyramid")]
    public void P3DeceptiveTrap() {
      var problem = new DeceptiveTrapProblem();
      problem.Length = 49;
      problem.TrapSize = 7;
      DoRun(problem, 100, 123, 0.857142857142857, 50);
      DoRun(problem, 9876, 123, 0.918367346938776, 981);
      DoRun(problem, 20000, 987, 1, 19977);
      problem.Length = 700;
      DoRun(problem, 100000, 987, 0.941428571428571, 96901);
    }

    // Unlike DeceptiveTrap, DeceptiveStepTrap likely contains neutral (fitness equal) modifications.
    [TestMethod]
    [TestProperty("Time", "medium")]
    [TestCategory("Algorithms.ParameterlessPopulationPyramid")]
    public void P3DeceptiveStepTrap() {
      var problem = new DeceptiveStepTrapProblem();
      problem.Length = 49;
      problem.TrapSize = 7;
      problem.StepSize = 2;
      DoRun(problem, 100, 123, 0.5, 6);
      DoRun(problem, 9876, 123, 0.785714285714286, 3489);
      DoRun(problem, 70000, 987, 1, 68292);
      problem.Length = 700;
      DoRun(problem, 100000, 987, 0.76, 58711);
    }

    // Unlike the Trap tests, HIFF uses higher order linkage learning.
    [TestMethod]
    [TestProperty("Time", "medium")]
    [TestCategory("Algorithms.ParameterlessPopulationPyramid")]
    public void P3HIFF() {
      var problem = new HIFFProblem();
      problem.Length = 32;
      DoRun(problem, 50, 12345, 0.375, 26);
      DoRun(problem, 1000, 12345, 1, 976);
      problem.Length = 512;
      DoRun(problem, 1000, 54321, 0.17361111111111, 440);
      DoRun(problem, 130000, 54321, 1, 89214);
    }
  }
}
