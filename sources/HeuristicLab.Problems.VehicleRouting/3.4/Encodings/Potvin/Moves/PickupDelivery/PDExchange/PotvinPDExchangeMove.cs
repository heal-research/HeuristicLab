#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Common;
using System.Collections.Generic;
using HeuristicLab.Problems.VehicleRouting.Encodings.General;
using HeuristicLab.Data;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Optimization;
using System;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinPDExchangeMove", "Item that describes a exchange move on a PDP representation.")]
  [StorableClass]
  public class PotvinPDExchangeMove: Item, IVRPMove {
    [Storable]
    public IVRPEncoding Individual { get; protected set; }
    
    [Storable]
    public int City { get; protected set; }

    [Storable]
    public int OldTour { get; protected set; }

    [Storable]
    public int Tour { get; protected set; }

    [Storable]
    public int Replaced { get; protected set; }
   
    public PotvinPDExchangeMove(): base() {
      City = -1;
      Tour = -1;

      Individual = null;
    }

    public PotvinPDExchangeMove(int city, int oldTour, int tour, int replaced, PotvinEncoding individual) {
      City = city;
      OldTour = oldTour;
      Tour = tour;
      Replaced = replaced;

      this.Individual = individual.Clone() as PotvinEncoding;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinPDExchangeMove(this, cloner);
    }

    protected PotvinPDExchangeMove(PotvinPDExchangeMove original, Cloner cloner)
      : base(original, cloner) {
      this.City = original.City;
      this.OldTour = original.OldTour;
      this.Tour = original.Tour;
      this.Replaced = original.Replaced;

      this.Individual = cloner.Clone(Individual) as PotvinEncoding;
    }

    #region IVRPMove Members

    [ThreadStatic]
    private static PotvinPDExchangeMoveEvaluator moveEvaluator;
    public VRPMoveEvaluator GetMoveEvaluator() {
      if (moveEvaluator == null)
        moveEvaluator = new PotvinPDExchangeMoveEvaluator();

      return moveEvaluator;
    }

    [ThreadStatic]
    private static PotvinPDExchangeMoveMaker moveMaker; 
    public VRPMoveMaker GetMoveMaker() {
      if (moveMaker == null)
        moveMaker = new PotvinPDExchangeMoveMaker();

      return moveMaker;
    }

    [ThreadStatic]
    private static PotvinPDExchangeMoveTabuMaker tabuMaker;
    public ITabuMaker GetTabuMaker() {
      if (tabuMaker == null)
        tabuMaker = new PotvinPDExchangeMoveTabuMaker();

      return tabuMaker;
    }

    [ThreadStatic]
    private static PotvinPDExchangeTabuCriterion tabuChecker;
    public ITabuChecker GetTabuChecker() {
      if (tabuChecker == null) {
        tabuChecker = new PotvinPDExchangeTabuCriterion();
        tabuChecker.UseAspirationCriterion.Value = false;
      }

      return tabuChecker;
    }

    [ThreadStatic]
    private static PotvinPDExchangeTabuCriterion softTabuChecker;
    public ITabuChecker GetSoftTabuChecker() {
      if (softTabuChecker == null) {
        softTabuChecker = new PotvinPDExchangeTabuCriterion();
        softTabuChecker.UseAspirationCriterion.Value = true;
      }

      return softTabuChecker;
    }

    #endregion
  }
}
