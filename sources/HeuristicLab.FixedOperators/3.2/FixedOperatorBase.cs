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
using HeuristicLab.Core;
using HeuristicLab.Operators;
using HeuristicLab.Data;
using System.Threading;
using System.Diagnostics;

namespace HeuristicLab.FixedOperators {
  class FixedOperatorBase : CombinedOperator {

    private class TasksList {
      public IOperator op;
      public IScope scope;
      public ManualResetEvent resetEvent;
    }
    /// <summary>
    /// Execution pointer shows which command actually is executed
    /// </summary>
    protected int executionPointer;

    /// <summary>
    /// Execution pointer if execution was aborted previously
    /// </summary>
    protected IntData persistedExecutionPointer;

    /// <summary>
    /// If true, mini engine is executed in its own thread.
    /// </summary>
    protected bool threaded;

    protected Thread engineThread;

    /// <summary>
    /// Current operator in execution.
    /// </summary>
    protected IOperator currentOperator;

    public FixedOperatorBase() : base() {
      //AddVariableInfo(new VariableInfo("ExecutionPointer", "Execution pointer for algorithm abortion", typeof(IntData), VariableKind.New));   
    } // FixedOperatorBase

    private bool IsExecuted() {
      return persistedExecutionPointer.Data > executionPointer;
    } // AlreadyExecuted

    protected virtual void Execute(IOperator op, IScope scope, bool runInThread) {
      if (!IsExecuted()) {
        if (runInThread) {
          try {
            // causes fatal computing overhead 5:30 minutes for sga
            //engineThread = new Thread(new ThreadStart(ExecuteOperation));
            //engineThread.Start();
            //engineThread.Join();

            // causes computing overhead, 11 sec for sga
            Stopwatch sw = new Stopwatch();
            sw.Start();
            TasksList tl = new TasksList() { op = op, scope = scope, resetEvent = new ManualResetEvent(false) };
            Console.WriteLine(sw.ElapsedTicks);     
            ThreadPool.QueueUserWorkItem(new WaitCallback(ExecuteOperationThreaded), (object)tl);
            Console.WriteLine(sw.ElapsedTicks);
            WaitHandle.WaitAll(new WaitHandle[] { tl.resetEvent });
            Console.WriteLine(sw.ElapsedTicks); 
          }
          catch (ThreadAbortException) {
            return;
          }
        } else {
          ExecuteOperation(op, scope);
        }
        persistedExecutionPointer.Data++;
      } // if not executed
      executionPointer++;
      Console.WriteLine(executionPointer);

      if (Canceled)
        throw new CancelException();
    } // Execute

    private void ExecuteOperationThreaded(object o) {
      TasksList tl = (TasksList)o;
      ExecuteOperation(tl.op, tl.scope);
      tl.resetEvent.Set();
    } // ExecuteOperationThreaded 

    protected void ExecuteOperation(IOperator op, IScope scope) {
      IOperation operation;
      currentOperator = op;
      operation = op.Execute(scope);
      if (operation != null) {
        //IOperator currentOperator;
        Stack<IOperation> executionStack = new Stack<IOperation>();
        executionStack.Push(op.Execute(scope));

        while (executionStack.Count > 0) {
          operation = executionStack.Pop();
          if (operation is AtomicOperation) {
            AtomicOperation atomicOperation = (AtomicOperation)operation;
            IOperation next = null;
            try {
              currentOperator = atomicOperation.Operator;
              next = currentOperator.Execute(atomicOperation.Scope);
            }
            catch (Exception) {
              throw new InvalidOperationException("Invalid Operation occured in FixedBase.Execute");
            }
            if (next != null)
              executionStack.Push(next);
          } else if (operation is CompositeOperation) {
            CompositeOperation compositeOperation = (CompositeOperation)operation;
            for (int i = compositeOperation.Operations.Count - 1; i >= 0; i--)
              executionStack.Push(compositeOperation.Operations[i]);
          } // else if
        } // while
      } // if (operation != null)
    } // ExecuteOperation

    public override IOperation Apply(IScope scope) {
      try {
        persistedExecutionPointer = scope.GetVariableValue<IntData>("ExecutionPointer", false);
      }
      catch (Exception) {
        persistedExecutionPointer = new IntData(0);
        scope.AddVariable(new Variable("ExecutionPointer", persistedExecutionPointer));
      }
      
      executionPointer = 0;
      return null;
    } // Apply

    public override void Abort() {
      base.Abort();
      currentOperator.Abort();
      //engineThread.Abort();
    }



  } // class FixedBase

  class CancelException : Exception { 
  
  } // class CancelException
} // namespace HeuristicLab.FixedOperators
