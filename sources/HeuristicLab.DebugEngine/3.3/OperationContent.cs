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

using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.DebugEngine {
  public class OperationContent : IContent {

    public IOperation Operation { get; private set; }
    public OperationCollection Collection { get; private set; }
    public IAtomicOperation AtomicOperation { get; private set; }
    public ExecutionContext ExecutionContext { get; private set; }
    public string Name { get; private set; }

    public OperationContent(IOperation operation) {
      Operation = operation;
      Collection = operation as OperationCollection;
      AtomicOperation = operation as IAtomicOperation;
      ExecutionContext = operation as ExecutionContext;
      if (AtomicOperation != null) {
        Name = Utils.Name(AtomicOperation);
      } else if (Collection != null) {
        Name = string.Format("{0} Operations", Collection.Count);
      } else {
        Name = "";
      }
    }

    public bool IsCollection { get { return Collection != null; } }
    public bool IsAtomic { get { return AtomicOperation != null; } }
    public bool IsContext { get { return ExecutionContext != null; } }
  }
}
