using System;
namespace HeuristicLab.Hive.Client.Common {
  public interface IJob {
    System.Xml.XmlNode GetXmlNode();
    event EventHandler JobStopped;
    long JobId { get; set; }
    int Progress { get; set; }
    void Run();
    bool Running { get; set; }
    void Start();
    void Stop();
  }
}
