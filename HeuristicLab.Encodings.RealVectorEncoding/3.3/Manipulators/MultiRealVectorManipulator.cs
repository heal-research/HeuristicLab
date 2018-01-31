#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Encodings.RealVectorEncoding {
  [Item("MultiRealVectorManipulator", "Randomly selects and applies one of its manipulators every time it is called.")]
  [StorableClass]
  public class MultiRealVectorManipulator : StochasticMultiBranch<IRealVectorManipulator>, IRealVectorManipulator, IStochasticOperator {
    public override bool CanChangeName {
      get { return false; }
    }
    protected override bool CreateChildOperation {
      get { return true; }
    }

    public ILookupParameter<RealVector> RealVectorParameter {
      get { return (ILookupParameter<RealVector>)Parameters["RealVector"]; }
    }
    public IValueLookupParameter<DoubleMatrix> BoundsParameter {
      get { return (IValueLookupParameter<DoubleMatrix>)Parameters["Bounds"]; }
    }

    [StorableConstructor]
    protected MultiRealVectorManipulator(bool deserializing) : base(deserializing) { }
    protected MultiRealVectorManipulator(MultiRealVectorManipulator original, Cloner cloner) : base(original, cloner) { }
    public MultiRealVectorManipulator()
      : base() {
      Parameters.Add(new LookupParameter<RealVector>("RealVector", "The real vector that is being manipulating."));
      Parameters.Add(new ValueLookupParameter<DoubleMatrix>("Bounds", "The lower and upper bounds for each dimension of the vector."));

      Operators.Add(new BreederGeneticAlgorithmManipulator());
      Operators.Add(new NormalAllPositionsManipulator());
      Operators.Add(new PolynomialAllPositionManipulator());
      Operators.Add(new PolynomialOnePositionManipulator());
      Operators.Add(new UniformOnePositionManipulator());

      SelectedOperatorParameter.ActualName = "SelectedManipulationOperator";
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiRealVectorManipulator(this, cloner);
    }

    protected override void Operators_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<IRealVectorManipulator>> e) {
      base.Operators_ItemsReplaced(sender, e);
      ParameterizeManipulators();
    }

    protected override void Operators_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<IRealVectorManipulator>> e) {
      base.Operators_ItemsAdded(sender, e);
      ParameterizeManipulators();
    }

    private void ParameterizeManipulators() {
      foreach (IRealVectorManipulator manipulator in Operators.OfType<IRealVectorManipulator>()) {
        manipulator.RealVectorParameter.ActualName = RealVectorParameter.Name;
        manipulator.BoundsParameter.ActualName = BoundsParameter.Name;
      }
      foreach (IStochasticOperator crossover in Operators.OfType<IStochasticOperator>()) {
        crossover.RandomParameter.ActualName = RandomParameter.Name;
      }
    }

    public override IOperation InstrumentedApply() {
      if (Operators.Count == 0) throw new InvalidOperationException(Name + ": Please add at least one real vector manipulator to choose from.");
      return base.InstrumentedApply();
    }
  }
}
