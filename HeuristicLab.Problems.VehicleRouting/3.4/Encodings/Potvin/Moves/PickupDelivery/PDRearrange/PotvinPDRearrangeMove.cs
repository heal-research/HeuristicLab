#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Encodings.General;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinPDRearrangeMove", "Item that describes a rearrange move on a PDP representation.")]
  [StorableClass]
  public class PotvinPDRearrangeMove : Item, IVRPMove {
    [Storable]
    public IVRPEncoding Individual { get; protected set; }

    [Storable]
    public int City { get; protected set; }

    [Storable]
    public int Tour { get; protected set; }

    public PotvinPDRearrangeMove()
      : base() {
      City = -1;
      Tour = -1;

      Individual = null;
    }

    public PotvinPDRearrangeMove(int city, int tour, PotvinEncoding individual) {
      City = city;
      Tour = tour;

      this.Individual = individual.Clone() as PotvinEncoding;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinPDRearrangeMove(this, cloner);
    }

    protected PotvinPDRearrangeMove(PotvinPDRearrangeMove original, Cloner cloner)
      : base(original, cloner) {
      this.City = original.City;
      this.Tour = original.Tour;

      this.Individual = cloner.Clone(Individual) as PotvinEncoding;
    }

    [StorableConstructor]
    protected PotvinPDRearrangeMove(bool deserializing) : base(deserializing) { }

    #region IVRPMove Members

    [ThreadStatic]
    private static PotvinPDRearrangeMoveEvaluator moveEvaluator;
    public VRPMoveEvaluator GetMoveEvaluator() {
      if (moveEvaluator == null)
        moveEvaluator = new PotvinPDRearrangeMoveEvaluator();

      return moveEvaluator;
    }

    [ThreadStatic]
    private static PotvinPDRearrangeMoveMaker moveMaker;
    public VRPMoveMaker GetMoveMaker() {
      if (moveMaker == null)
        moveMaker = new PotvinPDRearrangeMoveMaker();

      return moveMaker;
    }

    [ThreadStatic]
    private static PotvinPDRearrangeMoveTabuMaker tabuMaker;
    public ITabuMaker GetTabuMaker() {
      if (tabuMaker == null)
        tabuMaker = new PotvinPDRearrangeMoveTabuMaker();

      return tabuMaker;
    }

    [ThreadStatic]
    private static PotvinPDRearrangeTabuCriterion tabuChecker;
    public ITabuChecker GetTabuChecker() {
      if (tabuChecker == null) {
        tabuChecker = new PotvinPDRearrangeTabuCriterion();
        tabuChecker.UseAspirationCriterion.Value = false;
      }

      return tabuChecker;
    }

    [ThreadStatic]
    private static PotvinPDRearrangeTabuCriterion softTabuChecker;
    public ITabuChecker GetSoftTabuChecker() {
      if (softTabuChecker == null) {
        softTabuChecker = new PotvinPDRearrangeTabuCriterion();
        softTabuChecker.UseAspirationCriterion.Value = true;
      }

      return softTabuChecker;
    }

    #endregion
  }
}
