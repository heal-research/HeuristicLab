using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman68 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman68() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman68(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman68(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("II.13.34 rho_c_0*v/sqrt(1-v**2/c**2) | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "j" : "j_noise"; } }

    protected override string[] VariableNames {
      get { return noiseRatio == null ? new[] { "rho_c_0", "v", "c", "j" } : new[] { "rho_c_0", "v", "c", "j", "j_noise" }; }
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

      var j = new List<double>();

      data.Add(rho_c_0);
      data.Add(v);
      data.Add(c);
      data.Add(j);

      for (var i = 0; i < rho_c_0.Count; i++) {
        var res = rho_c_0[i] * v[i] / Math.Sqrt(1 - Math.Pow(v[i], 2) / Math.Pow(c[i], 2));
        j.Add(res);
      }

      var targetNoise = ValueGenerator.GenerateNoise(j, rand, noiseRatio);
      if (targetNoise != null) data.Add(targetNoise);

      return data;
    }
  }
}