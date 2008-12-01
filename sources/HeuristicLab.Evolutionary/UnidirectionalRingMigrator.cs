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
using HeuristicLab.Data;

namespace HeuristicLab.Evolutionary {
  /// <summary>
  /// Operator class that migrates one sub scope of each child to its left neighbour sub scope, like a ring.
  /// </summary>
  public class UnidirectionalRingMigrator : OperatorBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    /// <summary>
    /// Migrates every first sub scope of each child to its left neighbour (like a ring).
    /// <pre>                                                               
    ///                    scope                    scope             
    ///                /     |     \            /     |     \         
    ///               A      B      C    =>    A      B      C           
    ///              /|\    /|\    /|\        /|\    /|\    /|\       
    ///             G H I  J K L  M N O      H I J  K L M  N O G     
    /// </pre>
    /// </summary>
    /// <param name="scope">The scope whose sub scopes of the children should migrate.</param>
    /// <returns><c>null</c>.</returns>
    public override IOperation Apply(IScope scope) {
      IList<IScope> emigrantsList = new List<IScope>();

      for (int i = 0; i < scope.SubScopes.Count; i++) {
        IScope emigrants = scope.SubScopes[i].SubScopes[1];
        scope.SubScopes[i].RemoveSubScope(emigrants);
        emigrantsList.Add(emigrants);
      }

      // shift first emigrants to end of list
      emigrantsList.Add(emigrantsList[0]);
      emigrantsList.RemoveAt(0);

      for (int i = 0; i < scope.SubScopes.Count; i++)
        scope.SubScopes[i].AddSubScope(emigrantsList[i]);

      return null;
    }
  }
}
