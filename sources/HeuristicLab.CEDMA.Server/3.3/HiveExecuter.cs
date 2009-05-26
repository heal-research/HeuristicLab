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
using HeuristicLab.GP.StructureIdentification;
using HeuristicLab.Data;
using HeuristicLab.Grid;
using System.Diagnostics;
using HeuristicLab.Core;
using System.Threading;
using HeuristicLab.Hive.Engine;

namespace HeuristicLab.CEDMA.Server {
  public class HiveExecuter : ExecuterBase {
    private Dictionary<WaitHandle, Execution> activeExecutions;
    private Dictionary<WaitHandle, HiveEngine> activeEngines;
    private string hiveUrl;

    private TimeSpan StartJobInterval {
      get { return TimeSpan.FromMilliseconds(500); }
    }

    private TimeSpan WaitForFinishedJobsTimeout {
      get { return TimeSpan.FromMilliseconds(100); }
    }

    public HiveExecuter(IDispatcher dispatcher, IStore store, string hiveUrl)
      : base(dispatcher, store) {
      this.hiveUrl = hiveUrl;
      activeExecutions = new Dictionary<WaitHandle, Execution>();
      activeEngines = new Dictionary<WaitHandle, HiveEngine>();
    }

    protected override void StartJobs() {
      List<WaitHandle> wh = new List<WaitHandle>();
      while (true) {
        try {
          // start new jobs as long as there are less than MaxActiveJobs 
          while (wh.Count < MaxActiveJobs) {
            Thread.Sleep(StartJobInterval);
            // get an execution from the dispatcher and execute in grid via job-manager
            Execution execution = Dispatcher.GetNextJob();
            if (execution != null) {
              HiveEngine engine = new HiveEngine();
              engine.HiveServerUrl = hiveUrl;
              IOperator initialOp = execution.Engine.OperatorGraph.InitialOperator;
              engine.OperatorGraph.AddOperator(initialOp);
              engine.OperatorGraph.InitialOperator = initialOp;
              RegisterFinishedCallback(engine, execution, wh);
              engine.Reset();
              engine.Execute();
            }
          }
          // wait until any job is finished
          WaitHandle[] whArr = wh.ToArray();
          int readyHandleIndex = WaitHandle.WaitAny(whArr, WaitForFinishedJobsTimeout);
          if (readyHandleIndex != WaitHandle.WaitTimeout) {
            WaitHandle readyHandle = whArr[readyHandleIndex];
            wh.Remove(readyHandle);
            HiveEngine finishedEngine = activeEngines[readyHandle];
            activeEngines.Remove(readyHandle);

            Execution finishedExecution = activeExecutions[readyHandle];
            lock (activeExecutions) {
              activeExecutions.Remove(readyHandle);
            }
            StoreResults(finishedExecution, finishedEngine);
            readyHandle.Close();
          }
        }
        catch (Exception ex) {
          Trace.WriteLine("CEDMA Executer: Exception in job-management thread. " + ex.Message);
        }
      }
    }

    private void RegisterFinishedCallback(HiveEngine engine, Execution execution, List<WaitHandle> wh) {
      ManualResetEvent waithandle = new ManualResetEvent(false);
      wh.Add(waithandle);
      engine.Finished += (sender, args) => waithandle.Set();
      lock (activeExecutions) {
        activeExecutions.Add(waithandle, execution);
      }
      activeEngines.Add(waithandle, engine);
    }

    public override string[] GetJobs() {
      lock (activeExecutions) {
        string[] retVal = new string[activeExecutions.Count];
        int i = 0;
        foreach (Execution e in activeExecutions.Values) {
          retVal[i++] = "Target-Variable: " + e.TargetVariable + " " + e.Description;
        }
        return retVal;
      }
    }
  }
}
