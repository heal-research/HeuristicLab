#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Selection {
  [Item("Replacer", "Generic replacer that replaces sub-scopes of the remaining scope with those from the selected scope.")]
  [StorableClass]
  public class Replacer : AlgorithmOperator, IReplacer {
    public IValueLookupParameter<ISelector> ReplacedSelectorParameter {
      get { return (IValueLookupParameter<ISelector>)Parameters["ReplacedSelector"]; }
    }
    public IValueLookupParameter<ISelector> SelectedSelectorParameter {
      get { return (IValueLookupParameter<ISelector>)Parameters["SelectedSelector"]; }
    }

    [StorableConstructor]
    protected Replacer(bool deserializing) : base(deserializing) { }
    protected Replacer(Replacer original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new Replacer(this, cloner);
    }
    public Replacer() {
      Parameters.Add(new ValueLookupParameter<ISelector>("ReplacedSelector", "The selection operator to select those scopes that are to be replaced. If no selection operator is defined, random selection will be applied."));
      Parameters.Add(new ValueLookupParameter<ISelector>("SelectedSelector", "The selection operator to select those scopes that are replacing the others. If no selection operator is defined, random selection will be applied."));

      SubScopesProcessor ssp = new SubScopesProcessor();

      Placeholder replacedSelector = new Placeholder();
      replacedSelector.OperatorParameter.ActualName = ReplacedSelectorParameter.Name;

      LeftReducer leftReducer = new LeftReducer();

      Placeholder selectedSelector = new Placeholder();
      selectedSelector.OperatorParameter.ActualName = SelectedSelectorParameter.Name;

      RightReducer rightReducer = new RightReducer();

      MergingReducer merger = new MergingReducer();

      OperatorGraph.InitialOperator = ssp;
      ssp.Operators.Add(replacedSelector);
      ssp.Operators.Add(selectedSelector);
      ssp.Successor = merger;
      replacedSelector.Successor = leftReducer;
      leftReducer.Successor = null;
      selectedSelector.Successor = rightReducer;
      rightReducer.Successor = null;
      merger.Successor = null;
    }

    public override IOperation Apply() {
      if (ExecutionContext.Scope.SubScopes.Count != 2) throw new InvalidOperationException(Name + ": There must be two sub-scopes which should be replaced/merged.");
      int remaining = ExecutionContext.Scope.SubScopes[0].SubScopes.Count;
      int selected = ExecutionContext.Scope.SubScopes[1].SubScopes.Count;

      ISelector replacedSelector = ReplacedSelectorParameter.ActualValue;
      ISelector selectedSelector = SelectedSelectorParameter.ActualValue;

      if (replacedSelector == null) {
        ReplacedSelectorParameter.Value = new RandomSelector();
        replacedSelector = ReplacedSelectorParameter.Value;
      }
      replacedSelector.CopySelected = new BoolValue(false);
      replacedSelector.NumberOfSelectedSubScopesParameter.Value = new IntValue(Math.Min(remaining, selected));

      if (selectedSelector == null) {
        SelectedSelectorParameter.Value = new RandomSelector();
        selectedSelector = SelectedSelectorParameter.Value;
      }
      selectedSelector.CopySelected = new BoolValue(false);
      selectedSelector.NumberOfSelectedSubScopesParameter.Value = new IntValue(Math.Min(remaining, selected));

      return base.Apply();
    }
  }
}
