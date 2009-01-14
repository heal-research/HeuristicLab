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
using HeuristicLab.Hive.Client.Common;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using HeuristicLab.Core;
using HeuristicLab.Hive.JobBase;
using HeuristicLab.Hive.Contracts;

namespace HeuristicLab.Hive.Client.ExecutionEngine {
  public class Executor: MarshalByRefObject {
    public long JobId { get; set; }
    public IJob Job { get; set; }
    public MessageContainer.MessageType CurrentMessage { get; set; }
    public MessageQueue Queue { get; set; }
    public bool Running {
      get {
        return Job.Running;
      }
    }
    public double Progress {
      get {
        return Job.Progress;
      }
    }

    public DateTime CreationTime { get; set; }

    public void Start(byte[] serializedJob) {
      CreationTime = DateTime.Now;
      //Job = (IJob)PersistenceManager.RestoreFromGZip(serializedJob);
      //debug
      Job = new TestJob();
      Job.JobStopped += new EventHandler(Job_JobStopped);
      Job.Start();
    }

    public void Abort() {
      CurrentMessage = MessageContainer.MessageType.AbortJob;
      Job.Stop();      
    }

    void Job_JobStopped(object sender, EventArgs e) {
      if (CurrentMessage == MessageContainer.MessageType.NoMessage)
        Queue.AddMessage(new MessageContainer(MessageContainer.MessageType.FinishedJob, JobId));
      else if (CurrentMessage == MessageContainer.MessageType.RequestSnapshot)
        Queue.AddMessage(new MessageContainer(MessageContainer.MessageType.SnapshotReady, JobId));
      else if (CurrentMessage == MessageContainer.MessageType.AbortJob)
        Queue.AddMessage(new MessageContainer(MessageContainer.MessageType.JobAborted, JobId));
    }

    public byte[] GetSnapshot() {
      //if the job is still running, something went VERY bad.
      if (Job.Running) {
        return null;
      } else {
        // Clear the Status message
        CurrentMessage = MessageContainer.MessageType.NoMessage;
        // Pack the whole job inside an xml document
        byte[] job = SerializeJobObject();        
        // Restart the job
        Job.Start();
        // Return the Snapshot
        return job;
      }
    }

    public byte[] GetFinishedJob() {
      //Job isn't finished!
      if (Job.Running) {
        return null;
      } else {
        return SerializeJobObject();
      }
    }


    public void RequestSnapshot() {
      CurrentMessage = MessageContainer.MessageType.RequestSnapshot;
      Job.Stop();
    }

    private byte[] SerializeJobObject() {
      return PersistenceManager.SaveToGZip(Job);
    }

    private void RestoreJobObject(byte[] sjob) {
      Job = (IJob)PersistenceManager.RestoreFromGZip(sjob);
    }

    public Executor() {
      CurrentMessage = MessageContainer.MessageType.NoMessage;
      Job = new TestJob();
    }    
  }
}
