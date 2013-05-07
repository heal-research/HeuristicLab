#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Threading;
using System.Threading.Tasks;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.ParallelEngine {
  /// <summary>
  /// Represents an engine that executes its steps in parallel (if possible) using multiple threads.
  /// This engine is suitable for parallel processing on shared memory systems which provide multiple cores.
  /// </summary>
  [StorableClass]
  [Item("Parallel Engine", "Engine for parallel execution of algorithms using multiple threads (suitable for shared memory systems with multiple cores).")]
  public class ParallelEngine : Engine {
    private CancellationToken cancellationToken;
    private ParallelOptions parallelOptions;

    [Storable(DefaultValue = -1)]
    private int degreeOfParallelism;
    public int DegreeOfParallelism {
      get { return degreeOfParallelism; }
      set {
        if (degreeOfParallelism != value) {
          degreeOfParallelism = value;
          OnDegreeOfParallelismChanged();
        }
      }
    }

    [StorableConstructor]
    protected ParallelEngine(bool deserializing) : base(deserializing) { }
    protected ParallelEngine(ParallelEngine original, Cloner cloner)
      : base(original, cloner) {
      this.DegreeOfParallelism = original.DegreeOfParallelism;
    }
    public ParallelEngine()
      : base() {
      this.degreeOfParallelism = -1;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ParallelEngine(this, cloner);
    }

    public event EventHandler DegreeOfParallelismChanged;
    protected void OnDegreeOfParallelismChanged() {
      var handler = DegreeOfParallelismChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }


    protected override void Run(CancellationToken cancellationToken) {
      this.cancellationToken = cancellationToken;
      parallelOptions = new ParallelOptions();
      parallelOptions.MaxDegreeOfParallelism = DegreeOfParallelism;
      parallelOptions.CancellationToken = cancellationToken;
      Run(ExecutionStack);
    }

    private void Run(object state) {
      Stack<IOperation> executionStack = (Stack<IOperation>)state;
      IOperation next;
      OperationCollection coll;
      IAtomicOperation operation;

      while (executionStack.Count > 0) {
        cancellationToken.ThrowIfCancellationRequested();

        next = executionStack.Pop();
        if (next is OperationCollection) {
          coll = (OperationCollection)next;
          if (coll.Parallel) {
            Stack<IOperation>[] stacks = new Stack<IOperation>[coll.Count];
            for (int i = 0; i < coll.Count; i++) {
              stacks[i] = new Stack<IOperation>();
              stacks[i].Push(coll[i]);
            }
            try {
              Parallel.ForEach(stacks, parallelOptions, Run);
            }
            catch (OperationCanceledException ex) {
              OperationCollection remaining = new OperationCollection() { Parallel = true };
              for (int i = 0; i < stacks.Length; i++) {
                if (stacks[i].Count == 1)
                  remaining.Add(stacks[i].Pop());
                if (stacks[i].Count > 1) {
                  OperationCollection ops = new OperationCollection();
                  while (stacks[i].Count > 0)
                    ops.Add(stacks[i].Pop());
                  remaining.Add(ops);
                }
              }
              if (remaining.Count > 0) executionStack.Push(remaining);
              throw ex;
            }
          } else {
            for (int i = coll.Count - 1; i >= 0; i--)
              if (coll[i] != null) executionStack.Push(coll[i]);
          }
        } else if (next is IAtomicOperation) {
          operation = (IAtomicOperation)next;
          try {
            next = operation.Operator.Execute((IExecutionContext)operation, cancellationToken);
          }
          catch (Exception ex) {
            executionStack.Push(operation);
            if (ex is OperationCanceledException) throw ex;
            else throw new OperatorExecutionException(operation.Operator, ex);
          }
          if (next != null) executionStack.Push(next);

          if (operation.Operator.Breakpoint) {
            string message = string.Format("Breakpoint: {0}", operation.Operator.Name != string.Empty ? operation.Operator.Name : operation.Operator.ItemName);
            Log.LogMessage(message);
            throw new OperationCanceledException(message);
          }
        }
      }
    }
  }
}
