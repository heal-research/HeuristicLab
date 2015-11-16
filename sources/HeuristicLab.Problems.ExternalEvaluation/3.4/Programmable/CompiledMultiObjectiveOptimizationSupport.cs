using System;
using System.Linq;
using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;

namespace HeuristicLab.Problems.ExternalEvaluation {
  public class CompiledMultiObjectiveOptimizationSupport : CompiledOptimizationSupport, IMultiObjectiveOptimizationSupport {

    public void Analyze(Individual[] individuals, double[][] qualities, ResultCollection results, IRandom random) {
      // Use vars.yourVariable to access variables in the variable store i.e. yourVariable
      // Write or update results given the range of vectors and resulting qualities
      // Uncomment the following lines if you want to retrieve the best individual
      //var bestIndex = Maximization ?
      //         qualities.Select((v, i) => Tuple.Create(i, v)).OrderByDescending(x => x.Item2).First().Item1
      //       : qualities.Select((v, i) => Tuple.Create(i, v)).OrderBy(x => x.Item2).First().Item1;
      //var best = individuals[bestIndex];
    }

    // Implement further classes and methods
  }
}
