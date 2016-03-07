using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.Algorithms.DataAnalysis.MctsSymbolicRegression.Policies {
  public interface IActionStatistics {
    double AverageQuality { get; }
    int Tries { get; }
    bool Done { get; set; }
  }
}
