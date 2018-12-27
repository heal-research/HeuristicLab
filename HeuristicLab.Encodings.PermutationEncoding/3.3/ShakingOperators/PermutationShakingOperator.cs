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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Encodings.PermutationEncoding {
  /// <summary>
  /// A shaking operator for VNS.
  /// </summary>
  [Item("PermutationShakingOperator", "A shaking operator for VNS which uses available manipulation operators to perform the shaking.")]
  [StorableClass]
  public class PermutationShakingOperator : ShakingOperator<IPermutationManipulator>, IPermutationMultiNeighborhoodShakingOperator, IStochasticOperator {

    public ILookupParameter<Permutation> PermutationParameter {
      get { return (ILookupParameter<Permutation>)Parameters["Permutation"]; }
    }

    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }

    [StorableConstructor]
    protected PermutationShakingOperator(bool deserializing) : base(deserializing) { }
    protected PermutationShakingOperator(PermutationShakingOperator original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new PermutationShakingOperator(this, cloner);
    }
    public PermutationShakingOperator()
      : base() {
      Parameters.Add(new LookupParameter<Permutation>("Permutation", "The permutation to shake."));
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator that will be used for stochastic shaking operators."));
      foreach (IPermutationManipulator shaker in ApplicationManager.Manager.GetInstances<IPermutationManipulator>().OrderBy(x => x.Name))
        if (!(shaker is MultiPermutationManipulator)) Operators.Add(shaker);
    }

    #region Wiring of some parameters
    protected override void Operators_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<IPermutationManipulator>> e) {
      base.Operators_ItemsAdded(sender, e);
      ParameterizeOperators(e.Items);
    }

    protected override void Operators_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<IPermutationManipulator>> e) {
      base.Operators_ItemsReplaced(sender, e);
      ParameterizeOperators(e.Items);
    }

    private void ParameterizeOperators(IEnumerable<IndexedItem<IPermutationManipulator>> items) {
      if (items.Any()) {
        foreach (IStochasticOperator op in items.Select(x => x.Value).OfType<IStochasticOperator>())
          op.RandomParameter.ActualName = RandomParameter.Name;
        foreach (IPermutationManipulator op in items.Select(x => x.Value).OfType<IPermutationManipulator>())
          op.PermutationParameter.ActualName = PermutationParameter.Name;
      }
    }
    #endregion
  }
}
