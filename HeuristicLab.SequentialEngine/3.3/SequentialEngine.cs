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
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Fossil;

namespace HeuristicLab.SequentialEngine {
  /// <summary>
  /// Engine for sequential execution of algorithms.
  /// </summary>
  [StorableType("2100B28B-18B3-41F5-B8A2-1DD009ADF4A9")]
  [Item("Sequential Engine", "Engine for sequential execution of algorithms.")]
  public class SequentialEngine : Engine {
    [StorableConstructor]
    protected SequentialEngine(StorableConstructorFlag _) : base(_) { }
    protected SequentialEngine(SequentialEngine original, Cloner cloner) : base(original, cloner) { }
    public SequentialEngine() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SequentialEngine(this, cloner);
    }

    protected override void Run(CancellationToken cancellationToken) {
      IOperation next;
      OperationCollection coll;
      IAtomicOperation operation;

      while (ExecutionStack.Count > 0) {
        cancellationToken.ThrowIfCancellationRequested();

        next = ExecutionStack.Pop();
        if (next is OperationCollection) {
          coll = (OperationCollection)next;
          for (int i = coll.Count - 1; i >= 0; i--)
            if (coll[i] != null) ExecutionStack.Push(coll[i]);
        } else if (next is IAtomicOperation) {
          operation = (IAtomicOperation)next;
          try {
            next = operation.Operator.Execute((IExecutionContext)operation, cancellationToken);
          }
          catch (Exception ex) {
            ExecutionStack.Push(operation);
            if (ex is OperationCanceledException) throw;
            else throw new OperatorExecutionException(operation.Operator, ex);
          }
          if (next != null) ExecutionStack.Push(next);
        }
      }
    }
  }
}
