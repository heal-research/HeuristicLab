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
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Operators;

namespace HeuristicLab.FixedOperators {
  class FixedBase : CombinedOperator {

    protected virtual void Execute(IOperator op, IScope scope){
      IOperation operation;
      IOperator currentOperator;
      Stack<IOperation> executionStack = new Stack<IOperation>();
      executionStack.Push(op.Execute(scope));

      while (executionStack.Count > 0) {
        operation = executionStack.Pop();
        if (operation is AtomicOperation) {
          AtomicOperation atomicOperation = (AtomicOperation)operation;
          IOperation next = null;
          try {
            currentOperator = atomicOperation.Operator;
            next = atomicOperation.Operator.Execute(atomicOperation.Scope);
          }
          catch (Exception) {
            //Abort();
            //ThreadPool.QueueUserWorkItem(delegate(object state) { OnExceptionOccurred(ex); });
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
    } // Execute

  } // class FixedBase
} // namespace HeuristicLab.FixedOperators
