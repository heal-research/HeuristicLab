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
  public sealed class SequentialProcessor : MultipleSuccessorsOperator {
    public SequentialProcessor()
      : base() {
    }

    public override IExecutionContext Apply() {
      ExecutionContextCollection next = new ExecutionContextCollection();
      for (int i = 0; i < Successors.Count; i++) {
        if (Successors[i] != null)
          next.Add(new ExecutionContext(ExecutionContext.Parent, Successors[i], ExecutionContext.Scope));
      }
      return next;
    }
  }
}
