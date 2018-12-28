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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HEAL.Fossil;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("AlbaStochasticTranslocationSingleMoveGenerator", "An operator which generates a single translocation move for a VRP representation.  It is implemented as described in Alba, E. and Dorronsoro, B. (2004). Solving the Vehicle Routing Problem by Using Cellular Genetic Algorithms.")]
  [StorableType("AB2D93A5-BECF-4ABE-B704-5DA883426CAF")]
  public sealed class AlbaStochasticTranslocationSingleMoveGenerator : AlbaMoveGenerator, IAlbaTranslocationMoveOperator {
    [Storable]
    private TranslocationMoveGenerator generator = new StochasticTranslocationSingleMoveGenerator();

    public ILookupParameter<TranslocationMove> TranslocationMoveParameter {
      get {
        return generator.TranslocationMoveParameter;
      }
    }

    public override ILookupParameter VRPMoveParameter {
      get { return TranslocationMoveParameter; }
    }

    public ILookupParameter<Permutation> PermutationParameter {
      get {
        return generator.PermutationParameter;
      }
    }

    public IValueLookupParameter<IntValue> SampleSizeParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["SampleSize"]; }
    }

    [StorableConstructor]
    private AlbaStochasticTranslocationSingleMoveGenerator(StorableConstructorFlag _) : base(_) { }

    public AlbaStochasticTranslocationSingleMoveGenerator()
      : base() {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new AlbaStochasticTranslocationSingleMoveGenerator(this, cloner);
    }

    private AlbaStochasticTranslocationSingleMoveGenerator(AlbaStochasticTranslocationSingleMoveGenerator original, Cloner cloner)
      : base(original, cloner) {
    }

    public override IOperation InstrumentedApply() {
      IOperation next = base.InstrumentedApply();

      IVRPEncoding solution = VRPToursParameter.ActualValue;

      generator.PermutationParameter.ActualName = VRPToursParameter.ActualName;
      IAtomicOperation op = this.ExecutionContext.CreateChildOperation(generator);
      op.Operator.Execute((IExecutionContext)op, CancellationToken);

      foreach (IScope scope in this.ExecutionContext.Scope.SubScopes) {
        IVariable moveVariable = scope.Variables[
          TranslocationMoveParameter.ActualName];

        if (moveVariable.Value is TranslocationMove &&
          !(moveVariable.Value is AlbaTranslocationMove)) {
          TranslocationMove move = moveVariable.Value as TranslocationMove;
          moveVariable.Value =
            new AlbaTranslocationMove(
              move.Index1, move.Index2, move.Index3, solution as AlbaEncoding);
        }
      }

      return next;
    }
  }
}
