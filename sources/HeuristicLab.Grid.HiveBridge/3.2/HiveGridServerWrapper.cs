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
        Job restoredJob = (Job)PersistenceManager.RestoreFromGZip(response.Obj.Result);
        return restoredJob.SerializedJob;
      } else return null;
    }

    private HeuristicLab.Hive.Contracts.BusinessObjects.Job CreateJobObj(byte[] serializedEngine) {
      HeuristicLab.Hive.Contracts.BusinessObjects.Job jobObj = new HeuristicLab.Hive.Contracts.BusinessObjects.Job();

      // unzip and restore to determine the list of required plugins (NB: inefficient!)
      MemoryStream memStream = new MemoryStream();
      GZipStream stream = new GZipStream(memStream, CompressionMode.Decompress, true);
      XmlDocument document = new XmlDocument();
      document.Load(stream);

      Dictionary<Guid, IStorable> dictionary = new Dictionary<Guid, IStorable>();
      XmlNode rootNode = document.ChildNodes[0].ChildNodes[0];
      PersistenceManager.Restore(rootNode, dictionary);
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

      jobObj.SerializedJob = serializedEngine;
      jobObj.CoresNeeded = 1;
      jobObj.PluginsNeeded = pluginsNeeded;
      jobObj.State = HeuristicLab.Hive.Contracts.BusinessObjects.State.offline;
      return jobObj;
    }
  }
}
