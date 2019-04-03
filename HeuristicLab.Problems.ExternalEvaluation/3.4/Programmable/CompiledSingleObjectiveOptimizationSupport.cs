using System.Collections.Generic;
using HeuristicLab.Core;
using HeuristicLab.Optimization;

namespace HeuristicLab.Problems.ExternalEvaluation {
  public class CompiledSingleObjectiveOptimizationSupport : CompiledOptimizationSupport, ISingleObjectiveOptimizationSupport {

    public void Analyze(IEncodedSolution[] individuals, double[] qualities, ResultCollection results, IRandom random) {
      // Use vars.yourVariable to access variables in the variable store i.e. yourVariable
      // Write or update results given the range of vectors and resulting qualities
      // Uncomment the following lines if you want to retrieve the best individual
      // Maximization:
      // var bestIndex = qualities.Select((v, i) => Tuple.Create(i, v)).OrderByDescending(x => x.Item2).First().Item1;
      // Minimization:
      // var bestIndex = qualities.Select((v, i) => Tuple.Create(i, v)).OrderBy(x => x.Item2).First().Item1;
      // var best = individuals[bestIndex];
    }

    public IEnumerable<IEncodedSolution> GetNeighbors(IEncodedSolution individual, IRandom random) {
      // Use vars.yourVariable to access variables in the variable store i.e. yourVariable
      // Create new vectors, based on the given one that represent small changes
      // This method is only called from move-based algorithms (Local Search, Simulated Annealing, etc.)
      while (true) {
        // Algorithm will draw only a finite amount of samples
        // Change to a for-loop to return a concrete amount of neighbors
        var neighbor = (IEncodedSolution)individual.Clone();
        // For instance, perform a single bit-flip in a binary parameter
        //var bIndex = random.Next(neighbor.BinaryVector("b").Length);
        //neighbor.BinaryVector("b")[bIndex] = !neighbor.BinaryVector("b")[bIndex];
        yield return neighbor;
      }
    }

    // Implement further classes and methods
  }
}
