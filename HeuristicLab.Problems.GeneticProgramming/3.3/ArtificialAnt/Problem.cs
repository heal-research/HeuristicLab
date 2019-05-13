#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Diagnostics.Contracts;
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;


namespace HeuristicLab.Problems.GeneticProgramming.ArtificialAnt {
  [Item("Artificial Ant Problem", "Represents the Artificial Ant problem.")]
  [Creatable(CreatableAttribute.Categories.GeneticProgrammingProblems, Priority = 170)]
  [StorableType("D365171B-7077-4CC2-835C-1827EA67C843")]
  public sealed class Problem : SymbolicExpressionTreeProblem, IStorableContent {

    #region constant for default world (Santa Fe)

    private static readonly char[][] santaFeAntTrail = new[] {
      " ###                            ".ToCharArray(),
      "   #                            ".ToCharArray(),
      "   #                    .###..  ".ToCharArray(),
      "   #                    #    #  ".ToCharArray(),
      "   #                    #    #  ".ToCharArray(),
      "   ####.#####       .##..    .  ".ToCharArray(),
      "            #       .        #  ".ToCharArray(),
      "            #       #        .  ".ToCharArray(),
      "            #       #        .  ".ToCharArray(),
      "            #       #        #  ".ToCharArray(),
      "            .       #        .  ".ToCharArray(),
      "            #       .        .  ".ToCharArray(),
      "            #       .        #  ".ToCharArray(),
      "            #       #        .  ".ToCharArray(),
      "            #       #  ...###.  ".ToCharArray(),
      "            .   .#...  #        ".ToCharArray(),
      "            .   .      .        ".ToCharArray(),
      "            #   .      .        ".ToCharArray(),
      "            #   #      .#...    ".ToCharArray(),
      "            #   #          #    ".ToCharArray(),
      "            #   #          .    ".ToCharArray(),
      "            #   #          .    ".ToCharArray(),
      "            #   .      ...#.    ".ToCharArray(),
      "            #   .      #        ".ToCharArray(),
      " ..##..#####.   #               ".ToCharArray(),
      " #              #               ".ToCharArray(),
      " #              #               ".ToCharArray(),
      " #     .#######..               ".ToCharArray(),
      " #     #                        ".ToCharArray(),
      " .     #                        ".ToCharArray(),
      " .####..                        ".ToCharArray(),
      "                                ".ToCharArray()
    };


    #endregion

    #region Parameter Properties
    public IValueParameter<BoolMatrix> WorldParameter {
      get { return (IValueParameter<BoolMatrix>)Parameters["World"]; }
    }
    public IFixedValueParameter<IntValue> MaxTimeStepsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["MaximumTimeSteps"]; }
    }
    #endregion

    #region Properties
    public BoolMatrix World {
      get { return WorldParameter.Value; }
      set { WorldParameter.Value = value; }
    }
    public int MaxTimeSteps {
      get { return MaxTimeStepsParameter.Value.Value; }
      set { MaxTimeStepsParameter.Value.Value = value; }
    }
    #endregion

    public override bool Maximization {
      get { return true; }
    }

    #region item cloning and persistence
    // persistence
    [StorableConstructor]
    private Problem(StorableConstructorFlag _) : base(_) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() { }

    // cloning 
    private Problem(Problem original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new Problem(this, cloner);
    }
    #endregion

    public Problem() : base(new SymbolicExpressionTreeEncoding()) {
      BoolMatrix world = new BoolMatrix(ToBoolMatrix(santaFeAntTrail));
      Parameters.Add(new ValueParameter<BoolMatrix>("World", "The world for the artificial ant with scattered food items.", world));
      Parameters.Add(new FixedValueParameter<IntValue>("MaximumTimeSteps", "The number of time steps the artificial ant has available to collect all food items.", new IntValue(600)));

      var g = new SimpleSymbolicExpressionGrammar();
      g.AddSymbols(new string[] { "IfFoodAhead", "Prog2" }, 2, 2);
      g.AddSymbols(new string[] { "Prog3" }, 3, 3);
      g.AddTerminalSymbols(new string[] { "Move", "Left", "Right" });

      Encoding.TreeLength = 20;
      Encoding.TreeDepth = 10;
      Encoding.GrammarParameter.ReadOnly = false;
      Encoding.Grammar = g;
      Encoding.GrammarParameter.ReadOnly = true;

      BestKnownQuality = 89;
      BestKnownQualityParameter.ReadOnly = true;
    }


    public override double Evaluate(ISymbolicExpressionTree tree, IRandom random) {
      var interpreter = new Interpreter(tree, World, MaxTimeSteps);
      interpreter.Run();
      return interpreter.FoodEaten;
    }

    public override void Analyze(ISymbolicExpressionTree[] trees, double[] qualities, ResultCollection results, IRandom random) {
      const string bestSolutionResultName = "Best Solution";
      var bestQuality = Maximization ? qualities.Max() : qualities.Min();
      var bestIdx = Array.IndexOf(qualities, bestQuality);

      if (!results.ContainsKey(bestSolutionResultName)) {
        results.Add(new Result(bestSolutionResultName, new Solution(World, trees[bestIdx], MaxTimeSteps, qualities[bestIdx])));
      } else if (((Solution)(results[bestSolutionResultName].Value)).Quality < qualities[bestIdx]) {
        results[bestSolutionResultName].Value = new Solution(World, trees[bestIdx], MaxTimeSteps, qualities[bestIdx]);
      }
    }

    #region helpers
    private bool[,] ToBoolMatrix(char[][] ch) {
      var rows = ch.Length;
      var cols = ch[0].Length;
      var b = new bool[rows, cols];
      for (int r = 0; r < rows; r++) {
        Contract.Assert(ch[r].Length == cols); // all rows must have the same number of columns
        for (int c = 0; c < cols; c++) {
          b[r, c] = ch[r][c] == '#';
        }
      }
      return b;
    }
    #endregion
  }
}
