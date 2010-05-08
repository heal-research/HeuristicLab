using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Optimization;
using HeuristicLab.Core;
using HeuristicLab.Optimization.Interfaces;

namespace HeuristicLab.Problems.TestFunctions {
  public interface IRealVectorPSODecoder : IRealVectorDecoder {
    IParameter OriginalRealVectorParameter { get; }
  }
}
