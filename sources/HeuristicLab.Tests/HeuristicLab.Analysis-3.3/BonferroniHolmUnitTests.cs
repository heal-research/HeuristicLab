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

using System;
using HeuristicLab.Analysis.Statistics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Statistics.UnitTests {
  [TestClass]
  public class BonferroniHolmUnitTest {
    [TestMethod]
    public void BonferroniHolmUnitTest1() {
      /* example taken from 
       * http://www.mathworks.com/matlabcentral/fileexchange/28303-bonferroni-holm-correction-for-multiple-comparisons
       * 
       * p = 0.56, 0.22, 0.001, 0.04, 0.01
       * a = 0.05
       * cor_p = 0.560, 0.440, 0.005, 0.120, 0.040
       * h = 0, 0, 1, 0, 1
       * 
       */

      double[] correctedPValues = new double[] { 0.56, 0.44, 0.005, 0.12, 0.04 };
      double[] pVals = new[] { 0.56, 0.22, 0.001, 0.04, 0.01 };
      bool[] h = new bool[] { false, false, true, false, true };
      bool[] decision;

      var result = BonferroniHolm.Calculate(0.05, pVals, out decision);

      for (int i = 0; i < pVals.Length; i++) {
        Assert.AreEqual(correctedPValues[i], result[i]);
        Assert.AreEqual(h[i], decision[i]);
      }
    }

    [TestMethod]
    public void BonferroniHolmUnitTest2() {
      /* example taken from 
       * http://en.wikipedia.org/wiki/Holm-Bonferroni_method#Example
       * 
       * p = 0.01, 0.04, 0.03,  0.005
       * a = 0.05
       * cor_p = 0.03, 0.06, 0.06, 0.02
       * h = 1, 0, 0, 1
       * 
       */

      double[] correctedPValues = new double[] { 0.03, 0.06, 0.06, 0.02 };
      double[] pVals = new[] { 0.01, 0.04, 0.03, 0.005 };
      bool[] h = new bool[] { true, false, false, true };
      bool[] decision;

      var result = BonferroniHolm.Calculate(0.05, pVals, out decision);

      for (int i = 0; i < pVals.Length; i++) {
        Assert.AreEqual(correctedPValues[i], result[i]);
        Assert.AreEqual(h[i], decision[i]);
      }
    }

    [TestMethod]
    public void BonferroniHolmUnitTest3() {
      // comparison with R's p.adjust(p, "holm") method
      double[] correctedPValues = new double[] { 0.23262159, 0.05204139 };
      double[] pVals = new[] { 0.232621592948806, 0.0260206949805373 };
      bool[] decision;

      var result = BonferroniHolm.Calculate(0.05, pVals, out decision);

      for (int i = 0; i < pVals.Length; i++) {
        Assert.AreEqual(correctedPValues[i], Math.Round(result[i], 8));
      }
    }

    [TestMethod]
    public void BonferroniHolmUnitTest4() {
      // comparison with R's p.adjust(p, "holm") method
      double[] correctedPValues = new double[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
      double[] pVals = new[] { 0.1108139, 0.1241641, 0.2805913, 0.3715633, 0.3967397, 0.6165547, 0.7272018, 0.8774432, 0.9495787, 0.9755199 };
      bool[] decision;

      var result = BonferroniHolm.Calculate(0.05, pVals, out decision);

      for (int i = 0; i < pVals.Length; i++) {
        Assert.AreEqual(correctedPValues[i], Math.Round(result[i], 8));
      }
    }

    [TestMethod]
    public void BonferroniHolmUnitTest5() {
      // comparison with R's p.adjust(p, "holm") method
      double[] correctedPValues = new double[] { 1.389563e-05, 7.293675e-05, 2.330999e-04, 2.330999e-04, 4.370736e-04, 5.539326e-04 };
      double[] pVals = new[] { 2.315938e-06, 1.458735e-05, 5.827497e-05, 7.166659e-05, 2.185368e-04, 5.539326e-04 };
      bool[] decision;

      var result = BonferroniHolm.Calculate(0.05, pVals, out decision);

      for (int i = 0; i < pVals.Length; i++) {
        Assert.AreEqual(Math.Round(correctedPValues[i], 10), Math.Round(result[i], 10));
      }
    }
  }
}

