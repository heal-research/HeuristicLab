#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Core {
  public class OperatorExecutionException : Exception {
    private readonly IOperator op;
    public IOperator Operator {
      get { return op; }
    }

    private readonly string message;
    public override string Message {
      get {
        string name = "\"" + op.Name + "\"";
        var assembly = op.GetType().Assembly;
        if (!op.Name.Equals(op.ItemName)) name += " (" + op.ItemName + ")";
        if (!string.IsNullOrEmpty(assembly.Location)) {
          name += " [" + assembly.Location + ": " + AssemblyHelpers.GetFileVersion(assembly) + "]";
        }
        if (InnerException == null)
          return base.Message + name + message + ".";
        else
          return base.Message + name + ": " + InnerException.Message;
      }
    }

    public OperatorExecutionException(IOperator op) : this(op, string.Empty) { }
    public OperatorExecutionException(IOperator op, string message)
      : base("An exception was thrown by the operator ") {
      if (op == null) throw new ArgumentNullException();
      this.op = op;
      this.message = message;
    }
    public OperatorExecutionException(IOperator op, Exception innerException)
      : base("An exception was thrown by the operator ", innerException) {
      if (op == null) throw new ArgumentNullException();
      this.op = op;
    }
  }
}
