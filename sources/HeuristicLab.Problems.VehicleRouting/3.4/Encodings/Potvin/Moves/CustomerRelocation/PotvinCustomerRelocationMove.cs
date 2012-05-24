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

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinCustomerRelocationMove", "Item that describes a relocation move on a VRP representation.")]
  [StorableClass]
  public class PotvinCustomerRelocationMove: Item, IVRPMove {
    [Storable]
    public IVRPEncoding Individual { get; protected set; }
    
    [Storable]
    public int City { get; protected set; }

    [Storable]
    public int OldTour { get; protected set; }

    [Storable]
    public int Tour { get; protected set; }
   
    public PotvinCustomerRelocationMove(): base() {
      City = -1;
      Tour = -1;

      Individual = null;
    }

    public PotvinCustomerRelocationMove(int city, int oldTour, int tour, PotvinEncoding individual) {
      City = city;
      OldTour = oldTour;
      Tour = tour;

      this.Individual = individual.Clone() as PotvinEncoding;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinCustomerRelocationMove(this, cloner);
    }

    protected PotvinCustomerRelocationMove(PotvinCustomerRelocationMove original, Cloner cloner)
      : base(original, cloner) {
      this.City = original.City;
      this.OldTour = original.OldTour;
      this.Tour = original.Tour;

      this.Individual = cloner.Clone(Individual) as PotvinEncoding;
    }

    #region IVRPMove Members

    public VRPMoveEvaluator GetMoveEvaluator() {
      return new PotvinCustomerRelocationMoveEvaluator();
    }

    public VRPMoveMaker GetMoveMaker() {
      return new PotvinCustomerRelocationMoveMaker();
    }

    public ITabuMaker GetTabuMaker() {
      return new PotvinCustomerRelocationMoveTabuMaker();
    }

    public ITabuChecker GetTabuChecker() {
      return new PotvinCustomerRelocationMoveTabuCriterion();
    }

    public ITabuChecker GetSoftTabuChecker() {
      PotvinCustomerRelocationMoveTabuCriterion tabuChecker = new PotvinCustomerRelocationMoveTabuCriterion();
      tabuChecker.UseAspirationCriterion.Value = true;

      return tabuChecker;
    }

    #endregion
  }
}
