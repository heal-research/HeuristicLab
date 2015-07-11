using System;
using System.Linq;
using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Encodings.IntegerVectorEncoding;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Encodings.LinearLinkageEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Problems.Programmable;

namespace HeuristicLab.Problems.Programmable {
  public class CompiledMultiObjectiveProblemDefinition : CompiledProblemDefinition, IMultiObjectiveProblemDefinition {
    public bool[] Maximization { get { return new[] { false, false }; } }

    public override void Initialize() {
      // Use vars.yourVariable to access variables in the variable store i.e. yourVariable
      // Define the solution encoding which can also consist of multiple vectors, examples below
      //Encoding = new BinaryVectorEncoding("b", length: 5);
      //Encoding = new IntegerVectorEncoding("i", length: 5, min: 2, max: 14, step: 4);
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
      //.Add(Encoding = new LinearLinkageEncoding("l", length: 5))
      //.Add(Encoding = new SymbolicExpressionTreeEncoding("s", new SimpleSymbolicExpressionGrammar(), 50, 12))
      ;
      // Add additional initialization code e.g. private variables that you need for evaluating
    }

    public double[] Evaluate(Individual individual, IRandom random) {
      // Use vars.yourVariable to access variables in the variable store i.e. yourVariable
      var qualities = new[] { 0.0, 0.0 };
      //qualities = new [] { individual.RealVector("r").Sum(x => x * x), individual.RealVector("r").Sum(x => x * x * x) };
      return qualities;
    }

    public void Analyze(Individual[] individuals, double[][] qualities, ResultCollection results, IRandom random) {
      // Use vars.yourVariable to access variables in the variable store i.e. yourVariable
      // Write or update results given the range of vectors and resulting qualities
    }
    // Implement further classes and methods
  }
}

