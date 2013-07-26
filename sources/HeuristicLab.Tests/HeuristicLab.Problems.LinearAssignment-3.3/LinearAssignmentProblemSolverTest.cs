#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Problems.LinearAssignment.Tests_33 {
  /// <summary>
  ///This is a test class for LinearAssignmentProblemSolverTest and is intended
  ///to contain all LinearAssignmentProblemSolverTest Unit Tests
  ///</summary>
  [TestClass()]
  public class LinearAssignmentProblemSolverTest {

    private TestContext testContextInstance;

    /// <summary>
    ///Gets or sets the test context which provides
    ///information about and functionality for the current test run.
    ///</summary>
    public TestContext TestContext {
      get { return testContextInstance; }
      set { testContextInstance = value; }
    }


    /// <summary>
    ///A test for Solve
    ///</summary>
    [TestMethod]
    public void SolveTest() {
      double[,] costs = new double[,] {
        {  5,  9,  3,  6 },
        {  8,  7,  8,  2 },
        {  6, 10, 12,  7 },
        {  3, 10,  8,  6 }};
      double quality;
      Permutation p;
      p = new Permutation(PermutationTypes.Absolute, LinearAssignmentProblemSolver.Solve(new DoubleMatrix(costs), out quality));
      Assert.AreEqual(18, quality);
      Assert.IsTrue(p.Validate());

      costs = new double[,] {
        { 11,  7, 10, 17, 10 },
        { 13, 21,  7, 11, 13 },
        { 13, 13, 15, 13, 14 },
        { 18, 10, 13, 16, 14 },
        { 12,  8, 16, 19, 10 }};

      p = new Permutation(PermutationTypes.Absolute, LinearAssignmentProblemSolver.Solve(new DoubleMatrix(costs), out quality));
      Assert.AreEqual(51, quality);
      Assert.IsTrue(p.Validate());

      costs = new double[,] {
        {  3,  1,  1,  4 },
        {  4,  2,  2,  5 },
        {  5,  3,  4,  8 },
        {  4,  2,  5,  9 }};

      p = new Permutation(PermutationTypes.Absolute, LinearAssignmentProblemSolver.Solve(new DoubleMatrix(costs), out quality));
      Assert.AreEqual(13, quality);
      Assert.IsTrue(p.Validate());
    }
  }
}
