using System;
using System.Linq;
using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using ENCODING_NAMESPACE;
using HeuristicLab.Optimization;
using HeuristicLab.Problems.Programmable;

namespace HeuristicLab.Problems.Programmable {
  public class CompiledSingleObjectiveProblemDefinition : CompiledSingleObjectiveProblemDefinition<ENCODING_CLASS, SOLUTION_CLASS> {
    public override bool Maximization { get { return false; } }

    public override void Initialize() {
      // Use vars.yourVariable to access variables in the variable store i.e. yourVariable
      // Define e.g. the length of the solution encoding or the solution creator by modifying the Encoding property
      // Add additional initialization code e.g. private variables that you need for evaluating
    }

    public override double Evaluate(SOLUTION_CLASS solution, IRandom random) {
      // Use vars.yourVariable to access variables in the variable store i.e. yourVariable
      var quality = 0.0;
      return quality;
    }

    public override void Analyze(SOLUTION_CLASS[] solution, double[] qualities, ResultCollection results, IRandom random) {
      // Use vars.yourVariable to access variables in the variable store i.e. yourVariable
      // Write or update results given the range of vectors and resulting qualities
      // Uncomment the following lines if you want to retrieve the best individual

      //var orderedIndividuals = individuals.Zip(qualities, (i, q) => new { Individual = i, Quality = q }).OrderBy(z => z.Quality);
      //var best = Maximization ? orderedIndividuals.Last().Individual : orderedIndividuals.First().Individual;

      //if (!results.ContainsKey("Best Solution")) {
      //  results.Add(new Result("Best Solution", typeof(SOLUTION_CLASS)));
      //}
      //results["Best Solution"].Value = (IItem)best.Clone();
    }

    public override IEnumerable<SOLUTION_CLASS> GetNeighbors(SOLUTION_CLASS individual, IRandom random) {
      // Use vars.yourVariable to access variables in the variable store i.e. yourVariable
      // Create new vectors, based on the given one that represent small changes
      // This method is only called from move-based algorithms (Local Search, Simulated Annealing, etc.)
      while (true) {
        // Algorithm will draw only a finite amount of samples
        // Change to a for-loop to return a concrete amount of neighbors
        var neighbor = (SOLUTION_CLASS)individual.Clone();
        // modify the solution specified as neighbor
        yield return neighbor;
      }
    }

    // Implement further classes and methods
  }
}

