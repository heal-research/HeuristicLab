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

using System.Drawing;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HEAL.Attic;

namespace HeuristicLab.Problems.Scheduling {
  [Item("JSMForcingStrategy", "Represents a forcing strategy.")]
  [StorableType("B5170DC3-AEDD-46D2-9173-E37E0EEFD456")]
  public sealed class JSMForcingStrategy : ValueTypeValue<JSMForcingStrategyTypes> {

    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Enum; }
    }

    [StorableConstructor]
    private JSMForcingStrategy(StorableConstructorFlag _) : base(_) { }
    private JSMForcingStrategy(JSMForcingStrategy original, Cloner cloner) : base(original, cloner) { }
    public JSMForcingStrategy() { }
    public JSMForcingStrategy(JSMForcingStrategyTypes types) : base(types) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new JSMForcingStrategy(this, cloner);
    }
  }
}
