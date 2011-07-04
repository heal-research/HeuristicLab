#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.VehicleRouting.Encodings.General {
  [Item("MultiVRPMoveMaker", "Peforms multiple moves on a given VRP encoding and updates the quality.")]
  [StorableClass]
  public class LambdaInterchangeMoveMaker : VRPMoveOperator, IMoveMaker, IMultiVRPMoveOperator {
    public override bool CanChangeName {
      get { return false; }
    }
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ILookupParameter<DoubleValue> MoveQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["MoveQuality"]; }
    }
    public ILookupParameter VRPMoveParameter {
      get { return (ILookupParameter)Parameters["VRPMove"]; }
    }
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }

    public ILookupParameter<DoubleValue> MoveVehcilesUtilizedParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["MoveVehiclesUtilized"]; }
    }
    public ILookupParameter<DoubleValue> MoveTravelTimeParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["MoveTravelTime"]; }
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

    public ILookupParameter<DoubleValue> VehcilesUtilizedParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["VehiclesUtilized"]; }
    }
    public ILookupParameter<DoubleValue> TravelTimeParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["TravelTime"]; }
    }
    public ILookupParameter<DoubleValue> DistanceParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Distance"]; }
    }
    public ILookupParameter<DoubleValue> OverloadParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Overload"]; }
    }
    public ILookupParameter<DoubleValue> TardinessParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Tardiness"]; }
    }

    [StorableConstructor]
    protected LambdaInterchangeMoveMaker(bool deserializing) : base(deserializing) { }
    protected LambdaInterchangeMoveMaker(LambdaInterchangeMoveMaker original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new LambdaInterchangeMoveMaker(this, cloner);
    }

    public LambdaInterchangeMoveMaker()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality of the solution."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveQuality", "The relative quality of the move."));
      Parameters.Add(new LookupParameter<IVRPMove>("VRPMove", "The generated moves."));
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator which should be used for stochastic manipulation operators."));

      Parameters.Add(new LookupParameter<DoubleValue>("MoveVehiclesUtilized", "The number of vehicles utilized."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveTravelTime", "The total travel time."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveDistance", "The distance."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveOverload", "The overload."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveTardiness", "The tardiness."));

      Parameters.Add(new LookupParameter<DoubleValue>("VehiclesUtilized", "The number of vehicles utilized."));
      Parameters.Add(new LookupParameter<DoubleValue>("TravelTime", "The total travel time."));
      Parameters.Add(new LookupParameter<DoubleValue>("Distance", "The distance."));
      Parameters.Add(new LookupParameter<DoubleValue>("Overload", "The overload."));
      Parameters.Add(new LookupParameter<DoubleValue>("Tardiness", "The tardiness."));

    }

    public override IOperation Apply() {
      IOperation next = base.Apply();

      IVRPEncoding solution = VRPToursParameter.ActualValue as IVRPEncoding;

      IVRPMove move = VRPMoveParameter.ActualValue as IVRPMove;
      DoubleValue moveQuality = MoveQualityParameter.ActualValue;
      DoubleValue quality = QualityParameter.ActualValue;

      //perform move
      VRPToursParameter.ActualValue = move.MakeMove();

      quality.Value = moveQuality.Value;
      VehcilesUtilizedParameter.ActualValue = MoveVehcilesUtilizedParameter.ActualValue;
      TravelTimeParameter.ActualValue = MoveTravelTimeParameter.ActualValue;
      DistanceParameter.ActualValue = MoveDistanceParameter.ActualValue;
      OverloadParameter.ActualValue = MoveOverloadParameter.ActualValue;
      TardinessParameter.ActualValue = MoveTardinessParameter.ActualValue;

      return next;
    }
  }
}
