using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using System.Data.Linq;
using HeuristicLab.Hive.Server.DataAccess;
using System.IO;
using System.Data.SqlClient;

namespace HeuristicLab.Hive.Server.LINQDataAccess {
  public class JobDao: BaseDao<JobDto, Job>, IJobDao {
   
    #region IGenericDao<JobDto,Job> Members

    public JobDto FindById(Guid id) {
      return (from job in Context.Jobs
              where job.JobId.Equals(id)
              select EntityToDto(job, null)).SingleOrDefault();
    }

    public IEnumerable<JobDto> FindAll() {
      return (from job in Context.Jobs
              select EntityToDto(job, null)).ToList();
    }

    public byte[] GetBinaryJobFile(Guid jobId) {
      return (from job in Context.Jobs
              where job.JobId.Equals(jobId)
              select job.SerializedJob).SingleOrDefault().ToArray();
    }

    public JobDto Insert(JobDto bObj) {
      Job j = DtoToEntity(bObj, null);
      Context.Jobs.InsertOnSubmit(j);
      Context.SubmitChanges();
      bObj.Id = j.JobId;
      return bObj;
    }

    public SerializedJob InsertWithAttachedJob(SerializedJob job) {
      Job j = DtoToEntity(job.JobInfo, null);
      j.SerializedJob = job.SerializedJobData;
      Context.Jobs.InsertOnSubmit(j);
      Context.SubmitChanges();
      job.JobInfo.Id = j.JobId;
      return job;

    }

    public void Delete(JobDto bObj) {
      Job job = Context.Jobs.SingleOrDefault(j => j.JobId.Equals(bObj.Id));
      Context.Jobs.DeleteOnSubmit(job);
      Context.SubmitChanges();
    }

    public void Update(JobDto bObj) {
      Job job = Context.Jobs.SingleOrDefault(j => j.JobId.Equals(bObj.Id));
      DtoToEntity(bObj, job);
      Context.SubmitChanges();
    }

    public IEnumerable<JobDto> FindActiveJobsOfClient(ClientDto client) {
      return (from j in Context.Jobs
              where (j.JobState == Enum.GetName(typeof (State), State.calculating) ||
                     j.JobState == Enum.GetName(typeof (State), State.abort) ||
                     j.JobState == Enum.GetName(typeof (State), State.requestSnapshot) ||
                     j.JobState == Enum.GetName(typeof (State), State.requestSnapshotSent)) &&
                    (j.ResourceId.Equals(client.Id))
              select EntityToDto(j, null)).ToList();
    }
    public IEnumerable<JobDto> FindFittingJobsForClient(State state, int freeCores, int freeMemory) {
      return (from j in Context.Jobs
              where j.JobState == Enum.GetName(typeof (State), State.offline) &&
                    j.CoresNeeded <= freeCores &&
                    j.MemoryNeeded <= freeMemory
              select EntityToDto(j, null)).ToList();
      
    }

    public IEnumerable<JobDto> GetJobsByState(State state) {
      return (from j in Context.Jobs
              where (j.JobState == Enum.GetName(typeof (State), state))
              select EntityToDto(j, null)).ToList();
    }

    public void AssignClientToJob(Guid clientId, Guid jobId) {
      Client c = Context.Clients.SingleOrDefault(client => client.ResourceId.Equals(clientId));
      Job j = Context.Jobs.SingleOrDefault(job => job.JobId.Equals(jobId));
      c.Jobs.Add(j);
      j.Client = c;
      Context.SubmitChanges();      
    }

    public void SetJobOffline(JobDto job) {
      Job j = Context.Jobs.SingleOrDefault(jq => jq.JobId.Equals(job.Id));
      j.Client = null;
      j.JobState = Enum.GetName(typeof(State), State.offline); 
      Context.SubmitChanges();
    }

    public Stream GetSerializedJobStream(Guid jobId) {
      HiveDataContext hdc = new HiveDataContext();
      hdc.Transaction = null;

      VarBinarySource source =
        new VarBinarySource(
          hdc.Connection as SqlConnection, null,
          "Job", "SerializedJob", "JobId", jobId);

      return new VarBinaryStream(source);
    }

    #endregion

    public override Job DtoToEntity(JobDto source, Job target) {
      if (source == null)
        return null;
      if (target == null)
        target = new Job();

      target.CoresNeeded = source.CoresNeeded;
      target.MemoryNeeded = source.MemoryNeeded;

      target.DateCalculated = source.DateCalculated;
      target.DateCreated = source.DateCreated;
      target.JobId = source.Id;

      target.Percentage = source.Percentage;

      target.Priority = source.Priority;
      target.JobState = Enum.GetName(typeof(State), source.State);
      return target;
    }

    //Assigned ressources are not used atm!
    //Client is not used ATM - not sure when we stopped using those...
    public override JobDto EntityToDto(Job source, JobDto target) {
      if (source == null)
        return null;
      if(target == null)
        target = new JobDto();
    
      //target.ParentJob = null;
      //target.PluginsNeeded = null;
      //target.Client = null;
      //target.Project = null;
      
      target.CoresNeeded = source.CoresNeeded;
      target.MemoryNeeded = source.MemoryNeeded;



      target.DateCalculated = source.DateCalculated;
      target.DateCreated = source.DateCreated;
      target.Id = source.JobId;
      
          
      target.Percentage = source.Percentage;
      
      target.Priority = source.Priority;
      target.State = (State) Enum.Parse(typeof (State), source.JobState);
      return target;
    }
  }
}
