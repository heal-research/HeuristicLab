#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;

namespace HeuristicLab.Problems.Dynamic {
  [Item("Characteristic Calculator", "")]
  [StorableType("389790FF-2F4F-4526-9500-876926CF2E28")]
  public abstract class CharacteristicCalculator : ParameterizedNamedItem, ICharacteristicCalculator {
    [Storable] public IProblem Problem { get; set; }

    [Storable] protected CheckedItemList<StringValue> characteristics;
    public ReadOnlyCheckedItemList<StringValue> Characteristics => characteristics.AsReadOnly();

    [StorableConstructor]
    protected CharacteristicCalculator(StorableConstructorFlag _) : base(_) { }
    protected CharacteristicCalculator(CharacteristicCalculator original, Cloner cloner)
      : base(original, cloner) {
      characteristics = cloner.Clone(original.characteristics);
      Problem = cloner.Clone(original.Problem);
    }
    protected CharacteristicCalculator() {
      name = ItemName;
      description = ItemDescription;
      characteristics = new CheckedItemList<StringValue>();
    }

    public abstract bool CanCalculate();

    public abstract IEnumerable<IResult> Calculate();
  }
}
