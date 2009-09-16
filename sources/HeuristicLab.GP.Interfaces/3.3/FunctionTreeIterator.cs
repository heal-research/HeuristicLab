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
using System.Linq;
using System.Text;

namespace HeuristicLab.GP.Interfaces {
  public class FunctionTreeIterator {
    public static IEnumerable<IFunctionTree> IteratePrefix(IFunctionTree functionTree) {
      List<IFunctionTree> prefixForm = new List<IFunctionTree>();
      prefixForm.Add(functionTree);
      foreach (IFunctionTree subTree in functionTree.SubTrees) {
        prefixForm.AddRange(IteratePrefix(subTree));
      }
      return prefixForm;
    }

    public static IEnumerable<IFunctionTree> IteratePostfix(IFunctionTree functionTree) {
      List<IFunctionTree> postfixForm = new List<IFunctionTree>();
      foreach (IFunctionTree subTree in functionTree.SubTrees) {
        postfixForm.AddRange(IteratePostfix(subTree));
      }
      postfixForm.Add(functionTree);
      return postfixForm;
    }
  }
}
