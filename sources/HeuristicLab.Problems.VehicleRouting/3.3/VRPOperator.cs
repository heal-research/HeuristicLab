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
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Data;

namespace HeuristicLab.Problems.VehicleRouting {
  public abstract class VRPOperator : SingleSuccessorOperator, IVRPOperator {
    public int Cities {
      get { return CoordinatesParameter.ActualValue.Rows - 1; }
    }
    public ILookupParameter<DoubleMatrix> CoordinatesParameter {
      get { return (ILookupParameter<DoubleMatrix>)Parameters["Coordinates"]; }
    }
    public ILookupParameter<DoubleMatrix> DistanceMatrixParameter {
      get { return (ILookupParameter<DoubleMatrix>)Parameters["DistanceMatrix"]; }
    }
    public ILookupParameter<BoolValue> UseDistanceMatrixParameter {
      get { return (ILookupParameter<BoolValue>)Parameters["UseDistanceMatrix"]; }
    }
    public ILookupParameter<IntValue> VehiclesParameter {
      get { return (ILookupParameter<IntValue>)Parameters["Vehicles"]; }
    }
    public ILookupParameter<DoubleValue> CapacityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Capacity"]; }
    }
    public ILookupParameter<DoubleArray> DemandParameter {
      get { return (ILookupParameter<DoubleArray>)Parameters["Demand"]; }
    }
    public ILookupParameter<DoubleArray> ReadyTimeParameter {
      get { return (ILookupParameter<DoubleArray>)Parameters["ReadyTime"]; }
    }
    public ILookupParameter<DoubleArray> DueTimeParameter {
      get { return (ILookupParameter<DoubleArray>)Parameters["DueTime"]; }
    }
    public ILookupParameter<DoubleArray> ServiceTimeParameter {
      get { return (ILookupParameter<DoubleArray>)Parameters["ServiceTime"]; }
    }

    public VRPOperator()
      : base() {
      Parameters.Add(new ValueLookupParameter<IntValue>("Cities", "The city count."));
      Parameters.Add(new LookupParameter<DoubleMatrix>("Coordinates", "The coordinates of the cities."));
      Parameters.Add(new LookupParameter<DoubleMatrix>("DistanceMatrix", "The matrix which contains the distances between the cities."));
      Parameters.Add(new LookupParameter<BoolValue>("UseDistanceMatrix", "True if a distance matrix should be calculated and used for evaluation, otherwise false."));
      Parameters.Add(new LookupParameter<IntValue>("Vehicles", "The number of vehicles."));
      Parameters.Add(new LookupParameter<DoubleValue>("Capacity", "The capacity of each vehicle."));
      Parameters.Add(new LookupParameter<DoubleArray>("Demand", "The demand of each customer."));
      Parameters.Add(new LookupParameter<DoubleArray>("ReadyTime", "The ready time of each customer."));
      Parameters.Add(new LookupParameter<DoubleArray>("DueTime", "The due time of each customer."));
      Parameters.Add(new LookupParameter<DoubleArray>("ServiceTime", "The service time of each customer."));
    }
  }
}
