#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [Item("PotvinPDShiftMove", "Item that describes a shift move on a PDP representation.")]
  [StorableClass]
  public class PotvinPDShiftMove : Item, IVRPMove {
    [Storable]
    public IVRPEncoding Individual { get; protected set; }

    [Storable]
    public int City { get; protected set; }

    [Storable]
    public int OldTour { get; protected set; }

    [Storable]
    public int Tour { get; protected set; }

    public PotvinPDShiftMove()
      : base() {
      City = -1;
      Tour = -1;

      Individual = null;
    }

    public PotvinPDShiftMove(int city, int oldTour, int tour, PotvinEncoding individual) {
      City = city;
      OldTour = oldTour;
      Tour = tour;

      this.Individual = individual.Clone() as PotvinEncoding;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinPDShiftMove(this, cloner);
    }

    protected PotvinPDShiftMove(PotvinPDShiftMove original, Cloner cloner)
      : base(original, cloner) {
      this.City = original.City;
      this.OldTour = original.OldTour;
      this.Tour = original.Tour;

      this.Individual = cloner.Clone(Individual) as PotvinEncoding;
    }

    [StorableConstructor]
    protected PotvinPDShiftMove(bool deserializing) : base(deserializing) { }

    #region IVRPMove Members

    [ThreadStatic]
    private static PotvinPDShiftMoveEvaluator moveEvaluator;
    public VRPMoveEvaluator GetMoveEvaluator() {
      if (moveEvaluator == null)
        moveEvaluator = new PotvinPDShiftMoveEvaluator();

      return moveEvaluator;
    }

    [ThreadStatic]
    private static PotvinPDShiftMoveMaker moveMaker;
    public VRPMoveMaker GetMoveMaker() {
      if (moveMaker == null)
        moveMaker = new PotvinPDShiftMoveMaker();

      return moveMaker;
    }

    [ThreadStatic]
    private static PotvinPDShiftMoveTabuMaker tabuMaker;
    public ITabuMaker GetTabuMaker() {
      if (tabuMaker == null)
        tabuMaker = new PotvinPDShiftMoveTabuMaker();

      return tabuMaker;
    }

    [ThreadStatic]
    private static PotvinPDShiftTabuCriterion tabuChecker;
    public ITabuChecker GetTabuChecker() {
      if (tabuChecker == null) {
        tabuChecker = new PotvinPDShiftTabuCriterion();
        tabuChecker.UseAspirationCriterion.Value = false;
      }

      return tabuChecker;
    }

    [ThreadStatic]
    private static PotvinPDShiftTabuCriterion softTabuChecker;
    public ITabuChecker GetSoftTabuChecker() {
      if (softTabuChecker == null) {
        softTabuChecker = new PotvinPDShiftTabuCriterion();
        softTabuChecker.UseAspirationCriterion.Value = true;
      }

      return softTabuChecker;
    }
    #endregion
  }
}
