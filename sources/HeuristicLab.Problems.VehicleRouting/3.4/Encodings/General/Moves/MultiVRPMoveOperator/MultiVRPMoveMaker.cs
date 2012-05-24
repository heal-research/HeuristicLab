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
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using System.Collections.Generic;
using HeuristicLab.Problems.VehicleRouting.Variants;
using HeuristicLab.Common;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.General{
  [Item("MultiVRPMoveMaker", "Peforms a lambda interchange moves on a given VRP encoding and updates the quality.")]
  [StorableClass]
  public class MultiVRPMoveMaker : VRPMoveMaker, IMultiVRPMoveOperator, IGeneralVRPOperator {
    public override ILookupParameter VRPMoveParameter {
      get { return (ILookupParameter)Parameters["VRPMove"]; }
    }
    
    [StorableConstructor]
    private MultiVRPMoveMaker(bool deserializing) : base(deserializing) { }

    public MultiVRPMoveMaker()
      : base() {
      Parameters.Add(new LookupParameter<IVRPMove>("VRPMove", "The move."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiVRPMoveMaker(this, cloner);
    }

    protected MultiVRPMoveMaker(MultiVRPMoveMaker original, Cloner cloner)
      : base(original, cloner) {
    }

    protected override void PerformMove() {
      IVRPMove move = VRPMoveParameter.ActualValue as IVRPMove;

      VRPMoveMaker moveMaker = move.GetMoveMaker();
      moveMaker.VRPMoveParameter.ActualName = VRPMoveParameter.Name;
      IAtomicOperation op = this.ExecutionContext.CreateOperation(moveMaker);
      op.Operator.Execute((IExecutionContext)op, CancellationToken);
    }
  }
}
