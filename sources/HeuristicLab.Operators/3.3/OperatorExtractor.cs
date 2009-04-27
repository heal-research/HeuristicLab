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

namespace HeuristicLab.Operators {
  /// <summary>
  /// Retrieves an operator from a specified scope and returns a successor operation with this operation
  /// and scope.
  /// </summary>
  public class OperatorExtractor : OperatorBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return @"An operator extractor retrievs an operator from the scope it is applied on and returns a successor operation containing this operator and the current scope. Lookup for the operator is done recursively.

Operator extractors can be used to get those operators again that have been injected by combined operators."; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="OperatorExtractor"/> with 
    /// one variable info (<c>Operator</c>).
    /// </summary>
    public OperatorExtractor()
      : base() {
      AddVariableInfo(new VariableInfo("Operator", "Extracted operator", typeof(IOperator), VariableKind.In));
    }

    /// <summary>
    /// Gets an operator from the specified <paramref name="scope"/> and returns an 
    /// <see cref="AtomicOperation"/> containing this operator and scope.
    /// </summary>
    /// <param name="scope">The scope where to apply the operator on.</param>
    /// <returns>A new <see cref="AtomicOperation"/> containing the operator and the given
    /// <paramref name="scope"/>.</returns>
    public override IOperation Apply(IScope scope) {
      IOperator op = GetVariableValue<IOperator>("Operator", scope, true, true);
      return new AtomicOperation(op, scope);
    }
  }
}
