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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Collections;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Operators {
  /// <summary>
  /// Base class for stochastic multi operators.
  /// </summary>
  [Item("StochasticMultiOperator<T>", "Base class for stochastic multi operators.")]
  [StorableClass]
  public abstract class StochasticMultiOperator<T> : MultiOperator<T> where T : class, IOperator {
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

    public DoubleArray Probabilities {
      get { return ProbabilitiesParameter.Value; }
      set { ProbabilitiesParameter.Value = value; }
    }

    [StorableConstructor]
    protected StochasticMultiOperator(bool deserializing) : base(deserializing) { }
    /// <summary>
    /// Initializes a new instance of <see cref="StochasticMultiOperator"/> with two parameters
    /// (<c>Probabilities</c> and <c>Random</c>).
    /// </summary>
    public StochasticMultiOperator()
      : base() {
      Parameters.Add(new ValueLookupParameter<DoubleArray>("Probabilities", "The array of relative probabilities for each operator.", new DoubleArray()));
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator to use."));
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
        DoubleArray probs = new DoubleArray(Operators.Count);
        double avg = 0;
        if (Probabilities.Length > 0) {
          int zeros = 0;
          for (int i = 0; i < Probabilities.Length; i++) {
            if (Probabilities[i] == 0) zeros++;
            else avg += Probabilities[i];
          }
          if (Probabilities.Length - zeros > 0)
            avg /= (double)(Probabilities.Length - zeros);
          else avg = 1;
        } else avg = 1;

        var added = e.Items.OrderBy(x => x.Index).ToList();
        int insertCount = 0;
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
    /// match the number of operators.</exception>
    /// <returns>A new operation with the operator that was selected followed by the current operator's successor.</returns>
    public override IOperation Apply() {
      IRandom random = RandomParameter.ActualValue;
      DoubleArray probabilities = ProbabilitiesParameter.ActualValue;
      if(probabilities.Length != Operators.Count) {
        throw new InvalidOperationException(Name + ": The list of probabilities has to match the number of operators");
      }
      double sum = 0;
      for (int i = 0; i < Operators.Count; i++) {
        sum += probabilities[i];
      }
      double r = random.NextDouble() * sum;
      sum = 0;
      IOperator successor = null;
      for(int i = 0; i < Operators.Count; i++) {
        sum += probabilities[i];
        if(sum > r) {
          successor = Operators[i];
          break;
        }
      }
      OperationCollection next = new OperationCollection(base.Apply());
      if (successor != null) {
        if (CreateChildOperation)
          next.Insert(0, ExecutionContext.CreateChildOperation(successor));
        else next.Insert(0, ExecutionContext.CreateOperation(successor));
      }
      return next;
    }
  }
}
