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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Data;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.ALPS {
  /// <summary>
  /// Defines the growth of age limits for the layers.
  /// </summary>
  [StorableType("136DA41B-0AC1-49A8-B4D9-DCE73BDB7110")]
  public enum AgingScheme {
    Linear,
    Fibonacci,
    Polynomial,
    Exponential
  }

  /// <summary>
  /// Helper for calculating the age limits for a AgingScheme and a given AgeGap.
  /// </summary>
  public static class AgingSchemeCalculator {
    public static IntArray CalculateAgeLimits(this AgingScheme scheme, int ageGap, int numberOfLayers) {
      IEnumerable<int> schemeGenerator;
      switch (scheme) {
        case AgingScheme.Linear: schemeGenerator = LinearAgingScheme(); break;
        case AgingScheme.Fibonacci: schemeGenerator = FibonacciAgingScheme(); break;
        case AgingScheme.Polynomial: schemeGenerator = PolynomialAgingScheme(2); break;
        case AgingScheme.Exponential: schemeGenerator = ExponentialAgingScheme(2); break;
        default: throw new NotSupportedException("Aging Scheme " + scheme + " is not supported.");
      }

      return new IntArray(schemeGenerator.Select(a => a * ageGap).Take(numberOfLayers).ToArray());
    }

    #region Scheme definitions
    // 1 2 3 4 5 6 7 ...
    private static IEnumerable<int> LinearAgingScheme() {
      for (int i = 0; ; i++)
        yield return i + 1;
    }
    // 1 2 3 5 8 13 21 ...
    private static IEnumerable<int> FibonacciAgingScheme() {
      for (int i = 1, next = 2, temp; ; temp = next, next = i + next, i = temp)
        yield return i;
    }
    // (n^2): 1 2 4 9 16 25 36 ...
    private static IEnumerable<int> PolynomialAgingScheme(double exp) {
      yield return 1;
      yield return 2;
      for (int i = 2; ; i++)
        yield return (int)Math.Pow(i, exp);
    }
    // 1 2 4 8 16 32 64 ...
    private static IEnumerable<int> ExponentialAgingScheme(double @base) {
      for (int i = 0; ; i++)
        yield return (int)Math.Pow(@base, i);
    }
    #endregion
  }
}