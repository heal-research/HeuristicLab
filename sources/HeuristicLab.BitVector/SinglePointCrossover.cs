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
using System.Text;
using HeuristicLab.Core;

namespace HeuristicLab.BitVector {
  public class SinglePointCrossover : BitVectorCrossoverBase {
    public override string Description {
      get { return "Single point crossover for bit vectors."; }
    }

    public static bool[] Apply(IRandom random, bool[] parent1, bool[] parent2) {
      int length = parent1.Length;
      bool[] result = new bool[length];
      int breakPoint = random.Next(1, length);

      for (int i = 0; i < breakPoint; i++)
        result[i] = parent1[i];
      for (int i = breakPoint; i < length; i++)
        result[i] = parent2[i];

      return result;
    }

    protected override bool[] Cross(IScope scope, IRandom random, bool[] parent1, bool[] parent2) {
      return Apply(random, parent1, parent2);
    }
  }
}
