#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Core {
  public class OperatorExecutionException : Exception {
    private IOperator op;
    public IOperator Operator {
      get { return op; }
    }
    public override string Message {
      get {
        string name = "\"" + op.Name + "\"";
        if (!op.Name.Equals(op.ItemName)) name += " (" + op.ItemName + ")";

        if (InnerException == null)
          return base.Message + name + ".";
        else
          return base.Message + name + ": " + InnerException.Message;
      }
    }

    public OperatorExecutionException(IOperator op)
      : base("An exception was thrown by the operator ") {
      if (op == null) throw new ArgumentNullException();
      this.op = op;
    }
    public OperatorExecutionException(IOperator op, Exception innerException)
      : base("An exception was thrown by the operator ", innerException) {
      if (op == null) throw new ArgumentNullException();
      this.op = op;
    }
  }
}
