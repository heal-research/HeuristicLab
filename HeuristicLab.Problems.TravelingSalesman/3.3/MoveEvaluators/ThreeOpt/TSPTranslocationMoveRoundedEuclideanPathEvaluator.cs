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

using System;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Problems.TravelingSalesman {
  /// <summary>
  /// An operator to evaluate translocation or insertion moves (3-opt).
  /// </summary>
  [Item("TSPTranslocationMoveRoundedEuclideanPathEvaluator", "Operator for evaluating a translocation or insertion move (3-opt) based on rounded euclidean distances.")]
  [StorableType("AA52F35B-DEE3-4B21-A953-07773D4ED5BC")]
  public class TSPTranslocationMoveRoundedEuclideanPathEvaluator : TSPTranslocationMovePathEvaluator {
    [StorableConstructor]
    protected TSPTranslocationMoveRoundedEuclideanPathEvaluator(StorableConstructorFlag _) : base(_) { }
    protected TSPTranslocationMoveRoundedEuclideanPathEvaluator(TSPTranslocationMoveRoundedEuclideanPathEvaluator original, Cloner cloner) : base(original, cloner) { }
    public TSPTranslocationMoveRoundedEuclideanPathEvaluator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new TSPTranslocationMoveRoundedEuclideanPathEvaluator(this, cloner);
    }
    
    public override Type EvaluatorType {
      get { return typeof(TSPRoundedEuclideanPathEvaluator); }
    }

    protected override double CalculateDistance(double x1, double y1, double x2, double y2) {
      return Math.Round(Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)));
    }
  }
}
