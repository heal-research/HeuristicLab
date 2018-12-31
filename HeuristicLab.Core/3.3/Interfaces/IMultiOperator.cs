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


using System.Collections.Generic;
using HEAL.Fossil;

namespace HeuristicLab.Core {
  [StorableType("31136bcb-7c35-404d-bae7-2f9c5f17b767")]
  public interface IMultiOperator : IOperator {
    IEnumerable<IOperator> Operators { get; }
    bool AddOperator(IOperator op);
    bool RemoveOperator(IOperator op);
  }

  [StorableType("b2f95075-48df-43c8-b4a9-e404e357fabd")]
  public interface IMultiOperator<T> : IMultiOperator where T : class, IOperator {
    new IItemList<T> Operators { get; }
  }
}
