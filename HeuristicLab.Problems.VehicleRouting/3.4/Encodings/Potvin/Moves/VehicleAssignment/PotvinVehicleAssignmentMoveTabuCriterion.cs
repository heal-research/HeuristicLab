#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Problems.VehicleRouting.Variants;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinVehicleAssignmentMoveTabuCriterion", @"Checks if a certain vehicle assignment move is tabu.")]
  [StorableClass]
  public class PotvinVehicleAssignmentMoveTabuCriterion : SingleSuccessorOperator, IPotvinVehicleAssignmentMoveOperator, ITabuChecker, IPotvinOperator, IVRPMoveOperator {
    public override bool CanChangeName {
      get { return false; }
    }

    public ILookupParameter<PotvinVehicleAssignmentMove> VehicleAssignmentMoveParameter {
      get { return (ILookupParameter<PotvinVehicleAssignmentMove>)Parameters["PotvinVehicleAssignmentMove"]; }
    }
    public ILookupParameter VRPMoveParameter {
      get { return VehicleAssignmentMoveParameter; }
    }
    public ILookupParameter<IVRPEncoding> VRPToursParameter {
      get { return (ILookupParameter<IVRPEncoding>)Parameters["VRPTours"]; }
    }
    public ILookupParameter<IVRPProblemInstance> ProblemInstanceParameter {
      get { return (LookupParameter<IVRPProblemInstance>)Parameters["ProblemInstance"]; }
    }

    public ILookupParameter<ItemList<IItem>> TabuListParameter {
      get { return (ILookupParameter<ItemList<IItem>>)Parameters["TabuList"]; }
    }
    public ILookupParameter<BoolValue> MoveTabuParameter {
      get { return (ILookupParameter<BoolValue>)Parameters["MoveTabu"]; }
    }
    public IValueLookupParameter<BoolValue> MaximizationParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    public ILookupParameter<DoubleValue> MoveQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["MoveQuality"]; }
    }
    public ValueParameter<BoolValue> UseAspirationCriterionParameter {
      get { return (ValueParameter<BoolValue>)Parameters["UseAspirationCriterion"]; }
    }

    public ILookupParameter<DoubleValue> MoveDistanceParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["MoveDistance"]; }
    }
    public ILookupParameter<DoubleValue> MoveOverloadParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["MoveOverload"]; }
    }
    public ILookupParameter<DoubleValue> MoveTardinessParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["MoveTardiness"]; }
    }

    public BoolValue UseAspirationCriterion {
      get { return UseAspirationCriterionParameter.Value; }
      set { UseAspirationCriterionParameter.Value = value; }
    }

    [StorableConstructor]
    protected PotvinVehicleAssignmentMoveTabuCriterion(bool deserializing) : base(deserializing) { }
    protected PotvinVehicleAssignmentMoveTabuCriterion(PotvinVehicleAssignmentMoveTabuCriterion original, Cloner cloner) : base(original, cloner) { }
    public PotvinVehicleAssignmentMoveTabuCriterion()
      : base() {
      Parameters.Add(new LookupParameter<PotvinVehicleAssignmentMove>("PotvinVehicleAssignmentMove", "The moves that should be made."));
      Parameters.Add(new LookupParameter<IVRPEncoding>("VRPTours", "The VRP tours considered in the move."));
      Parameters.Add(new LookupParameter<IVRPProblemInstance>("ProblemInstance", "The VRP problem instance"));

      Parameters.Add(new LookupParameter<BoolValue>("MoveTabu", "The variable to store if a move was tabu."));
      Parameters.Add(new LookupParameter<ItemList<IItem>>("TabuList", "The tabu list."));
      Parameters.Add(new ValueParameter<BoolValue>("UseAspirationCriterion", "Whether to use the aspiration criterion or not.", new BoolValue(true)));
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem, else if it is a minimization problem."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveQuality", "The quality of the current move."));

      Parameters.Add(new LookupParameter<DoubleValue>("MoveDistance", "The distance of the move"));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveOverload", "The overload of the move"));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveTardiness", "The tardiness of the move"));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinVehicleAssignmentMoveTabuCriterion(this, cloner);
    }

    public override IOperation Apply() {
      ItemList<IItem> tabuList = TabuListParameter.ActualValue;
      double moveQuality = MoveQualityParameter.ActualValue.Value;
      bool useAspiration = UseAspirationCriterion.Value;
      bool isTabu = false;
      PotvinVehicleAssignmentMove move = VehicleAssignmentMoveParameter.ActualValue;

      foreach (IItem tabuMove in tabuList) {
        PotvinVehicleAssignmentMoveAttribute attribute = tabuMove as PotvinVehicleAssignmentMoveAttribute;

        if (attribute != null) {
          double distance = 0;
          if (MoveDistanceParameter.ActualValue != null)
            distance = MoveDistanceParameter.ActualValue.Value;

          double overload = 0;
          if (MoveOverloadParameter.ActualValue != null)
            overload = MoveOverloadParameter.ActualValue.Value;

          double tardiness = 0;
          if (MoveTardinessParameter.ActualValue != null)
            tardiness = MoveTardinessParameter.ActualValue.Value;

          IVRPProblemInstance instance = ProblemInstanceParameter.ActualValue;
          double quality = attribute.Distance * instance.DistanceFactor.Value;

          IHomogenousCapacitatedProblemInstance cvrp = instance as IHomogenousCapacitatedProblemInstance;
          if (cvrp != null)
            quality += attribute.Overload * cvrp.OverloadPenalty.Value;

          ITimeWindowedProblemInstance vrptw = instance as ITimeWindowedProblemInstance;
          if (vrptw != null)
            quality += attribute.Tardiness * vrptw.TardinessPenalty.Value;

          if (!useAspiration || moveQuality >= quality) {
            if (attribute.Tour == move.Tour1 && attribute.Vehicle == move.Individual.GetVehicleAssignment(move.Tour2)) {
              isTabu = true;
              break;
            }

            if (attribute.Tour == move.Tour2 && attribute.Vehicle == move.Individual.GetVehicleAssignment(move.Tour1)) {
              isTabu = true;
              break;
            }

            if (attribute.Distance == distance && attribute.Overload == overload && attribute.Tardiness == tardiness) {
              isTabu = true;
              break;
            }
          }
        }
      }

      MoveTabuParameter.ActualValue = new BoolValue(isTabu);
      return base.Apply();
    }
  }
}
