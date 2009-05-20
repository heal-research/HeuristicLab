//#region License Information
///* HeuristicLab
// * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
// *
// * This file is part of HeuristicLab.
// *
// * HeuristicLab is free software: you can redistribute it and/or modify
// * it under the terms of the GNU General Public License as published by
// * the Free Software Foundation, either version 3 of the License, or
// * (at your option) any later version.
// *
// * HeuristicLab is distributed in the hope that it will be useful,
// * but WITHOUT ANY WARRANTY; without even the implied warranty of
// * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// * GNU General Public License for more details.
// *
// * You should have received a copy of the GNU General Public License
// * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
// */
//#endregion

//using System;
//using System.Collections.Generic;
//using HeuristicLab.Core;
//using HeuristicLab.Operators;
//using HeuristicLab.Data;
//using System.Threading;
//using System.Diagnostics;

//namespace HeuristicLab.FixedOperators {
//  class FixedOperatorBase : CombinedOperator {
//    protected class SubScopesProcessor : OperatorBase {
//      public SubScopesProcessor(VirtualScope scope) {
//        this.scope = scope;
//      }
//      private VirtualScope scope;
//      public IList<IOperator> operators = new List<IOperator>();
//      public void AddOperator(IOperator op) {
//        operators.Add(op);
//      }

//      public void Unwind(Queue<KeyValuePair<IOperator, VirtualScope>> list, int currentOpIdx) {
//        int i = 0;
//        //for(int idx = 0; idx < scope.Scope.SubScopes.Count; idx++){
//        //  foreach (IOperator op in operators) {
//        //    list.Insert(currentOpIdx+i, new KeyValuePair<IOperator, VirtualScope>(op, new VirtualScope(scope.Scope.SubScopes[idx])));
//        //    i++;
//        //  } // foreach  
//        //} // for idx

//        //for (int idx = scope.Scope.SubScopes.Count - 1; idx >= 0 ; idx--) {
//        //  for (int y = operators.Count - 1; y >= 0; y-- ) {
//        //    list.Insert(currentOpIdx, new KeyValuePair<IOperator, VirtualScope>(operators[y], new VirtualScope(scope.Scope.SubScopes[idx])));
//        //  } // foreach  
//        //} // for idx

//        for (int idx = 0; idx < scope.Scope.SubScopes.Count; idx++) {
//          foreach (IOperator op in operators) {
//            list.Enqueue(new KeyValuePair<IOperator, VirtualScope>(op, new VirtualScope(scope.Scope.SubScopes[idx])));
//            i++;
//          } // foreach  
//        } // for idx
//      }
//    } // ForLoop
    
//    protected class VirtualScope {
//      public VirtualScope(VirtualScope parent, int subscopeIndex) {
//        this.parent = parent;
//        this.subscope = true;
//        this.subscopeIndex = subscopeIndex;
//      }
      
//      public VirtualScope(IScope scope, int subscopeIndex) {
//        this.scope = scope;
//        this.subscope = true;
//        this.subscopeIndex = subscopeIndex;
//      }

//      public VirtualScope(IScope scope) {
//        this.scope = scope;
//      }
//      private IScope scope;
//      private bool subscope;
//      private int subscopeIndex;
//      public IScope Scope {
//        get { return getScope(); }
//      }
//      private IScope getScope() {
//        IScope s;
//        if (parent != null)
//          s = parent.getScope();
//        else
//          s = scope;

//        if (subscope)
//          return s.SubScopes[subscopeIndex];
//        else
//          return scope;
//      }

//      private VirtualScope parent;

//      //public static implicit operator Scope(ScopeDescriptor s) {
//      //  return (Scope)s.getScope();
//      //} 
//    }
//    /// <summary>
//    /// true if algorithm is in execution
//    /// </summary>
//    protected BoolData inExecution;
    
//    /// <summary>
//    /// Current operator in execution.
//    /// </summary>
//    protected IOperator currentOperator;

//    protected IList<KeyValuePair<IOperator, VirtualScope>> operators;
//    protected Queue<KeyValuePair<IOperator, VirtualScope>> operatorsQueue;

//    public FixedOperatorBase() : base() {
//      operators = new List<KeyValuePair<IOperator, VirtualScope>>();
//      operatorsQueue = new Queue<KeyValuePair<IOperator, VirtualScope>>();
//      //AddVariableInfo(new VariableInfo("ExecutionPointer", "Execution pointer for algorithm abortion", typeof(IntData), VariableKind.New));   
//    } // FixedOperatorBase

//    protected void Execute() {
//      inExecution.Data = true;
//      KeyValuePair<IOperator, VirtualScope> ao;
//      int currentOpIdx = 0;
//      while (currentOpIdx < operators.Count && !Canceled) {
//        //ao = operators.Dequeue();
//        if (operatorsQueue.Count > 0) {
//          ao = operatorsQueue.Dequeue();
//        } else {
//          ao = operators[currentOpIdx];
//          currentOpIdx++;
//        }
//        if (ao.Key is SubScopesProcessor) {
//          SubScopesProcessor sp = (SubScopesProcessor)ao.Key;
//          sp.Unwind(operatorsQueue, currentOpIdx);
//        } else
//          ExecuteOperation(ao.Key, ao.Value.Scope);
//      } // while

//      if (operators.Count == 0)
//        inExecution.Data = false;
//    } // Execute()
    
//    protected virtual void Execute(IOperator op, VirtualScope scope) {
//      operators.Add(new KeyValuePair<IOperator, VirtualScope>(op, scope)) ;
//    } // Execute

//    protected void ExecuteOperation(IOperator op, IScope scope) {
//      IOperation operation;
//      currentOperator = op;
//      operation = op.Execute(scope);
//      if (operation != null) {
//        //IOperator currentOperator;
//        Stack<IOperation> executionStack = new Stack<IOperation>();
//        executionStack.Push(op.Execute(scope));

//        while (executionStack.Count > 0) {
//          operation = executionStack.Pop();
//          if (operation is AtomicOperation) {
//            AtomicOperation atomicOperation = (AtomicOperation)operation;
//            IOperation next = null;
//            try {
//              currentOperator = atomicOperation.Operator;
//              next = currentOperator.Execute(atomicOperation.Scope);
//            }
//            catch (Exception) {
//              throw new InvalidOperationException("Invalid Operation occured in FixedBase.Execute");
//            }
//            if (next != null)
//              executionStack.Push(next);
//          } else if (operation is CompositeOperation) {
//            CompositeOperation compositeOperation = (CompositeOperation)operation;
//            for (int i = compositeOperation.Operations.Count - 1; i >= 0; i--)
//              executionStack.Push(compositeOperation.Operations[i]);
//          } // else if
//        } // while
//      } // if (operation != null)
//    } // ExecuteOperation

//    public override IOperation Apply(IScope scope) {
//      try {
//        inExecution = scope.GetVariableValue<BoolData>("InExecution", false);
//      }
//      catch (Exception) {
//        inExecution = new BoolData(false);
//        scope.AddVariable(new Variable("InExecution", inExecution));
//        operators.Clear();
//      } // catch
//      return null;
//    } // Apply

//    public override void Abort() {
//      base.Abort();
//      currentOperator.Abort();
//      //engineThread.Abort();
//    }

//    public bool InExecution {
//      get { return inExecution.Data; }
//    }


//  } // class FixedBase

//  class CancelException : Exception { 
  
//  } // class CancelException
//} // namespace HeuristicLab.FixedOperators
