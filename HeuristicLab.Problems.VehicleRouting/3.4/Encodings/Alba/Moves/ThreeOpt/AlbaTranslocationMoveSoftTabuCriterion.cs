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
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Encodings.General;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("AlbaTranslocationMoveSoftTabuCriterion", "An operator which checks if translocation moves are tabu using a soft criterion for a VRP representation.  It is implemented as described in Alba, E. and Dorronsoro, B. (2004). Solving the Vehicle Routing Problem by Using Cellular Genetic Algorithms.")]
  [StorableClass]
  public sealed class AlbaTranslocationMoveSoftTabuCriterion : VRPMoveOperator, IAlbaTranslocationMoveOperator, ITabuChecker, IAlbaOperator {
    [Storable]
    private TranslocationMoveSoftTabuCriterion tabuChecker;
    private IPermutationMoveOperator PermutationMoveOperatorParameter {
      get { return tabuChecker; }
      set { tabuChecker = value as TranslocationMoveSoftTabuCriterion; }
    }

    public ILookupParameter<TranslocationMove> TranslocationMoveParameter {
      get { return tabuChecker.TranslocationMoveParameter; }
    }

    public override ILookupParameter VRPMoveParameter {
      get { return TranslocationMoveParameter; }
    }

    public ILookupParameter<Permutation> PermutationParameter {
      get { return tabuChecker.PermutationParameter; }
    }

    public ILookupParameter<BoolValue> MoveTabuParameter {
      get { return tabuChecker.MoveTabuParameter; }
    }

    public ILookupParameter<DoubleValue> MoveQualityParameter {
      get { return tabuChecker.MoveQualityParameter; }
    }

    public IValueLookupParameter<BoolValue> MaximizationParameter {
      get { return tabuChecker.MaximizationParameter; }
    }

    [StorableConstructor]
    private AlbaTranslocationMoveSoftTabuCriterion(bool deserializing) : base(deserializing) { }

    public AlbaTranslocationMoveSoftTabuCriterion()
      : base() {
      tabuChecker = new TranslocationMoveSoftTabuCriterion();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new AlbaTranslocationMoveSoftTabuCriterion(this, cloner);
    }

    private AlbaTranslocationMoveSoftTabuCriterion(AlbaTranslocationMoveSoftTabuCriterion original, Cloner cloner)
      : base(original, cloner) {
      tabuChecker = cloner.Clone(original.tabuChecker);
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
