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
using System.Globalization;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Problems.DataAnalysis;
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

      variableRelevances = new Dictionary<string, IEnumerable<KeyValuePair<string, double>>>();
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

    private Dictionary<string, IEnumerable<KeyValuePair<string, double>>> variableRelevances;
    public IEnumerable<KeyValuePair<string, double>> GetVariableRelevance(string targetVar) {
      return variableRelevances[targetVar];
    }

    protected override List<List<double>> GenerateValues() {
      // variable names are shuffled in the beginning (and sorted at the end)
      variableNames = variableNames.Shuffle(random).ToArray();

      // a third of all variables are independent vars
      List<List<double>> lvl0 = new List<List<double>>();
      int numLvl0 = (int)Math.Ceiling(numberOfFeatures * 0.33);

      List<string> description = new List<string>(); // store information how the variable is actually produced
      List<string[]> inputVarNames = new List<string[]>(); // store information to produce graphviz file
      List<double[]> relevances = new List<double[]>(); // stores variable relevance information (same order as given in inputVarNames)

      var nrand = new NormalDistributedRandom(random, 0, 1);
      for (int c = 0; c < numLvl0; c++) {
        inputVarNames.Add(new string[] { });
        relevances.Add(new double[] { });
        description.Add(" ~ N(0, 1)");
        lvl0.Add(Enumerable.Range(0, TestPartitionEnd).Select(_ => nrand.NextDouble()).ToList());
      }

      // lvl1 contains variables which are functions of vars in lvl0 (+ noise)
      int numLvl1 = (int)Math.Ceiling(numberOfFeatures * 0.33);
      List<List<double>> lvl1 = CreateVariables(lvl0, numLvl1, inputVarNames, description, relevances);

      // lvl2 contains variables which are functions of vars in lvl0 and lvl1 (+ noise)
      int numLvl2 = (int)Math.Ceiling(numberOfFeatures * 0.2);
      List<List<double>> lvl2 = CreateVariables(lvl0.Concat(lvl1).ToList(), numLvl2, inputVarNames, description, relevances);

      // lvl3 contains variables which are functions of vars in lvl0, lvl1 and lvl2 (+ noise)
      int numLvl3 = numberOfFeatures - numLvl0 - numLvl1 - numLvl2;
      List<List<double>> lvl3 = CreateVariables(lvl0.Concat(lvl1).Concat(lvl2).ToList(), numLvl3, inputVarNames, description, relevances);

      this.variableRelevances.Clear();
      for (int i = 0; i < variableNames.Length; i++) {
        var targetVarName = variableNames[i];
        var targetRelevantInputs =
          inputVarNames[i].Zip(relevances[i], (inputVar, rel) => new KeyValuePair<string, double>(inputVar, rel))
            .ToArray();
        variableRelevances.Add(targetVarName, targetRelevantInputs);
      }

      networkDefinition = string.Join(Environment.NewLine, variableNames.Zip(description, (n, d) => n + d).OrderBy(x => x));
      // for graphviz
      networkDefinition += Environment.NewLine + "digraph G {";
      for (int i = 0; i < variableNames.Length; i++) {
        var name = variableNames[i];
        var selectedVarNames = inputVarNames[i];
        var selectedRelevances = relevances[i];
        for (int j = 0; j < selectedVarNames.Length; j++) {
          var selectedVarName = selectedVarNames[j];
          var selectedRelevance = selectedRelevances[j];
          networkDefinition += Environment.NewLine + selectedVarName + " -> " + name +
            string.Format(CultureInfo.InvariantCulture, " [label={0:N3}]", selectedRelevance);
        }
      }
      networkDefinition += Environment.NewLine + "}";

      // return a random permutation of all variables (to mix lvl0, lvl1, ... variables)
      var allVars = lvl0.Concat(lvl1).Concat(lvl2).Concat(lvl3).ToList();
      var orderedVars = allVars.Zip(variableNames, Tuple.Create).OrderBy(t => t.Item2).Select(t => t.Item1).ToList();
      variableNames = variableNames.OrderBy(n => n).ToArray();
      return orderedVars;
    }

    private List<List<double>> CreateVariables(List<List<double>> allowedInputs, int numVars, List<string[]> inputVarNames, List<string> description, List<double[]> relevances) {
      var res = new List<List<double>>();
      for (int c = 0; c < numVars; c++) {
        string[] selectedVarNames;
        double[] relevance;
        var x = GenerateRandomFunction(random, allowedInputs, out selectedVarNames, out relevance);
        var sigma = x.StandardDeviation();
        var noisePrng = new NormalDistributedRandom(random, 0, sigma * Math.Sqrt(noiseRatio / (1.0 - noiseRatio)));
        res.Add(x.Select(t => t + noisePrng.NextDouble()).ToList());
        Array.Sort(selectedVarNames, relevance);
        inputVarNames.Add(selectedVarNames);
        relevances.Add(relevance);
        var desc = string.Format("f({0})", string.Join(",", selectedVarNames));
        // for the relevance information order variables by decreasing relevance
        var relevanceStr = string.Join(", ",
          selectedVarNames.Zip(relevance, Tuple.Create)
          .OrderByDescending(t => t.Item2)
          .Select(t => string.Format(CultureInfo.InvariantCulture, "{0}: {1:N3}", t.Item1, t.Item2)));
        description.Add(string.Format(" ~ N({0}, {1:N3}) [Relevances: {2}]", desc, noisePrng.Sigma, relevanceStr));
      }
      return res;
    }

    // sample the input variables that are actually used and sample from a Gaussian process
    private IEnumerable<double> GenerateRandomFunction(IRandom rand, List<List<double>> xs, out string[] selectedVarNames, out double[] relevance) {
      double r = -Math.Log(1.0 - rand.NextDouble()) * 2.0; // r is exponentially distributed with lambda = 2
      int nl = (int)Math.Floor(1.5 + r); // number of selected vars is likely to be between three and four
      if (nl > xs.Count) nl = xs.Count; // limit max

      var selectedIdx = Enumerable.Range(0, xs.Count).Shuffle(random)
        .Take(nl).ToArray();

      var selectedVars = selectedIdx.Select(i => xs[i]).ToArray();
      selectedVarNames = selectedIdx.Select(i => VariableNames[i]).ToArray();
      return SampleGaussianProcess(random, selectedVars, out relevance);
    }

    private IEnumerable<double> SampleGaussianProcess(IRandom random, List<double>[] xs, out double[] relevance) {
      int nl = xs.Length;
      int nRows = xs.First().Count;

      // sample u iid ~ N(0, 1)
      var u = Enumerable.Range(0, nRows).Select(_ => NormalDistributedRandom.NextDouble(random, 0, 1)).ToArray();

      // sample actual length-scales
      var l = Enumerable.Range(0, nl)
        .Select(_ => random.NextDouble() * 2 + 0.5)
        .ToArray();

      double[,] K = CalculateCovariance(xs, l);

      // decompose
      alglib.trfac.spdmatrixcholesky(ref K, nRows, false);


      // calc y = Lu
      var y = new double[u.Length];
      alglib.ablas.rmatrixmv(nRows, nRows, K, 0, 0, 0, u, 0, ref y, 0);

      // calculate relevance by removing dimensions
      relevance = CalculateRelevance(y, u, xs, l);


      // calculate variable relevance
      // as per Rasmussen and Williams "Gaussian Processes for Machine Learning" page 106:
      // ,,For the squared exponential covariance function [...] the l1, ..., lD hyperparameters
      // play the role of characteristic length scales [...]. Such a covariance function implements 
      // automatic relevance determination (ARD) [Neal, 1996], since the inverse of the length-scale 
      // determines how relevant an input is: if the length-scale has a very large value, the covariance 
      // will become almost independent of that input, effectively removing it from inference.''
      // relevance = l.Select(li => 1.0 / li).ToArray();

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
      for (int i = 0; i < l.Length; i++) {
        Array.Copy(l, changedL, changedL.Length);
        changedL[i] = double.MaxValue;
        var changedK = CalculateCovariance(xs, changedL);

        var yChanged = new double[u.Length];
        alglib.ablas.rmatrixmv(nRows, nRows, changedK, 0, 0, 0, u, 0, ref yChanged, 0);

        OnlineCalculatorError error;
        var mse = OnlineMeanSquaredErrorCalculator.Calculate(y, yChanged, out error);
        if (error != OnlineCalculatorError.None) mse = double.MaxValue;
        relevance[i] = mse;
      }
      // scale so that max relevance is 1.0
      var maxRel = relevance.Max();
      for (int i = 0; i < relevance.Length; i++) relevance[i] /= maxRel;
      return relevance;
    }

    private double[,] CalculateCovariance(List<double>[] xs, double[] l) {
      int nRows = xs.First().Count;
      double[,] K = new double[nRows, nRows];
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

      return K;
    }
  }
}
