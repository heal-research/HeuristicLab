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

using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.IntegerVectorEncoding;
using HeuristicLab.Optimization;
using HEAL.Fossil;

namespace HeuristicLab.Problems.Orienteering {
  [StorableType("4F942163-D321-4935-BF2E-780193BE9FFE")]
  public interface IOrienteeringEvaluator : ISingleObjectiveEvaluator {
    ILookupParameter<IntegerVector> IntegerVectorParameter { get; }
    ILookupParameter<DoubleArray> ScoresParameter { get; }
    ILookupParameter<DistanceMatrix> DistanceMatrixParameter { get; }
    ILookupParameter<DoubleValue> MaximumDistanceParameter { get; }
    ILookupParameter<DoubleValue> PointVisitingCostsParameter { get; }
    ILookupParameter<DoubleValue> PenaltyParameter { get; }

    OrienteeringEvaluationResult Evaluate(IntegerVector solution, DoubleArray scores,
      DistanceMatrix distances, double maximumDistance, double pointVisitingCosts);
  }
}