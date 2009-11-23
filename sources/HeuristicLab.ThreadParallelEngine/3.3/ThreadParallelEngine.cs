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
using System.Threading;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.ThreadParallelEngine {
  /// <summary>
  /// Implementation of an engine being able to run in parallel with threads.
  /// </summary>
  public class ThreadParallelEngine : EngineBase {
    #region Inner Class Task
    private class TaskList {
      public Stack<IOperation>[] tasks;
      public int next;
      public Semaphore semaphore;
    }
    #endregion

    // currently executed operators
    private IOperator[] currentOperators;
    private int operatorIndex;

    [Storable]
    private int myWorkers;
    /// <summary>
    /// Gets or sets the number of worker threads of the current engine.
    /// </summary>
    /// <remarks>Calls <see cref="OnWorkersChanged"/> in the setter.</remarks>
    public int Workers {
      get { return myWorkers; }
      set {
        if (value != myWorkers) {
          myWorkers = value;
          OnWorkersChanged();
        }
      }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ThreadParallelEngine"/> with the number of processors
    /// as number of worker threads.
    /// </summary>
    public ThreadParallelEngine() {
      myWorkers = Environment.ProcessorCount;
      currentOperators = new IOperator[1000];
    }


    /// <inheritdoc/>
    /// <returns>The cloned object as <see cref="ThreadParallelEngine"/>.</returns>
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      ThreadParallelEngine clone = (ThreadParallelEngine)base.Clone(clonedObjects);
      clone.myWorkers = Workers;
      return clone;
    }

    /// <summary>
    /// This execution method is not supported by ThreadParallelEngines.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown because the current instance of an engine
    /// does not support stepwise execution.</exception>
    /// <param name="steps">The number of steps to execute.</param>
    public override void ExecuteSteps(int steps) {
      throw new InvalidOperationException("ThreadParallelEngine doesn't support stepwise execution");
    }

    /// <inheritdoc/>
    public override void Abort() {
      base.Abort();
      for (int i = 0; i < currentOperators.Length; i++) {
        if (currentOperators[i] != null)
          currentOperators[i].Abort();
      }
    }
    /// <summary>
    /// Processes the next operation (if it is a compositeOperation and it can be executed in parallel it is 
    /// done).
    /// </summary>
    protected override void ProcessNextOperation() {
      operatorIndex = 1;
      ProcessNextOperation(myExecutionStack, 0);
    }
    private void ProcessNextOperation(Stack<IOperation> stack, int currentOperatorIndex) {
      IOperation operation = stack.Pop();
      if (operation is AtomicOperation) {
        AtomicOperation atomicOperation = (AtomicOperation)operation;
        IOperation next = null;
        try {
          currentOperators[currentOperatorIndex] = atomicOperation.Operator;
          next = atomicOperation.Operator.Execute(atomicOperation.Scope);
        }
        catch (Exception ex) {
          // push operation on stack again
          stack.Push(atomicOperation);
          Abort();
          ThreadPool.QueueUserWorkItem(delegate(object state) { OnExceptionOccurred(ex); });
        }
        if (next != null)
          stack.Push(next);
        OnOperationExecuted(atomicOperation);
        if (atomicOperation.Operator.Breakpoint) Abort();
      } else if (operation is CompositeOperation) {
        CompositeOperation compositeOperation = (CompositeOperation)operation;
        if (compositeOperation.ExecuteInParallel) {
          TaskList list = new TaskList();
          list.tasks = new Stack<IOperation>[compositeOperation.Operations.Count];
          for (int i = 0; i < list.tasks.Length; i++) {
            list.tasks[i] = new Stack<IOperation>();
            list.tasks[i].Push(compositeOperation.Operations[i]);
          }
          list.next = 0;
          list.semaphore = new Semaphore(0, Workers);

          for (int i = 0; i < Workers; i++)
            ThreadPool.QueueUserWorkItem(new WaitCallback(ProcessParallelOperation), list);
          for (int i = 0; i < Workers; i++)
            list.semaphore.WaitOne();
          list.semaphore.Close();

          if (Canceled) {
            // write back not finished tasks
            CompositeOperation remaining = new CompositeOperation();
            remaining.ExecuteInParallel = true;
            for (int i = 0; i < list.tasks.Length; i++) {
              if (list.tasks[i].Count > 0) {
                CompositeOperation task = new CompositeOperation();
                while (list.tasks[i].Count > 0)
                  task.AddOperation(list.tasks[i].Pop());
                remaining.AddOperation(task);
              }
            }
            if (remaining.Operations.Count > 0)
              stack.Push(remaining);
          }
        } else {
          for (int i = compositeOperation.Operations.Count - 1; i >= 0; i--)
            stack.Push(compositeOperation.Operations[i]);
        }
      }
    }
    private void ProcessParallelOperation(object state) {
      TaskList list = (TaskList)state;
      int currentOperatorIndex, next;

      do {
        lock (currentOperators) {
          currentOperatorIndex = operatorIndex;
          operatorIndex++;
        }
        lock (list) {
          next = list.next;
          list.next++;
        }

        if (next < list.tasks.Length) {
          Stack<IOperation> stack = list.tasks[next];
          while ((!Canceled) && (stack.Count > 0))
            ProcessNextOperation(stack, currentOperatorIndex);
        }
      } while ((!Canceled) && (next < list.tasks.Length));
      list.semaphore.Release();
    }

    /// <summary>
    /// Occurs when the number of workers has been changed.
    /// </summary>
    public event EventHandler WorkersChanged;
    /// <summary>
    /// Fires a new <c>WorkersChanged</c> event.
    /// </summary>
    protected virtual void OnWorkersChanged() {
      if (WorkersChanged != null)
        WorkersChanged(this, new EventArgs());
    }
  }
}
