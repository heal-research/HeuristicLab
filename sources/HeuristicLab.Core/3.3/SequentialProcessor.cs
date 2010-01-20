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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Common;
using HeuristicLab.Collections;

namespace HeuristicLab.Core {
  /// <summary>
  /// The base class for all operators.
  /// </summary>
  [Item("SequentialProcessor", "An operator which executes a sequence of operators.")]
  [Creatable("Test")]
  public class SequentialProcessor : OperatorBase, IOperator {
    public Parameter<OperatorList> Successors {
      get { return (Parameter<OperatorList>)Parameters["Successors"]; }
    }

    public SequentialProcessor()
      : base() {
      Parameters.Add(new Parameter<OperatorList>("Successors", "..."));
    }

    public override ExecutionContextCollection Apply(ExecutionContext context) {
      OperatorList successors = Successors.GetValue(context);
      ExecutionContextCollection next = new ExecutionContextCollection();
      if (successors != null) {
        foreach (IOperator op in successors)
          next.Add(new ExecutionContext(context.Parent, op, context.Scope));
        return next;
      }
      return next;
    }
  }
}
