using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman67 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman67() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman67(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman67(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("II.13.23 rho_c_0/sqrt(1-v**2/c**2) | {0} samples | {1}", trainingSamples,
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "rho_c" : "rho_c_noise"; } }

    protected override string[] VariableNames {
      get { return new[] {"rho_c_0", "v", "c", noiseRatio == null ? "rho_c" : "rho_c_noise"}; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"rho_c_0", "v", "c"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data    = new List<List<double>>();
      var rho_c_0 = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var v       = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 2).ToList();
      var c       = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 3, 10).ToList();

      var rho_c = new List<double>();

      data.Add(rho_c_0);
      data.Add(v);
      data.Add(c);
      data.Add(rho_c);

      for (var i = 0; i < rho_c_0.Count; i++) {
        var res = rho_c_0[i] / Math.Sqrt(1 - Math.Pow(v[i], 2) / Math.Pow(c[i], 2));
        rho_c.Add(res);
      }

      if (noiseRatio != null) {
        var rho_c_noise = new List<double>();
        var sigma_noise = (double) noiseRatio * rho_c.StandardDeviationPop();
        rho_c_noise.AddRange(rho_c.Select(md => md + NormalDistributedRandom.NextDouble(rand, 0, sigma_noise)));
        data.Remove(rho_c);
        data.Add(rho_c_noise);
      }

      return data;
    }
  }
}