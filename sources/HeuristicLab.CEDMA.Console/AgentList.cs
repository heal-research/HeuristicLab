using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using System.Collections;
using HeuristicLab.CEDMA.DB.Interfaces;

namespace HeuristicLab.CEDMA.Console {
  public class AgentList: ItemBase, IAgentList {
    private string serverUri;
    private List<IAgent> agentList;
    private IDatabase database;
    public IDatabase Database {
      get { return database; }
      set {
        database = value;
        ReloadList();
      }
    }

    private void ReloadList() {
      agentList.Clear();
      foreach(HeuristicLab.CEDMA.DB.Interfaces.IAgent a in database.GetAgents()) {
        Agent newAgent = new Agent();
        newAgent.Name = a.Name;
        agentList.Add(newAgent);
      }
      FireChanged();
    }

    public AgentList()
      : base() {
      agentList = new List<IAgent>();
    }
    
    public void Add(IAgent agent) {
      agentList.Add(agent);
    }

    public IEnumerator<IAgent> GetEnumerator() {
      return agentList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }

    public override IView CreateView() {
      return new AgentListView(this);
    }
  }
}
