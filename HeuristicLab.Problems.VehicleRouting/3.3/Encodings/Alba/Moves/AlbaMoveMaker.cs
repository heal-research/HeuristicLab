#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("AlbaTranslocationMoveMaker", "An operator which makes translocation moves for the Alba representation.")]
  [StorableClass]
  public abstract class AlbaMoveMaker : AlbaMoveOperator {
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
    protected AlbaMoveMaker(bool deserializing) : base(deserializing) { }
    protected AlbaMoveMaker(AlbaMoveMaker original, Cloner cloner) : base(original, cloner) { }
    public AlbaMoveMaker()
      : base() {
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
      IOperation successor = base.Apply();

      VehcilesUtilizedParameter.ActualValue = MoveVehcilesUtilizedParameter.ActualValue;
      TravelTimeParameter.ActualValue = MoveTravelTimeParameter.ActualValue;
      DistanceParameter.ActualValue = MoveDistanceParameter.ActualValue;
      OverloadParameter.ActualValue = MoveOverloadParameter.ActualValue;
      TardinessParameter.ActualValue = MoveTardinessParameter.ActualValue;

      return successor;
    }
  }
}
