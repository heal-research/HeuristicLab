using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class FeynmanBonus19 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public FeynmanBonus19() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public FeynmanBonus19(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public FeynmanBonus19(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format(
          "Weinberg 15.2.2: -1/(8*pi*G)*(c**4*k_f/r**2 + c**2*H_G**2*(1-2*alpha)) | {0} samples | {1}",
          trainingSamples, noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "pr" : "pr_noise"; } }

    protected override string[] VariableNames {
      get { return new[] {"G", "k_f", "r", "H_G", "alpha", "c", noiseRatio == null ? "pr" : "pr_noise"}; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"G", "k_f", "r", "H_G", "alpha", "c"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data  = new List<List<double>>();
      var G     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var k_f   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var r     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var H_G   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var alpha = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var c     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var pr = new List<double>();

      data.Add(G);
      data.Add(k_f);
      data.Add(r);
      data.Add(H_G);
      data.Add(alpha);
      data.Add(c);
      data.Add(pr);

      for (var i = 0; i < G.Count; i++) {
        var res = -1.0 / (8 * Math.PI * G[i]) * (Math.Pow(c[i], 4) * k_f[i] / Math.Pow(r[i], 2) +
                                                 Math.Pow(c[i], 2) * Math.Pow(H_G[i], 2) * (1 - 2 * alpha[i]));
        pr.Add(res);
      }

      if (noiseRatio != null) {
        var pr_noise    = new List<double>();
        var sigma_noise = (double) noiseRatio * pr.StandardDeviationPop();
        pr_noise.AddRange(pr.Select(md => md + NormalDistributedRandom.NextDouble(rand, 0, sigma_noise)));
        data.Remove(pr);
        data.Add(pr_noise);
      }

      return data;
    }
  }
}