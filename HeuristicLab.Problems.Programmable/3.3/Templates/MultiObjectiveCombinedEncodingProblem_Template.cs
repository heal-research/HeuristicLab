using System;
using System.Linq;
using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Problems.Programmable;
//using HeuristicLab.Encodings.BinaryVectorEncoding;
//using HeuristicLab.Encodings.IntegerVectorEncoding;
//using HeuristicLab.Encodings.RealVectorEncoding;
//using HeuristicLab.Encodings.PermutationEncoding;
//using HeuristicLab.Encodings.LinearLinkageEncoding;
//using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.Programmable {
  public class CompiledSingleObjectiveProblemDefinition : CompiledMultiObjectiveProblemDefinition<CombinedEncoding, CombinedSolution> {
    public override bool[] Maximization { get { return new[] { true, false }; } }

    public override void Initialize() {
      // Use vars.yourVariable to access variables in the variable store i.e. yourVariable
      // Define e.g. the length of the solution encoding or the solution creator by modifying the Encoding property
      // Add additional initialization code e.g. private variables that you need for evaluating
      //Encoding.Add(new BinaryVectorEncoding("b") { Length = 10 });
      //Encoding.Add(new IntegerVectorEncoding("i") { Length = 10, Bounds = new int[,] { { -100, 100 } } });
      //Encoding.Add(new RealVectorEncoding("r") { Length = 10, Bounds = new double[,] { { -100, 100 } } });
      //Encoding.Add(new PermutationEncoding("p") { Length = 20, PermutationType = PermutationTypes.Absolute });
      //Encoding.Add(new LinearLinkageEncoding("lle") { Length = 30 });
      //Encoding.Add(new SymbolicExpressionTreeEncoding("tree") { ... });
    }

    public override double[] Evaluate(CombinedSolution solution, IRandom random) {
      // Use vars.yourVariable to access variables in the variable store i.e. yourVariable
      var quality = new[] { 0.0, 0.0 };
      // var b = solution.GetSolution<BinaryVector>("b");
      // quality[0] = b.Count(x => x); // one max!
      // var r = solution.GetSolution<RealVector>("r");
      // quality[1] = r.Select((i, v) => new { Idx = i, Val = v }).Sum(x => b[x.Idx] ? x.Val * x.Val : 0.0); // sphere

      // NOTE: Check the Maximization property above (true or false)!
      return quality;
    }

    public override void Analyze(CombinedSolution[] solutions, double[][] qualities, ResultCollection results, IRandom random) {
      // Use vars.yourVariable to access variables in the variable store i.e. yourVariable
      // Write or update results given the range of vectors and resulting qualities
      // Uncomment the following lines if you want to retrieve the best solution
    }

    public override IEnumerable<CombinedSolution> GetNeighbors(CombinedSolution solution, IRandom random) {
      // Use vars.yourVariable to access variables in the variable store i.e. yourVariable
      // Create new vectors, based on the given one that represent small changes
      // This method is only called from move-based algorithms (Local Search, Simulated Annealing, etc.)
      while (true) {
        // Algorithm will draw only a finite amount of samples
        // Change to a for-loop to return a concrete amount of neighbors
        var neighbor = (CombinedSolution)solution.Clone();
        // modify the solution specified as neighbor
        yield return neighbor;
      }
    }

    // Implement further classes and methods
  }
}

