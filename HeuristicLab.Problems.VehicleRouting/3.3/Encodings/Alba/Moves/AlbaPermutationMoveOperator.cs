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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("AlbaPermutationMoveOperator", "A move operator for an Alba VRP representation using an inner permutation move operator.")]
  [StorableClass]
  public abstract class AlbaPermutationMoveOperator : AlbaMoveOperator {
    [Storable]
    protected abstract IPermutationMoveOperator PermutationMoveOperatorParameter { get; set; }

    [StorableConstructor]
    protected AlbaPermutationMoveOperator(bool deserializing) : base(deserializing) { }
    protected AlbaPermutationMoveOperator(AlbaPermutationMoveOperator original, Cloner cloner) : base(original, cloner) { }
    public AlbaPermutationMoveOperator()
      : base() {
    }

    public override IOperation Apply() {
      IOperation next = base.Apply();

      IVRPEncoding solution = VRPToursParameter.ActualValue;

      PermutationMoveOperatorParameter.PermutationParameter.ActualName = VRPToursParameter.ActualName;
      IAtomicOperation op = this.ExecutionContext.CreateChildOperation(PermutationMoveOperatorParameter);
      op.Operator.Execute((IExecutionContext)op, CancellationToken);

      return next;
    }
  }
}
