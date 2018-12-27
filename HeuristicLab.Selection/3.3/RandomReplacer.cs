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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Selection {
  [Item("RandomReplacer", "Replaces some randomly selected sub-scopes of the remaining scope with all those (or the best if there are more) from the selected scope.")]
  [StorableClass]
  public sealed class RandomReplacer : Replacer, IStochasticOperator, ISingleObjectiveReplacer {
    public override bool CanChangeName {
      get { return false; }
    }
    public override bool CanChangeDescription {
      get { return false; }
    }

    public ILookupParameter<ItemArray<DoubleValue>> QualityParameter {
      get { return (ILookupParameter<ItemArray<DoubleValue>>)Parameters["Quality"]; }
    }
    public IValueLookupParameter<BoolValue> MaximizationParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }

    [StorableConstructor]
    private RandomReplacer(bool deserializing) : base(deserializing) { }
    private RandomReplacer(RandomReplacer original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new RandomReplacer(this, cloner);
    }
    public RandomReplacer()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The quality of a solution."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem, otherwise false."));
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator to use."));

      RandomSelector randomSelector = new RandomSelector();
      randomSelector.RandomParameter.ActualName = RandomParameter.Name;
      ReplacedSelectorParameter.Value = randomSelector;
      ReplacedSelectorParameter.Hidden = true;
      BestSelector bestSelector = new BestSelector();
      bestSelector.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestSelector.QualityParameter.ActualName = QualityParameter.Name;
      SelectedSelectorParameter.Value = bestSelector;
      SelectedSelectorParameter.Hidden = true;
    }
  }
}
