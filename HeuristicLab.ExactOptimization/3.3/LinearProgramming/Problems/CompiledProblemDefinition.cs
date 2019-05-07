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

using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.ExactOptimization.LinearProgramming {

  public class CompiledProblemDefinition : Item {

    public CompiledProblemDefinition() {
    }

    protected CompiledProblemDefinition(CompiledProblemDefinition original, Cloner cloner) : base(original, cloner) {
    }

    public dynamic vars { get; set; }

    public override IDeepCloneable Clone(Cloner cloner) =>
      new CompiledProblemDefinition(this, cloner);

    public virtual void Initialize() {
    }
  }
}
