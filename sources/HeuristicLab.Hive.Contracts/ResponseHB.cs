using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HeuristicLab.Hive.Contracts {


  /// <summary>
  /// Response Heartbeat class
  /// Return value to hearbeats sent by the client
  /// </summary>
  [DataContract]
  public class ResponseHB : Response {
    [DataMember]
    public List<MessageContainer> ActionRequest { get; set; }
  }
}
