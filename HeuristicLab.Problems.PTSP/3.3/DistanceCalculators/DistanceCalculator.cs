#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;

namespace HeuristicLab.Problems.PTSP {
  [Item("Distance calculator", "Calculates the distance between two cities given as index of a coordinates matrix.")]
  [StorableType("7CC7E665-A6A7-478B-B978-7079E1DDBA70")]
  public abstract class DistanceCalculator : Item {
    [StorableConstructor]
    protected DistanceCalculator(StorableConstructorFlag _) : base(_) { }
    protected DistanceCalculator(DistanceCalculator original, Cloner cloner) : base(original, cloner) { }
    protected DistanceCalculator() { }

    public abstract double Calculate(int from, int to, DoubleMatrix coordinates);
  }
}
