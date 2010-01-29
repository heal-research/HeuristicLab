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
using HeuristicLab.Core;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators {
  /// <summary>
  /// A base class for operators which have only one successor.
  /// </summary>
  [Item("StandardOperator", "A base class for operators which have only one successor.")]
  [Creatable("Test")]
  [EmptyStorableClass]
  public abstract class StandardOperator : Operator {
    public OperatorParameter Successor {
      get { return (OperatorParameter)Parameters["Successor"]; }
    }

    public StandardOperator()
      : base() {
      Parameters.Add(new OperatorParameter("Successor", "Operator which is executed next"));
    }

    public override ExecutionContextCollection Apply(ExecutionContext context) {
      IOperator successor = Successor.GetValue(context);
      if (successor != null)
        return new ExecutionContextCollection(new ExecutionContext(context.Parent, successor, context.Scope));
      else
        return new ExecutionContextCollection();
    }
  }
}
