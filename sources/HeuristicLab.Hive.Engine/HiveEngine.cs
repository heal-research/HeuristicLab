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
using HeuristicLab.Hive.JobBase;
using HeuristicLab.Hive.Contracts.Interfaces;
using HeuristicLab.Hive.Contracts;

namespace HeuristicLab.Hive.Engine {
  /// <summary>
  /// Represents an engine that executes its operator-graph on the hive.
  /// in parallel.
  /// </summary>
  public class HiveEngine : ItemBase, IEngine, IEditable {
    private Job job;
    public string HiveServerUrl { get; set; }

    public HiveEngine() {
      job = new Job();
    }


    #region IEngine Members

    public IOperatorGraph OperatorGraph {
      get { return job.Engine.OperatorGraph; }
    }

    public IScope GlobalScope {
      get { return job.Engine.GlobalScope; }
    }

    public TimeSpan ExecutionTime {
      get { return job.Engine.ExecutionTime; }
    }

    public bool Running {
      get { return job.Engine.Running; }
    }

    public bool Canceled {
      get { return job.Engine.Canceled; }
    }

    public bool Terminated {
      get { return job.Engine.Terminated; }
    }

    public void Execute() {
      IExecutionEngineFacade executionEngineFacade = ServiceLocator.CreateExecutionEngineFacade(HiveServerUrl);
      HeuristicLab.Hive.Contracts.BusinessObjects.Job jobObj = new HeuristicLab.Hive.Contracts.BusinessObjects.Job();
      jobObj.SerializedJob = PersistenceManager.SaveToGZip(job);
      jobObj.State = HeuristicLab.Hive.Contracts.BusinessObjects.State.offline;
      ResponseObject<Contracts.BusinessObjects.Job> res = executionEngineFacade.AddJob(jobObj);
    }

    public void ExecuteStep() {
      throw new NotSupportedException();
    }

    public void ExecuteSteps(int steps) {
      throw new NotSupportedException();
    }

    public void Abort() {
      throw new NotImplementedException();
    }

    public void Reset() {
      throw new NotImplementedException();
    }

    public event EventHandler Initialized;

    public event EventHandler<OperationEventArgs> OperationExecuted;

    public event EventHandler<ExceptionEventArgs> ExceptionOccurred;

    public event EventHandler ExecutionTimeChanged;

    public event EventHandler Finished;

    #endregion

    public void RequestSnapshot() {
      throw new NotImplementedException();
    }

    public override IView CreateView() {
      return new HiveEngineEditor(this);
    }

    #region IEditable Members

    public IEditor CreateEditor() {
      return new HiveEngineEditor(this);
    }
    #endregion
  }
}
