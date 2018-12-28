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
using HEAL.Fossil;
using HeuristicLab.Problems.VehicleRouting.Encodings.General;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinTwoOptStarMove", "Item that describes a two opt star move on a VRP representation.")]
  [StorableType("54EBE079-E698-46A6-BCD3-033C0EA3B2DB")]
  public class PotvinTwoOptStarMove : Item, IVRPMove {
    [Storable]
    public IVRPEncoding Individual { get; protected set; }

    [Storable]
    public int Tour1 { get; protected set; }

    [Storable]
    public int X1 { get; protected set; }

    [Storable]
    public int Tour2 { get; protected set; }

    [Storable]
    public int X2 { get; protected set; }

    public PotvinTwoOptStarMove()
      : base() {
      X1 = -1;
      Tour1 = -1;
      X2 = -1;
      Tour2 = -1;

      Individual = null;
    }

    public PotvinTwoOptStarMove(int tour1, int x1, int tour2, int x2, PotvinEncoding individual) {
      Tour1 = tour1;
      X1 = x1;
      Tour2 = tour2;
      X2 = x2;

      this.Individual = individual.Clone() as PotvinEncoding;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinTwoOptStarMove(this, cloner);
    }

    protected PotvinTwoOptStarMove(PotvinTwoOptStarMove original, Cloner cloner)
      : base(original, cloner) {
      this.Tour1 = original.Tour1;
      this.X1 = original.X1;
      this.Tour2 = original.Tour2;
      this.X2 = original.X2;

      this.Individual = cloner.Clone(Individual) as PotvinEncoding;
    }

    [StorableConstructor]
    protected PotvinTwoOptStarMove(StorableConstructorFlag _) : base(_) { }

    #region IVRPMove Members

    public VRPMoveEvaluator GetMoveEvaluator() {
      return new PotvinTwoOptStarMoveEvaluator();
    }

    public VRPMoveMaker GetMoveMaker() {
      return new PotvinTwoOptStarMoveMaker();
    }

    public ITabuMaker GetTabuMaker() {
      return new PotvinTwoOptStarMoveTabuMaker();
    }

    public ITabuChecker GetTabuChecker() {
      return new PotvinTwoOptStarMoveTabuCriterion();
    }

    public ITabuChecker GetSoftTabuChecker() {
      PotvinTwoOptStarMoveTabuCriterion tabuChecker = new PotvinTwoOptStarMoveTabuCriterion();
      tabuChecker.UseAspirationCriterion.Value = true;

      return tabuChecker;
    }

    #endregion
  }
}
