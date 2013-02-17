#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public class FeatureSelection : ArtificialRegressionDataDescriptor {
    private int nTrainingSamples;
    private int nTestSamples;

    private int numberOfFeatures;
    private double selectionProbability;
    private double noiseRatio;
    private IRandom xRandom;
    private IRandom weightRandom;

    public override string Name { get { return string.Format("FeatSel-{0}-{1:0%}-{2:0%}", numberOfFeatures, selectionProbability, noiseRatio); } }
    public override string Description {
      get {
        return "This problem is specifically designed to test feature selection." + Environment.NewLine
               + "In this instance the number of rows for training (" + nTrainingSamples +
               ") is only slightly larger than the number of columns (" + numberOfFeatures +
               ") and only a subset of the columns must be selected for the predictive model." + Environment.NewLine
               + "The target variable is calculated as a noisy linear combination of randomly selected features: y = w * S + n." + Environment.NewLine
               + "Where is the S is a N x d matrix containing the selected columns from N x k the matrix of all features X" + Environment.NewLine
               + "For each feature the probability that it is selected is " + selectionProbability + "%" + Environment.NewLine
               + "X(i,j) ~ N(0, 1) iid, w(i) ~ U(0, 10) iid, n ~ N(0, sigma(w*S) * SQRT(" + noiseRatio + "))" + Environment.NewLine
               + "The noise level is " + noiseRatio + " * sigma, thus an optimal model has R² = "
               + Math.Round(optimalRSquared) + " (or equivalently: NMSE = " + noiseRatio + ")" + Environment.NewLine
               + "N = " + (nTrainingSamples + nTestSamples) + " (" + nTrainingSamples + " training, " + nTestSamples + " test)" + Environment.NewLine
               + "k = " + numberOfFeatures;
        ;
      }
    }

    private double[] w;
    public double[] Weights {
      get { return w; }
    }

    private string[] selectedFeatures;
    public string[] SelectedFeatures {
      get { return selectedFeatures; }
    }

    private double optimalRSquared;
    public double OptimalRSquared {
      get { return optimalRSquared; }
    }


    public FeatureSelection(int numberOfFeatures, double selectionProbability, double noiseRatio, IRandom xGenerator, IRandom weightGenerator)
      : this((int)Math.Round(numberOfFeatures * 1.2), 5000, numberOfFeatures,
      selectionProbability, noiseRatio, xGenerator, weightGenerator) { }

    public FeatureSelection(int nTrainingSamples, int nTestSamples, int numberOfFeatures,
      double selectionProbability, double noiseRatio, IRandom xGenerator, IRandom weightGenerator) {
      this.numberOfFeatures = numberOfFeatures;
      this.nTrainingSamples = nTrainingSamples;
      this.nTestSamples = nTestSamples;
      this.selectionProbability = selectionProbability;
      this.noiseRatio = noiseRatio;
      this.xRandom = xGenerator;
      this.weightRandom = weightGenerator;
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
      for (int i = 0; i < AllowedInputVariables.Count(); i++) {
        data.Add(Enumerable.Range(0, TestPartitionEnd)
          .Select(_ => xRandom.NextDouble())
          .ToList());
      }

      var random = new MersenneTwister();
      var selectedFeatures =
        Enumerable.Range(0, AllowedInputVariables.Count())
        .Where(_ => random.NextDouble() < selectionProbability)
        .ToArray();

      w = selectedFeatures.Select(_ => weightRandom.NextDouble()).ToArray();
      var target = new List<double>();
      for (int i = 0; i < data[0].Count; i++) {
        var s = selectedFeatures
          .Select(index => data[index][i])
          .ToArray();
        target.Add(ScalarProd(s, w));
      }
      var targetSigma = target.StandardDeviation();
      var noisePrng = new NormalDistributedRandom(random, 0, targetSigma * Math.Sqrt(noiseRatio));

      data.Add(target.Select(t => t + noisePrng.NextDouble()).ToList());

      // set property listing the selected features as string[]
      this.selectedFeatures = selectedFeatures.Select(i => AllowedInputVariables[i]).ToArray();
      optimalRSquared = 1 - noiseRatio;
      return data;
    }

    private double ScalarProd(double[] s, double[] w) {
      if (s.Length != w.Length) throw new ArgumentException();
      return s.Zip(w, (a, b) => a * b).Sum();
    }
  }
}
