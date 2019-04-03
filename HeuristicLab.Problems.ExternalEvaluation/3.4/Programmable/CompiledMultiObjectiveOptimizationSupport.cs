using HeuristicLab.Core;
using HeuristicLab.Optimization;

namespace HeuristicLab.Problems.ExternalEvaluation {
  public class CompiledMultiObjectiveOptimizationSupport : CompiledOptimizationSupport, IMultiObjectiveOptimizationSupport {

    public void Analyze(IEncodedSolution[] individuals, double[][] qualities, ResultCollection results, IRandom random) {
      // Use vars.yourVariable to access variables in the variable store i.e. yourVariable
      // Write or update results given the range of vectors and resulting qualities
    }

    // Implement further classes and methods
  }
}
