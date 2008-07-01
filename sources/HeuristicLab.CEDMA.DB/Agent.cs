using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq.Mapping;
using HeuristicLab.CEDMA.DB.Interfaces;

namespace HeuristicLab.CEDMA.DB {
  [Table(Name = "AgentTable")]
  class Agent : IAgent {
    [Column(IsPrimaryKey = true)]
    public int ID;
    [Column]
    public string Name;
    [Column]
    public AgentStatus Status;
    [Column]
    public byte[] RawData;

    string IAgent.Name {
      get { return this.Name; }
    }

    AgentStatus IAgent.Status {
      get { return this.Status; }
    }

    byte[] IAgent.RawData {
      get { return this.RawData; }
    }

    public void Save() {
      throw new NotImplementedException();
    }
  }
}
