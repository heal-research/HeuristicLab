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

using System;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.ALPS {
  [Item("LastLayerCloner", "An operator that creates a new layer by cloning the current last one.")]
  [StorableClass]
  public sealed class LastLayerCloner : SingleSuccessorOperator {
    public OperatorParameter NewLayerOperatorParameter {
      get { return (OperatorParameter)Parameters["NewLayerOperator"]; }
    }

    public IOperator NewLayerOperator {
      get { return NewLayerOperatorParameter.Value; }
      set { NewLayerOperatorParameter.Value = value; }
    }

    [StorableConstructor]
    private LastLayerCloner(bool deserializing) : base(deserializing) { }

    private LastLayerCloner(LastLayerCloner original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new LastLayerCloner(this, cloner);
    }

    public LastLayerCloner()
      : base() {
      Parameters.Add(new OperatorParameter("NewLayerOperator", "An operator that is performed on the new layer."));
    }

    public override IOperation Apply() {
      var scopes = ExecutionContext.Scope.SubScopes;
      if (scopes.Count < 1)
        throw new ArgumentException("At least one layer must exist.");

      var newScope = (IScope)scopes.Last().Clone();

      int scopeNumber;
      if (int.TryParse(newScope.Name, out scopeNumber))
        newScope.Name = (scopeNumber + 1).ToString();

      scopes.Add(newScope);

      var next = new OperationCollection(base.Apply());
      if (NewLayerOperator != null)
        next.Insert(0, ExecutionContext.CreateOperation(NewLayerOperator, newScope));
      return next;
    }
  }
}