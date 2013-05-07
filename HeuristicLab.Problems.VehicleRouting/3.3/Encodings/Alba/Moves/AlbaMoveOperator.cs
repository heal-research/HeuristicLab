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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("AlbaMoveOperator", "A move operator for an Alba VRP representation.")]
  [StorableClass]
  public abstract class AlbaMoveOperator : VRPMoveOperator {
    [StorableConstructor]
    protected AlbaMoveOperator(bool deserializing) : base(deserializing) { }
    protected AlbaMoveOperator(AlbaMoveOperator original, Cloner cloner) : base(original, cloner) { }
    public AlbaMoveOperator()
      : base() {
      AlbaEncoding.RemoveUnusedParameters(Parameters);
    }

    public override IOperation Apply() {
      IVRPEncoding solution = VRPToursParameter.ActualValue;
      if (!(solution is AlbaEncoding)) {
        VRPToursParameter.ActualValue = AlbaEncoding.ConvertFrom(solution, VehiclesParameter.ActualValue.Value,
          DistanceMatrixParameter);
      }

      return base.Apply();
    }
  }
}
