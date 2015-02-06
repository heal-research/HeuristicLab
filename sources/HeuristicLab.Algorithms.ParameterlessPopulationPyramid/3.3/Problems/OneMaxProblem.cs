#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 * and the BEACON Center for the Study of Evolution in Action.
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.ParameterlessPopulationPyramid.Problems {
  // This code is based off the publication
  // B. W. Goldman and W. F. Punch, "Parameter-less Population Pyramid," GECCO, pp. 785–792, 2014
  // and the original source code in C++11 available from: https://github.com/brianwgoldman/Parameter-less_Population_Pyramid
  [Item("Test One Max Problem", "Only a test class. Should be removed in the future.")]
  [StorableClass]
  [Creatable("Parameterless Population Pyramid")]
  public class OneMaxProblem : BinaryVectorProblem {
    [StorableConstructor]
    protected OneMaxProblem(bool deserializing) : base(deserializing) { }
    protected OneMaxProblem(OneMaxProblem original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) { return new OneMaxProblem(this, cloner); }

    public override bool Maximization {
      get { return true; }
    }
    public OneMaxProblem() : base() { }

    public override double Evaluate(bool[] individual) {
      if (individual.Length != Length) throw new ArgumentException("The individual has not the correct length.");
      double quality = 0;
      for (int i = 0; i < individual.Length; i++)
        if (individual[i]) quality++;
      return quality;
    }
  }
}
