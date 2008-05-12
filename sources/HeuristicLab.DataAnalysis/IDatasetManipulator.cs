using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.DataAnalysis {
  interface IDatasetManipulator {
    string Action { get; }
    void Execute(Dataset dataset);
  }
}
