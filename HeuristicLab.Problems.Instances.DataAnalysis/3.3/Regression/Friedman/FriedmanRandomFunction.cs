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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class FriedmanRandomFunction : ArtificialRegressionDataDescriptor {
    private readonly int nTrainingSamples;
    private readonly int nTestSamples;

    private readonly int numberOfFeatures;
    private readonly double noiseRatio;
    private readonly IRandom random;

    public override string Name { get { return string.Format("FriedmanRandomFunction-{0:0%} ({1} dim)", noiseRatio, numberOfFeatures); } }
    public override string Description {
      get {
        return "The data are generated using the random function generator described in 'Friedman: Greedy Function Approximation: A Gradient Boosting Machine, 1999'.";
      }
    }

    public FriedmanRandomFunction(int numberOfFeatures, double noiseRatio,
      IRandom rand)
      : this(500, 5000, numberOfFeatures, noiseRatio, rand) { }

    public FriedmanRandomFunction(int nTrainingSamples, int nTestSamples,
      int numberOfFeatures, double noiseRatio, IRandom rand) {
      this.nTrainingSamples = nTrainingSamples;
      this.nTestSamples = nTestSamples;
      this.noiseRatio = noiseRatio;
      this.random = rand;
      this.numberOfFeatures = numberOfFeatures;
    }

    protected override string TargetVariable { get { return "Y"; } }

    protected override string[] VariableNames {
      get { return AllowedInputVariables.Concat(new string[] { "Y" }).ToArray(); }
    }

    protected override string[] AllowedInputVariables {
      get {
        return Enumerable.Range(1, numberOfFeatures)
          .Select(i => string.Format("X{0:000}", i))
          .ToArray();
      }
    }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return nTrainingSamples; } }
    protected override int TestPartitionStart { get { return nTrainingSamples; } }
    protected override int TestPartitionEnd { get { return nTrainingSamples + nTestSamples; } }


    protected override List<List<double>> GenerateValues() {
      List<List<double>> data = new List<List<double>>();

      var nrand = new NormalDistributedRandom(random, 0, 1);
      for (int c = 0; c < numberOfFeatures; c++) {
        var datai = Enumerable.Range(0, TestPartitionEnd).Select(_ => nrand.NextDouble()).ToList();
        data.Add(datai);
      }
      var y = GenerateRandomFunction(random, data);

      var targetSigma = y.StandardDeviation();
      var noisePrng = new NormalDistributedRandom(random, 0, targetSigma * Math.Sqrt(noiseRatio / (1.0 - noiseRatio)));

      data.Add(y.Select(t => t + noisePrng.NextDouble()).ToList());

      return data;
    }

    // as described in Greedy Function Approximation paper
    private IEnumerable<double> GenerateRandomFunction(IRandom rand, List<List<double>> xs, int nTerms = 20) {
      int nRows = xs.First().Count;

      var gz = new List<double[]>();
      for (int i = 0; i < nTerms; i++) {

        // alpha ~ U(-1, 1)
        double alpha = rand.NextDouble() * 2 - 1;
        double r = -Math.Log(1.0 - rand.NextDouble()) * 2.0; // r is exponentially distributed with lambda = 2
        int nl = (int)Math.Floor(1.5 + r); // number of selected vars is likely to be between three and four


        var selectedVars = xs.Shuffle(random).Take(nl).ToArray();
        gz.Add(SampleRandomFunction(random, selectedVars)
          .Select(f => alpha * f)
          .ToArray());
      }
      // sum up
      return Enumerable.Range(0, nRows)
        .Select(r => gz.Sum(gzi => gzi[r]));
    }

    private IEnumerable<double> SampleRandomFunction(IRandom random, List<double>[] xs) {
      int nl = xs.Length;
      // mu is generated from same distribution as x
      double[] mu = Enumerable.Range(0, nl).Select(_ => random.NextDouble() * 2 - 1).ToArray();
      double[,] v = new double[nl, nl];
      var condNum = 4.0 / 0.01; // as given in the paper for max and min eigen values

      // temporarily use different random number generator in alglib
      var curRand = alglib.math.rndobject;
      alglib.math.rndobject = new System.Random(random.Next());

      alglib.matgen.spdmatrixrndcond(nl, condNum, ref v);
      // restore
      alglib.math.rndobject = curRand;

      int nRows = xs.First().Count;
      var z = new double[nl];
      var y = new double[nl];
      for (int i = 0; i < nRows; i++) {
        for (int j = 0; j < nl; j++) z[j] = xs[j][i] - mu[j];
        alglib.ablas.rmatrixmv(nl, nl, v, 0, 0, 0, z, 0, ref y, 0);

        // dot prod
        var s = 0.0;
        for (int j = 0; j < nl; j++) s += z[j] * y[j];

        yield return s;
      }
    }
  }
}
