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
using HeuristicLab.Data;

namespace HeuristicLab.RealVector {
  class LocalCrossover : RealVectorCrossoverBase {
    public override string Description {
      get { return "Local crossover for real vectors."; }
    }

    public static double[] Apply(IRandom random, double[] parent1, double[] parent2) {
      double factor;
      int length = parent1.Length;
      double[] result = new double[length];

      for (int i = 0; i < length; i++) {
        factor = random.NextDouble();
        result[i] = (factor * parent1[i]) + ((1 - factor) * parent2[i]);
      }
      return result;
    }

    protected override double[] Cross(IScope scope, IRandom random, double[] parent1, double[] parent2) {
      return Apply(random, parent1, parent2);
    }
  }
}
