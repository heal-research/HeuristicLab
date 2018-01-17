#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Random;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Problems.QuadraticAssignment.Tests {
  /// <summary>
  ///This is a test class for the QAP move evaluators
  ///</summary>
  [TestClass()]
  public class QAPSwapMoveEvaluatorTest {
    private const int ProblemSize = 10;
    private static DoubleMatrix symmetricDistances, symmetricWeights, asymmetricDistances, asymmetricWeights, nonZeroDiagonalWeights, nonZeroDiagonalDistances;
    private static Permutation assignment;
    private static MersenneTwister random;

    private TestContext testContextInstance;
    /// <summary>
    ///Gets or sets the test context which provides
    ///information about and functionality for the current test run.
    ///</summary>
    public TestContext TestContext {
      get { return testContextInstance; }
      set { testContextInstance = value; }
    }

    [ClassInitialize]
    public static void MyClassInitialize(TestContext testContext) {
      random = new MersenneTwister();
      symmetricDistances = new DoubleMatrix(ProblemSize, ProblemSize);
      symmetricWeights = new DoubleMatrix(ProblemSize, ProblemSize);
      asymmetricDistances = new DoubleMatrix(ProblemSize, ProblemSize);
      asymmetricWeights = new DoubleMatrix(ProblemSize, ProblemSize);
      nonZeroDiagonalDistances = new DoubleMatrix(ProblemSize, ProblemSize);
      nonZeroDiagonalWeights = new DoubleMatrix(ProblemSize, ProblemSize);
      for (int i = 0; i < ProblemSize - 1; i++) {
        for (int j = i + 1; j < ProblemSize; j++) {
          symmetricDistances[i, j] = random.Next(ProblemSize * 100);
          symmetricDistances[j, i] = symmetricDistances[i, j];
          symmetricWeights[i, j] = random.Next(ProblemSize * 100);
          symmetricWeights[j, i] = symmetricWeights[i, j];
          asymmetricDistances[i, j] = random.Next(ProblemSize * 100);
          asymmetricDistances[j, i] = random.Next(ProblemSize * 100);
          asymmetricWeights[i, j] = random.Next(ProblemSize * 100);
          asymmetricWeights[j, i] = random.Next(ProblemSize * 100);
          nonZeroDiagonalDistances[i, j] = random.Next(ProblemSize * 100);
          nonZeroDiagonalDistances[j, i] = random.Next(ProblemSize * 100);
          nonZeroDiagonalWeights[i, j] = random.Next(ProblemSize * 100);
          nonZeroDiagonalWeights[j, i] = random.Next(ProblemSize * 100);
        }
        nonZeroDiagonalDistances[i, i] = random.Next(ProblemSize * 100);
        nonZeroDiagonalWeights[i, i] = random.Next(ProblemSize * 100);
      }
      int index = random.Next(ProblemSize);
      if (nonZeroDiagonalDistances[index, index] == 0)
        nonZeroDiagonalDistances[index, index] = random.Next(1, ProblemSize * 100);
      index = random.Next(ProblemSize);
      if (nonZeroDiagonalWeights[index, index] == 0)
        nonZeroDiagonalWeights[index, index] = random.Next(1, ProblemSize * 100);
      assignment = new Permutation(PermutationTypes.Absolute, ProblemSize, random);
    }

    [TestMethod]
    [TestCategory("Problems.Assignment")]
    [TestProperty("Time", "short")]
    public void Swap2MoveEvaluatorFastEvaluationTest() {

      for (int i = 0; i < 500; i++) {
        Swap2Move lastMove = new Swap2Move(random.Next(ProblemSize), random.Next(ProblemSize));
        Permutation prevAssignment = (Permutation)assignment.Clone();
        Swap2Manipulator.Apply(assignment, lastMove.Index1, lastMove.Index2);
        Permutation nextAssignment = (Permutation)assignment.Clone();
        Swap2Move currentMove = new Swap2Move(random.Next(ProblemSize), random.Next(ProblemSize));
        Swap2Manipulator.Apply(nextAssignment, currentMove.Index1, currentMove.Index2);

        double moveBefore = QAPSwap2MoveEvaluator.Apply(prevAssignment, currentMove, symmetricWeights, symmetricDistances);
        double moveAfter = QAPSwap2MoveEvaluator.Apply(assignment, currentMove,
                moveBefore, symmetricWeights, symmetricDistances, lastMove);
        double before = QAPEvaluator.Apply(assignment, symmetricWeights, symmetricDistances);
        double after = QAPEvaluator.Apply(nextAssignment, symmetricWeights, symmetricDistances);

        Assert.IsTrue(moveAfter.IsAlmost(after - before), "Failed on symmetric matrices: " + Environment.NewLine
          + "Quality changed from " + before + " to " + after + " (" + (after - before).ToString() + "), but move quality change was " + moveAfter + ".");

        moveBefore = QAPSwap2MoveEvaluator.Apply(prevAssignment, currentMove, asymmetricWeights, asymmetricDistances);
        moveAfter = QAPSwap2MoveEvaluator.Apply(assignment, currentMove,
                moveBefore, asymmetricWeights, asymmetricDistances, lastMove);
        before = QAPEvaluator.Apply(assignment, asymmetricWeights, asymmetricDistances);
        after = QAPEvaluator.Apply(nextAssignment, asymmetricWeights, asymmetricDistances);

        Assert.IsTrue(moveAfter.IsAlmost(after - before), "Failed on asymmetric matrices: " + Environment.NewLine
          + "Quality changed from " + before + " to " + after + " (" + (after - before).ToString() + "), but move quality change was " + moveAfter + ".");

        moveBefore = QAPSwap2MoveEvaluator.Apply(prevAssignment, currentMove, nonZeroDiagonalWeights, nonZeroDiagonalDistances);
        moveAfter = QAPSwap2MoveEvaluator.Apply(assignment, currentMove,
                moveBefore, nonZeroDiagonalWeights, nonZeroDiagonalDistances, lastMove);
        before = QAPEvaluator.Apply(assignment, nonZeroDiagonalWeights, nonZeroDiagonalDistances);
        after = QAPEvaluator.Apply(nextAssignment, nonZeroDiagonalWeights, nonZeroDiagonalDistances);

        Assert.IsTrue(moveAfter.IsAlmost(after - before), "Failed on non-zero diagonal matrices: " + Environment.NewLine
          + "Quality changed from " + before + " to " + after + " (" + (after - before).ToString() + "), but move quality change was " + moveAfter + ".");
      }
    }

    [TestMethod]
    [TestCategory("Problems.Assignment")]
    [TestProperty("Time", "short")]
    public void Swap2MoveEvaluatorTest() {
      for (int i = 0; i < 500; i++) {
        int index1 = random.Next(ProblemSize);
        int index2 = random.Next(ProblemSize);

        // SYMMETRIC MATRICES
        double before = QAPEvaluator.Apply(assignment, symmetricWeights, symmetricDistances);
        Swap2Manipulator.Apply(assignment, index1, index2);
        double after = QAPEvaluator.Apply(assignment, symmetricWeights, symmetricDistances);
        double move = QAPSwap2MoveEvaluator.Apply(assignment, new Swap2Move(index1, index2, assignment), symmetricWeights, symmetricDistances);
        Assert.IsTrue(move.IsAlmost(before - after), "Failed on symmetric matrices");

        // ASYMMETRIC MATRICES
        before = QAPEvaluator.Apply(assignment, asymmetricWeights, asymmetricDistances);
        Permutation clone = (Permutation)assignment.Clone();
        Swap2Manipulator.Apply(assignment, index1, index2);
        after = QAPEvaluator.Apply(assignment, asymmetricWeights, asymmetricDistances);
        move = QAPSwap2MoveEvaluator.Apply(clone, new Swap2Move(index1, index2), asymmetricWeights, asymmetricDistances);
        Assert.IsTrue(move.IsAlmost(after - before), "Failed on asymmetric matrices");

        // NON-ZERO DIAGONAL ASSYMETRIC MATRICES
        before = QAPEvaluator.Apply(assignment, nonZeroDiagonalWeights, nonZeroDiagonalDistances);
        clone = (Permutation)assignment.Clone();
        Swap2Manipulator.Apply(assignment, index1, index2);
        after = QAPEvaluator.Apply(assignment, nonZeroDiagonalWeights, nonZeroDiagonalDistances);
        move = QAPSwap2MoveEvaluator.Apply(clone, new Swap2Move(index1, index2), nonZeroDiagonalWeights, nonZeroDiagonalDistances);
        Assert.IsTrue(move.IsAlmost(after - before), "Failed on non-zero diagonal matrices");
      }
    }

    [TestMethod]
    [TestCategory("Problems.Assignment")]
    [TestProperty("Time", "short")]
    public void InversionMoveEvaluatorTest() {
      for (int i = 0; i < 500; i++) {
        int index1 = random.Next(ProblemSize);
        int index2 = random.Next(ProblemSize);
        if (index1 > index2) { int h = index1; index1 = index2; index2 = h; }

        // SYMMETRIC MATRICES
        double before = QAPEvaluator.Apply(assignment, symmetricWeights, symmetricDistances);
        InversionManipulator.Apply(assignment, Math.Min(index1, index2), Math.Max(index1, index2));
        double after = QAPEvaluator.Apply(assignment, symmetricWeights, symmetricDistances);
        double move = QAPInversionMoveEvaluator.Apply(assignment, new InversionMove(index1, index2, assignment), symmetricWeights, symmetricDistances);
        Assert.IsTrue(move.IsAlmost(before - after), "Failed on symmetric matrices");

        // ASYMMETRIC MATRICES
        before = QAPEvaluator.Apply(assignment, asymmetricWeights, asymmetricDistances);
        Permutation clone = (Permutation)assignment.Clone();
        InversionManipulator.Apply(assignment, index1, index2);
        after = QAPEvaluator.Apply(assignment, asymmetricWeights, asymmetricDistances);
        move = QAPInversionMoveEvaluator.Apply(clone, new InversionMove(index1, index2), asymmetricWeights, asymmetricDistances);
        Assert.IsTrue(move.IsAlmost(after - before), "Failed on asymmetric matrices");

        // NON-ZERO DIAGONAL ASYMMETRIC MATRICES
        before = QAPEvaluator.Apply(assignment, nonZeroDiagonalWeights, nonZeroDiagonalDistances);
        clone = (Permutation)assignment.Clone();
        InversionManipulator.Apply(assignment, index1, index2);
        after = QAPEvaluator.Apply(assignment, nonZeroDiagonalWeights, nonZeroDiagonalDistances);
        move = QAPInversionMoveEvaluator.Apply(clone, new InversionMove(index1, index2), nonZeroDiagonalWeights, nonZeroDiagonalDistances);
        Assert.IsTrue(move.IsAlmost(after - before), "Failed on non-zero diagonal matrices");
      }
    }

    [TestMethod]
    [TestCategory("Problems.Assignment")]
    [TestProperty("Time", "short")]
    public void TranslocationMoveEvaluatorTest() {
      for (int i = 0; i < 500; i++) {
        int index1 = random.Next(assignment.Length - 1);
        int index2 = random.Next(index1 + 1, assignment.Length);
        int insertPointLimit = assignment.Length - index2 + index1 - 1;  // get insertion point in remaining part
        int insertPoint;
        if (insertPointLimit > 0)
          insertPoint = random.Next(insertPointLimit);
        else
          insertPoint = 0;

        // SYMMETRIC MATRICES
        double before = QAPEvaluator.Apply(assignment, symmetricWeights, symmetricDistances);
        Permutation clone = new Cloner().Clone(assignment);
        TranslocationManipulator.Apply(assignment, index1, index2, insertPoint);
        double after = QAPEvaluator.Apply(assignment, symmetricWeights, symmetricDistances);
        double move = QAPTranslocationMoveEvaluator.Apply(clone, new TranslocationMove(index1, index2, insertPoint, assignment), symmetricWeights, symmetricDistances);
        Assert.IsTrue(move.IsAlmost(after - before), "Failed on symmetric matrices");

        // ASYMMETRIC MATRICES
        before = QAPEvaluator.Apply(assignment, asymmetricWeights, asymmetricDistances);
        clone = new Cloner().Clone(assignment);
        TranslocationManipulator.Apply(assignment, index1, index2, insertPoint);
        after = QAPEvaluator.Apply(assignment, asymmetricWeights, asymmetricDistances);
        move = QAPTranslocationMoveEvaluator.Apply(clone, new TranslocationMove(index1, index2, insertPoint, assignment), asymmetricWeights, asymmetricDistances);
        Assert.IsTrue(move.IsAlmost(after - before), "Failed on asymmetric matrices");

        // NON-ZERO DIAGONAL ASYMMETRIC MATRICES
        before = QAPEvaluator.Apply(assignment, nonZeroDiagonalWeights, nonZeroDiagonalDistances);
        clone = new Cloner().Clone(assignment);
        TranslocationManipulator.Apply(assignment, index1, index2, insertPoint);
        after = QAPEvaluator.Apply(assignment, nonZeroDiagonalWeights, nonZeroDiagonalDistances);
        move = QAPTranslocationMoveEvaluator.Apply(clone, new TranslocationMove(index1, index2, insertPoint, assignment), nonZeroDiagonalWeights, nonZeroDiagonalDistances);
        Assert.IsTrue(move.IsAlmost(after - before), "Failed on non-zero diagonal matrices");
      }
    }

    [TestMethod]
    [TestCategory("Problems.Assignment")]
    [TestProperty("Time", "short")]
    public void ScrambleMoveEvaluatorTest() {
      for (int i = 0; i < 500; i++) {
        ScrambleMove scramble = StochasticScrambleMultiMoveGenerator.GenerateRandomMove(assignment, random);

        // SYMMETRIC MATRICES
        double before = QAPEvaluator.Apply(assignment, symmetricWeights, symmetricDistances);
        double move = QAPScrambleMoveEvaluator.Apply(assignment, scramble, symmetricWeights, symmetricDistances);
        ScrambleManipulator.Apply(assignment, scramble.StartIndex, scramble.ScrambledIndices);
        double after = QAPEvaluator.Apply(assignment, symmetricWeights, symmetricDistances);
        Assert.IsTrue(move.IsAlmost(after - before), "Failed on symmetric matrices");

        // ASYMMETRIC MATRICES
        before = QAPEvaluator.Apply(assignment, asymmetricWeights, asymmetricDistances);
        move = QAPScrambleMoveEvaluator.Apply(assignment, scramble, asymmetricWeights, asymmetricDistances);
        ScrambleManipulator.Apply(assignment, scramble.StartIndex, scramble.ScrambledIndices);
        after = QAPEvaluator.Apply(assignment, asymmetricWeights, asymmetricDistances);
        Assert.IsTrue(move.IsAlmost(after - before), "Failed on asymmetric matrices");

        // NON-ZERO DIAGONAL ASYMMETRIC MATRICES
        before = QAPEvaluator.Apply(assignment, nonZeroDiagonalWeights, nonZeroDiagonalDistances);
        move = QAPScrambleMoveEvaluator.Apply(assignment, scramble, nonZeroDiagonalWeights, nonZeroDiagonalDistances);
        ScrambleManipulator.Apply(assignment, scramble.StartIndex, scramble.ScrambledIndices);
        after = QAPEvaluator.Apply(assignment, nonZeroDiagonalWeights, nonZeroDiagonalDistances);
        Assert.IsTrue(move.IsAlmost(after - before), "Failed on non-zero diagonal matrices");
      }
    }

  }
}
