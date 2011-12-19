using System;

namespace HeuristicLab.Clients.Hive.SlaveCore {
  class JobAlreadyExistsException : Exception {
    public Guid JobId { get; set; }

    public JobAlreadyExistsException(Guid jobId) {
      this.JobId = jobId;
    }
  }
}
