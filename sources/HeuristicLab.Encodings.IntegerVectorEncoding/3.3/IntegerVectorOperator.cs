#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Operators;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.IntegerVectorEncoding {
  [Item("IntegerVectorOperator", "Base class for integer vectoro operators.")]
  [StorableClass]
  public abstract class IntegerVectorOperator : InstrumentedOperator, IIntegerVectorOperator {

    [StorableConstructor]
    protected IntegerVectorOperator(bool deserializing) : base(deserializing) { }
    protected IntegerVectorOperator(IntegerVectorOperator original, Cloner cloner) : base(original, cloner) { }
    public IntegerVectorOperator() : base() { }

    public static int RoundFeasible(int min, int max, int step, double value) {
      return Math.Max(min, Math.Min(max, (int)Math.Round((value - min) / (double)step) * step + min));
    }
    public static int FloorFeasible(int min, int max, int step, double value) {
      return Math.Max(min, Math.Min(max, (int)Math.Floor((value - min) / (double)step) * step + min));
    }
    public static int CeilingFeasible(int min, int max, int step, double value) {
      return Math.Max(min, Math.Min(max, (int)Math.Ceiling((value - min) / (double)step) * step + min));
    }
  }
}
