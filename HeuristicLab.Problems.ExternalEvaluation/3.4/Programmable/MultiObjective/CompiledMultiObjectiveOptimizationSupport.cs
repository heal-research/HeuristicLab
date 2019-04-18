using HeuristicLab.Core;
using HeuristicLab.Optimization;
using ENCODING_NAMESPACE;

namespace HeuristicLab.Problems.ExternalEvaluation {
  public class CompiledMultiObjectiveOptimizationSupport : CompiledOptimizationSupport, IMultiObjectiveOptimizationSupport<SOLUTION_CLASS> {

    public void Analyze(SOLUTION_CLASS[] solutions, double[][] qualities, ResultCollection results, IRandom random) {
      // Use vars.yourVariable to access variables in the variable store i.e. yourVariable
      // Write or update results given the range of vectors and resulting qualities
    }

    // Implement further classes and methods
  }
}
