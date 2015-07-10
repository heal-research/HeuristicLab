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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.TravelingSalesman {
  /// <summary>
  /// An operator to evaluate translocation or insertion moves (3-opt).
  /// </summary>
  [Item("TSPTranslocationMoveEuclideanPathEvaluator", "Operator for evaluating a translocation or insertion move (3-opt) based on euclidean distances.")]
  [StorableClass]
  public class TSPTranslocationMoveEuclideanPathEvaluator : TSPTranslocationMovePathEvaluator {
    [StorableConstructor]
    protected TSPTranslocationMoveEuclideanPathEvaluator(bool deserializing) : base(deserializing) { }
    protected TSPTranslocationMoveEuclideanPathEvaluator(TSPTranslocationMoveEuclideanPathEvaluator original, Cloner cloner) : base(original, cloner) { }
    public TSPTranslocationMoveEuclideanPathEvaluator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new TSPTranslocationMoveEuclideanPathEvaluator(this, cloner);
    }
    
    public override Type EvaluatorType {
      get { return typeof(TSPEuclideanPathEvaluator); }
    }

    protected override double CalculateDistance(double x1, double y1, double x2, double y2) {
      return Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
    }
  }
}
