using System.Collections.Generic;
using HeuristicLab.Core;
using HeuristicLab.Optimization;

namespace HeuristicLab.Problems.Programmable {
  public class CompiledSingleObjectiveProblemDefinition : CompiledProblemDefinition, ISingleObjectiveProblemDefinition {
    public bool Maximization { get { return false; } }

    public override void Initialize() {
      // Use vars.yourVariable to access variables in the variable store i.e. yourVariable
      // Define the solution encoding which can also consist of multiple vectors, examples below
      //Encoding = new BinaryVectorEncoding("b", length: 5);
      //Encoding = new IntegerVectorEncoding("i", length: 5, min: 2, max: 14, step: 2);
      //Encoding = new RealVectorEncoding("r", length: 5, min: -1.0, max: 1.0);
      //Encoding = new PermutationEncoding("p", length: 5, type: PermutationTypes.Absolute);
      //Encoding = new LinearLinkageEncoding("l", length: 5);
      //Encoding = new SymbolicExpressionTreeEncoding("s", new SimpleSymbolicExpressionGrammar(), 50, 12);
      // The encoding can also be a combination
      //Encoding = new MultiEncoding()
      //.Add(new BinaryVectorEncoding("b", length: 5))
      //.Add(new IntegerVectorEncoding("i", length: 5, min: 2, max: 14, step: 4))
      //.Add(new RealVectorEncoding("r", length: 5, min: -1.0, max: 1.0))
      //.Add(new PermutationEncoding("p", length: 5, type: PermutationTypes.Absolute))
      //.Add(new LinearLinkageEncoding("l", length: 5))
      //.Add(new SymbolicExpressionTreeEncoding("s", new SimpleSymbolicExpressionGrammar(), 50, 12))
      ;
      // Add additional initialization code e.g. private variables that you need for evaluating
    }

    public double Evaluate(Individual individual, IRandom random) {
      // Use vars.yourVariable to access variables in the variable store i.e. yourVariable
      var quality = 0.0;
      //quality = individual.RealVector("r").Sum(x => x * x);
      return quality;
    }

    public void Analyze(Individual[] individuals, double[] qualities, ResultCollection results, IRandom random) {
      // Use vars.yourVariable to access variables in the variable store i.e. yourVariable
      // Write or update results given the range of vectors and resulting qualities
      // Uncomment the following lines if you want to retrieve the best individual

      //var orderedIndividuals = individuals.Zip(qualities, (i, q) => new { Individual = i, Quality = q }).OrderBy(z => z.Quality);
      //var best = Maximization ? orderedIndividuals.Last().Individual : orderedIndividuals.First().Individual;

      //if (!results.ContainsKey("Best Solution")) {
      //  results.Add(new Result("Best Solution", typeof(RealVector)));
      //}
      //results["Best Solution"].Value = (IItem)best.RealVector("r").Clone();
    }

    public IEnumerable<Individual> GetNeighbors(Individual individual, IRandom random) {
      // Use vars.yourVariable to access variables in the variable store i.e. yourVariable
      // Create new vectors, based on the given one that represent small changes
      // This method is only called from move-based algorithms (Local Search, Simulated Annealing, etc.)
      while (true) {
        // Algorithm will draw only a finite amount of samples
        // Change to a for-loop to return a concrete amount of neighbors
        var neighbor = individual.Copy();
        // For instance, perform a single bit-flip in a binary parameter
        //var bIndex = random.Next(neighbor.BinaryVector("b").Length);
        //neighbor.BinaryVector("b")[bIndex] = !neighbor.BinaryVector("b")[bIndex];
        yield return neighbor;
      }
    }

    // Implement further classes and methods
  }
}

