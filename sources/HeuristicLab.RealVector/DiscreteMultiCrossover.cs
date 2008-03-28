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
  public class DiscreteMultiCrossover : RealVectorMultiCrossoverBase {
    public override string Description {
      get {
        return @"This creates a new offspring by combining the alleles in the parents such that each allele is randomly selected from one parent";
      }
    }

    public static double[] Apply(IRandom random, IList<double[]> parents) {
      int length = parents[0].Length;
      double[] result = new double[length];
      try {
        for (int i = 0; i < length; i++) {
          result[i] = parents[random.Next(parents.Count)][i];
        }
      } catch (IndexOutOfRangeException) {
        throw new InvalidOperationException("Cannot apply multicrossover to real vectors of different length.");
      }
      return result;
    }

    protected override double[] Cross(IScope scope, IRandom random, IList<double[]> parents) {
      return Apply(random, parents);
    }
  }
}
