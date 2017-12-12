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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.PTSP {
  [Item("Distance calculator", "Calculates the distance between two cities given as index of a coordinates matrix.")]
  [StorableClass]
  public abstract class DistanceCalculator : Item {
    [StorableConstructor]
    protected DistanceCalculator(bool deserializing) : base(deserializing) { }
    protected DistanceCalculator(DistanceCalculator original, Cloner cloner) : base(original, cloner) { }
    protected DistanceCalculator() { }

    public abstract double Calculate(int from, int to, DoubleMatrix coordinates);
  }
}
