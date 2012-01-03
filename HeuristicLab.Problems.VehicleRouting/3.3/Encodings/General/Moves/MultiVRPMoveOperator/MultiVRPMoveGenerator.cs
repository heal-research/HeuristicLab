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
using System.Collections.Generic;
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
  [Item("MultiVRPMoveGenerator", "Randomly selects and applies its move generators.")]
  [StorableClass]
  public class MultiVRPMoveGenerator : CheckedMultiOperator<IMultiVRPMoveGenerator>, IMultiVRPMoveOperator,
    IStochasticOperator, IMultiMoveGenerator {
    public override bool CanChangeName {
      get { return false; }
    }

    public IValueLookupParameter<IntValue> SampleSizeParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["SampleSize"]; }
    }

    public ILookupParameter<IVRPEncoding> VRPToursParameter {
      get { return (ILookupParameter<IVRPEncoding>)Parameters["VRPTours"]; }
    }

    public ILookupParameter VRPMoveParameter {
      get { return (ILookupParameter)Parameters["VRPMove"]; }
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

    public ValueLookupParameter<DoubleArray> ProbabilitiesParameter {
      get { return (ValueLookupParameter<DoubleArray>)Parameters["Probabilities"]; }
    }
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }

    public DoubleArray Probabilities {
      get { return ProbabilitiesParameter.Value; }
      set { ProbabilitiesParameter.Value = value; }
    }

    [StorableConstructor]
    protected MultiVRPMoveGenerator(bool deserializing) : base(deserializing) { }
    protected MultiVRPMoveGenerator(MultiVRPMoveGenerator original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiVRPMoveGenerator(this, cloner);
    }
    public MultiVRPMoveGenerator()
      : base() {
      Parameters.Add(new ValueLookupParameter<IntValue>("SampleSize", "The number of moves to generate."));
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator which should be used for stochastic manipulation operators."));
      Parameters.Add(new ValueLookupParameter<DoubleArray>("Probabilities", "The array of relative probabilities for each operator.", new DoubleArray()));

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

      Parameters.Add(new LookupParameter<IVRPEncoding>("VRPTours", "The VRP tours."));
      Parameters.Add(new LookupParameter<IVRPMove>("VRPMove", "The generated moves."));

      foreach (Type type in ApplicationManager.Manager.GetTypes(typeof(IMultiVRPMoveGenerator)).OrderBy(op => op.Name)) {
        if (!typeof(MultiOperator<IMultiVRPMoveGenerator>).IsAssignableFrom(type))
          Operators.Add((IMultiVRPMoveGenerator)Activator.CreateInstance(type), true);
      }
    }

    protected override void Operators_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<IMultiVRPMoveGenerator>> e) {
      base.Operators_ItemsRemoved(sender, e);
      if (Probabilities != null && Probabilities.Length > Operators.Count) {
        List<double> probs = new List<double>(Probabilities.Cast<double>());
        var sorted = e.Items.OrderByDescending(x => x.Index);
        foreach (IndexedItem<IMultiVRPMoveGenerator> item in sorted)
          if (probs.Count > item.Index) probs.RemoveAt(item.Index);
        Probabilities = new DoubleArray(probs.ToArray());
      }
    }

    protected override void Operators_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<IMultiVRPMoveGenerator>> e) {
      base.Operators_ItemsReplaced(sender, e);
      ParameterizeMoveGenerators();
    }

    protected override void Operators_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<IMultiVRPMoveGenerator>> e) {
      base.Operators_ItemsAdded(sender, e);
      ParameterizeMoveGenerators();

      if (Probabilities != null && Probabilities.Length < Operators.Count) {
        double avg = (Probabilities.Where(x => x > 0).Count() > 0) ? (Probabilities.Where(x => x > 0).Average()) : (1);
        // add the average of all probabilities in the respective places (the new operators)
        var added = e.Items.OrderBy(x => x.Index).ToList();
        int insertCount = 0;
        DoubleArray probs = new DoubleArray(Operators.Count);
        for (int i = 0; i < Operators.Count; i++) {
          if (insertCount < added.Count && i == added[insertCount].Index) {
            probs[i] = avg;
            insertCount++;
          } else if (i - insertCount < Probabilities.Length) {
            probs[i] = Probabilities[i - insertCount];
          } else probs[i] = avg;
        }
        Probabilities = probs;
      }
    }

    private void ParameterizeMoveGenerators() {
      foreach (IMultiVRPMoveOperator moveGenerator in Operators.OfType<IMultiVRPMoveOperator>()) {
        if (moveGenerator.CoordinatesParameter != null) moveGenerator.CoordinatesParameter.ActualName = CoordinatesParameter.Name;
        if (moveGenerator.DistanceMatrixParameter != null) moveGenerator.DistanceMatrixParameter.ActualName = DistanceMatrixParameter.Name;
        if (moveGenerator.UseDistanceMatrixParameter != null) moveGenerator.UseDistanceMatrixParameter.ActualName = UseDistanceMatrixParameter.Name;
        if (moveGenerator.VehiclesParameter != null) moveGenerator.VehiclesParameter.ActualName = VehiclesParameter.Name;
        if (moveGenerator.CapacityParameter != null) moveGenerator.CapacityParameter.ActualName = CapacityParameter.Name;
        if (moveGenerator.DemandParameter != null) moveGenerator.DemandParameter.ActualName = DemandParameter.Name;
        if (moveGenerator.ReadyTimeParameter != null) moveGenerator.ReadyTimeParameter.ActualName = ReadyTimeParameter.Name;
        if (moveGenerator.DueTimeParameter != null) moveGenerator.DueTimeParameter.ActualName = DueTimeParameter.Name;
        if (moveGenerator.ServiceTimeParameter != null) moveGenerator.ServiceTimeParameter.ActualName = ServiceTimeParameter.Name;

        moveGenerator.VRPToursParameter.ActualName = VRPToursParameter.Name;
        moveGenerator.VRPMoveParameter.ActualName = VRPMoveParameter.Name;
      }
      foreach (IStochasticOperator moveGenerator in Operators.OfType<IStochasticOperator>()) {
        moveGenerator.RandomParameter.ActualName = RandomParameter.Name;
      }
    }

    public override IOperation Apply() {
      if (Operators.Count == 0) throw new InvalidOperationException(Name + ": Please add at least one VRP move generator choose from.");
      OperationCollection next = new OperationCollection(base.Apply());

      for (int i = 0; i < SampleSizeParameter.ActualValue.Value; i++) {
        IRandom random = RandomParameter.ActualValue;
        DoubleArray probabilities = ProbabilitiesParameter.ActualValue;
        if (probabilities.Length != Operators.Count) {
          throw new InvalidOperationException(Name + ": The list of probabilities has to match the number of operators");
        }
        IOperator successor = null;
        var checkedOperators = Operators.CheckedItems;
        if (checkedOperators.Count() > 0) {
          // select a random operator from the checked operators
          double sum = (from indexedItem in checkedOperators select probabilities[indexedItem.Index]).Sum();
          if (sum == 0) throw new InvalidOperationException(Name + ": All selected operators have zero probability.");
          double r = random.NextDouble() * sum;
          sum = 0;
          foreach (var indexedItem in checkedOperators) {
            sum += probabilities[indexedItem.Index];
            if (sum > r) {
              successor = indexedItem.Value;
              break;
            }
          }
        }

        if (successor != null) {
          next.Insert(0, ExecutionContext.CreateChildOperation(successor));
        }
      }

      return next;
    }
  }
}
