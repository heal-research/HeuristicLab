#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Random;
using HeuristicLab.GP.Interfaces;

namespace HeuristicLab.GP.Operators {
  /// <summary>
  /// Implementation of a homologous crossover operator as described in: 
  /// William B. Langdon 
  /// Size Fair and Homologous Tree Genetic Programming Crossovers, 
  /// Genetic Programming and Evolvable Machines, Vol. 1, Number 1/2, pp. 95-119, April 2000
  /// </summary>
  public class LangdonHomologousCrossOver : SizeFairCrossOver {
    protected override IFunctionTree SelectReplacement(MersenneTwister random, List<int> replacedTrail, List<CrossoverPoint> crossoverPoints) {
      List<CrossoverPoint> bestPoints = new List<CrossoverPoint> { crossoverPoints[0] };
      int bestMatchLength = MatchingSteps(replacedTrail, crossoverPoints[0].trail);
      for (int i = 1; i < crossoverPoints.Count; i++) {
        int currentMatchLength = MatchingSteps(replacedTrail, crossoverPoints[i].trail);
        if (currentMatchLength > bestMatchLength) {
          bestMatchLength = currentMatchLength;
          bestPoints.Clear();
          bestPoints.Add(crossoverPoints[i]);
        } else if (currentMatchLength == bestMatchLength) {
          bestPoints.Add(crossoverPoints[i]);
        }
      }
      return bestPoints[random.Next(bestPoints.Count)].tree;
    }
    private int MatchingSteps(List<int> t1, List<int> t2) {
      int n = Math.Min(t1.Count, t2.Count);
      for (int i = 0; i < n; i++) if (t1[i] != t2[i]) return i;
      return n;
    }
  }
}
