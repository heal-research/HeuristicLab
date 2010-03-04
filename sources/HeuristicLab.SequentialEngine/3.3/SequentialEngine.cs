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
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.SequentialEngine {
  /// <summary>
  /// Represents an engine that executes its steps sequentially, also if they could be executed 
  /// in parallel.
  /// </summary>
  [EmptyStorableClass]
  [Item("Sequential Engine", "Engine for sequential execution of algorithms.")]
  public class SequentialEngine : Engine {
    private IOperator currentOperator;

    /// <summary>
    /// Deals with the next operation, if it is an <see cref="AtomicOperation"/> it is executed,
    /// if it is a <see cref="CompositeOperation"/> its single operations are pushed on the execution stack.
    /// </summary>
    /// <remarks>If an error occurs during the execution the operation is aborted and the operation
    /// is pushed on the stack again.<br/>
    /// If the execution was successful <see cref="EngineBase.OnOperationExecuted"/> is called.</remarks>
    protected override void ProcessNextOperator() {
      currentOperator = null;
      IOperation next = ExecutionStack.Pop();
      OperationCollection coll = next as OperationCollection;
      while (coll != null) {
        for (int i = coll.Count - 1; i >= 0; i--)
          ExecutionStack.Push(coll[i]);
        next = ExecutionStack.Count > 0 ? ExecutionStack.Pop() : null;
        coll = next as OperationCollection;
      }
      IAtomicOperation operation = next as IAtomicOperation;
      if (operation != null) {
        try {
          currentOperator = operation.Operator;
          ExecutionStack.Push(operation.Operator.Execute((IExecutionContext)operation));
          currentOperator = null;
        }
        catch (Exception ex) {
          ExecutionStack.Push(operation);
          Stop();
          OnExceptionOccurred(ex);
        }
        if (operation.Operator.Breakpoint)
          Stop();
      }
    }

    protected override void OnCanceledChanged() {
      if (Canceled && (currentOperator != null))
        currentOperator.Abort();
    }
  }
}
