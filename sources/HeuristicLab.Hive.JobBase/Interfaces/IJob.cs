using System;
using HeuristicLab.Core;
namespace HeuristicLab.Hive.JobBase {
  public interface IJob: IStorable {
    event EventHandler JobStopped;
    long JobId { get; set; }
    double Progress { get; }
    void Run();
    bool Running { get; set; }
    void Start();
    void Stop();
  }
}
