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
using System.ServiceModel;
using HeuristicLab.Tracing;

namespace HeuristicLab.Grid.HiveBridge {
  public class HiveGridServerWrapper : IGridServer {
    private const int MAX_CONNECTION_RETRIES = 10;
    private const int RETRY_TIMEOUT_SEC = 60;
    private string address;
    private IExecutionEngineFacade executionEngine;
    private object connectionLock = new object();

    public HiveGridServerWrapper(string address) {
      this.address = address;
    }

    public JobState JobState(Guid guid) {
      ResponseObject<SerializedJob> response = SavelyExecute(() => executionEngine.GetLastSerializedResult(guid, false));
      if (response != null) {
        return HeuristicLab.Grid.JobState.Busy;
      } else return HeuristicLab.Grid.JobState.Unknown;
    }

    public Guid BeginExecuteEngine(byte[] engine) {
      var jobObj = CreateJobObj(engine);

      ResponseObject<HeuristicLab.Hive.Contracts.BusinessObjects.JobDto> res = SavelyExecute(() => executionEngine.AddJob(jobObj));
      return res == null ? Guid.Empty : res.Obj.Id;
    }

    public byte[] TryEndExecuteEngine(Guid guid) {
      ResponseObject<SerializedJob> response = SavelyExecute(() => executionEngine.GetLastSerializedResult(guid, false));
      if (response != null &&
        response.Success && response.Obj != null) {
        HeuristicLab.Hive.Engine.Job restoredJob = (HeuristicLab.Hive.Engine.Job)PersistenceManager.RestoreFromGZip(response.Obj.SerializedJobData);
        // only return the engine when it wasn't canceled (result is only a snapshot)
        if (restoredJob.Progress == 1.0) {
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
        }
      }

      return null;
    }

    private HeuristicLab.Hive.Contracts.BusinessObjects.SerializedJob CreateJobObj(byte[] serializedEngine) {
      HeuristicLab.Hive.Contracts.BusinessObjects.JobDto jobObj = new HeuristicLab.Hive.Contracts.BusinessObjects.JobDto();

      List<HivePluginInfoDto> requiredPlugins = new List<HivePluginInfoDto>();
      IEngine engine = RestoreEngine(serializedEngine, requiredPlugins);

      HeuristicLab.Hive.Engine.Job job = new HeuristicLab.Hive.Engine.Job();
      job.Engine.OperatorGraph.AddOperator(engine.OperatorGraph.InitialOperator);
      job.Engine.OperatorGraph.InitialOperator = engine.OperatorGraph.InitialOperator;
      job.Engine.Reset();

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

      SerializedJob computableJob =
        new SerializedJob();
      computableJob.SerializedJobData = memStream.ToArray();
      jobObj.CoresNeeded = 1;
      jobObj.PluginsNeeded = requiredPlugins;
      jobObj.State = HeuristicLab.Hive.Contracts.BusinessObjects.State.offline;

      computableJob.JobInfo = jobObj;

      return computableJob;
    }

    private IEngine RestoreEngine(byte[] serializedEngine, List<HivePluginInfoDto> requiredPlugins) {
      // unzip and restore to determine the list of required plugins (NB: inefficient!)
      MemoryStream memStream = new MemoryStream(serializedEngine);
      GZipStream stream = new GZipStream(memStream, CompressionMode.Decompress, true);
      XmlDocument document = new XmlDocument();
      document.Load(stream);

      Dictionary<Guid, IStorable> dictionary = new Dictionary<Guid, IStorable>();
      XmlNode rootNode = document.ChildNodes[1].ChildNodes[0];
      IEngine engine = (IEngine)PersistenceManager.Restore(rootNode, dictionary);
      stream.Close();

      List<IPluginDescription> plugins = new List<IPluginDescription>();

      foreach (IStorable storeable in dictionary.Values) {
        IPluginDescription pluginInfo = ApplicationManager.Manager.GetDeclaringPlugin(storeable.GetType());
        if (!plugins.Contains(pluginInfo)) {
          plugins.Add(pluginInfo);
          foreach (var dependency in pluginInfo.Dependencies) {
            if (!plugins.Contains(dependency)) plugins.Add(dependency);
          }
        }
      }

      foreach (IPluginDescription uniquePlugin in plugins) {
        HivePluginInfoDto pluginInfo =
          new HivePluginInfoDto();
        pluginInfo.Name = uniquePlugin.Name;
        pluginInfo.Version = uniquePlugin.Version.ToString();
        pluginInfo.BuildDate = uniquePlugin.BuildDate;
        requiredPlugins.Add(pluginInfo);
      }
      return engine;
    }

    private TResult SavelyExecute<TResult>(Func<TResult> a) where TResult : Response {
      int retries = 0;
      if (executionEngine == null)
        executionEngine = ServiceLocator.CreateExecutionEngineFacade(address);

      do {
        try {
          lock (connectionLock) {
            return a();
          }
        }
        catch (TimeoutException) {
          retries++;
          Thread.Sleep(TimeSpan.FromSeconds(RETRY_TIMEOUT_SEC));
        }
        catch (CommunicationException) {
          executionEngine = ServiceLocator.CreateExecutionEngineFacade(address);
          retries++;
          Thread.Sleep(TimeSpan.FromSeconds(RETRY_TIMEOUT_SEC));
        }
      } while (retries < MAX_CONNECTION_RETRIES);
      Logger.Warn("Reached max connection retries");
      return null;
    }
  }
}
