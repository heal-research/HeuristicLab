#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2017 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Threading;
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableType("A6FB3D68-B298-4C89-9FD9-2D1415D131F5")]
  public interface ISplitter : IParameterizedNamedItem {
    void Initialize(IScope states);
    void Split(RegressionNodeTreeModel tree, IReadOnlyList<int> trainingRows, IScope stateScope, CancellationToken cancellationToken);
  }
}