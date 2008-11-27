using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.Hive.Client.Communication.ClientConsole {
  [Serializable]
  public class ConnectionContainer {
    public string IPAdress { get; set; }
    public int Port { get; set; }
  }
}
