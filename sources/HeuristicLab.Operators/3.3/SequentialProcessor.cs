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
using System.Xml;
using HeuristicLab.Collections;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators {
  /// <summary>
  /// Operator which executes multiple operators sequentially.
  /// </summary>
  [Item("SequentialProcessor", "An operator which executes multiple operators sequentially.")]
  [Creatable("Test")]
  [EmptyStorableClass]
  public sealed class SequentialProcessor : Operator, IOperator {
    public new ParameterCollection Parameters {
      get {
        return base.Parameters;
      }
    }
    IObservableKeyedCollection<string, IParameter> IOperator.Parameters {
      get { return Parameters; }
    }

    public SequentialProcessor()
      : base() {
    }

    public override ExecutionContextCollection Apply(ExecutionContext context) {
      ExecutionContextCollection next = new ExecutionContextCollection();
      foreach (IParameter param in Parameters) {
        IOperatorParameter opParam = param as IOperatorParameter;
        if (opParam != null) {
          IOperator op = (IOperator)opParam.GetValue(context);
          if (op != null) next.Add(new ExecutionContext(context.Parent, op, context.Scope));
        }
      }
      return next;
    }
  }
}
