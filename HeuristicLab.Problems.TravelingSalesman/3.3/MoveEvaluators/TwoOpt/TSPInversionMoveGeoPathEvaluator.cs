#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Fossil;

namespace HeuristicLab.Problems.TravelingSalesman {
  /// <summary>
  /// An operator to evaluate inversion moves (2-opt).
  /// </summary>
  [Item("TSPInversionMoveGeoPathEvaluator", "Operator for evaluating an inversion move (2-opt) based on geo (world) distances.")]
  [StorableType("DCBB9049-6C89-4249-BB7D-314B7C62E9E6")]
  public class TSPInversionMoveGeoPathEvaluator : TSPInversionMovePathEvaluator {
    public override Type EvaluatorType {
      get { return typeof(TSPGeoPathEvaluator); }
    }

    private const double PI = 3.141592;
    private const double RADIUS = 6378.388;

    [StorableConstructor]
    protected TSPInversionMoveGeoPathEvaluator(StorableConstructorFlag _) : base(_) { }
    protected TSPInversionMoveGeoPathEvaluator(TSPInversionMoveGeoPathEvaluator original, Cloner cloner) : base(original, cloner) { }
    public TSPInversionMoveGeoPathEvaluator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new TSPInversionMoveGeoPathEvaluator(this, cloner);
    }
    
    /// <summary>
    /// Calculates the distance between two points using the GEO distance metric (globe coordinates).
    /// </summary>
    /// <param name="x1">The x-coordinate of point 1.</param>
    /// <param name="y1">The y-coordinate of point 1.</param>
    /// <param name="x2">The x-coordinate of point 2.</param>
    /// <param name="y2">The y-coordinate of point 2.</param>
    /// <returns>The calculated distance.</returns>
    protected override double CalculateDistance(double x1, double y1, double x2, double y2) {
      double latitude1, longitude1, latitude2, longitude2;
      double q1, q2, q3;
      double length;

      latitude1 = ConvertToRadian(x1);
      longitude1 = ConvertToRadian(y1);
      latitude2 = ConvertToRadian(x2);
      longitude2 = ConvertToRadian(y2);

      q1 = Math.Cos(longitude1 - longitude2);
      q2 = Math.Cos(latitude1 - latitude2);
      q3 = Math.Cos(latitude1 + latitude2);

      length = (int)(RADIUS * Math.Acos(0.5 * ((1.0 + q1) * q2 - (1.0 - q1) * q3)) + 1.0);
      return (length);
    }

    private double ConvertToRadian(double x) {
      return PI * (Math.Truncate(x) + 5.0 * (x - Math.Truncate(x)) / 3.0) / 180.0;
    }
  }
}
