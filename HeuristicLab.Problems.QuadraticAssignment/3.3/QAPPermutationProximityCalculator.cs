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

using System.Collections.Generic;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;

namespace HeuristicLab.Problems.QuadraticAssignment {
  public static class QAPPermutationProximityCalculator {

    public static double CalculatePhenotypeSimilarity(Permutation a, Permutation b, DoubleMatrix weights, DoubleMatrix distances) {
      return 1.0 - CalculatePhenotypeDistance(a, b, weights, distances);
    }

    public static double CalculatePhenotypeDistance(Permutation a, Permutation b, DoubleMatrix weights, DoubleMatrix distances) {
      Dictionary<double, Dictionary<double, int>> alleles = new Dictionary<double, Dictionary<double, int>>();
      int distance = 0, len = a.Length;
      for (int x = 0; x < len; x++) {
        for (int y = 0; y < len; y++) {
          // there's a limited universe of double values as they're all drawn from the same matrix
          double dA = distances[a[x], a[y]], dB = distances[b[x], b[y]];
          if (dA == dB) continue;

          Dictionary<double, int> dAlleles;
          if (!alleles.ContainsKey(weights[x, y])) {
            dAlleles = new Dictionary<double, int>();
            alleles.Add(weights[x, y], dAlleles);
          } else dAlleles = alleles[weights[x, y]];

          int countA = 1, countB = -1;

          if (dAlleles.ContainsKey(dA)) countA += dAlleles[dA];
          if (dAlleles.ContainsKey(dB)) countB += dAlleles[dB];

          if (countA <= 0) distance--; // we've found in A an allele that was present in B
          else distance++; // we've found in A a new allele
          dAlleles[dA] = countA;

          if (countB >= 0) distance--; // we've found in B an allele that was present in A
          else distance++; // we've found in B a new allele
          dAlleles[dB] = countB;
        }
      }
      return distance / (double)(2 * len * len);
    }
  }
}
