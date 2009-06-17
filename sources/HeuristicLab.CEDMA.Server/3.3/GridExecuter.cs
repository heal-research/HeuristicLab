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
using HeuristicLab.Modeling;

namespace HeuristicLab.CEDMA.Server {
  public class GridExecuter : ExecuterBase {
    private JobManager jobManager;
    private Dictionary<WaitHandle, IAlgorithm> activeAlgorithms;

    private TimeSpan StartJobInterval {
      get { return TimeSpan.FromMilliseconds(500); }
    }

    private TimeSpan WaitForFinishedJobsTimeout {
      get { return TimeSpan.FromMilliseconds(100); }
    }

    public GridExecuter(IDispatcher dispatcher, IStore store, string gridUrl)
      : base(dispatcher, store) {
      this.jobManager = new JobManager(gridUrl);
      activeAlgorithms = new Dictionary<WaitHandle, IAlgorithm>();
      jobManager.Reset();
    }

    protected override void StartJobs() {
      List<WaitHandle> wh = new List<WaitHandle>();
      Dictionary<WaitHandle, AtomicOperation> activeOperations = new Dictionary<WaitHandle, AtomicOperation>();
      while (true) {
        try {
          // start new jobs as long as there are less than MaxActiveJobs 
          while (wh.Count < MaxActiveJobs) {
            Thread.Sleep(StartJobInterval);
            // get an execution from the dispatcher and execute in grid via job-manager
            IAlgorithm algorithm = Dispatcher.GetNextJob();
            if (algorithm != null) {
              AtomicOperation op = new AtomicOperation(algorithm.Engine.OperatorGraph.InitialOperator, algorithm.Engine.GlobalScope);
              WaitHandle opWh = jobManager.BeginExecuteOperation(algorithm.Engine.GlobalScope, op);
              wh.Add(opWh);
              activeOperations.Add(opWh, op);
              lock (activeAlgorithms) {
                activeAlgorithms.Add(opWh, algorithm);
              }
            }
          }
          // wait until any job is finished
          WaitHandle[] whArr = wh.ToArray();
          int readyHandleIndex = WaitHandle.WaitAny(whArr, WaitForFinishedJobsTimeout);
          if (readyHandleIndex != WaitHandle.WaitTimeout) {
            WaitHandle readyHandle = whArr[readyHandleIndex];
            AtomicOperation finishedOp = activeOperations[readyHandle];
            wh.Remove(readyHandle);
            IAlgorithm finishedAlgorithm = null;
            lock (activeAlgorithms) {
              finishedAlgorithm = activeAlgorithms[readyHandle];
              activeAlgorithms.Remove(readyHandle);
            }
            activeOperations.Remove(readyHandle);
            readyHandle.Close();
            try {
              ProcessingEngine finishedEngine = jobManager.EndExecuteOperation(finishedOp);
              StoreResults(finishedAlgorithm);
            }
            catch (Exception badEx) {
              Trace.WriteLine("CEDMA Executer: Exception in job execution thread. " + badEx.Message);
            }
          }
        }
        catch (Exception ex) {
          Trace.WriteLine("CEDMA Executer: Exception in job-management thread. " + ex.Message);
        }
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
