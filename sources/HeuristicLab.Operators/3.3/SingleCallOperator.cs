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
using System.Text;
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators {
  /// <summary>
  /// An operator which applies a specified operator on the current scope.
  /// </summary>
  [Item("SingleCallOperator", "An operator which applies a specified operator on the current scope.")]
  [Creatable("Test")]
  [EmptyStorableClass]
  public class SingleCallOperator : SingleSuccessorOperator {
    protected OperatorParameter OperatorParameter {
      get { return (OperatorParameter)Parameters["Operator"]; }
    }
    public IOperator Operator {
      get { return OperatorParameter.Value; }
      set { OperatorParameter.Value = value; }
    }

    public SingleCallOperator()
      : base() {
      Parameters.Add(new OperatorParameter("Operator", "The operator which should be applied on the current scope."));
    }

    public override IExecutionSequence Apply() {
      ExecutionContextCollection next = new ExecutionContextCollection(base.Apply());
      if (Operator != null)
        next.Insert(0, new ExecutionContext(ExecutionContext.Parent, Operator, ExecutionContext.Scope));
      return next;
    }
  }
}
