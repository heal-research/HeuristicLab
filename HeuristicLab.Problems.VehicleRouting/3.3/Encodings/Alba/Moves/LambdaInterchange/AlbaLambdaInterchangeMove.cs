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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Encodings.General;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("InversionMove", "Item that describes a lambda move on a VRP representation.  It is implemented as described in Alba, E. and Dorronsoro, B. (2004). Solving the Vehicle Routing Problem by Using Cellular Genetic Algorithms.")]
  [StorableClass]
  public class AlbaLambdaInterchangeMove : Item, IVRPMove {
    [Storable]
    public IVRPEncoding Individual { get; protected set; }

    [Storable]
    public int Tour1 { get; protected set; }

    [Storable]
    public int Position1 { get; protected set; }

    [Storable]
    public int Length1 { get; protected set; }

    [Storable]
    public int Tour2 { get; protected set; }

    [Storable]
    public int Position2 { get; protected set; }

    [Storable]
    public int Length2 { get; protected set; }

    [StorableConstructor]
    protected AlbaLambdaInterchangeMove(bool deserializing) : base(deserializing) { }
    protected AlbaLambdaInterchangeMove(AlbaLambdaInterchangeMove original, Cloner cloner)
      : base(original, cloner) {
      Tour1 = original.Tour1;
      Position1 = original.Position1;
      Length1 = original.Length1;

      Tour2 = original.Tour2;
      Position2 = original.Position2;
      Length2 = original.Length2;

      Individual = cloner.Clone(original.Individual);
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new AlbaLambdaInterchangeMove(this, cloner);
    }

    public AlbaLambdaInterchangeMove()
      : base() {
      Tour1 = -1;
      Position1 = -1;
      Length1 = -1;

      Tour2 = -1;
      Position2 = -1;
      Length2 = -1;

      Individual = null;
    }

    public AlbaLambdaInterchangeMove(int tour1, int position1, int length1,
      int tour2, int position2, int length2, AlbaEncoding permutation) {
      Tour1 = tour1;
      Position1 = position1;
      Length1 = length1;

      Tour2 = tour2;
      Position2 = position2;
      Length2 = length2;

      this.Individual = permutation.Clone() as AlbaEncoding;
    }

    #region IVRPMove Members

    public TourEvaluation GetMoveQuality(
      IntValue vehicles,
      DoubleArray dueTimeArray, DoubleArray serviceTimeArray, DoubleArray readyTimeArray,
      DoubleArray demandArray, DoubleValue capacity, DoubleMatrix coordinates,
      DoubleValue fleetUsageFactor, DoubleValue timeFactor, DoubleValue distanceFactor,
      DoubleValue overloadPenalty, DoubleValue tardinessPenalty,
      ILookupParameter<DoubleMatrix> distanceMatrix, Data.BoolValue useDistanceMatrix) {
      return AlbaLambdaInterchangeMoveEvaluator.GetMoveQuality(Individual as AlbaEncoding, this, vehicles,
        dueTimeArray, serviceTimeArray, readyTimeArray, demandArray, capacity,
        coordinates, fleetUsageFactor, timeFactor, distanceFactor,
        overloadPenalty, tardinessPenalty, distanceMatrix, useDistanceMatrix);
    }

    public IVRPEncoding MakeMove() {
      AlbaLambdaInterchangeMoveMaker.Apply(Individual as AlbaEncoding, this);

      return Individual;
    }

    #endregion
  }
}
