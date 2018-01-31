#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.GeneticProgramming.LawnMower {
  [StorableClass]
  [Creatable(CreatableAttribute.Categories.GeneticProgrammingProblems, Priority = 160)]
  [Item("Lawn Mower Problem", "The lawn mower demo problem for genetic programming.")]
  public class Problem : SymbolicExpressionTreeProblem {
    private const string LawnWidthParameterName = "LawnWidth";
    private const string LawnLengthParameterName = "LawnLength";

    public IFixedValueParameter<IntValue> LawnWidthParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[LawnWidthParameterName]; }
    }
    public IFixedValueParameter<IntValue> LawnLengthParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[LawnLengthParameterName]; }
    }

    public override bool Maximization {
      get { return true; }
    }

    #region item cloning and persistence
    [StorableConstructor]
    protected Problem(bool deserializing) : base(deserializing) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() { }

    protected Problem(Problem original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new Problem(this, cloner);
    }
    #endregion

    public Problem()
      : base() {
      Parameters.Add(new FixedValueParameter<IntValue>(LawnWidthParameterName, "Width of the lawn.", new IntValue(8)));
      Parameters.Add(new FixedValueParameter<IntValue>(LawnLengthParameterName, "Length of the lawn.", new IntValue(8)));

      var g = new SimpleSymbolicExpressionGrammar();
      g.AddSymbols(new string[] { "Sum", "Prog" }, 2, 2);
      g.AddSymbols(new string[] { "Frog" }, 1, 1);
      g.AddTerminalSymbols(new string[] { "Left", "Forward" });
      // initialize 20 ephemeral random constants in [0..32[
      var fastRand = new FastRandom(314159);
      for (int i = 0; i < 20; i++) {
        g.AddTerminalSymbol(string.Format("{0},{1}", fastRand.Next(0, 32), fastRand.Next(0, 32)));
      }

      Encoding = new SymbolicExpressionTreeEncoding(g, 1000, 17);
    }

    public override void Analyze(ISymbolicExpressionTree[] trees, double[] qualities, ResultCollection results, IRandom random) {
      const string bestSolutionResultName = "Best Solution";
      var bestQuality = Maximization ? qualities.Max() : qualities.Min();
      var bestIdx = Array.IndexOf(qualities, bestQuality);

      if (!results.ContainsKey(bestSolutionResultName)) {
        results.Add(new Result(bestSolutionResultName, new Solution(trees[bestIdx], LawnLengthParameter.Value.Value, LawnWidthParameter.Value.Value, bestQuality)));
      } else if (((Solution)(results[bestSolutionResultName].Value)).Quality < qualities[bestIdx]) {
        results[bestSolutionResultName].Value = new Solution(trees[bestIdx], LawnLengthParameter.Value.Value, LawnWidthParameter.Value.Value, bestQuality);
      }
    }

    public override double Evaluate(ISymbolicExpressionTree tree, IRandom random) {
      var length = LawnLengthParameter.Value.Value;
      var width = LawnWidthParameter.Value.Value;

      var lawn = Interpreter.EvaluateLawnMowerProgram(length, width, tree);
      // count number of squares that have been mowed
      int numberOfMowedCells = 0;
      for (int i = 0; i < length; i++)
        for (int j = 0; j < width; j++)
          if (lawn[i, j]) {
            numberOfMowedCells++;
          }
      return numberOfMowedCells;
    }
  }
}
