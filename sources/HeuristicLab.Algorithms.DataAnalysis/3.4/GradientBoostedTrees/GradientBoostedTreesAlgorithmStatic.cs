#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 * and the BEACON Center for the Study of Evolution in Action.
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
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Random;

namespace HeuristicLab.Algorithms.DataAnalysis {
  public static class GradientBoostedTreesAlgorithmStatic {
    #region static API

    public interface IGbmState {
      IRegressionModel GetModel();
      double GetTrainLoss();
      double GetTestLoss();
      IEnumerable<KeyValuePair<string, double>> GetVariableRelevance();
    }

    // created through factory method
    // GbmState details are private API users can only use methods from IGbmState
    private class GbmState : IGbmState {
      internal IRegressionProblemData problemData { get; private set; }
      internal ILossFunction lossFunction { get; private set; }
      internal int maxSize { get; private set; }
      internal double nu { get; private set; }
      internal double r { get; private set; }
      internal double m { get; private set; }
      internal int[] trainingRows { get; private set; }
      internal int[] testRows { get; private set; }
      internal RegressionTreeBuilder treeBuilder { get; private set; }

      private readonly uint randSeed;
      private MersenneTwister random { get; set; }

      // array members (allocate only once)
      internal double[] pred;
      internal double[] predTest;
      internal double[] y;
      internal int[] activeIdx;
      internal double[] pseudoRes;

      private readonly IList<IRegressionModel> models;
      private readonly IList<double> weights;

      public GbmState(IRegressionProblemData problemData, ILossFunction lossFunction, uint randSeed, int maxSize, double r, double m, double nu) {
        // default settings for MaxSize, Nu and R
        this.maxSize = maxSize;
        this.nu = nu;
        this.r = r;
        this.m = m;

        this.randSeed = randSeed;
        random = new MersenneTwister(randSeed);
        this.problemData = problemData;
        this.trainingRows = problemData.TrainingIndices.ToArray();
        this.testRows = problemData.TestIndices.ToArray();
        this.lossFunction = lossFunction;

        int nRows = trainingRows.Length;

        y = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, trainingRows).ToArray();

        treeBuilder = new RegressionTreeBuilder(problemData, random);

        activeIdx = Enumerable.Range(0, nRows).ToArray();

        var zeros = Enumerable.Repeat(0.0, nRows).ToArray();
        double f0 = lossFunction.LineSearch(y, zeros, activeIdx, 0, nRows - 1); // initial constant value (mean for squared errors)
        pred = Enumerable.Repeat(f0, nRows).ToArray();
        predTest = Enumerable.Repeat(f0, testRows.Length).ToArray();
        pseudoRes = new double[nRows];

        models = new List<IRegressionModel>();
        weights = new List<double>();
        // add constant model
        models.Add(new ConstantRegressionModel(f0));
        weights.Add(1.0);
      }

      public IRegressionModel GetModel() {
#pragma warning disable 618
        var model = new GradientBoostedTreesModel(models, weights);
#pragma warning restore 618
        // we don't know the number of iterations here but the number of weights is equal 
        // to the number of iterations + 1 (for the constant model)
        // wrap the actual model in a surrogate that enables persistence and lazy recalculation of the model if necessary
        return new GradientBoostedTreesModelSurrogate(problemData, randSeed, lossFunction, weights.Count - 1, maxSize, r, m, nu, model);
      }
      public IEnumerable<KeyValuePair<string, double>> GetVariableRelevance() {
        return treeBuilder.GetVariableRelevance();
      }

      public double GetTrainLoss() {
        int nRows = y.Length;
        return lossFunction.GetLoss(y, pred) / nRows;
      }
      public double GetTestLoss() {
        var yTest = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, testRows);
        var nRows = testRows.Length;
        return lossFunction.GetLoss(yTest, predTest) / nRows;
      }

      internal void AddModel(IRegressionModel m, double weight) {
        models.Add(m);
        weights.Add(weight);
      }
    }

    // simple interface
    public static IRegressionSolution TrainGbm(IRegressionProblemData problemData, ILossFunction lossFunction, int maxSize, double nu, double r, double m, int maxIterations, uint randSeed = 31415) {
      Contract.Assert(r > 0);
      Contract.Assert(r <= 1.0);
      Contract.Assert(nu > 0);
      Contract.Assert(nu <= 1.0);

      var state = (GbmState)CreateGbmState(problemData, lossFunction, randSeed, maxSize, r, m, nu);

      for (int iter = 0; iter < maxIterations; iter++) {
        MakeStep(state);
      }

      var model = state.GetModel();
      return new RegressionSolution(model, (IRegressionProblemData)problemData.Clone());
    }

    // for custom stepping & termination
    public static IGbmState CreateGbmState(IRegressionProblemData problemData, ILossFunction lossFunction, uint randSeed, int maxSize = 3, double r = 0.66, double m = 0.5, double nu = 0.01) {
      return new GbmState(problemData, lossFunction, randSeed, maxSize, r, m, nu);
    }

    // use default settings for maxSize, nu, r from state
    public static void MakeStep(IGbmState state) {
      var gbmState = state as GbmState;
      if (gbmState == null) throw new ArgumentException("state");

      MakeStep(gbmState, gbmState.maxSize, gbmState.nu, gbmState.r, gbmState.m);
    }

    // allow dynamic adaptation of maxSize, nu and r (even though this is not used)
    public static void MakeStep(IGbmState state, int maxSize, double nu, double r, double m) {
      var gbmState = state as GbmState;
      if (gbmState == null) throw new ArgumentException("state");

      var problemData = gbmState.problemData;
      var lossFunction = gbmState.lossFunction;
      var yPred = gbmState.pred;
      var yPredTest = gbmState.predTest;
      var treeBuilder = gbmState.treeBuilder;
      var y = gbmState.y;
      var activeIdx = gbmState.activeIdx;
      var pseudoRes = gbmState.pseudoRes;
      var trainingRows = gbmState.trainingRows;
      var testRows = gbmState.testRows;

      // copy output of gradient function to pre-allocated rim array (pseudo-residual per row and model)
      int rimIdx = 0;
      foreach (var g in lossFunction.GetLossGradient(y, yPred)) {
        pseudoRes[rimIdx++] = g;
      }

      var tree = treeBuilder.CreateRegressionTreeForGradientBoosting(pseudoRes, yPred, maxSize, activeIdx, lossFunction, r, m);

      int i = 0;
      foreach (var pred in tree.GetEstimatedValues(problemData.Dataset, trainingRows)) {
        yPred[i] = yPred[i] + nu * pred;
        i++;
      }
      // update predictions for validation set
      i = 0;
      foreach (var pred in tree.GetEstimatedValues(problemData.Dataset, testRows)) {
        yPredTest[i] = yPredTest[i] + nu * pred;
        i++;
      }

      gbmState.AddModel(tree, nu);
    }
    #endregion
  }
}
