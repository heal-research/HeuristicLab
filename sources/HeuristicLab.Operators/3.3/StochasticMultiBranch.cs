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
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators {
  /// <summary>
  /// Selects one of its branches (if there are any) given a list of relative probabilities.
  /// </summary>
  [Item("StochasticMultiBranch", "Selects one of its branches (if there are any) given a list of relative probabilities.")]
  [StorableClass]
  public abstract class StochasticMultiBranch<T> : CheckedMultiOperator<T> where T : class, IOperator {
    /// <summary>
    /// Should return true if the StochasticMultiOperator should create a new child operation with the selected successor
    /// or if it should create a new operation. If you need to shield the parameters of the successor you should return true here.
    /// </summary>
    protected abstract bool CreateChildOperation { get; }

    public ValueLookupParameter<DoubleArray> ProbabilitiesParameter {
      get { return (ValueLookupParameter<DoubleArray>)Parameters["Probabilities"]; }
    }
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ValueParameter<BoolValue> TraceSelectedOperatorParameter {
      get { return (ValueParameter<BoolValue>)Parameters["TraceSelectedOperator"]; }
    }
    public LookupParameter<StringValue> SelectedOperatorParameter {
      get { return (LookupParameter<StringValue>)Parameters["SelectedOperator"]; }
    }

    public DoubleArray Probabilities {
      get { return ProbabilitiesParameter.Value; }
      set { ProbabilitiesParameter.Value = value; }
    }

    [StorableConstructor]
    protected StochasticMultiBranch(bool deserializing) : base(deserializing) { }
    protected StochasticMultiBranch(StochasticMultiBranch<T> original, Cloner cloner)
      : base(original, cloner) {
    }
    /// <summary>
    /// Initializes a new instance of <see cref="StochasticMultiOperator"/> with two parameters
    /// (<c>Probabilities</c> and <c>Random</c>).
    /// </summary>
    public StochasticMultiBranch()
      : base() {
      Parameters.Add(new ValueLookupParameter<DoubleArray>("Probabilities", "The array of relative probabilities for each operator.", new DoubleArray()));
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator to use."));
      Parameters.Add(new LookupParameter<StringValue>("SelectedOperator", "If the TraceSelectedOperator flag is set, the name of the operator is traced in this parameter."));
      Parameters.Add(new ValueParameter<BoolValue>("TraceSelectedOperator", "Indicates, if the selected operator should be traced.", new BoolValue(false)));
      SelectedOperatorParameter.Hidden = false;
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code, remove with 3.4
      if (!Parameters.ContainsKey("SelectedOperator")) {
        Parameters.Add(new LookupParameter<StringValue>("SelectedOperator", "If the TraceSelectedOperator flag is set, the name of the operator is traced in this parameter."));
        SelectedOperatorParameter.Hidden = false;
      }
      if (!Parameters.ContainsKey("TraceSelectedOperator")) {
        Parameters.Add(new ValueParameter<BoolValue>("TraceSelectedOperator", "Indicates, if the selected operator should be traced.", new BoolValue(false)));
      }
      #endregion
    }

    protected override void Operators_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<T>> e) {
      base.Operators_ItemsRemoved(sender, e);
      if (Probabilities != null && Probabilities.Length > Operators.Count) {
        List<double> probs = new List<double>(Probabilities.Cast<double>());
        var sorted = e.Items.OrderByDescending(x => x.Index);
        foreach (IndexedItem<T> item in sorted)
          if (probs.Count > item.Index) probs.RemoveAt(item.Index);
        Probabilities = new DoubleArray(probs.ToArray());
      }
    }

    protected override void Operators_ItemsAdded(object sender, HeuristicLab.Collections.CollectionItemsChangedEventArgs<IndexedItem<T>> e) {
      base.Operators_ItemsAdded(sender, e);
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

    /// <summary>
    /// Applies an operator of the branches to the current scope with a 
    /// specific probability.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the list of probabilites does not
    /// match the number of operators, the list of selected operators is empty, 
    /// or all selected operators have zero probabitlity.</exception>
    /// <returns>A new operation with the operator that was selected followed by the current operator's successor.</returns>
    public override IOperation Apply() {
      IRandom random = RandomParameter.ActualValue;
      DoubleArray probabilities = ProbabilitiesParameter.ActualValue;
      if (probabilities.Length != Operators.Count) {
        throw new InvalidOperationException(Name + ": The list of probabilities has to match the number of operators");
      }
      IOperator successor = null;
      int index = -1;
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
            index = indexedItem.Index;
            break;
          }
        }
      }
      OperationCollection next = new OperationCollection(base.Apply());
      if (successor != null) {
        if (TraceSelectedOperatorParameter.Value.Value)
          SelectedOperatorParameter.ActualValue = new StringValue(index + ": " + successor.Name);

        if (CreateChildOperation)
          next.Insert(0, ExecutionContext.CreateChildOperation(successor));
        else next.Insert(0, ExecutionContext.CreateOperation(successor));
      } else {
        if (TraceSelectedOperatorParameter.Value.Value)
          SelectedOperatorParameter.ActualValue = new StringValue("");
      }
      return next;
    }
  }

  /// <summary>
  /// Selects one of its branches (if there are any) given a list of relative probabilities.
  /// </summary>
  [Item("StochasticMultiBranch", "Selects one of its branches (if there are any) given a list of relative probabilities.")]
  [StorableClass]
  public class StochasticMultiBranch : StochasticMultiBranch<IOperator> {
    [StorableConstructor]
    protected StochasticMultiBranch(bool deserializing) : base(deserializing) { }
    protected StochasticMultiBranch(StochasticMultiBranch original, Cloner cloner)
      : base(original, cloner) {
    }
    public StochasticMultiBranch() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new StochasticMultiBranch(this, cloner);
    }

    protected override bool CreateChildOperation {
      get { return false; }
    }
  }
}
