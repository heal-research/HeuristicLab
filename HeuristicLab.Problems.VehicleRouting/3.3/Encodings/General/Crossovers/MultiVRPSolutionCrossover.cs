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
  [Item("MultiVRPSolutionCrossover", "Randomly selects and applies one of its crossovers every time it is called.")]
  [StorableClass]
  public class MultiVRPSolutionCrossover : StochasticMultiBranch<IVRPCrossover>, IVRPCrossover, IStochasticOperator {
    public override bool CanChangeName {
      get { return false; }
    }
    protected override bool CreateChildOperation {
      get { return true; }
    }

    public ILookupParameter<ItemArray<IVRPEncoding>> ParentsParameter {
      get { return (ScopeTreeLookupParameter<IVRPEncoding>)Parameters["Parents"]; }
    }

    public ILookupParameter<IVRPEncoding> ChildParameter {
      get { return (ILookupParameter<IVRPEncoding>)Parameters["Child"]; }
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
    protected MultiVRPSolutionCrossover(bool deserializing) : base(deserializing) { }
    protected MultiVRPSolutionCrossover(MultiVRPSolutionCrossover original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiVRPSolutionCrossover(this, cloner);
    }
    public MultiVRPSolutionCrossover()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<IVRPEncoding>("Parents", "The parent permutations which should be crossed."));
      ParentsParameter.ActualName = "VRPTours";
      Parameters.Add(new LookupParameter<IVRPEncoding>("Child", "The child permutation resulting from the crossover."));
      ChildParameter.ActualName = "VRPTours";

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

      foreach (Type type in ApplicationManager.Manager.GetTypes(typeof(IVRPCrossover)).OrderBy(op => op.Name)) {
        if (!typeof(MultiOperator<IVRPCrossover>).IsAssignableFrom(type))
          Operators.Add((IVRPCrossover)Activator.CreateInstance(type), true);
      }
    }

    protected override void Operators_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<IVRPCrossover>> e) {
      base.Operators_ItemsReplaced(sender, e);
      ParameterizeCrossovers();
    }

    protected override void Operators_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<IVRPCrossover>> e) {
      base.Operators_ItemsAdded(sender, e);
      ParameterizeCrossovers();
    }

    private void ParameterizeCrossovers() {
      foreach (IVRPCrossover crossover in Operators.OfType<IVRPCrossover>()) {
        crossover.ChildParameter.ActualName = ChildParameter.Name;
        crossover.ParentsParameter.ActualName = ParentsParameter.Name;

        if (crossover.CoordinatesParameter != null) crossover.CoordinatesParameter.ActualName = CoordinatesParameter.Name;
        if (crossover.DistanceMatrixParameter != null) crossover.DistanceMatrixParameter.ActualName = DistanceMatrixParameter.Name;
        if (crossover.UseDistanceMatrixParameter != null) crossover.UseDistanceMatrixParameter.ActualName = UseDistanceMatrixParameter.Name;
        if (crossover.VehiclesParameter != null) crossover.VehiclesParameter.ActualName = VehiclesParameter.Name;
        if (crossover.CapacityParameter != null) crossover.CapacityParameter.ActualName = CapacityParameter.Name;
        if (crossover.DemandParameter != null) crossover.DemandParameter.ActualName = DemandParameter.Name;
        if (crossover.ReadyTimeParameter != null) crossover.ReadyTimeParameter.ActualName = ReadyTimeParameter.Name;
        if (crossover.DueTimeParameter != null) crossover.DueTimeParameter.ActualName = DueTimeParameter.Name;
        if (crossover.ServiceTimeParameter != null) crossover.ServiceTimeParameter.ActualName = ServiceTimeParameter.Name;
      }
      foreach (IStochasticOperator crossover in Operators.OfType<IStochasticOperator>()) {
        crossover.RandomParameter.ActualName = RandomParameter.Name;
      }
    }

    public override IOperation Apply() {
      if (Operators.Count == 0) throw new InvalidOperationException(Name + ": Please add at least one permutation crossover to choose from.");
      return base.Apply();
    }
  }
}
