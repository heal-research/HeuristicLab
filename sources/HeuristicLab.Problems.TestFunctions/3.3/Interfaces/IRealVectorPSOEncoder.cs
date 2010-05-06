using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Optimization;
using HeuristicLab.Core;

namespace HeuristicLab.Problems.TestFunctions {
  public interface IRealVectorPSOEncoder : IRealVectorEncoder {
    IParameter OriginalRealVectorParameter { get; }
  }
}
