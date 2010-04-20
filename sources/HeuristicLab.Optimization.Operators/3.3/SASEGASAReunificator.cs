#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Operators;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization.Operators {
  /// <summary>
  /// Joins all sub sub scopes of a specified scope, reduces the number of sub 
  /// scopes by 1 and uniformly partitions the sub sub scopes again, maintaining the order.
  /// </summary>
  /*[Item("SASEGASAReunificator", "This operator merges the villages in a migration phase and redistributes the individuals. It is implemented as described in Affenzeller, M. et al. 2009. Genetic Algorithms and Genetic Programming - Modern Concepts and Practical Applications, CRC Press.")]
  [StorableClass]
  public class SASEGASAReunificator : SingleSuccessorOperator, IMigrator {

    /// <summary>
    /// Joins all sub sub scopes of the given <paramref name="scope"/>, reduces the number of sub 
    /// scopes by 1 and uniformly partitions the sub sub scopes again, maintaining the order.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when only 0 or 1 sub scope is available.</exception>
    /// <param name="scope">The current scope whose sub scopes to reduce.</param>
    /// <returns><c>null</c>.</returns>
    public override IOperation Apply() {
      IScope scope = ExecutionContext.Scope;
      int villageCount = scope.SubScopes.Count;
      if (villageCount <= 1)
        throw new InvalidOperationException("SASEGASA reunification requires 2 or more sub-scopes");

      // get all villages
      IList<IScope> population = new List<IScope>();
      for (int i = 0; i < villageCount; i++) {
        while (scope.SubScopes[i].SubScopes[0].SubScopes.Count > 0) {
          population.Add(scope.SubScopes[i].SubScopes[0].SubScopes[0]);
          scope.SubScopes[i].SubScopes.Remove(scope.SubScopes[i].SubScopes[0].SubScopes[0]);
        }
        scope.SubScopes[i].SubScopes.Clear();
      }

      // reduce number of villages by 1 and partition the population again
      scope.SubScopes.Remove(scope.SubScopes[scope.SubScopes.Count - 1]);
      villageCount--;
      int populationPerVillage = population.Count / villageCount;
      for (int i = 0; i < villageCount; i++) {
        scope.SubScopes[i].SubScopes.Add(new Scope());
        scope.SubScopes[i].SubScopes.Add(new Scope());
        for (int j = 0; j < populationPerVillage; j++) {
          scope.SubScopes[i].SubScopes[1].SubScopes.Add(population[0]);
          population.RemoveAt(0);
        }
      }

      // add remaining sub-sub-scopes to last sub-scope
      while (population.Count > 0) {
        scope.SubScopes[scope.SubScopes.Count - 1].SubScopes[1].SubScopes.Add(population[0]);
        population.RemoveAt(0);
      }

      return base.Apply();
    }
  }*/
}
