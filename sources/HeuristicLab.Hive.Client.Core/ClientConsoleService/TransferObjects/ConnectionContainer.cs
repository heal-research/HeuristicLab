using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HeuristicLab.Hive.Client.Core.ClientConsoleService {
  [DataContract]
  public class ConnectionContainer {
    [DataMember]
    public string IPAdress { get; set; }
    [DataMember]
    public int Port { get; set; }
  }
}
