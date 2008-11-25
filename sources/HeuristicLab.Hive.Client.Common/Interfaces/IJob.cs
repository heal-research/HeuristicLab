using System;
using HeuristicLab.Core;
namespace HeuristicLab.Hive.Client.Common {
  public interface IJob: IStorable {
    event EventHandler JobStopped;
    long JobId { get; set; }
    double Progress { get; set; }
    void Run();
    bool Running { get; set; }
    void Start();
    void Stop();
  }
}
