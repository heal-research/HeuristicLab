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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.VehicleRouting.Encodings.General;

namespace HeuristicLab.Problems.VehicleRouting {
  [Item("VRPShakingOperator", "A shaking operator for VNS that applies available mutation operators.")]
  [StorableClass]
  public class VehicleRoutingShakingOperator : ShakingOperator<IVRPManipulator>, IVRPMultiNeighborhoodShakingOperator, IStochasticOperator {
    #region Parameters
    public ILookupParameter<IVRPEncoding> VRPToursParameter {
      get { return (ILookupParameter<IVRPEncoding>)Parameters["VRPTours"]; }
    }

    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }

    public ILookupParameter<DoubleMatrix> CoordinatesParameter {
      get { return (ILookupParameter<DoubleMatrix>)Parameters["Coordinates"]; }
    }

    public int Cities {
      get {
        if (CoordinatesParameter.ActualValue != null)
          return CoordinatesParameter.ActualValue.Rows;
        else return 0;
      }
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
    #endregion

    [StorableConstructor]
    protected VehicleRoutingShakingOperator(bool deserializing) : base(deserializing) { }
    protected VehicleRoutingShakingOperator(VehicleRoutingShakingOperator original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new VehicleRoutingShakingOperator(this, cloner);
    }
    public VehicleRoutingShakingOperator()
      : base() {
      Parameters.Add(new LookupParameter<IVRPEncoding>("VRPTours", "The vrp tour encoding to shake."));
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator that will be used for stochastic shaking operators."));
      Parameters.Add(new LookupParameter<DoubleMatrix>("Coordinates", "The coordinates of the cities."));
      Parameters.Add(new LookupParameter<DoubleMatrix>("DistanceMatrix", "The matrix which contains the distances between the cities."));
      Parameters.Add(new LookupParameter<BoolValue>("UseDistanceMatrix", "True if a distance matrix should be calculated and used for evaluation, otherwise false."));
      Parameters.Add(new LookupParameter<IntValue>("Vehicles", "The number of vehicles."));
      Parameters.Add(new LookupParameter<DoubleValue>("Capacity", "The capacity of each vehicle."));
      Parameters.Add(new LookupParameter<DoubleArray>("Demand", "The demand of each customer."));
      Parameters.Add(new LookupParameter<DoubleArray>("ReadyTime", "The ready time of each customer."));
      Parameters.Add(new LookupParameter<DoubleArray>("DueTime", "The due time of each customer."));
      Parameters.Add(new LookupParameter<DoubleArray>("ServiceTime", "The service time of each customer."));

      foreach (IVRPManipulator shaker in ApplicationManager.Manager.GetInstances<IVRPManipulator>().OrderBy(x => x.Name))
        if (!(shaker is MultiVRPSolutionManipulator)) Operators.Add(shaker);
    }

    #region Wiring of some parameters
    protected override void Operators_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<IVRPManipulator>> e) {
      base.Operators_ItemsAdded(sender, e);
      ParameterizeOperators(e.Items);
    }

    protected override void Operators_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<IVRPManipulator>> e) {
      base.Operators_ItemsReplaced(sender, e);
      ParameterizeOperators(e.Items);
    }

    private void ParameterizeOperators(IEnumerable<IndexedItem<IVRPManipulator>> items) {
      if (items.Any()) {
        foreach (IStochasticOperator op in items.Select(x => x.Value).OfType<IStochasticOperator>())
          op.RandomParameter.ActualName = RandomParameter.Name;
        foreach (IVRPManipulator op in items.Select(x => x.Value).OfType<IVRPManipulator>()) {
          op.VRPToursParameter.ActualName = VRPToursParameter.Name;
          if (op.CoordinatesParameter != null) op.CoordinatesParameter.ActualName = CoordinatesParameter.Name;
          if (op.DistanceMatrixParameter != null) op.DistanceMatrixParameter.ActualName = DistanceMatrixParameter.Name;
          if (op.UseDistanceMatrixParameter != null) op.UseDistanceMatrixParameter.ActualName = UseDistanceMatrixParameter.Name;
          if (op.VehiclesParameter != null) op.VehiclesParameter.ActualName = VehiclesParameter.Name;
          if (op.CapacityParameter != null) op.CapacityParameter.ActualName = CapacityParameter.Name;
          if (op.DemandParameter != null) op.DemandParameter.ActualName = DemandParameter.Name;
          if (op.ReadyTimeParameter != null) op.ReadyTimeParameter.ActualName = ReadyTimeParameter.Name;
          if (op.DueTimeParameter != null) op.DueTimeParameter.ActualName = DueTimeParameter.Name;
          if (op.ServiceTimeParameter != null) op.ServiceTimeParameter.ActualName = ServiceTimeParameter.Name;
        }
      }
    }
    #endregion
  }
}
