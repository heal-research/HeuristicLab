using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class FeynmanBonus3 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public FeynmanBonus3() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public FeynmanBonus3(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public FeynmanBonus3(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format(
          "Compton Scattering: E_n/(1+E_n/(m*c**2)*(1-cos(theta))) | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "K" : "K_noise"; } }

    protected override string[] VariableNames {
      get { return noiseRatio == null ? new[] { "E_n", "m", "c", "theta", "K" } : new[] { "E_n", "m", "c", "theta", "K", "K_noise" }; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"E_n", "m", "c", "theta"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data  = new List<List<double>>();
      var E_n   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var m     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var c     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var theta = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();

      var K = new List<double>();

      data.Add(E_n);
      data.Add(m);
      data.Add(c);
      data.Add(theta);
      data.Add(K);

      for (var i = 0; i < E_n.Count; i++) {
        var res = E_n[i] / (1 + E_n[i] / (m[i] * Math.Pow(c[i], 2)) * (1 - Math.Cos(theta[i])));
        K.Add(res);
      }

      var targetNoise = GetNoisyTarget(K, rand);
      if (targetNoise != null) data.Add(targetNoise);

      return data;
    }
  }
}