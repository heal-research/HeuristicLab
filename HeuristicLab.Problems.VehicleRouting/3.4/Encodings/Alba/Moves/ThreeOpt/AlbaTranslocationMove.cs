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
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Optimization;
using HEAL.Attic;
using HeuristicLab.Problems.VehicleRouting.Encodings.General;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("AlbaTranslocationMove", "Item that describes a translocation move on a VRP representation.  It is implemented as described in Alba, E. and Dorronsoro, B. (2004). Solving the Vehicle Routing Problem by Using Cellular Genetic Algorithms.")]
  [StorableType("C9BA8BFE-712F-4575-82E6-B94C604C001B")]
  public class AlbaTranslocationMove : TranslocationMove, IVRPMove {
    public IVRPEncoding Individual { get { return Permutation as AlbaEncoding; } }

    public AlbaTranslocationMove(int index1, int index2, int index3) :
      base(index1, index2, index3) {
    }

    public AlbaTranslocationMove(int index1, int index2, int index3, AlbaEncoding individual) :
      base(index1, index2, index3, individual.Clone() as AlbaEncoding) {
    }

    [StorableConstructor]
    protected AlbaTranslocationMove(StorableConstructorFlag _) : base(_) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new AlbaTranslocationMove(this, cloner);
    }

    protected AlbaTranslocationMove(AlbaTranslocationMove original, Cloner cloner)
      : base(original, cloner) {
      this.Index1 = original.Index1;
      this.Index2 = original.Index2;
      this.Index3 = original.Index3;

      this.Permutation = cloner.Clone(Permutation) as AlbaEncoding;
    }

    #region IVRPMove Members

    public VRPMoveEvaluator GetMoveEvaluator() {
      return new AlbaTranslocationMoveEvaluator();
    }

    public VRPMoveMaker GetMoveMaker() {
      return new AlbaTranslocationMoveMaker();
    }

    public ITabuMaker GetTabuMaker() {
      return new AlbaTranslocationMoveTabuMaker();
    }

    public ITabuChecker GetTabuChecker() {
      return new AlbaTranslocationMoveHardTabuCriterion();
    }

    public ITabuChecker GetSoftTabuChecker() {
      return new AlbaTranslocationMoveSoftTabuCriterion();
    }

    #endregion
  }
}
