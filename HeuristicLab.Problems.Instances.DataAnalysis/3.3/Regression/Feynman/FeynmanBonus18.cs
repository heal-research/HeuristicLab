using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class FeynmanBonus18 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public FeynmanBonus18() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public FeynmanBonus18(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public FeynmanBonus18(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("Weinberg 15.2.1: 3/(8*pi*G)*(c**2*k_f/r**2+H_G**2) | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "rho_0" : "rho_0_noise"; } }

    protected override string[] VariableNames {
      get { return noiseRatio == null ? new[] { "G", "k_f", "r", "H_G", "c", "rho_0" } : new[] { "G", "k_f", "r", "H_G", "c", "rho_0", "rho_0_noise" }; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"G", "k_f", "r", "H_G", "c"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data = new List<List<double>>();
      var G    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var k_f  = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var r    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var H_G  = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var c    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var rho_0 = new List<double>();

      data.Add(G);
      data.Add(k_f);
      data.Add(r);
      data.Add(H_G);
      data.Add(c);
      data.Add(rho_0);

      for (var i = 0; i < G.Count; i++) {
        var res = 3.0 / (8 * Math.PI * G[i]) * (Math.Pow(c[i], 2) * k_f[i] / Math.Pow(r[i], 2) + Math.Pow(H_G[i], 2));
        rho_0.Add(res);
      }

      var targetNoise = GetNoisyTarget(rho_0, rand);
      if (targetNoise != null) data.Add(targetNoise);

      return data;
    }
  }
}