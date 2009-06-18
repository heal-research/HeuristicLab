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
using HeuristicLab.Modeling;

namespace HeuristicLab.CEDMA.Server {
  public class HiveExecuter : ExecuterBase {
    private Dictionary<WaitHandle, IAlgorithm> activeAlgorithms;
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
      activeAlgorithms = new Dictionary<WaitHandle, IAlgorithm>();
      activeEngines = new Dictionary<WaitHandle, HiveEngine>();
    }

    protected override void StartJobs() {
      List<WaitHandle> wh = new List<WaitHandle>();
      while (true) {
        try {
          // start new jobs as long as there are less than MaxActiveJobs 
          while (wh.Count < MaxActiveJobs) {
            Thread.Sleep(StartJobInterval);
            // get an algo from the dispatcher and execute in grid via job-manager
            IAlgorithm algorithm = Dispatcher.GetNextJob();
            if (algorithm != null) {
              HiveEngine engine = new HiveEngine();
              engine.HiveServerUrl = hiveUrl;
              IOperator initialOp = algorithm.Engine.OperatorGraph.InitialOperator;
              engine.OperatorGraph.AddOperator(initialOp);
              engine.OperatorGraph.InitialOperator = initialOp;
              RegisterFinishedCallback(engine, algorithm, wh);
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

            IAlgorithm finishedAlgorithm = activeAlgorithms[readyHandle];
            lock (activeAlgorithms) {
              activeAlgorithms.Remove(readyHandle);
            }
            SetResults(finishedEngine.GlobalScope, finishedAlgorithm.Engine.GlobalScope);
            StoreResults(finishedAlgorithm);
            readyHandle.Close();
          }
        }
        catch (Exception ex) {
          HeuristicLab.Tracing.HiveLogger.Debug("CEDMA Executer: Exception in job-management thread. " + ex.Message + "\n" + ex.StackTrace);
        }
      }
    }

    private void RegisterFinishedCallback(HiveEngine engine, IAlgorithm algorithm, List<WaitHandle> wh) {
      ManualResetEvent waithandle = new ManualResetEvent(false);
      wh.Add(waithandle);
      engine.Finished += (sender, args) => waithandle.Set();
      lock (activeAlgorithms) {
        activeAlgorithms.Add(waithandle, algorithm);
      }
      activeEngines.Add(waithandle, engine);
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
