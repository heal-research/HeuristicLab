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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinVehicleAssignmentMoveMaker", "Peforms the vehicle assignment move on a given VRP encoding and updates the quality.")]
  [StorableClass]
  public class PotvinVehicleAssignmentMoveMaker : PotvinMoveMaker, IPotvinVehicleAssignmentMoveOperator, IMoveMaker {
    public ILookupParameter<PotvinVehicleAssignmentMove> VehicleAssignmentMoveParameter {
      get { return (ILookupParameter<PotvinVehicleAssignmentMove>)Parameters["PotvinVehicleAssignmentMove"]; }
    }

    public override ILookupParameter VRPMoveParameter {
      get { return VehicleAssignmentMoveParameter; }
    }

    public ILookupParameter<VariableCollection> MemoriesParameter {
      get { return (ILookupParameter<VariableCollection>)Parameters["Memories"]; }
    }

    public IValueParameter<StringValue> AdditionFrequencyMemoryKeyParameter {
      get { return (IValueParameter<StringValue>)Parameters["AdditionFrequencyMemoryKey"]; }
    }

    [StorableConstructor]
    protected PotvinVehicleAssignmentMoveMaker(bool deserializing) : base(deserializing) { }

    public PotvinVehicleAssignmentMoveMaker()
      : base() {
      Parameters.Add(new LookupParameter<PotvinVehicleAssignmentMove>("PotvinVehicleAssignmentMove", "The moves that should be made."));

      Parameters.Add(new LookupParameter<VariableCollection>("Memories", "The TS memory collection."));
      Parameters.Add(new ValueParameter<StringValue>("AdditionFrequencyMemoryKey", "The key that is used for the addition frequency in the TS memory.", new StringValue("AdditionFrequency")));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinVehicleAssignmentMoveMaker(this, cloner);
    }

    protected PotvinVehicleAssignmentMoveMaker(PotvinVehicleAssignmentMoveMaker original, Cloner cloner)
      : base(original, cloner) {
    }

    public static void Apply(PotvinEncoding solution, PotvinVehicleAssignmentMove move, IVRPProblemInstance problemInstance) {
      int vehicle1 = solution.VehicleAssignment[move.Tour1];
      int vehicle2 = solution.VehicleAssignment[move.Tour2];

      solution.VehicleAssignment[move.Tour1] = vehicle2;
      solution.VehicleAssignment[move.Tour2] = vehicle1;
    }

    protected override void PerformMove() {
      PotvinVehicleAssignmentMove move = VehicleAssignmentMoveParameter.ActualValue;

      PotvinEncoding newSolution = move.Individual.Clone() as PotvinEncoding;
      Apply(newSolution, move, ProblemInstance);
      newSolution.Repair();
      VRPToursParameter.ActualValue = newSolution;
    }
  }
}
