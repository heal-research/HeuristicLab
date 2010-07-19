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
using System.Linq;
using System.Text;
using HeuristicLab.Operators;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;

namespace HeuristicLab.Algorithms.NSGA2 {
  [Item("DefaultCrossover", "This operator creates a parent by copying a single individual.")]
  [StorableClass]
  public class DefaultCrossover : SingleSuccessorOperator, ICrossover, IStochasticOperator {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }

    public DefaultCrossover()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator."));
    }

    public override IOperation Apply() {
      IScope scope = ExecutionContext.Scope;
      int index = RandomParameter.ActualValue.Next(scope.SubScopes.Count);
      IScope child = scope.SubScopes[index];

      foreach (IVariable var in child.Variables)
        scope.Variables.Add((IVariable)var.Clone());

      return base.Apply();
    }
  }
}
