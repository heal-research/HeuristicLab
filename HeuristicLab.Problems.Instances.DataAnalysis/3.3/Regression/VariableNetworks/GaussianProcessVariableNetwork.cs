#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using HeuristicLab.Core;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public sealed class GaussianProcessVariableNetwork : VariableNetwork {
    private int numberOfFeatures;
    private double noiseRatio;

    public override string Name { get { return string.Format("GaussianProcessVariableNetwork-{0:0%} ({1} dim)", noiseRatio, numberOfFeatures); } }

    public GaussianProcessVariableNetwork(int numberOfFeatures, double noiseRatio,
      IRandom rand)
      : base(250, 250, numberOfFeatures, noiseRatio, rand) {
      this.noiseRatio = noiseRatio;
      this.numberOfFeatures = numberOfFeatures;
    }

    // sample the input variables that are actually used and sample from a Gaussian process
    protected override IEnumerable<double> GenerateRandomFunction(IRandom rand, List<List<double>> xs, out string[] selectedVarNames, out double[] relevance) {
      int nl = SampleNumberOfVariables(rand, xs.Count);

      var selectedIdx = Enumerable.Range(0, xs.Count).Shuffle(rand)
        .Take(nl).ToArray();

      var selectedVars = selectedIdx.Select(i => xs[i]).ToArray();
      selectedVarNames = selectedIdx.Select(i => VariableNames[i]).ToArray();
      return SampleGaussianProcess(rand, selectedVars, out relevance);
    }

    private IEnumerable<double> SampleGaussianProcess(IRandom rand, List<double>[] xs, out double[] relevance) {
      int nl = xs.Length;
      int nRows = xs.First().Count;

      // sample u iid ~ N(0, 1)
      var u = Enumerable.Range(0, nRows).Select(_ => NormalDistributedRandom.NextDouble(rand, 0, 1)).ToArray();

      // sample actual length-scales
      var l = Enumerable.Range(0, nl)
        .Select(_ => rand.NextDouble() * 2 + 0.5)
        .ToArray();

      double[,] K = CalculateCovariance(xs, l);

      // decompose
      alglib.trfac.spdmatrixcholesky(ref K, nRows, false);


      // calc y = Lu
      var y = new double[u.Length];
      alglib.ablas.rmatrixmv(nRows, nRows, K, 0, 0, 0, u, 0, ref y, 0);

      // calculate relevance by removing dimensions
      relevance = CalculateRelevance(y, u, xs, l);

      return y;
    }

    // calculate variable relevance based on removal of variables
    //  1) to remove a variable we set it's length scale to infinity (no relation of the variable value to the target)
    //  2) calculate MSE of the original target values (y) to the updated targes y' (after variable removal)
    //  3) relevance is larger if MSE(y,y') is large
    //  4) scale impacts so that the most important variable has impact = 1
    private double[] CalculateRelevance(double[] y, double[] u, List<double>[] xs, double[] l) {
      int nRows = xs.First().Count;
      var changedL = new double[l.Length];
      var relevance = new double[l.Length];
      for(int i = 0; i < l.Length; i++) {
        Array.Copy(l, changedL, changedL.Length);
        changedL[i] = double.MaxValue;
        var changedK = CalculateCovariance(xs, changedL);

        var yChanged = new double[u.Length];
        alglib.ablas.rmatrixmv(nRows, nRows, changedK, 0, 0, 0, u, 0, ref yChanged, 0);

        OnlineCalculatorError error;
        var mse = OnlineMeanSquaredErrorCalculator.Calculate(y, yChanged, out error);
        if(error != OnlineCalculatorError.None) mse = double.MaxValue;
        relevance[i] = mse;
      }
      // scale so that max relevance is 1.0
      var maxRel = relevance.Max();
      for(int i = 0; i < relevance.Length; i++) relevance[i] /= maxRel;
      return relevance;
    }

    private double[,] CalculateCovariance(List<double>[] xs, double[] l) {
      int nRows = xs.First().Count;
      double[,] K = new double[nRows, nRows];
      for(int r = 0; r < nRows; r++) {
        double[] xi = xs.Select(x => x[r]).ToArray();
        for(int c = 0; c <= r; c++) {
          double[] xj = xs.Select(x => x[c]).ToArray();
          double dSqr = xi.Zip(xj, (xik, xjk) => (xik - xjk))
            .Select(dk => dk * dk)
            .Zip(l, (dk, lk) => dk / lk)
            .Sum();
          K[r, c] = Math.Exp(-dSqr);
        }
      }
      // add a small diagonal matrix for numeric stability
      for(int i = 0; i < nRows; i++) {
        K[i, i] += 1.0E-7;
      }

      return K;
    }
  }
}
