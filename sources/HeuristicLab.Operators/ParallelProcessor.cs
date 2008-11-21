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
  /// Performs <c>n</c> operators on the given scope; operations can be executed in parallel.
  /// </summary>
  public class ParallelProcessor : OperatorBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    /// <summary>
    /// Applies <c>n</c> operators on the given <paramref name="scope"/>.
    /// </summary>
    /// <param name="scope">The scope to apply the operators on.</param>
    /// <returns>A new <see cref="CompositeOperation"/> with the <c>n</c> operators applied 
    /// to the given <paramref name="scope"/> with the <c>ExecuteInParallel</c> set to <c>true</c>.</returns>
    public override IOperation Apply(IScope scope) {
      CompositeOperation next = new CompositeOperation();
      next.ExecuteInParallel = true;
      for (int i = 0; i < SubOperators.Count; i++)
        next.AddOperation(new AtomicOperation(SubOperators[i], scope));
      return next;
    }
  }
}
