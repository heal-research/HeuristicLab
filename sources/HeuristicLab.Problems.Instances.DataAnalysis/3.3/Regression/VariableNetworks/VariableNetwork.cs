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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class VariableNetwork : ArtificialRegressionDataDescriptor {
    private int nTrainingSamples;
    private int nTestSamples;

    private int numberOfFeatures;
    private double noiseRatio;
    private IRandom random;

    public override string Name { get { return string.Format("VariableNetwork-{0:0%} ({1} dim)", noiseRatio, numberOfFeatures); } }
    private string networkDefinition;
    public string NetworkDefinition { get { return networkDefinition; } }
    public override string Description {
      get {
        return "The data are generated specifically to test methods for variable network analysis.";
      }
    }

    public VariableNetwork(int numberOfFeatures, double noiseRatio,
      IRandom rand)
      : this(250, 250, numberOfFeatures, noiseRatio, rand) { }

    public VariableNetwork(int nTrainingSamples, int nTestSamples,
      int numberOfFeatures, double noiseRatio, IRandom rand) {
      this.nTrainingSamples = nTrainingSamples;
      this.nTestSamples = nTestSamples;
      this.noiseRatio = noiseRatio;
      this.random = rand;
      this.numberOfFeatures = numberOfFeatures;
      // default variable names
      variableNames = Enumerable.Range(1, numberOfFeatures)
        .Select(i => string.Format("X{0:000}", i))
        .ToArray();
    }

    private string[] variableNames;
    protected override string[] VariableNames {
      get {
        return variableNames;
      }
    }

    // there is no specific target variable in variable network analysis but we still need to specify one
    protected override string TargetVariable { get { return VariableNames.Last(); } }

    protected override string[] AllowedInputVariables {
      get {
        return VariableNames.Take(numberOfFeatures - 1).ToArray();
      }
    }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return nTrainingSamples; } }
    protected override int TestPartitionStart { get { return nTrainingSamples; } }
    protected override int TestPartitionEnd { get { return nTrainingSamples + nTestSamples; } }


    protected override List<List<double>> GenerateValues() {
      // variable names are shuffled in the beginning (and sorted at the end)
      variableNames = variableNames.Shuffle(random).ToArray();

      // a third of all variables are independent vars
      List<List<double>> lvl0 = new List<List<double>>();
      int numLvl0 = (int)Math.Ceiling(numberOfFeatures * 0.33);

      List<string> description = new List<string>(); // store information how the variable is actually produced
      List<string[]> inputVarNames = new List<string[]>(); // store information to produce graphviz file

      var nrand = new NormalDistributedRandom(random, 0, 1);
      for (int c = 0; c < numLvl0; c++) {
        var datai = Enumerable.Range(0, TestPartitionEnd).Select(_ => nrand.NextDouble()).ToList();
        inputVarNames.Add(new string[] { });
        description.Add(" ~ N(0, 1)");
        lvl0.Add(datai);
      }

      // lvl1 contains variables which are functions of vars in lvl0 (+ noise)
      List<List<double>> lvl1 = new List<List<double>>();
      int numLvl1 = (int)Math.Ceiling(numberOfFeatures * 0.33);
      for (int c = 0; c < numLvl1; c++) {
        string[] selectedVarNames;
        var x = GenerateRandomFunction(random, lvl0, out selectedVarNames);
        var sigma = x.StandardDeviation();
        var noisePrng = new NormalDistributedRandom(random, 0, sigma * Math.Sqrt(noiseRatio / (1.0 - noiseRatio)));
        lvl1.Add(x.Select(t => t + noisePrng.NextDouble()).ToList());
        Array.Sort(selectedVarNames);
        inputVarNames.Add(selectedVarNames);
        var desc = string.Format("f({0})", string.Join(",", selectedVarNames));
        description.Add(string.Format(" ~ N({0}, {1:N3})", desc, noisePrng.Sigma));
      }

      // lvl2 contains variables which are functions of vars in lvl0 and lvl1 (+ noise)
      List<List<double>> lvl2 = new List<List<double>>();
      int numLvl2 = (int)Math.Ceiling(numberOfFeatures * 0.2);
      for (int c = 0; c < numLvl2; c++) {
        string[] selectedVarNames;
        var x = GenerateRandomFunction(random, lvl0.Concat(lvl1).ToList(), out selectedVarNames);
        var sigma = x.StandardDeviation();
        var noisePrng = new NormalDistributedRandom(random, 0, sigma * Math.Sqrt(noiseRatio / (1.0 - noiseRatio)));
        lvl2.Add(x.Select(t => t + noisePrng.NextDouble()).ToList());
        Array.Sort(selectedVarNames);
        inputVarNames.Add(selectedVarNames);
        var desc = string.Format("f({0})", string.Join(",", selectedVarNames));
        description.Add(string.Format(" ~ N({0}, {1:N3})", desc, noisePrng.Sigma));
      }

      // lvl3 contains variables which are functions of vars in lvl0, lvl1 and lvl2 (+ noise)
      List<List<double>> lvl3 = new List<List<double>>();
      int numLvl3 = numberOfFeatures - numLvl0 - numLvl1 - numLvl2;
      for (int c = 0; c < numLvl3; c++) {
        string[] selectedVarNames;
        var x = GenerateRandomFunction(random, lvl0.Concat(lvl1).Concat(lvl2).ToList(), out selectedVarNames);
        var sigma = x.StandardDeviation();
        var noisePrng = new NormalDistributedRandom(random, 0, sigma * Math.Sqrt(noiseRatio / (1.0 - noiseRatio)));
        lvl3.Add(x.Select(t => t + noisePrng.NextDouble()).ToList());
        Array.Sort(selectedVarNames);
        inputVarNames.Add(selectedVarNames);
        var desc = string.Format("f({0})", string.Join(",", selectedVarNames));
        description.Add(string.Format(" ~ N({0}, {1:N3})", desc, noisePrng.Sigma));
      }
      networkDefinition = string.Join(Environment.NewLine, variableNames.Zip(description, (n, d) => n + d).OrderBy(x => x));
      // for graphviz
      networkDefinition += Environment.NewLine + "digraph G {";
      foreach (var t in variableNames.Zip(inputVarNames, Tuple.Create).OrderBy(t => t.Item1)) {
        var name = t.Item1;
        var selectedVarNames = t.Item2;
        foreach (var selectedVarName in selectedVarNames) {
          networkDefinition += Environment.NewLine + selectedVarName + " -> " + name;
        }
      }
      networkDefinition += Environment.NewLine + "}";

      // return a random permutation of all variables
      var allVars = lvl0.Concat(lvl1).Concat(lvl2).Concat(lvl3).ToList();
      var orderedVars = allVars.Zip(variableNames, Tuple.Create).OrderBy(t => t.Item2).Select(t => t.Item1).ToList();
      variableNames = variableNames.OrderBy(n => n).ToArray();
      return orderedVars;
    }

    // sample the input variables that are actually used and sample from a Gaussian process
    private IEnumerable<double> GenerateRandomFunction(IRandom rand, List<List<double>> xs, out string[] selectedVarNames) {
      double r = -Math.Log(1.0 - rand.NextDouble()) * 2.0; // r is exponentially distributed with lambda = 2
      int nl = (int)Math.Floor(1.5 + r); // number of selected vars is likely to be between three and four
      if (nl > xs.Count) nl = xs.Count; // limit max

      var selectedIdx = Enumerable.Range(0, xs.Count).Shuffle(random)
        .Take(nl).ToArray();

      var selectedVars = selectedIdx.Select(i => xs[i]).ToArray();
      selectedVarNames = selectedIdx.Select(i => VariableNames[i]).ToArray();
      return SampleGaussianProcess(random, selectedVars);
    }

    private IEnumerable<double> SampleGaussianProcess(IRandom random, List<double>[] xs) {
      int nl = xs.Length;
      int nRows = xs.First().Count;
      double[,] K = new double[nRows, nRows];

      // sample length-scales
      var l = Enumerable.Range(0, nl)
        .Select(_ => random.NextDouble() * 2 + 0.5)
        .ToArray();
      // calculate covariance matrix
      for (int r = 0; r < nRows; r++) {
        double[] xi = xs.Select(x => x[r]).ToArray();
        for (int c = 0; c <= r; c++) {
          double[] xj = xs.Select(x => x[c]).ToArray();
          double dSqr = xi.Zip(xj, (xik, xjk) => (xik - xjk))
            .Select(dk => dk * dk)
            .Zip(l, (dk, lk) => dk / lk)
            .Sum();
          K[r, c] = Math.Exp(-dSqr);
        }
      }

      // add a small diagonal matrix for numeric stability
      for (int i = 0; i < nRows; i++) {
        K[i, i] += 1.0E-7;
      }

      // decompose
      alglib.trfac.spdmatrixcholesky(ref K, nRows, false);

      // sample u iid ~ N(0, 1)
      var u = Enumerable.Range(0, nRows).Select(_ => NormalDistributedRandom.NextDouble(random, 0, 1)).ToArray();

      // calc y = Lu
      var y = new double[u.Length];
      alglib.ablas.rmatrixmv(nRows, nRows, K, 0, 0, 0, u, 0, ref y, 0);

      return y;
    }
  }
}
