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
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.IntegerVectorEncoding {
  [Item("BoundedIntegerVectorManipulator", "Base class for bounded integer vector manipulators.")]
  [StorableClass]
  public abstract class BoundedIntegerVectorManipulator : IntegerVectorManipulator, IBoundedIntegerVectorOperator {

    public IValueLookupParameter<IntMatrix> BoundsParameter {
      get { return (IValueLookupParameter<IntMatrix>)Parameters["Bounds"]; }
    }

    [StorableConstructor]
    protected BoundedIntegerVectorManipulator(bool deserializing) : base(deserializing) { }
    protected BoundedIntegerVectorManipulator(BoundedIntegerVectorManipulator original, Cloner cloner) : base(original, cloner) { }
    public BoundedIntegerVectorManipulator()
      : base() {
      Parameters.Add(new ValueLookupParameter<IntMatrix>("Bounds", "The bounds matrix can contain one row for each dimension with three columns specifying minimum (inclusive), maximum (exclusive), and step size. If less rows are given the matrix is cycled."));
    }

    protected sealed override void Manipulate(IRandom random, IntegerVector integerVector) {
      if (BoundsParameter.ActualValue == null) throw new InvalidOperationException("BoundedIntegerVectorManipulator: Parameter " + BoundsParameter.ActualName + " could not be found.");
      ManipulateBounded(random, integerVector, BoundsParameter.ActualValue);
    }

    protected abstract void ManipulateBounded(IRandom random, IntegerVector integerVector, IntMatrix bounds);
  }
}
