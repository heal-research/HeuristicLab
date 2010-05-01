using System;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using System.IO;

namespace HeuristicLab.Hive.Server.DataAccess {
  public interface IJobDao: IGenericDao<JobDto> {
    IEnumerable<JobDto> FindActiveJobsOfClient(ClientDto client);

    IEnumerable<JobDto> GetJobsByState(State state);

    void AssignClientToJob(Guid clientId, Guid jobId);

    void SetJobOffline(JobDto job);

    SerializedJob InsertWithAttachedJob(SerializedJob job);

    byte[] GetBinaryJobFile(Guid jobId); 

    IEnumerable<JobDto> FindFittingJobsForClient(State state, int freeCores, int freeMemory, Guid clientGuid);
    Stream GetSerializedJobStream(Guid jobId);

    IEnumerable<JobDto> FindWithLimitations(State jobState, int offset, int count);
  }
}
