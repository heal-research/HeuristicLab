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

namespace HeuristicLab.Operators {
  /// <summary>
  /// Placeholder and also used for testing; Does nothing.
  /// </summary>
  public class EmptyOperator : OperatorBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return "An empty operator just does nothing. Useful for testing and as a place holder for sub-operators of SequentialSubScopesProcessor and ParallelSubScopesProcessor."; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="EmptyOperator"/>.
    /// </summary>
    public EmptyOperator()
      : base() {
    }

    /// <summary>
    /// Does nothing.
    /// </summary>
    /// <param name="scope">The scope to apply the operator on.</param>
    /// <returns><c>null</c>.</returns>
    public override IOperation Apply(IScope scope) {
      return null;
    }
  }
}
