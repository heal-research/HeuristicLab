#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.GP.StructureIdentification;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HeuristicLab.DataAnalysis;
using System;
using HeuristicLab.GP.Interfaces;
using HeuristicLab.Random;
using HeuristicLab.GP.Operators;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
namespace HeuristicLab.GP.Test {
  public class Util {

    public static void InitTree(IFunctionTree tree, MersenneTwister twister, List<string> varNames) {
      foreach (var node in FunctionTreeIterator.IteratePostfix(tree)) {
        if (node is VariableFunctionTree) {
          var varNode = node as VariableFunctionTree;
          varNode.Weight = twister.NextDouble() * 20.0 - 10.0;
          varNode.SampleOffset = 0;
          varNode.VariableName = varNames[twister.Next(varNames.Count)];
        } else if (node is ConstantFunctionTree) {
          var constantNode = node as ConstantFunctionTree;
          constantNode.Value = twister.NextDouble() * 20.0 - 10.0;
        }
      }
    }

    public static FunctionLibrary CreateFunctionLibrary() {
      FunctionLibrary functionLibrary = new FunctionLibrary();

      Variable variable = new Variable();
      Constant constant = new Constant();
      Differential differential = new Differential();
      Addition addition = new Addition();
      And and = new And();
      //Average average = new Average();
      Cosinus cosinus = new Cosinus();
      Division division = new Division();
      Equal equal = new Equal();
      Exponential exponential = new Exponential();
      GreaterThan greaterThan = new GreaterThan();
      IfThenElse ifThenElse = new IfThenElse();
      LessThan lessThan = new LessThan();
      Logarithm logarithm = new Logarithm();
      Multiplication multiplication = new Multiplication();
      Not not = new Not();
      Or or = new Or();
      Power power = new Power();
      Signum signum = new Signum();
      Sinus sinus = new Sinus();
      Sqrt sqrt = new Sqrt();
      Subtraction subtraction = new Subtraction();
      Tangens tangens = new Tangens();
      Xor xor = new Xor();


      List<IFunction> booleanFunctions = new List<IFunction>();
      booleanFunctions.Add(and);
      booleanFunctions.Add(equal);
      booleanFunctions.Add(greaterThan);
      booleanFunctions.Add(lessThan);
      booleanFunctions.Add(not);
      booleanFunctions.Add(or);
      booleanFunctions.Add(xor);

      List<IFunction> doubleFunctions = new List<IFunction>();
      doubleFunctions.Add(differential);
      doubleFunctions.Add(variable);
      doubleFunctions.Add(constant);
      doubleFunctions.Add(addition);
      // doubleFunctions.Add(average);
      doubleFunctions.Add(cosinus);
      doubleFunctions.Add(division);
      doubleFunctions.Add(exponential);
      doubleFunctions.Add(ifThenElse);
      doubleFunctions.Add(logarithm);
      doubleFunctions.Add(multiplication);
      doubleFunctions.Add(power);
      doubleFunctions.Add(signum);
      doubleFunctions.Add(sinus);
      doubleFunctions.Add(sqrt);
      doubleFunctions.Add(subtraction);
      doubleFunctions.Add(tangens);

      SetAllowedSubOperators(and, booleanFunctions);
      SetAllowedSubOperators(equal, doubleFunctions);
      SetAllowedSubOperators(greaterThan, doubleFunctions);
      SetAllowedSubOperators(lessThan, doubleFunctions);
      SetAllowedSubOperators(not, booleanFunctions);
      SetAllowedSubOperators(or, booleanFunctions);
      SetAllowedSubOperators(xor, booleanFunctions);
      SetAllowedSubOperators(addition, doubleFunctions);
      //SetAllowedSubOperators(average, doubleFunctions);
      SetAllowedSubOperators(cosinus, doubleFunctions);
      SetAllowedSubOperators(division, doubleFunctions);
      SetAllowedSubOperators(exponential, doubleFunctions);
      SetAllowedSubOperators(ifThenElse, 0, booleanFunctions);
      SetAllowedSubOperators(ifThenElse, 1, doubleFunctions);
      SetAllowedSubOperators(ifThenElse, 2, doubleFunctions);
      SetAllowedSubOperators(logarithm, doubleFunctions);
      SetAllowedSubOperators(multiplication, doubleFunctions);
      SetAllowedSubOperators(power, doubleFunctions);
      SetAllowedSubOperators(signum, doubleFunctions);
      SetAllowedSubOperators(sinus, doubleFunctions);
      SetAllowedSubOperators(sqrt, doubleFunctions);
      SetAllowedSubOperators(subtraction, doubleFunctions);
      SetAllowedSubOperators(tangens, doubleFunctions);

      functionLibrary.AddFunction(differential);
      functionLibrary.AddFunction(variable);
      functionLibrary.AddFunction(constant);
      functionLibrary.AddFunction(addition);
      // functionLibrary.AddFunction(average);
      functionLibrary.AddFunction(and);
      functionLibrary.AddFunction(cosinus);
      functionLibrary.AddFunction(division);
      functionLibrary.AddFunction(equal);
      functionLibrary.AddFunction(exponential);
      functionLibrary.AddFunction(greaterThan);
      functionLibrary.AddFunction(ifThenElse);
      functionLibrary.AddFunction(lessThan);
      functionLibrary.AddFunction(logarithm);
      functionLibrary.AddFunction(multiplication);
      functionLibrary.AddFunction(not);
      functionLibrary.AddFunction(power);
      functionLibrary.AddFunction(or);
      functionLibrary.AddFunction(signum);
      functionLibrary.AddFunction(sinus);
      functionLibrary.AddFunction(sqrt);
      functionLibrary.AddFunction(subtraction);
      functionLibrary.AddFunction(tangens);
      functionLibrary.AddFunction(xor);

      variable.SetConstraints(0, 0);
      differential.SetConstraints(0, 0);

      return functionLibrary;

    }

    private static void SetAllowedSubOperators(IFunction f, IEnumerable<IFunction> gs) {
      for (int i = 0; i < f.MaxSubTrees; i++) {
        SetAllowedSubOperators(f, i, gs);
      }
    }

    private static void SetAllowedSubOperators(IFunction f, int i, IEnumerable<IFunction> gs) {
      foreach (var g in gs) {
        f.AddAllowedSubFunction(g, i);
      }
    }

    public static IFunctionTree[] CreateRandomTrees(MersenneTwister twister, Dataset dataset, FunctionLibrary funLib, int popSize) {
      return CreateRandomTrees(twister, dataset, funLib, popSize, 1, 200);
    }

    public static IFunctionTree[] CreateRandomTrees(MersenneTwister twister, Dataset dataset, int popSize) {
      return CreateRandomTrees(twister, dataset, popSize, 1, 200);
    }

    public static IFunctionTree[] CreateRandomTrees(MersenneTwister twister, Dataset dataset, int popSize, int minSize, int maxSize) {
      return CreateRandomTrees(twister, dataset, Util.CreateFunctionLibrary(), popSize, minSize, maxSize);
    }

    public static IFunctionTree[] CreateRandomTrees(MersenneTwister twister, Dataset dataset, FunctionLibrary funLib, int popSize, int minSize, int maxSize) {
      IFunctionTree[] randomTrees = new IFunctionTree[popSize];
      for (int i = 0; i < randomTrees.Length; i++) {
        randomTrees[i] = ProbabilisticTreeCreator.Create(twister, funLib, minSize, maxSize, maxSize + 1);
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
      Dataset ds = new Dataset(data);
      ds.SetVariableName(0, "y");
      return ds;
    }

    public static double NodesPerSecond(long nNodes, Stopwatch watch) {
      return nNodes / (watch.ElapsedMilliseconds / 1000.0);
    }

    public static void EvaluateTrees(IFunctionTree[] trees, ITreeEvaluator evaluator, Dataset dataset, int repetitions) {
      double[] estimation = new double[dataset.Rows];
      // warm up
      for (int i = 0; i < trees.Length; i++) {
        evaluator.PrepareForEvaluation(dataset, trees[i]);
        for (int row = 1; row < dataset.Rows; row++) {
          estimation[row] = evaluator.Evaluate(row);
        }
      }

      Stopwatch watch = new Stopwatch();
      Stopwatch compileWatch = new Stopwatch();
      long nNodes = 0;
      for (int rep = 0; rep < repetitions; rep++) {
        watch.Start();
        for (int i = 0; i < trees.Length; i++) {
          compileWatch.Start();
          evaluator.PrepareForEvaluation(dataset, trees[i]);
          nNodes += trees[i].GetSize() * (dataset.Rows - 1);
          compileWatch.Stop();
          for (int row = 1; row < dataset.Rows; row++) {
            estimation[row] = evaluator.Evaluate(row);
          }
        }
        watch.Stop();
      }
      Assert.Inconclusive("Random tree evaluation performance of " + evaluator.GetType() + ":" +
        watch.ElapsedMilliseconds + "ms (" + compileWatch.ElapsedMilliseconds + " ms) " +
        Util.NodesPerSecond(nNodes, watch) + " nodes/sec");
    }
  }
}
