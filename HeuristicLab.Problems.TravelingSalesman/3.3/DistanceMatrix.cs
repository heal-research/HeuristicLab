#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.TravelingSalesman {
  /// <summary>
  /// Represents a distance matrix of a Traveling Salesman Problem.
  /// </summary>
  [Item("DistanceMatrix", "Represents a distance matrix of a Traveling Salesman Problem.")]
  [StorableClass]
  public sealed class DistanceMatrix : DoubleMatrix {
    [StorableConstructor]
    private DistanceMatrix(bool deserializing) : base(deserializing) { }
    private DistanceMatrix(DistanceMatrix original, Cloner cloner) {
      throw new NotSupportedException("Distance matrices cannot be cloned.");
    }
    public DistanceMatrix() : base() { }
    public DistanceMatrix(int rows, int columns) : base(rows, columns) { }
    public DistanceMatrix(int rows, int columns, IEnumerable<string> columnNames) : base(rows, columns, columnNames) { }
    public DistanceMatrix(int rows, int columns, IEnumerable<string> columnNames, IEnumerable<string> rowNames) : base(rows, columns, columnNames, rowNames) { }
    public DistanceMatrix(double[,] elements) : base(elements) { }
    public DistanceMatrix(double[,] elements, IEnumerable<string> columnNames) : base(elements, columnNames) { }
    public DistanceMatrix(double[,] elements, IEnumerable<string> columnNames, IEnumerable<string> rowNames) : base(elements, columnNames, rowNames) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      // distance matrices are not cloned for performance reasons
      cloner.RegisterClonedObject(this, this);
      return this;
    }
  }
}
