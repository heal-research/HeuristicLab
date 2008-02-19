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
using System.Threading;

namespace HeuristicLab.SequentialEngine {
  public class SequentialEngine : EngineBase, IEditable {
    private IOperator currentOperator;

    public override IView CreateView() {
      return new SequentialEngineEditor(this);
    }
    public virtual IEditor CreateEditor() {
      return new SequentialEngineEditor(this);
    }

    public override void Abort() {
      base.Abort();
      if (currentOperator != null)
        currentOperator.Abort();
    }

    protected override void ProcessNextOperation() {
      IOperation operation = myExecutionStack.Pop();
      if (operation is AtomicOperation) {
        AtomicOperation atomicOperation = (AtomicOperation)operation;
        IOperation next = null;
        try {
          currentOperator = atomicOperation.Operator;
          next = atomicOperation.Operator.Execute(atomicOperation.Scope);
        }
        catch (Exception ex) {
          // push operation on stack again
          myExecutionStack.Push(atomicOperation);
          Abort();
          ThreadPool.QueueUserWorkItem(delegate(object state) { OnExceptionOccurred(ex);});
        }
        if (next != null)
          myExecutionStack.Push(next);
        OnOperationExecuted(atomicOperation);
        if (atomicOperation.Operator.Breakpoint) Abort();
      } else if (operation is CompositeOperation) {
        CompositeOperation compositeOperation = (CompositeOperation)operation;
        for (int i = compositeOperation.Operations.Count - 1; i >= 0; i--)
          myExecutionStack.Push(compositeOperation.Operations[i]);
      }
    }
  }
}
