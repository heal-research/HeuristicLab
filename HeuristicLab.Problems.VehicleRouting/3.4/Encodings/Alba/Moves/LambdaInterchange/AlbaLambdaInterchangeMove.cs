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
using HeuristicLab.Optimization;
using HEAL.Attic;
using HeuristicLab.Problems.VehicleRouting.Encodings.General;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("AlbaLambdaInterchangeMove", "Item that describes a lambda move on a VRP representation.  It is implemented as described in Alba, E. and Dorronsoro, B. (2004). Solving the Vehicle Routing Problem by Using Cellular Genetic Algorithms.")]
  [StorableType("81F387D5-917B-45FB-842A-2DD305B8E313")]
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

    [StorableConstructor]
    protected AlbaLambdaInterchangeMove(StorableConstructorFlag _) : base(_) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new AlbaLambdaInterchangeMove(this, cloner);
    }

    protected AlbaLambdaInterchangeMove(AlbaLambdaInterchangeMove original, Cloner cloner)
      : base(original, cloner) {
      this.Tour1 = original.Tour1;
      this.Position1 = original.Position1;
      this.Length1 = original.Length1;

      this.Tour2 = original.Tour2;
      this.Position2 = original.Position2;
      this.Length2 = original.Length2;

      this.Individual = cloner.Clone(Individual) as AlbaEncoding;
    }

    #region IVRPMove Members

    public VRPMoveEvaluator GetMoveEvaluator() {
      return new AlbaLambdaInterchangeMoveEvaluator();
    }

    public VRPMoveMaker GetMoveMaker() {
      return new AlbaLambdaInterchangeMoveMaker();
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
