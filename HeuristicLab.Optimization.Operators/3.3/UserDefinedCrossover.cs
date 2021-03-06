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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Optimization.Operators {
  [Item("UserDefinedCrossover", "A crossover that can be customized with operators which it will execute one after another.")]
  [StorableType("93EE357C-60D8-492B-AC69-F5D48370CAE1")]
  public class UserDefinedCrossover : UserDefinedOperator, ICrossover {
    [StorableConstructor]
    protected UserDefinedCrossover(StorableConstructorFlag _) : base(_) { }
    protected UserDefinedCrossover(UserDefinedCrossover original, Cloner cloner) : base(original, cloner) { }
    public UserDefinedCrossover() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new UserDefinedCrossover(this, cloner);
    }
  }
}
