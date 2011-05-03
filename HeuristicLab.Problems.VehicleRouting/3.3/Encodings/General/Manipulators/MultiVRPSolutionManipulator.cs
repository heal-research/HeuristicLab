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
  [Item("MultiVRPSolutionManipulator", "Randomly selects and applies one of its manipulators every time it is called.")]
  [StorableClass]
  public class MultiVRPSolutionManipulator : StochasticMultiBranch<IVRPManipulator>, IVRPManipulator, IStochasticOperator {
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
    protected MultiVRPSolutionManipulator(bool deserializing) : base(deserializing) { }
    protected MultiVRPSolutionManipulator(MultiVRPSolutionManipulator original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiVRPSolutionManipulator(this, cloner);
    }
    public MultiVRPSolutionManipulator()
      : base() {
      Parameters.Add(new LookupParameter<IVRPEncoding>("VRPTours", "The VRP tours to be manipulated."));

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

      foreach (Type type in ApplicationManager.Manager.GetTypes(typeof(IVRPManipulator)).OrderBy(op => op.Name)) {
        if (!typeof(MultiOperator<IVRPManipulator>).IsAssignableFrom(type)) {
          IVRPManipulator op = (IVRPManipulator)Activator.CreateInstance(type);
          bool operatorChecked = true;
          if (op is HeuristicLab.Problems.VehicleRouting.Encodings.Potvin.PotvinLocalSearchManipulator ||
            op is HeuristicLab.Problems.VehicleRouting.Encodings.Prins.PrinsLSManipulator)
            operatorChecked = false;
          Operators.Add(op, operatorChecked);
        }
      }
    }

    protected override void Operators_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<IVRPManipulator>> e) {
      base.Operators_ItemsReplaced(sender, e);
      ParameterizeManipulators();
    }

    protected override void Operators_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<IVRPManipulator>> e) {
      base.Operators_ItemsAdded(sender, e);
      ParameterizeManipulators();
    }

    private void ParameterizeManipulators() {
      foreach (IVRPManipulator manipulator in Operators.OfType<IVRPManipulator>()) {
        manipulator.VRPToursParameter.ActualName = VRPToursParameter.Name;

        if (manipulator.CoordinatesParameter != null) manipulator.CoordinatesParameter.ActualName = CoordinatesParameter.Name;
        if (manipulator.DistanceMatrixParameter != null) manipulator.DistanceMatrixParameter.ActualName = DistanceMatrixParameter.Name;
        if (manipulator.UseDistanceMatrixParameter != null) manipulator.UseDistanceMatrixParameter.ActualName = UseDistanceMatrixParameter.Name;
        if (manipulator.VehiclesParameter != null) manipulator.VehiclesParameter.ActualName = VehiclesParameter.Name;
        if (manipulator.CapacityParameter != null) manipulator.CapacityParameter.ActualName = CapacityParameter.Name;
        if (manipulator.DemandParameter != null) manipulator.DemandParameter.ActualName = DemandParameter.Name;
        if (manipulator.ReadyTimeParameter != null) manipulator.ReadyTimeParameter.ActualName = ReadyTimeParameter.Name;
        if (manipulator.DueTimeParameter != null) manipulator.DueTimeParameter.ActualName = DueTimeParameter.Name;
        if (manipulator.ServiceTimeParameter != null) manipulator.ServiceTimeParameter.ActualName = ServiceTimeParameter.Name;
      }
      foreach (IStochasticOperator manipulator in Operators.OfType<IStochasticOperator>()) {
        manipulator.RandomParameter.ActualName = RandomParameter.Name;
      }
    }

    public override IOperation Apply() {
      if (Operators.Count == 0) throw new InvalidOperationException(Name + ": Please add at least one permutation manipulator to choose from.");
      return base.Apply();
    }
  }
}
