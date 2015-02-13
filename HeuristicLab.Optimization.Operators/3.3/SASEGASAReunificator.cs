#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization.Operators {
  /// <summary>
  /// Joins all sub sub scopes of a specified scope, reduces the number of sub 
  /// scopes by 1 and uniformly partitions the sub sub scopes again, maintaining the order.
  /// </summary>
  [Item("SASEGASAReunificator", "This operator merges the villages (sub-scopes) and redistributes the individuals. It is implemented as described in Affenzeller, M. et al. 2009. Genetic Algorithms and Genetic Programming - Modern Concepts and Practical Applications, CRC Press.")]
  [StorableClass]
  public class SASEGASAReunificator : SingleSuccessorOperator {

    public LookupParameter<IntValue> VillageCountParameter {
      get { return (LookupParameter<IntValue>)Parameters["VillageCount"]; }
    }

    [StorableConstructor]
    protected SASEGASAReunificator(bool deserializing) : base(deserializing) { }
    protected SASEGASAReunificator(SASEGASAReunificator original, Cloner cloner) : base(original, cloner) { }
    public SASEGASAReunificator()
      : base() {
      Parameters.Add(new LookupParameter<IntValue>("VillageCount", "The number of villages left after the reunification."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SASEGASAReunificator(this, cloner);
    }

    /// <summary>
    /// Joins all sub sub scopes of the given <paramref name="scope"/>, reduces the number of sub 
    /// scopes by 1 and uniformly partitions the sub sub scopes again, maintaining the order.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when there are less than 2 sub-scopes available or when VillageCount does not equal the number of sub-scopes.</exception>
    /// <param name="scope">The current scope whose sub scopes to reduce.</param>
    /// <returns><c>null</c>.</returns>
    public override IOperation Apply() {
      IScope scope = ExecutionContext.Scope;
      if (VillageCountParameter.ActualValue == null) VillageCountParameter.ActualValue = new IntValue(scope.SubScopes.Count);
      int villageCount = VillageCountParameter.ActualValue.Value;
      if (villageCount <= 1)
        throw new InvalidOperationException(Name + ": Reunification requires 2 or more sub-scopes");
      if (villageCount != scope.SubScopes.Count)
        throw new InvalidOperationException(Name + ": VillageCount does not equal the number of sub-scopes");

      // get all villages
      List<IScope> population = new List<IScope>();
      for (int i = 0; i < villageCount; i++) {
        population.AddRange(scope.SubScopes[i].SubScopes);
        scope.SubScopes[i].SubScopes.Clear();
      }

      // reduce number of villages by 1 and partition the population again
      scope.SubScopes.RemoveAt(scope.SubScopes.Count - 1);
      villageCount--;
      int populationPerVillage = population.Count / villageCount;
      for (int i = 0; i < villageCount; i++) {
        scope.SubScopes[i].SubScopes.AddRange(population.GetRange(0, populationPerVillage));
        population.RemoveRange(0, populationPerVillage);
      }

      // add remaining individuals to last village
      scope.SubScopes[scope.SubScopes.Count - 1].SubScopes.AddRange(population);
      population.Clear();

      VillageCountParameter.ActualValue.Value = villageCount;

      return base.Apply();
    }
  }
}
