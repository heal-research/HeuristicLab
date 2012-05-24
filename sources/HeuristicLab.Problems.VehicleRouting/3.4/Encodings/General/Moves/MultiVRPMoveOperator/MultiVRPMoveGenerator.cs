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

using System;
using System.Linq;
using HeuristicLab.Collections;
using HeuristicLab.Core;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Data;
using System.Collections.Generic;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Problems.VehicleRouting.Variants;
using HeuristicLab.Common;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.General {
  [Item("MultiVRPMoveGenerator", "Randomly selects and applies its move generators.")]
  [StorableClass]
  public class MultiVRPMoveGenerator : CheckedMultiOperator<IMultiVRPMoveGenerator>, IMultiVRPMoveOperator,
    IStochasticOperator, IMoveGenerator, IGeneralVRPOperator, IMultiVRPOperator {
    public override bool CanChangeName {
      get { return false; }
    }

    public IValueLookupParameter<IntValue> SelectedOperatorsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["SelectedOperators"]; }
    }

    public ILookupParameter<IVRPEncoding> VRPToursParameter {
      get { return (ILookupParameter<IVRPEncoding>)Parameters["VRPTours"]; }
    }

    public ILookupParameter VRPMoveParameter {
      get { return (ILookupParameter)Parameters["VRPMove"]; }
    }

    public ILookupParameter<IVRPProblemInstance> ProblemInstanceParameter {
      get { return (LookupParameter<IVRPProblemInstance>)Parameters["ProblemInstance"]; }
    }

    public IVRPProblemInstance ProblemInstance {
      get { return ProblemInstanceParameter.ActualValue; }
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
    private MultiVRPMoveGenerator(bool deserializing) : base(deserializing) { }
    public MultiVRPMoveGenerator()
      : base() {
      Parameters.Add(new ValueLookupParameter<IntValue>("SelectedOperators", "The number of selected operators.", new IntValue(1)));
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator which should be used for stochastic manipulation operators."));
      Parameters.Add(new ValueLookupParameter<DoubleArray>("Probabilities", "The array of relative probabilities for each operator.", new DoubleArray()));
      Parameters.Add(new LookupParameter<IVRPProblemInstance>("ProblemInstance", "The VRP problem instance"));

      Parameters.Add(new LookupParameter<IVRPEncoding>("VRPTours", "The VRP tours."));
      Parameters.Add(new LookupParameter<IVRPMove>("VRPMove", "The generated moves."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiVRPMoveGenerator(this, cloner);
    }

    protected MultiVRPMoveGenerator(MultiVRPMoveGenerator original, Cloner cloner)
      : base(original, cloner) {
    }

    public void SetOperators(IEnumerable<IOperator> operators) {
      foreach (IOperator op in operators) {
        if (op is IMultiVRPMoveGenerator && !(op is MultiOperator<IMultiVRPMoveGenerator>)) {
          Operators.Add(op.Clone() as IMultiVRPMoveGenerator, true);
        }
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
        moveGenerator.ProblemInstanceParameter.ActualName = ProblemInstanceParameter.Name;
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

      for (int i = 0; i < SelectedOperatorsParameter.ActualValue.Value; i++) {
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
