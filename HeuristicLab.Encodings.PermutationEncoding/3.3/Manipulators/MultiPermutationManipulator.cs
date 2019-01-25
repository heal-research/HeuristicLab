#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Attic;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Encodings.PermutationEncoding {
  [Item("MultiPermutationManipulator", "Randomly selects and applies one of its manipulators every time it is called.")]
  [StorableType("21EA3717-C907-490B-82E7-34A0B1A94EB0")]
  public class MultiPermutationManipulator : StochasticMultiBranch<IPermutationManipulator>, IPermutationManipulator, IStochasticOperator {
    public override bool CanChangeName {
      get { return false; }
    }
    protected override bool CreateChildOperation {
      get { return true; }
    }

    public ILookupParameter<Permutation> PermutationParameter {
      get { return (ILookupParameter<Permutation>)Parameters["Permutation"]; }
    }

    [StorableConstructor]
    protected MultiPermutationManipulator(StorableConstructorFlag _) : base(_) { }
    protected MultiPermutationManipulator(MultiPermutationManipulator original, Cloner cloner) : base(original, cloner) { }
    public MultiPermutationManipulator()
      : base() {
      Parameters.Add(new LookupParameter<Permutation>("Permutation", "The permutation that is being manipulated."));

      foreach (Type type in ApplicationManager.Manager.GetTypes(typeof(IPermutationManipulator))) {
        if (!typeof(MultiOperator<IPermutationManipulator>).IsAssignableFrom(type))
          Operators.Add((IPermutationManipulator)Activator.CreateInstance(type), true);
      }

      SelectedOperatorParameter.ActualName = "SelectedManipulationOperator";
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiPermutationManipulator(this, cloner);
    }

    protected override void Operators_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<IPermutationManipulator>> e) {
      base.Operators_ItemsReplaced(sender, e);
      ParameterizeManipulators();
    }

    protected override void Operators_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<IPermutationManipulator>> e) {
      base.Operators_ItemsAdded(sender, e);
      ParameterizeManipulators();
    }

    private void ParameterizeManipulators() {
      foreach (IPermutationManipulator manipulator in Operators.OfType<IPermutationManipulator>()) {
        manipulator.PermutationParameter.ActualName = PermutationParameter.Name;
      }
      foreach (IStochasticOperator op in Operators.OfType<IStochasticOperator>()) {
        op.RandomParameter.ActualName = RandomParameter.Name;
      }
    }

    public override IOperation InstrumentedApply() {
      if (Operators.Count == 0) throw new InvalidOperationException(Name + ": Please add at least one permutation manipulator to choose from.");
      return base.InstrumentedApply();
    }
  }
}
