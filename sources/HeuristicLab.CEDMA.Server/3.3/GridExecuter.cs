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
using System.Windows.Forms;
using HeuristicLab.PluginInfrastructure;
using System.Net;
using System.ServiceModel;
using HeuristicLab.CEDMA.DB.Interfaces;
using HeuristicLab.CEDMA.DB;
using System.ServiceModel.Description;
using System.Linq;
using HeuristicLab.CEDMA.Core;
using HeuristicLab.Data;
using HeuristicLab.Grid;
using System.Diagnostics;
using HeuristicLab.Core;
using System.Threading;
using HeuristicLab.Modeling;

namespace HeuristicLab.CEDMA.Server {
  public class GridExecuter : ExecuterBase {
    private JobManager jobManager;
    private Dictionary<AsyncGridResult, IAlgorithm> activeAlgorithms;

    private TimeSpan StartJobInterval {
      get { return TimeSpan.FromMilliseconds(500); }
    }

    private TimeSpan WaitForFinishedJobsTimeout {
      get { return TimeSpan.FromMilliseconds(100); }
    }

    private TimeSpan WaitForNewJobsInterval {
      get { return TimeSpan.FromSeconds(3); }
    }

    public GridExecuter(IDispatcher dispatcher, IStore store, IGridServer server)
      : base(dispatcher, store) {
      this.jobManager = new JobManager(server);
      activeAlgorithms = new Dictionary<AsyncGridResult, IAlgorithm>();
      jobManager.Reset();
    }

    protected override void StartJobs() {
      Dictionary<WaitHandle, AsyncGridResult> asyncResults = new Dictionary<WaitHandle, AsyncGridResult>();
      while (true) {
        try {
          // start new jobs as long as there are less than MaxActiveJobs 
          while (asyncResults.Count < MaxActiveJobs) {
            Thread.Sleep(StartJobInterval);
            // get an execution from the dispatcher and execute in grid via job-manager
            IAlgorithm algorithm = Dispatcher.GetNextJob();
            if (algorithm != null) {
              AtomicOperation op = new AtomicOperation(algorithm.Engine.OperatorGraph.InitialOperator, algorithm.Engine.GlobalScope);
              ProcessingEngine procEngine = new ProcessingEngine(algorithm.Engine.GlobalScope, op);
              procEngine.OperatorGraph.AddOperator(algorithm.Engine.OperatorGraph.InitialOperator);
              procEngine.OperatorGraph.InitialOperator = algorithm.Engine.OperatorGraph.InitialOperator;
              procEngine.Reset();
              AsyncGridResult asyncResult = jobManager.BeginExecuteEngine(procEngine);
              asyncResults.Add(asyncResult.WaitHandle, asyncResult);
              lock (activeAlgorithms) {
                activeAlgorithms.Add(asyncResult, algorithm);
              }
              OnChanged();
            }
          }
          if (asyncResults.Count > 0) {
            WaitHandle[] whArr = asyncResults.Keys.ToArray();
            int readyHandleIndex = WaitAny(whArr, WaitForFinishedJobsTimeout);
            if (readyHandleIndex != WaitHandle.WaitTimeout) {
              WaitHandle readyHandle = whArr[readyHandleIndex];
              IAlgorithm finishedAlgorithm = null;
              AsyncGridResult finishedResult = null;
              lock (activeAlgorithms) {
                finishedResult = asyncResults[readyHandle];
                finishedAlgorithm = activeAlgorithms[finishedResult];
                activeAlgorithms.Remove(finishedResult);
                asyncResults.Remove(readyHandle);
              }
              OnChanged();
              try {
                IEngine finishedEngine = jobManager.EndExecuteEngine(finishedResult);
                SetResults(finishedEngine.GlobalScope, finishedAlgorithm.Engine.GlobalScope);
                StoreResults(finishedAlgorithm);
              }
              catch (Exception badEx) {
                HeuristicLab.Tracing.Logger.Error("CEDMA Executer: Exception in job execution thread. " + badEx.Message+Environment.NewLine+badEx.StackTrace);
              }
            }
          } else {
            Thread.Sleep(WaitForNewJobsInterval);
          }
        }

        catch (Exception ex) {
          HeuristicLab.Tracing.Logger.Warn("CEDMA Executer: Exception in job-management thread. " + ex.Message + Environment.NewLine + ex.StackTrace);
        }
      }
    }

    // wait until any job is finished
    private int WaitAny(WaitHandle[] wh, TimeSpan WaitForFinishedJobsTimeout) {
      if (wh.Length <= 64) {
        return WaitHandle.WaitAny(wh, WaitForFinishedJobsTimeout);
      } else {
        for (int i = 0; i < wh.Length; i++) {
          if (wh[i].WaitOne(WaitForFinishedJobsTimeout)) {
            return i;
          }
        }
        return WaitHandle.WaitTimeout;
      }
    }

    public override string[] GetJobs() {
      lock (activeAlgorithms) {
        string[] retVal = new string[activeAlgorithms.Count];
        int i = 0;
        foreach (IAlgorithm a in activeAlgorithms.Values) {
          retVal[i++] = a.Name + " " + a.Dataset.GetVariableName(a.TargetVariable);
        }
        return retVal;
      }
    }
  }
}
