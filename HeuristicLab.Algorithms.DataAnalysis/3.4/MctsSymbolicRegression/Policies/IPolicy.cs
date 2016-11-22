using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;

namespace HeuristicLab.Algorithms.DataAnalysis.MctsSymbolicRegression.Policies {
  public interface IPolicy : IItem {
    int Select(IEnumerable<IActionStatistics> actions, IRandom random);
    void Update(IActionStatistics action, double q);
    IActionStatistics CreateActionStatistics();
  }
}
