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

using System.Threading;
using HeuristicLab.Algorithms.GeneticAlgorithm;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Optimization.Tests {
  [TestClass()]
  public class AlgorithmEventsTest {
    /// <summary>
    ///A test for EvaluateFunction
    ///</summary>
    [TestMethod]
    [TestCategory("Problems.Optimization")]
    public void ExecutionStateEventRegistrationTest() {
      var alg = new EventTestAlgorithm();
      var problem = new EventTestProblem();
      Assert.AreEqual(0,problem.State);
      alg.Problem = problem;
      Assert.AreEqual(1,problem.State);
      alg.Prepare();
      Assert.AreEqual(1,problem.State);
      alg.Start(CancellationToken.None);
      Assert.AreEqual(4,problem.State);
    }

    /// <summary>
    ///A test for EvaluateFunction
    ///</summary>
    [TestMethod]
    [TestCategory("Problems.Optimization")]
    public void ExecutionStateEventDeregistrationTest() {
      var alg = new EventTestAlgorithm();
      var problem = new EventTestProblem();
      var problem2 = new EventTestProblem();

      Assert.AreEqual(0,problem.State);
      alg.Problem = problem;
      alg.Problem = problem2;
      Assert.AreEqual(1,problem.State);
      alg.Prepare();
      Assert.AreEqual(1,problem.State);
      alg.Start(CancellationToken.None);
      Assert.AreEqual(1,problem.State);
    }
  }
}