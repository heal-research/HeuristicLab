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
using HEAL.Fossil;

namespace HeuristicLab.Optimization.Operators {
  [Item("UserDefinedSolutionCreator", "A solution creator that can be customized with operators which it will execute one after another.")]
  [StorableType("E6B305A2-A5E5-4DAC-933D-1DC1AA50ED7B")]
  public class UserDefinedSolutionCreator : UserDefinedOperator, ISolutionCreator {
    [StorableConstructor]
    protected UserDefinedSolutionCreator(StorableConstructorFlag _) : base(_) { }
    protected UserDefinedSolutionCreator(UserDefinedSolutionCreator original, Cloner cloner) : base(original, cloner) { }
    public UserDefinedSolutionCreator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new UserDefinedSolutionCreator(this, cloner);
    }
  }
}
