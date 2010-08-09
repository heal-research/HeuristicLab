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
using HeuristicLab.Problems.VehicleRouting.Encodings;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using System;

namespace HeuristicLab.Problems.VehicleRouting {
  [Item("VRPOperator", "A VRP operator.")]
  [StorableClass]
  public abstract class VRPOperator : SingleSuccessorOperator, IVRPOperator {
    public int Cities {
      get { return CoordinatesParameter.ActualValue.Rows - 1; }
    }
    public ILookupParameter<DoubleMatrix> CoordinatesParameter {
      get {
        if (Parameters.ContainsKey("Coordinates"))
          return (ILookupParameter<DoubleMatrix>)Parameters["Coordinates"];
        else
          return null;
      }
    }
    public ILookupParameter<DoubleMatrix> DistanceMatrixParameter {
      get {
        if (Parameters.ContainsKey("DistanceMatrix"))
          return (ILookupParameter<DoubleMatrix>)Parameters["DistanceMatrix"];
        else
          return null;
      }
    }
    public ILookupParameter<BoolValue> UseDistanceMatrixParameter {
      get {
        if (Parameters.ContainsKey("UseDistanceMatrix"))
          return (ILookupParameter<BoolValue>)Parameters["UseDistanceMatrix"];
        else
          return null;
      }
    }
    public ILookupParameter<IntValue> VehiclesParameter {
      get {
        if (Parameters.ContainsKey("Vehicles"))
          return (ILookupParameter<IntValue>)Parameters["Vehicles"];
        else
          return null;
      }
    }
    public ILookupParameter<DoubleValue> CapacityParameter {
      get {
        if (Parameters.ContainsKey("Capacity"))
          return (ILookupParameter<DoubleValue>)Parameters["Capacity"];
        else
          return null;
      }
    }
    public ILookupParameter<DoubleArray> DemandParameter {
      get {
        if (Parameters.ContainsKey("Demand"))
          return (ILookupParameter<DoubleArray>)Parameters["Demand"];
        else
          return null;
      }
    }
    public ILookupParameter<DoubleArray> ReadyTimeParameter {
      get {
        if (Parameters.ContainsKey("ReadyTime"))
          return (ILookupParameter<DoubleArray>)Parameters["ReadyTime"];
        else
          return null;
      }
    }
    public ILookupParameter<DoubleArray> DueTimeParameter {
      get {
        if (Parameters.ContainsKey("DueTime"))
          return (ILookupParameter<DoubleArray>)Parameters["DueTime"];
        else
          return null;
      }
    }
    public ILookupParameter<DoubleArray> ServiceTimeParameter {
      get {
        if (Parameters.ContainsKey("ServiceTime"))
          return (ILookupParameter<DoubleArray>)Parameters["ServiceTime"];
        else
          return null;
      }
    }

    [StorableConstructor]
    protected VRPOperator(bool deserializing) : base(deserializing) { }

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

    protected bool Feasible(Tour tour) {
      return tour.Feasible(
                  DueTimeParameter.ActualValue,
                  ServiceTimeParameter.ActualValue,
                  ReadyTimeParameter.ActualValue,
                  DemandParameter.ActualValue,
                  CapacityParameter.ActualValue,
                  CoordinatesParameter.ActualValue,
                  DistanceMatrixParameter,
                  UseDistanceMatrixParameter.ActualValue);
    }

    protected double GetLength(Tour tour) {
      return tour.GetLength(
                CoordinatesParameter.ActualValue,
                DistanceMatrixParameter,
                UseDistanceMatrixParameter.ActualValue);
    }
  }
}
