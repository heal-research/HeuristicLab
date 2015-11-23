using System;
using System.Linq;
using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using {0}
using HeuristicLab.Optimization;
using HeuristicLab.Problems.Programmable;

namespace HeuristicLab.Problems.Programmable {
  public class CompiledSingleObjectiveProblemDefinition : CompiledProblemDefinition<{1}, {2}>, ISingleObjectiveProblemDefinition<{1}, {2}> {
    public bool Maximization { get { return false; } }

    public override void Initialize() {
      // Use vars.yourVariable to access variables in the variable store i.e. yourVariable
      // Define e.g. the length of the solution encoding or the solution creator by modifying the Encoding property
      // Add additional initialization code e.g. private variables that you need for evaluating
    }

    public double Evaluate({2} individual, IRandom random) {
      // Use vars.yourVariable to access variables in the variable store i.e. yourVariable
      var quality = 0.0;
      return quality;
    }

    public void Analyze({2}[] individuals, double[] qualities, ResultCollection results, IRandom random) {
      // Use vars.yourVariable to access variables in the variable store i.e. yourVariable
      // Write or update results given the range of vectors and resulting qualities
      // Uncomment the following lines if you want to retrieve the best individual

      //var orderedIndividuals = individuals.Zip(qualities, (i, q) => new { Individual = i, Quality = q }).OrderBy(z => z.Quality);
      //var best = Maximization ? orderedIndividuals.Last().Individual : orderedIndividuals.First().Individual;

      //if (!results.ContainsKey("Best Solution")) {
      //  results.Add(new Result("Best Solution", typeof({20)));
      //}
      //results["Best Solution"].Value = (IItem)best.Clone();
    }

    public IEnumerable<{2}> GetNeighbors({2} individual, IRandom random) {
      // Use vars.yourVariable to access variables in the variable store i.e. yourVariable
      // Create new vectors, based on the given one that represent small changes
      // This method is only called from move-based algorithms (Local Search, Simulated Annealing, etc.)
      while (true) {
        // Algorithm will draw only a finite amount of samples
        // Change to a for-loop to return a concrete amount of neighbors
        var neighbor = ({2})individual.Clone();
        // modify the solution specified as neighbor
        yield return neighbor;
      }
    }

    // Implement further classes and methods
  }
}

