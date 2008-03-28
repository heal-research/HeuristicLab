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
  public class IntermediateMultiCrossover : RealVectorMultiCrossoverBase {
    public override string Description {
      get {
        return @"This creates a new offspring by computing the centroid of a number of parents";
      }
    }

    public static double[] Apply(IList<double[]> parents) {
      int length = parents[0].Length;
      double[] result = new double[length];
      for (int i = 0; i < length; i++) {
        double sum = 0.0;
        for (int j = 0; j < parents.Count; j++)
          sum += parents[j][i];
        result[i] = sum / parents.Count;
      }
      return result;
    }

    protected override double[] Cross(IScope scope, IRandom random, IList<double[]> parents) {
      return Apply(parents);
    }
  }
}
