#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Diagnostics.Contracts;
using System.Linq;
using HeuristicLab.Algorithms.DataAnalysis.MctsSymbolicRegression.Policies;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;
using HeuristicLab.Random;

namespace HeuristicLab.Algorithms.DataAnalysis.MctsSymbolicRegression {
  public static class MctsSymbolicRegressionStatic {
    // TODO: SGD with adagrad instead of lbfgs?
    // TODO: check Taylor expansion capabilities (ln(x), sqrt(x), exp(x)) in combination with GBT
    // TODO: optimize for 3 targets concurrently (y, 1/y, exp(y), and log(y))? Would simplify the number of possible expressions again
    #region static API

    public interface IState {
      bool Done { get; }
      ISymbolicRegressionModel BestModel { get; }
      double BestSolutionTrainingQuality { get; }
      double BestSolutionTestQuality { get; }
      int TotalRollouts { get; }
      int EffectiveRollouts { get; }
      int FuncEvaluations { get; }
      int GradEvaluations { get; } // number of gradient evaluations (* num parameters) to get a value representative of the effort comparable to the number of function evaluations
      // TODO other stats on LM optimizer might be interesting here
    }

    // created through factory method
    private class State : IState {
      private const int MaxParams = 100;

      // state variables used by MCTS
      internal readonly Automaton automaton;
      internal IRandom random { get; private set; }
      internal readonly Tree tree;
      internal readonly Func<byte[], int, double> evalFun;
      internal readonly IPolicy treePolicy;
      // MCTS might get stuck. Track statistics on the number of effective rollouts
      internal int totalRollouts;
      internal int effectiveRollouts;


      // state variables used only internally (for eval function)
      private readonly IRegressionProblemData problemData;
      private readonly double[][] x;
      private readonly double[] y;
      private readonly double[][] testX;
      private readonly double[] testY;
      private readonly double[] scalingFactor;
      private readonly double[] scalingOffset;
      private readonly int constOptIterations;
      private readonly double lowerEstimationLimit, upperEstimationLimit;

      private readonly ExpressionEvaluator evaluator, testEvaluator;

      // values for best solution
      private double bestRSq;
      private byte[] bestCode;
      private int bestNParams;
      private double[] bestConsts;

      // stats
      private int funcEvaluations;
      private int gradEvaluations;

      // buffers
      private readonly double[] ones; // vector of ones (as default params)
      private readonly double[] constsBuf;
      private readonly double[] predBuf, testPredBuf;
      private readonly double[][] gradBuf;

      public State(IRegressionProblemData problemData, uint randSeed, int maxVariables, bool scaleVariables, int constOptIterations,
        IPolicy treePolicy = null,
        double lowerEstimationLimit = double.MinValue, double upperEstimationLimit = double.MaxValue,
        bool allowProdOfVars = true,
        bool allowExp = true,
        bool allowLog = true,
        bool allowInv = true,
        bool allowMultipleTerms = false) {

        this.problemData = problemData;
        this.constOptIterations = constOptIterations;
        this.evalFun = this.Eval;
        this.lowerEstimationLimit = lowerEstimationLimit;
        this.upperEstimationLimit = upperEstimationLimit;

        random = new MersenneTwister(randSeed);

        // prepare data for evaluation
        double[][] x;
        double[] y;
        double[][] testX;
        double[] testY;
        double[] scalingFactor;
        double[] scalingOffset;
        // get training and test datasets (scale linearly based on training set if required)
        GenerateData(problemData, scaleVariables, problemData.TrainingIndices, out x, out y, out scalingFactor, out scalingOffset);
        GenerateData(problemData, problemData.TestIndices, scalingFactor, scalingOffset, out testX, out testY);
        this.x = x;
        this.y = y;
        this.testX = testX;
        this.testY = testY;
        this.scalingFactor = scalingFactor;
        this.scalingOffset = scalingOffset;
        this.evaluator = new ExpressionEvaluator(y.Length, lowerEstimationLimit, upperEstimationLimit);
        // we need a separate evaluator because the vector length for the test dataset might differ 
        this.testEvaluator = new ExpressionEvaluator(testY.Length, lowerEstimationLimit, upperEstimationLimit);

        this.automaton = new Automaton(x, maxVariables, allowProdOfVars, allowExp, allowLog, allowInv, allowMultipleTerms);
        this.treePolicy = treePolicy ?? new Ucb();
        this.tree = new Tree() { state = automaton.CurrentState, actionStatistics = treePolicy.CreateActionStatistics() };

        // reset best solution
        this.bestRSq = 0;
        // code for default solution (constant model)
        this.bestCode = new byte[] { (byte)OpCodes.LoadConst0, (byte)OpCodes.Exit };
        this.bestNParams = 0;
        this.bestConsts = null;

        // init buffers
        this.ones = Enumerable.Repeat(1.0, MaxParams).ToArray();
        constsBuf = new double[MaxParams];
        this.predBuf = new double[y.Length];
        this.testPredBuf = new double[testY.Length];

        this.gradBuf = Enumerable.Range(0, MaxParams).Select(_ => new double[y.Length]).ToArray();
      }

      #region IState inferface
      public bool Done { get { return tree != null && tree.Done; } }

      public double BestSolutionTrainingQuality {
        get {
          evaluator.Exec(bestCode, x, bestConsts, predBuf);
          return RSq(y, predBuf);
        }
      }

      public double BestSolutionTestQuality {
        get {
          testEvaluator.Exec(bestCode, testX, bestConsts, testPredBuf);
          return RSq(testY, testPredBuf);
        }
      }

      // takes the code of the best solution and creates and equivalent symbolic regression model
      public ISymbolicRegressionModel BestModel {
        get {
          var treeGen = new SymbolicExpressionTreeGenerator(problemData.AllowedInputVariables.ToArray());
          var interpreter = new SymbolicDataAnalysisExpressionTreeLinearInterpreter();

          var t = new SymbolicExpressionTree(treeGen.Exec(bestCode, bestConsts, bestNParams, scalingFactor, scalingOffset));
          var model = new SymbolicRegressionModel(problemData.TargetVariable, t, interpreter, lowerEstimationLimit, upperEstimationLimit);

          // model has already been scaled linearly in Eval
          return model;
        }
      }

      public int TotalRollouts { get { return totalRollouts; } }
      public int EffectiveRollouts { get { return effectiveRollouts; } }
      public int FuncEvaluations { get { return funcEvaluations; } }
      public int GradEvaluations { get { return gradEvaluations; } } // number of gradient evaluations (* num parameters) to get a value representative of the effort comparable to the number of function evaluations

      #endregion

      private double Eval(byte[] code, int nParams) {
        double[] optConsts;
        double q;
        Eval(code, nParams, out q, out optConsts);

        if (q > bestRSq) {
          bestRSq = q;
          bestNParams = nParams;
          this.bestCode = new byte[code.Length];
          this.bestConsts = new double[bestNParams];

          Array.Copy(code, bestCode, code.Length);
          Array.Copy(optConsts, bestConsts, bestNParams);
        }

        return q;
      }

      private void Eval(byte[] code, int nParams, out double rsq, out double[] optConsts) {
        // we make a first pass to determine a valid starting configuration for all constants
        // constant c in log(c + f(x)) is adjusted to guarantee that x is positive (see expression evaluator)
        // scale and offset are set to optimal starting configuration
        // assumes scale is the first param and offset is the last param
        double alpha;
        double beta;

        // reset constants
        Array.Copy(ones, constsBuf, nParams);
        evaluator.Exec(code, x, constsBuf, predBuf, adjustOffsetForLogAndExp: true);
        funcEvaluations++;

        // calc opt scaling (alpha*f(x) + beta)
        OnlineCalculatorError error;
        OnlineLinearScalingParameterCalculator.Calculate(predBuf, y, out alpha, out beta, out error);
        if (error == OnlineCalculatorError.None) {
          constsBuf[0] *= beta;
          constsBuf[nParams - 1] = constsBuf[nParams - 1] * beta + alpha;
        }
        if (nParams <= 2 || constOptIterations <= 0) {
          // if we don't need to optimize parameters then we are done
          // changing scale and offset does not influence r²
          rsq = RSq(y, predBuf);
          optConsts = constsBuf;
        } else {
          // optimize constants using the starting point calculated above
          OptimizeConstsLm(code, constsBuf, nParams, 0.0, nIters: constOptIterations);

          evaluator.Exec(code, x, constsBuf, predBuf);
          funcEvaluations++;

          rsq = RSq(y, predBuf);
          optConsts = constsBuf;
        }
      }



      #region helpers
      private static double RSq(IEnumerable<double> x, IEnumerable<double> y) {
        OnlineCalculatorError error;
        double r = OnlinePearsonsRCalculator.Calculate(x, y, out error);
        return error == OnlineCalculatorError.None ? r * r : 0.0;
      }


      private void OptimizeConstsLm(byte[] code, double[] consts, int nParams, double epsF = 0.0, int nIters = 100) {
        double[] optConsts = new double[nParams]; // allocate a smaller buffer for constants opt (TODO perf?)
        Array.Copy(consts, optConsts, nParams);

        alglib.minlmstate state;
        alglib.minlmreport rep = null;
        alglib.minlmcreatevj(y.Length, optConsts, out state);
        alglib.minlmsetcond(state, 0.0, epsF, 0.0, nIters);
        //alglib.minlmsetgradientcheck(state, 0.000001);
        alglib.minlmoptimize(state, Func, FuncAndJacobian, null, code);
        alglib.minlmresults(state, out optConsts, out rep);
        funcEvaluations += rep.nfunc;
        gradEvaluations += rep.njac * nParams;

        if (rep.terminationtype < 0) throw new ArgumentException("lm failed: termination type = " + rep.terminationtype);

        // only use optimized constants if successful
        if (rep.terminationtype >= 0) {
          Array.Copy(optConsts, consts, optConsts.Length);
        }
      }

      private void Func(double[] arg, double[] fi, object obj) {
        var code = (byte[])obj;
        evaluator.Exec(code, x, arg, predBuf); // gradients are nParams x vLen
        for (int r = 0; r < predBuf.Length; r++) {
          var res = predBuf[r] - y[r];
          fi[r] = res;
        }
      }
      private void FuncAndJacobian(double[] arg, double[] fi, double[,] jac, object obj) {
        int nParams = arg.Length;
        var code = (byte[])obj;
        evaluator.ExecGradient(code, x, arg, predBuf, gradBuf); // gradients are nParams x vLen
        for (int r = 0; r < predBuf.Length; r++) {
          var res = predBuf[r] - y[r];
          fi[r] = res;

          for (int k = 0; k < nParams; k++) {
            jac[r, k] = gradBuf[k][r];
          }
        }
      }
      #endregion
    }

    public static IState CreateState(IRegressionProblemData problemData, uint randSeed, int maxVariables = 3,
      bool scaleVariables = true, int constOptIterations = 0,
      IPolicy policy = null,
      double lowerEstimationLimit = double.MinValue, double upperEstimationLimit = double.MaxValue,
      bool allowProdOfVars = true,
      bool allowExp = true,
      bool allowLog = true,
      bool allowInv = true,
      bool allowMultipleTerms = false
      ) {
      return new State(problemData, randSeed, maxVariables, scaleVariables, constOptIterations,
        policy,
        lowerEstimationLimit, upperEstimationLimit,
        allowProdOfVars, allowExp, allowLog, allowInv, allowMultipleTerms);
    }

    // returns the quality of the evaluated solution
    public static double MakeStep(IState state) {
      var mctsState = state as State;
      if (mctsState == null) throw new ArgumentException("state");
      if (mctsState.Done) throw new NotSupportedException("The tree search has enumerated all possible solutions.");

      return TreeSearch(mctsState);
    }
    #endregion

    private static double TreeSearch(State mctsState) {
      var automaton = mctsState.automaton;
      var tree = mctsState.tree;
      var eval = mctsState.evalFun;
      var rand = mctsState.random;
      var treePolicy = mctsState.treePolicy;
      double q = 0;
      bool success = false;
      do {
        automaton.Reset();
        success = TryTreeSearchRec(rand, tree, automaton, eval, treePolicy, out q);
        mctsState.totalRollouts++;
      } while (!success && !tree.Done);
      mctsState.effectiveRollouts++;
      return q;
    }

    // tree search might fail because of constraints for expressions
    // in this case we get stuck we just restart
    // see ConstraintHandler.cs for more info
    private static bool TryTreeSearchRec(IRandom rand, Tree tree, Automaton automaton, Func<byte[], int, double> eval, IPolicy treePolicy,
      out double q) {
      Tree selectedChild = null;
      Contract.Assert(tree.state == automaton.CurrentState);
      Contract.Assert(!tree.Done);
      if (tree.children == null) {
        if (automaton.IsFinalState(tree.state)) {
          // final state
          tree.Done = true;

          // EVALUATE
          byte[] code; int nParams;
          automaton.GetCode(out code, out nParams);
          q = eval(code, nParams);

          treePolicy.Update(tree.actionStatistics, q);
          return true; // we reached a final state
        } else {
          // EXPAND 
          int[] possibleFollowStates;
          int nFs;
          automaton.FollowStates(automaton.CurrentState, out possibleFollowStates, out nFs);
          if (nFs == 0) {
            // stuck in a dead end (no final state and no allowed follow states)
            q = 0;
            tree.Done = true;
            tree.children = null;
            return false;
          }
          tree.children = new Tree[nFs];
          for (int i = 0; i < tree.children.Length; i++)
            tree.children[i] = new Tree() { children = null, state = possibleFollowStates[i], actionStatistics = treePolicy.CreateActionStatistics() };

          selectedChild = nFs > 1 ? SelectFinalOrRandom(automaton, tree, rand) : tree.children[0];
        }
      } else {
        // tree.children != null
        // UCT selection within tree
        int selectedIdx = 0;
        if (tree.children.Length > 1) {
          selectedIdx = treePolicy.Select(tree.children.Select(ch => ch.actionStatistics), rand);
        }
        selectedChild = tree.children[selectedIdx];
      }
      // make selected step and recurse
      automaton.Goto(selectedChild.state);
      var success = TryTreeSearchRec(rand, selectedChild, automaton, eval, treePolicy, out q);
      if (success) {
        // only update if successful
        treePolicy.Update(tree.actionStatistics, q);
      }

      tree.Done = tree.children.All(ch => ch.Done);
      if (tree.Done) {
        tree.children = null; // cut off the sub-branch if it has been fully explored
      }
      return success;
    }

    private static Tree SelectFinalOrRandom(Automaton automaton, Tree tree, IRandom rand) {
      // if one of the new children leads to a final state then go there
      // otherwise choose a random child
      int selectedChildIdx = -1;
      // find first final state if there is one
      for (int i = 0; i < tree.children.Length; i++) {
        if (automaton.IsFinalState(tree.children[i].state)) {
          selectedChildIdx = i;
          break;
        }
      }
      // no final state -> select a the first child
      if (selectedChildIdx == -1) {
        selectedChildIdx = 0;
      }
      return tree.children[selectedChildIdx];
    }

    // scales data and extracts values from dataset into arrays
    private static void GenerateData(IRegressionProblemData problemData, bool scaleVariables, IEnumerable<int> rows,
      out double[][] xs, out double[] y, out double[] scalingFactor, out double[] scalingOffset) {
      xs = new double[problemData.AllowedInputVariables.Count()][];

      var i = 0;
      if (scaleVariables) {
        scalingFactor = new double[xs.Length];
        scalingOffset = new double[xs.Length];
      } else {
        scalingFactor = null;
        scalingOffset = null;
      }
      foreach (var var in problemData.AllowedInputVariables) {
        if (scaleVariables) {
          var minX = problemData.Dataset.GetDoubleValues(var, rows).Min();
          var maxX = problemData.Dataset.GetDoubleValues(var, rows).Max();
          var range = maxX - minX;

          // scaledX = (x - min) / range
          var sf = 1.0 / range;
          var offset = -minX / range;
          scalingFactor[i] = sf;
          scalingOffset[i] = offset;
          i++;
        }
      }

      GenerateData(problemData, rows, scalingFactor, scalingOffset, out xs, out y);
    }

    // extract values from dataset into arrays
    private static void GenerateData(IRegressionProblemData problemData, IEnumerable<int> rows, double[] scalingFactor, double[] scalingOffset,
     out double[][] xs, out double[] y) {
      xs = new double[problemData.AllowedInputVariables.Count()][];

      int i = 0;
      foreach (var var in problemData.AllowedInputVariables) {
        var sf = scalingFactor == null ? 1.0 : scalingFactor[i];
        var offset = scalingFactor == null ? 0.0 : scalingOffset[i];
        xs[i++] =
          problemData.Dataset.GetDoubleValues(var, rows).Select(xi => xi * sf + offset).ToArray();
      }

      y = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, rows).ToArray();
    }
  }
}
