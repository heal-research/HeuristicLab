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
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using System.IO;
using System.Xml;
using System.IO.Compression;

namespace HeuristicLab.Hive.Engine {
  /// <summary>
  /// Represents an engine that executes its operator-graph on the hive.
  /// in parallel.
  /// </summary>
  public class HiveEngine : ItemBase, IEngine, IEditable {
    private const int SNAPSHOT_POLLING_INTERVAL_MS = 1000;
    private const int RESULT_POLLING_INTERVAL_MS = 10000;
    private Guid jobId;
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

    public ThreadPriority Priority {
      get { return job.Engine.Priority; }
      set { job.Engine.Priority = value; }
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
      var jobObj = CreateJobObj();

      IExecutionEngineFacade executionEngineFacade = ServiceLocator.CreateExecutionEngineFacade(HiveServerUrl);
      ResponseObject<Contracts.BusinessObjects.Job> res = executionEngineFacade.AddJob(jobObj);
      jobId = res.Obj.Id;

      StartResultPollingThread();
    }

    private void StartResultPollingThread() {
      // start a backgroud thread to poll the final result of the job
      Thread t = new Thread(() => {
        IExecutionEngineFacade executionEngineFacade = ServiceLocator.CreateExecutionEngineFacade(HiveServerUrl);
        ResponseObject<JobResult> response = null;
        do {
          response = executionEngineFacade.GetLastResult(jobId, true);
          if (response.Success && response.StatusMessage == ApplicationConstants.RESPONSE_JOB_RESULT_NOT_YET_HERE) {
            Thread.Sleep(RESULT_POLLING_INTERVAL_MS);
          }
        } while (response.Success && response.StatusMessage == ApplicationConstants.RESPONSE_JOB_RESULT_NOT_YET_HERE);
        if (response.Success) {
          JobResult jobResult = response.Obj;
          if (jobResult != null) {
            job = (Job)PersistenceManager.RestoreFromGZip(jobResult.Result);
            OnFinished();
          }
        } else {
          Exception ex = new Exception(response.Obj.Exception.Message);
          ThreadPool.QueueUserWorkItem(delegate(object state) { OnExceptionOccurred(ex); });
        }
      });
      t.Start();
    }

    public void RequestSnapshot() {
      IExecutionEngineFacade executionEngineFacade = ServiceLocator.CreateExecutionEngineFacade(HiveServerUrl);

      // poll until snapshot is ready
      ResponseObject<JobResult> response;

      // request snapshot
      Response snapShotResponse = executionEngineFacade.RequestSnapshot(jobId);
      if (snapShotResponse.StatusMessage == ApplicationConstants.RESPONSE_JOB_IS_NOT_BEEING_CALCULATED) {
        response = executionEngineFacade.GetLastResult(jobId, false);
      } else {
        do {
          response = executionEngineFacade.GetLastResult(jobId, true);
          if (response.Success && response.StatusMessage == ApplicationConstants.RESPONSE_JOB_RESULT_NOT_YET_HERE) {
            Thread.Sleep(SNAPSHOT_POLLING_INTERVAL_MS);
          }
        } while (response.Success && response.StatusMessage == ApplicationConstants.RESPONSE_JOB_RESULT_NOT_YET_HERE);
      }
      if (response.Success) {
        JobResult jobResult = response.Obj;
        if (jobResult != null) {
          job = (Job)PersistenceManager.RestoreFromGZip(jobResult.Result);
          //PluginManager.ControlManager.ShowControl(job.Engine.CreateView());
        }
      } else {
        Exception ex = new Exception(response.Obj.Exception.Message);
        ThreadPool.QueueUserWorkItem(delegate(object state) { OnExceptionOccurred(ex); });
      }
    }

    public void ExecuteStep() {
      throw new NotSupportedException();
    }

    public void ExecuteSteps(int steps) {
      throw new NotSupportedException();
    }

    public void Abort() {
      IExecutionEngineFacade executionEngineFacade = ServiceLocator.CreateExecutionEngineFacade(HiveServerUrl);
      executionEngineFacade.AbortJob(jobId);
      OnFinished();
    }

    public void Reset() {
      job.Engine.Reset();
      jobId = Guid.NewGuid();
      OnInitialized();
    }

    private HeuristicLab.Hive.Contracts.BusinessObjects.Job CreateJobObj() {
      HeuristicLab.Hive.Contracts.BusinessObjects.Job jobObj = new HeuristicLab.Hive.Contracts.BusinessObjects.Job();

      MemoryStream memStream = new MemoryStream();
      GZipStream stream = new GZipStream(memStream, CompressionMode.Compress, true);
      XmlDocument document = PersistenceManager.CreateXmlDocument();
      Dictionary<Guid, IStorable> dictionary = new Dictionary<Guid, IStorable>();
      XmlNode rootNode = document.CreateElement("Root");
      document.AppendChild(rootNode);
      rootNode.AppendChild(PersistenceManager.Persist(job, document, dictionary));
      document.Save(stream);
      stream.Close();
      jobObj.SerializedJob = memStream.ToArray();

      DiscoveryService service = new DiscoveryService();
      List<PluginInfo> plugins = new List<PluginInfo>();

      foreach (IStorable storeable in dictionary.Values) {
        PluginInfo pluginInfo = service.GetDeclaringPlugin(storeable.GetType());
        if (!plugins.Contains(pluginInfo)) {
          plugins.Add(pluginInfo);
          foreach (var dependency in pluginInfo.Dependencies) {
            if (!plugins.Contains(dependency)) plugins.Add(dependency);
          }
        }
      }

      List<HivePluginInfo> pluginsNeeded =
        new List<HivePluginInfo>();
      foreach (PluginInfo uniquePlugin in plugins) {
        HivePluginInfo pluginInfo =
          new HivePluginInfo();
        pluginInfo.Name = uniquePlugin.Name;
        pluginInfo.Version = uniquePlugin.Version.ToString();
        pluginInfo.BuildDate = uniquePlugin.BuildDate;
        pluginsNeeded.Add(pluginInfo);
      }

      jobObj.CoresNeeded = 1;
      jobObj.PluginsNeeded = pluginsNeeded;
      jobObj.State = HeuristicLab.Hive.Contracts.BusinessObjects.State.offline;
      return jobObj;
    }

    public event EventHandler Initialized;
    /// <summary>
    /// Fires a new <c>Initialized</c> event.
    /// </summary>
    protected virtual void OnInitialized() {
      if (Initialized != null)
        Initialized(this, new EventArgs());
    }

    public event EventHandler<OperationEventArgs> OperationExecuted;
    /// <summary>
    /// Fires a new <c>OperationExecuted</c> event.
    /// </summary>
    /// <param name="operation">The operation that has been executed.</param>
    protected virtual void OnOperationExecuted(IOperation operation) {
      if (OperationExecuted != null)
        OperationExecuted(this, new OperationEventArgs(operation));
    }

    public event EventHandler<ExceptionEventArgs> ExceptionOccurred;
    /// <summary>
    /// Aborts the execution and fires a new <c>ExceptionOccurred</c> event.
    /// </summary>
    /// <param name="exception">The exception that was thrown.</param>
    protected virtual void OnExceptionOccurred(Exception exception) {
      Abort();
      if (ExceptionOccurred != null)
        ExceptionOccurred(this, new ExceptionEventArgs(exception));
    }

    public event EventHandler ExecutionTimeChanged;
    /// <summary>
    /// Fires a new <c>ExecutionTimeChanged</c> event.
    /// </summary>
    protected virtual void OnExecutionTimeChanged() {
      if (ExecutionTimeChanged != null)
        ExecutionTimeChanged(this, new EventArgs());
    }

    public event EventHandler Finished;
    /// <summary>
    /// Fires a new <c>Finished</c> event.
    /// </summary>
    protected virtual void OnFinished() {
      if (Finished != null)
        Finished(this, new EventArgs());
    }

    #endregion

    public override IView CreateView() {
      return new HiveEngineEditor(this);
    }

    #region IEditable Members

    public IEditor CreateEditor() {
      return new HiveEngineEditor(this);
    }
    #endregion

    public override System.Xml.XmlNode GetXmlNode(string name, System.Xml.XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlAttribute attr = document.CreateAttribute("HiveServerUrl");
      attr.Value = HiveServerUrl;
      node.Attributes.Append(attr);
      node.AppendChild(PersistenceManager.Persist("Job", job, document, persistedObjects));
      return node;
    }

    public override void Populate(System.Xml.XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      HiveServerUrl = node.Attributes["HiveServerUrl"].Value;
      job = (Job)PersistenceManager.Restore(node.SelectSingleNode("Job"), restoredObjects);
    }
  }
}
