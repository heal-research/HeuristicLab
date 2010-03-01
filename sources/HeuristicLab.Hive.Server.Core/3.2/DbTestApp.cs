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
using HeuristicLab.PluginInfrastructure;
using System.Net;
using HeuristicLab.Hive.Contracts;
using HeuristicLab.Hive.Contracts.Interfaces;
using HeuristicLab.Hive.Server.DataAccess;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using System.Diagnostics;
using HeuristicLab.DataAccess.Interfaces;
using System.IO;
using HeuristicLab.Hive.Server.Core;
using HeuristicLab.Core;
using HeuristicLab.Hive.Server.LINQDataAccess;

namespace HeuristicLab.Hive.Server {
  [Application("Hive DB Test App", "Test Application for the Hive DataAccess Layer")]
  class HiveDbTestApplication : ApplicationBase {
    /*  private void TestClientAdapter() {
        IClientAdapter clientAdapter =
          ServiceLocator.GetClientAdapter();

        ClientInfo client = new ClientInfo();
        client.Login = DateTime.Now;
        clientAdapter.Update(client);

        ClientInfo clientRead =
          clientAdapter.GetById(client.Id);
        Debug.Assert(
          clientRead != null &&
          client.Id == clientRead.Id);

        client.CpuSpeedPerCore = 2000;
        clientAdapter.Update(client);
        clientRead =
          clientAdapter.GetById(client.Id);
        Debug.Assert(
         clientRead != null &&
         client.Id == clientRead.Id &&
         clientRead.CpuSpeedPerCore == 2000);

        ICollection<ClientInfo> clients = 
          clientAdapter.GetAll();
        int count = clients.Count;

        clientAdapter.Delete(client);

        clients = clientAdapter.GetAll();
        Debug.Assert(count - 1 == clients.Count);
      } 

      private void TestClientGroupAdapter() {
        ISessionFactory factory =
          ServiceLocator.GetSessionFactory();

        ISession session =
          factory.GetSessionForCurrentThread();

        ITransaction trans = null;

        try {
          IClientGroupAdapter clientGroupAdapter =
          session.GetDataAdapter<ClientGroup, IClientGroupAdapter>();

          trans =
            session.BeginTransaction();

          ClientInfo client =
            new ClientInfo();
          client.Name = "Stefan";
          client.Login = DateTime.Now;

          ClientInfo client2 =
            new ClientInfo();
          client2.Name = "Martin";
          client2.Login = DateTime.Now;

          ClientInfo client3 =
            new ClientInfo();
          client3.Name = "Heinz";
          client3.Login = DateTime.Now;

          ClientGroup group =
            new ClientGroup();

          ClientGroup subGroup =
            new ClientGroup();
          subGroup.Resources.Add(client);

          group.Resources.Add(client3);
          group.Resources.Add(client2);
          group.Resources.Add(subGroup);

          clientGroupAdapter.Update(group);

          ClientGroup read =
            clientGroupAdapter.GetById(group.Id);

          ICollection<ClientGroup> clientGroups =
            clientGroupAdapter.GetAll();

          IClientAdapter clientAdapter =
            session.GetDataAdapter<ClientInfo, IClientAdapter>();

          clientAdapter.Delete(client3);

          read =
             clientGroupAdapter.GetById(group.Id);

          clientGroupAdapter.Delete(subGroup);

          read =
             clientGroupAdapter.GetById(group.Id);

          clientGroups =
            clientGroupAdapter.GetAll();

          clientGroupAdapter.Delete(group);

          clientGroups =
            clientGroupAdapter.GetAll();

          clientAdapter.Delete(client);
          clientAdapter.Delete(client2);
        }
        finally {
          if (trans != null)
            trans.Rollback();

          session.EndSession();
        }
      }

      private void InsertTestClientGroups() {
        ISessionFactory factory =
          ServiceLocator.GetSessionFactory();

        ISession session =
          factory.GetSessionForCurrentThread();

        ITransaction trans = null;

        try {
          IClientGroupAdapter clientGroupAdapter =
          session.GetDataAdapter<ClientGroup, IClientGroupAdapter>();

          trans =
            session.BeginTransaction();

          ClientInfo client =
            new ClientInfo();
          client.Name = "Stefan";
          client.Login = DateTime.Now;

          ClientInfo client2 =
            new ClientInfo();
          client2.Name = "Martin";
          client2.Login = DateTime.Now;

          ClientGroup group =
            new ClientGroup();
          group.Name = "Gruppe1";

          ClientGroup subGroup =
            new ClientGroup();
          subGroup.Name = "Untergruppe1";
          subGroup.Resources.Add(client);

          group.Resources.Add(client2);
          group.Resources.Add(subGroup);

          clientGroupAdapter.Update(group);

          trans.Commit();
        }
        finally {
          session.EndSession();
        }
      }

      private void TestJobAdapter() {
        IJobAdapter jobAdapter = 
          ServiceLocator.GetJobAdapter();
        IClientAdapter clientAdapter =
          ServiceLocator.GetClientAdapter();

        Job job = new Job();

        ClientInfo client = new ClientInfo();
        client.Login = DateTime.Now;

        job.Client = client;
        jobAdapter.Update(job);

        ICollection<Job> jobs = jobAdapter.GetAll();

        jobAdapter.Delete(job);
        clientAdapter.Delete(client);

        jobs = jobAdapter.GetAll();
      }

      private void TestJobResultsAdapter() {
        Job job = new Job();

        ClientInfo client = new ClientInfo();
        client.Login = DateTime.Now;

        job.Client = client;

        IJobResultsAdapter resultsAdapter = 
          ServiceLocator.GetJobResultsAdapter();

        byte[] resultByte = {0x0f, 0x1f, 0x2f, 0x3f, 0x4f};

        JobResult result = new JobResult();
        result.Client = client;
        result.Job = job;
        result.Result = resultByte;

        resultsAdapter.Update(result);

        JobResult read =
          resultsAdapter.GetById(result.Id);
        Debug.Assert(
          read.Id == result.Id &&
          result.Client.Id == read.Client.Id &&
          result.Job.Id == read.Job.Id &&
          result.Result == result.Result);

        int count =
          resultsAdapter.GetAll().Count;

        resultsAdapter.Delete(result);

        ICollection<JobResult> allResults =
          resultsAdapter.GetAll();

        Debug.Assert(allResults.Count == count - 1);

        IJobAdapter jboAdapter =
          ServiceLocator.GetJobAdapter();
        jboAdapter.Delete(job);
        IClientAdapter clientAdapter =
          ServiceLocator.GetClientAdapter();
        clientAdapter.Delete(client);
      }      

      private void TestTransaction() {
        ISessionFactory factory =
          ServiceLocator.GetSessionFactory();

        ISession session =
          factory.GetSessionForCurrentThread();

        IClientAdapter clientAdapter =
          session.GetDataAdapter<ClientInfo, IClientAdapter>();

        ITransaction trans =
          session.BeginTransaction();

        ClientInfo client = new ClientInfo();
        client.Login = DateTime.Now;
        clientAdapter.Update(client);

        trans.Rollback();

        session.EndSession();
      }  */

    private void TestJobStreaming() {
      ISessionFactory factory =
         ServiceLocator.GetSessionFactory();

      ISession session =
           factory.GetSessionForCurrentThread();

      IJobAdapter jobAdapter =
        session.GetDataAdapter<HeuristicLab.Hive.Contracts.BusinessObjects.Job, IJobAdapter>();

      Stream s = jobAdapter.GetSerializedJobStream(
        new Guid("1b35f32b-d880-4c76-86af-4b4e283b30e6"), true);

      int length = 0;

      FileStream fs =
        new FileStream(@"W:\\result.gz", FileMode.Create);

      byte[] buffer = new byte[1024];
      while ((length = s.Read(buffer, 0, buffer.Length)) > 0) {
        fs.Write(buffer, 0, length);
      }

      fs.Close();
      s.Close();

      session.EndSession();
    }

    private void TestJobResultStreaming() {
      ISessionFactory factory =
         ServiceLocator.GetSessionFactory();

      ISession session =
           factory.GetSessionForCurrentThread();

      IJobResultsAdapter jobResultsAdapter =
        session.GetDataAdapter<JobResult, IJobResultsAdapter>();

      Stream s = jobResultsAdapter.GetSerializedJobResultStream(
        new Guid("c20b11a9-cde1-4d7f-8499-23dedb5a65ed"), true);

      int length = 0;

      FileStream fs =
        new FileStream(@"W:\\result.gz", FileMode.Create);

      byte[] buffer = new byte[1024];
      while ((length = s.Read(buffer, 0, buffer.Length)) > 0) {
        fs.Write(buffer, 0, length);
      }

      fs.Close();
      s.Close();

      session.EndSession();
    }

    private void TestJobResultDeserialization() {
      ExecutionEngineFacade executionEngineFacade =
        new ExecutionEngineFacade();

      ResponseObject<SerializedJobResult> response =
        executionEngineFacade.GetLastSerializedResult(
        new Guid("56ce20bc-067b-424d-a7df-67aaace7c850"), false);

      IStorable restoredJob =
        PersistenceManager.RestoreFromGZip(response.Obj.SerializedJobResultData);
    }

    private void TestLINQImplementation() {      
      ClientDao clientDao = new ClientDao();      
      ClientInfo info = new ClientInfo();
      info.Id = Guid.NewGuid();
      info.FreeMemory = 1000;
      info.Login = DateTime.Now;
      info.Memory = 1000;
      info.Name = "jackie";
      info.NrOfCores = 3;
      info.NrOfFreeCores = 2;
      info.CpuSpeedPerCore = 2500;
      info.State = State.idle;
      info = clientDao.Insert(info);

    }

    public override void Run() {
      //TestClientGroupAdapter();
      //InsertTestClientGroups();
      //TestJobStreaming();
      //TestJobResultStreaming();
      //TestJobResultDeserialization();

      TestLINQImplementation();

    }


  }
}
