#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [Item("AlbaTranslocationMoveMaker", "An operator which makes translocation moves for a VRP representation.  It is implemented as described in Alba, E. and Dorronsoro, B. (2004). Solving the Vehicle Routing Problem by Using Cellular Genetic Algorithms.")]
  [StorableClass]
  public sealed class AlbaTranslocationMoveMaker : AlbaMoveMaker, IAlbaTranslocationMoveOperator {
    [Storable]
    private TranslocationMoveMaker moveMaker;

    public ILookupParameter<TranslocationMove> TranslocationMoveParameter {
      get { return moveMaker.TranslocationMoveParameter; }
    }

    public override ILookupParameter VRPMoveParameter {
      get { return TranslocationMoveParameter; }
    }

    public ILookupParameter<Permutation> PermutationParameter {
      get { return moveMaker.PermutationParameter; }
    }

    [StorableConstructor]
    private AlbaTranslocationMoveMaker(bool deserializing) : base(deserializing) { }

    public AlbaTranslocationMoveMaker()
      : base() {
      moveMaker = new TranslocationMoveMaker();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new AlbaTranslocationMoveMaker(this, cloner);
    }

    private AlbaTranslocationMoveMaker(AlbaTranslocationMoveMaker original, Cloner cloner)
      : base(original, cloner) {
      moveMaker = cloner.Clone(original.moveMaker);
    }

    protected override void PerformMove() {
      IVariable moveVariable = this.ExecutionContext.Scope.Variables[
        TranslocationMoveParameter.ActualName];
      TranslocationMove move = moveVariable.Value as TranslocationMove;
      VRPToursParameter.ActualValue = move.Permutation as AlbaEncoding;

      moveMaker.PermutationParameter.ActualName = VRPToursParameter.ActualName;
      IAtomicOperation op = this.ExecutionContext.CreateChildOperation(moveMaker);
      op.Operator.Execute((IExecutionContext)op, CancellationToken);
    }
  }
}
