using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman85 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman85() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman85(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman85(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("II.38.14 Y/(2*(1+sigma)) | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "mu_S" : "mu_S_noise"; } }

    protected override string[] VariableNames {
      get { return noiseRatio == null ? new[] { "Y", "sigma", "mu_S" } : new[] { "Y", "sigma", "mu_S", "mu_S_noise" }; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"Y", "sigma"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data  = new List<List<double>>();
      var Y     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var sigma = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var mu_S = new List<double>();

      data.Add(Y);
      data.Add(sigma);
      data.Add(mu_S);

      for (var i = 0; i < Y.Count; i++) {
        var res = Y[i] / (2 * (1 + sigma[i]));
        mu_S.Add(res);
      }

      var targetNoise = GetNoisyTarget(mu_S, rand);
      if (targetNoise != null) data.Add(targetNoise);

      return data;
    }
  }
}