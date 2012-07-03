#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System;
using System.Linq;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.General {
  [Item("MultiVRPSolutionCreator", "Randomly selects and applies one of its creator every time it is called.")]
  [StorableClass]
  public class MultiVRPSolutionCreator : StochasticMultiBranch<IVRPCreator>, IVRPCreator, IStochasticOperator {
    public override bool CanChangeName {
      get { return false; }
    }
    protected override bool CreateChildOperation {
      get { return true; }
    }

    public ILookupParameter<IVRPEncoding> VRPToursParameter {
      get { return (ILookupParameter<IVRPEncoding>)Parameters["VRPTours"]; }
    }

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

    [StorableConstructor]
    protected MultiVRPSolutionCreator(bool deserializing) : base(deserializing) { }
    protected MultiVRPSolutionCreator(MultiVRPSolutionCreator original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiVRPSolutionCreator(this, cloner);
    }

    public MultiVRPSolutionCreator()
      : base() {
      Parameters.Add(new LookupParameter<IVRPEncoding>("VRPTours", "The new VRP tours."));

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

      foreach (Type type in ApplicationManager.Manager.GetTypes(typeof(IVRPCreator)).OrderBy(op => op.Name)) {
        if (!typeof(MultiOperator<IVRPCreator>).IsAssignableFrom(type))
          Operators.Add((IVRPCreator)Activator.CreateInstance(type), true);
      }
    }

    protected override void Operators_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<IVRPCreator>> e) {
      base.Operators_ItemsReplaced(sender, e);
      ParameterizeManipulators();
    }

    protected override void Operators_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<IVRPCreator>> e) {
      base.Operators_ItemsAdded(sender, e);
      ParameterizeManipulators();
    }

    private void ParameterizeManipulators() {
      foreach (IVRPCreator creator in Operators.OfType<IVRPCreator>()) {
        creator.VRPToursParameter.ActualName = VRPToursParameter.Name;

        if (creator.CoordinatesParameter != null) creator.CoordinatesParameter.ActualName = CoordinatesParameter.Name;
        if (creator.DistanceMatrixParameter != null) creator.DistanceMatrixParameter.ActualName = DistanceMatrixParameter.Name;
        if (creator.UseDistanceMatrixParameter != null) creator.UseDistanceMatrixParameter.ActualName = UseDistanceMatrixParameter.Name;
        if (creator.VehiclesParameter != null) creator.VehiclesParameter.ActualName = VehiclesParameter.Name;
        if (creator.CapacityParameter != null) creator.CapacityParameter.ActualName = CapacityParameter.Name;
        if (creator.DemandParameter != null) creator.DemandParameter.ActualName = DemandParameter.Name;
        if (creator.ReadyTimeParameter != null) creator.ReadyTimeParameter.ActualName = ReadyTimeParameter.Name;
        if (creator.DueTimeParameter != null) creator.DueTimeParameter.ActualName = DueTimeParameter.Name;
        if (creator.ServiceTimeParameter != null) creator.ServiceTimeParameter.ActualName = ServiceTimeParameter.Name;
      }
      foreach (IStochasticOperator manipulator in Operators.OfType<IStochasticOperator>()) {
        manipulator.RandomParameter.ActualName = RandomParameter.Name;
      }
    }

    public override IOperation Apply() {
      if (Operators.Count == 0) throw new InvalidOperationException(Name + ": Please add at least one VRP creator to choose from.");
      return base.Apply();
    }
  }
}
