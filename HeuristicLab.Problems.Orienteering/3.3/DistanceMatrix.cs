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

namespace HeuristicLab.Problems.Orienteering {
  /// <summary>
  /// Represents a distance matrix of a Orienteering Salesman Problem.
  /// </summary>
  [Item("DistanceMatrix", "Represents a distance matrix of a Orienteering Problem.")]
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

    public double CalculateTourLength(IList<int> path, double pointVisitingCosts) {
      double length = 0.0;
      for (int i = 1; i < path.Count; i++)
        length += this[path[i - 1], path[i]];
      // Add the fixed penalty for every vertex except for the starting and ending vertex
      length += (path.Count - 2) * pointVisitingCosts;
      return length;
    }
    public double CalculateInsertionCosts(IList<int> path, int insertPosition, int point, double pointVisitingCosts) {
      double detour = this[path[insertPosition - 1], point] + this[point, path[insertPosition]];
      detour += pointVisitingCosts;
      detour -= this[path[insertPosition - 1], path[insertPosition]];
      return detour;
    }
    public double CalculateReplacementCosts(List<int> path, int replacePosition, int point) {
      double detour = this[path[replacePosition - 1], point] + this[point, path[replacePosition + 1]];
      detour -= this[path[replacePosition - 1], path[replacePosition]] + this[path[replacePosition], path[replacePosition + 1]];
      return detour;
    }
    public double CalculateRemovementSaving(List<int> path, int removePosition, double pointVisitingCosts) {
      double saving = this[path[removePosition - 1], path[removePosition]];
      saving += this[path[removePosition], path[removePosition + 1]];
      saving -= this[path[removePosition - 1], path[removePosition + 1]];
      saving += pointVisitingCosts;
      return saving;
    }
  }
}