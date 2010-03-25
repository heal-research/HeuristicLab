#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Core;

namespace HeuristicLab.GP.Interfaces {
  public interface IFunctionTree : IStorable {
    bool HasLocalParameters { get; }
    IList<IFunctionTree> SubTrees { get; }
    IFunction Function { get; }
    IOperation CreateShakingOperation(IScope scope);
    IOperation CreateInitOperation(IScope scope);
    int GetSize();
    int GetHeight();
    void AddSubTree(IFunctionTree tree);
    void InsertSubTree(int index, IFunctionTree tree);
    void RemoveSubTree(int index);
    string ToString();
  }
}
