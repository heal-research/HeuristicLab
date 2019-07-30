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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Optimization;
using HEAL.Attic;
using HeuristicLab.Problems.VehicleRouting.Encodings.General;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("AlbaTranslocationMoveTabuMaker", "An operator which makes translocation moves tabu for a VRP representation.  It is implemented as described in Alba, E. and Dorronsoro, B. (2004). Solving the Vehicle Routing Problem by Using Cellular Genetic Algorithms.")]
  [StorableType("7C706677-1688-4D5F-8C86-D2F479CADA40")]
  public sealed class AlbaTranslocationMoveTabuMaker : VRPMoveOperator, IAlbaTranslocationMoveOperator, ITabuMaker, IAlbaOperator {
    [Storable]
    private TranslocationMoveTabuMaker moveTabuMaker;
    private IPermutationMoveOperator PermutationMoveOperatorParameter {
      get { return moveTabuMaker; }
      set { moveTabuMaker = value as TranslocationMoveTabuMaker; }
    }

    public ILookupParameter<Permutation> PermutationParameter {
      get { return moveTabuMaker.PermutationParameter; }
    }

    public ILookupParameter<TranslocationMove> TranslocationMoveParameter {
      get { return moveTabuMaker.TranslocationMoveParameter; }
    }

    public override ILookupParameter VRPMoveParameter {
      get { return TranslocationMoveParameter; }
    }

    public ILookupParameter<DoubleValue> MoveQualityParameter {
      get { return moveTabuMaker.MoveQualityParameter; }
    }

    public ILookupParameter<DoubleValue> QualityParameter {
      get { return moveTabuMaker.QualityParameter; }
    }

    public IValueLookupParameter<BoolValue> MaximizationParameter {
      get { return moveTabuMaker.MaximizationParameter; }
    }

    [StorableConstructor]
    private AlbaTranslocationMoveTabuMaker(StorableConstructorFlag _) : base(_) { }

    public AlbaTranslocationMoveTabuMaker()
      : base() {
      moveTabuMaker = new TranslocationMoveTabuMaker();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new AlbaTranslocationMoveTabuMaker(this, cloner);
    }

    private AlbaTranslocationMoveTabuMaker(AlbaTranslocationMoveTabuMaker original, Cloner cloner)
      : base(original, cloner) {
      moveTabuMaker = cloner.Clone(original.moveTabuMaker);
    }

    public override IOperation InstrumentedApply() {
      IOperation next = base.InstrumentedApply();

      IVRPEncoding solution = VRPToursParameter.ActualValue;

      PermutationMoveOperatorParameter.PermutationParameter.ActualName = VRPToursParameter.ActualName;
      IAtomicOperation op = this.ExecutionContext.CreateChildOperation(PermutationMoveOperatorParameter);
      op.Operator.Execute((IExecutionContext)op, CancellationToken);

      return next;
    }
  }
}
