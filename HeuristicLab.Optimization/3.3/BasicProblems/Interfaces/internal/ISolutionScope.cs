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

using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Optimization {
  [StorableType("E019F914-0D0C-4668-970F-0EAE4A270038")]
  interface ISolutionScope : IScope, ISolutionContext { }

  [StorableType("E8A9C9B5-19E4-424B-8125-9732C76AA17C")]
  interface ISingleObjectiveSolutionScope<TEncodedSolution> : ISolutionScope, ISingleObjectiveSolutionContext<TEncodedSolution>
      where TEncodedSolution : class, IEncodedSolution { }

  [StorableType("14F7D2BD-83CE-40FC-B389-68D23A8E544B")]
  interface IMultiObjectiveSolutionScope<TEncodedSolution> : ISolutionScope, IMultiObjectiveSolutionContext<TEncodedSolution>
      where TEncodedSolution : class, IEncodedSolution { }
}
