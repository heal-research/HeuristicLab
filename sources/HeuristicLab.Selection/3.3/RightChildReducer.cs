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
using HeuristicLab.Operators;

namespace HeuristicLab.Selection {
  /// <summary>
  /// Reduces the sub scopes by one level, so that the right sub scope contains also the right child scopes
  /// of the left sub scope and the left sub scope represents its left child scope.
  /// <pre>                                                      
  ///                   scope             scope  
  ///                   / | \             /   \      
  ///                  L ... R   =>      A     R                 
  ///                / | \    \              / /\ \       
  ///               A ... LR   C             C D E F      
  ///                     /|\                              
  ///                    D E F                             
  /// </pre>
  /// </summary>
  public class RightChildReducer : ReducerBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    /// <summary>
    /// Reduces the right child of the left sub scope and adds its sub scopes to the right sub scope.
    /// The left sub scope is also narrowed, which means it represents then its left child.
    /// </summary>
    /// <param name="scope">The current scope to reduce.</param>
    /// <returns>A list of the new reduced sub scopes.</returns>
    protected override ICollection<IScope> Reduce(IScope scope) {
      IScope rightChild = scope.SubScopes[scope.SubScopes.Count - 1];
      IScope leftChild = scope.SubScopes[0];
      IScope leftRightChild = leftChild.SubScopes[leftChild.SubScopes.Count - 1];

      // merge right children
      for (int i = 0; i < leftRightChild.SubScopes.Count; i++)
        rightChild.AddSubScope(leftRightChild.SubScopes[i]);

      leftChild = leftChild.SubScopes[0];

      List<IScope> subScopes = new List<IScope>();
      subScopes.Add(leftChild);
      subScopes.Add(rightChild);
      return subScopes;
    }
  }
}
