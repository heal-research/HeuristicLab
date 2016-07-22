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

using System;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.Binary {
  [Item("Hierararchical If and only If problem (HIFF)", "Genome evaluated in nested subsets to see if each subset contains either all 0s or all 1s.")]
  [StorableClass]
  [Creatable(CreatableAttribute.Categories.CombinatorialProblems, Priority = 220)]
  public class HIFFProblem : BinaryProblem {
    [StorableConstructor]
    protected HIFFProblem(bool deserializing) : base(deserializing) { }
    protected HIFFProblem(HIFFProblem original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new HIFFProblem(this, cloner);
    }

    public override bool Maximization {
      get { return true; }
    }

    public HIFFProblem()
      : base() {
      Encoding.Length = 64;
    }
    // In the GECCO paper, Section 4.1
    public override double Evaluate(BinaryVector individual, IRandom random) {
      int[] level = new int[individual.Length];
      int levelLength = individual.Length;

      // Initialize the level to the current solution
      for (int i = 0; i < levelLength; i++) {
        level[i] = Convert.ToInt32(individual[i]);
      }
      int power = 1;
      int nextLength = levelLength / 2;
      int total = 0;
      int maximum = 0;

      // Keep going while the next level actual has bits in it
      while (nextLength > 0) {
        int[] nextLevel = new int[nextLength];
        // Construct the next level using the current level
        for (int i = 0; i + 1 < levelLength; i += 2) {
          if (level[i] == level[i + 1] && level[i] != -1) {
            // Score points for a correct setting at this level
            total += power;
            nextLevel[i / 2] = level[i];
          } else {
            nextLevel[i / 2] = -1;
          }
          // Keep track of the maximum possible score
          maximum += power;
        }
        level = nextLevel;
        levelLength = nextLength;
        nextLength = levelLength / 2;
        power *= 2;
      }

      // Convert to percentage of total
      return (double)total / maximum;
    }
  }
}
