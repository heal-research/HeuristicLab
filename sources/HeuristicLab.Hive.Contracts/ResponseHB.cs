using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HeuristicLab.Hive.Contracts {

  /// <summary>
  /// If a client sends a heartbeat the server can request sereral actions
  /// actions:
  ///  - jobReady: an new Job is ready to be calculated
  ///  - jobSnapshot: send a snapshot of the current status of the calculated job
  ///  - abortJobwithReturn: abort the job and return all existing results
  ///  - abortJobwithoutReturn: abort the job and don't send back anything
  ///  - nothingToDo: there is nothing to do at the moment
  /// </summary>
  public enum Action { jobReady, jobSnapshot, abortJobwithReturn, abortJobwithoutReturn, nothingToDo }

  /// <summary>
  /// Response Heartbeat class
  /// Return value to hearbeats sent by the client
  /// </summary>
  [DataContract]
  public class ResponseHB : Response {
    /// <summary>
    /// The server can send more than one actionRequest to the client
    /// So they are stored in one Map
    /// key [long]: JobId
    /// value [Action]: action
    /// </summary>
    [DataMember]
    public Dictionary<long, Action> ActionRequest { get; set; }
  }
}
