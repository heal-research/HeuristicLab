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
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.Functions {
  public interface IFunctionTree : IItem {
    int Size { get; }
    int Height { get; }
    IList<IFunctionTree> SubTrees { get; }
    ICollection<IVariable> LocalVariables { get; }
    IFunction Function { get; }
    IVariable GetLocalVariable(string name);
    void AddVariable(IVariable variable);
    void RemoveVariable(string name);
    void AddSubTree(IFunctionTree tree);
    void InsertSubTree(int index, IFunctionTree tree);
    void RemoveSubTree(int index);

    double Evaluate(Dataset dataset, int sampleIndex);
  }
}
