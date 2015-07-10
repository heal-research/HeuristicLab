#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Random;
namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Tests {
  internal class Util {

    public static void InitTree(ISymbolicExpressionTree tree, MersenneTwister twister, List<string> varNames) {
      foreach (var node in tree.IterateNodesPostfix()) {
        if (node is VariableTreeNode) {
          var varNode = node as VariableTreeNode;
          varNode.Weight = twister.NextDouble() * 20.0 - 10.0;
          varNode.VariableName = varNames[twister.Next(varNames.Count)];
        } else if (node is ConstantTreeNode) {
          var constantNode = node as ConstantTreeNode;
          constantNode.Value = twister.NextDouble() * 20.0 - 10.0;
        }
      }
    }


    public static ISymbolicExpressionTree[] CreateRandomTrees(MersenneTwister twister, Dataset dataset, ISymbolicExpressionGrammar grammar, int popSize) {
      return CreateRandomTrees(twister, dataset, grammar, popSize, 1, 200, 3, 3);
    }

    public static ISymbolicExpressionTree[] CreateRandomTrees(MersenneTwister twister, Dataset dataset, ISymbolicExpressionGrammar grammar,
      int popSize, int minSize, int maxSize,
      int maxFunctionDefinitions, int maxFunctionArguments) {
      foreach (Variable variableSymbol in grammar.Symbols.OfType<Variable>()) {
        variableSymbol.VariableNames = dataset.VariableNames.Skip(1);
      }
      ISymbolicExpressionTree[] randomTrees = new ISymbolicExpressionTree[popSize];
      for (int i = 0; i < randomTrees.Length; i++) {
        randomTrees[i] = ProbabilisticTreeCreator.Create(twister, grammar, maxSize, 10);
      }
      return randomTrees;
    }


    public static Dataset CreateRandomDataset(MersenneTwister twister, int rows, int columns) {
      double[,] data = new double[rows, columns];
      for (int i = 0; i < rows; i++) {
        for (int j = 0; j < columns; j++) {
          data[i, j] = twister.NextDouble() * 2.0 - 1.0;
        }
      }
      IEnumerable<string> variableNames = new string[] { "y" }.Concat(Enumerable.Range(0, columns - 1).Select(x => "x" + x.ToString()));
      Dataset ds = new Dataset(variableNames, data);
      return ds;
    }

    public static double NodesPerSecond(long nNodes, Stopwatch watch) {
      return nNodes / (watch.ElapsedMilliseconds / 1000.0);
    }

    public static double CalculateEvaluatedNodesPerSec(ISymbolicExpressionTree[] trees, ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, Dataset dataset, int repetitions) {
      // warm up
      IEnumerable<int> rows = Enumerable.Range(0, dataset.Rows).ToList();
      long nNodes = 0;
      double c = 0;
      for (int i = 0; i < trees.Length; i++) {
        nNodes += trees[i].Length * (dataset.Rows - 1);
        c = interpreter.GetSymbolicExpressionTreeValues(trees[i], dataset, rows).Count(); // count needs to evaluate all rows
      }

      Stopwatch watch = new Stopwatch();
      for (int rep = 0; rep < repetitions; rep++) {
        watch.Start();
        c = 0;
        for (int i = 0; i < trees.Length; i++) {
          interpreter.GetSymbolicExpressionTreeValues(trees[i], dataset, rows).Count(); // count needs to evaluate all rows
        }
        watch.Stop();
      }
      Console.WriteLine("Random tree evaluation performance of " + interpreter.GetType() + ": " +
        watch.ElapsedMilliseconds + "ms " +
        Util.NodesPerSecond(nNodes * repetitions, watch) + " nodes/sec");
      return Util.NodesPerSecond(nNodes * repetitions, watch);
    }
  }
}
