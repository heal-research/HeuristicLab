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
using System.Threading;
using HeuristicLab.Hive.Contracts.Interfaces;
using HeuristicLab.Hive.Contracts;
using System.IO;
using System.IO.Compression;
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Hive.Contracts.BusinessObjects;

namespace HeuristicLab.Grid.HiveBridge {
  public class HiveGridServerWrapper : IGridServer {
    private string address;

    public HiveGridServerWrapper(string address) {
      this.address = address;
    }

    public JobState JobState(Guid guid) {
      IExecutionEngineFacade executionEngineFacade = ServiceLocator.CreateExecutionEngineFacade(address);
      ResponseObject<JobResult> response = executionEngineFacade.GetLastResult(guid, false);
      if (response.Success == true &&
        (response.StatusMessage == ApplicationConstants.RESPONSE_JOB_RESULT_NOT_YET_HERE ||
          response.StatusMessage == ApplicationConstants.RESPONSE_JOB_REQUEST_SET ||
          response.StatusMessage == ApplicationConstants.RESPONSE_JOB_REQUEST_ALLREADY_SET ||
          response.StatusMessage == ApplicationConstants.RESPONSE_JOB_JOB_RESULT_SENT)) {
          return HeuristicLab.Grid.JobState.Busy;
      } else return HeuristicLab.Grid.JobState.Unknown;
    }

    public Guid BeginExecuteEngine(byte[] engine) {
      var jobObj = CreateJobObj(engine);

      IExecutionEngineFacade executionEngineFacade = ServiceLocator.CreateExecutionEngineFacade(address);
      ResponseObject<HeuristicLab.Hive.Contracts.BusinessObjects.Job> res = executionEngineFacade.AddJob(jobObj);
      return res.Obj.Id;
    }

    public byte[] TryEndExecuteEngine(Guid guid) {
      IExecutionEngineFacade executionEngineFacade = ServiceLocator.CreateExecutionEngineFacade(address);
      ResponseObject<JobResult> response = executionEngineFacade.GetLastResult(guid, false);
      if (response.Success && response.Obj != null) {
        HeuristicLab.Hive.Engine.Job restoredJob = (HeuristicLab.Hive.Engine.Job)PersistenceManager.RestoreFromGZip(response.Obj.Result);
        // Serialize the engine
        MemoryStream memStream = new MemoryStream();
        GZipStream stream = new GZipStream(memStream, CompressionMode.Compress, true);
        XmlDocument document = PersistenceManager.CreateXmlDocument();
        Dictionary<Guid, IStorable> dictionary = new Dictionary<Guid, IStorable>();
        XmlNode rootNode = document.CreateElement("Root");
        document.AppendChild(rootNode);
        rootNode.AppendChild(PersistenceManager.Persist(restoredJob.Engine, document, dictionary));
        document.Save(stream);
        stream.Close();
        return memStream.ToArray();
      } else return null;
    }

    private HeuristicLab.Hive.Contracts.BusinessObjects.Job CreateJobObj(byte[] serializedEngine) {
      HeuristicLab.Hive.Contracts.BusinessObjects.Job jobObj = new HeuristicLab.Hive.Contracts.BusinessObjects.Job();

      List<HivePluginInfo> requiredPlugins = new List<HivePluginInfo>();
      IEngine engine = RestoreEngine(serializedEngine, requiredPlugins);

      HeuristicLab.Hive.Engine.Job job = new HeuristicLab.Hive.Engine.Job();
      job.Engine.OperatorGraph.AddOperator(engine.OperatorGraph.InitialOperator);
      job.Engine.OperatorGraph.InitialOperator = engine.OperatorGraph.InitialOperator;

      // Serialize the job
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
      jobObj.CoresNeeded = 1;
      jobObj.PluginsNeeded = requiredPlugins;
      jobObj.State = HeuristicLab.Hive.Contracts.BusinessObjects.State.offline;
      return jobObj;
    }

    private IEngine RestoreEngine(byte[] serializedEngine, List<HivePluginInfo> requiredPlugins) {
      // unzip and restore to determine the list of required plugins (NB: inefficient!)
      MemoryStream memStream = new MemoryStream(serializedEngine);
      GZipStream stream = new GZipStream(memStream, CompressionMode.Decompress, true);
      XmlDocument document = new XmlDocument();
      document.Load(stream);

      Dictionary<Guid, IStorable> dictionary = new Dictionary<Guid, IStorable>();
      XmlNode rootNode = document.ChildNodes[1].ChildNodes[0];
      IEngine engine = (IEngine)PersistenceManager.Restore(rootNode, dictionary);
      stream.Close();

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

      foreach (PluginInfo uniquePlugin in plugins) {
        HivePluginInfo pluginInfo =
          new HivePluginInfo();
        pluginInfo.Name = uniquePlugin.Name;
        pluginInfo.Version = uniquePlugin.Version.ToString();
        pluginInfo.BuildDate = uniquePlugin.BuildDate;
        requiredPlugins.Add(pluginInfo);
      }
      return engine;
    }
  }
}
