#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Common;
using HeuristicLab.Problems.VehicleRouting.Encodings.General;
using HeuristicLab.Data;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("InversionMove", "Item that describes a simple local search move on an Alba VRP representation.")]
  [StorableClass]
  public class SimpleLocalSearchMove : TwoIndexMove, IVRPMove {
    public SimpleLocalSearchMove()
      : base() {
    }

    public SimpleLocalSearchMove(int index1, int index2)
      : base(index1, index2, null) {
    }

    public SimpleLocalSearchMove(int index1, int index2, AlbaEncoding permutation)
      : base(index1, index2, permutation) {
    }

    public override IDeepCloneable Clone(HeuristicLab.Common.Cloner cloner) {
      SimpleLocalSearchMove clone = new SimpleLocalSearchMove(
        Index1, Index2);

      if (Permutation != null)
        clone.Permutation = (AlbaEncoding)cloner.Clone(Permutation);

      cloner.RegisterClonedObject(this, clone);
      return clone;
    }

    #region IVRPMove Members

    public TourEvaluation GetMoveQuality(
      IVRPEncoding individual, 
      DoubleArray dueTimeArray, DoubleArray serviceTimeArray, DoubleArray readyTimeArray, 
      DoubleArray demandArray, DoubleValue capacity, DoubleMatrix coordinates,
      DoubleValue fleetUsageFactor, DoubleValue timeFactor, DoubleValue distanceFactor,
      DoubleValue overloadPenalty, DoubleValue tardinessPenalty,
      ILookupParameter<DoubleMatrix> distanceMatrix, Data.BoolValue useDistanceMatrix) {
        return SimpleLocalSearchMoveEvaluator.GetMoveQuality(individual as AlbaEncoding, this,
          dueTimeArray, serviceTimeArray, readyTimeArray, demandArray, capacity,
          coordinates, fleetUsageFactor, timeFactor, distanceFactor,
          overloadPenalty, tardinessPenalty, distanceMatrix, useDistanceMatrix);
    }

    public void MakeMove(IRandom random, IVRPEncoding individual) {
      SimpleLocalSearchMoveMaker.Apply(individual as AlbaEncoding, this);
    }

    #endregion
  }
}
