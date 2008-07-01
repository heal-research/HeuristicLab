using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;

namespace HeuristicLab.CEDMA.Console {
  public interface IAgentList : IItem, IEnumerable<IAgent> {
    void Add(IAgent agent);
  }
}
