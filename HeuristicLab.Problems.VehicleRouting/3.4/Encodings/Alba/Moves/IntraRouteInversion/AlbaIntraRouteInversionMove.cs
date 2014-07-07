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
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Encodings.General;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("AlbaIntraRouteInversionMove", "Item that describes an intra route inversion move on a VRP representation.  It is implemented as described in Alba, E. and Dorronsoro, B. (2004). Solving the Vehicle Routing Problem by Using Cellular Genetic Algorithms.")]
  [StorableClass]
  public class AlbaIntraRouteInversionMove : TwoIndexMove, IVRPMove {
    public IVRPEncoding Individual { get { return Permutation as AlbaEncoding; } }

    [StorableConstructor]
    protected AlbaIntraRouteInversionMove(bool deserializing) : base(deserializing) { }

    protected AlbaIntraRouteInversionMove(AlbaIntraRouteInversionMove original, Cloner cloner)
      : base(original, cloner) {
    }

    public AlbaIntraRouteInversionMove()
      : base() {
    }

    public AlbaIntraRouteInversionMove(int index1, int index2)
      : base(index1, index2, null) {
    }

    public AlbaIntraRouteInversionMove(int index1, int index2, AlbaEncoding permutation)
      : base(index1, index2, null) {
      this.Permutation = permutation.Clone() as AlbaEncoding;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new AlbaIntraRouteInversionMove(this, cloner);
    }

    #region IVRPMove Members

    public VRPMoveEvaluator GetMoveEvaluator() {
      return new AlbaIntraRouteInversionMoveEvaluator();
    }

    public VRPMoveMaker GetMoveMaker() {
      return new AlbaIntraRouteInversionMoveMaker();
    }

    public ITabuMaker GetTabuMaker() {
      return null;
    }

    public ITabuChecker GetTabuChecker() {
      return null;
    }

    public ITabuChecker GetSoftTabuChecker() {
      return null;
    }

    #endregion
  }
}
