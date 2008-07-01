using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using System.Xml;

namespace HeuristicLab.CEDMA.Console {
  public class Agent : ItemBase, IAgent {
    private string name;
    private OperatorGraph operatorGraph;
    public string Name {
      get { return name; }
      set { name = value; }
    }

    public IOperatorGraph OperatorGraph {
      get { return operatorGraph; }
    }

    public Agent() : base() {
      operatorGraph = new OperatorGraph();
    }
  }
}
