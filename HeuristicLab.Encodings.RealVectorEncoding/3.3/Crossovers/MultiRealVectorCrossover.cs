#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Encodings.RealVectorEncoding {
  [Item("MultiRealVectorCrossover", "Randomly selects and applies one of its crossovers every time it is called.")]
  [StorableClass]
  public class MultiRealVectorCrossover : StochasticMultiBranch<IRealVectorCrossover>, IRealVectorCrossover, IStochasticOperator {
    public override bool CanChangeName {
      get { return false; }
    }
    protected override bool CreateChildOperation {
      get { return true; }
    }

    public ILookupParameter<ItemArray<RealVector>> ParentsParameter {
      get { return (ILookupParameter<ItemArray<RealVector>>)Parameters["Parents"]; }
    }
    public ILookupParameter<RealVector> ChildParameter {
      get { return (ILookupParameter<RealVector>)Parameters["Child"]; }
    }
    public IValueLookupParameter<DoubleMatrix> BoundsParameter {
      get { return (IValueLookupParameter<DoubleMatrix>)Parameters["Bounds"]; }
    }

    [StorableConstructor]
    protected MultiRealVectorCrossover(bool deserializing) : base(deserializing) { }
    protected MultiRealVectorCrossover(MultiRealVectorCrossover original, Cloner cloner)
      : base(original, cloner) {
    }
    public MultiRealVectorCrossover()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<RealVector>("Parents", "The parent real vector which should be crossed."));
      ParentsParameter.ActualName = "RealVector";
      Parameters.Add(new LookupParameter<RealVector>("Child", "The child real vector resulting from the crossover."));
      ChildParameter.ActualName = "RealVector";
      Parameters.Add(new ValueLookupParameter<DoubleMatrix>("Bounds", "The lower and upper bounds for each dimension of the vector."));

      foreach (Type type in ApplicationManager.Manager.GetTypes(typeof(IRealVectorCrossover))) {
        if (!typeof(MultiOperator<IRealVectorCrossover>).IsAssignableFrom(type))
          Operators.Add((IRealVectorCrossover)Activator.CreateInstance(type), true);
      }

      SelectedOperatorParameter.ActualName = "SelectedCrossoverOperator";
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiRealVectorCrossover(this, cloner);
    }

    protected override void Operators_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<IRealVectorCrossover>> e) {
      base.Operators_ItemsReplaced(sender, e);
      ParameterizeCrossovers();
    }

    protected override void Operators_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<IRealVectorCrossover>> e) {
      base.Operators_ItemsAdded(sender, e);
      ParameterizeCrossovers();
    }

    private void ParameterizeCrossovers() {
      foreach (IRealVectorCrossover crossover in Operators.OfType<IRealVectorCrossover>()) {
        crossover.ChildParameter.ActualName = ChildParameter.Name;
        crossover.ParentsParameter.ActualName = ParentsParameter.Name;
        crossover.BoundsParameter.ActualName = BoundsParameter.Name;
      }
      foreach (IStochasticOperator crossover in Operators.OfType<IStochasticOperator>()) {
        crossover.RandomParameter.ActualName = RandomParameter.Name;
      }
    }

    public override IOperation InstrumentedApply() {
      if (Operators.Count == 0) throw new InvalidOperationException(Name + ": Please add at least one real vector crossover to choose from.");
      return base.InstrumentedApply();
    }
  }
}
