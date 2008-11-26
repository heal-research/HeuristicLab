using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using System.Runtime.Serialization;

namespace HeuristicLab.Hive.Contracts {

  /// <summary>
  /// Response Job class
  /// If a client pulls a Job from the server he gets a ResponseJob as answer
  /// </summary>
  [DataContract]
  public class ResponseJob : Response {
    [DataMember]
    public long JobId { get; set; }
    [DataMember]
    public byte[] SerializedJob { get; set; }
  }
}
