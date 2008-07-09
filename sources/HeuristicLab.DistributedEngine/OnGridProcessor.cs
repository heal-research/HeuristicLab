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
using HeuristicLab.Data;
using HeuristicLab.Grid;
using System.Threading;

namespace HeuristicLab.DistributedEngine {
  public class OnGridProcessor : OperatorBase {
    public override string Description {
      get { return "TASK."; }
    }

    public OnGridProcessor()
      : base() {
      AddVariableInfo(new VariableInfo("OperatorGraph", "The operator graph that should be executed on the grid", typeof(IOperatorGraph), VariableKind.In));
      AddVariableInfo(new VariableInfo("GridServerUrl", "Url of the grid server", typeof(StringData), VariableKind.In));
    }

    public override IOperation Apply(IScope scope) {
      IOperatorGraph operatorGraph = scope.GetVariableValue<IOperatorGraph>("OperatorGraph", false);
      string gridServerUrl = scope.GetVariableValue<StringData>("GridServerUrl", true).Data;
      JobManager jobManager = new JobManager(gridServerUrl);
      jobManager.Reset();
      Scope globalScope = new Scope();
      AtomicOperation operation = new AtomicOperation(operatorGraph.InitialOperator, globalScope);
      WaitHandle w = jobManager.BeginExecuteOperation(globalScope, operation);
      
      ThreadPool.QueueUserWorkItem(delegate(object status) {
        w.WaitOne();
        jobManager.EndExecuteOperation(operation);
      });
      return null;
    }
  }
}
