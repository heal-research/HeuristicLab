using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;

namespace HeuristicLab.CEDMA.Console {
  public interface IAgent : IItem {
    string Name { get; }
    IOperatorGraph OperatorGraph { get; }
  }
}
