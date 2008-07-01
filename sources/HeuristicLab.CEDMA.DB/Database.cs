using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
using HeuristicLab.CEDMA.DB.Interfaces;
using System.ServiceModel;

namespace HeuristicLab.CEDMA.DB {
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
  public class Database : DataContext, IDatabase {
    Table<Agent> Agents;

    public Database() : this("c:\\cedma.mdf") { }

    public Database(string connection) : base(connection) { 
    }

    public IList<IAgent> GetAgents() {
      List<IAgent> result = new List<IAgent>();
      foreach(Agent a in Agents) {
        result.Add(a);
      }
      return result;
    }

    public IAgent CreateAgent() {
      Agent newAgent = new Agent();
      newAgent.Name = DateTime.Now.ToString();
      newAgent.Status = AgentStatus.Waiting;
      newAgent.RawData = null;
      Agents.InsertOnSubmit(newAgent);
      this.SubmitChanges();
      return newAgent;
    }
  }
}