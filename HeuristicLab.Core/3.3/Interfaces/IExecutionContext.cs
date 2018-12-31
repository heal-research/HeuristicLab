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
using HEAL.Fossil;

namespace HeuristicLab.Core {
  [StorableType("90ba79ce-8166-4cc2-8871-0653b4dd59cc")]
  /// <summary>
  /// Interface which represents an execution context.
  /// </summary>
  public interface IExecutionContext : IDeepCloneable {
    IExecutionContext Parent { get; }
    IKeyedItemCollection<string, IParameter> Parameters { get; }
    IScope Scope { get; }

    IAtomicOperation CreateOperation(IOperator op);
    IAtomicOperation CreateOperation(IOperator op, IScope scope);
    IAtomicOperation CreateChildOperation(IOperator op);
    IAtomicOperation CreateChildOperation(IOperator op, IScope scope);
  }
}
