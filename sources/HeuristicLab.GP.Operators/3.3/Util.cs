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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Core;
using HeuristicLab.GP.Interfaces;
using HeuristicLab.Selection;

namespace HeuristicLab.GP.Operators {
  internal static class Util {
    internal static CompositeOperation CreateInitializationOperation(ICollection<IFunctionTree> trees, IScope scope) {
      // create a backup of sub scopes to restore after initialization
      Scope backupScope = new Scope("backup");
      foreach (Scope subScope in scope.SubScopes) {
        backupScope.AddSubScope(subScope);
      }

      CompositeOperation initializationOperation = new CompositeOperation();
      Scope tempScope = new Scope("Temp. initialization scope");

      var parametricTrees = trees.Where(t => t.HasLocalParameters);
      foreach (IFunctionTree tree in parametricTrees) {
        initializationOperation.AddOperation(tree.CreateInitOperation(tempScope));
      }
      scope.AddSubScope(tempScope);
      scope.AddSubScope(backupScope);
      // add an operation to remove the temporary scopes        
      initializationOperation.AddOperation(new AtomicOperation(new RightReducer(), scope));
      return initializationOperation;
    }
  }
}
