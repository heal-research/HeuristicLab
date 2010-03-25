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
using System.Linq;
using System.Text;
using HeuristicLab.Hive.Contracts.Interfaces;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using HeuristicLab.Hive.Contracts;
using HeuristicLab.PluginInfrastructure;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.ServiceModel;
using HeuristicLab.Hive.Server.Core.InternalInterfaces;
using System.Transactions;

namespace HeuristicLab.Hive.Server.Core {
  [ServiceBehavior(InstanceContextMode =
    InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class ClientFacade: IClientFacade {

    public ClientFacade() {      
    }

    private IClientCommunicator clientCommunicator =
      ServiceLocator.GetClientCommunicator();

    #region IClientCommunicator Members

    public Response Login(ClientDto clientInfo) {
      return clientCommunicator.Login(clientInfo);
    }

    public ResponseHB ProcessHeartBeat(HeartBeatData hbData) {
      return clientCommunicator.ProcessHeartBeat(hbData);
    }


    public ResponseJob SendJob(Guid clientId) {
      return clientCommunicator.SendJob(clientId);
    }

    /*public ResponseSerializedJob SendSerializedJob(Guid clientId) {
      return clientCommunicator.SendSerializedJob(clientId);
    } */

    public ResponseResultReceived StoreFinishedJobResult(Guid clientId,
      Guid jobId,
      byte[] result,
      double percentage, 
      Exception exception) {
      return clientCommunicator.StoreFinishedJobResult(clientId, jobId, result, percentage, exception);
    }

    public Response Logout(Guid clientId) {
      return clientCommunicator.Logout(clientId);
    }

    public Response IsJobStillNeeded(Guid jobId) {
      return clientCommunicator.IsJobStillNeeded(jobId);
    }

    public ResponsePlugin SendPlugins(List<HivePluginInfoDto> pluginList) {
      return clientCommunicator.SendPlugins(pluginList);
    }

    public ResponseResultReceived ProcessSnapshot(Guid clientId, Guid jobId, byte[] result, double percentage, Exception exception) {
      return clientCommunicator.ProcessSnapshot(clientId, jobId, result, percentage, exception);
    }


    public ResponseCalendar GetCalendar(Guid clientId) {
      return clientCommunicator.GetCalendar(clientId);
    }

    public Response SetCalendarStatus(Guid clientId, CalendarState state) {
      return clientCommunicator.SetCalendarStatus(clientId, state);      
    }
    #endregion

    #region IClientFacade Members

    public Stream SendStreamedJob(Guid clientId) {
      MultiStream stream =
        new MultiStream();

      ResponseJob job = null;

      job = this.SendJob(clientId);      

      //first send response
      stream.AddStream(
        new StreamedObject<ResponseJob>(job));

      IJobManager jobManager = 
        ServiceLocator.GetJobManager();

      //second stream the job binary data
      
      if(job.Job != null)
        stream.AddStream(
          ((IInternalJobManager) (jobManager)).
            GetJobStreamById(
            job.Job.Id));
      
      OperationContext clientContext = OperationContext.Current;
        clientContext.OperationCompleted += new EventHandler(delegate(object sender, EventArgs args) {
          if (stream != null) {
            stream.Dispose();
          }
        });

      return stream;
    }

    public Stream SendStreamedPlugins(List<HivePluginInfoDto> pluginList) {
      return
        new StreamedObject<ResponsePlugin>(
          this.SendPlugins(pluginList));
    }

    public ResponseResultReceived StoreFinishedJobResultStreamed(Stream stream) {
      return ((IInternalClientCommunicator)
        clientCommunicator).ProcessJobResult(stream, true);
    } 

    public ResponseResultReceived ProcessSnapshotStreamed(Stream stream) {
      return ((IInternalClientCommunicator)
        clientCommunicator).ProcessJobResult(stream, false);
    }



    #endregion
  }
}
