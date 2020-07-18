using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman77 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman77() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman77(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman77(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("II.34.11 g_*q*B/(2*m) | {0} samples | {1}", trainingSamples,
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "omega" : "omega_noise"; } }

    protected override string[] VariableNames {
      get { return new[] {"g_", "q", "B", "m", noiseRatio == null ? "omega" : "omega_noise"}; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"g_", "q", "B", "m"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data = new List<List<double>>();
      var g_   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var q    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var B    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var m    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var omega = new List<double>();

      data.Add(g_);
      data.Add(q);
      data.Add(B);
      data.Add(m);
      data.Add(omega);

      for (var i = 0; i < g_.Count; i++) {
        var res = g_[i] * q[i] * B[i] / (2 * m[i]);
        omega.Add(res);
      }

      if (noiseRatio != null) {
        var omega_noise = new List<double>();
        var sigma_noise = (double) noiseRatio * omega.StandardDeviationPop();
        omega_noise.AddRange(omega.Select(md => md + NormalDistributedRandom.NextDouble(rand, 0, sigma_noise)));
        data.Remove(omega);
        data.Add(omega_noise);
      }

      return data;
    }
  }
}