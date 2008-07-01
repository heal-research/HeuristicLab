using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.CEDMA.DB.Interfaces {
  public interface IAgent {
    string Name { get; }
    AgentStatus Status { get; }
    byte[] RawData { get; }
    void Save();
  }
}
