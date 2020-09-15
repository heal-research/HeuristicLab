using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using ENCODING_NAMESPACE;

namespace HeuristicLab.Problems.ExternalEvaluation {
  public class CompiledSingleObjectiveOptimizationSupport : CompiledOptimizationSupport, ISingleObjectiveOptimizationSupport<SOLUTION_CLASS> {

    // private Result<StringValue> myResult;

    public void InitializeResults() {
      // Initialize the results that you intend to produce
      // Uncomment the field above and the code below to add to the problem's results
      // if (!Results.TryGetValue("My Result", out myResult))
      //   Results.Add(myResult = new Result<StringValue>("My Result", "My result description."));
    }

    public void Analyze(ISingleObjectiveSolutionContext<SOLUTION_CLASS>[] solutionContexts, IRandom random) {
      // Use vars.yourVariable to access variables in the variable store i.e. yourVariable
      // Write or update results given the range of vectors and resulting qualities
      // Uncomment the following lines if you want to retrieve the current best individual
      // Maximization:
      // var best = solutionContexts.OrderByDescending(x => x.EvaluationResult.Quality).First();
      // Minimization:
      // var best = solutionContexts.OrderBy(x => x.EvaluationResult.Quality).First();
      // 
      // myResult.Value = new StringValue("Custom result");
    }

    public IEnumerable<SOLUTION_CLASS> GetNeighbors(SOLUTION_CLASS solution, IRandom random) {
      // Use vars.yourVariable to access variables in the variable store i.e. yourVariable
      // Create new vectors, based on the given one that represent small changes
      // This method is only called from move-based algorithms (Local Search, Simulated Annealing, etc.)
      while (true) {
        // Algorithm will draw only a finite amount of samples
        // Change to a for-loop to return a concrete amount of neighbors
        var neighbor = (SOLUTION_CLASS)solution.Clone();
        // For instance, perform a single bit-flip in a binary parameter
        //var bIndex = random.Next(solution.Length);
        //neighbor[bIndex] = !neighbor[bIndex];
        yield return neighbor;
      }
    }

    // Implement further classes and methods
  }
}
