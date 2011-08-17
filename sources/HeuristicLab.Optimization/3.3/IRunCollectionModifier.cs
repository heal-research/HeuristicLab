using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;

namespace HeuristicLab.Optimization {
  public interface IRunCollectionModifier : IItem {    
    void Modify(List<IRun> runs);
  }
}
