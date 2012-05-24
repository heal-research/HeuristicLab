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
  [Item("PotvinVehicleAssignmentMove", "Item that describes a relocation move on a VRP representation.")]
  [StorableClass]
  public class PotvinVehicleAssignmentMove: Item, IVRPMove {
    [Storable]
    public IVRPEncoding Individual { get; protected set; }
    
    [Storable]
    public int Tour1 { get; protected set; }

    [Storable]
    public int Tour2 { get; protected set; }

   
    public PotvinVehicleAssignmentMove(): base() {
      Tour1 = -1;
      Tour2 = -1;

      Individual = null;
    }

    public PotvinVehicleAssignmentMove(int tour1, int tour2, PotvinEncoding individual) {
      Tour1 = tour1;
      Tour2 = tour2;

      this.Individual = individual.Clone() as PotvinEncoding;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinVehicleAssignmentMove(this, cloner);
    }

    protected PotvinVehicleAssignmentMove(PotvinVehicleAssignmentMove original, Cloner cloner)
      : base(original, cloner) {
        this.Tour1 = original.Tour1;
        this.Tour2 = original.Tour2;

      this.Individual = cloner.Clone(Individual) as PotvinEncoding;
    }

    #region IVRPMove Members

    public VRPMoveEvaluator GetMoveEvaluator() {
      return new PotvinVehicleAssignmentMoveEvaluator();
    }

    public VRPMoveMaker GetMoveMaker() {
      return new PotvinVehicleAssignmentMoveMaker();
    }

    public ITabuMaker GetTabuMaker() {
      return new PotvinVehicleAssignmentMoveTabuMaker();
    }

    public ITabuChecker GetTabuChecker() {
      return new PotvinVehicleAssignmentMoveTabuCriterion();
    }

    public ITabuChecker GetSoftTabuChecker() {
      PotvinVehicleAssignmentMoveTabuCriterion tabuChecker = new PotvinVehicleAssignmentMoveTabuCriterion();
      tabuChecker.UseAspirationCriterion.Value = true;

      return tabuChecker;
    }

    #endregion
  }
}
