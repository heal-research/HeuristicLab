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

namespace HeuristicLab.RealVector {
  public class ContinuousManipulator : RealVectorManipulatorBase {
    public override string Description {
      get { return "This operator randomly selects two elements of the vector, calculates its average value and replaces the first selected element with the new value."; }
    }

    public static double[] Apply(IRandom random, double[] vector) {
      int length = vector.Length;
      int index1, index2;
      index1 = random.Next(length);
      index2 = random.Next(length);

      while (index2 == index1) {
        index2 = random.Next(length);
      }

      vector[index1] = (vector[index1] + vector[index2]) / 2;

      return vector;
    }

    protected override double[] Manipulate(IScope scope, IRandom random, double[] vector) {
      return Apply(random, vector);
    }
  }
}
